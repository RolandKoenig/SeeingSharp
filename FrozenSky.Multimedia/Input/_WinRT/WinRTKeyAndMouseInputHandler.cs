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
#if UNIVERSAL
using FrozenSky.Infrastructure;
using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
using FrozenSky.Multimedia.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input;
using Windows.UI.Xaml.Input;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(FrozenSky.Multimedia.Input.WinRTKeyAndMouseInputHandler),
    contractType: typeof(FrozenSky.Multimedia.Input.IFrozenSkyInputHandler))]

namespace FrozenSky.Multimedia.Input
{
    class WinRTKeyAndMouseInputHandler : IFrozenSkyInputHandler
    {
        #region objects from outside
        private FrozenSkyPanelPainter m_painter;
        private IInputEnabledView m_focusHandler;
        private RenderLoop m_renderLoop;
        private Camera3DBase m_camera;
        #endregion

        #region state variables for camera movement
        private bool m_isDragging;
        private PointerPoint m_lastDragPoint;
        #endregion

        /// <summary>
        /// Gets a list containing all supported view types.
        /// </summary>
        public Type[] GetSupportedViewTypes()
        {
            return new Type[]
            {
                typeof(FrozenSkyPanelPainter)
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
        /// Starts input handling.
        /// </summary>
        /// <param name="viewObject">The view object (e. g. Direct3D11Canvas).</param>
        /// <param name="cameraObject">The camera object (e. g. PerspectiveCamera3D).</param>
        public void Start(object viewObject, object cameraObject)
        {
            m_painter = viewObject as FrozenSkyPanelPainter;
            if (m_painter == null) { throw new ArgumentException("Unable to handle given view object!"); }

            m_focusHandler = m_painter as IInputEnabledView;
            if (m_focusHandler == null) { throw new ArgumentException("Unable to handle given view object!"); }

            m_renderLoop = m_focusHandler.RenderLoop;
            if (m_renderLoop == null) { throw new ArgumentException("Unable to handle given view object!"); }

            m_camera = cameraObject as Camera3DBase;
            if (m_camera == null) { throw new ArgumentException("Unable to handle given camera object!"); }

            // Register all events
            m_painter.TargetPanel.PointerExited += OnTargetPanelPointerExited;
            m_painter.TargetPanel.PointerWheelChanged += OnTargetPanelPointerWheelChanged;
            m_painter.TargetPanel.PointerPressed += OnTargetPanelPointerPressed;
            m_painter.TargetPanel.PointerReleased += OnTargetPanelPointerReleased;
            m_painter.TargetPanel.PointerMoved += OnTargetPanelPointerMoved;
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
            m_painter.TargetPanel.PointerExited -= OnTargetPanelPointerExited;
            m_painter.TargetPanel.PointerWheelChanged -= OnTargetPanelPointerWheelChanged;
            m_painter.TargetPanel.PointerPressed -= OnTargetPanelPointerPressed;
            m_painter.TargetPanel.PointerReleased -= OnTargetPanelPointerReleased;
            m_painter.TargetPanel.PointerMoved -= OnTargetPanelPointerMoved;
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

        private void OnTargetPanelPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            StopCameraDragging();
        }

        private void OnTargetPanelPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            StartCameraDragging(e.GetCurrentPoint(m_painter.TargetPanel));
        }

        private void OnTargetPanelPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Camera3DBase camera = m_renderLoop.Camera;
            if ((camera != null) &&
                (m_isDragging))
            {
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

        private void OnTargetPanelPointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            Camera3DBase camera = m_renderLoop.Camera;
            if (camera != null)
            {
                camera.Zoom((float)(e.GetCurrentPoint(m_painter.TargetPanel).Properties.MouseWheelDelta / 100.0));
            }
        }

        /// <summary>
        /// Called when mouse leaves the target panel.
        /// </summary>
        private void OnTargetPanelPointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            StopCameraDragging();
        }
    }
}
#endif