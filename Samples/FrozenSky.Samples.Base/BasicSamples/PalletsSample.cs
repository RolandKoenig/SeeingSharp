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
using FrozenSky.Infrastructure;
using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
using FrozenSky.Multimedia.Objects;
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(FrozenSky.Samples.Base.BasicSamples.PalletsSample),
    contractType: typeof(FrozenSky.Samples.Base.SampleBase))]

namespace FrozenSky.Samples.Base.BasicSamples
{
    [SampleInfo(Constants.SAMPLEGROUP_BASIC, "Pallets")]
    public class PalletsSample : SampleBase
    {
        private const int SIDE_LENGTH = 20;
        private const float SPACE_X = 1.4f;
        private const float SPACE_Y = 1.3f;
        private const float SPACE_Z = 1f;

        private static readonly Color4[] POSSIBLE_COLORS = new Color4[]
        {
            Color4.LightBlue,
            Color4.SteelBlue,
            Color4.LightSteelBlue,
        };

        /// <summary>
        /// Called when the sample has to startup.
        /// </summary>
        /// <param name="targetRenderLoop">The target render loop.</param>
        public override async Task OnStartup(RenderLoop targetRenderLoop)
        {
            // Build dummy scene
            Scene scene = targetRenderLoop.Scene;
            Camera3DBase camera = targetRenderLoop.Camera as Camera3DBase;

            // Build scene initially if we are on first load
            if (scene.CountObjects <= 0)
            {
                await scene.ManipulateSceneAsync((manipulator) =>
                {
                    // Create pallet geometry resource
                    PalletType pType = new PalletType();
                    pType.ContentColor = Color4.Transparent;
                    var resPalletGeometry = manipulator.AddResource<GeometryResource>(
                        () => new GeometryResource(pType));

                    // Create floor
                    SampleSceneBuilder.BuildStandardConveyorFloor(
                        manipulator, Scene.DEFAULT_LAYER_NAME);

                    // Build the wall object
                    PalletsAppendWallObjectToScene(manipulator, SIDE_LENGTH);

                    // Trigger building of the pallet stack
                    PalletsBuildPalletCube(manipulator, new NamedOrGenericKey[] { resPalletGeometry }, SIDE_LENGTH);
                });

                // Configure camera
                camera.Position = new Vector3(30f, 30f, 30f);
                camera.Target = new Vector3(0f, 0f, 0f);
                camera.UpdateCamera();
            }
        }

        public static async void BuildPalletsDemoTextured(RenderLoop renderLoop)
        {
            // Build dummy scene
            Scene scene = renderLoop.Scene;
            Camera3DBase camera = renderLoop.Camera as Camera3DBase;

            // Build scene initially if we are on first load
            if (scene.CountObjects <= 0)
            {
                await scene.ManipulateSceneAsync((manipulator) =>
                {
                    NamedOrGenericKey resMaterialLogo = manipulator.AddSimpleColoredMaterial(
                        new AssemblyResourceUriBuilder(
                            "FrozenSky.Samples.Base", false, 
                            "Textures/LogoTexture.png"));

                    // Create pallet geometry resource
                    SingleMaterialPalletType pType = new SingleMaterialPalletType();
                    pType.ContentColor = Color4.Transparent;
                    var resPalletGeometry = manipulator.AddResource<GeometryResource>(
                        () => new GeometryResource(pType));
                    SingleMaterialPalletType pType2 = new SingleMaterialPalletType();
                    pType2.ContentColor = Color4.Transparent;
                    pType2.PalletMaterial = resMaterialLogo;
                    var resPalletGeometry2 = manipulator.AddResource<GeometryResource>(
                        () => new GeometryResource(pType2));

                    // Create floor
                    SampleSceneBuilder.BuildStandardConveyorFloor(
                        manipulator, Scene.DEFAULT_LAYER_NAME);

                    // Build the wall object
                    PalletsAppendWallObjectToScene(manipulator, SIDE_LENGTH);

                    // Trigger building of the pallet stack
                    PalletsBuildPalletCube(
                        manipulator,
                        new NamedOrGenericKey[] { resPalletGeometry, resPalletGeometry2 },
                        SIDE_LENGTH);
                });

                // Configure camera
                camera.Position = new Vector3(30f, 30f, 30f);
                camera.Target = new Vector3(0f, 0f, 0f);
                camera.UpdateCamera();
            }
        }

        /// <summary>
        /// Builds the demo scene.
        /// </summary>
        /// <param name="manipulator">Current scene manipulator object.</param>
        /// <param name="sideLength">The side length of the pallet cube.</param>
        /// <param name="camera">The camera to be manipulated too.</param>
        private static void PalletsBuildPalletCube(SceneManipulator manipulator, NamedOrGenericKey[] resPalletGeometrys, int sideLength)
        {
            // Build the scene
            Vector3 startPosition = new Vector3(-sideLength * SPACE_X / 2f, 0f, -sideLength * SPACE_Z / 2f);
            Random randomizer = new Random(Environment.TickCount);
            for (int loopX = 0; loopX < sideLength; loopX++)
            {
                for (int loopY = 0; loopY < sideLength; loopY++)
                {
                    for (int loopZ = 0; loopZ < sideLength; loopZ++)
                    {
                        GenericObject palletObject = new GenericObject(
                            resPalletGeometrys[randomizer.Next(0, resPalletGeometrys.Length)],
                            new Vector3(loopX * SPACE_X, loopY * SPACE_Y, loopZ * SPACE_Z) + startPosition);
                        palletObject.Color = POSSIBLE_COLORS[randomizer.Next(0, POSSIBLE_COLORS.Length)];
                        palletObject.EnableShaderGeneratedBorder();
                        palletObject.IsStatic = true;
                        manipulator.Add(palletObject);
                    }
                }
            }
        }

        private static void PalletsAppendWallObjectToScene(SceneManipulator manipulator, int sideLength)
        {
            // Define wall object (define geometry and create object for the scene).
            var resWallTexture = manipulator.AddTexture(
                new AssemblyResourceUriBuilder(
                    "FrozenSky.Samples.Base", false,
                    "Assets/Textures/Wall.png"));
            var resWallMaterial = manipulator.AddSimpleColoredMaterial(resWallTexture);

            VertexStructure wallStructure = new VertexStructure();
            wallStructure.EnableTextureTileMode(new Vector2(2f, 2f));
            wallStructure.BuildCube24V(
                new Vector3(-sideLength * SPACE_X / 2f - 10f, 0f, -sideLength * SPACE_Z / 2f),
                new Vector3(0.2f, sideLength * SPACE_Y, sideLength * SPACE_Z),
                Color4.Gray);
            wallStructure.Material = resWallMaterial;
            var resWallGeometry = manipulator.AddGeometry(wallStructure);
            GenericObject wallObject = manipulator.AddGeneric(resWallGeometry, new Vector3(6.3f, 0f, 0f));
        }
    }
}
