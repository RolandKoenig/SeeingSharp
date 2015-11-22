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

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.Samples.Base.MFSamples.VideoTextureSample),
    contractType: typeof(SeeingSharp.Samples.Base.SampleBase))]

namespace SeeingSharp.Samples.Base.MFSamples
{
    [SampleInfo(
        Constants.SAMPLEGROUP_MF, "VideoTexture",
        Constants.SAMPLE_MF_VIDEOTEXTURE_ORDER,
        "https://github.com/RolandKoenig/SeeingSharp/blob/master/Samples/SeeingSharp.Samples.Base/_Samples/MFSamples/VideoTextureSample.cs",
        SampleTargetPlatform.All)]
    public class VideoTextureSample : SampleBase
    {
        private AsyncRealtimeVideoReader m_videoReader;

        /// <summary>
        /// Called when the sample has to startup.
        /// </summary>
        /// <param name="targetRenderLoop">The target render loop.</param>
        public override async Task OnStartupAsync(RenderLoop targetRenderLoop)
        {
            targetRenderLoop.EnsureNotNull("targetRenderLoop");

            // Get and configure the camera
            Camera3DBase camera = targetRenderLoop.Camera as PerspectiveCamera3D;
            camera.Position = new Vector3(0f, 5f, -7f);
            camera.Target = new Vector3(0f, 0f, 0f);
            camera.UpdateCamera();

            // Open the video file
            //  see original vide at: https://www.flickr.com/photos/mlinksva/20939860191/in/photolist-xUofUg-xzKr45-xzKn65-xzhfV1-xPfMqh-wTxmUk-wSd5bL-xwAUF3-wSbSW3-xNqVYR-xNqUyB-xKte5J-xKtbMh-wQKVu5-wQVbiV-wQLHns-xKsHiQ-xvapef-xKsF1y-xMMyY2-xvbbXC-xvb5Xy-xNqkBK-xvb25o-xvafSW-xvb4FW-xKsavw-xMM69V-wQUtWn-xvgDSv-xLW62q-xvbig9-wQL8p7-xMMFog-wQV8CK-wQLGcS-wQLLZY-wQKN9U-xvhorD-xNq5we-xvbh7f-wQU3KF-xLWiZE-xvgVqe-wQKKEq-xvaJN3-xNqjhF-wQUEhH-xKspPw-wQRXGt
            //  Licensed under public domain
            ResourceLink videoLink = new AssemblyResourceUriBuilder(
                "SeeingSharp.Samples.Base", false,
                "Assets/Videos/DummyVideoLandscape.mp4");
            m_videoReader = new AsyncRealtimeVideoReader(videoLink);
            m_videoReader.VideoReachedEnd += (sender, eArgs) =>
            {
                m_videoReader.SetCurrentPosition(TimeSpan.Zero);
            };

            // Define scene
            await targetRenderLoop.Scene.ManipulateSceneAsync((manipulator) =>
            {
                // Create floor
                SampleSceneBuilder.BuildStandardConveyorFloor(
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
            });
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
        }
    }
}
