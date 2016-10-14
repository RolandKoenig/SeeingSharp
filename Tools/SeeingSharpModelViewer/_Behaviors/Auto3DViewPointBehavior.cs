#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# Tools. More info at
     - https://github.com/RolandKoenig/SeeingSharp/tree/master/Tools (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)

	This program is distributed under the terms of the Microsoft Public License (Ms-PL)-
	More info at https://msdn.microsoft.com/en-us/library/ff647676.aspx
*/
#endregion License information (SeeingSharp and all based games/applications)

using SeeingSharp;
using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Views;
using SeeingSharp.Util;
using System;
using System.Windows.Interactivity;

namespace SeeingSharpModelViewer
{
    public class Auto3DViewPointBehavior : Behavior<SeeingSharpRendererElement>
    {
        private IDisposable m_subscription;

        protected override void OnAttached()
        {
            base.OnAttached();

            m_subscription = SeeingSharpApplication.Current.UIMessenger.Subscribe<NewModelLoadedMessage>(
                OnMessage_NewModelLoadedMessage);
            this.AssociatedObject.Loaded += OnAssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.Loaded -= OnAssociatedObject_Loaded;
            CommonTools.SafeDispose(ref m_subscription);
        }

        private async void ApplyNewCameraLocation()
        {
            if (this.AssociatedObject == null) { return; }

            await this.AssociatedObject.RenderLoop.WaitForNextFinishedRenderAsync();

            await this.AssociatedObject.RenderLoop.MoveCameraToDefaultLocationAsync(
                EngineMath.RAD_45DEG, EngineMath.RAD_45DEG);
        }

        private void OnAssociatedObject_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ApplyNewCameraLocation();
        }

        private void OnMessage_NewModelLoadedMessage(NewModelLoadedMessage message)
        {
            this.ApplyNewCameraLocation();
        }
    }
}