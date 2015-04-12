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
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Samples.Base
{
    public static partial class SampleSceneBuilder
    {
        public static async void BuildAsyncAnimationDemo(this RenderLoop renderLoop)
        {
            // Build dummy scene
            Scene scene = renderLoop.Scene;
            Camera3DBase camera = renderLoop.Camera as Camera3DBase;

            // Build scene initially if we are on first load
            if (scene.CountObjects <= 0)
            {
                NamedOrGenericKey resGeometry = NamedOrGenericKey.Empty;

                await scene.ManipulateSceneAsync((manipulator) =>
                {
                    // Create floor
                    manipulator.BuildStandardConveyorFloor(Scene.DEFAULT_LAYER_NAME);

#if DESKTOP || WINDOWS_PHONE
                    ObjectType bananaType = ACFileLoader.ImportObjectType(
                        new Uri("/FrozenSky.Samples.Base;component/Assets/Models/Banana.ac", UriKind.Relative));
#else
                    ObjectType bananaType = ACFileLoader.ImportObjectType(new Uri("ms-appx:///FrozenSky.Samples.Base/Assets/Models/Banana.ac"));
#endif

                    resGeometry = manipulator.AddGeometry(bananaType);
                });

                // Triggers async object creation
                TriggerAsyncObjectCreation(renderLoop, resGeometry);

                // Configure camera
                camera.Position = new Vector3(-10f, 8f, -10f);
                camera.Target = new Vector3(5f, 0f, 5f);
                camera.UpdateCamera();
            }
        }

        private static async void TriggerAsyncObjectCreation(RenderLoop renderLoop, NamedOrGenericKey resGeometry)
        {
            while(true)
            {
                // Wait some time
                await Task.Delay(1000);

                if (!renderLoop.IsAttachedToVisibleView) { continue; }

                // Create a new object within the scene
                GenericObject newGenericObject = null;
                await renderLoop.Scene.ManipulateSceneAsync((manipulator) =>
                {
                    newGenericObject = manipulator.AddGeneric(resGeometry);
                });
                
                // Attach move behavior on the created object.
                AttachMoveBehavior(newGenericObject);
            }
        }

        /// <summary>
        /// Starts and controls the animation for the given object.
        /// </summary>
        /// <param name="targetObject">The object to be animated.</param>
        private static async void AttachMoveBehavior(GenericObject targetObject)
        {
            Random randomizer = new Random(Environment.TickCount);

            // Set initial values
            targetObject.Position = new Vector3(0f, 2f, -2f);
            targetObject.Scaling = new Vector3(5f, 5f, 5f);
            targetObject.TransformationType = SpacialTransformationType.ScalingTranslationEulerAngles;
            targetObject.RotationEuler = new Vector3(0f, EngineMath.RAD_270DEG, 0f);
            targetObject.Opacity = 0f;

            // Create initial animation 
            await targetObject.BuildAnimationSequence()
                .Move3DBy(new Vector3(0f, -1.5f, 0f), TimeSpan.FromSeconds(0.5))
                .Scale3DTo(new Vector3(1f, 1f, 1f), TimeSpan.FromSeconds(0.5))
                .ChangeFloatBy(
                    () => targetObject.Opacity,
                    (actValue) => targetObject.Opacity = actValue,
                    1f,
                    TimeSpan.FromSeconds(0.5))
                .ApplyAsync();

            // Ensure correct opacity
            targetObject.Opacity = 1f;

            // Accelerate banana
            await targetObject.BuildAnimationSequence()
                .Move3DBy(
                    new Vector3(0f, 0f, 1f),
                    new MovementSpeed(2.0f, 1.5f))
                .ApplyAsync();

            // Enter looping animation (move around the rectangle)
            do
            {
                await targetObject.BuildAnimationSequence()
                    //Move on right line
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_270DEG, 0f), TimeSpan.FromSeconds(0.3))
                    .Move3DBy(new Vector3(0f, 0f, 5f), TimeSpan.FromSeconds(3.0))
                    .WaitFinished()
                    //Move on front line
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_180DEG, 0f), TimeSpan.FromSeconds(0.3))
                    .Move3DBy(new Vector3(-5f, 0f, 0f), TimeSpan.FromSeconds(3.0))
                    .Scale3DTo(new Vector3(2f, 2f, 2f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    //Move on Left line
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_90DEG, 0f), TimeSpan.FromSeconds(0.3))
                    .Move3DBy(new Vector3(0f, 0f, -5f), TimeSpan.FromSeconds(3.0))
                    .Scale3DTo(new Vector3(1f, 1f, 1f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    //Move on bottom line
                    .RotateEulerAnglesTo(new Vector3(0f, 0f, 0f), TimeSpan.FromSeconds(0.3))
                    .Move3DBy(new Vector3(5f, 0f, 0f), TimeSpan.FromSeconds(3.0))
                    .ApplyAsync();
            }
            while (randomizer.Next(0, 100) > 70);

            // Show remove animation
            await targetObject.BuildAnimationSequence()
                .Move3DBy(new Vector3(1.5f, 0f, 0f), TimeSpan.FromSeconds(0.5))
                .Scale3DTo(new Vector3(5f, 5f, 5f), TimeSpan.FromSeconds(0.5))
                .ChangeFloatBy(
                    () => targetObject.Opacity,
                    (actValue) => targetObject.Opacity = actValue,
                    -1f,
                    TimeSpan.FromSeconds(0.5))
                .ApplyAsync();

            // Remove the banana from the scene finally
            await targetObject.Scene.ManipulateSceneAsync((manipulator) =>
            {
                manipulator.Remove(targetObject);
            });
        }
    }
}
