using FrozenSky.Infrastructure;
using FrozenSky.Multimedia.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Tests.Rendering
{
    internal static class UnitTestHelper
    {
        public static async Task InitializeWithGrahicsAsync()
        {
            // Initialize main application singleton
            if (!FrozenSkyApplication.IsInitialized)
            {
                await FrozenSkyApplication.InitializeAsync(
                    Assembly.GetExecutingAssembly(),
                    new Assembly[]{ typeof(GraphicsCore).Assembly },
                    new string[0]);
            }

            // Initialize the graphics engine
            if (!GraphicsCore.IsInitialized) 
            { 
                GraphicsCore.Initialize(TargetHardware.Direct3D11, false);
                GraphicsCore.Current.SetDefaultDeviceToSoftware();
                GraphicsCore.Current.DefaultDevice.ForceDetailLevel(DetailLevel.High);
            }

            Assert.IsTrue(GraphicsCore.IsInitialized, "GraphicsCore could not be initialized!");
        }
    }
}
