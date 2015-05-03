#region License information (SeeingSharp and all based games/applications)
/*
    SeeingSharp and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2015 Roland König (RolandK)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Util;
using SeeingSharp.Checking;
using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Namespace mappings
using MF = SharpDX.MediaFoundation;

namespace SeeingSharp.Multimedia.DrawingVideo
{
    public class AsyncRealtimeVideoReader : MediaFoundationVideoReader
    {
        #region Video processing resources
        private Task m_processingTask;
        private CancellationTokenSource m_processingCancelSource;
        private SeeingSharpMediaBuffer m_currentBuffer;
        private object m_currentBufferLock;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRealtimeVideoReader"/> class.
        /// </summary>
        /// <param name="videoSource">The source of the video file.</param>
        /// <param name="immediateStart">True to start video reading immediately.</param>
        public AsyncRealtimeVideoReader(ResourceLink videoSource, bool immediateStart = true)
            : base(videoSource)
        {
            // Start immediately if requested
            m_currentBufferLock = new object();
            m_currentBuffer = null;

            if (immediateStart) { this.Start(); }
        }

        /// <summary>
        /// Start video reading.
        /// </summary>
        public void Start()
        {
            if (this.IsStarted) { throw new SeeingSharpGraphicsException("Unable to start video reading more than one time!"); }

            m_processingCancelSource = new CancellationTokenSource();
            m_processingTask = this.PerformVideoReadingAsync(m_processingCancelSource.Token);
        }

        /// <summary>
        /// Gets the current video frame.
        /// Null is returned if there is still no frame available.
        /// </summary>
        public SeeingSharpMediaBuffer GetCurrentFrame()
        {
            lock(m_currentBufferLock)
            {
                if (m_currentBuffer != null) { return m_currentBuffer.CopyPointer(); }
                else { return null; }
            }
        }

        /// <summary>
        /// Performs asynchronous video reading.
        /// </summary>
        /// <param name="cancelToken">The cancel token.</param>
        private async Task PerformVideoReadingAsync(CancellationToken cancelToken)
        {
            // Ensure that we are working on the ThreadPool
            //  => Thread.CurrentThread.IsThreadPoolThread not possible on WinRT platform
            await Task.Delay(100).ConfigureAwait(false);

            bool doContinue = true;
            while (doContinue && (!cancelToken.IsCancellationRequested))
            {
                // Read next frame
                bool currentBufferChanged = false;
                lock (m_currentBufferLock)
                {
                    SeeingSharpMediaBuffer mediaBuffer = this.ReadFrameInternal();
                    if(mediaBuffer != null)
                    {
                        GraphicsHelper.SafeDispose(ref m_currentBuffer);
                        m_currentBuffer = mediaBuffer;
                        currentBufferChanged = true;
                    }
                }

                // Wait some time because we could no read the last frame
                if(currentBufferChanged)
                {
                    await Task.Delay(200);
                }
            }
        }

        public override void Dispose()
        {
            m_processingCancelSource.Cancel();
            m_processingCancelSource = null;
            m_processingTask = null;

            base.Dispose();
        }

        /// <summary>
        /// Did we start this reader previously?
        /// </summary>
        public bool IsStarted
        {
            get { return m_processingTask != null; }
        }
    }
}
