using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeeingSharp.DesktopFullscreenTest
{
    static class Program
    {
        private static FullscreenRenderTarget s_renderTarget;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Idle += OnApplication_FirstIdle;
            Application.Run();
        }

        private static async void OnApplication_FirstIdle(object sender, EventArgs e)
        {
            Application.Idle -= OnApplication_FirstIdle;

            await GraphicsCore.InitializeDefaultAsync();

            s_renderTarget = new FullscreenRenderTarget(
                GraphicsCore.Current.DefaultOutput);
            s_renderTarget.ClearColor = Color4.CornflowerBlue;
        }
    }
}
