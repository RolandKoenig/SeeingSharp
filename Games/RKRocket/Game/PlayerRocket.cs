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
    public class PlayerRocket : GameObject2D
    {
        #region Resources
        private StandardBitmapResource m_playerBitmap;
        #endregion

        #region State
        private float m_xPos;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerRocket"/> class.
        /// </summary>
        public PlayerRocket()
        {
            m_xPos = Constants.GFX_SCREEN_VPIXEL_WIDTH / 2f;

            m_playerBitmap = GraphicsResources.Bitmap_Player;
        }

        protected override void UpdateInternal(SceneRelatedUpdateState updateState)
        {
            // Get input states
            MouseOrPointerState mouseState = updateState.MouseOrPointer;

            // Set x location depending on primary mouse location
            float newXPos = Constants.GFX_SCREEN_VPIXEL_WIDTH * mouseState.PositionRelative.X;
            if(newXPos < 50f) { newXPos = 50f; }
            if(newXPos > Constants.GFX_SCREEN_VPIXEL_WIDTH - 50) { newXPos = Constants.GFX_SCREEN_VPIXEL_WIDTH - 50f; }
            m_xPos = newXPos;

            // Create projectiles on mouse hit
            if(mouseState.IsButtonHit(MouseButton.Left))
            {
                Projectile newProjectile = new Projectile(new Vector2(
                    m_xPos, Constants.GFX_ROCKET_VPIXEL_Y_CENTER - Constants.GFX_ROCKET_VPIXEL_HEIGHT / 2f));
                base.Scene.ManipulateSceneAsync((manipulator) => manipulator.Add(newProjectile))
                    .FireAndForget();
            }
        }

        /// <summary>
        /// Perform Direct2D rendering.
        /// </summary>
        /// <param name="renderState">The current render state.</param>
        protected override void OnRender_2DOverlay(RenderState renderState)
        {
            Graphics2D graphics = renderState.Graphics2D;

            RectangleF destRectangle = new RectangleF(
                m_xPos - (Constants.GFX_ROCKET_VPIXEL_WIDTH / 2f),
                Constants.GFX_ROCKET_VPIXEL_Y_CENTER - (Constants.GFX_ROCKET_VPIXEL_HEIGHT / 2f),
                Constants.GFX_ROCKET_VPIXEL_WIDTH,
                Constants.GFX_ROCKET_VPIXEL_HEIGHT);

            graphics.DrawBitmap(
                m_playerBitmap,
                destRectangle,
                interpolationMode: BitmapInterpolationMode.Linear);
        }
    }
}
