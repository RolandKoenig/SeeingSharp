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
using Xunit;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Views;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Util;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Infrastructure;
using SeeingSharp;

// Some namespace mappings
using GDI = System.Drawing;

namespace SeeingSharp.Tests.Rendering
{
    [Collection("Rendering_SeeingSharp")]
    public class ModelLoadingAndRenderingTests
    {
        public const int MANIPULATE_WAIT_TIME = 500;
        public const string TEST_DUMMY_FILE_NAME = "UnitTest.Screenshot.png";
        public const string TEST_CATEGORY = "SeeingSharp Multimedia Model Loading and Rendering";

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task LoadAndRender_StlFile()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            using (MemoryRenderTarget memRenderTarget = new MemoryRenderTarget(1024, 1024))
            {
                memRenderTarget.ClearColor = Color4.CornflowerBlue;

                // Get and configure the camera
                PerspectiveCamera3D camera = memRenderTarget.Camera as PerspectiveCamera3D;
                camera.Position = new Vector3(-4f, 4f, -4f);
                camera.Target = new Vector3(2f, 0f, 2f);
                camera.UpdateCamera();

                // Import Fox model
                StlImportOptions importOptions = new StlImportOptions();
                importOptions.ResourceCoordinateSystem = CoordinateSystem.LeftHanded_UpZ;
                IEnumerable<SceneObject> loadedObjects = await memRenderTarget.Scene.ImportAsync(
                    new AssemblyResourceLink(
                        typeof(ModelLoadingAndRenderingTests),
                        "Ressources.Models.Fox.stl"),
                    importOptions);

                // Wait for it to be visible
                await memRenderTarget.Scene.WaitUntilVisibleAsync(loadedObjects, memRenderTarget.RenderLoop);

                // Take screenshot
                GDI.Bitmap screenshot = await memRenderTarget.RenderLoop.GetScreenshotGdiAsync();

                // Calculate and check difference
                bool isNearEqual = BitmapComparison.IsNearEqual(
                    screenshot, Properties.Resources.ReferenceImage_ModelStl);
                Assert.True(isNearEqual, "Difference to reference image is to big!");
            }

            // Finishing checks
            Assert.True(GraphicsCore.Current.MainLoop.RegisteredRenderLoopCount == 0, "RenderLoops where not disposed correctly!");
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task LoadAndRender_ACFlatShadedObject()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            using (MemoryRenderTarget memRenderTarget = new MemoryRenderTarget(1024, 1024))
            {
                memRenderTarget.ClearColor = Color4.CornflowerBlue;

                // Get and configure the camera
                PerspectiveCamera3D camera = memRenderTarget.Camera as PerspectiveCamera3D;
                camera.Position = new Vector3(-3f, 3f, -3f);
                camera.Target = new Vector3(2f, 0f, 2f);
                camera.UpdateCamera();

                // Define scene
                SceneObject newObject = null;
                await memRenderTarget.Scene.ManipulateSceneAsync((manipulator) =>
                {
                    NamedOrGenericKey geoResource = manipulator.AddResource<GeometryResource>(
                        () => new GeometryResource(ACFileLoader.ImportObjectType(Properties.Resources.ModelFlatShading)));

                    newObject = manipulator.AddGeneric(geoResource);
                });
                await memRenderTarget.Scene.WaitUntilVisibleAsync(newObject, memRenderTarget.RenderLoop);

                // Take screenshot
                GDI.Bitmap screenshot = await memRenderTarget.RenderLoop.GetScreenshotGdiAsync();

                //screenshot.DumpToDesktop(TEST_DUMMY_FILE_NAME);

                // Calculate and check difference
                bool isNearEqual = BitmapComparison.IsNearEqual(
                    screenshot, Properties.Resources.ReferenceImage_FlatShadedObject);
                Assert.True(isNearEqual, "Difference to reference image is to big!");
            }

            // Finishing checks
            Assert.True(GraphicsCore.Current.MainLoop.RegisteredRenderLoopCount == 0, "RenderLoops where not disposed correctly!");
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task LoadAndRender_ACShadedObject()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            using (MemoryRenderTarget memRenderTarget = new MemoryRenderTarget(1024, 1024))
            {
                memRenderTarget.ClearColor = Color4.CornflowerBlue;

                // Get and configure the camera
                PerspectiveCamera3D camera = memRenderTarget.Camera as PerspectiveCamera3D;
                camera.Position = new Vector3(-1.5f, 3f, -1.5f); 
                camera.Target = new Vector3(1f, 0f, 1f); 
                camera.UpdateCamera();

                // Define scene
                SceneObject newObject = null;
                await memRenderTarget.Scene.ManipulateSceneAsync((manipulator) =>
                {
                    NamedOrGenericKey geoResource = manipulator.AddResource<GeometryResource>(
                        () => new GeometryResource(ACFileLoader.ImportObjectType(Properties.Resources.ModelShaded)));

                    newObject = manipulator.AddGeneric(geoResource);
                });
                await memRenderTarget.Scene.WaitUntilVisibleAsync(newObject, memRenderTarget.RenderLoop);

                // Take screenshot
                GDI.Bitmap screenshot = await memRenderTarget.RenderLoop.GetScreenshotGdiAsync();

                //screenshot.DumpToDesktop(TEST_DUMMY_FILE_NAME);

                // Calculate and check difference
                bool isNearEqual = BitmapComparison.IsNearEqual(
                    screenshot, Properties.Resources.ReferenceImage_ShadedObject);
                Assert.True(isNearEqual, "Difference to reference image is to big!");
            }

            // Finishing checks
            Assert.True(GraphicsCore.Current.MainLoop.RegisteredRenderLoopCount == 0, "RenderLoops where not disposed correctly!");
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task LoadAndRender_ACTwoSidedObject()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            using (MemoryRenderTarget memRenderTarget = new MemoryRenderTarget(1024, 1024))
            {
                memRenderTarget.ClearColor = Color4.CornflowerBlue;

                // Get and configure the camera
                PerspectiveCamera3D camera = memRenderTarget.Camera as PerspectiveCamera3D;
                camera.Position = new Vector3(-1.5f, 1.5f, -1.5f);
                camera.Target = new Vector3(1f, 0f, 1f);
                camera.UpdateCamera();

                // Define scene
                SceneObject newObject = null;
                await memRenderTarget.Scene.ManipulateSceneAsync((manipulator) =>
                {
                    NamedOrGenericKey geoResource = manipulator.AddResource<GeometryResource>(
                        () => new GeometryResource(ACFileLoader.ImportObjectType(Properties.Resources.ModelTwoSided)));

                    newObject = manipulator.AddGeneric(geoResource);
                });
                await memRenderTarget.Scene.WaitUntilVisibleAsync(newObject, memRenderTarget.RenderLoop);

                // Take screenshot
                GDI.Bitmap screenshot = await memRenderTarget.RenderLoop.GetScreenshotGdiAsync();

                //screenshot.DumpToDesktop(TEST_DUMMY_FILE_NAME);

                // Calculate and check difference
                bool isNearEqual = BitmapComparison.IsNearEqual(
                    screenshot, Properties.Resources.ReferenceImage_TwoSidedObject);
                Assert.True(isNearEqual, "Difference to reference image is to big!");
            }

            // Finishing checks
            Assert.True(GraphicsCore.Current.MainLoop.RegisteredRenderLoopCount == 0, "RenderLoops where not disposed correctly!");
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task LoadAndRender_ACSingleSidedObject()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            using (MemoryRenderTarget memRenderTarget = new MemoryRenderTarget(1024, 1024))
            {
                memRenderTarget.ClearColor = Color4.CornflowerBlue;

                // Get and configure the camera
                PerspectiveCamera3D camera = memRenderTarget.Camera as PerspectiveCamera3D;
                camera.Position = new Vector3(-1.5f, 1.5f, -1.5f);
                camera.Target = new Vector3(1f, 0f, 1f);
                camera.UpdateCamera();

                // Define scene
                SceneObject newObject = null;
                await memRenderTarget.Scene.ManipulateSceneAsync((manipulator) =>
                {
                    NamedOrGenericKey geoResource = manipulator.AddResource<GeometryResource>(
                        () => new GeometryResource(ACFileLoader.ImportObjectType(Properties.Resources.ModelSingleSided)));

                    newObject = manipulator.AddGeneric(geoResource);
                });
                await memRenderTarget.Scene.WaitUntilVisibleAsync(newObject, memRenderTarget.RenderLoop);

                // Take screenshot
                GDI.Bitmap screenshot = await memRenderTarget.RenderLoop.GetScreenshotGdiAsync();

                //screenshot.DumpToDesktop(TEST_DUMMY_FILE_NAME);

                // Calculate and check difference
                bool isNearEqual = BitmapComparison.IsNearEqual(
                    screenshot, Properties.Resources.ReferenceImage_SingleSidedObject);
                Assert.True(isNearEqual, "Difference to reference image is to big!");
            }

            // Finishing checks
            Assert.True(GraphicsCore.Current.MainLoop.RegisteredRenderLoopCount == 0, "RenderLoops where not disposed correctly!");
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task LoadAndRender_ACTexturedObject()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            using (MemoryRenderTarget memRenderTarget = new MemoryRenderTarget(1024, 1024))
            {
                memRenderTarget.ClearColor = Color4.CornflowerBlue;

                // Get and configure the camera
                PerspectiveCamera3D camera = memRenderTarget.Camera as PerspectiveCamera3D;
                camera.Position = new Vector3(-1f, 1f, -1f);
                camera.Target = new Vector3(0f, 0f, 0f);
                camera.UpdateCamera();

                // Define scene
                SceneObject newObject = null;
                await memRenderTarget.Scene.ManipulateSceneAsync((manipulator) =>
                {
                    NamedOrGenericKey geoResource = manipulator.AddResource<GeometryResource>(
                        () => new GeometryResource(ACFileLoader.ImportObjectType(new AssemblyResourceLink(
                            Assembly.GetExecutingAssembly(),
                            "SeeingSharp.Tests.Rendering.Ressources.Models",
                            "ModelTextured.ac"))));

                    newObject = manipulator.AddGeneric(geoResource);
                });
                await memRenderTarget.Scene.WaitUntilVisibleAsync(newObject, memRenderTarget.RenderLoop);

                // Take screenshot
                GDI.Bitmap screenshot = await memRenderTarget.RenderLoop.GetScreenshotGdiAsync();

                //screenshot.DumpToDesktop(TEST_DUMMY_FILE_NAME);

                // Calculate and check difference
                bool isNearEqual = BitmapComparison.IsNearEqual(
                    screenshot, Properties.Resources.ReferenceImage_TexturedObject);
                Assert.True(isNearEqual, "Difference to reference image is to big!");
            }

            // Finishing checks
            Assert.True(GraphicsCore.Current.MainLoop.RegisteredRenderLoopCount == 0, "RenderLoops where not disposed correctly!");
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task ImportAndRender_ACShadedObject()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            IEnumerable<SceneObject> importedObjects = null;

            using (MemoryRenderTarget memRenderTarget = new MemoryRenderTarget(1024, 1024))
            {
                memRenderTarget.ClearColor = Color4.CornflowerBlue;

                // Get and configure the camera
                PerspectiveCamera3D camera = memRenderTarget.Camera as PerspectiveCamera3D;
                camera.Position = new Vector3(-1.5f, 3f, -1.5f);
                camera.Target = new Vector3(1f, 0f, 1f);
                camera.UpdateCamera();

                ResourceLink objSource = new AssemblyResourceLink(
                    typeof(ModelLoadingAndRenderingTests),
                    "Ressources.Models.ModelShaded.ac");
                importedObjects = await memRenderTarget.Scene.ImportAsync(objSource);

                // Wait until ac file is completely loaded
                await memRenderTarget.Scene.WaitUntilVisibleAsync(importedObjects, memRenderTarget.RenderLoop);

                // Take screenshot
                GDI.Bitmap screenshot = await memRenderTarget.RenderLoop.GetScreenshotGdiAsync();

                //screenshot.DumpToDesktop(TEST_DUMMY_FILE_NAME);

                // Calculate and check difference
                bool isNearEqual = BitmapComparison.IsNearEqual(
                    screenshot, Properties.Resources.ReferenceImage_ShadedObject);
                Assert.True(isNearEqual, "Difference to reference image is to big!");
            }

            // Finishing checks
            Assert.True(GraphicsCore.Current.MainLoop.RegisteredRenderLoopCount == 0, "RenderLoops where not disposed correctly!");
            Assert.NotNull(importedObjects);
            Assert.True(importedObjects.Count() == 1);
        }
    }
}
