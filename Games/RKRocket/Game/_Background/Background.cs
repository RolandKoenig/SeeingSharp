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
using SeeingSharp.Util;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKRocket.Game
{
    public class Background : GameObject2D
    {
        #region Graphics Resources
        private SolidBrushResource m_backBrush;
        private StandardBitmapResource m_starBitmap;
        #endregion

        #region Logic
        private Random m_random;
        private List<Star> m_generatedStars;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Background"/> class.
        /// </summary>
        public Background()
        {
            m_backBrush = new SolidBrushResource(
                Color4.BlueColor.ChangeAlphaTo(0.01f));
            m_starBitmap = new StandardBitmapResource(
                new AssemblyResourceUriBuilder(
                    "RKRocket", true,
                    "Assets/Bitmaps/StarGray_128x128.png"));

            m_generatedStars = new List<Star>();
            m_random = new Random();
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        /// <param name="updateState">Current update state.</param>
        protected override void UpdateInternal(UpdateState updateState)
        {
            if((m_generatedStars.Count < Constants.GFX_BACKGROUND_MAX_STAR_COUNT) &&
               (m_random.Next(0, 100) < Constants.GFX_BACKGROUND_STAR_CREATE_PROPABILITY))
            {
                // Create the star object
                Star newStar = new Star(m_random.Next(60, 100) / 100f);
                newStar.Position = new Vector2(
                    (float)m_random.Next(10, (int)Constants.GFX_SCREEN_VPIXEL_WIDTH - 10),
                    -20f);
                newStar.Opacity = 0f;

                // Define the animation (move from top to bottom)
                this.BuildAnimationSequence(newStar)
                    .Move2DTo(
                        new Vector2(newStar.Position.X, Constants.GFX_SCREEN_VPIXEL_HEIGHT + 150f),
                        (float)m_random.Next(400, 450))
                    .ChangeOpacityTo(1f, TimeSpan.FromMilliseconds(250f))
                    .WaitForCondition(() => newStar.Position.Y > Constants.GFX_SCREEN_VPIXEL_HEIGHT - 150f)
                    .ChangeOpacityTo(0f, TimeSpan.FromMilliseconds(250f))
                    .WaitFinished()
                    .ApplyAsSecondary(() => m_generatedStars.Remove(newStar));

                // Register new star
                m_generatedStars.Add(newStar);
            }
        }

        /// <summary>
        /// Contains all 2D rendering logic for this object.
        /// </summary>
        protected override void OnRender_2DOverlay(RenderState renderState)
        {
            Graphics2D graphics = renderState.Graphics2D;

            // Draw the background rectangle
            graphics.FillRectangle(
                new RectangleF(0f, 0f, Constants.GFX_SCREEN_VPIXEL_WIDTH, Constants.GFX_SCREEN_VPIXEL_HEIGHT),
                m_backBrush);

            // Draw all stars
            foreach(Star actStar in m_generatedStars)
            {
                graphics.DrawBitmap(
                    m_starBitmap,
                    new RectangleF(
                        actStar.Position.X, actStar.Position.Y,
                        Constants.GFX_BACKGROUND_STAR_VPIXEL_WIDTH * actStar.Scaling,
                        Constants.GFX_BACKGROUND_STAR_VPIXEL_HEIGHT * actStar.Scaling),
                    opacity: 0.5f * actStar.Opacity,
                    interpolationMode: BitmapInterpolationMode.Linear);
            }
        }
    }
}
