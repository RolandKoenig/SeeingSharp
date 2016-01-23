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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Multimedia.Core;
using SeeingSharp;
using System.Numerics;
using SeeingSharp.Multimedia.Drawing2D;

namespace RKRocket.Game
{
    public class ForegroundEntity : GameObject2D
    {
        #region Graphics resources
        private StandardBitmapResource m_cloudBitmap;
        #endregion

        #region Runtime properties
        private bool m_isPaused;
        private float m_foregroundScaling;
        #endregion

        public ForegroundEntity()
        {
            m_cloudBitmap = GraphicsResources.Bitmap_Cloud;
        }

        protected override void OnRender_2DOverlay(RenderState renderState)
        {
            Graphics2D graphics = renderState.Graphics2D;

            if (m_isPaused)
            {
                // Draw the foreground rectangle
                using (graphics.BlockForLocalTransform_ReplacePrevious(Matrix3x2.Identity))
                {
                    RectangleF targetRectangle = new RectangleF(0f, 0f, graphics.ScreenWidth, graphics.ScreenHeight);
                    targetRectangle.Inflate(m_foregroundScaling, m_foregroundScaling);

                    graphics.DrawBitmap(
                        m_cloudBitmap,
                        targetRectangle,
                        0.5f + (m_foregroundScaling / 500f));
                }
            }   
        }

        protected override void UpdateInternal(SceneRelatedUpdateState updateState)
        {
            if (m_isPaused != updateState.IsPaused)
            {
                m_isPaused = updateState.IsPaused;

                // Start/Stop background scale animation
                if(m_isPaused)
                {
                    this.BuildAnimationSequence()
                        .ChangeFloatBy(
                            () => m_foregroundScaling,
                            (value) => m_foregroundScaling = value,
                            200f,
                            TimeSpan.FromSeconds(5.0))
                        .WaitFinished()
                        .WaitUntilTimePassed(TimeSpan.FromSeconds(1.5))
                        .WaitFinished()
                        .ChangeFloatBy(
                            () => m_foregroundScaling,
                            (value) => m_foregroundScaling = value,
                            -200f,
                            TimeSpan.FromSeconds(5.0))
                        .WaitFinished()
                        .WaitUntilTimePassed(TimeSpan.FromSeconds(1.5))
                        .ApplyAndRewind(ignorePause: true);
                }
                else
                {
                    base.AnimationHandler.CancelAnimations();
                    m_foregroundScaling = 0f;
                }
            }
        }

        protected override int RenderZOrder
        {
            get
            {
                return Constants.ZORDER_FOREGROUND;
            }
        }
    }
}
