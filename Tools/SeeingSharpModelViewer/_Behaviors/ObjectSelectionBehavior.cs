#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# Tools. More info at 
     - https://github.com/RolandKoenig/SeeingSharp/tree/master/Tools (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
	This program is distributed under the terms of the Microsoft Public License (Ms-PL)-
	More info at https://msdn.microsoft.com/en-us/library/ff647676.aspx
*/
#endregion
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Multimedia.Views;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace SeeingSharpModelViewer
{
    public class ObjectSelectionBehavior : Behavior<SeeingSharpRendererElement>
    {
        #region Refresh timer
        private DispatcherTimer m_refreshTimer;
        private bool m_onRefresh;
        #endregion

        #region Mouse state
        private bool m_isMouseInside;
        private Point m_mouseLocation;
        #endregion

        #region Scene state
        private Scene m_scene;
        private RenderLoop m_renderLoop;
        private GenericObject m_hoveredObject;
        private GenericObject m_selectedObject;
        #endregion

        public ObjectSelectionBehavior()
        {
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            if(m_refreshTimer == null)
            {
                m_scene = this.AssociatedObject.Scene;
                m_renderLoop = this.AssociatedObject.RenderLoop;

                this.AssociatedObject.MouseMove += OnAssociatedObject_MouseMove;
                this.AssociatedObject.MouseLeave += OnAssociatedObject_MouseLeave;
                this.AssociatedObject.MouseEnter += OnAssociatedObject_MouseEnter;

                m_refreshTimer = new DispatcherTimer(DispatcherPriority.Input);
                m_refreshTimer.Tick += OnRefeshTimer_Tick;
                m_refreshTimer.Interval = TimeSpan.FromMilliseconds(100.0);
                m_refreshTimer.Start();
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if(m_refreshTimer != null)
            {
                this.AssociatedObject.MouseMove -= OnAssociatedObject_MouseMove;
                this.AssociatedObject.MouseLeave -= OnAssociatedObject_MouseLeave;
                this.AssociatedObject.MouseEnter -= OnAssociatedObject_MouseEnter;

                m_refreshTimer.Stop();
                m_refreshTimer.Tick -= OnRefeshTimer_Tick;
                m_refreshTimer = null;
            }
        }

        private async void OnRefeshTimer_Tick(object sender, EventArgs e)
        {
            if (sender != m_refreshTimer) { return; }
            if (this.AssociatedObject == null) { return; }

            if (m_onRefresh) { return; }
            m_onRefresh = true;
            try
            {
                // Check for empty position
                if(m_mouseLocation == new Point()) { return; }

                System.Diagnostics.Debug.WriteLine($"{m_mouseLocation.X:F2}, {m_mouseLocation.Y:F2}");

                // Pick objects
                List<SceneObject> pickedObjects = await m_renderLoop.PickObjectAsync(
                    new SeeingSharp.Point((int)m_mouseLocation.X, (int)m_mouseLocation.Y),
                    new PickingOptions());
                GenericObject topUnderCursor = pickedObjects
                    .Where((actPicked) => actPicked is GenericObject)
                    .FirstOrDefault() as GenericObject;

                // Check current state after async action
                if (sender != m_refreshTimer) { return; }
                if (this.AssociatedObject == null) { return; }

                // Sync
                GenericObject newHovered = null;
                if((topUnderCursor != null) && (m_selectedObject != topUnderCursor)) { newHovered = topUnderCursor; }

                // Apply changes to the scene
                if (m_hoveredObject != newHovered)
                {
                    await m_scene.ManipulateSceneAsync(manipulator =>
                    {
                        if(m_hoveredObject != null) { manipulator.ClearLayer(Constants.LAYER_HOVER); }
                        m_hoveredObject = newHovered;
                        if(m_hoveredObject != null)
                        {
                            GenericObject newHovering = new GenericObject(m_hoveredObject.GeometryResourceKey);
                            newHovering.TransformationType = SpacialTransformationType.TakeFromOtherObject;
                            newHovering.TransformSourceObject = newHovered;
                            newHovering.Color = SeeingSharp.Color.Blue;
                            manipulator.Add(newHovering, Constants.LAYER_HOVER);
                        }
                    });
                }
            }
            finally
            {
                m_onRefresh = false;
            }
        }

        private void OnAssociatedObject_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            m_isMouseInside = true;
        }

        private void OnAssociatedObject_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            m_isMouseInside = false;
        }

        private void OnAssociatedObject_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!m_isMouseInside) { m_isMouseInside = true; }

            m_mouseLocation = this.AssociatedObject.GetPixelLocation(
                e.GetPosition(this.AssociatedObject));
        }
    }
}
