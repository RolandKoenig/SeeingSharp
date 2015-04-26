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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Util;
using SeeingSharp.Multimedia.Drawing2D;
using SeeingSharp.Multimedia.Drawing3D;

// Some namespace mappings
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;
using D2D = SharpDX.Direct2D1;

#if DESKTOP
using D3D10 = SharpDX.Direct3D10;
#endif

namespace SeeingSharp.Multimedia.Core
{
    /// <summary>
    /// This class provides a generic way to draw Direct2D content into a Direct3D texture (typically the render target).
    ///   - This works by default in Windows 8.
    ///   - In Windows 7, the platform update is needed to to the default way
    ///     http://msdn.microsoft.com/en-us/library/windows/desktop/jj863687(v=vs.85).aspx
    ///   - Otherwhiese, the Fallback-Method is used (render Direct2D content to a shared texture, then render it onto the render target)
    /// </summary>
    class Direct2DOverlayRenderer : IDisposable
    {
        private static readonly NamedOrGenericKey RES_KEY_FALLBACK_TEXTURE = GraphicsCore.GetNextGenericResourceKey();

        // Graphics object
        private Graphics2D m_graphics2D;

        // Given resources
        private EngineDevice m_device;
        private D3D11.Texture2D m_renderTarget3D;

        // Own 2D render target resource
        private D2D.RenderTarget m_renderTarget2D;

        // Only set if we need it (e. g. Win7, Win 2008 platforms)
        private ResourceDictionary m_resourceDict;
#if DESKTOP
        private ChangeableTextureResource m_changeableTexture;
        private TexturePainterHelper m_texturePainter;
        private D3D11.Texture2D m_renderTarget3DShared11;
        private D3D10.Texture2D m_renderTarget3DShared10;
        private D3D11.ShaderResourceView m_renderTarget3DShared11View;
        private DXGI.Resource m_renderTarget3DSharedDxgi;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="Direct2DOverlayRenderer"/> class.
        /// </summary>
        public Direct2DOverlayRenderer(EngineDevice device, D3D11.Texture2D renderTarget3D, int viewWidth, int viewHeight, DpiScaling dpiScaling)
        {
            m_device = device;
            m_renderTarget3D = renderTarget3D;

            CreateResources(viewWidth, viewHeight, dpiScaling);
        }

        /// <summary>
        /// Disposes all resources of this object completely.
        /// </summary>
        public void Dispose()
        {
            // Dispose all created objects
            GraphicsHelper.SafeDispose(ref m_renderTarget2D);
#if DESKTOP
            GraphicsHelper.SafeDispose(ref m_renderTarget3DSharedDxgi);
            GraphicsHelper.SafeDispose(ref m_renderTarget3DShared10);
            GraphicsHelper.SafeDispose(ref m_renderTarget3DShared11View);
            GraphicsHelper.SafeDispose(ref m_renderTarget3DShared11);
#endif

            // Unload all created resources
            if (m_resourceDict != null)
            {
                m_resourceDict.UnloadResources();
                m_resourceDict.Clear();
                m_resourceDict = null;
            }

#if DESKTOP
            // Reset remaining members
            m_changeableTexture = null;
            m_texturePainter = null;
#endif
        }

        /// <summary>
        /// Creates all resources 
        /// </summary>
        private void CreateResources(int viewWidth, int viewHeight, DpiScaling dpiScaling)
        {
            // Calculate the screen size in device independent units
            Size2F scaledScreenSize = new Size2F(
                (float)viewWidth / dpiScaling.ScaleFactorX,
                (float)viewHeight / dpiScaling.ScaleFactorY);

            // Try to create a Direct2D render target based on the Direct3D 11 texture directly
            //  => This should work starting with windows 8
            try
            {
                using (DXGI.Surface dxgiSurface = m_renderTarget3D.QueryInterface<DXGI.Surface>())
                {
                    m_renderTarget2D = new D2D.RenderTarget(
                        m_device.Core.FactoryD2D,
                        dxgiSurface,
                        new D2D.RenderTargetProperties()
                        {
                            MinLevel = D2D.FeatureLevel.Level_10,
                            Type = D2D.RenderTargetType.Default, //m_device.IsSoftware ? D2D.RenderTargetType.Software : D2D.RenderTargetType.Hardware,
                            Usage = D2D.RenderTargetUsage.ForceBitmapRemoting,
                            PixelFormat = new D2D.PixelFormat(GraphicsHelper.DEFAULT_TEXTURE_FORMAT, D2D.AlphaMode.Premultiplied),
                            DpiX = dpiScaling.DpiX,
                            DpiY = dpiScaling.DpiY
                        });
                    m_graphics2D = new Graphics2D(m_device, m_renderTarget2D, scaledScreenSize);
                    return;
                }
            }
            catch (Exception) { }

            // Next step does not work on software devices
            if (m_device.IsSoftware) { return; }

            // Fallback method: Build a brigde using a Direct3D 10 texture
            //  => This should work on all Desktop-OS
#if DESKTOP
            try
            {
                m_resourceDict = new ResourceDictionary(m_device);

                // Create the shared texture (same texture with Direct3D 11 and Direct3D 10 reference to it)
                m_renderTarget3DShared11 = GraphicsHelper.CreateSharedTexture(m_device, (int)scaledScreenSize.Width, (int)scaledScreenSize.Height);
                m_renderTarget3DShared11View = new D3D11.ShaderResourceView(m_device.DeviceD3D11, m_renderTarget3DShared11);
                m_renderTarget3DSharedDxgi = m_renderTarget3DShared11.QueryInterface<DXGI.Resource>();
                IntPtr sharedHandle = m_renderTarget3DSharedDxgi.SharedHandle;
                m_renderTarget3DShared10 = m_device.DeviceD3D10.OpenSharedResource<D3D10.Texture2D>(sharedHandle);

                // Create the render target
                using (DXGI.Surface dxgiSurface = m_renderTarget3DShared10.QueryInterface<DXGI.Surface>())
                {
                    m_renderTarget2D = new D2D.RenderTarget(
                        m_device.Core.FactoryD2D,
                        dxgiSurface,
                        new D2D.RenderTargetProperties()
                        {
                            MinLevel = D2D.FeatureLevel.Level_10,
                            Type = D2D.RenderTargetType.Hardware,
                            Usage = D2D.RenderTargetUsage.ForceBitmapRemoting,
                            PixelFormat = new D2D.PixelFormat(GraphicsHelper.DEFAULT_TEXTURE_FORMAT_SHARING_D2D, D2D.AlphaMode.Premultiplied),
                            DpiX = dpiScaling.DpiX,
                            DpiY = dpiScaling.DpiY
                        });
                    m_graphics2D = new Graphics2D(m_device, m_renderTarget2D, scaledScreenSize);
                }
            }
            catch (Exception) { }
#endif
        }

        /// <summary>
        /// Begins the draw.
        /// </summary>
        public void BeginDraw(RenderState renderState)
        {
            // Start Direct2D rendering
            m_renderTarget2D.BeginDraw();

#if DESKTOP
            // Fallback method: Clear Direct2D buffer before starting with rendering
            //                  Later we draw the contents of this texture to the render target as a fullscreen texture
            if (m_renderTarget3DShared10 != null)
            {
                m_renderTarget2D.Clear(Color4.Transparent.ToDXColor());

                // Create resources for later texture drawing
                if (m_texturePainter == null)
                {
                    // Precreate the texture painter
                    m_changeableTexture = m_resourceDict.GetResourceAndEnsureLoaded<ChangeableTextureResource>(
                        RES_KEY_FALLBACK_TEXTURE,
                        () => new ChangeableTextureResource());
                    m_texturePainter = new TexturePainterHelper(RES_KEY_FALLBACK_TEXTURE);
                    m_texturePainter.LoadResources(m_resourceDict);
                }
            }
#endif
        }

        /// <summary>
        /// Finishes Direct2D drawing.
        /// </summary>
        public void EndDraw(RenderState renderState)
        {
            // Finish Direct2D drawing
            m_renderTarget2D.EndDraw();

#if DESKTOP
            if (m_renderTarget3DShared10 != null)
            {
                // Flush render calls on Direct3D 10 device
                m_device.DeviceD3D10.Flush();

                // Draw Direct2D contents to the render target (just draw the texture)
                m_changeableTexture.SetContents(m_renderTarget3DShared11, m_renderTarget3DShared11View);
                m_texturePainter.RenderPlain(renderState);
            }
#endif
        }

        /// <summary>
        /// Is this resource loaded correctly?
        /// </summary>
        public bool IsLoaded
        {
            get { return m_renderTarget2D != null; }
        }

        /// <summary>
        /// Gets the Direct2D render target.
        /// </summary>
        internal D2D.RenderTarget RenderTarget2D
        {
            get { return m_renderTarget2D; }
        }

        /// <summary>
        /// Gets the 2D graphics object.
        /// </summary>
        internal Graphics2D Graphics
        {
            get { return m_graphics2D; }
        }
    }
}