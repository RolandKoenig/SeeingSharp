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
using System.Numerics;
using SeeingSharp;
using SeeingSharp.Util;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing2D;

namespace RKRocket.Game
{
    public class FragmentEntity : GameObject2D, IAnimatableObjectPosition2D
    {
        #region Resources
        private BrushResource m_fragmentBrush;
        #endregion

        #region Logic
        private Vector2 m_position;
        private Vector2 m_direction;
        private float m_speed;
        private TimeSpan m_totalLiveTime;
        #endregion

        public FragmentEntity(Vector2 initialPosition, Vector2 direction, float speed)
        {
            m_fragmentBrush = GraphicsResources.Brush_Fragment;

            m_position = initialPosition;
            m_direction = direction;
            m_speed = speed;
            m_totalLiveTime = TimeSpan.Zero;
        }

        /// <summary>
        /// Updates the fragments position.
        /// </summary>
        /// <param name="updateState">State of the update.</param>
        protected override void UpdateInternal(SceneRelatedUpdateState updateState)
        {
            // Check whether we are finished
            m_totalLiveTime = m_totalLiveTime + updateState.UpdateTime;
            if(m_totalLiveTime >= Constants.FRAGMENT_LIVETIME)
            {
                base.Scene.ManipulateSceneAsync(maniupulator => maniupulator.Remove(this))
                    .FireAndForget();
                return;
            }

            // Update location
            m_position = 
                m_position + 
                m_direction * (float)(m_speed * updateState.UpdateTime.TotalSeconds);
        }

        /// <summary>
        /// Renders the fragment.
        /// </summary>
        /// <param name="renderState"></param>
        protected override void OnRender_2DOverlay(RenderState renderState)
        {
            Graphics2D graphics = renderState.Graphics2D;

            RectangleF destRectangle = new RectangleF(
                m_position.X - Constants.FRAGMENT_VPIXEL_WIDTH / 2f,
                m_position.Y - Constants.FRAGMENT_VPIXEL_HEIGHT / 2f,
                Constants.FRAGMENT_VPIXEL_WIDTH,
                Constants.FRAGMENT_VPIXEL_HEIGHT);

        }

        public Vector2 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }
    }
}
