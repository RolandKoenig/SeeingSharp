#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it.
    More info at
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
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
#endregion License information (SeeingSharp and all based games/applications)

#if DESKTOP

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.DrawingVideo;
using SeeingSharp.Util;
using MF = SharpDX.MediaFoundation;

// Namespace mappings
using SDX = SharpDX;

namespace SeeingSharp.Multimedia.Views
{
    public class MediaPlayerComponent : Component, IDisposable
    {
        private const string CATEGORY_NAME = "Video Player";

        #region Own properties
        private MediaPlayerState m_state;
        private Control m_targetControl;
        private ResourceLink m_currentVideoLink;
        private CaptureDeviceInfo m_currentCaptureDevice;
        private TimeSpan m_currentVideoDuration;
        private bool m_isPaused;
        #endregion Own properties

        #region References to Media Foundation
        private Stream m_videoSourceStreamNet;
        private MF.ByteStream m_videoSourceStream;
        private MF.MediaSession m_mediaSession;
        private MF.VideoDisplayControl m_displayControl;
        private MFSessionEventListener m_sessionEventHandler;
        #endregion References to Media Foundation

        /// <summary>
        /// Raised when the current state has changed.
        /// </summary>
        [Category(CATEGORY_NAME)]
        public event EventHandler StateChanged;

        /// <summary>
        /// Raised when the paused flag has changed.
        /// </summary>
        [Category(CATEGORY_NAME)]
        public event EventHandler IsPausedChanged;

        [Category(CATEGORY_NAME)]
        public event EventHandler VideoFinished;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlayerComponent"/> class.
        /// </summary>
        public MediaPlayerComponent()
        {
            m_currentVideoLink = null;
            m_currentCaptureDevice = null;
        }

        /// <summary>
        /// Pauses the video currently playing.
        /// </summary>
        public async Task PauseVideoAsync()
        {
            // Check for correct state
            if (this.State != MediaPlayerState.Playing)
            {
                throw new InvalidOperationException("Unable to pause video as long as there is no video playing!");
            }

            // Video already paused?
            if (m_isPaused) { return; }

            // Pause the video
            Task waitTask = m_sessionEventHandler.WaitForEventAsync(MF.MediaEventTypes.SessionPaused, CancellationToken.None);
            m_mediaSession.Pause();
            await waitTask;

            // Updates current paused state
            this.IsPaused = true;
        }

        /// <summary>
        /// Resumes the video from current position.
        /// </summary>
        public async Task ResumeVideoAsync()
        {
            // Check for correct state
            if (this.State != MediaPlayerState.Playing)
            {
                throw new InvalidOperationException("Unable to resume video as long as there is no video playing!");
            }

            // Video is not paused?
            if (!m_isPaused) { return; }

            // Resume the video from current location
            await StartSessionInternalAsync(false);

            // Updates current paused state
            this.IsPaused = false;
        }

        public async Task ShowCaptureDeviceAsync(CaptureDeviceInfo captureDevice)
        {
            // Check for correct state
            if (this.State != MediaPlayerState.NothingToDo)
            {
                throw new InvalidOperationException("Unable to open video file as long as there is another video playing!");
            }

            // Apply new state
            this.State = MediaPlayerState.Opening;

            try
            {
                // Create media session and a corresponding event listener obect for async events
                MF.MediaFactory.CreateMediaSession(null, out m_mediaSession);
                m_sessionEventHandler = MFSessionEventListener.AttachTo(m_mediaSession);
                m_sessionEventHandler.EndOfPresentation += OnSessionEventHandlerEndOfPresentationReached;

                // Create the media source
                using (MF.MediaSource mediaSource = captureDevice.CreateMediaSource())
                {
                    // Show the video
                    await ShowVideoAsync(mediaSource);
                }

                // Video opened successfully
                m_currentVideoLink = null;
                m_currentCaptureDevice = captureDevice;
                this.State = MediaPlayerState.Playing;
            }
            catch (Exception)
            {
                // Unload all resources in case of an exception
                DisposeResources();

                throw;
            }
        }

        /// <summary>
        /// Opens the given video file and plays it directly.
        /// </summary>
        /// <param name="videoLink">The link to the video file.</param>
        public async Task OpenAndShowVideoFileAsync(ResourceLink videoLink)
        {
            // Check for correct state
            if (this.State != MediaPlayerState.NothingToDo)
            {
                throw new InvalidOperationException("Unable to open video file as long as there is another video playing!");
            }

            // Apply new state
            this.State = MediaPlayerState.Opening;

            try
            {
                // Create media session and a corresponding event listener obect for async events
                MF.MediaFactory.CreateMediaSession(null, out m_mediaSession);
                m_sessionEventHandler = MFSessionEventListener.AttachTo(m_mediaSession);
                m_sessionEventHandler.EndOfPresentation += OnSessionEventHandlerEndOfPresentationReached;

                // Create source object
                MF.SourceResolver sourceResolver = new MF.SourceResolver();
                MF.ObjectType objType = MF.ObjectType.Invalid;
                m_videoSourceStreamNet = videoLink.OpenInputStream();
                m_videoSourceStream = new MF.ByteStream(m_videoSourceStreamNet);
                SharpDX.ComObject objSource = sourceResolver.CreateObjectFromStream(
                    m_videoSourceStream,
                    "Dummy." + videoLink.FileExtension,
                    MF.SourceResolverFlags.MediaSource,
                    out objType);
                using (MF.MediaSource mediaSource = objSource.QueryInterface<MF.MediaSource>())
                {
                    GraphicsHelper.SafeDispose(ref objSource);
                    GraphicsHelper.SafeDispose(ref sourceResolver);

                    await ShowVideoAsync(mediaSource);
                }

                // Video opened successfully
                m_currentVideoLink = videoLink;
                m_currentCaptureDevice = null;
                this.State = MediaPlayerState.Playing;
            }
            catch (Exception)
            {
                // Unload all resources in case of an exception
                DisposeResources();

                throw;
            }
        }

        private async Task ShowVideoAsync(MF.MediaSource mediaSource)
        {
            // Create topology
            MF.Topology topology;
            MF.PresentationDescriptor presentationDescriptor;
            MF.MediaFactory.CreateTopology(out topology);
            mediaSource.CreatePresentationDescriptor(out presentationDescriptor);
            int streamDescriptorCount = presentationDescriptor.StreamDescriptorCount;

            bool containsVideoStream = false;
            for (int loop = 0; loop < streamDescriptorCount; loop++)
            {
                SDX.Bool selected = false;
                MF.StreamDescriptor streamDescriptor;
                presentationDescriptor.GetStreamDescriptorByIndex(loop, out selected, out streamDescriptor);
                if (selected)
                {
                    // Create source node
                    MF.TopologyNode sourceNode = null;
                    MF.MediaFactory.CreateTopologyNode(MF.TopologyType.SourceStreamNode, out sourceNode);
                    sourceNode.Set(MF.TopologyNodeAttributeKeys.Source, mediaSource);
                    sourceNode.Set(MF.TopologyNodeAttributeKeys.PresentationDescriptor, presentationDescriptor);
                    sourceNode.Set(MF.TopologyNodeAttributeKeys.StreamDescriptor, streamDescriptor);

                    // Create output node
                    MF.TopologyNode outputNode = null;
                    MF.MediaTypeHandler mediaTypeHandler = streamDescriptor.MediaTypeHandler;
                    Guid majorType = mediaTypeHandler.MajorType;
                    MF.MediaFactory.CreateTopologyNode(MF.TopologyType.OutputNode, out outputNode);
                    if (MF.MediaTypeGuids.Audio == majorType)
                    {
                        MF.Activate audioRenderer;
                        MF.MediaFactory.CreateAudioRendererActivate(out audioRenderer);
                        outputNode.Object = audioRenderer;
                        GraphicsHelper.SafeDispose(ref audioRenderer);
                    }
                    else if (MF.MediaTypeGuids.Video == majorType)
                    {
                        if (m_targetControl == null)
                        {
                            throw new SeeingSharpException("Unable to display vido when MediaPlayerComponent is not bound to a target control!");
                        }

                        containsVideoStream = true;
                        MF.Activate videoRenderer;
                        MF.MediaFactory.CreateVideoRendererActivate(
                            m_targetControl.Handle,
                            out videoRenderer);
                        outputNode.Object = videoRenderer;
                        GraphicsHelper.SafeDispose(ref videoRenderer);
                    }

                    // Append nodes to topology
                    topology.AddNode(sourceNode);
                    topology.AddNode(outputNode);
                    sourceNode.ConnectOutput(0, outputNode, 0);

                    // Clear COM references
                    GraphicsHelper.SafeDispose(ref sourceNode);
                    GraphicsHelper.SafeDispose(ref outputNode);
                    GraphicsHelper.SafeDispose(ref mediaTypeHandler);
                }

                // Clear COM references
                GraphicsHelper.SafeDispose(ref streamDescriptor);
            }

            // Get the total duration of the video
            long durationLong = 0;
            try
            {
                durationLong = presentationDescriptor.Get<long>(MF.PresentationDescriptionAttributeKeys.Duration);
                m_currentVideoDuration = TimeSpan.FromTicks(durationLong);
            }
            catch (SharpDX.SharpDXException)
            {
                m_currentVideoDuration = TimeSpan.MaxValue;
            }

            // Dispose reference to the presentation descriptor
            GraphicsHelper.SafeDispose(ref presentationDescriptor);

            // Apply build topology to the session
            Task<MF.MediaEvent> topologyReadyWaiter = m_sessionEventHandler.WaitForEventAsync(
                MF.MediaEventTypes.SessionTopologyStatus,
                (eventData) => eventData.Get<MF.TopologyStatus>(MF.EventAttributeKeys.TopologyStatus) == MF.TopologyStatus.Ready,
                CancellationToken.None);
            m_mediaSession.SetTopology(MF.SessionSetTopologyFlags.None, topology);
            await topologyReadyWaiter;

            // Clear reference to the topology
            GraphicsHelper.SafeDispose(ref topology);

            // Query for display control service
            if (containsVideoStream)
            {
                using (MF.ServiceProvider serviceProvider = m_mediaSession.QueryInterface<MF.ServiceProvider>())
                {
                    m_displayControl = serviceProvider.GetService<MF.VideoDisplayControl>(
                        new Guid("{0x1092a86c, 0xab1a, 0x459a,{0xa3, 0x36, 0x83, 0x1f, 0xbc, 0x4d, 0x11, 0xff}}"));
                }
            }

            // Start playing the video
            await StartSessionInternalAsync(true);
        }

        /// <summary>
        /// Starts playing the video.
        /// </summary>
        private async Task StartSessionInternalAsync(bool restart)
        {
            SharpDX.Win32.Variant startTimeStamp;

            // Build play parameters
            if (restart)
            {
                startTimeStamp = new SharpDX.Win32.Variant();
                startTimeStamp.Type = SharpDX.Win32.VariantType.Default;
                startTimeStamp.ElementType = SharpDX.Win32.VariantElementType.Long;
                startTimeStamp.Value = (long)0;
            }
            else
            {
                startTimeStamp = new SharpDX.Win32.Variant();
                startTimeStamp.Type = SharpDX.Win32.VariantType.Default;
                startTimeStamp.ElementType = SharpDX.Win32.VariantElementType.Empty;
            }

            // Start playing the video
            Task waitTask = m_sessionEventHandler.WaitForEventAsync(MF.MediaEventTypes.SessionStarted, CancellationToken.None);
            m_mediaSession.Start(null, startTimeStamp);
            await waitTask;
        }

        /// <summary>
        /// Closes the current video.
        /// This method returns immediately if there is no video playing.
        /// </summary>
        public async Task CloseVideoAsync()
        {
            // Handle current state
            if (this.State != MediaPlayerState.Playing) { return; }
            this.State = MediaPlayerState.Closing;

            try
            {
                // Call close and wait for result
                Task<MF.MediaEvent> closeSessionTask = m_sessionEventHandler.WaitForEventAsync(
                    MF.MediaEventTypes.SessionClosed,
                    CancellationToken.None);
                m_mediaSession.Close();
                await closeSessionTask;
            }
            finally
            {
                // Disposes all resources
                this.DisposeResources();
            }

            //VideoFinished.Raise(this, EventArgs.Empty);
        }

        /// <summary>
        /// Current session reports finished video.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void OnSessionEventHandlerEndOfPresentationReached(object sender, EventArgs e)
        {
            if (sender != m_sessionEventHandler) { return; }

            if (this.RestartVideoWhenFinished)
            {
                // Start the session again
                await StartSessionInternalAsync(true);
            }
            else
            {
                VideoFinished.Raise(this, EventArgs.Empty);

                // Close current video
                await CloseVideoAsync();
            }
        }

        /// <summary>
        /// Called when the target control changes its size.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnTargetControlResize(object sender, EventArgs e)
        {
            if ((m_displayControl != null) &&
                (m_targetControl != null))
            {
                m_displayControl.SetVideoPosition(
                    null,
                    new SDX.Rectangle(0, 0, m_targetControl.Width, m_targetControl.Height));
            }
        }

        /// <summary>
        /// Called when the target control requested painting.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PaintEventArgs"/> instance containing the event data.</param>
        private void OnTargetControlPaint(object sender, PaintEventArgs e)
        {
            if ((m_displayControl != null) &&
                (m_targetControl != null))
            {
                m_displayControl.RepaintVideo();
            }
        }

        /// <summary>
        /// Disposes all current resources.
        /// </summary>
        private void DisposeResources()
        {
            // Shutdown current session
            if (m_mediaSession != null)
            {
                m_mediaSession.Shutdown();
            }

            // Clear all references
            m_currentVideoLink = null;
            m_currentCaptureDevice = null;
            m_currentVideoDuration = TimeSpan.Zero;
            GraphicsHelper.SafeDispose(ref m_displayControl);
            GraphicsHelper.SafeDispose(ref m_mediaSession);
            m_sessionEventHandler = null;

            GraphicsHelper.SafeDispose(ref m_videoSourceStream);
            GraphicsHelper.SafeDispose(ref m_videoSourceStreamNet);

            // Apply new state
            this.IsPaused = false;
            this.State = MediaPlayerState.NothingToDo;
        }

        /// <summary>
        /// Gets the state of the media player.
        /// </summary>
        [Browsable(false)]
        public MediaPlayerState State
        {
            get { return m_state; }
            private set
            {
                if (m_state != value)
                {
                    m_state = value;
                    if (StateChanged != null) { StateChanged(this, EventArgs.Empty); }
                }
            }
        }

        /// <summary>
        /// Gets a reference to the current displayed video.
        /// </summary>
        [Browsable(false)]
        public ResourceLink CurrentVideoFile
        {
            get { return m_currentVideoLink; }
        }

        /// <summary>
        /// Gets a reference to the current CaptureDevice.
        /// </summary>
        [Browsable(false)]
        public CaptureDeviceInfo CurrentCaptureDevice
        {
            get { return m_currentCaptureDevice; }
        }

        /// <summary>
        /// Gets the total duration of the currently displayed video.
        /// </summary>
        [Browsable(false)]
        public TimeSpan CurrentVideoDuration
        {
            get { return m_currentVideoDuration; }
        }

        /// <summary>
        /// Restart the video when playing has finished?
        /// </summary>
        [Category(CATEGORY_NAME)]
        public bool RestartVideoWhenFinished
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the target control which is used for displaying the video.
        /// </summary>
        [Category(CATEGORY_NAME)]
        public Control TargetControl
        {
            get { return m_targetControl; }
            set
            {
                // Check current state
                if (this.State != MediaPlayerState.NothingToDo)
                {
                    throw new InvalidOperationException("Unable to change TargetControl property as long there is a video playing!");
                }

                // Switch the target control
                if (m_targetControl != value)
                {
                    if (m_targetControl != null)
                    {
                        m_targetControl.Resize -= OnTargetControlResize;
                        m_targetControl.Paint -= OnTargetControlPaint;
                    }
                    m_targetControl = value;
                    if (m_targetControl != null)
                    {
                        m_targetControl.Resize += OnTargetControlResize;
                        m_targetControl.Paint += OnTargetControlPaint;
                    }
                }
            }
        }

        /// <summary>
        /// Is the video paused currently?
        /// </summary>
        [Browsable(false)]
        public bool IsPaused
        {
            get { return m_isPaused; }
            private set
            {
                if (m_isPaused != value)
                {
                    m_isPaused = value;
                    if (IsPausedChanged != null) { IsPausedChanged(this, EventArgs.Empty); }
                }
            }
        }
    }
}

#endif