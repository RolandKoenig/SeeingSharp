﻿#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Views;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Util;
using Xunit;
using SeeingSharp.Multimedia.Objects;

namespace SeeingSharp.Tests.Rendering
{
    [Collection("Rendering_SeeingSharp")]
    public class BasicTests
    {
        public const string TEST_CATEGORY = "SeeingSharp Multimedia Basics";

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

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void Check_ViewPoint_ReadWriterJson()
        {
            ResourceLink dummyFile = "DummyFile.json";
            Camera3DViewPoint viewPointOriginal = new Camera3DViewPoint();
            viewPointOriginal.CameraType = Camera3DType.Perspective;
            viewPointOriginal.OrthographicZoomFactor = 10f;
            viewPointOriginal.Position = new Vector3(2f, 3f, 4f);
            viewPointOriginal.Rotation = new Vector2(1f, 1.5f);

            viewPointOriginal.ToResourceLink(dummyFile);
            Camera3DViewPoint loadedOne = Camera3DViewPoint.FromResourceLink(dummyFile);

            Assert.Equal(viewPointOriginal.CameraType, loadedOne.CameraType);
            Assert.Equal(viewPointOriginal.OrthographicZoomFactor, loadedOne.OrthographicZoomFactor);
            Assert.Equal(viewPointOriginal.Position, loadedOne.Position);
            Assert.Equal(viewPointOriginal.Rotation, loadedOne.Rotation);
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
        public void Check_SceneMessenger_Registration_Deregistration()
        {
            Scene dummyScene = new Scene(
                name: "DummyScene",
                registerOnMessenger: true);
            try
            {
                Assert.True(SeeingSharpMessenger.CountGlobalMessengers == 1);
                Assert.True(SeeingSharpMessenger.GetByName("DummyScene") == dummyScene.Messenger);
            }
            finally
            {
                dummyScene.DeregisterMessaging();
            }

            Assert.True(SeeingSharpMessenger.CountGlobalMessengers == 0);
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void Check_SceneMessenger_WrongPublish()
        {
            Scene dummyScene = new Scene(
                name: "DummyScene",
                registerOnMessenger: true);
            SeeingSharpException publishException = null;
            try
            {
                dummyScene.Messenger.Publish<DummyMessage>();
            }
            catch (SeeingSharpException ex)
            {
                publishException = ex;
            }
            finally
            {
                dummyScene.DeregisterMessaging();
            }

            Assert.NotNull(publishException);
            Assert.True(SeeingSharpMessenger.CountGlobalMessengers == 0);
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task Check_SceneMessenger_CorrectPublish()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            Scene dummyScene = new Scene(
                name: "DummyScene",
                registerOnMessenger: true);
            Exception publishException = null;
            try
            {
                // Publish a message during scene update pass
                using (MemoryRenderTarget renderTarget = new MemoryRenderTarget(1024, 1024))
                {
                    renderTarget.Scene = dummyScene;
                    await dummyScene.PerformBeforeUpdateAsync(() =>
                    {
                        dummyScene.Messenger.Publish<DummyMessage>();
                    });
                }

                // Wait while the render loop is surely unsubscribed from main loop
                await GraphicsCore.Current.MainLoop.WaitForNextPassedLoop();
                await GraphicsCore.Current.MainLoop.WaitForNextPassedLoop();
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
            Assert.True(SeeingSharpMessenger.CountGlobalMessengers == 0);
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task Check_SceneInfo_SimpleQuery()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            using (MemoryRenderTarget memRenderTarget = new MemoryRenderTarget(1024, 1024))
            {
                memRenderTarget.ClearColor = Color4.CornflowerBlue;

                // Get and configure the camera
                PerspectiveCamera3D camera = memRenderTarget.Camera as PerspectiveCamera3D;
                camera.Position = new Vector3(0f, 5f, -7f);
                camera.Target = new Vector3(0f, 0f, 0f);
                camera.UpdateCamera();

                // Define scene
                SceneObject rootObject = null;
                await memRenderTarget.Scene.ManipulateSceneAsync((manipulator) =>
                {
                    NamedOrGenericKey geoResource = manipulator.AddResource<GeometryResource>(
                        () => new GeometryResource(new PalletType()));

                    GenericObject newObject = manipulator.AddGeneric(geoResource);
                    newObject.RotationEuler = new Vector3(0f, EngineMath.RAD_90DEG / 2f, 0f);
                    newObject.Scaling = new Vector3(2f, 2f, 2f);

                    rootObject = newObject;
                });

                SceneObjectInfo objInfo = await memRenderTarget.Scene.GetSceneObjectInfoAsync(rootObject);

                Assert.NotNull(objInfo);
                Assert.True(objInfo.OriginalObject == rootObject);
                Assert.True(objInfo.Type == SceneObjectInfoType.GenericObject);
            }

            // Finishing checks
            Assert.True(GraphicsCore.Current.MainLoop.RegisteredRenderLoopCount == 0, "RenderLoops where not disposed correctly!");
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        private class DummyMessage : SeeingSharpMessage
        { 
        }
    }
}
