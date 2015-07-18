using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace RKRocket
{
    /// <summary>
    /// Stellt das anwendungsspezifische Verhalten bereit, um die Standardanwendungsklasse zu ergänzen.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Lounches the SeeingSharp application.
        /// </summary>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Initialize application and graphics
            Exception initException = null;
            if (!SeeingSharpApplication.IsInitialized)
            {
                await SeeingSharpApplication.InitializeAsync(
                    this.GetType().GetTypeInfo().Assembly,
                    new Assembly[]
                    {
                        typeof(SeeingSharpApplication).GetTypeInfo().Assembly,
                        typeof(GraphicsCore).GetTypeInfo().Assembly,
                    },
                    new string[] { e.Arguments });
                try
                {
                    GraphicsCore.Initialize(
                        TargetHardware.Direct3D11,
                        false);

                    // Force high texture quality on tablet devices
                    foreach (EngineDevice actDevice in GraphicsCore.Current.LoadedDevices)
                    {
                        if (actDevice.IsSoftware) { continue; }
                        actDevice.Configuration.TextureQuality = TextureQuality.Hight;
                    }

                    // Initialize the UI environment
                    SeeingSharpApplication.Current.InitializeUIEnvironment();
                }
                catch (Exception ex)
                {
                    initException = ex;
                }
            }

            // Create the main game page and associate it withe the main window
            if (initException == null)
            {
                MainPage gamePage = new MainPage();
                Window.Current.Content = gamePage;
            }
            else
            {
                return;
                //ExceptionInfo exInfo = new ExceptionInfo(initException);

                //DummyPage exceptionPage = new DummyPage();
                //exceptionPage.DataContext = exInfo;
                //Window.Current.Content = exceptionPage;
            }

            // Ensure that the main window is activated
            Window.Current.Activate();
        }
    }
}
