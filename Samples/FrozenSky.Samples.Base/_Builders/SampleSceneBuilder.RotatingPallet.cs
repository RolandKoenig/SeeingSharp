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

using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
using FrozenSky.Multimedia.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Samples.Base
{
    public static partial class SampleSceneBuilder
    {
        public static async void BuildRotatingPalletDemo(this RenderLoop renderLoop)
        {
            // Build dummy scene
            Scene scene = renderLoop.Scene;
            Camera3DBase camera = renderLoop.Camera as Camera3DBase;

            // Build scene initially if we are on first load
            if (scene.CountObjects <= 0)
            {
                await renderLoop.Scene.ManipulateSceneAsync((manipulator) =>
                {
                    // Create floor
                    manipulator.BuildStandardConveyorFloor(Scene.DEFAULT_LAYER_NAME);

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
                        .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_180DEG, EngineMath.RAD_45DEG), TimeSpan.FromSeconds(2.0))
                        .WaitFinished()
                        .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_360DEG, EngineMath.RAD_45DEG), TimeSpan.FromSeconds(2.0))
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
        }
    }
}
