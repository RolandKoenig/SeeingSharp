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
    public class BlockEntity : GameObject2D, IAnimatableObjectPosition2D
    {
        #region Graphics Resources
        private StandardBitmapResource m_blockBitmap;
        #endregion

        #region Logic
        private Vector2 m_position;
        private bool m_isLeaving;
        private float m_opacity;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockEntity"/> class.
        /// </summary>
        public BlockEntity()
        {
            m_blockBitmap = GraphicsResources.Bitmap_Blocks[0];
            m_opacity = Constants.BLOCK_OPACITY_NORMAL;
        }

        /// <summary>
        /// Gets the bounds of this object for the collision system.
        /// </summary>
        public BoundingBox GetBoundsForCollisionSystem()
        {
            // 
            Vector3 start = new Vector3(
                m_position.X - Constants.BLOCK_VPIXEL_WIDTH / 2f,
                1f,
                m_position.Y - Constants.BLOCK_VPIXEL_HEIGHT / 2f);
            Vector3 size = new Vector3(
                Constants.BLOCK_VPIXEL_WIDTH,
                1f,
                Constants.BLOCK_VPIXEL_HEIGHT);

            start = start + size * 0.1f;
            size = size * 0.8f;

            return new BoundingBox(start, start + size);
        }

        protected override void UpdateInternal(SceneRelatedUpdateState updateState)
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
                opacity: m_opacity,
                interpolationMode: BitmapInterpolationMode.Linear);
        }

        /// <summary>
        /// The CollisionSystem notifies us that a projectile collided with a block.
        /// </summary>
        private void OnMessage_Received(MessageCollisionProjectileToBlockDetected message)
        {
            if (message.Block != this) { return; }

            m_isLeaving = true;

            // Animation for leaving the screen
            this.BuildAnimationSequence()
                .Move2DTo(
                    new Vector2(this.Position.X, Constants.BLOCK_LEAVING_Y_TARGET + 300f),
                    new MovementSpeed(
                        Constants.BLOCK_LEAVING_MAX_SPEED, 
                        Constants.BLOCK_LEAVING_ACCELERATION, 
                        0f))
                .WaitForCondition(() => this.Position.Y >= Constants.BLOCK_LEAVING_Y_TARGET)
                .ChangeFloatBy(
                    () => m_opacity,
                    (actValue) => m_opacity = actValue,
                    -m_opacity,
                    TimeSpan.FromMilliseconds(300))
                .WaitFinished()
                .CallAction(() => base.Scene.ManipulateSceneAsync((maniulator) => maniulator.Remove(this)))
                .Apply();

            // Animation for changing the opacity value
            this.BuildAnimationSequence()
                .ChangeFloatBy(
                    () => m_opacity,
                    (actValue) => m_opacity = actValue,
                    Constants.BLOCK_OPACITY_WHEN_LEAVING - m_opacity,
                    TimeSpan.FromMilliseconds(Constants.BLOCK_OPACITY_CHANGING_TIME_MS))
                .ApplyAsSecondary();
        }

        public Vector2 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        /// <summary>
        /// Returns true when this block leaves the screen.
        /// </summary>
        public bool IsLeaving
        {
            get { return m_isLeaving; }
        }
    }
}
