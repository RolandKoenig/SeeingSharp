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
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.DrawingVideo;
using SeeingSharp.Checking;
using SeeingSharp.Util;
using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Namespace mappings
using D3D11 = SharpDX.Direct3D11;
using MF = SharpDX.MediaFoundation;

namespace SeeingSharp.Multimedia.Drawing3D
{
    /// <summary>
    /// A texture which displays a thumbnail of a video.
    /// </summary>
    public class VideoThumbnailTextureResource : TextureResource, IRenderableResource
    {
        #region Direct3D resources
        private D3D11.Texture2D m_texture;
        private D3D11.ShaderResourceView m_textureView;
        private int m_videoWidth;
        private int m_videoHeight;
        #endregion

        #region Resources for thumbnail reading
        private FrameByFrameVideoReader m_videoReader;
        private MemoryMappedTexture32bpp m_thumbnailFrame;
        private TimeSpan m_thumbnailTimestamp;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoThumbnailTextureResource"/> class.
        /// </summary>
        /// <param name="videoSource">The video source.</param>
        /// <param name="timestamp">The Timestamp from which to create the timestamp.</param>
        public VideoThumbnailTextureResource(ResourceLink videoSource, TimeSpan timestamp)
        {
            videoSource.EnsureNotNull(nameof(videoSource));
            timestamp.EnsureLongerOrEqualZero(nameof(timestamp));

            // Create the FrameByFrameVideoReader
            try
            {
                m_videoReader = new FrameByFrameVideoReader(videoSource);
                m_videoReader.EnsureSeekable(nameof(m_videoReader));
            }
            catch
            {
                GraphicsHelper.SafeDispose(ref m_videoReader);
                throw;
            }

            // Store width and height of the video (to prepare target texture)
            m_videoWidth = m_videoReader.FrameSize.Width;
            m_videoHeight = m_videoReader.FrameSize.Height;

            m_thumbnailTimestamp = timestamp;
        }

        /// <summary>
        /// Loads all resources.
        /// </summary>
        /// <param name="device">The device on which to load all resources.</param>
        /// <param name="resources">The current ResourceDictionary.</param>
        protected override void LoadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            // Prepare texture
            m_texture = GraphicsHelper.CreateCpuWritableTexture(
                device, m_videoWidth, m_videoHeight);
            m_textureView = new D3D11.ShaderResourceView(device.DeviceD3D11_1, m_texture);

            // Read the thumbnail
            m_videoReader.SetCurrentPosition(m_thumbnailTimestamp);
            m_thumbnailFrame = m_videoReader.ReadFrame();
            m_thumbnailFrame.SetAllAlphaValuesToOne_ARGB();
        }

        /// <summary>
        /// Unloads all resources.
        /// </summary>
        /// <param name="device">The device on which the resources where loaded.</param>
        /// <param name="resources">The current ResourceDictionary.</param>
        protected override void UnloadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            GraphicsHelper.SafeDispose(ref m_textureView);
            GraphicsHelper.SafeDispose(ref m_texture);
            GraphicsHelper.SafeDispose(ref m_videoReader);
            GraphicsHelper.SafeDispose(ref m_thumbnailFrame);
        }

        /// <summary>
        /// Triggers internal update within the resource (e. g. Render to Texture).
        /// </summary>
        /// <param name="updateState">Current state of update process.</param>
        public void Update(SceneRelatedUpdateState updateState)
        {

        }

        /// <summary>
        /// Triggers internal rendering within the resource (e. g. Render to Texture).
        /// </summary>
        /// <param name="renderState">Current render state.</param>
        public void Render(RenderState renderState)
        {
            D3D11.DeviceContext deviceContext = renderState.Device.DeviceImmediateContextD3D11;

            // Upload the last read video frame to texture buffer on the graphics device
            if(m_thumbnailFrame != null)
            {
                SharpDX.DataBox dataBox = deviceContext.MapSubresource(m_texture, 0, D3D11.MapMode.WriteDiscard, D3D11.MapFlags.None);
                try
                {
                    unsafe
                    {
                        int* frameBufferPointerNative = (int*)m_thumbnailFrame.Pointer.ToPointer();
                        int* textureBufferPointerNative = (int*)dataBox.DataPointer.ToPointer();

                        // Performance optimization using MemCopy
                        //  see http://www.rolandk.de/wp/2015/05/wie-schnell-man-speicher-falsch-kopieren-kann/
                        for (int loopY = 0; loopY < m_videoHeight; loopY++)
                        {
                            IntPtr rowStartTexture = new IntPtr(textureBufferPointerNative + (dataBox.RowPitch / 4) * loopY);
                            IntPtr rowStartSource = new IntPtr(frameBufferPointerNative + (m_videoWidth) * loopY);
                            CommonTools.CopyMemory(rowStartSource, rowStartTexture, (ulong)dataBox.RowPitch);
                        }
                    }

                }
                finally
                {
                    GraphicsHelper.SafeDispose(ref m_thumbnailFrame);
                    deviceContext.UnmapSubresource(m_texture, 0);
                }
            }
        }

        /// <summary>
        /// Is the resource loaded?
        /// </summary>
        public override bool IsLoaded
        {
            get { return m_texture != null; }
        }

        /// <summary>
        /// Gets the texture object.
        /// </summary>
        public override D3D11.Texture2D Texture
        {
            get { return m_texture; }
        }

        /// <summary>
        /// Gets a ShaderResourceView targeting the texture.
        /// </summary>
        public override D3D11.ShaderResourceView TextureView
        {
            get { return m_textureView; }
        }

        /// <summary>
        /// Gets the size of the texture array.
        /// 1 for normal textures.
        /// 6 for cubemap textures.
        /// </summary>
        public override int ArraySize
        {
            get { return 1; }
        }
    }
}