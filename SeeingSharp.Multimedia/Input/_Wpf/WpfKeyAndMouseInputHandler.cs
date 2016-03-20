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
#if DESKTOP
using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Views;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.Multimedia.Input.WpfKeyAndMouseInputHandler),
    contractType: typeof(SeeingSharp.Multimedia.Input.IInputHandler))]

namespace SeeingSharp.Multimedia.Input
{
    class WpfKeyAndMouseInputHandler : IInputHandler
    {
#region Target objects
        private SeeingSharpRendererElement m_rendererElement;
#endregion

#region Helper
        private bool m_lastDragPointValid;
        private System.Windows.Point m_lastDragPoint;
#endregion

#region Input states
        private MouseOrPointerState m_stateMouseOrPointer;
        private KeyboardState m_stateKeyboard;
#endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfKeyAndMouseInputHandler"/> class.
        /// </summary>
        public WpfKeyAndMouseInputHandler()
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
            return new Type[]
            {
                typeof(SeeingSharpRendererElement)
            };
        }

        /// <summary>
        /// Starts input handling.
        /// </summary>
        /// <param name="viewObject">The view object (e. g. Direct3D11Canvas).</param>
        public void Start(object viewObject)
        {
            m_rendererElement = viewObject as SeeingSharpRendererElement;

            if(m_rendererElement != null)
            {
                // Register all events needed for mouse camera dragging
                m_rendererElement.MouseWheel += OnRendererElement_MouseWheel;
                m_rendererElement.MouseDown += OnRendererElement_MouseDown;
                m_rendererElement.MouseUp += OnRendererElement_MouseUp;
                m_rendererElement.MouseMove += OnRendererElement_MouseMove;
                m_rendererElement.MouseLeave += OnRendererElement_MouseLeave;
                m_rendererElement.GotFocus += OnRenderElement_GotFocus;
                m_rendererElement.LostFocus += OnRendererElement_LostFocus;
                m_rendererElement.LostKeyboardFocus += OnRendererElement_LostKeyboardFocus;
                m_rendererElement.PreviewMouseUp += OnRendererElement_PreviewMouseUp;
                m_rendererElement.KeyUp += OnRendererElement_KeyUp;
                m_rendererElement.KeyDown += OnRendererElement_KeyDown;
            }
        }

        /// <summary>
        /// Generic method thet gets iteratively after this handler was started.
        /// </summary>
        public void UpdateMovement()
        {

        }

        /// <summary>
        /// Stops input handling.
        /// </summary>
        public void Stop()
        {
            // Deregister all events
            if(m_rendererElement != null)
            {
                m_rendererElement.MouseWheel -= OnRendererElement_MouseWheel;
                m_rendererElement.MouseDown -= OnRendererElement_MouseDown;
                m_rendererElement.MouseUp -= OnRendererElement_MouseUp;
                m_rendererElement.MouseMove -= OnRendererElement_MouseMove;
                m_rendererElement.MouseLeave -= OnRendererElement_MouseLeave;
                m_rendererElement.LostFocus -= OnRendererElement_LostFocus;
                m_rendererElement.LostKeyboardFocus -= OnRendererElement_LostKeyboardFocus;
                m_rendererElement.GotFocus -= OnRenderElement_GotFocus;
                m_rendererElement.PreviewMouseUp -= OnRendererElement_PreviewMouseUp;
            }

            m_rendererElement = null;

            m_stateKeyboard = new KeyboardState();
            m_stateMouseOrPointer = new MouseOrPointerState();
        }

        /// <summary>
        /// Querries all current input states.
        /// </summary>
        public IEnumerable<InputStateBase> GetInputStates()
        {
            yield return m_stateMouseOrPointer;
            yield return m_stateKeyboard;
        }

        private void OnRendererElement_KeyDown(object sender, KeyEventArgs e)
        {
            m_stateKeyboard.NotifyKeyDown((WinVirtualKey)KeyInterop.VirtualKeyFromKey(e.Key));
        }

        private void OnRendererElement_KeyUp(object sender, KeyEventArgs e)
        {
            m_stateKeyboard.NotifyKeyUp((WinVirtualKey)KeyInterop.VirtualKeyFromKey(e.Key));
        }

        /// <summary>
        /// Called when user uses the mouse wheel for zooming.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRendererElement_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            m_stateMouseOrPointer.NotifyMouseWheel(e.Delta);
        }

        private void OnRenderElement_GotFocus(object sender, RoutedEventArgs e)
        {
            m_stateKeyboard.NotifyFocusGot();
        }

        private void OnRendererElement_LostFocus(object sender, RoutedEventArgs e)
        {
            m_stateKeyboard.NotifyFocusLost();
        }

        private void OnRendererElement_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            m_stateKeyboard.NotifyFocusLost();
        }

        private void OnRendererElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            m_rendererElement.Focus();

            switch (e.ChangedButton)
            {
                case System.Windows.Input.MouseButton.Left:
                    m_stateMouseOrPointer.NotifyButtonDown(MouseButton.Left);
                    break;

                case System.Windows.Input.MouseButton.Middle:
                    m_stateMouseOrPointer.NotifyButtonDown(MouseButton.Middle);
                    break;

                case System.Windows.Input.MouseButton.Right:
                    m_stateMouseOrPointer.NotifyButtonDown(MouseButton.Right);
                    break;

                case System.Windows.Input.MouseButton.XButton1:
                    m_stateMouseOrPointer.NotifyButtonDown(MouseButton.Extended1);
                    break;

                case System.Windows.Input.MouseButton.XButton2:
                    m_stateMouseOrPointer.NotifyButtonDown(MouseButton.Extended2);
                    break;
            }
        }

        private void OnRendererElement_MouseLeave(object sender, MouseEventArgs e)
        {
            m_stateMouseOrPointer.NotifyInside(false);

            m_lastDragPointValid = false;
            m_lastDragPoint = new System.Windows.Point();
        }

        private void OnRendererElement_MouseMove(object sender, MouseEventArgs e)
        {
            m_stateMouseOrPointer.NotifyInside(true);

            System.Windows.Point currentPosition = e.GetPosition(m_rendererElement);
            if (m_lastDragPointValid)
            {
                m_stateMouseOrPointer.NotifyMouseLocation(
                    Vector2Ex.FromWpfPoint(currentPosition),
                    Vector2Ex.FromWpfVector(currentPosition - m_lastDragPoint),
                    Vector2Ex.FromWpfSize(m_rendererElement.RenderSize));
            }

            m_lastDragPointValid = true;
            m_lastDragPoint = currentPosition;
        }

        private void OnRendererElement_MouseUp(object sender, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case System.Windows.Input.MouseButton.Left:
                    m_stateMouseOrPointer.NotifyButtonUp(MouseButton.Left);
                    break;

                case System.Windows.Input.MouseButton.Middle:
                    m_stateMouseOrPointer.NotifyButtonUp(MouseButton.Middle);
                    break;

                case System.Windows.Input.MouseButton.Right:
                    m_stateMouseOrPointer.NotifyButtonUp(MouseButton.Right);
                    break;

                case System.Windows.Input.MouseButton.XButton1:
                    m_stateMouseOrPointer.NotifyButtonUp(MouseButton.Extended1);
                    break;

                case System.Windows.Input.MouseButton.XButton2:
                    m_stateMouseOrPointer.NotifyButtonUp(MouseButton.Extended2);
                    break;
            }
        }

        private void OnRendererElement_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case System.Windows.Input.MouseButton.Left:
                    m_stateMouseOrPointer.NotifyButtonUp(MouseButton.Left);
                    break;

                case System.Windows.Input.MouseButton.Middle:
                    m_stateMouseOrPointer.NotifyButtonUp(MouseButton.Middle);
                    break;

                case System.Windows.Input.MouseButton.Right:
                    m_stateMouseOrPointer.NotifyButtonUp(MouseButton.Right);
                    break;

                case System.Windows.Input.MouseButton.XButton1:
                    m_stateMouseOrPointer.NotifyButtonUp(MouseButton.Extended1);
                    break;

                case System.Windows.Input.MouseButton.XButton2:
                    m_stateMouseOrPointer.NotifyButtonUp(MouseButton.Extended2);
                    break;
            }
        }
    }
}
#endif