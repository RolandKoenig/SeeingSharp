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
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Some namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace FrozenSky.Multimedia.Drawing3D
{
    public class FocusPostprocessEffectResource : PostprocessEffectResource
    {
        // Resource keys
        private NamedOrGenericKey KEY_MATERIAL = GraphicsCore.GetNextGenericResourceKey();
        private NamedOrGenericKey KEY_RENDER_TARGET = GraphicsCore.GetNextGenericResourceKey();

        // Configuration
        private bool m_forceSimpleMethod;

        // Resources
        private SingleForcedColorMaterialResource m_singleForcedColor;
        private RenderTargetTextureResource m_renderTarget;
        private TexturePainterHelper m_texturePainter;
        private DefaultResources m_defaultResources;

        /// <summary>
        /// Initializes a new instance of the <see cref="FocusPostprocessEffectResource" /> class.
        /// </summary>
        public FocusPostprocessEffectResource()
            : this(false)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FocusPostprocessEffectResource"/> class.
        /// </summary>
        /// <param name="forceSimpleMethod">Force simple mode. Default to false.</param>
        public FocusPostprocessEffectResource(bool forceSimpleMethod)
        {
            m_texturePainter = new TexturePainterHelper(KEY_RENDER_TARGET);
            m_forceSimpleMethod = forceSimpleMethod;
        }

        /// <summary>
        /// Loads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void LoadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            m_singleForcedColor = resources.GetResourceAndEnsureLoaded<SingleForcedColorMaterialResource>(
                KEY_MATERIAL,
                () => new SingleForcedColorMaterialResource());
            m_renderTarget = resources.GetResourceAndEnsureLoaded<RenderTargetTextureResource>(
                KEY_RENDER_TARGET,
                () => new RenderTargetTextureResource());
            m_defaultResources = resources.GetResourceAndEnsureLoaded<DefaultResources>(DefaultResources.RESOURCE_KEY);

            m_texturePainter.LoadResources(resources);
        }

        /// <summary>
        /// Unloads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override void UnloadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            m_singleForcedColor = null;
            m_renderTarget = null;
            m_renderTarget.UnloadResource();
            m_texturePainter.UnloadResources();
            m_defaultResources = null;
        }

        /// <summary>
        /// Notifies that rendering begins.
        /// </summary>
        /// <param name="renderState">The current render state.</param>
        /// <param name="passID">The ID of the current pass (starting with 0)</param>
        internal override void NotifyBeforeRender(RenderState renderState, int passID)
        {
            if (renderState.Device.IsHighDetailSupported && (!m_forceSimpleMethod))
            {
                switch (passID)
                {
                    //******************************
                    // 1. Pass: Draw all pixels that ly behind other already rendered elements
                    case 0:
                        // Force the single color material
                        renderState.ForceMaterial(m_singleForcedColor);

                        // Apply current render target size an push render target texture on current rendering stack
                        m_renderTarget.ApplySize(renderState);
                        m_renderTarget.PushOnRenderState(renderState, PushRenderTargetMode.OvertakePreviousDepthBuffer);

                        // Clear current render target
                        renderState.ClearCurrentColorBuffer(new Color(1f, 1f, 1f, 0f));

                        // Configure stencil state (invert z logic, disable z writes)
                        renderState.Device.DeviceImmediateContextD3D11.OutputMerger.DepthStencilState = m_defaultResources.DepthStencilStateInvertedZTest;
                        break;

                    //******************************
                    // 2. Pass: Draw all visible pixels with some blur effect
                    case 1:
                        // Force the single color material
                        renderState.ForceMaterial(m_singleForcedColor);

                        // Push render target texture on current rendering stack again
                        m_renderTarget.PushOnRenderState(renderState, PushRenderTargetMode.OvertakePreviousDepthBuffer);

                        // Clear current render target
                        renderState.ClearCurrentColorBuffer(new Color(1f, 1f, 1f, 0f));

                        // Change raster state
                        renderState.Device.DeviceImmediateContextD3D11.Rasterizer.State = m_defaultResources.RasterStateBiased;
                        break;
                }
            }
            else
            {
                renderState.ForceMaterial(m_singleForcedColor);

                // Change raster state
                renderState.Device.DeviceImmediateContextD3D11.Rasterizer.State = m_defaultResources.RasterStateBiased;
            }
        }

        /// <summary>
        /// Notifies that rendering has finished.
        /// </summary>
        /// <param name="renderState">The current render state.</param>
        /// <param name="passID">The ID of the current pass (starting with 0)</param>
        /// <returns>
        /// True, if rendering should continue with next pass. False if postprocess effect is finished.
        /// </returns>
        internal override bool NotifyAfterRender(RenderState renderState, int passID)
        {
            D3D11.DeviceContext deviceContext = renderState.Device.DeviceImmediateContextD3D11;

            if (renderState.Device.IsHighDetailSupported && (!m_forceSimpleMethod))
            {
                // Reset settings made on render state (needed for all passes)
                deviceContext.Rasterizer.State = m_defaultResources.RasterStateDefault;
                deviceContext.OutputMerger.DepthStencilState = m_defaultResources.DepthStencilStateDefault;
                renderState.ForceMaterial(null);

                // Reset render target (needed for all passes)
                m_renderTarget.PopFromRenderState(renderState);

                // Render result of current pass to the main render target
                switch (passID)
                {
                    case 0:
                        m_texturePainter.RenderBlured(renderState, 0.1f, 0.0f, 0f);
                        return true;

                    case 1:
                        m_texturePainter.RenderBlured(renderState, 0.5f, 0.8f, 0f);
                        return false;
                }
            }
            else
            {
                // Reset changes from before
                deviceContext.Rasterizer.State = m_defaultResources.RasterStateDefault;
                renderState.ForceMaterial(null);

                // Now we ware finished
                return false;
            }

            return false;
        }

        /// <summary>
        /// Is the resource loaded?
        /// </summary>
        public override bool IsLoaded
        {
            get 
            {
                return (m_renderTarget != null) &&
                       (m_singleForcedColor != null) &&
                       (m_texturePainter.IsLoaded);
            }
        }
    }
}
