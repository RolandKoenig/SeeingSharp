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
using SeeingSharp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Checking;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Objects;
using System.Numerics;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.Samples.Base.BasicSamples.ParentChildSample),
    contractType: typeof(SeeingSharp.Samples.Base.SampleBase))]

namespace SeeingSharp.Samples.Base.BasicSamples
{
    [SampleInfo(
        Constants.SAMPLEGROUP_BASIC, "Parent Child",
        Constants.SAMPLE_BASICS_PARENT_CHILD_ORDER,
        "https://github.com/RolandKoenig/SeeingSharp/blob/master/Samples/SeeingSharp.Samples/_Samples/BasicSamples/ParentChildSample.cs")]
    public class ParentChildSample : SampleBase
    {
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

                // Create pallet geometry resource
                PalletType pType = new PalletType();
                pType.ContentColor = Color4.Transparent;
                var resPalletGeometry = manipulator.AddResource<GeometryResource>(
                    () => new GeometryResource(pType));

                //********************************
                // Create parent object
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

                //********************************
                // Create first level childs
                GenericObject actChild = manipulator.AddGeneric(resPalletGeometry);
                actChild.Position = new Vector3(-2f, 0f, 0f);
                actChild.Scaling = new Vector3(0.5f, 0.5f, 0.5f);
                manipulator.AddChild(palletObject, actChild);

                actChild = manipulator.AddGeneric(resPalletGeometry);
                actChild.Position = new Vector3(0f, 0f, 2f);
                actChild.Scaling = new Vector3(0.5f, 0.5f, 0.5f);
                manipulator.AddChild(palletObject, actChild);

                //********************************
                // Create second level parent/child relationships
                GenericObject actSecondLevelParent = manipulator.AddGeneric(resPalletGeometry);
                actSecondLevelParent.Position = new Vector3(3f, 0f, 0f);
                actSecondLevelParent.Scaling = new Vector3(0.8f, 0.8f, 0.8f);
                actSecondLevelParent.Color = Color4.BlueColor;
                actSecondLevelParent.BuildAnimationSequence()
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_180DEG, 0f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_360DEG, 0f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .CallAction(() => actSecondLevelParent.RotationEuler = Vector3.Zero)
                    .ApplyAndRewind();
                manipulator.AddChild(palletObject, actSecondLevelParent);

                GenericObject actSecondLevelChild = manipulator.AddGeneric(resPalletGeometry);
                actSecondLevelChild.Position = new Vector3(1f, 0f, 0f);
                actSecondLevelChild.Scaling = new Vector3(0.4f, 0.4f, 0.4f);
                manipulator.AddChild(actSecondLevelParent, actSecondLevelChild);

                actSecondLevelChild = manipulator.AddGeneric(resPalletGeometry);
                actSecondLevelChild.Position = new Vector3(-1f, 0f, 0f);
                actSecondLevelChild.Scaling = new Vector3(0.4f, 0.4f, 0.4f);
                manipulator.AddChild(actSecondLevelParent, actSecondLevelChild);

                actSecondLevelChild = manipulator.AddGeneric(resPalletGeometry);
                actSecondLevelChild.Position = new Vector3(0f, 0f, 1f);
                actSecondLevelChild.Scaling = new Vector3(0.4f, 0.4f, 0.4f);
                manipulator.AddChild(actSecondLevelParent, actSecondLevelChild);
            });

            // Configure camera
            camera.Position = new Vector3(5f, 5f, 5f);
            camera.Target = new Vector3(0f, 0.5f, 0f);
            camera.UpdateCamera();
        }
    }
}
