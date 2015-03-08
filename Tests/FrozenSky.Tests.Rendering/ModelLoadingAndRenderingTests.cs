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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Views;
using FrozenSky.Multimedia.Drawing3D;
using FrozenSky.Util;
using FrozenSky.Multimedia.Objects;
using FrozenSky.Infrastructure;
using FrozenSky;

// Some namespace mappings
using GDI = System.Drawing;

namespace FrozenSky.Tests.Rendering
{
    [TestClass]
    public class ModelLoadingAndRenderingTests
    {
        public const int MANIPULATE_WAIT_TIME = 500;
        public const string TEST_DUMMY_FILE_NAME = "UnitTest.Screenshot.png";
        public const string TEST_CATEGORY = "FrozenSky Multimedia Model Loading and Rendering";

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
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
                    screenshot, Properties.Resources.ReferenceImageFlatShadedObject);
                Assert.IsTrue(isNearEqual, "Difference to reference image is to big!");
            }

            // Finishing checks
            Assert.IsTrue(GraphicsCore.Current.MainLoop.RegisteredRenderLoopCount == 0, "RenderLoops where not disposed correctly!");
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
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
                    screenshot, Properties.Resources.ReferenceImageShadedObject);
                Assert.IsTrue(isNearEqual, "Difference to reference image is to big!");
            }

            // Finishing checks
            Assert.IsTrue(GraphicsCore.Current.MainLoop.RegisteredRenderLoopCount == 0, "RenderLoops where not disposed correctly!");
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
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
                    screenshot, Properties.Resources.ReferenceImageTwoSidedObject);
                Assert.IsTrue(isNearEqual, "Difference to reference image is to big!");
            }

            // Finishing checks
            Assert.IsTrue(GraphicsCore.Current.MainLoop.RegisteredRenderLoopCount == 0, "RenderLoops where not disposed correctly!");
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
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
                    screenshot, Properties.Resources.ReferenceImageSingleSidedObject);
                Assert.IsTrue(isNearEqual, "Difference to reference image is to big!");
            }

            // Finishing checks
            Assert.IsTrue(GraphicsCore.Current.MainLoop.RegisteredRenderLoopCount == 0, "RenderLoops where not disposed correctly!");
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
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
                            "FrozenSky.Tests.Rendering.Ressources.Models",
                            "ModelTextured.ac"))));

                    newObject = manipulator.AddGeneric(geoResource);
                });
                await memRenderTarget.Scene.WaitUntilVisibleAsync(newObject, memRenderTarget.RenderLoop);

                // Take screenshot
                GDI.Bitmap screenshot = await memRenderTarget.RenderLoop.GetScreenshotGdiAsync();

                //screenshot.DumpToDesktop(TEST_DUMMY_FILE_NAME);

                // Calculate and check difference
                bool isNearEqual = BitmapComparison.IsNearEqual(
                    screenshot, Properties.Resources.ReferenceImageTexturedObject);
                Assert.IsTrue(isNearEqual, "Difference to reference image is to big!");
            }

            // Finishing checks
            Assert.IsTrue(GraphicsCore.Current.MainLoop.RegisteredRenderLoopCount == 0, "RenderLoops where not disposed correctly!");
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
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

                ResourceSource objSource = new AssemblyResourceLink(
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
                    screenshot, Properties.Resources.ReferenceImageShadedObject);
                Assert.IsTrue(isNearEqual, "Difference to reference image is to big!");
            }

            // Finishing checks
            Assert.IsTrue(GraphicsCore.Current.MainLoop.RegisteredRenderLoopCount == 0, "RenderLoops where not disposed correctly!");
            Assert.IsNotNull(importedObjects);
            Assert.IsTrue(importedObjects.Count() == 1);
        }
    }
}
