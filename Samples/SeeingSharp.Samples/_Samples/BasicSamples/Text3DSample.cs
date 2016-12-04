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
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Checking;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Infrastructure;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.Samples.Base.BasicSamples.Text3DSample),
    contractType: typeof(SeeingSharp.Samples.Base.SampleBase))]

namespace SeeingSharp.Samples.Base.BasicSamples
{
    [SampleInfo(
        Constants.SAMPLEGROUP_BASIC, "Text3D",
        Constants.SAMPLE_BASICS_TEXT_3D_ORDER,
        "https://github.com/RolandKoenig/SeeingSharp/blob/master/Samples/SeeingSharp.Samples/_Samples/BasicSamples/Text3DSample.cs",
        SampleTargetPlatform.All)]
    public class Text3DSample : SampleBase
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

            await targetRenderLoop.Scene.ManipulateSceneAsync((manipulator) =>
            {
                // Create floor
                SampleSceneBuilder.BuildStandardFloor(
                    manipulator, Scene.DEFAULT_LAYER_NAME);

                TextGeometryOptions textOptions = TextGeometryOptions.Default;
                textOptions.FontSize = 50;
                textOptions.MakeVolumetricText = true;
                textOptions.SurfaceVertexColor = Color.Blue;
                textOptions.VolumetricSideSurfaceVertexColor = Color4.CornflowerBlue;

                GenericObject textObject = manipulator.Add3DText($"Seeing# {Environment.NewLine} Text3D Sample", textOptions);
                textObject.YPos = textOptions.VolumetricTextDepth;
            });

            // Configure camera
            camera.Position = new Vector3(0.7f, 8.5f, -15f);
            camera.RelativeTarget = new Vector3(0.44f, -0.62f, 0.64f);
            camera.UpdateCamera();
        }
    }
}
