#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2015 Roland König (RolandK)

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

using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrozenSky.Multimedia.Drawing3D;
using FrozenSky.Multimedia.Drawing2D;
using FrozenSky.Multimedia.Views;
using FrozenSky.Multimedia.Objects;
using FrozenSky.Multimedia.Core;
using FrozenSky.Util;

using GDI = System.Drawing;

namespace FrozenSky.Tests.Rendering
{
    [Collection("Rendering_FrozenSky")]
    public class Drawing2DTests
    {
        public const string TEST_CATEGORY = "FrozenSky Multimedia Drawing 2D";

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task Render_SimpleText_SimpleSingleColor()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            using (SolidBrushResource solidBrush = new SolidBrushResource(Color4.RedColor))
            using (TextFormatResource textFormat = new TextFormatResource("Arial", 70))
            using (MemoryRenderTarget memRenderTarget = new MemoryRenderTarget(1024, 1024))
            {
                // Perform rendering
                memRenderTarget.ClearColor = Color4.CornflowerBlue;
                await memRenderTarget.RenderLoop.Register2DDrawingLayerAsync((graphics) =>
                {
                    // 2D rendering is made here
                    graphics.DrawText(
                        string.Format("Just a dummy text ;){0}Just a dummy text ;)", Environment.NewLine),
                        textFormat,
                        new RectangleF(10, 10, 512, 512),
                        solidBrush);
                });
                await memRenderTarget.AwaitRenderAsync();

                // Take screenshot
                GDI.Bitmap screenshot = await memRenderTarget.RenderLoop.GetScreenshotGdiAsync();
                //screenshot.DumpToDesktop("Blub.png");

                // Calculate and check difference
                float diff = BitmapComparison.CalculatePercentageDifference(
                    screenshot, Properties.Resources.ReferenceImage_SimpleText_SingleColor);
                Assert.True(diff < 0.2, "Difference to reference image is to big!");
            }
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task Render_SimpleRoundedRect_Filled()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            using (SolidBrushResource solidBrush = new SolidBrushResource(Color4.Gray))
            using (MemoryRenderTarget memRenderTarget = new MemoryRenderTarget(1024, 1024))
            {
                // Perform rendering
                memRenderTarget.ClearColor = Color4.CornflowerBlue;
                await memRenderTarget.RenderLoop.Register2DDrawingLayerAsync((graphics) =>
                {
                    // 2D rendering is made here
                    graphics.FillRoundedRectangle(
                        new RectangleF(10, 10, 512, 512), 30, 30,
                        solidBrush);
                });
                await memRenderTarget.AwaitRenderAsync();

                // Take screenshot
                GDI.Bitmap screenshot = await memRenderTarget.RenderLoop.GetScreenshotGdiAsync();
                //screenshot.DumpToDesktop("Blub.png");

                // Calculate and check difference
                float diff = BitmapComparison.CalculatePercentageDifference(
                    screenshot, Properties.Resources.ReferenceImage_SimpleRoundedRectFilled);
                Assert.True(diff < 0.2, "Difference to reference image is to big!");
            }
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task Render_SimpleRoundedRect_Filled_Over3D()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            using (SolidBrushResource solidBrush = new SolidBrushResource(Color4.Gray.ChangeAlphaTo(0.5f)))
            using (MemoryRenderTarget memRenderTarget = new MemoryRenderTarget(1024, 1024))
            {
                memRenderTarget.ClearColor = Color4.CornflowerBlue;

                // Get and configure the camera
                PerspectiveCamera3D camera = memRenderTarget.Camera as PerspectiveCamera3D;
                camera.Position = new Vector3(0f, 5f, -5f);
                camera.Target = new Vector3(0f, 1f, 0f);
                camera.UpdateCamera();

                // Define scene
                await memRenderTarget.Scene.ManipulateSceneAsync((manipulator) =>
                {
                    // Define object
                    NamedOrGenericKey geoResource = manipulator.AddResource<GeometryResource>(
                        () => new GeometryResource(new PalletStackType(
                            NamedOrGenericKey.Empty, 10)));
                    var newObject = manipulator.AddGeneric(geoResource);
                    newObject.RotationEuler = new Vector3(0f, EngineMath.RAD_90DEG / 2f, 0f);
                    newObject.Scaling = new Vector3(2f, 2f, 2f);
                    newObject.Color = Color4.Goldenrod;
                    newObject.EnableShaderGeneratedBorder();
                });

                // Define 2D overlay
                await memRenderTarget.RenderLoop.Register2DDrawingLayerAsync((graphics) =>
                {
                    // 2D rendering is made here
                    graphics.FillRoundedRectangle(
                        new RectangleF(10, 10, 512, 512), 30, 30,
                        solidBrush);
                });

                // Take screenshot
                GDI.Bitmap screenshot = await memRenderTarget.RenderLoop.GetScreenshotGdiAsync();
                //screenshot.DumpToDesktop("Blub.png");

                // Calculate and check difference
                float diff = BitmapComparison.CalculatePercentageDifference(
                    screenshot, Properties.Resources.ReferenceImage_RoundedRectOver3D);
                Assert.True(diff < 0.2, "Difference to reference image is to big!");
            }

            // Finishing checks
            Assert.True(GraphicsCore.Current.MainLoop.RegisteredRenderLoopCount == 0, "RenderLoops where not disposed correctly!");
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task Render_DebugLayer()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            using (DebugDrawingLayer debugLayer = new DebugDrawingLayer())
            using (MemoryRenderTarget memRenderTarget = new MemoryRenderTarget(1024, 1024))
            {
                // Perform rendering
                memRenderTarget.ClearColor = Color4.CornflowerBlue;
                await memRenderTarget.RenderLoop.Register2DDrawingLayerAsync(debugLayer);
                await memRenderTarget.AwaitRenderAsync();

                // Take screenshot
                GDI.Bitmap screenshot = await memRenderTarget.RenderLoop.GetScreenshotGdiAsync();
                //screenshot.DumpToDesktop("Blub.png");

                // Calculate and check difference
                float diff = BitmapComparison.CalculatePercentageDifference(
                    screenshot, Properties.Resources.ReferenceImage_DebugDawingLayer);
                Assert.True(diff < 0.2, "Difference to reference image is to big!");
            }
        }
    }
}
