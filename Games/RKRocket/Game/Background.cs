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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKRocket.Game
{
    public class Background : GameObject2D
    {
        private SolidBrushResource m_blackBrush;

        /// <summary>
        /// Initializes a new instance of the <see cref="Background"/> class.
        /// </summary>
        public Background()
        {
            m_blackBrush = new SolidBrushResource(
                Color4.Black);
        }

        /// <summary>
        /// Contains all 2D rendering logic for this object.
        /// </summary>
        protected override void OnRender_2DOverlay(RenderState renderState)
        {
            Graphics2D graphics = renderState.Graphics2D;

            graphics.FillRectangle(
                new RectangleF(0f, 0f, Constants.GFX_SCREEN_VPIXEL_WIDTH, Constants.GFX_SCREEN_VPIXEL_HEIGHT),
                m_blackBrush);
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        /// <param name="updateState">Current update state.</param>
        protected override void UpdateInternal(UpdateState updateState)
        {

        }
    }
}
