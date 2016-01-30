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
using SeeingSharp.Util;
using SeeingSharp.Infrastructure;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.Samples.Base.BasicSamples.AsyncAnimationPalletSample),
    contractType: typeof(SeeingSharp.Samples.Base.SampleBase))]

namespace SeeingSharp.Samples.Base.BasicSamples
{
    [SampleInfo(
        Constants.SAMPLEGROUP_BASIC, "AsyncAnimation", 
        Constants.SAMPLE_BASICS_ASYNCANIM_ORDER,
        "https://github.com/RolandKoenig/SeeingSharp/blob/master/Samples/SeeingSharp.Samples.Base/_Samples/BasicSamples/AsyncAnimationSample.cs",
        SampleTargetPlatform.All)]
    public class AsyncAnimationPalletSample : SampleBase
    {
        private const int MAX_COUNT_BANANAS = 15;

        private RenderLoop m_renderLoop;

        /// <summary>
        /// Called when the sample has to startup.
        /// </summary>
        /// <param name="targetRenderLoop">The target render loop.</param>
        public override async Task OnStartupAsync(RenderLoop targetRenderLoop) 
        {
            targetRenderLoop.EnsureNotNull("targetRenderLoop");

            // Build dummy scene
            m_renderLoop = targetRenderLoop;
            Scene scene = m_renderLoop.Scene;
            Camera3DBase camera = m_renderLoop.Camera as Camera3DBase;

            // Build scene initially if we are on first load
            if (scene.CountObjects <= 0)
            {
                NamedOrGenericKey resGeometry = NamedOrGenericKey.Empty;

                await scene.ManipulateSceneAsync((manipulator) =>
                {
                    // Create floor
                    SampleSceneBuilder.BuildStandardFloor(
                        manipulator, Scene.DEFAULT_LAYER_NAME);

                    // Define banana object
                    ObjectType bananaType = ACFileLoader.ImportObjectType(
                        new AssemblyResourceUriBuilder(
                            "SeeingSharp.Samples.Base", false, 
                            "Assets/Models/Banana.ac"));
                    resGeometry = manipulator.AddGeometry(bananaType);
                });

                // Triggers async object creation
                TriggerAsyncObjectCreation(m_renderLoop, resGeometry);

                // Configure camera
                camera.Position = new Vector3(-10f, 8f, -10f);
                camera.Target = new Vector3(5f, 0f, 5f);
                camera.UpdateCamera();
            }
        }

        private async void TriggerAsyncObjectCreation(RenderLoop renderLoop, NamedOrGenericKey resGeometry)
        {
            while (!base.IsClosed) 
            {
                // Wait some time
                await Task.Delay(1000);
                if (base.IsClosed) { return; }

                if (!renderLoop.IsAttachedToVisibleView) { continue; }

                // Create a new object within the scene
                GenericObject newGenericObject = null;
                await renderLoop.Scene.ManipulateSceneAsync((manipulator) =>
                {
                    int bananaCount = manipulator.GetSceneObjects(Scene.DEFAULT_LAYER_NAME)
                        .Count((actObject) => actObject.Tag1 as string == "Banana");
                    if (bananaCount < MAX_COUNT_BANANAS)
                    {
                        newGenericObject = manipulator.AddGeneric(resGeometry);
                        newGenericObject.Tag1 = "Banana";
                    }
                });
                if (base.IsClosed) { return; }

                // Attach move behavior on the created object.
                if (newGenericObject != null)
                {
                    AttachMoveBehavior(newGenericObject);
                }
            }
        }

        /// <summary>
        /// Starts and controls the animation for the given object.
        /// </summary>
        /// <param name="targetObject">The object to be animated.</param>
        private async void AttachMoveBehavior(GenericObject targetObject)
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
