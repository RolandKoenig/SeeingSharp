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
#endregion License information (SeeingSharp and all based games/applications)

#if DESKTOP

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;

// Some namespace mapings
using GDI = System.Drawing;
using WinForms = System.Windows.Forms;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.Multimedia.Input.WinFormsKeyAndMouseInputHandler),
    contractType: typeof(SeeingSharp.Multimedia.Input.IInputHandler))]

namespace SeeingSharp.Multimedia.Input
{
    internal class WinFormsKeyAndMouseInputHandler : IInputHandler
    {
        private const float MOVEMENT = 0.3f;
        private const float ROTATION = 0.01f;
        private static readonly Dictionary<Keys, WinVirtualKey> s_keyMappingDict;

        #region References to the view
        private Control m_currentControl;
        private RenderLoop m_renderLoop;
        private Camera3DBase m_camera;
        private IInputEnabledView m_focusHandler;
        #endregion References to the view

        #region Input states
        private MouseOrPointerState m_stateMouseOrPointer;
        private KeyboardState m_stateKeyboard;
        #endregion

        #region Some helper variables
        private GDI.Point m_lastMousePoint;
        private bool m_isMouseInside;
        private bool m_controlDown;
        private List<Keys> m_pressedKeys;
        #endregion Some helper variables

        /// <summary>
        /// Initializes the <see cref="WinFormsKeyAndMouseInputHandler"/> class.
        /// </summary>
        static WinFormsKeyAndMouseInputHandler()
        {
            // First look for all key codes we have
            Dictionary<int, WinVirtualKey> supportedKeyCodes = new Dictionary<int, WinVirtualKey>();
            foreach(WinVirtualKey actVirtualKey in Enum.GetValues(typeof(WinVirtualKey)))
            {
                supportedKeyCodes[(int)actVirtualKey] = actVirtualKey;
            }

            // Build the mapping dictionary
            s_keyMappingDict = new Dictionary<Keys, WinVirtualKey>();
            foreach (Keys actKeyMember in Enum.GetValues(typeof(Keys)))
            {
                int actKeyCode = (int)actKeyMember;
                if(supportedKeyCodes.ContainsKey(actKeyCode))
                {
                    s_keyMappingDict[actKeyMember] = supportedKeyCodes[actKeyCode];
                }
                else
                {
                    s_keyMappingDict[actKeyMember] = WinVirtualKey.None;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WinFormsKeyAndMouseInputHandler"/> class.
        /// </summary>
        public WinFormsKeyAndMouseInputHandler()
        {
            m_pressedKeys = new List<Keys>();

            m_stateMouseOrPointer = new MouseOrPointerState();
            m_stateKeyboard = new KeyboardState();
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
        public SeeingSharpInputMode[] GetSupportedInputModes()
        {
            return new[]
            {
                SeeingSharpInputMode.FreeCameraMovement
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
            m_currentControl.MouseUp += OnMouseUp;
            m_currentControl.MouseDown += OnMouseDown;
            m_currentControl.MouseLeave += OnMouseLeave;
            m_currentControl.MouseMove += OnMouseMove;
            m_currentControl.MouseWheel += OnMouseWheel;
            m_currentControl.KeyUp += OnKeyUp;
            m_currentControl.KeyDown += OnKeyDown;
            m_currentControl.LostFocus += OnLostFocus;
            m_currentControl.GotFocus += OnGotFocus;

            m_controlDown = false;
        }

        /// <summary>
        /// Stops input handling.
        /// </summary>
        public void Stop()
        {
            if (m_currentControl != null)
            {
                m_currentControl.MouseEnter -= OnMouseEnter;
                m_currentControl.MouseClick -= OnMouseClick;
                m_currentControl.MouseLeave -= OnMouseLeave;
                m_currentControl.MouseMove -= OnMouseMove;
                m_currentControl.MouseWheel -= OnMouseWheel;
                m_currentControl.MouseUp -= OnMouseUp;
                m_currentControl.MouseDown -= OnMouseDown;
                m_currentControl.KeyUp -= OnKeyUp;
                m_currentControl.KeyDown -= OnKeyDown;
                m_currentControl.LostFocus -= OnLostFocus;
                m_currentControl.GotFocus -= OnGotFocus;
            }

            m_currentControl = null;
            m_focusHandler = null;
            m_renderLoop = null;
        }

        /// <summary>
        /// Querries all current input states.
        /// </summary>
        public IEnumerable<InputStateBase> GetInputStates()
        {
            yield return m_stateMouseOrPointer;
            yield return m_stateKeyboard;
        }

        /// <summary>
        /// Called when the mouse enters the screen.
        /// </summary>
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

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            switch(e.Button)
            {
                case MouseButtons.Left:
                    m_stateMouseOrPointer.NotifyButtonDown(MouseButton.Left);
                    break;

                case MouseButtons.Middle:
                    m_stateMouseOrPointer.NotifyButtonDown(MouseButton.Middle);
                    break;

                case MouseButtons.Right:
                    m_stateMouseOrPointer.NotifyButtonDown(MouseButton.Right);
                    break;

                case MouseButtons.XButton1:
                    m_stateMouseOrPointer.NotifyButtonDown(MouseButton.Extended1);
                    break;

                case MouseButtons.XButton2:
                    m_stateMouseOrPointer.NotifyButtonDown(MouseButton.Extended2);
                    break;
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    m_stateMouseOrPointer.NotifyButtonUp(MouseButton.Left);
                    break;

                case MouseButtons.Middle:
                    m_stateMouseOrPointer.NotifyButtonUp(MouseButton.Middle);
                    break;

                case MouseButtons.Right:
                    m_stateMouseOrPointer.NotifyButtonUp(MouseButton.Right);
                    break;

                case MouseButtons.XButton1:
                    m_stateMouseOrPointer.NotifyButtonUp(MouseButton.Extended1);
                    break;

                case MouseButtons.XButton2:
                    m_stateMouseOrPointer.NotifyButtonUp(MouseButton.Extended2);
                    break;
            }
        }

        /// <summary>
        /// Called when the mouse leaves the screen.
        /// </summary>
        private void OnMouseLeave(object sender, EventArgs e)
        {
            m_lastMousePoint = System.Drawing.Point.Empty;
            m_isMouseInside = false;
            m_controlDown = false;
        }

        /// <summary>
        /// Called when the mouse moves on the screen.
        /// </summary>
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

                m_stateMouseOrPointer.NotifyMouseLocation(
                    new Vector2((float)e.X, (float)e.Y),
                    new Vector2((float)moving.X, (float)moving.Y),
                    Vector2Ex.FromSize2(m_renderLoop.ViewInformation.CurrentViewSize));
            }
        }

        /// <summary>
        /// Called when mouse wheel is used.
        /// </summary>
        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            if (m_isMouseInside)
            {
                float multiplyer = 1f;
                if (m_controlDown) { multiplyer = 2f; }

                m_camera.Zoom((e.Delta / 100f) * multiplyer);

                m_stateMouseOrPointer.NotifyMouseWheel(e.Delta);
            }
        }

        /// <summary>
        /// Called when a key is up
        /// </summary>
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            // Remove the pressed key from the collection
            while (m_pressedKeys.Contains(e.KeyCode)) { m_pressedKeys.Remove(e.KeyCode); }

            // Notify event to keyboard state
            WinVirtualKey actKeyCode = s_keyMappingDict[e.KeyCode];
            if (actKeyCode != WinVirtualKey.None)
            {
                m_stateKeyboard.NotifyKeyUp(actKeyCode);
            }
        }

        /// <summary>
        /// Called when a key is down.
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Handle key input
            if (!m_pressedKeys.Contains(e.KeyCode)) { m_pressedKeys.Add(e.KeyCode); }

            m_controlDown = e.Control;

            // Notify event to keyboard state
            WinVirtualKey actKeyCode = s_keyMappingDict[e.KeyCode];
            if (actKeyCode != WinVirtualKey.None)
            {
                m_stateKeyboard.NotifyKeyDown(actKeyCode);
            }
        }

        /// <summary>
        /// Called when the focus on the target control is lost.
        /// </summary>
        private void OnLostFocus(object sender, EventArgs e)
        {
            // Clear all button states
            m_pressedKeys.Clear();
            m_controlDown = false;

            m_stateKeyboard.NotifyFocusLost();
        }

        private void OnGotFocus(object sender, EventArgs e)
        {
            m_stateKeyboard.NotifyFocusGot();
        }
    }
}

#endif