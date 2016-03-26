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
    targetType: typeof(SeeingSharp.Samples.Base.BasicSamples.TransparentPalletsSample),
    contractType: typeof(SeeingSharp.Samples.Base.SampleBase))]

namespace SeeingSharp.Samples.Base.BasicSamples
{
    [SampleInfo(
        Constants.SAMPLEGROUP_BASIC, "TransparentPallets",
        Constants.SAMPLE_BASICS_TRANSPARENTPALLETS_ORDER,
        "https://github.com/RolandKoenig/SeeingSharp/blob/master/Samples/SeeingSharp.Samples.Base/_Samples/BasicSamples/TransparentPalletsSample.cs",
        SampleTargetPlatform.All)]
    public class TransparentPalletsSample : SampleBase
    {
        /// <summary>
        /// Called when the sample has to startup.
        /// </summary>
        /// <param name="targetRenderLoop">The target render loop.</param>
        /// <returns></returns>
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

                    // Create the wall in the middle
                    AppendWallObjectToScene(manipulator);

                    // Now create all pallets
                    AppendPalletObjectsToScene(manipulator);
                });

                // Configure camera
                camera.Position = new Vector3(30f, 30f, 30f);
                camera.Target = new Vector3(0f, 0f, 0f);
                camera.UpdateCamera();
            }
        }

        private static void AppendWallObjectToScene(SceneManipulator manipulator)
        {
            //Define wall object (define geometry and create object for the scene).
            var resWallTexture = manipulator.AddTexture(
                new AssemblyResourceUriBuilder(
                    "SeeingSharp.Samples.Base", false, 
                    "Assets/Textures/Wall.png"));
            var resWallMaterial = manipulator.AddSimpleColoredMaterial(resWallTexture);

            VertexStructure wallStructure = new VertexStructure();
            wallStructure.FirstSurface.EnableTextureTileMode(new Vector2(2f, 2f));
            wallStructure.FirstSurface.BuildCube24V(
                new Vector3(-0.1f, 0f, -13f),
                new Vector3(0.2f, 10f, 26f),
                Color4.Gray);
            wallStructure.FirstSurface.Material = resWallMaterial;
            var resWallGeometry = manipulator.AddGeometry(wallStructure);
            GenericObject wallObject = manipulator.AddGeneric(resWallGeometry, new Vector3(6.3f, 0f, 0f));
            wallObject.DisableShaderGeneratedBorder();
        }

        private static void AppendPalletObjectsToScene(SceneManipulator manipulator)
        {
            Util.NamedOrGenericKey resPalletGeometry;
            Util.NamedOrGenericKey resPalletGeometryRed;
            Util.NamedOrGenericKey resPalletGeometryGreen;

            // Define pallet geometry
            PalletType palletType = new PalletType();
            PalletType palletTypeRed = new PalletType();
            PalletType palletTypeGreen = new PalletType();
            palletTypeRed.ContentColor = Color4.DarkRed;
            palletTypeGreen.ContentColor = Color4.DarkOliveGreen;

            // Append pallet geometry to scene 
            resPalletGeometry = manipulator.AddGeometry(palletType);
            resPalletGeometryRed = manipulator.AddGeometry(palletTypeRed);
            resPalletGeometryGreen = manipulator.AddGeometry(palletTypeGreen);

            //Append objects to the scene
            Vector3 relocVector = new Vector3(0.5f, 0f, 0);
            List<GenericObject> standardPallets = new List<GenericObject>();
            List<GenericObject> transparentPallets = new List<GenericObject>();
            for (int loopX = 0; loopX < 20; loopX++)
            {
                float xPos = loopX * 1.2f;
                if (loopX >= 5) { xPos += 1f; }

                //White pallets
                standardPallets.Add(manipulator.AddGeneric(resPalletGeometry, relocVector + new Vector3(xPos, 0f, 0f)));
                transparentPallets.Add(manipulator.AddGeneric(resPalletGeometry, relocVector + new Vector3(xPos, 0f, 1.5f)));
                transparentPallets.Add(manipulator.AddGeneric(resPalletGeometry, relocVector + new Vector3(xPos, 0f, -1.5f)));

                //Red pallets
                transparentPallets.Add(manipulator.AddGeneric(resPalletGeometryRed, relocVector + new Vector3(xPos, 0f, 5.0f)));
                standardPallets.Add(manipulator.AddGeneric(resPalletGeometryRed, relocVector + new Vector3(xPos, 0f, 6.5f)));
                transparentPallets.Add(manipulator.AddGeneric(resPalletGeometryRed, relocVector + new Vector3(xPos, 0f, 8f)));
                transparentPallets.Add(manipulator.AddGeneric(resPalletGeometryRed, relocVector + new Vector3(xPos, 0f, 9.5f)));

                //Red pallets
                transparentPallets.Add(manipulator.AddGeneric(resPalletGeometryRed, relocVector + new Vector3(xPos, 5f, 5.0f)));
                standardPallets.Add(manipulator.AddGeneric(resPalletGeometryRed, relocVector + new Vector3(xPos, 5f, 6.5f)));
                transparentPallets.Add(manipulator.AddGeneric(resPalletGeometryRed, relocVector + new Vector3(xPos, 5f, 8f)));
                transparentPallets.Add(manipulator.AddGeneric(resPalletGeometryRed, relocVector + new Vector3(xPos, 5f, 9.5f)));

                //Red pallets
                transparentPallets.Add(manipulator.AddGeneric(resPalletGeometryGreen, relocVector + new Vector3(xPos, 0f, -5.0f)));
                standardPallets.Add(manipulator.AddGeneric(resPalletGeometryGreen, relocVector + new Vector3(xPos, 0f, -6.5f)));
                transparentPallets.Add(manipulator.AddGeneric(resPalletGeometryGreen, relocVector + new Vector3(xPos, 0f, -8f)));
                transparentPallets.Add(manipulator.AddGeneric(resPalletGeometryGreen, relocVector + new Vector3(xPos, 0f, -9.5f)));
            }

            standardPallets.ForEachInEnumeration((actPallet) =>
            {
                actPallet.BorderMultiplyer = 50f;
                actPallet.BorderPart = 0.01f;
            });
            transparentPallets.ForEachInEnumeration((actPallet) =>
            {
                actPallet.BorderMultiplyer = 50f;
                actPallet.BorderPart = 0.01f;
                actPallet.Opacity = 0.5f;
            });
        }
    }
}
