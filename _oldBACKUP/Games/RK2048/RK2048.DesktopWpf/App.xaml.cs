using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace RK2048.DesktopWpf
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            // Initialize application and graphics
            if (!SeeingSharpApplication.IsInitialized)
            {
                SeeingSharpApplication.InitializeAsync(
                    typeof(App).GetTypeInfo().Assembly,
                    new Assembly[]
                    {
                        typeof(SeeingSharpApplication).GetTypeInfo().Assembly,
                        typeof(GraphicsCore).GetTypeInfo().Assembly
                    },
                    new string[] { })
                    .Wait();
                GraphicsCore.Initialize(
                    TargetHardware.Direct3D11,
                    false);
            }
        }
    }
}
