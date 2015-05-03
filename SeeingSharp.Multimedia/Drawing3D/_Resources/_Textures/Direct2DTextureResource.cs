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
#endregion

using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing2D;
using SeeingSharp.Checking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Namespace mappings
using D3D11 = SharpDX.Direct3D11;
using D2D = SharpDX.Direct2D1;
using DXGI = SharpDX.DXGI;
#if DESKTOP
using D3D10 = SharpDX.Direct3D10;
#endif

namespace SeeingSharp.Multimedia.Drawing3D
{
    public class Direct2DTextureResource : TextureResource, IRenderableResource
    {
        // Intial configuration
        #region
        private Custom2DDrawingLayer m_drawingLayer;
        private int m_width;
        private int m_height;
        #endregion

        // Resources for rendering
        #region
        private Graphics2D m_graphics2D;
        private D2D.RenderTarget m_renderTarget2D;
        private D3D11.Texture2D m_renderTargetTexture;
        private D3D11.ShaderResourceView m_renderTargetTextureView;
#if DESKTOP
        private D3D10.Texture2D m_renderTarget3DShared10;
#endif
        private DXGI.Resource m_renderTarget3DSharedDxgi;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Direct2DTextureResource"/> class.
        /// </summary>
        /// <param name="drawingLayer">The drawing layer.</param>
        /// <param name="height">The width of the generated texture.</param>
        /// <param name="width">The height of the generated texture.</param>
        public Direct2DTextureResource(Custom2DDrawingLayer drawingLayer, int width, int height)
        {
            drawingLayer.EnsureNotNull("drawingLayer");
            width.EnsurePositive("width");
            height.EnsurePositive("height");

            m_drawingLayer = drawingLayer;
            m_width = width;
            m_height = height;
        }

        /// <summary>
        /// Triggers internal update within the resource (e. g. Render to Texture).
        /// </summary>
        /// <param name="updateState">Current state of update process.</param>
        public void Update(UpdateState updateState)
        {

        }

        /// <summary>
        /// Triggers internal rendering within the resource (e. g. Render to Texture).
        /// </summary>
        /// <param name="renderState">Current render state.</param>
        public void Render(RenderState renderState)
        {
            m_renderTarget2D.BeginDraw();
            try
            {
                if (m_graphics2D != null)
                {
                    m_drawingLayer.Draw2DInternal(m_graphics2D);
                }
            }
            finally
            {
                m_renderTarget2D.EndDraw();
            }
        }

        /// <summary>
        /// Loads all resource.
        /// </summary>
        /// <param name="device">The device on which to load all resources.</param>
        /// <param name="resources">The current ResourceDictionary.</param>
        protected override void LoadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            // Try to create a Direct2D render target based on the Direct3D 11 texture directly
            //  => This should work starting with windows 8
            try
            {
                m_renderTargetTexture = GraphicsHelper.CreateRenderTargetTexture(
                    device, m_width, m_height, new GraphicsViewConfiguration() { AntialiasingEnabled = false });
                m_renderTargetTextureView = new D3D11.ShaderResourceView(device.DeviceD3D11, m_renderTargetTexture);
                using (DXGI.Surface dxgiSurface = m_renderTargetTexture.QueryInterface<DXGI.Surface>())
                {
                    m_renderTarget2D = new D2D.RenderTarget(
                        device.Core.FactoryD2D,
                        dxgiSurface,
                        new D2D.RenderTargetProperties()
                        {
                            MinLevel = D2D.FeatureLevel.Level_10,
                            Type = D2D.RenderTargetType.Default, 
                            Usage = D2D.RenderTargetUsage.ForceBitmapRemoting,
                            PixelFormat = new D2D.PixelFormat(GraphicsHelper.DEFAULT_TEXTURE_FORMAT, D2D.AlphaMode.Premultiplied),
                        });
                    m_graphics2D = new Graphics2D(device, m_renderTarget2D, new Size2F(m_width, m_height));
                    return;
                }
            }
            catch (Exception) 
            {
                GraphicsHelper.SafeDispose(ref m_renderTarget2D);
                GraphicsHelper.SafeDispose(ref m_renderTargetTextureView);
                GraphicsHelper.SafeDispose(ref m_renderTargetTexture);
            }

            // No software device (or WinRT) supported on fallback mode
#if DESKTOP
            if (device.IsSoftware) { return; }

            // Create the shared texture (same texture with Direct3D 11 and Direct3D 10 reference to it)
            m_renderTargetTexture = GraphicsHelper.CreateSharedTexture(device, m_width, m_height);
            m_renderTargetTextureView = new D3D11.ShaderResourceView(device.DeviceD3D11, m_renderTargetTexture);
            m_renderTarget3DSharedDxgi = m_renderTargetTexture.QueryInterface<DXGI.Resource>();
            IntPtr sharedHandle = m_renderTarget3DSharedDxgi.SharedHandle;
            m_renderTarget3DShared10 = device.DeviceD3D10.OpenSharedResource<D3D10.Texture2D>(sharedHandle);

            // Create the render target
            using (DXGI.Surface dxgiSurface = m_renderTarget3DShared10.QueryInterface<DXGI.Surface>())
            {
                m_renderTarget2D = new D2D.RenderTarget(
                    device.Core.FactoryD2D,
                    dxgiSurface,
                    new D2D.RenderTargetProperties()
                    {
                        MinLevel = D2D.FeatureLevel.Level_10,
                        Type = D2D.RenderTargetType.Hardware,
                        Usage = D2D.RenderTargetUsage.ForceBitmapRemoting,
                        PixelFormat = new D2D.PixelFormat(GraphicsHelper.DEFAULT_TEXTURE_FORMAT_SHARING_D2D, D2D.AlphaMode.Premultiplied)
                    });
                m_graphics2D = new Graphics2D(device, m_renderTarget2D, new Size2F(m_width, m_height));
            }
#endif
        }

        /// <summary>
        /// Unloads all resources.
        /// </summary>
        /// <param name="device">The device on which the resources where loaded.</param>
        /// <param name="resources">The current ResourceDictionary.</param>
        protected override void UnloadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            m_graphics2D = null;
            GraphicsHelper.SafeDispose(ref m_renderTarget2D);
#if DESKTOP
            GraphicsHelper.SafeDispose(ref m_renderTarget3DShared10);
#endif
            GraphicsHelper.SafeDispose(ref m_renderTarget3DSharedDxgi);
            GraphicsHelper.SafeDispose(ref m_renderTargetTextureView);
            GraphicsHelper.SafeDispose(ref m_renderTargetTexture);
        }

        /// <summary>
        /// Is the resource loaded?
        /// </summary>
        public override bool IsLoaded
        {
            get { return m_graphics2D != null; }
        }

        /// <summary>
        /// Gets the texture object.
        /// </summary>
        public override D3D11.Texture2D Texture
        {
            get { return m_renderTargetTexture; }
        }

        /// <summary>
        /// Gets a ShaderResourceView targeting the texture.
        /// </summary>
        public override D3D11.ShaderResourceView TextureView
        {
            get { return m_renderTargetTextureView; }
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
