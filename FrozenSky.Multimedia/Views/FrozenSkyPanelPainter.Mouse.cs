#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2014 Roland König (RolandK)

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
using FrozenSky.Multimedia.Drawing3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input;
using Windows.UI.Xaml.Input;

namespace FrozenSky.Multimedia.Views
{
    public partial class FrozenSkyPanelPainter 
    {
        private bool m_isDragging;
        private PointerPoint m_lastDragPoint;

        /// <summary>
        /// Initializes simple camera control.
        /// </summary>
        private void InitializeMouseMovement()
        {
            //m_targetPanel.PointerExited += OnTargetPanelPointerExited;
            //m_targetPanel.PointerWheelChanged += OnTargetPanelPointerWheelChanged;
            //m_targetPanel.PointerPressed += OnTargetPanelPointerPressed;
            //m_targetPanel.PointerReleased += OnTargetPanelPointerReleased;
            //m_targetPanel.PointerMoved += OnTargetPanelPointerMoved;
        }

        private void UnloadMouseMovement()
        {
            //m_targetPanel.PointerExited -= OnTargetPanelPointerExited;
            //m_targetPanel.PointerWheelChanged -= OnTargetPanelPointerWheelChanged;
            //m_targetPanel.PointerPressed -= OnTargetPanelPointerPressed;
            //m_targetPanel.PointerReleased -= OnTargetPanelPointerReleased;
            //m_targetPanel.PointerMoved -= OnTargetPanelPointerMoved;
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
            StartCameraDragging(e.GetCurrentPoint(m_targetPanel.Panel));
        }

        private void OnTargetPanelPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PerspectiveCamera3D perspectiveCamera = m_renderLoop.Camera as PerspectiveCamera3D;
            if ((perspectiveCamera != null) &&
                (m_isDragging))
            {
                PointerPoint currentPoint = e.GetCurrentPoint(m_targetPanel.Panel);

                Vector2 moveDistance = new Vector2(
                    (float)(currentPoint.Position.X - m_lastDragPoint.Position.X),
                    (float)(currentPoint.Position.Y - m_lastDragPoint.Position.Y));

                if (currentPoint.Properties.IsLeftButtonPressed)
                {
                    perspectiveCamera.Strave((float)((double)moveDistance.X / 50));
                    perspectiveCamera.UpDown((float)(-(double)moveDistance.Y / 50));
                }
                else if (currentPoint.Properties.IsRightButtonPressed)
                {
                    perspectiveCamera.Rotate(
                         (float)(-(double)moveDistance.X / 300),
                         (float)(-(double)moveDistance.Y / 300));
                }

               m_lastDragPoint = currentPoint;
            }
        }

        private void OnTargetPanelPointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            PerspectiveCamera3D perspectiveCamera = m_renderLoop.Camera as PerspectiveCamera3D;
            if (perspectiveCamera != null)
            {
                perspectiveCamera.Zoom((float)(e.GetCurrentPoint(m_targetPanel.Panel).Properties.MouseWheelDelta / 100.0));
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
