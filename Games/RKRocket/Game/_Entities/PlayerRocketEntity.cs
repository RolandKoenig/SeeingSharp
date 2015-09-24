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
    public class PlayerRocketEntity : GameObject2D
    {
        #region Resources
        private StandardBitmapResource m_playerBitmap;
        private PolygonGeometryResource m_collisionGeometry;
        #endregion

        #region State
        private float m_xPos;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerRocketEntity"/> class.
        /// </summary>
        public PlayerRocketEntity()
        {
            m_xPos = Constants.GFX_SCREEN_VPIXEL_WIDTH / 2f;

            m_playerBitmap = GraphicsResources.Bitmap_Player;
        }

        public Geometry2DResourceBase GetCollisionGeometry()
        {
            return m_collisionGeometry;
        }

        /// <summary>
        /// Updates this object.
        /// </summary>
        /// <param name="updateState">State of the update.</param>
        protected override void UpdateInternal(SceneRelatedUpdateState updateState)
        {
            // Get input states
            MouseOrPointerState mouseState = updateState.DefaultMouseOrPointer;
            GamepadState gamepadState = updateState.DefaultGamepad;
            KeyboardState keyboardState = updateState.DefaultKeyboard;

            // Initialize variables for input control
            bool isFireHit = false;
            float moveDistance = 0f;
            float maxMoveDistance = (float)updateState.UpdateTime.TotalSeconds * Constants.SIM_ROCKET_MAX_X_MOVEMENT;

            // Capture mouse state
            if (mouseState.IsButtonHit(MouseButton.Left)) { isFireHit = true; }
            if (mouseState.MoveDistanceRelative != Vector2.Zero)
            {
                float newMouseX = 0f;
                float currentViewWidthDip =
                    (mouseState.ScreenSizeDip.Y / Constants.GFX_SCREEN_VPIXEL_HEIGHT) * Constants.GFX_SCREEN_VPIXEL_WIDTH;
                if (mouseState.ScreenSizeDip.X > currentViewWidthDip)
                {
                    float relativMouseX = 
                        (mouseState.PositionDip.X - (mouseState.ScreenSizeDip.X - currentViewWidthDip) / 2f) / 
                        currentViewWidthDip;
                    if(relativMouseX > 1f) { relativMouseX = 1f; }
                    if(relativMouseX < -1f) { relativMouseX = -1f; }

                    newMouseX = relativMouseX * Constants.GFX_SCREEN_VPIXEL_WIDTH;
                }
                else
                {
                    newMouseX = Constants.GFX_SCREEN_VPIXEL_WIDTH * mouseState.PositionRelative.X;
                }
                moveDistance = newMouseX - m_xPos;
            }

            // Capture keyboard state
            if (keyboardState.IsConnected)
            {
                isFireHit |=
                    keyboardState.IsKeyHit(WinVirtualKey.Space) ||
                    keyboardState.IsKeyHit(WinVirtualKey.Enter);

                if (keyboardState.IsKeyDown(WinVirtualKey.Left) ||
                    keyboardState.IsKeyDown(WinVirtualKey.A) ||
                    keyboardState.IsKeyDown(WinVirtualKey.NumPad4))
                {
                    moveDistance = -maxMoveDistance;
                }
                else if (keyboardState.IsKeyDown(WinVirtualKey.Right) ||
                    keyboardState.IsKeyDown(WinVirtualKey.D) ||
                    keyboardState.IsKeyDown(WinVirtualKey.NumPad6))
                {
                    moveDistance = maxMoveDistance;
                }
            }

            // Capture gamepad state
            if (gamepadState.IsConnected)
            {
                isFireHit |= 
                    gamepadState.IsButtonHit(GamepadButton.X) ||
                    gamepadState.IsButtonHit(GamepadButton.A) ||
                    gamepadState.IsButtonHit(GamepadButton.B) ||
                    gamepadState.IsButtonHit(GamepadButton.Y);

                if (gamepadState.IsButtonDown(GamepadButton.DPadLeft)){ moveDistance = -maxMoveDistance; }
                else if (gamepadState.IsButtonDown(GamepadButton.DPadRight)){ moveDistance = maxMoveDistance; }
                else if(Math.Abs((int)gamepadState.LeftThumbX) > 10000)
                {
                    moveDistance = (gamepadState.LeftThumbX / (float)short.MaxValue) * maxMoveDistance;
                }
                else if(Math.Abs((int)gamepadState.RightThumbX) > 10000)
                {
                    moveDistance = (gamepadState.RightThumbX / (float)short.MaxValue) * maxMoveDistance;
                }
            }

            // Apply states
            if(moveDistance != 0f)
            {
                m_xPos = m_xPos + moveDistance;
                if(m_xPos < 50f) { m_xPos = 50f; }
                if(m_xPos > Constants.GFX_SCREEN_VPIXEL_WIDTH - 50) { m_xPos = Constants.GFX_SCREEN_VPIXEL_WIDTH - 50f; }
            }
            if (isFireHit)
            {
                ProjectileEntity newProjectile = new ProjectileEntity(new Vector2(
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

            // DEBUG: Draw collision geometry
            if (Constants.RENDER_COLLISION_GEOMETRY)
            {
                using (SolidBrushResource brush = new SolidBrushResource(Color4.Aqua.ChangeAlphaTo(0.5f)))
                using (var disposable = graphics.BlockForLocalTransform_TransformLocal(
                    Matrix3x2.CreateTranslation(m_xPos, Constants.GFX_ROCKET_VPIXEL_Y_CENTER)))
                {
                    graphics.FillGeometry(m_collisionGeometry, brush);
                }
            }
        }

        /// <summary>
        /// Called when this object was added to the scene.
        /// </summary>
        protected override void OnAddedToScene(Scene newScene)
        {
            base.OnAddedToScene(newScene);

            float halfWidth = Constants.GFX_ROCKET_VPIXEL_WIDTH / 2f;
            Polygon2D collisionPolygon = new Polygon2D(new Vector2[]
            {
                new Vector2(-halfWidth, Constants.GFX_ROCKET_VPIXEL_HEIGHT * 0.4f),
                new Vector2(0f, -Constants.GFX_ROCKET_VPIXEL_HEIGHT * 0.6f),
                new Vector2(halfWidth, Constants.GFX_ROCKET_VPIXEL_HEIGHT * 0.4f)
            });
            m_collisionGeometry = new PolygonGeometryResource(collisionPolygon);
        }

        /// <summary>
        /// Called when this object was removed from the scene.
        /// </summary>
        protected override void OnRemovedFromScene(Scene oldScene)
        {
            base.OnRemovedFromScene(oldScene);

            CommonTools.SafeDispose(ref m_collisionGeometry);
        }

        public Vector2 Position
        {
            get
            {
                return new Vector2(m_xPos, Constants.GFX_ROCKET_VPIXEL_Y_CENTER);
            }
        }
    }
}
