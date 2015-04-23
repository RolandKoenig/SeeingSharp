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
using FrozenSky;
using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
using FrozenSky.Checking;
using FrozenSky.Util;
using RKVideoMemory.Assets.Textures;
using RKVideoMemory.Data;
using RKVideoMemory.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKVideoMemory.Game
{
    public class GameMap
    {
        private const int CARDMAP_WIDTH = 5;

        private Card[,] m_cardMap;
        private List<CardPair> m_cardPairs;

        /// <summary>
        /// Clears the current map from the given scene.
        /// </summary>
        /// <param name="scene">The scene to be cleard..</param>
        internal async Task ClearAsync(Scene scene)
        {
            m_cardPairs.EnsureNotNull("m_cardPairs");

            await scene.ManipulateSceneAsync((manipulator) =>
            {
                foreach(CardPair actPair in m_cardPairs)
                {
                    foreach (Card actCard in actPair.Cards)
                    {
                        manipulator.Remove(actCard);
                    }
                    manipulator.Remove(actPair);
                }
            });
        }

        /// <summary>
        /// Builds all game objects for the given level.
        /// </summary>
        /// <param name="currentLevel">The current level.</param>
        /// <param name="scene">The scene to which to add all objects.</param>
        internal async Task BuildLevelAsync(LevelData currentLevel, Scene scene)
        {
            int tilesX = Constants.TILEMAP_X_COUNT;
            int tilesY = Constants.TILEMAP_Y_COUNT;
            float tileDistX = Constants.TILE_DISTANCE_X;
            float tileDistY = Constants.TILE_DISTANCE_Y;
            Vector3 midPoint = new Vector3((tilesX - 1) * tileDistX / 2f, 0f, (tilesY - 1) * tileDistY/ 2f);

            m_cardMap = new Card[tilesX, tilesY];
            m_cardPairs = new List<CardPair>();
            Random randomizer = new Random(Environment.TickCount);

            await scene.ManipulateSceneAsync((manipulator) =>
            {
                var resBackgroundMaterial1= manipulator.AddSimpleColoredMaterial(
                    new AssemblyResourceLink(
                        typeof(Textures),
                        "Tile1.png"));
                var resBackgroundMaterial2 = manipulator.AddSimpleColoredMaterial(
                    new AssemblyResourceLink(
                        typeof(Textures),
                        "Tile2.png"));
                foreach (MemoryPairData actPairData in currentLevel.MemoryPairs)
                {
                    CardPair actCardPair = new CardPair(actPairData);

                    // Define all resources needed for a card for this pair
                    var resTitleMaterial = manipulator.AddSimpleColoredMaterial(actPairData.TitleFile);
                    var resGeometry1 = manipulator.AddGeometry(new CardObjectType()
                        {
                            FrontMaterial = resTitleMaterial,
                            BackMaterial = resBackgroundMaterial1
                        });
                    var resGeometry2 = manipulator.AddGeometry(new CardObjectType()
                    {
                        FrontMaterial = resTitleMaterial,
                        BackMaterial = resBackgroundMaterial2
                    });

                    // Create both cards for this pair
                    Card cardA = new Card(resGeometry1, actCardPair);
                    Card cardB = new Card(resGeometry2, actCardPair);
                    Tuple<int, int> slotA = SearchFreeCardSlot(m_cardMap, randomizer);
                    m_cardMap[slotA.Item1, slotA.Item2] = cardA;
                    Tuple<int, int> slotB = SearchFreeCardSlot(m_cardMap, randomizer);
                    m_cardMap[slotB.Item1, slotB.Item2] = cardB;

                    // Add both cards to the scene
                    cardA.Position = new Vector3(slotA.Item1 * tileDistX, 0f, slotA.Item2 * tileDistY) - midPoint;
                    cardB.Position = new Vector3(slotB.Item1 * tileDistX, 0f, slotB.Item2 * tileDistY) - midPoint;
                    manipulator.Add(cardA);
                    manipulator.Add(cardB);

                    // Assigns the cards to the pair object
                    actCardPair.Cards = new Card[] { cardA, cardB };

                    m_cardPairs.Add(actCardPair);
                }
            });
        }

        /// <summary>
        /// Searches the next free slot in the card map.
        /// </summary>
        /// <param name="cardMap">The card map.</param>
        /// <param name="randomizer">The randomizer.</param>
        private static Tuple<int, int> SearchFreeCardSlot(Card[,] cardMap, Random randomizer)
        {
            Tuple<int, int> result = null;

            while(result == null)
            {
                int xPos = randomizer.Next(0, cardMap.GetLength(0));
                int yPos = randomizer.Next(0, cardMap.GetLength(1));
                if(cardMap[xPos, yPos] == null)
                {
                    result = Tuple.Create(xPos, yPos);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the total count of pairs on the map.
        /// </summary>
        public int CountPairs
        {
            get
            {
                if (m_cardPairs == null) { return 0; }
                return m_cardPairs.Count;
            }
        }
    }
}
