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
    targetType: typeof(SeeingSharp.Samples.Base.BasicSamples.SingleModelSample),
    contractType: typeof(SeeingSharp.Samples.Base.SampleBase))]

namespace SeeingSharp.Samples.Base.BasicSamples
{
    [SampleInfo(
        Constants.SAMPLEGROUP_BASIC, "SingleModel",
        Constants.SAMPLE_BASCIS_SINGLEMODEL_ORDER,
        "https://github.com/RolandKoenig/SeeingSharp/blob/master/Samples/SeeingSharp.Samples.Base/_Samples/BasicSamples/SingleModelSample.cs",
        SampleTargetPlatform.AllExcludingSmartphone)]
    public class SingleModelSample : SampleBase
    {
        /// <summary>
        /// Called when the sample has to startup.
        /// </summary>
        /// <param name="targetRenderLoop">The target render loop.</param>
        public override async Task OnStartupAsync(RenderLoop targetRenderLoop)
        {
            targetRenderLoop.EnsureNotNull(nameof(targetRenderLoop));

            // Build dummy scene
            Scene scene = targetRenderLoop.Scene;
            Camera3DBase camera = targetRenderLoop.Camera as Camera3DBase;

            // Build scene initially if we are on first load
            if (scene.CountObjects <= 0)
            {
                await scene.ManipulateSceneAsync((manipulator) =>
                {
                    // Create floor
                    SampleSceneBuilder.BuildStandardFloor(
                        manipulator, Scene.DEFAULT_LAYER_NAME);

                    // Load model resource
                    ObjectType modelType = ACFileLoader.ImportObjectType(
                        new AssemblyResourceUriBuilder(
                            "SeeingSharp.Samples.Base", false,
                            "Assets/Models/Penguin.ac"));
                    var resModel = manipulator.AddResource(() => new GeometryResource(modelType));
                    
                    // Create and add an instance to the scene
                    GenericObject modelObject = new GenericObject(resModel);
                    modelObject.Scaling = new Vector3(10f, 10f, 10f);
                    manipulator.Add(modelObject);
                });

                // Configure camera
                camera.Position = new Vector3(-10f, 20f, 20f);
                camera.Target = new Vector3(0f, 6f, 0f);
                camera.UpdateCamera();
            }
        }
    }
}
