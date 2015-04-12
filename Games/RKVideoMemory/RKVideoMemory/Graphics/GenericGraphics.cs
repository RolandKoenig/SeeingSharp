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
using RKVideoMemory.Assets.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKVideoMemory.Graphics
{
    public static class GenericGraphics
    {
        public static Task BuildBackgroundAsync(this Scene sceneObject)
        {
            return sceneObject.ManipulateSceneAsync((manipulator) =>
            {
                SceneLayer bgLayer = manipulator.AddLayer(Constants.GFX_LAYER_BACKGROUND);
                manipulator.SetLayerOrderID(
                    bgLayer, 
                    Constants.GFX_LAYER_BACKGROUND_ORDERID);

                var resBackgroundTexture = manipulator.AddTexture(
                    new AssemblyResourceLink(
                        typeof(Textures),
                        "Background.png"));
                manipulator.Add(new TexturePainter(resBackgroundTexture), Constants.GFX_LAYER_BACKGROUND);
            });
        }
    }
}
