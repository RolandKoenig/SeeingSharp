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
        public static async void BuildSingleModelDemo(this RenderLoop renderLoop)
        {
            // Build dummy scene
            Scene scene = renderLoop.Scene;
            Camera3DBase camera = renderLoop.Camera as Camera3DBase;

            // Build scene initially if we are on first load
            if (scene.CountObjects <= 0)
            {
                await scene.ManipulateSceneAsync((manipulator) =>
                {
                    // Create floor
                    manipulator.BuildStandardConveyorFloor(Scene.DEFAULT_LAYER_NAME);

                    // Load model resource
#if DESKTOP
                    ObjectType modelType = ACFileLoader.ImportObjectType(
                        new Uri("/FrozenSky.Samples.Base;component/Assets/Models/Penguin.ac", UriKind.Relative));
                    var resModel = manipulator.AddResource(() => new GeometryResource(modelType));
#else
                    ObjectType modelType = ACFileLoader.ImportObjectType(new Uri("ms-appx:///FrozenSky.Samples.Base/Assets/Models/Penguin.ac"));
                    var resModel = manipulator.AddResource(() => new GeometryResource(modelType));
#endif
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
