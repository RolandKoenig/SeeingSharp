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

using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Checking;
using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing2D;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Util;
using SeeingSharp.Samples.Resources;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.Samples.Base.Direct2D.Direct2DSimpleEffect),
    contractType: typeof(SeeingSharp.Samples.Base.SampleBase))]

namespace SeeingSharp.Samples.Base.Direct2D
{
    [SampleInfo(
        Constants.SAMPLEGROUP_DIRECT2D, "Simple Effect",
        Constants.SAMPLE_DIRECT2D_SIMPLE_EFFECT,
        "https://github.com/RolandKoenig/SeeingSharp/blob/master/Samples/SeeingSharp.Samples/_Samples/Direct2D/Direct2DSimpleEffect.cs",
        SampleTargetPlatform.All)]
    public class Direct2DSimpleEffect : SampleBase
    {
        private StandardBitmapResource m_starBitmap;
        private GaussianBlurEffectResource m_startBitmapShaded;

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

            // Define resources
            m_starBitmap = new StandardBitmapResource(
                new AssemblyResourceLink(
                    typeof(SeeingSharpSampleResources),
                    "Bitmaps.StarColored_128x128.png"));
            m_startBitmapShaded = new GaussianBlurEffectResource(m_starBitmap);
            m_startBitmapShaded.StandardDeviation = 4f;

            // Define 2D overlay
            Action<Graphics2D> draw2DAction = (graphics) =>
            {
                graphics.DrawBitmap(
                    m_starBitmap,
                    new Vector2(10f, 10f),
                    1f,
                    BitmapInterpolationMode.Linear);
                graphics.DrawImage(
                    m_startBitmapShaded,
                    new Vector2(150f, 10f));
            };

            await targetRenderLoop.Scene.ManipulateSceneAsync((manipulator) =>
            {
                // Add the 2D layer to the scene
                manipulator.AddDrawingLayer(draw2DAction);

                // Create floor
                SampleSceneBuilder.BuildStandardFloor(
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
            });

            // Configure camera
            camera.Position = new Vector3(2f, 2f, 2f);
            camera.Target = new Vector3(0f, 0.5f, 0f);
            camera.UpdateCamera();
        }

        public override void OnClosed()
        {
            base.OnClosed();

            CommonTools.SafeDispose(ref m_startBitmapShaded);
            CommonTools.SafeDispose(ref m_starBitmap);
        }
    }
}