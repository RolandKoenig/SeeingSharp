﻿#region License information (FrozenSky and all based games/applications)
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
using Xunit;
using FrozenSky.Util;

namespace FrozenSky.Tests.Rendering
{
    [Collection("Rendering_FrozenSky")]
    public class BasicTests
    {
        public const string TEST_CATEGORY = "FrozenSky Multimedia Basics";

        /// <summary>
        /// Checks for existence of a software device.
        /// </summary>
        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task Check_SoftwareDevice()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            EngineDevice softwareDevice = GraphicsCore.Current.LoadedDevices.FirstOrDefault(
                (actDevice) => actDevice.IsSoftware);

            Assert.NotNull(softwareDevice);
        }

        /// <summary>
        /// Check for existence of a default device.
        /// </summary>
        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task Check_DefaultDevice()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            Assert.NotNull(GraphicsCore.Current.DefaultDevice);
        }

        /// <summary>
        /// This method checks for correct behavior on device switches.
        /// </summary>
        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task Check_DeviceSwitch()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

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

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void Check_SceneMessageHandler_Registration_Deregistration()
        {
            Scene dummyScene = new Scene(
                name: "DummyScene",
                registerForMessaging: true);
            try
            {
                Assert.True(FrozenSkyMessageHandler.CountGlobalMessageHandlers == 1);
                Assert.True(FrozenSkyMessageHandler.GetByName("DummyScene") == dummyScene.MessageHandler);
            }
            finally
            {
                dummyScene.DeregisterMessaging();
            }

            Assert.True(FrozenSkyMessageHandler.CountGlobalMessageHandlers == 0);
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void Check_SceneMessageHandler_WrongPublish()
        {
            Scene dummyScene = new Scene(
                name: "DummyScene",
                registerForMessaging: true);
            FrozenSkyException publishException = null;
            try
            {
                dummyScene.MessageHandler.Publish<DummyMessage>();
            }
            catch(FrozenSkyException ex)
            {
                publishException = ex;
            }
            finally
            {
                dummyScene.DeregisterMessaging();
            }

            Assert.NotNull(publishException);
            Assert.True(FrozenSkyMessageHandler.CountGlobalMessageHandlers == 0);
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task Check_SceneMessageHandler_CorrectPublish()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            using (MemoryRenderTarget renderTarget = new MemoryRenderTarget(1024, 1024))
            {
                Scene dummyScene = new Scene(
                    name: "DummyScene",
                    registerForMessaging: true);
                renderTarget.Scene = dummyScene;

                Exception publishException = null;
                try
                {
                    await dummyScene.PerformBeforeUpdateAsync(() =>
                    {
                        dummyScene.MessageHandler.Publish<DummyMessage>();
                    });
                }
                catch (Exception ex)
                {
                    publishException = ex;
                }
                finally
                {
                    dummyScene.DeregisterMessaging();
                }

                Assert.Null(publishException);
                Assert.True(FrozenSkyMessageHandler.CountGlobalMessageHandlers == 0);
            }
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        private class DummyMessage : FrozenSkyMessage
        { 
        }
    }
}