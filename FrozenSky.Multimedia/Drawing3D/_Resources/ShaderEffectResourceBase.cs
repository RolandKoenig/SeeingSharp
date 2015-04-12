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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Some namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace FrozenSky.Multimedia.Drawing3D
{
    public abstract class ShaderEffectResourceBase : Resource
    {
        private static readonly NamedOrGenericKey RES_KEY_VERTEX_SHADER = GraphicsCore.GetNextGenericResourceKey();

        private VertexShaderResource m_vertexShader;
        private DefaultResources m_defaultResources;

        /// <summary>
        /// Applies alpha based sprite rendering.
        /// </summary>
        /// <param name="deviceContext">The target device context.</param>
        protected void ApplySpriteRendering(D3D11.DeviceContext deviceContext)
        {
            deviceContext.VertexShader.Set(m_vertexShader.VertexShader);

            deviceContext.OutputMerger.DepthStencilState = m_defaultResources.DepthStencilStateAllwaysPassDepth;
        }

        /// <summary>
        /// Discards alpha based sprite rendering.
        /// </summary>
        /// <param name="deviceContext">The target device context.</param>
        protected void DiscardSpriteRendering(D3D11.DeviceContext deviceContext)
        {
            deviceContext.OutputMerger.DepthStencilState = m_defaultResources.DepthStencilStateDefault;
        }

        /// <summary>
        /// Applies alpha based sprite rendering.
        /// </summary>
        /// <param name="deviceContext">The target device context.</param>
        protected void ApplyAlphaBasedSpriteRendering(D3D11.DeviceContext deviceContext)
        {
            deviceContext.VertexShader.Set(m_vertexShader.VertexShader);

            deviceContext.OutputMerger.DepthStencilState = m_defaultResources.DepthStencilStateAllwaysPassDepth;
            deviceContext.OutputMerger.BlendState = m_defaultResources.AlphaBlendingBlendState;
        }

        /// <summary>
        /// Discards alpha based sprite rendering.
        /// </summary>
        /// <param name="deviceContext">The target device context.</param>
        protected void DiscardAlphaBasedSpriteRendering(D3D11.DeviceContext deviceContext)
        {
            deviceContext.OutputMerger.BlendState = m_defaultResources.DefaultBlendState;
            deviceContext.OutputMerger.DepthStencilState = m_defaultResources.DepthStencilStateDefault;
        }

        /// <summary>
        /// Loads the resource.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void LoadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            m_defaultResources = resources.DefaultResources;
            m_vertexShader = resources.GetResourceAndEnsureLoaded(
                RES_KEY_VERTEX_SHADER,
                () => GraphicsHelper.GetVertexShaderResource(device, "Postprocessing", "PostprocessVertexShader"));
        }

        /// <summary>
        /// Unloads the resource.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void UnloadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            m_defaultResources = null;
            m_vertexShader = null;
        }
    }
}
