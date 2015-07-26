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

using SeeingSharp.Infrastructure;
using SeeingSharp.Checking;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Util;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.Samples.Base.BasicSamples.SkyboxRenderingSample),
    contractType: typeof(SeeingSharp.Samples.Base.SampleBase))]

namespace SeeingSharp.Samples.Base.BasicSamples
{
    [SampleInfo(
        Constants.SAMPLEGROUP_BASIC, "SkyboxRendering",
        Constants.SAMPLE_BASICS_SKYBOX_ORDER,
        "https://github.com/RolandKoenig/SeeingSharp/blob/master/Samples/SeeingSharp.Samples.Base/_Samples/BasicSamples/SkyboxRenderingSample.cs")]
    public class SkyboxRenderingSample : SampleBase
    {
        /// <summary>
        /// Called when the sample has to startup.
        /// </summary>
        /// <param name="targetRenderLoop">The target render loop.</param>
        /// <returns></returns>
        public override async Task OnStartupAsync(RenderLoop targetRenderLoop)
        {
            targetRenderLoop.EnsureNotNull("targetRenderLoop");

            // Build dummy scene
            Scene scene = targetRenderLoop.Scene;
            Camera3DBase camera = targetRenderLoop.Camera as Camera3DBase;

            // Build scene initially if we are on first load
            if (scene.CountObjects <= 0)
            {
                await scene.ManipulateSceneAsync((manipulator) =>
                {
                    // Create floor
                    SampleSceneBuilder.BuildStandardConveyorFloor(
                        manipulator, Scene.DEFAULT_LAYER_NAME);

                    // Create pallet geometry resource
                    PalletType pType = new PalletType();
                    pType.ContentColor = Color4.Transparent;
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
                        .CallAction(() => palletObject.RotationEuler = Vector3.Zero)
                        .ApplyAndRewind();

                    var resSkyboxTexture = manipulator.AddTexture(
                        new AssemblyResourceUriBuilder(
                            "SeeingSharp.Samples.Base", false,
                            "Assets/Textures/Skybox.dds"));

                    // Create the skybox on a new layer
                    manipulator.AddLayer("Skybox");
                    SkyboxObject skyboxObject = new SkyboxObject(resSkyboxTexture);
                    manipulator.Add(skyboxObject, "Skybox");
                });

                // Configure camera
                camera.Position = new Vector3(5f, 3f, 5f);
                camera.Target = new Vector3(0f, 2f, 0f);
                camera.UpdateCamera();
            }
        }
    }
}
