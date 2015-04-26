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
using SeeingSharp.Multimedia.DrawingVideo;
using SeeingSharp.Checking;
using SeeingSharp.Util;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace SeeingSharp.Multimedia.Drawing3D
{
    /// <summary>
    /// A texture which displays a video on it using Media Foundation.
    /// See https://msdn.microsoft.com/de-de/library/windows/desktop/dd389281(v=vs.85).aspx
    /// </summary>
    public class VideoTextureResource : TextureResource, IRenderableResource
    {
        #region Configuration
        private ResourceLink m_videoSource;
        #endregion

        #region Direct3D resources
        private D3D11.Texture2D m_texture;
        private D3D11.ShaderResourceView m_textureView;
        private int m_currentWidth;
        private int m_currentHeight;
        #endregion

        #region Media foundation resources
        private MediaFoundationVideoReader m_videoReader;
        private MemoryMappedTexture32bpp m_videoFrameBuffer;
        private bool m_newFrameArrived;
        #endregion

        public VideoTextureResource(ResourceLink videoSource)
        {
            videoSource.EnsureNotNull("videoSource");

            m_videoSource = videoSource;
            m_videoReader = new MediaFoundationVideoReader(m_videoSource);
            m_videoFrameBuffer = new MemoryMappedTexture32bpp(m_videoReader.FrameSize);

            m_currentWidth = m_videoReader.FrameSize.Width;
            m_currentHeight = m_videoReader.FrameSize.Height;
        }

        protected override void LoadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            m_texture = GraphicsHelper.CreateCpuWritableTexture(
                device, m_currentWidth, m_currentHeight);
            m_textureView = new D3D11.ShaderResourceView(device.DeviceD3D11, m_texture);
        }

        protected override void UnloadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            GraphicsHelper.SafeDispose(ref m_videoFrameBuffer);
            GraphicsHelper.SafeDispose(ref m_videoReader);

            GraphicsHelper.SafeDispose(ref m_textureView);
            GraphicsHelper.SafeDispose(ref m_texture);
        }

        /// <summary>
        /// Triggers internal update within the resource (e. g. Render to Texture).
        /// </summary>
        /// <param name="updateState">Current state of update process.</param>
        public void Update(UpdateState updateState)
        {
            if (m_videoReader.EndReached && (!m_videoReader.IsSeekable)) { return; }

            int countTries = 0;
            while(!m_videoReader.ReadFrame(m_videoFrameBuffer))
            {
                countTries++;

                // Handle End-Reached event
                if (m_videoReader.EndReached) 
                {
                    if (!m_videoReader.IsSeekable) { return; }
                    m_videoReader.SetCurrentPosition(TimeSpan.Zero);
                }

                if (countTries > 3) { return; }
            }

            m_newFrameArrived = true;
        }

        /// <summary>
        /// Triggers internal rendering within the resource (e. g. Render to Texture).
        /// </summary>
        /// <param name="renderState">Current render state.</param>
        public void Render(RenderState renderState)
        {
            D3D11.DeviceContext deviceContext = renderState.Device.DeviceImmediateContextD3D11;

            // Upload the last read video frame to texture buffer on the graphics device
            if (m_newFrameArrived)
            {
                m_newFrameArrived = false;
                SharpDX.DataBox dataBox = deviceContext.MapSubresource(m_texture, 0, D3D11.MapMode.WriteDiscard, D3D11.MapFlags.None);
                try
                {
                    unsafe
                    {
                        int* frameBufferPointerNative = (int*)m_videoFrameBuffer.Pointer.ToPointer();
                        int* textureBufferPointerNative = (int*)dataBox.DataPointer.ToPointer();
                        int textureBufferRowPixels = dataBox.RowPitch / 4;
                        for (int loopX = 0; loopX < m_currentWidth; loopX++)
                        {
                            for (int loopY = 0; loopY < m_currentHeight; loopY++)
                            {
                                int actIndexVideo = loopX + (loopY * m_currentWidth);
                                int actIndexTexture = loopX + (loopY * textureBufferRowPixels);
                                textureBufferPointerNative[actIndexTexture] = frameBufferPointerNative[actIndexVideo];
                            }
                        }
                    }
                }
                finally
                {
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
