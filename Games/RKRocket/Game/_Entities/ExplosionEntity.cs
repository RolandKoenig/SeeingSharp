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
using System.Numerics;
using SeeingSharp.Multimedia.Drawing2D;
using SeeingSharp;

namespace RKRocket.Game
{
    public class ExplosionEntity : GameObject2D
    {
        #region Graphics resources
        private StandardBitmapResource m_explBitmap;
        #endregion

        #region Logic
        private Vector2 m_position;
        private int m_actFrameIndex;
        #endregion

        public ExplosionEntity(Vector2 position)
        {
            m_position = position;

            m_explBitmap = GraphicsResources.Bitmap_Explosion;

            this.BuildAnimationSequence()
                .ChangeIntBy(
                    () => m_actFrameIndex,
                    (actValue) => m_actFrameIndex = actValue,
                    Constants.SIM_EXPLOSION_FRAME_COUNT,
                    TimeSpan.FromMilliseconds(Constants.SIM_EXPLOSION_DURATION_MS))
                .WaitFinished()
                .RemoveObjectFromScene()
                .Apply();
        }

        protected override void UpdateInternal(SceneRelatedUpdateState updateState)
        {

        }

        protected override void OnRender_2DOverlay(RenderState renderState)
        {
            RectangleF destinationRect = new RectangleF(
                m_position.X - Constants.GFX_EXPLOSION_VPIXEL_WIDTH / 2f,
                m_position.Y - Constants.GFX_EXPLOSION_VPIXEL_HEIGHT / 2f,
                Constants.GFX_EXPLOSION_VPIXEL_WIDTH, Constants.GFX_EXPLOSION_VPIXEL_HEIGHT);
            renderState.Graphics2D.DrawBitmap(
                m_explBitmap,
                destinationRect,
                frameIndex: m_actFrameIndex);
        }
    }
}
