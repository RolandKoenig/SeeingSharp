using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace SeeingSharp.UwpWithoutXamlTest
{
    internal class AppView : IFrameworkView, IDisposable
    {
        private bool m_windowClosed;

        public void Initialize(CoreApplicationView applicationView)
        {
            SeeingSharpApplication.InitializeAsync(
                this.GetType().GetTypeInfo().Assembly,
                new Assembly[]
                {
                        typeof(SeeingSharpApplication).GetTypeInfo().Assembly,
                        typeof(GraphicsCore).GetTypeInfo().Assembly
                },
                new string[] { }).Wait();
            GraphicsCore.Initialize();

            applicationView.Activated += this.OnViewActivated;

            // Register event handlers for app lifecycle.
            CoreApplication.Suspending += this.OnSuspending;
            CoreApplication.Resuming += this.OnResuming;
        }

        public void Load(string entryPoint)
        {

        }

        /// <summary>
        /// A method that starts the app view.
        /// </summary>
        public void Run()
        {
            while(!m_windowClosed)
            {

            }
        }

        public void SetWindow(CoreWindow window)
        {
            // Register for notification that the app window is being closed.
            window.Closed += this.OnWindowClosed;
        }

        public void Uninitialize()
        {

        }

        public void Dispose()
        {

        }

        private void OnWindowClosed(CoreWindow sender, CoreWindowEventArgs arg)
        {
            m_windowClosed = true;
        }

        /// <summary>
        /// Called when the app view is activated. Activates the app's CoreWindow.
        /// </summary>
        private void OnViewActivated(CoreApplicationView sender, IActivatedEventArgs args)
        {
            sender.CoreWindow.Activate();
        }

        private void OnSuspending(object sender, SuspendingEventArgs args)
        {
            // Save app state asynchronously after requesting a deferral. Holding a deferral
            // indicates that the application is busy performing suspending operations. Be
            // aware that a deferral may not be held indefinitely; after about five seconds,
            // the app will be forced to exit.
            var deferral = args.SuspendingOperation.GetDeferral();

            Task.Run(() =>
            {
                //deviceResources.Trim();

                //if (null != main)
                //{
                //    main.SaveAppState();
                //}

                ////
                //// TODO: Insert code here to save your app state.
                ////

                deferral.Complete();
            });
        }

        private void OnResuming(object sender, object args)
        {
            //// Restore any data or state that was unloaded on suspend. By default, data
            //// and state are persisted when resuming from suspend. Note that this event
            //// does not occur if the app was previously terminated.

            //if (null != main)
            //{
            //    main.LoadAppState();
            //}

            ////
            //// TODO: Insert code here to load your app state.
            ////
        }
    }
}
