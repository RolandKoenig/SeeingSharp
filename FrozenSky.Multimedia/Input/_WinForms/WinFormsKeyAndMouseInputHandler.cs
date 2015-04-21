#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
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

#if DESKTOP
using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FrozenSky.Infrastructure;

// Some namespace mapings
using WinForms = System.Windows.Forms;
using GDI = System.Drawing;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(FrozenSky.Multimedia.Input.WinFormsKeyAndMouseInputHandler),
    contractType: typeof(FrozenSky.Multimedia.Input.IFrozenSkyInputHandler))]

namespace FrozenSky.Multimedia.Input
{
    class WinFormsKeyAndMouseInputHandler : IFrozenSkyInputHandler
    {
        private const float MOVEMENT = 0.3f;
        private const float ROTATION = 0.01f;

        // References to the view
        private Control m_currentControl;
        private RenderLoop m_renderLoop;
        private Camera3DBase m_camera;
        private IInputEnabledView m_focusHandler;

        // Some helper variables
        private GDI.Point m_lastMousePoint;
        private bool m_isMouseInside;
        private bool m_controlDown;
        private List<Keys> m_pressedKeys;

        /// <summary>
        /// Initializes a new instance of the <see cref="WinFormsKeyAndMouseInputHandler"/> class.
        /// </summary>
        public WinFormsKeyAndMouseInputHandler()
        {
            m_pressedKeys = new List<Keys>();
        }

        /// <summary>
        /// Generic method thet gets iteratively after this handler was started.
        /// </summary>
        public void UpdateMovement()
        {
            if (!m_currentControl.Focused)
            {
                m_pressedKeys.Clear();
                m_controlDown = false;
                return;
            }

            // Define multiplyer
            float multiplyer = 1f;
            if (m_controlDown) { multiplyer = 2f; }

            // Perform moving bassed on keyboard
            foreach (Keys actKey in m_pressedKeys)
            {
                switch (actKey)
                {
                    case Keys.Up:
                    case Keys.W:
                        m_camera.Zoom(MOVEMENT * multiplyer);
                        break;

                    case Keys.Down:
                    case Keys.S:
                        m_camera.Zoom(-MOVEMENT * multiplyer);
                        break;

                    case Keys.Left:
                    case Keys.A:
                        m_camera.Strave(-MOVEMENT * multiplyer);
                        break;

                    case Keys.Right:
                    case Keys.D:
                        m_camera.Strave(MOVEMENT * multiplyer);
                        break;

                    case Keys.Q:
                    case Keys.NumPad3:
                        m_camera.Move(new Vector3(0f, -MOVEMENT * multiplyer, 0f));
                        break;

                    case Keys.E:
                    case Keys.NumPad9:
                        m_camera.Move(new Vector3(0f, MOVEMENT * multiplyer, 0f));
                        break;

                    case Keys.NumPad4:
                        m_camera.Rotate(ROTATION, 0f);
                        break;

                    case Keys.NumPad2:
                        m_camera.Rotate(0f, -ROTATION);
                        break;

                    case Keys.NumPad6:
                        m_camera.Rotate(-ROTATION, 0f);
                        break;

                    case Keys.NumPad8:
                        m_camera.Rotate(0f, ROTATION);
                        break;
                }
            }
        }

        /// <summary>
        /// Gets a list containing all supported view types.
        /// </summary>
        public Type[] GetSupportedViewTypes()
        {
            return new Type[] { typeof(Control) };
        }

        /// <summary>
        /// Gets a list containing all supported camera types.
        /// </summary>
        public Type[] GetSupportedCameraTypes()
        {
            return new Type[] { typeof(Camera3DBase) };
        }

        /// <summary>
        /// Gets an array containing all supported input modes.
        /// </summary>
        public FrozenSkyInputMode[] GetSupportedInputModes()
        {
            return new[]
            {
                FrozenSkyInputMode.FreeCameraMovement
            };
        }

        /// <summary>
        /// Starts input handling.
        /// </summary>
        /// <param name="viewObject">The view object (e. g. Direct3D11Canvas).</param>
        /// <param name="cameraObject">The camera object (e. g. Camera3DBase).</param>
        public void Start(object viewObject, object cameraObject)
        {
            m_currentControl = viewObject as Control;
            if (m_currentControl == null) { throw new ArgumentException("Unable to handle given view object!"); }

            m_focusHandler = m_currentControl as IInputEnabledView;
            if (m_focusHandler == null) { throw new ArgumentException("Unable to handle given view object!"); }

            m_renderLoop = m_focusHandler.RenderLoop;
            if (m_renderLoop == null) { throw new ArgumentException("Unable to handle given view object!"); }

            m_camera = cameraObject as Camera3DBase;
            if (m_camera == null) { throw new ArgumentException("Unable to handle given camera object!"); }

            m_currentControl.MouseEnter += OnMouseEnter;
            m_currentControl.MouseClick += OnMouseClick;
            m_currentControl.MouseLeave += OnMouseLeave;
            m_currentControl.MouseMove += OnMouseMove;
            m_currentControl.MouseWheel += OnMouseWheel;
            m_currentControl.KeyUp += OnKeyUp;
            m_currentControl.KeyDown += OnKeyDown;

            m_controlDown = false;
        }

        /// <summary>
        /// Stops input handling.
        /// </summary>
        public void Stop()
        {
            if(m_currentControl != null)
            {
                m_currentControl.MouseEnter -= OnMouseEnter;
                m_currentControl.MouseClick -= OnMouseClick;
                m_currentControl.MouseLeave -= OnMouseLeave;
                m_currentControl.MouseMove -= OnMouseMove;
                m_currentControl.MouseWheel -= OnMouseWheel;
                m_currentControl.KeyUp -= OnKeyUp;
                m_currentControl.KeyDown -= OnKeyDown;
            }

            m_currentControl = null;
            m_focusHandler = null;
            m_renderLoop = null;
        }

        /// <summary>
        /// Called when the mouse enters the screen.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        private void OnMouseEnter(object sender, EventArgs e)
        {
            m_lastMousePoint = m_currentControl.PointToClient(Cursor.Position);
            m_isMouseInside = true;
        }

        /// <summary>
        /// Called when user clicks on this panel.
        /// </summary>
        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            m_currentControl.Focus();
        }

        /// <summary>
        /// Called when the mouse leaves the screen.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        private void OnMouseLeave(object sender, EventArgs e)
        {
            m_lastMousePoint = System.Drawing.Point.Empty;
            m_isMouseInside = false;
            m_controlDown = false;
        }

        /// <summary>
        /// Called when the mouse moves on the screen.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (m_isMouseInside)
            {
                Point moving = new Point(e.X - m_lastMousePoint.X, e.Y - m_lastMousePoint.Y);
                m_lastMousePoint = e.Location;

                // Perform moving based on mouse
                switch (e.Button)
                {
                    case MouseButtons.Right:
                        m_camera.Rotate(-moving.X / 200f, -moving.Y / 200f);
                        break;

                    case MouseButtons.Left:
                        m_camera.Strave(moving.X / 50f);
                        m_camera.UpDown(-moving.Y / 50f);
                        break;

                    case (MouseButtons.Right | MouseButtons.Left):
                        m_camera.Zoom(moving.Y / -50f);
                        break;
                }
            }
        }

        /// <summary>
        /// Called when mouse wheel is used.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            if (m_isMouseInside)
            {
                float multiplyer = 1f;
                if (m_controlDown) { multiplyer = 2f; }

                m_camera.Zoom((e.Delta / 100f) * multiplyer);
            }
        }

        /// <summary>
        /// Called when a key is up
        /// </summary>
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            // Remove the pressed key from the collection
            while (m_pressedKeys.Contains(e.KeyCode)) { m_pressedKeys.Remove(e.KeyCode); }
        }

        /// <summary>
        /// Called when a key is down.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs"/> that contains the event data.</param>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Handle key input
            if (!m_pressedKeys.Contains(e.KeyCode)) { m_pressedKeys.Add(e.KeyCode); }

            m_controlDown = e.Control;
        }

        /// <summary>
        /// Called when the focus on the target control is lost.
        /// </summary>
        private void OnLostFocus(object sender, EventArgs e)
        {
            // Clear all button states
            m_pressedKeys.Clear();
            m_controlDown = false;
        }
    }
}
#endif