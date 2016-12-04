#region License information (SeeingSharp and all based games/applications)
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
#if DESKTOP
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Checking;
using SeeingSharp.Infrastructure;
using SeeingSharp.Util;
using SeeingSharp.Multimedia.DrawingVideo;
using SeeingSharp.Samples.Resources;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.Samples.Base.MFSamples.CameraCaptureSample),
    contractType: typeof(SeeingSharp.Samples.Base.SampleBase))]

namespace SeeingSharp.Samples.Base.MFSamples
{
    [SampleInfo(
        Constants.SAMPLEGROUP_MF, "Camera Capture",
        Constants.SAMPLE_MF_CAMERACAPTURE_ORDER,
        "https://github.com/RolandKoenig/SeeingSharp/blob/master/Samples/SeeingSharp.Samples.Base/_Samples/MFSamples/CameraCaptureSample.cs",
        SampleTargetPlatform.All)]
    public class CameraCaptureSample : SampleBase
    {
        private AsyncRealtimeVideoReader m_videoReader;
        private CaptureDeviceChooser m_deviceChooser;

        /// <summary>
        /// Called when the sample has to startup.
        /// </summary>
        /// <param name="targetRenderLoop">The target render loop.</param>
        public override async Task OnStartupAsync(RenderLoop targetRenderLoop)
        {
            targetRenderLoop.EnsureNotNull(nameof(targetRenderLoop));

            // Start the device chooser
            m_deviceChooser = new CaptureDeviceChooser();

            // Show fallback-scene if we don't have a capture device
            if(m_deviceChooser.DeviceCount <= 0)
            {
                await OnStartupAsync_Fallback(targetRenderLoop);
                return;
            }

            // Get and configure the camera
            Camera3DBase camera = targetRenderLoop.Camera as PerspectiveCamera3D;
            camera.Position = new Vector3(0f, 5f, -7f);
            camera.Target = new Vector3(0f, 0f, 0f);
            camera.UpdateCamera();

            // Open the video file
            m_videoReader = new AsyncRealtimeVideoReader(m_deviceChooser.DeviceInfos.First());
            m_videoReader.VideoReachedEnd += (sender, eArgs) =>
            {
                m_videoReader.SetCurrentPosition(TimeSpan.Zero);
            };

            // Define scene
            await targetRenderLoop.Scene.ManipulateSceneAsync((manipulator) =>
            {
                // Create floor
                SampleSceneBuilder.BuildStandardFloor(
                    manipulator, Scene.DEFAULT_LAYER_NAME);

                // Define texture and resource
                var resVideoTexture = manipulator.AddResource<VideoTextureResource>(
                    () => new VideoTextureResource(m_videoReader));
                var resVideoMaterial = manipulator.AddSimpleColoredMaterial(resVideoTexture, addToAlpha: 1f);
                var geoResource = manipulator.AddResource<GeometryResource>(
                    () => new GeometryResource(new PalletType(
                        palletMaterial: NamedOrGenericKey.Empty,
                        contentMaterial: resVideoMaterial)));

                // Add the object
                GenericObject newObject = manipulator.AddGeneric(geoResource);
                newObject.RotationEuler = new Vector3(0f, EngineMath.RAD_90DEG / 2f, 0f);
                newObject.Scaling = new Vector3(2f, 2f, 2f);
                newObject.EnableShaderGeneratedBorder();

                // Start pallet rotating animation
                newObject.BuildAnimationSequence()
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_180DEG, 0f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_360DEG, 0f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .RotateEulerAnglesTo(new Vector3(0f, 0f, 0f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .CallAction(() => newObject.RotationEuler = Vector3.Zero)
                    .ApplyAndRewind();
            });
        }

        private async Task OnStartupAsync_Fallback(RenderLoop targetRenderLoop)
        {
            targetRenderLoop.EnsureNotNull(nameof(targetRenderLoop));

            // Build dummy scene
            Scene scene = targetRenderLoop.Scene;
            Camera3DBase camera = targetRenderLoop.Camera as Camera3DBase;

            await targetRenderLoop.Scene.ManipulateSceneAsync((manipulator) =>
            {
                // Create floor
                SampleSceneBuilder.BuildStandardFloor(
                    manipulator, Scene.DEFAULT_LAYER_NAME);

                // Define texture and material resource
                var resTexture = manipulator.AddTexture(
                    new AssemblyResourceLink(
                        typeof(SeeingSharpSampleResources),
                        "Textures.NoCaptureDevice.png"));
                var resMaterial = manipulator.AddSimpleColoredMaterial(resTexture);

                // Create pallet geometry resource
                PalletType pType = new PalletType();
                pType.ContentMaterial = resMaterial;
                var resPalletGeometry = manipulator.AddResource<GeometryResource>(
                    () => new GeometryResource(pType));

                // Create pallet object
                GenericObject palletObject = manipulator.AddGeneric(resPalletGeometry);
                palletObject.Color = Color4.GreenColor;
                palletObject.EnableShaderGeneratedBorder();
                palletObject.BuildAnimationSequence()
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_180DEG, 0f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_360DEG, 0f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .RotateEulerAnglesTo(new Vector3(0f, 0f, 0f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .CallAction(() => palletObject.RotationEuler = Vector3.Zero)
                    .ApplyAndRewind();
            });

            // Configure camera
            camera.Position = new Vector3(2f, 2f, 2f);
            camera.Target = new Vector3(0f, 0.5f, 0f);
            camera.UpdateCamera();
        }

        /// <summary>
        /// Called when the sample is closed.
        /// Scene objects and resources are automatically removed, no need to do it
        /// manually in this method's implementation.
        /// </summary>
        public override void OnClosed()
        {
            base.OnClosed();

            CommonTools.SafeDispose(ref m_videoReader);
            CommonTools.SafeDispose(ref m_deviceChooser);
        }
    }
}
#endif
