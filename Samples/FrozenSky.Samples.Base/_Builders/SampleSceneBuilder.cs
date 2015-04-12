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
        /// <summary>
        /// Builds the standard scene.
        /// </summary>
        /// <param name="newScene">The scenegraph to be updated.</param>
        /// <param name="newCamera">The camera to be updated.</param>
        public static void BuildStandardConveyorFloor(this SceneManipulator manipulator, string sceneLayer)
        {
            SceneLayer bgLayer = manipulator.AddLayer("BACKGROUND");
            manipulator.SetLayerOrderID(bgLayer, 0);
            manipulator.SetLayerOrderID(Scene.DEFAULT_LAYER_NAME, 1);

#if DESKTOP || WINDOWS_PHONE
            ResourceLink sourceWallTexture = new Uri("/FrozenSky.Samples.Base;component/Assets/Textures/Background.dds", UriKind.Relative);
            ResourceLink sourceTileTexture = new Uri("/FrozenSky.Samples.Base;component/Assets/Textures/Floor.dds", UriKind.Relative);
#else
            ResourceLink sourceWallTexture = new Uri("ms-appx:///FrozenSky.Samples.Base/Assets/Textures/Background.png");
            ResourceLink sourceTileTexture = new Uri("ms-appx:///FrozenSky.Samples.Base/Assets/Textures/Floor.png");
#endif

            var resBackgroundTexture = manipulator.AddTexture(sourceWallTexture);
            manipulator.Add(new TexturePainter(resBackgroundTexture), bgLayer.Name);

            // Define textures and materials
#if WINDOWS_PHONE
            var resTileTexture = manipulator.AddResource(() => new StandardTextureResource(sourceTileTexture));
            var resTileMaterial = manipulator.AddResource(() => new SimpleColoredMaterialResource(resTileTexture));
            //var resTileMaterial = manipulator.AddResource(() => new SimpleColoredMaterialResource());
#else
            var resTileTexture = manipulator.AddResource(() => new StandardTextureResource(sourceTileTexture));
            var resTileMaterial = manipulator.AddResource(() => new SimpleColoredMaterialResource(resTileTexture));
#endif

            // Define floor geometry
            FloorType floorType = new FloorType(new Vector2(4f, 4f), 0f);
            floorType.BottomMaterial = resTileMaterial;
            floorType.DefaultFloorMaterial = resTileMaterial;
            floorType.SideMaterial = resTileMaterial;
            floorType.SetTilemap(25, 25);

            // Add floor to scene
            var resFloorGeometry = manipulator.AddResource((() => new GeometryResource(floorType)));
            var floorObject = manipulator.AddGeneric(resFloorGeometry, sceneLayer);
        }
    }
}
