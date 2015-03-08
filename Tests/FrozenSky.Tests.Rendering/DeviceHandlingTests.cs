#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2014 Roland König (RolandK)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Views;
using FrozenSky.Infrastructure;
using System.Reflection;

namespace FrozenSky.Tests.Rendering
{
    [TestClass]
    public class DeviceHandlingTests
    {
        public const string TEST_CATEGORY = "FrozenSky Multimedia Device-Handling";

        [TestInitialize]
        public void Initialize()
        {
            UnitTestHelper.InitializeWithGrahicsAsync().Wait();
        }

        /// <summary>
        /// Checks for existence of a software device.
        /// </summary>
        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CheckForSoftwareDevice()
        {
            EngineDevice softwareDevice = GraphicsCore.Current.LoadedDevices.FirstOrDefault(
                (actDevice) => actDevice.IsSoftware);

            Assert.IsNotNull(softwareDevice, "No software device created!");
        }

        /// <summary>
        /// Check for existence of a default device.
        /// </summary>
        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CheckForDefaultDevice()
        {
            Assert.IsNotNull(GraphicsCore.Current.DefaultDevice, "Default device not set!");
        }

        /// <summary>
        /// This method checks for correct behavior on device switches.
        /// </summary>
        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CheckDeviceSwitch()
        {
            List<EngineDevice> devices = new List<EngineDevice>(GraphicsCore.Current.LoadedDevices);
            if (devices.Count < 2) { return; }

            using (MemoryRenderTarget renderTarget = new MemoryRenderTarget(1024, 1024))
            {
                renderTarget.ClearColor = Color4.CornflowerBlue;

                for (int loop = 0; loop < 10; loop++)
                {
                    renderTarget.RenderLoop.SetRenderingDevice(devices[0]);
                    renderTarget.AwaitRenderAsync().Wait(1000);

                    renderTarget.RenderLoop.SetRenderingDevice(devices[1]);
                    renderTarget.AwaitRenderAsync().Wait(1000);
                }
            }
        }
    }
}
