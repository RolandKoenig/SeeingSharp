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
using System.IO;

//Some namespace mappings
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;
using TKGFX = SharpDX.Toolkit.Graphics;

#if !WINDOWS_PHONE
using WIC = SharpDX.WIC;
using System.Collections.Generic;
#endif

namespace FrozenSky.Multimedia.Drawing3D
{
    public class StandardTextureResource : TextureResource
    {
        // Given configuration
        private ResourceSource m_resourceSourceHighQuality;
        private ResourceSource m_resourceSourceLowQuality;

        // Loaded resources
        private D3D11.Texture2D m_texture;
        private D3D11.ShaderResourceView m_textureView;

        private bool m_isCubeTexture;
        private bool m_isRenderTarget;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardTextureResource" /> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public StandardTextureResource(ResourceSource textureSource)
        {
            m_resourceSourceHighQuality = textureSource;
            m_resourceSourceLowQuality = textureSource;
        }

        internal StandardTextureResource(byte[] textureBytes, int width, int height, DXGI.Format textureFormat)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardTextureResource" /> class.
        /// </summary>
        /// <param name="highQualityTextureSource">High quality version of the texture.</param>
        /// <param name="lowQualityTextureSource">Low quality version of the texture.</param>
        public StandardTextureResource(ResourceSource highQualityTextureSource, ResourceSource lowQualityTextureSource)
        {
            m_resourceSourceHighQuality = highQualityTextureSource;
            m_resourceSourceLowQuality = lowQualityTextureSource;
        }

        /// <summary>
        /// Loads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        /// <exception cref="FrozenSkyGraphicsException"></exception>
        protected override void LoadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            // Select source texture
            ResourceSource source = m_resourceSourceLowQuality;
            if (device.Configuration.TextureQuality == TextureQuality.Hight) { source = m_resourceSourceHighQuality; }

            // Load the texture
            try
            {
                using (Stream inStream = source.OpenInputStream())
                {
                    TKGFX.Image rawImage = TKGFX.Image.Load(inStream);
                    if (rawImage == null) { throw new FrozenSkyException("Unable to load image!"); }

                    m_texture = GraphicsHelper.CreateTexture(device, rawImage);
                }
            }
            catch(Exception)
            {
#if DESKTOP
                // Load default texture from a bitmap
                m_texture = GraphicsHelper.LoadTextureFromBitmap(device, Properties.Resources.Blank_16x16);
#else
                throw;
#endif
            }

            // Create view for shaders
            m_textureView = new D3D11.ShaderResourceView(device.DeviceD3D11, m_texture);

            // Some checking..
            m_isCubeTexture =
                (m_texture.Description.ArraySize == 6) &&
                ((m_texture.Description.OptionFlags & D3D11.ResourceOptionFlags.TextureCube) == D3D11.ResourceOptionFlags.TextureCube);
            m_isRenderTarget =
                (m_texture.Description.BindFlags & D3D11.BindFlags.RenderTarget) == D3D11.BindFlags.RenderTarget;
        }

        /// <summary>
        /// Unloads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void UnloadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            m_textureView = GraphicsHelper.DisposeObject(m_textureView);
            m_texture = GraphicsHelper.DisposeObject(m_texture);

            m_isCubeTexture = false;
            m_isRenderTarget = false;
        }

        /// <summary>
        /// Gets the texture object.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
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
        /// Is the object loaded correctly?
        /// </summary>
        public override bool IsLoaded
        {
            get { return m_textureView != null; }
        }

        /// <summary>
        /// Is this texture a cube texture?
        /// </summary>
        public bool IsCubeTexture
        {
            get { return m_isCubeTexture; }
        }

        /// <summary>
        /// Is this texture a render target texture?
        /// </summary>
        public bool IsRenderTargetTexture
        {
            get { return m_isRenderTarget; }
        }

        /// <summary>
        /// Gets the size of the texture array.
        /// 1 for normal textures.
        /// 6 for cubemap textures.
        /// </summary>
        public override int ArraySize
        {
            get { return m_texture.Description.ArraySize; }
        }
    }
}
