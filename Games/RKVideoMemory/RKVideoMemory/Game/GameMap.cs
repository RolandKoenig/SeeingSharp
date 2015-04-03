using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
using RKVideoMemory.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKVideoMemory.Game
{
    public class GameMap
    {
        /// <summary>
        /// Builds all game objects for the given level.
        /// </summary>
        /// <param name="currentLevel">The current level.</param>
        /// <param name="scene">The scene to which to add all objects.</param>
        internal async Task BuildLevelAsync(LevelData currentLevel, Scene scene)
        {
            await scene.ManipulateSceneAsync((manipulator) =>
            {
                foreach (MemoryPairData actPairData in currentLevel.MemoryPairs)
                {
                    var resTitleTexture = manipulator.AddTexture(
                        actPairData.TitleFile);

                }
            });
        }
    }
}
