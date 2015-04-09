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
*/
#endregion
using FrozenSky.Multimedia.Core;
using FrozenSky.Util;
using FrozenSky.Checking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Namespace mappings
using MF = SharpDX.MediaFoundation;

namespace FrozenSky.Multimedia.DrawingVideo
{
    /// <summary>
    /// This object reads video streams from a file source using the MediaFoundation.
    /// See https://msdn.microsoft.com/de-de/library/windows/desktop/dd389281(v=vs.85).aspx
    /// </summary>
    public class MediaFoundationVideoReader : IDisposable
    {
        #region Configuration
        private ResourceLink m_videoSource;
        #endregion

        #region Media foundation resources
        private Stream m_videoSourceStream;
        private MF.SourceReader m_sourceReader;
        private Size2 m_frameSize;
        private bool m_endReached;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaFoundationVideoReader"/> class.
        /// </summary>
        /// <param name="videoSource">The source video file.</param>
        public MediaFoundationVideoReader(ResourceLink videoSource)
        {
            try
            {
                m_videoSource = videoSource;

                // Create the source reader
#if DESKTOP
                DesktopFileSystemResourceLink fileVideoResource = videoSource as DesktopFileSystemResourceLink;
                if(fileVideoResource != null)
                {
                    m_sourceReader = new MF.SourceReader(fileVideoResource.FilePath);
                }
#endif
                if (m_sourceReader == null)
                {
                    m_videoSourceStream = m_videoSource.OpenInputStream();
                    m_sourceReader = new MF.SourceReader(m_videoSourceStream);
                }

                // Read some information about the source
                using (MF.MediaType mediaType = m_sourceReader.GetCurrentMediaType(MF.SourceReaderIndex.FirstVideoStream))
                {
                    long frameSizeLong = mediaType.Get(MF.MediaTypeAttributeKeys.FrameSize);
                    m_frameSize = new Size2(MFHelper.GetValuesByMFEncodedInts(frameSizeLong));
                }

                // Set the source type to video / uncompressed format
                using (MF.MediaType mediaType = new MF.MediaType())
                {
                    mediaType.Set(MF.MediaTypeAttributeKeys.MajorType, MF.MediaTypeGuids.Video);
                    mediaType.Set(MF.MediaTypeAttributeKeys.Subtype, MFVideoFormats.FORMAT_RBG32); //MF.VideoFormatGuids.Rgb32);
                    mediaType.Set(MF.MediaTypeAttributeKeys.FrameSize, MFHelper.GetMFEncodedIntsByValues(m_frameSize.Width, m_frameSize.Height));
                    m_sourceReader.SetCurrentMediaType(
                        MF.SourceReaderIndex.FirstVideoStream, 
                        mediaType);
                }
            }
            catch(Exception)
            {
                this.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Reads the next frame and puts it into a newly generated buffer.
        /// </summary>
        public MemoryMappedTexture32bpp ReadFrame()
        {
            MemoryMappedTexture32bpp result = new MemoryMappedTexture32bpp(m_frameSize);
            try
            {
                if (this.ReadFrame(result)) { return result; }
                else 
                {
                    result.Dispose();
                    return null;
                }
            }
            catch(Exception)
            {
                result.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Reads the next frame and puts it into the provided buffer.
        /// </summary>
        /// <param name="targetBuffer">The target buffer to write to.</param>
        public bool ReadFrame(MemoryMappedTexture32bpp targetBuffer)
        {
            if (this.IsDisposed) { throw new ObjectDisposedException(this.GetType().FullName); }
            targetBuffer.EnsureNotNull("targetBuffer");
            if((targetBuffer.Width != m_frameSize.Width) ||
               (targetBuffer.Height != m_frameSize.Height))
            {
                throw new FrozenSkyGraphicsException("Size of the given buffer does not match the video size!");
            }

            MF.SourceReaderFlags readerFlags;
            long timestamp;
            int dummyStreamIndex;
            using (MF.Sample nextSample = m_sourceReader.ReadSample(
                MF.SourceReaderIndex.FirstVideoStream,
                MF.SourceReaderControlFlags.None,
                out dummyStreamIndex,
                out readerFlags,
                out timestamp))
            {
                // Check for end-of-stream
                if (readerFlags == MF.SourceReaderFlags.Endofstream)
                {
                    m_endReached = true;
                    return false;
                }

                // No sample received
                if(nextSample == null)
                {
                    return false;
                }

                // Copy pixel data into target buffer
                if (nextSample.BufferCount > 0)
                {
                    using (MF.MediaBuffer mediaBuffer = nextSample.GetBufferByIndex(0))
                    {
                        int cbMaxLength;
                        int cbCurrentLenght;
                        IntPtr mediaBufferPointer = mediaBuffer.Lock(out cbMaxLength, out cbCurrentLenght);
                        try
                        {
                            //byte[] rawBytes = new byte[cbMaxLength];
                            //for(int loop=0 ; loop<cbMaxLength; loop++)
                            //{
                            //    rawBytes[loop] = System.Runtime.InteropServices.Marshal.ReadByte(rawBytes, loop);
                            //}

                            int stride = m_frameSize.Width * 4;
                            MF.MediaFactory.CopyImage(
                                targetBuffer.Pointer,
                                stride,
                                mediaBufferPointer,
                                stride,
                                stride,
                                m_frameSize.Height);
                        }
                        finally
                        {
                            mediaBuffer.Unlock();
                        }
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Disposes all resources.
        /// </summary>
        public void Dispose()
        {
            GraphicsHelper.SafeDispose(ref m_sourceReader);
            GraphicsHelper.SafeDispose(ref m_videoSourceStream);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        public bool IsDisposed
        {
            get { return m_sourceReader == null; }
        }

        /// <summary>
        /// Did we reach the end of the video stream?
        /// </summary>
        public bool EndReached
        {
            get { return m_endReached; }
        }

        /// <summary>
        /// Gets the pixel size per frame.
        /// </summary>
        public Size2 FrameSize
        {
            get { return m_frameSize; }
        }
    }
}
