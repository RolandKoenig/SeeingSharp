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

#if DESKTOP
using SeeingSharp.Multimedia.Core;
using System.Drawing;

//Some namespace mappings
using D3D11 = SharpDX.Direct3D11;
using GDI = System.Drawing;

namespace SeeingSharp.Multimedia.Drawing3D
{
    public class DrawingBrushTextureResource : TextureResource
    {
        //Direct3D 11 Resources
        private D3D11.Texture2D m_texture;
        private D3D11.ShaderResourceView m_textureView;

        //Standard members
        private Brush m_drawingBrush;
        private int m_width;
        private int m_height;

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingBrushTextureResource"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="drawingBrush">The drawing brush.</param>
        public DrawingBrushTextureResource(Brush drawingBrush)
            : this(drawingBrush, 128, 128)
        {
  
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingBrushTextureResource"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="drawingBrush">The drawing brush.</param>
        /// <param name="height">Width of the texture.</param>
        /// <param name="width">Height of the texture.</param>
        public DrawingBrushTextureResource(Brush drawingBrush, int width, int height)
        {
            m_drawingBrush = drawingBrush;
            m_width = width;
            m_height = height;
        }

        /// <summary>
        /// Loads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void LoadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            using (Bitmap drawingBitmap = new Bitmap(m_width, m_height))
            {
                using (GDI.Graphics graphics = GDI.Graphics.FromImage(drawingBitmap))
                {
                    graphics.FillRectangle(
                        m_drawingBrush,
                        new GDI.Rectangle(0, 0, m_width, m_height));
                    graphics.Dispose();
                }

                m_texture = GraphicsHelper.LoadTextureFromBitmap(device, drawingBitmap);
                m_textureView = new D3D11.ShaderResourceView(device.DeviceD3D11, m_texture);
            }
        }

        /// <summary>
        /// Unloads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void UnloadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            m_textureView = GraphicsHelper.DisposeObject(m_textureView);
            m_texture = GraphicsHelper.DisposeObject(m_texture);
        }

        /// <summary>
        /// Is the resource loaded?
        /// </summary>
        /// <value></value>
        public override bool IsLoaded
        {
            get { return (m_texture != null) && (m_textureView != null); }
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
            get { return m_texture.Description.ArraySize; }
        }
    }
}
#endif