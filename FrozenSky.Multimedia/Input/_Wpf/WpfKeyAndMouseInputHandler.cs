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
using FrozenSky.Infrastructure;
using FrozenSky.Multimedia.Drawing3D;
using FrozenSky.Multimedia.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(FrozenSky.Multimedia.Input.WpfKeyAndMouseInputHandler),
    contractType: typeof(FrozenSky.Multimedia.Input.IFrozenSkyInputHandler))]

namespace FrozenSky.Multimedia.Input
{
    class WpfKeyAndMouseInputHandler : IFrozenSkyInputHandler
    {
        private const float MOVEMENT = 0.3f;
        private const float ROTATION = 0.01f;

        #region Target objects
        private FrozenSkyRendererElement m_rendererElement;
        private Camera3DBase m_camera;
        #endregion

        #region Current movement state
        private bool m_isDragging;
        private System.Windows.Point m_lastDragPoint;
        private List<Key> m_pressedKeys;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfKeyAndMouseInputHandler"/> class.
        /// </summary>
        public WpfKeyAndMouseInputHandler()
        {
            m_pressedKeys = new List<Key>();
        }

        /// <summary>
        /// Gets a list containing all supported view types.
        /// </summary>
        public Type[] GetSupportedViewTypes()
        {
            return new Type[]
            {
                typeof(FrozenSkyRendererElement)
            };
        }

        /// <summary>
        /// Gets a list containing all supported camera types.
        /// </summary>
        public Type[] GetSupportedCameraTypes()
        {
            return new Type[]
            {
                typeof(Camera3DBase)
            };
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
            m_rendererElement = viewObject as FrozenSkyRendererElement;
            m_camera = cameraObject as Camera3DBase;

            if(m_rendererElement != null)
            {
                //Register all events needed for mouse camera dragging
                m_rendererElement.MouseWheel += OnRendererElement_MouseWheel;
                m_rendererElement.MouseDown += OnRendererElement_MouseDown;
                m_rendererElement.MouseUp += OnRendererElement_MouseUp;
                m_rendererElement.MouseMove += OnRendererElement_MouseMove;
                m_rendererElement.MouseLeave += OnRendererElement_MouseLeave;
                m_rendererElement.LostFocus += OnRendererElement_LostFocus;
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
            if (!m_rendererElement.IsFocused)
            {
                m_pressedKeys.Clear();
                return;
            }

            // Define multiplyer
            float multiplyer = 1f;

            // Perform moving bassed on keyboard
            foreach (Key actKey in m_pressedKeys)
            {
                switch (actKey)
                {
                    case Key.Up:
                    case Key.W:
                        m_camera.Zoom(MOVEMENT * multiplyer);
                        break;

                    case Key.Down:
                    case Key.S:
                        m_camera.Zoom(-MOVEMENT * multiplyer);
                        break;

                    case Key.Left:
                    case Key.A:
                        m_camera.Strave(-MOVEMENT * multiplyer);
                        break;

                    case Key.Right:
                    case Key.D:
                        m_camera.Strave(MOVEMENT * multiplyer);
                        break;

                    case Key.Q:
                    case Key.NumPad3:
                        m_camera.Move(new Vector3(0f, -MOVEMENT * multiplyer, 0f));
                        break;

                    case Key.E:
                    case Key.NumPad9:
                        m_camera.Move(new Vector3(0f, MOVEMENT * multiplyer, 0f));
                        break;

                    case Key.NumPad4:
                        m_camera.Rotate(ROTATION, 0f);
                        break;

                    case Key.NumPad2:
                        m_camera.Rotate(0f, -ROTATION);
                        break;

                    case Key.NumPad6:
                        m_camera.Rotate(-ROTATION, 0f);
                        break;

                    case Key.NumPad8:
                        m_camera.Rotate(0f, ROTATION);
                        break;
                }
            }
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
                m_rendererElement.PreviewMouseUp -= OnRendererElement_PreviewMouseUp;
            }

            m_rendererElement = null;
            m_camera = null;
            m_pressedKeys.Clear();
            m_lastDragPoint = new System.Windows.Point();
            m_isDragging = false;
        }

        private void StartCameraDragging(MouseButtonEventArgs e)
        {
            m_rendererElement.Focus();

            m_isDragging = true;
            m_rendererElement.Cursor = Cursors.Cross;
            m_lastDragPoint = e.GetPosition(m_rendererElement);
        }

        private void StopCameraDragging()
        {
            m_isDragging = false;
            m_rendererElement.Cursor = Cursors.Hand;
        }

        private void OnRendererElement_KeyDown(object sender, KeyEventArgs e)
        {
            if (!m_pressedKeys.Contains(e.Key)) { m_pressedKeys.Add(e.Key); }
        }

        private void OnRendererElement_KeyUp(object sender, KeyEventArgs e)
        {
            while(m_pressedKeys.Remove(e.Key))
            {

            }
        }

        /// <summary>
        /// Called when user uses the mouse wheel for zooming.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRendererElement_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            m_camera.Zoom((float)(e.Delta / 100.0));
        }

        private void OnRendererElement_LostFocus(object sender, RoutedEventArgs e)
        {
            StopCameraDragging();
        }

        private void OnRendererElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StartCameraDragging(e);
        }

        private void OnRendererElement_MouseLeave(object sender, MouseEventArgs e)
        {
            StopCameraDragging();
        }

        private void OnRendererElement_MouseMove(object sender, MouseEventArgs e)
        {
            if ((m_camera != null) &&
               (m_isDragging))
            {
                System.Windows.Point newDragPoint = e.GetPosition(m_rendererElement);
                Vector2 moveDistance = new Vector2(
                    (float)(newDragPoint.X - m_lastDragPoint.X),
                    (float)(newDragPoint.Y - m_lastDragPoint.Y));

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    m_camera.Strave((float)((double)moveDistance.X / 50));
                    m_camera.UpDown((float)(-(double)moveDistance.Y / 50));
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    m_camera.Rotate(
                        (float)(-(double)moveDistance.X / 300),
                        (float)(-(double)moveDistance.Y / 300));
                }

                m_lastDragPoint = newDragPoint;
            }
        }

        private void OnRendererElement_MouseUp(object sender, MouseButtonEventArgs e)
        {
            StopCameraDragging();
        }

        private void OnRendererElement_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            StopCameraDragging();
        }
    }
}
#endif