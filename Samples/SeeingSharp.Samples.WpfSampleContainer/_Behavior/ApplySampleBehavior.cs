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
using SeeingSharp.Multimedia.Views;
using SeeingSharp.Samples.Base;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace WpfSampleContainer
{
    public class ApplySampleBehavior : Behavior<SeeingSharpRendererElement>
    {
        public static readonly DependencyProperty BottomStatusBarProperty =
            DependencyProperty.Register("BottomStatusBar", typeof(FrameworkElement), typeof(ApplySampleBehavior), new PropertyMetadata(null));
        public static readonly DependencyProperty MainContentGridProperty =
            DependencyProperty.Register("MainContentGrid", typeof(FrameworkElement), typeof(ApplySampleBehavior), new PropertyMetadata(null));

        private IEnumerable<MessageSubscription> m_subscriptions;
        private SampleBase m_appliedSample;
        private bool m_isChangingSample;

        protected override void OnAttached()
        {
            base.OnAttached();

            m_appliedSample = null;
            m_subscriptions = SeeingSharpApplication.Current.UIMessenger.SubscribeAll(this);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            m_subscriptions.ForEachInEnumeration(
                (actSubscription) => actSubscription.Unsubscribe());
            m_subscriptions = null;
        }

        private async void OnMessage_Received(MessageSampleChanged message)
        {
            SeeingSharpRendererElement renderElement = base.AssociatedObject;
            if(renderElement == null){ return; }
            if (m_isChangingSample) { return; }

            m_isChangingSample = true;
            try
            {
                if (MainContentGrid != null) { MainContentGrid.IsEnabled = false; }
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
                    await renderElement.RenderLoop.Scene.ManipulateSceneAsync((manipulator) =>
                        {
                            manipulator.Clear(true);
                        });

                    // Apply new scene
                    m_appliedSample = SampleFactory.Current.ApplySample(
                        renderElement.RenderLoop,
                        message.NewSample.SampleDescription);

                    // Ensure that we see all objects of the newly loaded sample
                    await renderElement.RenderLoop.WaitForNextFinishedRenderAsync();
                    await renderElement.RenderLoop.WaitForNextFinishedRenderAsync();
                }
            }
            finally
            {
                m_isChangingSample = false;
                if (MainContentGrid != null) { MainContentGrid.IsEnabled = true; }
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
    }
}
