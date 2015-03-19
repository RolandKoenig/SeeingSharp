using FrozenSky.Infrastructure;
using FrozenSky.Multimedia.Core;
using FrozenSky.RKKinectLounge.Base;
using FrozenSky.RKKinectLounge.Modules.Kinect;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace FrozenSky.RKKinectLounge
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Main startup logic of the application.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.Windows.StartupEventArgs" />, das die Ereignisdaten enthält.</param>
        protected async override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Default initializations
            await FrozenSkyApplication.InitializeAsync(
                Assembly.GetExecutingAssembly(),
                new Assembly[]{
                    typeof(GraphicsCore).Assembly
                },
                new string[0]);
            GraphicsCore.Initialize(TargetHardware.Direct3D11, false);

            // Initialize UI environment
            FrozenSkyApplication.Current.InitializeUIEnvironment();

            // Load all ResourceDictionaries defined by loaded modules
            ModuleManager.LoadedModules.ForEach(actModule =>
            {
                foreach(var actDictionary in actModule.GetGlobalResourceDictionaries())
                {
                    this.Resources.MergedDictionaries.Add(actDictionary);
                }
            });

            // Create and open the main window
            MainWindow newMainWindow = new MainWindow();
            newMainWindow.Show();
            this.MainWindow = newMainWindow;
        }
    }
}
