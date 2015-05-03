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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Multimedia.Core;

//Some namespace mappings
using TKGFX = SharpDX.Toolkit.Graphics;
using D3D11 = SharpDX.Direct3D11;
using WIC = SharpDX.WIC;

namespace SeeingSharp.Multimedia.Drawing3D
{
    internal class ChangeableTextureResource : TextureResource
    {
        // Loaded resources
        private D3D11.Texture2D m_texture;
        private D3D11.ShaderResourceView m_textureView;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardTextureResource" /> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        internal ChangeableTextureResource()
        {

        }

        /// <summary>
        /// Loads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        /// <exception cref="GraphicsEngineException"></exception>
        protected override void LoadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {

        }

        /// <summary>
        /// Unloads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void UnloadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {

        }

        internal void SetContents(D3D11.Texture2D texture, D3D11.ShaderResourceView textureView)
        {
            m_texture = texture;
            m_textureView = textureView;
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
            get { return true; }
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
