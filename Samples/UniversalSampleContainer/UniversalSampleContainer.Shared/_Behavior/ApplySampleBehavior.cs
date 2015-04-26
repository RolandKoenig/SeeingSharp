#region License information (SeeingSharp and all based games/applications)
/*
    SeeingSharp and all games/applications based on it (more info at http://www.rolandk.de/wp)
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
using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Views;
using SeeingSharp.Samples.Base;
using SeeingSharp.Util;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UniversalSampleContainer
{
    public class ApplySampleBehavior : DependencyObject, IBehavior
    {
        public static readonly DependencyProperty BottomStatusBarProperty =
            DependencyProperty.Register("BottomStatusBar", typeof(FrameworkElement), typeof(ApplySampleBehavior), new PropertyMetadata(null));
        public static readonly DependencyProperty MainContentGridProperty =
            DependencyProperty.Register("MainContentGrid", typeof(FrameworkElement), typeof(ApplySampleBehavior), new PropertyMetadata(null));

        private DependencyObject m_associatedObject;
        private SeeingSharpPanelPainter m_painter;
        private IEnumerable<MessageSubscription> m_subscriptions;
        private SampleBase m_appliedSample;
        private bool m_isChangingSample;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplySampleBehavior"/> class.
        /// </summary>
        public ApplySampleBehavior()
        {
            m_painter = new SeeingSharpPanelPainter();
            m_painter.Camera = new PerspectiveCamera3D();
        }

        /// <summary>
        /// Attaches the specified associated object.
        /// </summary>
        /// <param name="associatedObject">The associated object.</param>
        public void Attach(DependencyObject associatedObject)
        {
            m_associatedObject = associatedObject;
            m_appliedSample = null;
            m_subscriptions = SeeingSharpApplication.Current.UIMessenger.SubscribeAll(this);

            SwapChainPanel renderElement = m_associatedObject as SwapChainPanel;
            if(renderElement != null)
            {
                m_painter.Attach(renderElement);
            }
        }

        /// <summary>
        /// Detaches this instance.
        /// </summary>
        public void Detach()
        {
            SwapChainPanel renderElement = m_associatedObject as SwapChainPanel;
            if (renderElement != null)
            {
                m_painter.Detach();
            }

            m_subscriptions.ForEachInEnumeration(
                (actSubscription) => actSubscription.Unsubscribe());
            m_subscriptions = null;
        }

        private async void OnMessage_Received(MessageSampleChanged message)
        {
            SwapChainPanel renderElement = m_associatedObject as SwapChainPanel;
            if(renderElement == null){ return; }
            if (m_isChangingSample) { return; }

            m_isChangingSample = true;
            try
            {
                if (MainContentGrid != null) { MainContentGrid.IsHitTestVisible = false; }
                if (BottomStatusBar != null) { BottomStatusBar.Visibility = Visibility.Visible; }

                if (message.NewSample != null)
                {
                    // Sets closed state on currently applied sample
                    if (m_appliedSample != null)
                    {
                        m_appliedSample.SetClosed();
                        m_appliedSample = null;
                    }

                    // Clear previous scene first
                    await m_painter.RenderLoop.Scene.ManipulateSceneAsync((manipulator) =>
                        {
                            manipulator.Clear(true);
                        });

                    // Apply new scene
                    m_appliedSample = SampleFactory.Current.ApplySample(
                        m_painter.RenderLoop,
                        message.NewSample.SampleDescription);

                    // Ensure that we see all objects of the newly loaded sample
                    await m_painter.RenderLoop.WaitForNextFinishedRenderAsync();
                    await m_painter.RenderLoop.WaitForNextFinishedRenderAsync();
                }
            }
            finally
            {
                m_isChangingSample = false;
                if (MainContentGrid != null) { MainContentGrid.IsHitTestVisible = true; }
                if (BottomStatusBar != null) { BottomStatusBar.Visibility = Visibility.Collapsed; }
            }
        }

        public FrameworkElement MainContentGrid
        {
            get { return (FrameworkElement)GetValue(MainContentGridProperty); }
            set { SetValue(MainContentGridProperty, value); }
        }


        public FrameworkElement BottomStatusBar
        {
            get { return (FrameworkElement)GetValue(BottomStatusBarProperty); }
            set { SetValue(BottomStatusBarProperty, value); }
        }

        public DependencyObject AssociatedObject
        {
            get { return m_associatedObject; }
        }

        public Color ClearColor
        {
            get { return m_painter.ClearColor; }
            set { m_painter.ClearColor = value; }
        }
    }
}
