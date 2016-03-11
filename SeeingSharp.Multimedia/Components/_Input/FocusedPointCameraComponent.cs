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
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Multimedia.Components
{
    public class FocusedPointCameraComponent 
        : SceneComponent<FocusedPointCameraComponent.PerSceneContext>
    {
        #region constants
        private const float SINGLE_ROTATION_H = EngineMath.RAD_180DEG / 100f;
        private const float SINGLE_ROTATION_V = EngineMath.RAD_90DEG / 100f;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FocusedPointCameraComponent"/> class.
        /// </summary>
        public FocusedPointCameraComponent()
        {
            this.FocusedLocation = Vector3.Zero;
            this.CameraDistanceInitial = 4f;
            this.CameraDistanceMin = 3f;
            this.CameraDistanceMax = 10f;
            this.CameraHVRotationInitial = new Vector2(
                EngineMath.RAD_45DEG,
                EngineMath.RAD_45DEG);
        }

        /// <summary>
        /// Attaches this component to a scene.
        /// Be careful, this method gets called from a background thread of seeing#!
        /// It may also be called from multiple scenes in parallel or simply withoud previous Detach call.
        /// </summary>
        /// <param name="manipulator">The manipulator of the scene we attach to.</param>
        /// <param name="correspondingView">The view which attached this component.</param>
        /// <returns></returns>
        protected override PerSceneContext Attach(SceneManipulator manipulator, ViewInformation correspondingView)
        {
            PerSceneContext result = new PerSceneContext();
            result.CameraDistance = this.CameraDistanceInitial;
            result.CameraHVRotation = this.CameraHVRotationInitial;
            return result;
        }

        /// <summary>
        /// Detaches this component from a scene.
        /// Be careful, this method gets called from a background thread of seeing#!
        /// It may also be called from multiple scenes in parallel.
        /// </summary>
        /// <param name="manipulator">The manipulator of the scene we attach to.</param>
        /// <param name="correspondingView">The view which attached this component.</param>
        /// <param name="componentContext">A context variable containing all createded objects during call of Attach.</param>
        protected override void Detach(SceneManipulator manipulator, ViewInformation correspondingView, PerSceneContext componentContext)
        {
   
        }

        protected override void Update(SceneRelatedUpdateState updateState, ViewInformation correspondingView, PerSceneContext componentContext)
        {
            Camera3DBase actCamera = correspondingView.Camera;
            if (actCamera == null) { return; }

            foreach (var actInputState in updateState.GetInputStates(correspondingView))
            {
                // Handle keyboard
                KeyboardState actKeyboardState = actInputState as KeyboardState;
                if (actKeyboardState != null)
                {
                    UpdateForKeyboard(componentContext, actCamera, actKeyboardState);
                    continue;
                }

                // Handle mouse (or pointer)
                MouseOrPointerState mouseState = actInputState as MouseOrPointerState;
                if (mouseState != null)
                {
                    UpdateForMouse(componentContext, actCamera, mouseState);
                }
            }

            // Ensure that our values are in allowed ranges
            float maxRad = EngineMath.RAD_90DEG * 0.99f;
            float minRad = EngineMath.RAD_90DEG * -0.99f;
            componentContext.CameraHVRotation.X = componentContext.CameraHVRotation.X % EngineMath.RAD_360DEG;
            if (componentContext.CameraDistance < this.CameraDistanceMin) { componentContext.CameraDistance = this.CameraDistanceMin; }
            if (componentContext.CameraDistance > this.CameraDistanceMax) { componentContext.CameraDistance = this.CameraDistanceMax; }
            if (componentContext.CameraHVRotation.Y <= minRad) { componentContext.CameraHVRotation.Y = minRad; }
            if (componentContext.CameraHVRotation.Y >= maxRad){ componentContext.CameraHVRotation.Y = maxRad; }

            // Update camera position and rotation
            Vector3 cameraOffset = Vector3.UnitX;
            cameraOffset = Vector3.TransformNormal(
                cameraOffset,
                Matrix4x4.CreateRotationY(componentContext.CameraHVRotation.X));
            cameraOffset = Vector3.TransformNormal(
                cameraOffset,
                Matrix4x4.CreateFromAxisAngle(Vector3.Cross(cameraOffset, Vector3.UnitY), componentContext.CameraHVRotation.Y));
            actCamera.Position = this.FocusedLocation + cameraOffset * componentContext.CameraDistance;
            actCamera.Target = this.FocusedLocation;
        }

        /// <summary>
        /// Update camera for keyboard input.
        /// </summary>
        private static void UpdateForKeyboard(
            PerSceneContext componentContext, Camera3DBase actCamera, 
            KeyboardState actKeyboardState)
        {
            foreach (WinVirtualKey actKey in actKeyboardState.KeysDown)
            {
                switch (actKey)
                {
                    case WinVirtualKey.Up:
                    case WinVirtualKey.W:
                    case WinVirtualKey.NumPad8:
                        componentContext.CameraHVRotation = componentContext.CameraHVRotation +
                            new Vector2(0f, SINGLE_ROTATION_V);
                        break;

                    case WinVirtualKey.Down:
                    case WinVirtualKey.S:
                    case WinVirtualKey.NumPad2:
                        componentContext.CameraHVRotation = componentContext.CameraHVRotation -
                            new Vector2(0f, SINGLE_ROTATION_V);
                        break;

                    case WinVirtualKey.Left:
                    case WinVirtualKey.A:
                    case WinVirtualKey.NumPad4:
                        componentContext.CameraHVRotation = componentContext.CameraHVRotation -
                            new Vector2(SINGLE_ROTATION_H, 0f);
                        break;

                    case WinVirtualKey.Right:
                    case WinVirtualKey.D:
                    case WinVirtualKey.NumPad6:
                        componentContext.CameraHVRotation = componentContext.CameraHVRotation +
                            new Vector2(SINGLE_ROTATION_H, 0f);
                        break;
                }
            }
        }

        /// <summary>
        /// Update camera for mouse input.
        /// </summary>
        private static void UpdateForMouse(
            PerSceneContext componentContext, Camera3DBase actCamera, 
            MouseOrPointerState mouseState)
        {
            //// Handle mouse move
            //if (mouseState.MoveDistanceDip != Vector2.Zero)
            //{
            //    Vector2 moving = mouseState.MoveDistanceDip;
            //    if (mouseState.IsButtonDown(MouseButton.Left) &&
            //        mouseState.IsButtonDown(MouseButton.Right))
            //    {
            //        actCamera.Zoom(moving.Y / -50f);
            //    }
            //    else if (mouseState.IsButtonDown(MouseButton.Left))
            //    {
            //        actCamera.Strave(moving.X / 50f);
            //        actCamera.UpDown(-moving.Y / 50f);
            //    }
            //    else if (mouseState.IsButtonDown(MouseButton.Right))
            //    {
            //        actCamera.Rotate(-moving.X / 200f, -moving.Y / 200f);
            //    }
            //}

            //// Handle mouse wheel
            //if (mouseState.WheelDelta != 0)
            //{
            //    float multiplyer = 1f;
            //    if (isControlKeyDown) { multiplyer = 2f; }
            //    actCamera.Zoom((mouseState.WheelDelta / 100f) * multiplyer);
            //}
        }

        public Vector3 FocusedLocation
        {
            get;
            set;
        }

        public float CameraDistanceInitial
        {
            get;
            set;
        }

        public float CameraDistanceMin
        {
            get;
            set;
        }

        public float CameraDistanceMax
        {
            get;
            set;
        }

        public Vector2 CameraHVRotationInitial
        {
            get;
            set;
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        public class PerSceneContext
        {
            public float CameraDistance;
            public Vector2 CameraHVRotation;
        }
    }
}
