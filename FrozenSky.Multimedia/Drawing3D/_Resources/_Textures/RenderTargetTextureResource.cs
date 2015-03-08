#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2014 Roland König (RolandK)

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Some namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace FrozenSky.Multimedia.Drawing3D
{
    internal class RenderTargetTextureResource : TextureResource
    {
        private int m_width;
        private int m_heigth;
        private bool m_antialiasingEnabled;
        private AntialiasingQualityLevel m_antialiasingQuality;
        private SharpDX.ViewportF m_viewportF;

        // Resources for direct3D 11 rendering
        private D3D11.Texture2D m_renderTargetDepth;
        private D3D11.RenderTargetView m_renderTargetView;
        private D3D11.ShaderResourceView m_shaderResourceView;
        private D3D11.DepthStencilView m_renderTargetDepthView;
        private D3D11.Texture2D m_renderTarget;
        private D3D11.Texture2D m_shaderResource;
        private bool m_shaderResourceCreated;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargetTextureResource" /> class.
        /// </summary>
        public RenderTargetTextureResource()
            : base()
        {
            m_width = -1;
            m_heigth = -1;
            m_viewportF = new SharpDX.ViewportF();
            m_shaderResourceCreated = false;
        }

        /// <summary>
        /// Applies the given size.
        /// </summary>
        /// <param name="renderState">The render state used for creating all resources.</param>
        /// <param name="viewSize">The view size to be used.</param>
        public void ApplySize(RenderState renderState)
        {
            ViewInformation viewInfo = renderState.ViewInformation;
            GraphicsViewConfiguration viewConfig = viewInfo.ViewConfiguration;

            Size2 currentViewSize = viewInfo.CurrentViewSize;
            bool currentAntialiasingEnabled = viewConfig.AntialiasingEnabled;
            AntialiasingQualityLevel currentAntialiasingQuality = viewConfig.AntialiasingQuality;

            if ((m_width != currentViewSize.Width) ||
                (m_heigth != currentViewSize.Height) ||
                (m_antialiasingEnabled != currentAntialiasingEnabled) ||
                (m_antialiasingQuality != currentAntialiasingQuality))
            {
                // Dispose old resources
                GraphicsHelper.SafeDispose(ref m_renderTargetView);
                GraphicsHelper.SafeDispose(ref m_shaderResourceView);
                GraphicsHelper.SafeDispose(ref m_renderTargetDepthView);
                GraphicsHelper.SafeDispose(ref m_renderTarget);
                GraphicsHelper.SafeDispose(ref m_renderTargetDepth);
                if (m_shaderResourceCreated) { GraphicsHelper.SafeDispose(ref m_shaderResource); }

                // Create texture resources
                m_renderTarget = GraphicsHelper.CreateRenderTargetTexture(
                    renderState.Device, currentViewSize.Width, currentViewSize.Height, renderState.ViewInformation.ViewConfiguration);
                m_shaderResource = m_renderTarget;
                if (renderState.ViewInformation.ViewConfiguration.AntialiasingEnabled)
                {
                    m_shaderResource = GraphicsHelper.CreateTexture(renderState.Device, currentViewSize.Width, currentViewSize.Height);
                    m_shaderResourceCreated = true;
                }
                else
                {
                    m_shaderResourceCreated = false;
                }
                m_renderTargetDepth = GraphicsHelper.CreateDepthBufferTexture(
                    renderState.Device, currentViewSize.Width, currentViewSize.Height, renderState.ViewInformation.ViewConfiguration);
                m_renderTargetView = new D3D11.RenderTargetView(renderState.Device.DeviceD3D11, m_renderTarget);
                m_renderTargetDepthView = new D3D11.DepthStencilView(renderState.Device.DeviceD3D11, m_renderTargetDepth);
                m_shaderResourceView = new D3D11.ShaderResourceView(renderState.Device.DeviceD3D11, m_shaderResource);

                // Remember values
                m_width = currentViewSize.Width;
                m_heigth = currentViewSize.Height;
                m_antialiasingEnabled = currentAntialiasingEnabled;
                m_antialiasingQuality = currentAntialiasingQuality;
                m_viewportF = renderState.Viewport;
            }
        }

        /// <summary>
        /// Pushes this render target on the given render state.
        /// </summary>
        /// <param name="renderState">The render state to push to.</param>
        /// <param name="mode"></param>
        internal void PushOnRenderState(RenderState renderState, PushRenderTargetMode mode)
        {
            switch(mode)
            {
                    // Push all buffers on render state
                case PushRenderTargetMode.Default:
                    renderState.PushRenderTarget(
                        new RenderTargets(m_renderTargetView, m_renderTargetDepthView),
                        m_viewportF, renderState.Camera, renderState.ViewInformation);
                    break;

                    // Push only render target on render state
                case PushRenderTargetMode.OvertakePreviousDepthBuffer:
                    D3D11.DepthStencilView depthStencilView = renderState.CurrentRenderTargets.DepthStencilBuffer;
                    if (depthStencilView == null) { throw new FrozenSkyGraphicsException("No previous single depth stencil buffer available!"); }

                    renderState.PushRenderTarget(
                        new RenderTargets(m_renderTargetView, m_renderTargetDepthView),
                        m_viewportF, renderState.Camera, renderState.ViewInformation);
                    break;

                default:
                    throw new FrozenSkyGraphicsException("Unable to push on render state: Mode " + mode + " not found!");
            }
        }

        /// <summary>
        /// Pops the render target from the given render state.
        /// </summary>
        /// <param name="renderState">The render state.</param>
        internal void PopFromRenderState(RenderState renderState)
        {
            renderState.PopRenderTarget();

            // Copy texture data when in antialiasing ode
            if (m_antialiasingEnabled)
            {
                renderState.Device.DeviceImmediateContextD3D11.ResolveSubresource(
                    m_renderTarget, 0, m_shaderResource, 0, GraphicsHelper.DEFAULT_TEXTURE_FORMAT);
            }
        }

        /// <summary>
        /// Loads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void LoadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
         
        }

        /// <summary>
        /// Unloads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void UnloadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            GraphicsHelper.SafeDispose(ref m_renderTargetDepthView);
            GraphicsHelper.SafeDispose(ref m_renderTargetView);
            GraphicsHelper.SafeDispose(ref m_shaderResourceView);
            GraphicsHelper.SafeDispose(ref m_renderTargetDepth);
            GraphicsHelper.SafeDispose(ref m_renderTarget);

            // Unload shader resource if it was created explecitely
            if(m_shaderResourceCreated)
            {
                GraphicsHelper.SafeDispose(ref m_shaderResource);
                m_shaderResourceCreated = false;
            }
        }

        /// <summary>
        /// Is the resource loaded?
        /// </summary>
        public override bool IsLoaded
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the texture itself.
        /// </summary>
        public override D3D11.Texture2D Texture
        {
            get { return m_renderTarget; }
        }

        /// <summary>
        /// Gets the shader resource view to the texture.
        /// </summary>
        public override D3D11.ShaderResourceView TextureView
        {
            get { return m_shaderResourceView; }
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
