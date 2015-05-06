#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
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

using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.DrawingVideo;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Multimedia.Views;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

// Namespace mappings
using GDI = System.Drawing;

namespace SeeingSharp.Tests.Rendering
{
    [Collection("Rendering_SeeingSharp")]
    public class DrawingVideoTests
    {
        public const string TEST_CATEGORY = "SeeingSharp Multimedia Drawing Video";

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task ReadSimple_WmvVideo_Seek()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            ResourceLink videoLink = new AssemblyResourceLink(
                this.GetType().Assembly,
                "SeeingSharp.Tests.Rendering.Ressources.Videos",
                "DummyVideo.wmv");
            GDI.Bitmap bitmapFrame = null;
            using (FrameByFrameVideoReader videoReader = new FrameByFrameVideoReader(videoLink))
            using (MemoryMappedTexture32bpp actFrameBuffer = new MemoryMappedTexture32bpp(videoReader.FrameSize))
            {
                videoReader.SetCurrentPosition(TimeSpan.FromSeconds(2.0));
                videoReader.ReadFrame(actFrameBuffer);

                actFrameBuffer.SetAllAlphaValuesToOne();

                using (bitmapFrame = GraphicsHelper.LoadBitmapFromMappedTexture(actFrameBuffer))
                {
                    Assert.NotNull(bitmapFrame);
                    Assert.True(videoReader.CurrentPosition > TimeSpan.FromSeconds(1.9));
                    Assert.True(videoReader.CurrentPosition < TimeSpan.FromSeconds(2.1));
                    Assert.True(videoReader.Duration > TimeSpan.FromSeconds(3.9));
                    Assert.True(videoReader.Duration < TimeSpan.FromSeconds(4.1));
                    Assert.True(videoReader.IsSeekable);
                    Assert.True(
                        BitmapComparison.IsNearEqual(bitmapFrame, Properties.Resources.ReferenceImage_VideoFrameWmv_Seek));
                }
            }
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task ReadSimple_WmvVideo()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            ResourceLink videoLink = new AssemblyResourceLink(
                this.GetType().Assembly,
                "SeeingSharp.Tests.Rendering.Ressources.Videos",
                "DummyVideo.wmv");
            GDI.Bitmap bitmapFrame10 = null;
            using (FrameByFrameVideoReader videoReader = new FrameByFrameVideoReader(videoLink))
            using (MemoryMappedTexture32bpp actFrameBuffer = new MemoryMappedTexture32bpp(videoReader.FrameSize))
            {
                int frameIndex = 0;
                while(!videoReader.EndReached)
                {
                    if (videoReader.ReadFrame(actFrameBuffer))
                    {
                        actFrameBuffer.SetAllAlphaValuesToOne();

                        frameIndex++;
                        if (frameIndex != 10) { continue; }

                        bitmapFrame10 = GraphicsHelper.LoadBitmapFromMappedTexture(actFrameBuffer);
                        break;
                    }
                }

                Assert.NotNull(bitmapFrame10);
                Assert.True(videoReader.Duration > TimeSpan.FromSeconds(3.9));
                Assert.True(videoReader.Duration < TimeSpan.FromSeconds(4.1));
                Assert.True(videoReader.IsSeekable);
                Assert.True(
                    BitmapComparison.IsNearEqual(bitmapFrame10, Properties.Resources.ReferenceImage_VideoFrameWmv));
            }
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task ReadSimple_Mp4Video()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            ResourceLink videoLink = new AssemblyResourceLink(
                this.GetType().Assembly,
                "SeeingSharp.Tests.Rendering.Ressources.Videos",
                "DummyVideo.mp4");
            GDI.Bitmap bitmapFrame10 = null;
            using (FrameByFrameVideoReader videoReader = new FrameByFrameVideoReader(videoLink))
            using (MemoryMappedTexture32bpp actFrameBuffer = new MemoryMappedTexture32bpp(videoReader.FrameSize))
            {
                int frameIndex = 0;
                while (!videoReader.EndReached)
                {
                    if (videoReader.ReadFrame(actFrameBuffer))
                    {
                        actFrameBuffer.SetAllAlphaValuesToOne();

                        frameIndex++;
                        if (frameIndex != 10) { continue; }

                        bitmapFrame10 = GraphicsHelper.LoadBitmapFromMappedTexture(actFrameBuffer);
                        break;
                    }
                }

                Assert.NotNull(bitmapFrame10);
                Assert.True(videoReader.Duration > TimeSpan.FromSeconds(3.9));
                Assert.True(videoReader.Duration < TimeSpan.FromSeconds(4.1));
                Assert.True(videoReader.IsSeekable);
                Assert.True(
                    BitmapComparison.IsNearEqual(bitmapFrame10, Properties.Resources.ReferenceImage_VideoFrameMp4));
            }
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task RenderSimple_Mp4Video()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            // Configure video rendering
            string videoFilePath = Path.Combine(Environment.CurrentDirectory, "Video.mp4");
            try
            {
                Mp4VideoWriter videoWriter = new Mp4VideoWriter(videoFilePath);
                videoWriter.Bitrate = 1500;

                // Perform video rendering
                await RenderSimple_Generic(videoWriter, 100);

                // Check results
                Assert.True(File.Exists(videoFilePath));
            }
            finally
            {
                if (File.Exists(videoFilePath))
                {
                    File.Delete(videoFilePath);
                }
            }
        }

        /// <summary>
        /// Base method used as common test scenario for all video renderers.
        /// </summary>
        /// <param name="videoWriter">The VideoWriter to be testet.</param>
        /// <param name="countFrames">Total count of frames to be rendered.</param>
        /// <param name="doAnimate">Execute animation during recording.</param>
        private async Task RenderSimple_Generic(SeeingSharpVideoWriter videoWriter, int countFrames, bool doAnimate = true)
        {
            using (MemoryRenderTarget memRenderTarget = new MemoryRenderTarget(1024, 1024))
            {
                memRenderTarget.ClearColor = Color4.CornflowerBlue;

                // Get and configure the camera
                PerspectiveCamera3D camera = memRenderTarget.Camera as PerspectiveCamera3D;
                camera.Position = new Vector3(0f, 5f, -7f);
                camera.Target = new Vector3(0f, 0f, 0f);
                camera.UpdateCamera();

                // Define scene
                await memRenderTarget.Scene.ManipulateSceneAsync((manipulator) =>
                {
                    NamedOrGenericKey geoResource = manipulator.AddResource<GeometryResource>(
                        () => new GeometryResource(new PalletType()));

                    var newObject = manipulator.AddGeneric(geoResource);
                    newObject.RotationEuler = new Vector3(0f, EngineMath.RAD_90DEG / 2f, 0f);
                    newObject.Scaling = new Vector3(2f, 2f, 2f);

                    if (doAnimate)
                    {
                        newObject.BuildAnimationSequence()
                            .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_180DEG, 0f), TimeSpan.FromMilliseconds(500.0))
                            .WaitFinished()
                            .RotateEulerAnglesTo(new Vector3(0f, 0, 0f), TimeSpan.FromMilliseconds(500.0))
                            .WaitFinished()
                            .ApplyAndRewind();
                    }
                });

                await memRenderTarget.RenderLoop.WaitForNextFinishedRenderAsync();

                // Start video rendering
                await memRenderTarget.RenderLoop.RegisterVideoWriterAsync(videoWriter);

                // Write about 100 frames to the video
                for (int loop = 0; loop < countFrames; loop++)
                {
                    await memRenderTarget.RenderLoop.WaitForNextFinishedRenderAsync();
                }

                // finish video rendering
                await memRenderTarget.RenderLoop.FinishVideoWriterAsync(videoWriter);
            }

            // Make shure that all the renderloop is correctly disposed
            Assert.True(GraphicsCore.Current.MainLoop.RegisteredRenderLoopCount == 0, "RenderLoops where not disposed correctly!");
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task Render3D_VideoTexture()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            ResourceLink videoLink = new AssemblyResourceLink(
                this.GetType().Assembly,
                "SeeingSharp.Tests.Rendering.Ressources.Videos",
                "DummyVideo.mp4");
            using (MemoryRenderTarget memRenderTarget = new MemoryRenderTarget(1024, 1024))
            {
                memRenderTarget.ClearColor = Color4.CornflowerBlue;

                // Get and configure the camera
                PerspectiveCamera3D camera = memRenderTarget.Camera as PerspectiveCamera3D;
                camera.Position = new Vector3(0f, 5f, -7f);
                camera.Target = new Vector3(0f, 0f, 0f);
                camera.UpdateCamera();

                // Define scene
                await memRenderTarget.Scene.ManipulateSceneAsync((manipulator) =>
                {
                    var resVideoTexture = manipulator.AddResource<VideoThumbnailTextureResource>(
                        () => new VideoThumbnailTextureResource(videoLink, TimeSpan.FromMilliseconds(300.0)));
                    var resVideoMaterial = manipulator.AddSimpleColoredMaterial(resVideoTexture, addToAlpha: 1f);
                    var geoResource = manipulator.AddResource<GeometryResource>(
                        () => new GeometryResource(new PalletType(
                            palletMaterial: NamedOrGenericKey.Empty,
                            contentMaterial: resVideoMaterial)));

                    GenericObject newObject = manipulator.AddGeneric(geoResource);
                    newObject.RotationEuler = new Vector3(0f, EngineMath.RAD_90DEG / 2f, 0f);
                    newObject.Scaling = new Vector3(2f, 2f, 2f);
                });

                // Take screenshot
                GDI.Bitmap screenshot = await memRenderTarget.RenderLoop.GetScreenshotGdiAsync();
                screenshot = await memRenderTarget.RenderLoop.GetScreenshotGdiAsync();

                //screenshot.DumpToDesktop("Blub.png");

                // Calculate and check difference
                bool isNearEqual = BitmapComparison.IsNearEqual(
                    screenshot, Properties.Resources.ReferenceImage_SimpleObject_VideoTexture);
                Assert.True(isNearEqual, "Difference to reference image is to big!");
            }

            // Finishing checks
            Assert.True(GraphicsCore.Current.MainLoop.RegisteredRenderLoopCount == 0, "RenderLoops where not disposed correctly!");
        }
    }
}
