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

#if UNIVERSAL
using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.Multimedia.Input.WinRTKeyAndMouseInputHandler),
    contractType: typeof(SeeingSharp.Multimedia.Input.ISeeingSharpInputHandler))]

namespace SeeingSharp.Multimedia.Input
{
    class WinRTKeyAndMouseInputHandler : ISeeingSharpInputHandler
    {
        private const float MOVEMENT = 0.3f;
        private const float ROTATION = 0.01f;

        #region objects from outside
        private SeeingSharpPanelPainter m_painter;
        private IInputEnabledView m_focusHandler;
        private RenderLoop m_renderLoop;
        private Camera3DBase m_camera;
        #endregion

        #region local resources
        private Button m_dummyButtonForFocus;
        private bool m_hasFocus;
        #endregion

        #region state variables for camera movement
        private bool m_isDragging;
        private PointerPoint m_lastDragPoint;
        private List<VirtualKey> m_pressedKeys;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="WinRTKeyAndMouseInputHandler"/> class.
        /// </summary>
        public WinRTKeyAndMouseInputHandler()
        {
            m_pressedKeys = new List<VirtualKey>();
        }

        /// <summary>
        /// Gets a list containing all supported view types.
        /// </summary>
        public Type[] GetSupportedViewTypes()
        {
            return new Type[]
            {
                typeof(SeeingSharpPanelPainter)
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
        /// <param name="cameraObject">The camera object (e. g. PerspectiveCamera3D).</param>
        public void Start(object viewObject, object cameraObject)
        {
            m_painter = viewObject as SeeingSharpPanelPainter;
            if (m_painter == null) { throw new ArgumentException("Unable to handle given view object!"); }

            m_focusHandler = m_painter as IInputEnabledView;
            if (m_focusHandler == null) { throw new ArgumentException("Unable to handle given view object!"); }

            m_renderLoop = m_focusHandler.RenderLoop;
            if (m_renderLoop == null) { throw new ArgumentException("Unable to handle given view object!"); }

            m_camera = cameraObject as Camera3DBase;
            if (m_camera == null) { throw new ArgumentException("Unable to handle given camera object!"); }

            // Register all events
            m_painter.TargetPanel.PointerExited += OnTargetPanel_PointerExited;
            m_painter.TargetPanel.PointerWheelChanged += OnTargetPanel_PointerWheelChanged;
            m_painter.TargetPanel.PointerPressed += OnTargetPanel_PointerPressed;
            m_painter.TargetPanel.PointerReleased += OnTargetPanel_PointerReleased;
            m_painter.TargetPanel.PointerMoved += OnTargetPanel_PointerMoved;
            m_painter.TargetPanel.KeyUp += OnTargetPanel_KeyUp;
            m_painter.TargetPanel.KeyDown += OnTargetPanel_KeyDown;

            // Create the dummy button for focus management
            //  see posts on: https://social.msdn.microsoft.com/Forums/en-US/54e4820d-d782-45d9-a2b1-4e3a13340788/set-focus-on-swapchainpanel-control?forum=winappswithcsharp
            m_dummyButtonForFocus = new Button();
            m_dummyButtonForFocus.Content = "Button";
            m_dummyButtonForFocus.Width = 0;
            m_dummyButtonForFocus.Height = 0;
            m_dummyButtonForFocus.HorizontalAlignment = HorizontalAlignment.Left;
            m_dummyButtonForFocus.VerticalAlignment = VerticalAlignment.Top;
            m_dummyButtonForFocus.KeyDown += OnDummyButtonForFocus_KeyDown;
            m_dummyButtonForFocus.KeyUp += OnDummyButtonForFocus_KeyUp;
            m_dummyButtonForFocus.LostFocus += OnDummyButtonForFocus_LostFocus;
            m_dummyButtonForFocus.GotFocus += OnDummyButtonForFocus_GotFocus;
            m_painter.TargetPanel.Children.Add(m_dummyButtonForFocus);

            // Set focus on the target 
            m_dummyButtonForFocus.Focus(FocusState.Programmatic);
        }

        /// <summary>
        /// Stops input handling.
        /// </summary>
        public void Stop()
        {
            m_hasFocus = false;

            // Remove the dummy button
            if (m_dummyButtonForFocus != null)
            {
                m_dummyButtonForFocus.KeyDown -= OnDummyButtonForFocus_KeyDown;
                m_dummyButtonForFocus.KeyUp -= OnDummyButtonForFocus_KeyUp;
                m_dummyButtonForFocus.LostFocus -= OnDummyButtonForFocus_LostFocus;
                m_dummyButtonForFocus.GotFocus -= OnDummyButtonForFocus_GotFocus;

                m_painter.TargetPanel.Children.Remove(m_dummyButtonForFocus);
            }

            // Deregister all events
            m_painter.TargetPanel.PointerExited -= OnTargetPanel_PointerExited;
            m_painter.TargetPanel.PointerWheelChanged -= OnTargetPanel_PointerWheelChanged;
            m_painter.TargetPanel.PointerPressed -= OnTargetPanel_PointerPressed;
            m_painter.TargetPanel.PointerReleased -= OnTargetPanel_PointerReleased;
            m_painter.TargetPanel.PointerMoved -= OnTargetPanel_PointerMoved;
            m_painter.TargetPanel.KeyUp -= OnTargetPanel_KeyUp;
            m_painter.TargetPanel.KeyDown -= OnTargetPanel_KeyDown;
        }

        /// <summary>
        /// Generic method thet gets iteratively after this handler was started.
        /// </summary>
        public void UpdateMovement()
        {
            // Define multiplyer
            float multiplyer = 1f;

            // Perform moving bassed on keyboard
            foreach (VirtualKey actKey in m_pressedKeys)
            {
                switch (actKey)
                {
                    case VirtualKey.Up:
                    case VirtualKey.W:
                        m_camera.Zoom(MOVEMENT * multiplyer);
                        break;

                    case VirtualKey.Down:
                    case VirtualKey.S:
                        m_camera.Zoom(-MOVEMENT * multiplyer);
                        break;

                    case VirtualKey.Left:
                    case VirtualKey.A:
                        m_camera.Strave(-MOVEMENT * multiplyer);
                        break;

                    case VirtualKey.Right:
                    case VirtualKey.D:
                        m_camera.Strave(MOVEMENT * multiplyer);
                        break;

                    case VirtualKey.Q:
                    case VirtualKey.NumberPad3:
                        m_camera.Move(new Vector3(0f, -MOVEMENT * multiplyer, 0f));
                        break;

                    case VirtualKey.E:
                    case VirtualKey.NumberPad9:
                        m_camera.Move(new Vector3(0f, MOVEMENT * multiplyer, 0f));
                        break;

                    case VirtualKey.NumberPad4:
                        m_camera.Rotate(ROTATION, 0f);
                        break;

                    case VirtualKey.NumberPad2:
                        m_camera.Rotate(0f, -ROTATION);
                        break;

                    case VirtualKey.NumberPad6:
                        m_camera.Rotate(-ROTATION, 0f);
                        break;

                    case VirtualKey.NumberPad8:
                        m_camera.Rotate(0f, ROTATION);
                        break;
                }
            }
        }

        /// <summary>
        /// Starts camera dragging.
        /// </summary>
        /// <param name="currentPoint"></param>
        private void StartCameraDragging(PointerPoint currentPoint)
        {
            m_isDragging = true;
            m_lastDragPoint = currentPoint;
        }

        /// <summary>
        /// Stops camera dragging.
        /// </summary>
        private void StopCameraDragging()
        {
            m_isDragging = false;
        }

        private void OnTargetPanel_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (!m_pressedKeys.Contains(e.Key)) { m_pressedKeys.Add(e.Key); }
        }

        private void OnTargetPanel_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            while (m_pressedKeys.Remove(e.Key)) { }
        }

        private void OnDummyButtonForFocus_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            // This enables bubbling of the keyboard event
            e.Handled = false;
        }

        private void OnDummyButtonForFocus_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            // This enables bubbling of the keyboard event
            e.Handled = false;
        }

        private void OnDummyButtonForFocus_LostFocus(object sender, RoutedEventArgs e)
        {
            m_pressedKeys.Clear();

            m_hasFocus = false;
        }

        private void OnDummyButtonForFocus_GotFocus(object sender, RoutedEventArgs e)
        {
            m_hasFocus = true;
        }

        private void OnTargetPanel_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Set focus on target
            if (m_dummyButtonForFocus != null)
            {
                m_dummyButtonForFocus.Focus(FocusState.Programmatic);
            }

            StopCameraDragging();

            // Needed here because we loose focus again by default on left mouse button
            e.Handled = true;
        }

        private void OnTargetPanel_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Set focus on target
            if(m_dummyButtonForFocus != null)
            {
                m_dummyButtonForFocus.Focus(FocusState.Programmatic);
            }

            StartCameraDragging(e.GetCurrentPoint(m_painter.TargetPanel));

            // Needed here because we loose focus again by default on left mouse button
            e.Handled = true;
        }

        private void OnTargetPanel_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Camera3DBase camera = m_renderLoop.Camera;
            if ((camera != null) &&
                (m_isDragging))
            {
                // Set focus on target
                if (m_dummyButtonForFocus != null)
                {
                    m_dummyButtonForFocus.Focus(FocusState.Programmatic);
                }

                // Calculate move distance
                PointerPoint currentPoint = e.GetCurrentPoint(m_painter.TargetPanel);
                Vector2 moveDistance = new Vector2(
                    (float)(currentPoint.Position.X - m_lastDragPoint.Position.X),
                    (float)(currentPoint.Position.Y - m_lastDragPoint.Position.Y));

                if (currentPoint.Properties.IsLeftButtonPressed)
                {
                    camera.Strave((float)((double)moveDistance.X / 50));
                    camera.UpDown((float)(-(double)moveDistance.Y / 50));
                }
                else if (currentPoint.Properties.IsRightButtonPressed)
                {
                    camera.Rotate(
                         (float)(-(double)moveDistance.X / 300),
                         (float)(-(double)moveDistance.Y / 300));
                }

                m_lastDragPoint = currentPoint;
            }
        }

        private void OnTargetPanel_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (!m_hasFocus) { return; }

            Camera3DBase camera = m_renderLoop.Camera;
            if (camera != null)
            {
                camera.Zoom((float)(e.GetCurrentPoint(m_painter.TargetPanel).Properties.MouseWheelDelta / 100.0));
            }
        }

        /// <summary>
        /// Called when mouse leaves the target panel.
        /// </summary>
        private void OnTargetPanel_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            StopCameraDragging();
        }
    }
}
#endif