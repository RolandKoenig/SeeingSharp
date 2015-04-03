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
