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
using System.Reflection;
using System.Runtime.InteropServices;

//Some namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace FrozenSky.Multimedia.Drawing3D
{
    public class SpriteMaterialResource : MaterialResource
    {
        //Resource keys
        private static readonly NamedOrGenericKey RES_KEY_VERTEX_SHADER = GraphicsCore.GetNextGenericResourceKey();
        private static readonly NamedOrGenericKey RES_KEY_PIXEL_SHADER = GraphicsCore.GetNextGenericResourceKey();
        private static readonly NamedOrGenericKey RES_KEY_PIXEL_SHADER_BLUR = GraphicsCore.GetNextGenericResourceKey();
        private static readonly NamedOrGenericKey RES_KEY_PIXEL_SHADER_EDGE_RENDER = GraphicsCore.GetNextGenericResourceKey();

        //Some configuration
        private NamedOrGenericKey m_textureKey;

        //Resource members
        private D3D11.SamplerState m_samplerState;
        private TextureResource m_textureResource;
        private VertexShaderResource m_vertexShader;
        private PixelShaderResource m_pixelShader;
        private PixelShaderResource m_pixelShaderBlur;
        private PixelShaderResource m_pixelShaderEdgeRender;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteMaterialResource"/> class.
        /// </summary>
        /// <param name="textureKey">The name of the texture to be rendered.</param>
        public SpriteMaterialResource(NamedOrGenericKey textureKey)
        {
            m_textureKey = textureKey;
        }

        /// <summary>
        /// Loads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void LoadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            //Load all required shaders and constant buffers
            m_vertexShader = resources.GetResourceAndEnsureLoaded(
                RES_KEY_VERTEX_SHADER,
                () => GraphicsHelper.GetVertexShaderResource(device, "Sprite", "SpriteVertexShader"));
            m_pixelShader = resources.GetResourceAndEnsureLoaded(
                RES_KEY_PIXEL_SHADER,
                () => GraphicsHelper.GetPixelShaderResource(device, "Sprite", "SpritePixelShader"));
            m_pixelShaderBlur = resources.GetResourceAndEnsureLoaded(
                RES_KEY_PIXEL_SHADER_BLUR,
                () => GraphicsHelper.GetPixelShaderResource(device, "Sprite", "SpriteBlurPixelShader"));
            m_pixelShaderEdgeRender = resources.GetResourceAndEnsureLoaded(
                RES_KEY_PIXEL_SHADER_EDGE_RENDER,
                () => GraphicsHelper.GetPixelShaderResource(device, "Sprite", "SpriteEdgeDetectPixelShader"));

            //Load the texture if any configured.
            if (!m_textureKey.IsEmpty)
            {
                //Get texture resource
                m_textureResource = resources.GetResourceAndEnsureLoaded<TextureResource>(m_textureKey);
            }

            //samplerDesk.Filter = D3D11.Filter.Anisotropic;
            m_samplerState = GraphicsHelper.CreateDefaultTextureSampler(device);
        }

        /// <summary>
        /// Unloads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void UnloadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            m_samplerState = GraphicsHelper.DisposeObject(m_samplerState);

            m_vertexShader = null;
            m_pixelShader = null;
            m_textureResource = null;
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
        /// <exception cref="FrozenSkyGraphicsException">Effect  + this.Effect +  not supported!</exception>
        internal override void Apply(RenderState renderState, MaterialApplyInstancingMode instancingMode, MaterialResource previousMaterial)
        {
            D3D11.DeviceContext deviceContext = renderState.Device.DeviceImmediateContextD3D11;
            bool isResourceSameType =
                (previousMaterial != null) &&
                (previousMaterial.ResourceType == base.ResourceType);

            // Apply sampler
            if (!isResourceSameType)
            {
                deviceContext.PixelShader.SetSampler(0, m_samplerState);
            }

            // Set texture resource (if set)
            if (m_textureResource != null)
            {
                deviceContext.PixelShader.SetShaderResource(0, m_textureResource.TextureView);
            }
            else
            {
                deviceContext.PixelShader.SetShaderResource(0, null);
            }

            // Set shader resources
            switch (this.Effect)
            {
                case TexturePainterEffect.Blur:
                    deviceContext.VertexShader.Set(m_vertexShader.VertexShader);
                    deviceContext.PixelShader.Set(m_pixelShaderBlur.PixelShader);
                    break;

                case TexturePainterEffect.Standard:
                    deviceContext.VertexShader.Set(m_vertexShader.VertexShader);
                    deviceContext.PixelShader.Set(m_pixelShader.PixelShader);
                    break;

                case TexturePainterEffect.EdgeRendering:
                    deviceContext.VertexShader.Set(m_vertexShader.VertexShader);
                    deviceContext.PixelShader.Set(m_pixelShaderEdgeRender.PixelShader);
                    break;

                default:
                    throw new FrozenSkyGraphicsException("Effect " + this.Effect + " not supported!");
            }
        }

        /// <summary>
        /// Gets the key of the texture resource.
        /// </summary>
        public NamedOrGenericKey TextureKey
        {
            get { return m_textureKey; }
        }

        /// <summary>
        /// Is the resource loaded?
        /// </summary>
        public override bool IsLoaded
        {
            get { return m_vertexShader != null; }
        }

        public TexturePainterEffect Effect 
        {
            get;
            set; 
        }
    }
}
