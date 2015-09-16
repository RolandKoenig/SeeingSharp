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
using SeeingSharp.Util;
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
    public class ProjectileEntity : GameObject2D
    {
        #region Resources
        private StandardBitmapResource m_bitmapProjectile;
        #endregion

        #region State
        private Vector2 m_currentLocation;
        private float m_currentSpeed;
        private bool m_relevantForCollisionSystem;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectileEntity"/> class.
        /// </summary>
        public ProjectileEntity(Vector2 startLocation)
        {
            m_bitmapProjectile = GraphicsResources.Bitmap_Projectile;
            m_currentLocation = startLocation;
            m_currentSpeed = Constants.SIM_PROJECTILE_SPEED;
            m_relevantForCollisionSystem = true;
        }

        /// <summary>
        /// Gets the bounds of this object for the collision system.
        /// </summary>
        public BoundingSphere GetBoundsForCollisionSystem()
        {
            return new BoundingSphere(
                new Vector3(m_currentLocation.X, 0f, m_currentLocation.Y),
                Math.Max(Constants.GFX_PROJECTILE_VPIXEL_WIDTH, Constants.GFX_PROJECTILE_VPIXEL_HEIGHT) * 0.45f);
        }

        /// <summary>
        /// Updates the projectile.
        /// </summary>
        /// <param name="updateState">State of the update.</param>
        protected override void UpdateInternal(SceneRelatedUpdateState updateState)
        {
            float updateTimeSeconds = (float)updateState.UpdateTime.TotalSeconds;

            // Calculate moving distance and next speed value
            float movingDistance = 
                0.5f * Constants.SIM_PROJECTILE_BRAKE_RETARDATION * (float)Math.Pow(updateTimeSeconds, 2.0f) +
                m_currentSpeed * updateTimeSeconds;
            m_currentSpeed = Constants.SIM_PROJECTILE_BRAKE_RETARDATION * updateTimeSeconds + m_currentSpeed;

            // Update projectile location
            m_currentLocation.Y = m_currentLocation.Y + movingDistance;

            // Delete the projectile if we are out of the screen area
            if(m_currentLocation.Y > Constants.GFX_SCREEN_VPIXEL_HEIGHT + 100f)
            {
                base.Scene.ManipulateSceneAsync((manipulator) => manipulator.Remove(this))
                    .FireAndForget(); ;
            }
        }

        /// <summary>
        /// Contains all 2D rendering logic for this object.
        /// </summary>
        /// <param name="renderState">The current state of the renderer.</param>
        protected override void OnRender_2DOverlay(RenderState renderState)
        {
            Graphics2D graphics = renderState.Graphics2D;

            RectangleF destRectangle = new RectangleF(
                m_currentLocation.X - (Constants.GFX_PROJECTILE_VPIXEL_WIDTH / 2f),
                m_currentLocation.Y - (Constants.GFX_PROJECTILE_VPIXEL_HEIGHT / 2f),
                Constants.GFX_PROJECTILE_VPIXEL_WIDTH,
                Constants.GFX_PROJECTILE_VPIXEL_HEIGHT);

            float transparencyLevel = 1f - Math.Min(Math.Max(m_currentLocation.Y - Constants.GFX_SCREEN_VPIXEL_HEIGHT, 0f) / 100f, 1f);
            graphics.DrawBitmap(
                m_bitmapProjectile,
                destRectangle,
                opacity: transparencyLevel,
                interpolationMode: BitmapInterpolationMode.Linear);
        }

        /// <summary>
        /// The CollisionSystem notifies us that a projectile collided with a block.
        /// </summary>
        private void OnMessage_Received(MessageCollisionProjectileToBlockDetected message)
        {
            if(message.Projectile != this) { return; }

            m_currentSpeed = Constants.SIM_PROJECTILE_SPEED_AFTER_COLLISION;
            m_relevantForCollisionSystem = false;
        }

        /// <summary>
        /// Gets the current position of the projectile.
        /// </summary>
        public Vector2 Position
        {
            get { return m_currentLocation; }
        }

        public bool IsRelevantForCollisionSystem
        {
            get { return m_relevantForCollisionSystem; }
        }
    }
}
