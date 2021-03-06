﻿#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
	Exception are projects where it is noted otherwhise.
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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using SeeingSharp.Util;
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
        private IInputEnabledView m_focusHandler;
        #endregion References to the view

        #region Input states
        private MouseOrPointerState m_stateMouseOrPointer;
        private KeyboardState m_stateKeyboard;
        #endregion

        #region Some helper variables
        private GDI.Point m_lastMousePoint;
        private bool m_isMouseInside;
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
            m_stateMouseOrPointer = new MouseOrPointerState();
            m_stateMouseOrPointer.Type = MouseOrPointerType.Mouse;

            m_stateKeyboard = new KeyboardState();
        }

        /// <summary>
        /// Gets a list containing all supported view types.
        /// </summary>
        public Type[] GetSupportedViewTypes()
        {
            return new Type[] { typeof(Control), typeof(IInputControlHost) };
        }

        /// <summary>
        /// Starts input handling.
        /// </summary>
        /// <param name="viewObject">The view object (e. g. Direct3D11Canvas).</param>
        public void Start(IInputEnabledView viewObject)
        {
            m_currentControl = viewObject as Control;
            if (m_currentControl == null)
            {
                IInputControlHost inputControlHost = viewObject as IInputControlHost;
                m_currentControl = inputControlHost?.GetWinFormsInputControl();
                if (m_currentControl == null) { throw new ArgumentException("Unable to handle given view object!"); }
            }

            m_focusHandler = viewObject as IInputEnabledView;
            if (m_focusHandler == null) { throw new ArgumentException("Unable to handle given view object!"); }

            m_renderLoop = m_focusHandler.RenderLoop;
            if (m_renderLoop == null) { throw new ArgumentException("Unable to handle given view object!"); }

            // Perform event registrations on UI thread
            viewObject.RenderLoop.UISynchronizationContext.PostAsync(() =>
            {
                if (m_currentControl == null) { return; }

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

                // Handle initial focus state
                if(m_currentControl.Focused || m_currentControl.ContainsFocus)
                {
                    m_stateKeyboard.NotifyFocusGot();
                }
            }).FireAndForget();
        }

        /// <summary>
        /// Stops input handling.
        /// </summary>
        public void Stop()
        {
            // Perform event deregistrations on UI thread
            if (m_currentControl != null)
            {
                Control currentControl = m_currentControl;
                Action removeEventRegistrationsAction = new Action(() =>
                {
                    if(currentControl == null) { return; }

                    currentControl.MouseEnter -= OnMouseEnter;
                    currentControl.MouseClick -= OnMouseClick;
                    currentControl.MouseLeave -= OnMouseLeave;
                    currentControl.MouseMove -= OnMouseMove;
                    currentControl.MouseWheel -= OnMouseWheel;
                    currentControl.MouseUp -= OnMouseUp;
                    currentControl.MouseDown -= OnMouseDown;
                    currentControl.KeyUp -= OnKeyUp;
                    currentControl.KeyDown -= OnKeyDown;
                    currentControl.LostFocus -= OnLostFocus;
                    currentControl.GotFocus -= OnGotFocus;
                });

                if (m_currentControl.IsHandleCreated) { m_currentControl.BeginInvoke(removeEventRegistrationsAction); }
                else { removeEventRegistrationsAction(); }
            }

            // Set local references to zero
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
            if(m_currentControl == null) { return; }

            m_lastMousePoint = m_currentControl.PointToClient(Cursor.Position);
            m_isMouseInside = true;

            m_stateMouseOrPointer.NotifyInside(true);
        }

        /// <summary>
        /// Called when user clicks on this panel.
        /// </summary>
        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (m_currentControl == null) { return; }

            m_currentControl.Focus();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (m_currentControl == null) { return; }

            switch (e.Button)
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
            if (m_currentControl == null) { return; }

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
            if (m_currentControl == null) { return; }

            m_lastMousePoint = System.Drawing.Point.Empty;
            m_isMouseInside = false;

            m_stateMouseOrPointer.NotifyInside(false);
        }

        /// <summary>
        /// Called when the mouse moves on the screen.
        /// </summary>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (m_currentControl == null) { return; }

            if (m_isMouseInside)
            {
                Point moving = new Point(e.X - m_lastMousePoint.X, e.Y - m_lastMousePoint.Y);
                m_lastMousePoint = e.Location;

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
            if (m_currentControl == null) { return; }

            if (m_isMouseInside)
            {
                m_stateMouseOrPointer.NotifyMouseWheel(e.Delta);
            }
        }

        /// <summary>
        /// Called when a key is up
        /// </summary>
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (m_currentControl == null) { return; }

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
            if (m_currentControl == null) { return; }

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
            if (m_currentControl == null) { return; }

            m_stateKeyboard.NotifyFocusLost();
        }

        private void OnGotFocus(object sender, EventArgs e)
        {
            if (m_currentControl == null) { return; }

            m_stateKeyboard.NotifyFocusGot();
        }
    }
}