#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing2D;
using SeeingSharp.Multimedia.Input;
using SeeingSharp.Util;
using SeeingSharp;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKRocket.Game
{
    public class TargetCrossEntity : GameObject2D
    {
        #region Resources
        private StandardBitmapResource m_targetCrossBitmap;
        #endregion

        #region Game properties
        private Vector2 m_relativeMousePos;
        private bool m_mouseAvailable;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerRocketEntity"/> class.
        /// </summary>
        public TargetCrossEntity()
        {
            m_targetCrossBitmap = GraphicsResources.Bitmap_TargetCross;
        }

        /// <summary>
        /// Updates this object.
        /// </summary>
        /// <param name="updateState">State of the update.</param>
        protected override void UpdateInternal(SceneRelatedUpdateState updateState)
        {
            MouseOrPointerState mouseState = updateState.DefaultMouseOrPointer;

            m_mouseAvailable =
                (mouseState != MouseOrPointerState.Dummy) &&
                (mouseState.IsMouseInsideView);
            if(m_mouseAvailable)
            {
                m_relativeMousePos = mouseState.PositionRelative;
            }
        }

        /// <summary>
        /// Perform Direct2D rendering.
        /// </summary>
        /// <param name="renderState">The current render state.</param>
        protected override void OnRender_2DOverlay(RenderState renderState)
        {
            if (m_mouseAvailable)
            {
                Graphics2D graphics = renderState.Graphics2D;

                Vector2 targetLocation = new Vector2(
                    graphics.ScreenSize.Width * m_relativeMousePos.X,
                    graphics.ScreenSize.Height * m_relativeMousePos.Y);
                graphics.DrawBitmap(
                    m_targetCrossBitmap,
                    new RectangleF(
                        targetLocation.X - Constants.GFX_TARGET_CROSS_WIDTH / 2f,
                        targetLocation.Y - Constants.GFX_TARGET_CROSS_HEIGHT / 2f,
                        Constants.GFX_TARGET_CROSS_WIDTH,
                        Constants.GFX_TARGET_CROSS_HEIGHT),
                    opacity: 0.5f);
            }
        }

        protected override int RenderZOrder
        {
            get
            {
                return Constants.ZORDER_TARGET_CROSS;
            }
        }
    }
}
