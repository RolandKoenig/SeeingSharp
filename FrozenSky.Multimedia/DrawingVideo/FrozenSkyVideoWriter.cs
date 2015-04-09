﻿#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyTools.DataAnnotations;
using FrozenSky.Util;
using FrozenSky.Checking;
using FrozenSky.Multimedia.Core;

// Some namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace FrozenSky.Multimedia.DrawingVideo
{
    /// <summary>
    /// A common base class for all video writers provided by the graphics engine.
    /// </summary>
    public abstract class FrozenSkyVideoWriter
    {
        #region Configuration
        private ResourceLink m_targetFile;
        #endregion

        #region Runtime values
        private Size2 m_videoSize;
        private bool m_hasStarted;
        private bool m_hasFinished;
        private Exception m_startException;
        private Exception m_drawException;
        private Exception m_finishExeption;
        #endregion
        
        /// <summary>
        /// Occurs when recording was finished (by success or failure).
        /// </summary>
        public event EventHandler RecordingFinished;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrozenSkyVideoWriter"/> class.
        /// </summary>
        /// <param name="targetFile">The target file to write to.</param>
        public FrozenSkyVideoWriter(ResourceLink targetFile)
        {
            m_targetFile = targetFile;
        }

        /// <summary>
        /// Starts to render the video.
        /// </summary>
        internal void StartRendering(Size2 videoSize)
        {
            videoSize.EnsureNotEmpty("videoSize");
            if (m_hasStarted) { throw new FrozenSkyGraphicsException("VideoWriter has already started before!"); }
            if (m_hasFinished) { throw new FrozenSkyGraphicsException("VideoWriter has already finished before!"); }

            m_videoSize = videoSize;

            // Reset exceptions
            m_drawException = null;
            m_startException = null;
            m_finishExeption = null;

            // Ensure that the target directory exists
            try
            {
                this.StartRenderingInternal(m_videoSize);
                m_hasStarted = true;
            }
            catch(Exception ex)
            {
                m_startException = ex;
                m_hasStarted = false;
            }
        }

        /// <summary>
        /// Draws the given frame to the video.
        /// </summary>
        /// <param name="device">The device on which the given framebuffer is created.</param>
        /// <param name="uploadedTexture">The texture which should be added to the video.</param>
        public void DrawFrame(EngineDevice device, MemoryMappedTexture32bpp uploadedTexture)
        {
            try
            {
                device.EnsureNotNull("device");
                uploadedTexture.EnsureNotNull("uploadedTexture");
                if (!m_hasStarted) { throw new FrozenSkyGraphicsException("VideoWriter is not started!"); }
                if (m_hasFinished) { throw new FrozenSkyGraphicsException("VideoWriter has already finished before!"); }

                // Check for correct image size
                if (m_videoSize != uploadedTexture.PixelSize)
                {
                    throw new FrozenSkyGraphicsException("Size has changed during recording!");
                }

                this.DrawFrameInternal(device, uploadedTexture);
            }
            catch(Exception ex)
            {
                m_drawException = ex;
            }
        }

        /// <summary>
        /// Finished the rendered video.
        /// </summary>
        internal void FinishRendering()
        {
            try
            {
                if (!m_hasStarted) { throw new FrozenSkyGraphicsException("VideoWriter is not started!"); }
                if (m_hasFinished) { throw new FrozenSkyGraphicsException("VideoWriter has already finished before!"); }

                FinishRenderingInternal();
            }
            catch(Exception ex)
            {
                m_finishExeption = ex;
            }
            finally
            {
                m_hasFinished = true;
                this.RecordingFinished.Raise(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Checks whether changes on the configuration of this object are valid currently.
        /// The method throws an exception, if not.
        /// </summary>
        protected void CheckWhetherChangesAreValid()
        {
            if (m_hasStarted || m_hasFinished) { throw new FrozenSkyGraphicsException("Unable to do changed when VideoWriter is running!"); }
        }

        /// <summary>
        /// Starts rendering to the target.
        /// </summary>
        /// <param name="videoPixelSize">The pixel size of the video.</param>
        protected abstract void StartRenderingInternal(Size2 videoPixelSize);

        /// <summary>
        /// Draws the given frame to the video.
        /// </summary>
        /// <param name="device">The device on which the given framebuffer is created.</param>
        /// <param name="uploadedTexture">The texture which should be added to the video.</param>
        protected abstract void DrawFrameInternal(EngineDevice device, MemoryMappedTexture32bpp uploadedTexture);

        /// <summary>
        /// Finishes rendering to the target (closes the video file).
        /// Video rendering can not be started again from this point on.
        /// </summary>
        protected abstract void FinishRenderingInternal();

        /// <summary>
        /// Gets the target file this VideoWriter is writing to.
        /// </summary>
        [Browsable(false)]
        public ResourceLink TargetFile
        {
            get { return m_targetFile; }
        }

        /// <summary>
        /// Has rendering started?
        /// </summary>
        [Browsable(false)]
        public bool HasStarted
        {
            get { return m_hasStarted; }
        }

        /// <summary>
        /// Has rendering finished?
        /// </summary>
        [Browsable(false)]
        public bool HasFinished
        {
            get { return m_hasFinished; }
        }

        /// <summary>
        /// The RenderLoop this VideoWriter is currently associated to.
        /// </summary>
        [Browsable(false)]
        public RenderLoop AssociatedRenderLoop
        {
            get;
            internal set;
        }
        
        /// <summary>
        /// Gets the current video size.
        /// </summary>
        [Browsable(false)]
        public Size2 VideoSize
        {
            get { return m_videoSize; }
        }

        [Browsable(false)]
        public Exception LastStartException
        {
            get { return m_startException; }
        }

        [Browsable(false)]
        public Exception LastDrawException
        {
            get { return m_drawException; }
        }

        [Browsable(false)]
        public Exception LastFinishException
        {
            get { return m_finishExeption; }
        }

        [Browsable(false)]
        public string ErrorText
        {
            get
            {
                if (m_startException != null)
                {
                    return string.Format(Translatables.ERROR_VIDEO_START, m_startException.Message);
                }
                else if (m_drawException != null)
                {
                    return string.Format(Translatables.ERROR_VIDEO_RENDERING, m_drawException.Message);
                }
                else if (m_finishExeption != null)
                {
                    return string.Format(Translatables.ERROR_VIDEO_COMPLETING, m_finishExeption.Message);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
