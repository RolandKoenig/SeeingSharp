using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Components;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Multimedia.Views;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace SeeingSharp.HoloLense3DTest
{
    internal class AppView : IFrameworkView, IDisposable
    {
        #region Main window
        private CoreWindow m_mainWindow;
        private SeeingSharpHolographicsSpacePainter m_mainWindowPainter;
        #endregion

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
            GraphicsCore.Initialize(enableDebug: true);

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
            RenderLoop targetRenderLoop = m_mainWindowPainter.RenderLoop;
            Camera3DBase camera = targetRenderLoop.Camera;

            // Configure camera
            camera.Position = new Vector3(-5f, -5f, -5f);
            camera.Target = new Vector3(0f, 0.5f, 0f);
            camera.UpdateCamera();

            targetRenderLoop.Scene.ManipulateSceneAsync((manipulator) =>
            {
                // Create pallet geometry resource
                PalletType pType = new PalletType();
                pType.ContentColor = Color4.Transparent;
                var resPalletGeometry = manipulator.AddResource<GeometryResource>(
                    () => new GeometryResource(pType));

                // Create pallet object
                GenericObject palletObject = manipulator.AddGeneric(resPalletGeometry);
                palletObject.Position = new Vector3();
                palletObject.Color = Color4.GreenColor;
                palletObject.EnableShaderGeneratedBorder();
                palletObject.BuildAnimationSequence()
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_180DEG, 0f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_360DEG, 0f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .CallAction(() => palletObject.RotationEuler = Vector3.Zero)
                    .ApplyAndRewind();

            }).FireAndForget();

            targetRenderLoop.SceneComponents.Add(
                new FreeMovingCameraComponent());

            m_mainWindow.Activate();
            m_mainWindow.Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessUntilQuit);
        }

        public void SetWindow(CoreWindow window)
        {
            m_mainWindow = window;

            m_mainWindowPainter = new SeeingSharpHolographicsSpacePainter(window);
        }

        public void Uninitialize()
        {

        }

        public void Dispose()
        {

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
