using FrozenSky.Infrastructure;
using FrozenSky.Multimedia.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FrozenSky.Multimedia
{
    public abstract class FrozenSkyWpfGraphicsApplication : Application
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
                this.GetType().Assembly,
                new Assembly[]{ Assembly.GetExecutingAssembly() },
                new string[0]);
            GraphicsCore.Initialize(TargetHardware.Direct3D11, false);

            // Initialize UI environment
            FrozenSkyApplication.Current.InitializeUIEnvironment();

            // Create and open the main window
            ShowMainWindow();
        }

        /// <summary>
        /// Shows the main window.
        /// </summary>
        protected abstract void ShowMainWindow();
    }
}
