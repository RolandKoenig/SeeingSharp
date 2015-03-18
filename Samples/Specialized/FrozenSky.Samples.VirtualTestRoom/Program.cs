using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using FrozenSky.Infrastructure;
using FrozenSky.Multimedia.Core;

namespace FrozenSky.Samples.VirtualTestRoom
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Default initializations
            FrozenSkyApplication.InitializeAsync(
                Assembly.GetExecutingAssembly(),
                new Assembly[]{
                    typeof(GraphicsCore).Assembly
                },
                new string[0]).Wait();
            GraphicsCore.Initialize(TargetHardware.Direct3D11, false);

            // Run the application
            MainWindow mainWindow = new MainWindow();
            FrozenSkyApplication.Current.InitializeUIEnvironment();
            Application.Run(mainWindow);
        }
    }
}
