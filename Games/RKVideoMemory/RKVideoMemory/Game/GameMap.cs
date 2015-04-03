using FrozenSky;
using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
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
        /// Builds all game objects for the given level.
        /// </summary>
        /// <param name="currentLevel">The current level.</param>
        /// <param name="scene">The scene to which to add all objects.</param>
        internal async Task BuildLevelAsync(LevelData currentLevel, Scene scene)
        {
            m_cardMap = new Card[5, 5];
            m_cardPairs = new List<CardPair>();
            Random randomizer = new Random(Environment.TickCount);

            await scene.ManipulateSceneAsync((manipulator) =>
            {
                var resBackgroundMaterial= manipulator.AddSimpleColoredMaterial(
                    new AssemblyResourceLink(
                        typeof(Textures),
                        "CardBackground.png"));
                foreach (MemoryPairData actPairData in currentLevel.MemoryPairs)
                {
                    CardPair actCardPair = new CardPair(actPairData);

                    // Define all resources needed for a card for this pair
                    var resTitleMaterial = manipulator.AddSimpleColoredMaterial(actPairData.TitleFile);
                    var resGeometry = manipulator.AddGeometry(new CardObjectType()
                        {
                            FrontMaterial = resTitleMaterial,
                            BackMaterial = resBackgroundMaterial
                        });

                    // Create both cards for this pair
                    Card cardA = new Card(resGeometry, actCardPair);
                    Card cardB = new Card(resGeometry, actCardPair);
                    Tuple<int, int> slotA = SearchFreeCardSlot(m_cardMap, randomizer);
                    m_cardMap[slotA.Item1, slotA.Item2] = cardA;
                    Tuple<int, int> slotB = SearchFreeCardSlot(m_cardMap, randomizer);
                    m_cardMap[slotB.Item1, slotB.Item2] = cardB;

                    // Add both cards to the scene
                    cardA.Position = new Vector3(slotA.Item1 * 1.5f, 0f, slotA.Item2 * 1.5f);
                    cardB.Position = new Vector3(slotB.Item1 * 1.5f, 0f, slotB.Item2 * 1.5f);
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
    }
}
