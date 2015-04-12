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

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrozenSky.Util;
using FrozenSky.Multimedia.Core;

// Some namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace FrozenSky.Multimedia.Drawing3D
{
    public class SingleForcedColorMaterialResource : MaterialResource
    {
        // Static resource keys
        private static readonly NamedOrGenericKey RES_KEY_VERTEX_SHADER = GraphicsCore.GetNextGenericResourceKey();
        private static readonly NamedOrGenericKey RES_KEY_PIXEL_SHADER = GraphicsCore.GetNextGenericResourceKey();

        // Resource members
        private VertexShaderResource m_vertexShader;
        private PixelShaderResource m_pixelShader;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleForcedColorMaterialResource" /> class.
        /// </summary>
        public SingleForcedColorMaterialResource()
        {

        }

        /// <summary>
        /// Generates the requested input layout.
        /// </summary>
        /// <param name="device">The device on which to create the input layout.</param>
        /// <param name="inputElements">An array of InputElements describing vertex input structure.</param>
        /// <param name="instancingMode">Instancing mode for which to generate the input layout for.</param>
        internal override D3D11.InputLayout GenerateInputLayout(EngineDevice device, D3D11.InputElement[] inputElements, MaterialApplyInstancingMode instancingMode)
        {
            switch (instancingMode)
            {
                case MaterialApplyInstancingMode.SingleObject:
                    return new D3D11.InputLayout(device.DeviceD3D11, m_vertexShader.ShaderBytecode, inputElements);

                default:
                    throw new FrozenSkyGraphicsException(this.GetType() + " does not support " + typeof(MaterialApplyInstancingMode) + "." + instancingMode + "!");
            }
        }

        /// <summary>
        /// Applies the material to the given render state.
        /// </summary>
        /// <param name="renderState">Current render state</param>
        /// <param name="instancingMode">The instancing mode for which to apply the material.</param>
        /// <param name="previousMaterial">The previously applied material.</param>
        internal override void Apply(RenderState renderState, MaterialApplyInstancingMode instancingMode, MaterialResource previousMaterial)
        {
            D3D11.DeviceContext deviceContext = renderState.Device.DeviceImmediateContextD3D11;

            // Apply constants and shader resources
            deviceContext.VertexShader.Set(m_vertexShader.VertexShader);
            deviceContext.PixelShader.Set(m_pixelShader.PixelShader);
        }

        /// <summary>
        /// Loads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void LoadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            // Load all required shaders and constant buffers
            m_vertexShader = resources.GetResourceAndEnsureLoaded(
                RES_KEY_VERTEX_SHADER,
                () => GraphicsHelper.GetVertexShaderResource(device, "Common", "SingleForcedColorVertexShader"));
            m_pixelShader = resources.GetResourceAndEnsureLoaded(
                RES_KEY_PIXEL_SHADER,
                () => GraphicsHelper.GetPixelShaderResource(device, "Common", "SingleForcedColorPixelShader"));
        }

        /// <summary>
        /// Unloads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void UnloadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            m_pixelShader = null;
            m_vertexShader = null;
        }

        public override bool IsLoaded
        {
            get
            {
                return (m_vertexShader != null) &&
                       (m_pixelShader != null);
            }
        }
    }
}
