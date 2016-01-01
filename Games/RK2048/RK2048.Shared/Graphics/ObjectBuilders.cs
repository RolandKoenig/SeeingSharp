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

using SeeingSharp;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Util;
using System.Linq;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Text;

namespace RK2048.Graphics
{
    internal static class ObjectBuilders
    {
        /// <summary>
        /// Defines all tile resources.
        /// </summary>
        /// <param name="manipulator"></param>
        public static void DefineTileResoures(this SceneManipulator manipulator)
        {
            // NOTE: For later versions of SeeingSharp: We should be able to change the texture per object
            //       Currently we have to create one VertexStructure per texture - very inefficient for this case!

            float tileWidth = Constants.TILE_WIDTH;
            float tilePadding = Constants.TILE_PADDING;

            Vector3 cubeSize = new Vector3(tileWidth - tilePadding * 2f, 0.1f, tileWidth - tilePadding * 2f);
            Vector3 cubeStart = -cubeSize / 2f;
 
            // Create all tile resources
            for(int loopID = 0; loopID < Constants.RES_GEO_TILES_BY_ID.Length; loopID++)
            {
                // Define texture resources
                var resTileMaterial = manipulator.AddSimpleColoredMaterial(
                    new AssemblyResourceUriBuilder("RK2048", true, string.Format("Assets/Textures/Tile{0}.png", Constants.TILE_VALUE_BY_ID[loopID])),
                    new AssemblyResourceUriBuilder("RK2048", true, string.Format("Assets/Textures/Tile{0}.Low.png", Constants.TILE_VALUE_BY_ID[loopID])));

                // Define current vertex structure
                VertexStructure tileStructure = new VertexStructure(1024 * 3, 1024);
                tileStructure.BuildCubeSides16V(cubeStart, cubeSize, Constants.COLOR_TILE_BASE)
                    .DisableTexture();
                tileStructure.BuildCubeBottom4V(cubeSize, cubeSize, Constants.COLOR_TILE_BASE)
                    .DisableTexture();
                tileStructure.BuildCubeTop4V(cubeStart, cubeSize, Constants.COLOR_TILE_BASE);
                 
                //tileStructure.BuildTextGeometry("2", TextGeometryOptions.Default);
                tileStructure.Material = resTileMaterial;

                // Append the resource to the scene
                manipulator.AddResource(
                    () => new GeometryResource(tileStructure),
                    Constants.RES_GEO_TILES_BY_ID[loopID]);
            }
        }

        /// <summary>
        /// Builds the background texture.
        /// </summary>
        /// <param name="manipulator">The manipulator that is used for building.</param>
        public static void BuildBackground(this SceneManipulator manipulator)
        {
            SceneLayer bgLayer = manipulator.AddLayer("BACKGROUND");
            manipulator.SetLayerOrderID(bgLayer, 0);
            manipulator.SetLayerOrderID(Scene.DEFAULT_LAYER_NAME, 1);

            var resBackgroundTexture = manipulator.AddTexture(
                new AssemblyResourceUriBuilder("RK2048", true, "Assets/Textures/Background.png"));
            manipulator.Add(new TexturePainter(resBackgroundTexture), bgLayer.Name);
        }

        /// <summary>
        /// Builds the game field..
        /// </summary>
        /// <param name="manipulator">The manipulator that is used for building.</param>
        public static void BuildGameField(this SceneManipulator manipulator)
        {
            ResourceLink sourceTileTexture = new AssemblyResourceUriBuilder("RK2048", true, "Assets/Textures/Floor.png");
            ResourceLink sourceTileTextureLow = new AssemblyResourceUriBuilder("RK2048", true, "Assets/Textures/Floor.Low.png");

            // Define textures and materials
            var resTileTexture = manipulator.AddResource(() => new StandardTextureResource(sourceTileTexture, sourceTileTextureLow));
            var resTileMaterial = manipulator.AddResource(() => new SimpleColoredMaterialResource(resTileTexture));

            // Define floor geometry
            FloorType floorType = new FloorType(
                new Vector2(Constants.TILE_WIDTH, Constants.TILE_WIDTH), 0f);
            floorType.BottomMaterial = resTileMaterial;
            floorType.DefaultFloorMaterial = resTileMaterial;
            floorType.SideMaterial = resTileMaterial;
            floorType.SetTilemap(4, 4);

            // Add floor to scene
            var resFloorGeometry = manipulator.AddResource((() => new GeometryResource(floorType)));
            var floorObject = manipulator.AddGeneric(resFloorGeometry);
            floorObject.EnableShaderGeneratedBorder();
        }

        /// <summary>
        /// Builds all labels around the game field.
        /// </summary>
        /// <param name="manipulator">The manipulator that is used for building.</param>
        public static void BuildGameFieldLabels(this SceneManipulator manipulator)
        {
            //// Define text options
            //TextGeometryOptions textOptionsHeader = TextGeometryOptions.Default;
            //textOptionsHeader.FontSize = 40;
            //textOptionsHeader.SurfaceVertexColor = Constants.COLOR_TEXT;
            //textOptionsHeader.VolumetricSideSurfaceVertexColor = Constants.COLOR_TEXT * 0.6f;
            //TextGeometryOptions textOptionsNormal = TextGeometryOptions.Default;
            //textOptionsNormal.FontSize = 12;
            //textOptionsNormal.SurfaceVertexColor = Constants.COLOR_TEXT;
            //textOptionsNormal.VolumetricSideSurfaceVertexColor = Constants.COLOR_TEXT * 0.6f;

            //// Append header text
            //GenericObject textObject = manipulator.Add3DText(Translatables.MAIN_PAGE_TITLE, textOptionsHeader);
            //textObject.Position = new Vector3(-6f, 0.1f, 9f);
            //textObject.EnableShaderGeneratedBorder();

            //// Append a short subheader
            //GenericObject textObject2 = manipulator.Add3DText(Translatables.MAIN_PAGE_SUBTITLE, textOptionsNormal);
            //textObject2.Position = new Vector3(-5.9f, 0.1f, 5.5f);
            //textObject2.EnableShaderGeneratedBorder();
        }
    }
}
