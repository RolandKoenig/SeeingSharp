using FrozenSky.Infrastructure;
using FrozenSky.Multimedia.Core;
using FrozenSky.Util;
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
            if (!FrozenSkyApplication.IsInitialized)
            {
                FrozenSkyApplication.InitializeAsync(
                    typeof(App).GetTypeInfo().Assembly,
                    new Assembly[]
                    {
                        typeof(FrozenSkyApplication).GetTypeInfo().Assembly,
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
