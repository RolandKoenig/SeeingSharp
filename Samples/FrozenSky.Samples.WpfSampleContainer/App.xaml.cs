using FrozenSky.Infrastructure;
using FrozenSky.Multimedia.Core;
using FrozenSky.Samples.Base;
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace WpfSampleContainer
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Default initializations
            FrozenSkyApplication.InitializeAsync(
                Assembly.GetExecutingAssembly(),
                new Assembly[]{
                    typeof(GraphicsCore).Assembly,
                    typeof(SampleBase).Assembly
                },
                new string[0]).Wait();
            FrozenSkyApplication.Current.InitializeUIEnvironment();
            GraphicsCore.Initialize(TargetHardware.Direct3D11, false);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
        }
    }
}
