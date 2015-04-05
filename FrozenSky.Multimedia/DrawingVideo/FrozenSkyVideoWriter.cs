#region License information (FrozenSky and all based games/applications)
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

	********************************************************************************
    Additional permission under GNU GPL version 3 section 7

    If you modify this Program, or any covered work, by linking or combining it with 
	DirectX (or a modified version of that library), containing parts covered by the 
	terms of [name of library's license], the licensors of this Program grant you additional 
	permission to convey the resulting work. {Corresponding Source for a non-source form of 
	such a combination shall include the source code for the parts of DirectX used 
	as well as that of the covered work.}
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
using FrozenSky.Multimedia.Core;

#if UNIVERSAL
using Windows.Storage;
#endif

// Some namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace FrozenSky.Multimedia.DrawingVideo
{
    /// <summary>
    /// A common base class for all video writers provided by the graphics engine.
    /// </summary>
    public abstract class FrozenSkyVideoWriter
    {
        // Configuration
        #region
        private string m_fileNameTemplate;
#if DESKTOP
        private string m_targetFolderPath;
#endif
        #endregion

        // Runtime values
        #region
        private int m_fileCounter;
#if DESKTOP
        private string m_lastFileName;
#endif
        private Size2 m_videoSize;
        private bool m_isStarted;
        private Exception m_lastStartException;
        private Exception m_lastDrawException;
        private Exception m_lastFinishExeption;
        #endregion

        public event EventHandler RecordingFinished;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrozenSkyVideoWriter"/> class.
        /// </summary>
        public FrozenSkyVideoWriter()
        {
            m_fileCounter = -1;
        }

        public void ResetRenderExceptions()
        {
            m_lastFinishExeption = null;
            m_lastDrawException = null;
            m_lastStartException = null;
        }

        /// <summary>
        /// Gets the next file to be written by this video writer.
        /// </summary>
        /// <param name="restoreFileCounter">Do resture the file counter before returning the new filename?</param>
        protected string GetNextFileName(bool restoreFileCounter = false)
        {
            int prevFileCounter = m_fileCounter;

            // Query for next file name
            m_fileCounter++;
#if DESKTOP
            string actFileName = Path.Combine(m_targetFolderPath, string.Format(m_fileNameTemplate, m_fileCounter));
            while (File.Exists(actFileName))
            {
                m_fileCounter++;
                actFileName = Path.Combine(m_targetFolderPath, string.Format(m_fileNameTemplate, m_fileCounter));
            }

            if (restoreFileCounter) { m_fileCounter = prevFileCounter; }
            m_lastFileName = actFileName;

            return actFileName;
#else
            throw new NotImplementedException("Not implemented for WinRT currently!");
#endif
        }

        /// <summary>
        /// Starts to render the video.
        /// </summary>
        internal void StartRendering(Size2 videoSize)
        {
            if (m_isStarted) { throw new FrozenSkyGraphicsException("VideoWriter is already started!"); }

            m_videoSize = videoSize;

#if UNIVERSAL
            throw new NotImplementedException("Not implemented for WinRT currently!");
#else

            // Reset exceptions
            m_lastDrawException = null;
            m_lastStartException = null;
            m_lastFinishExeption = null;

            // Ensure that the target directory exists
            try
            {
                if (!Directory.Exists(m_targetFolderPath))
                {
                    Directory.CreateDirectory(m_targetFolderPath);
                }

                this.StartRenderingInternal(m_videoSize);
                m_isStarted = true;
            }
            catch(Exception ex)
            {
                m_lastStartException = ex;
                m_isStarted = false;
            }
#endif
        }

        /// <summary>
        /// Draws the given frame to the video.
        /// </summary>
        /// <param name="device">The device on which the given framebuffer is created.</param>
        /// <param name="uploadedTexture">The texture which should be added to the video.</param>
        public void DrawFrame(EngineDevice device, MemoryMappedTexture32bpp uploadedTexture)
        {
            if (!m_isStarted) { throw new FrozenSkyGraphicsException("VideoWriter is not started!"); }

            // Check for correct image size
            if(m_videoSize != uploadedTexture.PixelSize)
            {
                throw new FrozenSkyGraphicsException("Size has changed during recording!");
            }

            try
            {
                this.DrawFrameInternal(device, uploadedTexture);
            }
            catch(Exception ex)
            {
                m_lastDrawException = ex;
            }
        }

        /// <summary>
        /// Finished the rendered video.
        /// </summary>
        internal void FinishRendering()
        {
            try
            {
                FinishRenderingInternal();
            }
            catch(Exception ex)
            {
                m_lastFinishExeption = ex;
            }
            finally
            {
                m_isStarted = false;
                this.RecordingFinished.Raise(this, EventArgs.Empty);
            }
        }

        protected void CheckWhetherChangesAreValid()
        {
            if (m_isStarted) { throw new FrozenSkyGraphicsException("Unable to do changed when VideoWriter is running!"); }
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

#if DESKTOP
        /// <summary>
        /// Gets or sets the target directory.
        /// </summary>
        [DirectoryPath]
        public string TargetDirectory
        {
            get { return m_targetFolderPath; }
            set
            {
                this.CheckWhetherChangesAreValid();
                m_targetFolderPath = value;
            }
        }
#endif
       
        public string FileNameTemplate
        {
            get { return m_fileNameTemplate; }
            set
            {
                this.CheckWhetherChangesAreValid();
                m_fileNameTemplate = value;
            }
        }

#if DESKTOP
        [Browsable(false)]
        public string LastGeneratedFileName
        {
            get { return m_lastFileName; }
        }
#endif

        /// <summary>
        /// Was rendering started?
        /// </summary>
        [Browsable(false)]
        public bool IsStarted
        {
            get { return m_isStarted; }
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
            get { return m_lastStartException; }
        }

        [Browsable(false)]
        public Exception LastDrawException
        {
            get { return m_lastDrawException; }
        }

        [Browsable(false)]
        public Exception LastFinishException
        {
            get { return m_lastFinishExeption; }
        }

//#if DESKTOP
//        [Browsable(false)]
//        public string ErrorText
//        {
//            get
//            {
//                if(m_lastStartException != null)
//                {
//                    return string.Format(Localizables.ERROR_VIDEO_START, m_lastStartException.Message);
//                }
//                else if(m_lastDrawException != null)
//                {
//                    return string.Format(Localizables.ERROR_VIDEO_RENDERING, m_lastDrawException.Message);
//                }
//                else if(m_lastFinishExeption != null)
//                {
//                    return string.Format(Localizables.ERROR_VIDEO_COMPLETING, m_lastFinishExeption.Message);
//                }
//                else
//                {
//                    return base.ErrorWithoutHtml;
//                }
//            }
//        }
//#endif
    }
}
