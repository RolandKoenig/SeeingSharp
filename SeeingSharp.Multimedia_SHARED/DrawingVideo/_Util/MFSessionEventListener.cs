#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
	Exception are projects where it is noted otherwhise.
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
#if DESKTOP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// Namespace mappings
using SDX = SharpDX;
using MF = SharpDX.MediaFoundation;

namespace SeeingSharp.Multimedia.DrawingVideo
{
    class MFSessionEventListener : MF.IAsyncCallback
    {
        #region The captured synchronization context
        private SynchronizationContext m_syncContext;
        #endregion

        #region Helper for WaitForEventAsync method
        private List<Func<MF.MediaEvent, bool>> m_asyncEventWaiters;
        private object m_asyncEventWaitersLock;
        #endregion

        #region The session this event listener listens to
        private MF.MediaSession m_session;
        #endregion

        /// <summary>
        /// Raises when the end of a video is reached.
        /// </summary>
        public event EventHandler EndOfPresentation;

        /// <summary>
        /// Initializes a new instance of the <see cref="MFSessionEventListener"/> class.
        /// </summary>
        /// <param name="session">The session to listen to.</param>
        private MFSessionEventListener(MF.MediaSession session)
        {
            m_syncContext = SynchronizationContext.Current;
            m_session = session;

            m_asyncEventWaiters = new List<Func<MF.MediaEvent, bool>>();
            m_asyncEventWaitersLock = new object();

            // Start listening on the session object
            m_session.BeginGetEvent(this, null);
        }

        /// <summary>
        /// Attaches this listener on the given session.
        /// </summary>
        /// <param name="session">The session object to attach to.</param>
        internal static MFSessionEventListener AttachTo(MF.MediaSession session)
        {
            return new MFSessionEventListener(session);
        }

        /// <summary>
        /// This method waits for an asynchronous event message from the MF framework.
        /// </summary>
        /// <param name="eventType">The type of the event to wait for.</param>
        /// <param name="cancelToken">The cancellation token.</param>
        internal Task<MF.MediaEvent> WaitForEventAsync(
            MF.MediaEventTypes eventType,
            CancellationToken cancelToken)
        {
            return WaitForEventAsync(eventType, null, cancelToken);
        }

        /// <summary>
        /// This method waits for an asynchronous event message from the MF framework.
        /// </summary>
        /// <param name="eventType">The type of the event to wait for.</param>
        /// <param name="specificEventCheckingFunc">A specific event checking func implemented by the caller. Returns false if the event is not handled.</param>
        /// <param name="cancelToken">The cancellation token.</param>
        internal Task<MF.MediaEvent> WaitForEventAsync(
            MF.MediaEventTypes eventType,
            Func<MF.MediaEvent, bool> specificEventCheckingFunc,
            CancellationToken cancelToken)
        {
            TaskCompletionSource<MF.MediaEvent> eventWaitTaskResult = new TaskCompletionSource<MF.MediaEvent>();

            // Define the wait action
            Func<MF.MediaEvent, bool> waitFunc = (givenEventData) =>
            {
                // Handle cancellation
                if (cancelToken.IsCancellationRequested)
                {
                    eventWaitTaskResult.TrySetCanceled();
                    return true;
                }

                // Is this event that one we are waiting for?
                if (givenEventData.TypeInfo != eventType)
                {
                    return false;
                }

                // Handle result of the given event
                if (givenEventData.Status != SharpDX.Result.Ok)
                {
                    eventWaitTaskResult.TrySetException(new SDX.SharpDXException(givenEventData.Status));
                }
                else
                {
                    // If given, then execute special event checking
                    if (specificEventCheckingFunc != null)
                    {
                        if (!specificEventCheckingFunc(givenEventData)) { return false; }
                    }

                    // Finish task 
                    eventWaitTaskResult.TrySetResult(givenEventData);
                }
                return true;
            };

            // Register waiter
            lock (m_asyncEventWaitersLock)
            {
                m_asyncEventWaiters.Add(waitFunc);
            }

            return eventWaitTaskResult.Task;
        }

        /// <summary>
        /// Führt anwendungsspezifische Aufgaben aus, die mit dem Freigeben, Zurückgeben oder Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
        /// </summary>
        public void Dispose()
        {
            if (this.Shadow != null)
            {
                this.Shadow.Dispose();
                this.Shadow = null;
            }
        }

        public void Invoke(MF.AsyncResult asyncResultRef)
        {
            MF.MediaEventTypes eventType;

            try
            {
                using (MF.MediaEvent mediaEvent = m_session.EndGetEvent(asyncResultRef))
                {
                    // Look for existing awaitor action and handle it if there is one
                    lock (m_asyncEventWaitersLock)
                    {
                        for (int loop = 0; loop < m_asyncEventWaiters.Count; loop++)
                        {
                            if (m_asyncEventWaiters[loop](mediaEvent))
                            {
                                m_asyncEventWaiters.RemoveAt(loop);
                                break;
                            }
                        }
                    }

                    // Some generic event handling
                    eventType = mediaEvent.TypeInfo;
                    switch (eventType)
                    {
                        case MF.MediaEventTypes.EndOfPresentation:
                            m_syncContext.Post((arg) =>
                            {
                                if (EndOfPresentation != null) { EndOfPresentation(this, EventArgs.Empty); }
                            }, null);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Dispatch exception to UI thread
                m_syncContext.Post((arg) =>
                {
                    throw new SeeingSharpException("Exception curing MediaFoundation session event processing", ex);
                }, null);
            }

            // Reregister for next event
            m_session.BeginGetEvent(this, null);
        }

        /// <summary>
        /// Gets the identifier of the work queue on which the callback is dispatched. See remarks.
        /// </summary>
        /// <value>
        /// The work queue identifier.
        /// </value>
        /// <msdn-id>bb970381</msdn-id>
        ///   <unmanaged>HRESULT IMFAsyncCallback::GetParameters([Out] MFASYNC_CALLBACK_FLAGS* pdwFlags,[Out] unsigned int* pdwQueue)</unmanaged>
        ///   <unmanaged-short>IMFAsyncCallback::GetParameters</unmanaged-short>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <remarks>
        ///   <p>This value can specify one of the standard Media Foundation work queues, or a work queue created by the application. For list of standard Media Foundation work queues, see <strong>Work Queue Identifiers</strong>. To create a new work queue, call <strong><see cref="M:SharpDX.MediaFoundation.MediaFactory.AllocateWorkQueue(System.Int32@)" /></strong>. The default value is <strong><see cref="F:SharpDX.MediaFoundation.WorkQueueType.Standard" /></strong>.</p> <p>If the work queue is not compatible with the value returned in <em>pdwFlags</em>, the Media Foundation platform returns <strong><see cref="F:SharpDX.MediaFoundation.ResultCode.InvalidWorkqueue" /></strong> when it tries to dispatch the callback. (See <strong><see cref="M:SharpDX.MediaFoundation.MediaFactory.PutWorkItem(System.Int32,System.IntPtr,SharpDX.ComObject)" /></strong>.)</p>
        /// </remarks>
        public MF.WorkQueueId WorkQueueId
        {
            get { return MF.WorkQueueId.Standard; }
        }

        /// <summary>
        /// Gets or sets the unmanaged shadow callback.
        /// </summary>
        /// <value>
        /// The unmanaged shadow callback.
        /// </value>
        /// <remarks>
        /// This property is set whenever this instance has an unmanaged shadow callback
        /// registered. This callback must be disposed when disposing this instance.
        /// </remarks>
        public IDisposable Shadow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a flag indicating the behavior of the callback object's <strong><see cref="M:SharpDX.MediaFoundation.IAsyncCallback.Invoke(SharpDX.MediaFoundation.AsyncResult)" /></strong> method. Default behavior should be <see cref="F:SharpDX.MediaFoundation.AsyncCallbackFlags.None" />.
        /// </summary>
        /// <value>
        /// The a flag indicating the behavior of the callback object's <strong><see cref="M:SharpDX.MediaFoundation.IAsyncCallback.Invoke(SharpDX.MediaFoundation.AsyncResult)" /></strong> method.
        /// </value>
        /// <msdn-id>bb970381</msdn-id>
        ///   <unmanaged>HRESULT IMFAsyncCallback::GetParameters([Out] MFASYNC_CALLBACK_FLAGS* pdwFlags,[Out] unsigned int* pdwQueue)</unmanaged>
        ///   <unmanaged-short>IMFAsyncCallback::GetParameters</unmanaged-short>
        /// <exception cref="System.NotImplementedException"></exception>
        public MF.AsyncCallbackFlags Flags
        {
            get { return MF.AsyncCallbackFlags.None; }
        }
    }
}
#endif