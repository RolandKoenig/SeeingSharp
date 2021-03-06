﻿#region License information (SeeingSharp and all based games/applications)
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Util;
using SeeingSharp.Checking;
using SeeingSharp.Multimedia.Core;

// Some namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace SeeingSharp.Multimedia.DrawingVideo
{
    /// <summary>
    /// A common base class for all video writers provided by the graphics engine.
    /// </summary>
    public abstract class SeeingSharpVideoWriter
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
        /// Initializes a new instance of the <see cref="SeeingSharpVideoWriter"/> class.
        /// </summary>
        /// <param name="targetFile">The target file to write to.</param>
        public SeeingSharpVideoWriter(ResourceLink targetFile)
        {
            m_targetFile = targetFile;
        }

        /// <summary>
        /// Starts to render the video.
        /// </summary>
        internal void StartRendering(Size2 videoSize)
        {
            videoSize.EnsureNotEmpty(nameof(videoSize));
            if (m_hasStarted) { throw new SeeingSharpGraphicsException($"{nameof(SeeingSharpVideoWriter)} has already started before!"); }
            if (m_hasFinished) { throw new SeeingSharpGraphicsException($"{nameof(SeeingSharpVideoWriter)} has already finished before!"); }

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
                device.EnsureNotNull(nameof(device));
                uploadedTexture.EnsureNotNull(nameof(uploadedTexture));
                if (!m_hasStarted) { throw new SeeingSharpGraphicsException($"{nameof(SeeingSharpVideoWriter)} is not started!"); }
                if (m_hasFinished) { throw new SeeingSharpGraphicsException($"{nameof(SeeingSharpVideoWriter)} has already finished before!"); }

                // Check for correct image size
                if (m_videoSize != uploadedTexture.PixelSize)
                {
                    throw new SeeingSharpGraphicsException("Size has changed during recording!");
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
                if (!m_hasStarted) { throw new SeeingSharpGraphicsException($"{nameof(SeeingSharpVideoWriter)} is not started!"); }
                if (m_hasFinished) { throw new SeeingSharpGraphicsException($"{nameof(SeeingSharpVideoWriter)} has already finished before!"); }

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
            if (m_hasStarted || m_hasFinished) { throw new SeeingSharpGraphicsException("Unable to do changed when VideoWriter is running!"); }
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
        public ResourceLink TargetFile
        {
            get { return m_targetFile; }
        }

        /// <summary>
        /// Has rendering started?
        /// </summary>
        public bool HasStarted
        {
            get { return m_hasStarted; }
        }

        /// <summary>
        /// Has rendering finished?
        /// </summary>
        public bool HasFinished
        {
            get { return m_hasFinished; }
        }

        /// <summary>
        /// The RenderLoop this VideoWriter is currently associated to.
        /// </summary>
        public RenderLoop AssociatedRenderLoop
        {
            get;
            internal set;
        }
        
        /// <summary>
        /// Gets the current video size.
        /// </summary>
        public Size2 VideoSize
        {
            get { return m_videoSize; }
        }

        public Exception LastStartException
        {
            get { return m_startException; }
        }

        public Exception LastDrawException
        {
            get { return m_drawException; }
        }

        public Exception LastFinishException
        {
            get { return m_finishExeption; }
        }

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
