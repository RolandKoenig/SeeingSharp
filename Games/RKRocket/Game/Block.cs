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
using SeeingSharp;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing2D;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RKRocket.Game
{
    public class Block : GameObject2D, IAnimatableObjectPosition2D
    {
        #region Graphics Resources
        private StandardBitmapResource m_blockBitmap;
        #endregion

        #region Logic
        private Vector2 m_position;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Block"/> class.
        /// </summary>
        public Block()
        {
            m_blockBitmap = GraphicsResources.Bitmap_Blocks[0];
        }

        protected override void UpdateInternal(UpdateState updateState)
        {

        }

        /// <summary>
        /// Contains all 2D rendering logic for this object.
        /// </summary>
        /// <param name="renderState">The current state of the renderer.</param>
        protected override void OnRender_2DOverlay(RenderState renderState)
        {
            Graphics2D graphics = renderState.Graphics2D;

            RectangleF destRectangle = new RectangleF(
                m_position.X - Constants.BLOCK_VPIXEL_WIDTH / 2f,
                m_position.Y - Constants.BLOCK_VPIXEL_HEIGHT / 2f,
                Constants.BLOCK_VPIXEL_WIDTH,
                Constants.BLOCK_VPIXEL_HEIGHT);
            graphics.DrawBitmap(
                m_blockBitmap, destRectangle,
                opacity: Constants.BLOCK_OPACITY_NORMAL,
                interpolationMode: BitmapInterpolationMode.Linear);
        }

        public Vector2 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }
    }
}
