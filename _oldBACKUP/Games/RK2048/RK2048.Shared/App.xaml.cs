using SeeingSharp;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// Die Vorlage "Leere Anwendung" ist unter http://go.microsoft.com/fwlink/?LinkId=234227 dokumentiert.

namespace RK2048
{
    /// <summary>
    /// Stellt das anwendungsspezifische Verhalten bereit, um die Standardanwendungsklasse zu ergänzen.
    /// </summary>
    public sealed partial class App : Application
    {
        /// <summary>
        /// Initialisiert das Singletonanwendungsobjekt. Dies ist die erste Zeile von erstelltem Code
        /// und daher das logische Äquivalent von main() bzw. WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Anwendung durch den Endbenutzer normal gestartet wird.  Weitere Einstiegspunkte
        /// werden verwendet, wenn die Anwendung zum Öffnen einer bestimmten Datei, zum Anzeigen
        /// von Suchergebnissen usw. gestartet wird.
        /// </summary>
        /// <param name="e">Details über Startanforderung und -prozess.</param>
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
                        typeof(GraphicsCore).GetTypeInfo().Assembly
                    },
                    new string[] { e.Arguments });
                try
                {
                    GraphicsCore.Initialize(
                        TargetHardware.Direct3D11,
                        false);

#if WINDOWS_APP
                    // Force high texture quality on tablet devices
                    foreach(EngineDevice actDevice in GraphicsCore.Current.LoadedDevices)
                    {
                        if (actDevice.IsSoftware) { continue; }
                        actDevice.Configuration.TextureQuality = TextureQuality.Hight;
                    }
#endif
                }
                catch (Exception ex) 
                {
                    initException = ex;
                }
            }

            // Create the main game page and associate it withe the main window
            if (initException == null)
            {
                MainGamePage gamePage = new MainGamePage();
                Window.Current.Content = gamePage;
            }
            else
            {
                ExceptionInfo exInfo = new ExceptionInfo(initException);

                DummyPage exceptionPage = new DummyPage();
                exceptionPage.DataContext = exInfo;
                Window.Current.Content = exceptionPage;
            }

            // Ensure that the main window is activated
            Window.Current.Activate();
        }
    }
}