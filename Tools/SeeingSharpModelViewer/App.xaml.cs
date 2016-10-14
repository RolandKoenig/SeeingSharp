using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using System.Reflection;
using System.Windows;

namespace SeeingSharpModelViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected async override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Default initializations
            await SeeingSharpApplication.InitializeAsync(
                Assembly.GetExecutingAssembly(),
                new Assembly[]{
                    typeof(GraphicsCore).Assembly
                },
                new string[0]);

            // Initialize UI and Graphics
            SeeingSharpApplication.Current.InitializeUIEnvironment();
            GraphicsCore.Initialize(TargetHardware.Direct3D11, false);

            // Load the main window
            MainWindow mainWindow = new SeeingSharpModelViewer.MainWindow();
            SeeingSharpApplication.Current.InitializeAutoErrorReporting_Wpf(this, mainWindow);
            mainWindow.Show();
        }
    }
}