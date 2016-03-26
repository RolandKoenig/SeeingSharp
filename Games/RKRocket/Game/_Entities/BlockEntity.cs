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
using SeeingSharp;
using SeeingSharp.Checking;
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
    public class BlockEntity : GameObject2D, IAnimatableObjectPosition2D
    {
        #region Graphics Resources
        private StandardBitmapResource m_blockBitmap;
        #endregion

        #region Logic
        private Vector2 m_position;
        private bool m_isLeaving;
        private float m_opacity;
        private float m_rotation;
        private int m_points;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockEntity"/> class.
        /// </summary>
        public BlockEntity(int points)
        {
            points.EnsureInRange(1, GraphicsResources.Bitmap_Blocks.Length, nameof(points));

            m_points = points;
            m_blockBitmap = GraphicsResources.Bitmap_Blocks[m_points - 1];
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

            start = start + size * 0.02f;
            size = size * 0.96f;

            return new BoundingBox(start, start + size);
        }

        protected override void UpdateInternal(SceneRelatedUpdateState updateState)
        {
            if((m_position.Y > Constants.BLOCK_SIM_MAX_Y) &&
               (!this.IsLeaving))
            {
                base.Messenger.Publish(new MessageGameOver(GameOverReason.BlockReachedFooter));
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
                m_position.X - Constants.BLOCK_VPIXEL_WIDTH / 2f,
                m_position.Y - Constants.BLOCK_VPIXEL_HEIGHT / 2f,
                Constants.BLOCK_VPIXEL_WIDTH,
                Constants.BLOCK_VPIXEL_HEIGHT);

            Matrix3x2 prevMatrix = Matrix3x2.Identity;
            if(m_rotation > 0f)
            {
                // Apply transform based on local rotation
                prevMatrix = graphics.Transform;
                graphics.Transform = Matrix3x2.CreateRotation(m_rotation, m_position) * prevMatrix;
            }
            try
            {
                // Calculate opacity value depending on y position of the block
                float localOpacity = m_opacity;
                if(m_position.Y < 0f) { localOpacity = 0f; }
                else if (m_position.Y < 20f)
                {
                    localOpacity = (m_position.Y / 20f) * m_opacity;
                }
                else { localOpacity = m_opacity; }
                
                // Ensure correct opacity value
                if(localOpacity < 0f) { localOpacity = 0f; }

                // Draw the block
                graphics.DrawBitmap(
                    m_blockBitmap, destRectangle,
                    opacity: localOpacity,
                    interpolationMode: BitmapInterpolationMode.Linear);
            }
            finally
            {
                // Restore previous transform
                if (m_rotation > 0f)
                {
                    graphics.Transform = prevMatrix;
                }
            }
        }

        private void OnMessage_Received(MessageBlocksFallOneCellDown message)
        {
            if (m_isLeaving) { return; }

            // Trigger move down
            this.BuildAnimationSequence()
                .Move2DBy(new Vector2(0f, Constants.BLOCK_CELL_HEIGHT), TimeSpan.FromSeconds(0.5))
                .Apply();
        }

        /// <summary>
        /// The CollisionSystem notifies us that a projectile collided with a block.
        /// </summary>
        private void OnMessage_Received(MessageCollisionProjectileToBlockDetected message)
        {
            if (message.Block != this) { return; }

            m_points--;
            if (m_points > 0)
            {
                m_blockBitmap = GraphicsResources.Bitmap_Blocks[m_points - 1];
            }
            else
            {   
                m_isLeaving = true;

                // Cancel all current animations
                this.AnimationHandler.CancelAnimations();

                // Animation for leaving the screen
                this.BuildAnimationSequence()
                    .Move2DTo(
                        new Vector2(this.Position.X, Constants.BLOCK_LEAVING_Y_TARGET + 300f),
                        new MovementSpeed(
                            Constants.BLOCK_LEAVING_MAX_SPEED,
                            Constants.BLOCK_LEAVING_ACCELERATION,
                            0f))
                    .ChangeFloatBy(
                        () => m_rotation,
                        (actVaue) => m_rotation = actVaue,
                        EngineMath.DegreeToRadian(240f),
                        TimeSpan.FromSeconds(5.0))
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

                // Publish leaving message
                base.Messenger.Publish(new MessageBlockStartsLeaving(this));
            }
        }

        /// <summary>
        /// Handles the NewGame event.
        /// </summary>
        private void OnMessage_Received(MessageNewGame message)
        {
            // Remove this object from the scene
            base.Scene.ManipulateSceneAsync((manipulator) => manipulator.Remove(this))
                .FireAndForget();
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

        public bool IsRelevantForCollisionSystem
        {
            get { return m_position.Y >= 25f; }
        }

        public int Points
        {
            get { return m_points; }
        }

        protected override int RenderZOrder
        {
            get
            {
                return Constants.ZORDER_BLOCK;
            }
        }
    }
}
