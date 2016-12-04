#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Checking;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Util;
using SharpDX;

// Namespace mappings
using MF = SharpDX.MediaFoundation;

namespace SeeingSharp.Multimedia.DrawingVideo
{
    /// <summary>
    /// This object reads video streams from a file source using the MediaFoundation.
    /// See https://msdn.microsoft.com/de-de/library/windows/desktop/dd389281(v=vs.85).aspx
    /// </summary>
    public abstract class MediaFoundationVideoReader : IDisposable, ICheckDisposed
    {
        #region Configuration
        private ResourceLink m_videoSource;
        #endregion Configuration

        #region Media foundation resources
        private Stream m_videoSourceStreamNet;
        private MF.ByteStream m_videoSourceStream;
        private MF.SourceReader m_sourceReader;
        private bool m_endReached;
        #endregion Media foundation resources

        #region Video properties
        private Size2 m_frameSize;
        private long m_durationLong;
        private long m_currentPositionLong;
        private MediaSourceCharacteristics_Internal m_characteristics;
        #endregion Video properties

#if DESKTOP
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaFoundationVideoReader"/> class.
        /// </summary>
        /// <param name="captureDevice">The capture device.</param>
        public MediaFoundationVideoReader(CaptureDeviceInfo captureDevice)
        {
            captureDevice.EnsureNotNullOrDisposed(nameof(captureDevice));

            try
            {
                // Create the source reader
                using (MF.MediaAttributes mediaAttributes = new MF.MediaAttributes(1))
                {
                    // We need the 'EnableVideoProcessing' attribute because of the RGB32 format
                    // see (lowest post): http://msdn.developer-works.com/article/11388495/How+to+use+SourceReader+(for+H.264+to+RGB+conversion)%3F
                    mediaAttributes.Set(MF.SourceReaderAttributeKeys.EnableVideoProcessing, 1);
                    mediaAttributes.Set(MF.SourceReaderAttributeKeys.DisableDxva, 1);
                    mediaAttributes.Set(MF.SourceReaderAttributeKeys.DisconnectMediasourceOnShutdown, 1);

                    // Create the MediaSource object by given capture device
                    using (MF.MediaSource mediaSource = captureDevice.CreateMediaSource())
                    {
                        // Create the source reader
                        m_sourceReader = new MF.SourceReader(mediaSource, mediaAttributes);
                    }
                }


                // Apply source configuration
                using (MF.MediaType mediaType = new MF.MediaType())
                {
                    mediaType.Set(MF.MediaTypeAttributeKeys.MajorType, MF.MediaTypeGuids.Video);
                    mediaType.Set(MF.MediaTypeAttributeKeys.Subtype, MF.VideoFormatGuids.Rgb32);
                    m_sourceReader.SetCurrentMediaType(
                        MF.SourceReaderIndex.FirstVideoStream,
                        mediaType);
                    m_sourceReader.SetStreamSelection(MF.SourceReaderIndex.FirstVideoStream, new SharpDX.Mathematics.Interop.RawBool(true));
                }
                // Read some information about the source
                using (MF.MediaType mediaType = m_sourceReader.GetCurrentMediaType(MF.SourceReaderIndex.FirstVideoStream))
                {
                    long frameSizeLong = mediaType.Get(MF.MediaTypeAttributeKeys.FrameSize);
                    m_frameSize = new Size2(MFHelper.GetValuesByMFEncodedInts(frameSizeLong));
                }

                // Get additional properties
                m_durationLong = 0;
                m_characteristics = (MediaSourceCharacteristics_Internal)m_sourceReader.GetPresentationAttribute(
                    MF.SourceReaderIndex.MediaSource, MF.SourceReaderAttributeKeys.MediaSourceCharacteristics);
            }
            catch(Exception)
            {
                this.Dispose();
                throw;
            }
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaFoundationVideoReader"/> class.
        /// </summary>
        /// <param name="videoSource">The source video file.</param>
        public MediaFoundationVideoReader(ResourceLink videoSource)
        {
            videoSource.EnsureNotNull(nameof(videoSource));

            try
            {
                m_videoSource = videoSource;

                // Create the source reader
                using (MF.MediaAttributes mediaAttributes = new MF.MediaAttributes(1))
                {
                    // We need the 'EnableVideoProcessing' attribute because of the RGB32 format
                    // see (lowest post): http://msdn.developer-works.com/article/11388495/How+to+use+SourceReader+(for+H.264+to+RGB+conversion)%3F
                    mediaAttributes.Set(MF.SourceReaderAttributeKeys.EnableVideoProcessing, 1);
                    mediaAttributes.Set(MF.SourceReaderAttributeKeys.DisableDxva, 1);

                    // Wrap the .net stream to a MF Bytestream
                    m_videoSourceStreamNet = m_videoSource.OpenInputStream();
                    m_videoSourceStream = new MF.ByteStream(m_videoSourceStreamNet);
                    try
                    {
                        using (MF.MediaAttributes byteStreamAttributes = m_videoSourceStream.QueryInterface<MF.MediaAttributes>())
                        {
                            byteStreamAttributes.Set(MF.ByteStreamAttributeKeys.OriginName, "Dummy." + videoSource.FileExtension);
                        }
                    }
                    catch (SharpDXException)
                    {
                        // The interface MF.MediaAttributes is not available on some platforms
                        // (occured during tests on Windows 7 without Platform Update)
                    }

                    // Create the sourcereader by custom native method (needed because of the ByteStream arg)
                    IntPtr sourceReaderPointer = IntPtr.Zero;
                    SharpDX.Result sdxResult = NativeMethods.MFCreateSourceReaderFromByteStream_Native(
                        m_videoSourceStream.NativePointer,
                        mediaAttributes.NativePointer,
                        out sourceReaderPointer);
                    sdxResult.CheckError();

                    m_sourceReader = new MF.SourceReader(sourceReaderPointer);
                }

                // Apply source configuration
                using (MF.MediaType mediaType = new MF.MediaType())
                {
                    mediaType.Set(MF.MediaTypeAttributeKeys.MajorType, MF.MediaTypeGuids.Video);
                    mediaType.Set(MF.MediaTypeAttributeKeys.Subtype, MF.VideoFormatGuids.Rgb32);
                    m_sourceReader.SetCurrentMediaType(
                        MF.SourceReaderIndex.FirstVideoStream,
                        mediaType);
                    m_sourceReader.SetStreamSelection(MF.SourceReaderIndex.FirstVideoStream, new SharpDX.Mathematics.Interop.RawBool(true));
                }

                // Read some information about the source
                using (MF.MediaType mediaType = m_sourceReader.GetCurrentMediaType(MF.SourceReaderIndex.FirstVideoStream))
                {
                    long frameSizeLong = mediaType.Get(MF.MediaTypeAttributeKeys.FrameSize);
                    m_frameSize = new Size2(MFHelper.GetValuesByMFEncodedInts(frameSizeLong));
                }

                // Get additional propertie3s
                m_durationLong = m_sourceReader.GetPresentationAttribute(
                    MF.SourceReaderIndex.MediaSource, MF.PresentationDescriptionAttributeKeys.Duration);
                m_characteristics = (MediaSourceCharacteristics_Internal)m_sourceReader.GetPresentationAttribute(
                    MF.SourceReaderIndex.MediaSource, MF.SourceReaderAttributeKeys.MediaSourceCharacteristics);
            }
            catch (Exception)
            {
                this.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Reads the next frame and returns the corresponding buffer.
        /// Null is returned if there was nothing to read.
        /// </summary>
        protected SeeingSharpMediaBuffer ReadFrameInternal()
        {
            if (m_endReached) { return null; }

            MF.SourceReaderFlags readerFlags;
            int dummyStreamIndex;
            using (MF.Sample nextSample = m_sourceReader.ReadSample(
                MF.SourceReaderIndex.FirstVideoStream,
                MF.SourceReaderControlFlags.None,
                out dummyStreamIndex,
                out readerFlags,
                out m_currentPositionLong))
            {
                // Check for end-of-stream
                if (readerFlags == MF.SourceReaderFlags.Endofstream)
                {
                    m_endReached = true;
                    return null;
                }

                // No sample received
                if (nextSample == null)
                {
                    return null;
                }

                // Reset end-reached flag (maybe the user called SetPosition again..)
                m_endReached = false;

                // Copy pixel data into target buffer
                if (nextSample.BufferCount > 0)
                {
                    return new SeeingSharpMediaBuffer(nextSample.GetBufferByIndex(0));
                }
            }

            return null;
        }

        /// <summary>
        /// Sets the current position of this video reader.
        /// </summary>
        /// <param name="position">The position to be set.</param>
        /// <param name="updateEndReached">Do update the EndReached flag?</param>
        public void SetCurrentPosition(TimeSpan position, bool updateEndReached = true)
        {
            position.EnsureLongerOrEqualZero(nameof(position));
            position.EnsureShorterOrEqualThan(this.Duration, nameof(position));
            this.EnsureSeekable("this");

            m_sourceReader.SetCurrentPosition(position.Ticks);

            if (updateEndReached)
            {
                m_endReached = position >= this.Duration;
            }
        }

        /// <summary>
        /// Disposes all native resources.
        /// </summary>
        public virtual void Dispose()
        {
            GraphicsHelper.SafeDispose(ref m_sourceReader);
            GraphicsHelper.SafeDispose(ref m_videoSourceStream);
            GraphicsHelper.SafeDispose(ref m_videoSourceStreamNet);
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

        /// <summary>
        /// Gets the total duration of the video.
        /// </summary>
        public TimeSpan Duration
        {
            get 
            {
                if (m_durationLong <= 0) { return TimeSpan.MaxValue; }
                return TimeSpan.FromMilliseconds((double)(m_durationLong / 10000));
            }
        }

        /// <summary>
        /// Gets the current time position within the video.
        /// </summary>
        public TimeSpan CurrentPosition
        {
            get { return TimeSpan.FromMilliseconds((double)(m_currentPositionLong / 10000)); }
        }

        public bool IsSeekable
        {
            get
            {
                return m_characteristics.HasFlag(MediaSourceCharacteristics_Internal.CanSeek) &&
                       (!m_characteristics.HasFlag(MediaSourceCharacteristics_Internal.HasSlowSeek));
            }
        }
    }
}