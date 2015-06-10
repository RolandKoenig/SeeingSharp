#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it.
    More info at
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
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
#endregion License information (SeeingSharp and all based games/applications)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RKVideoMemory.Assets.Textures;
using RKVideoMemory.Data;
using RKVideoMemory.Util;
using SeeingSharp;
using SeeingSharp.Checking;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Util;

namespace RKVideoMemory.Game
{
    public class GameScreenManagerLogic : SceneLogicalObject
    {
        private const int CARDMAP_WIDTH = 5;

        #region all needed level data
        private LevelData m_currentLevel;
        #endregion all needed level data

        #region Members describing the current screen
        private Card[,] m_cardMapOnScreen;
        private List<CardPairLogic> m_cardPairsOnScreen;
        private int m_actScreenIndex;
        #endregion Members describing the current screen

        #region Graphics resource keys
        private Scene m_scene;
        private NamedOrGenericKey m_resBackgroundMaterial1;
        private NamedOrGenericKey m_resBackgroundMaterial2;
        #endregion Graphics resource keys

        /// <summary>
        /// Clears the current map from the given scene.
        /// </summary>
        /// <param name="scene">The scene to be cleard..</param>
        internal async Task ClearAsync(Scene scene)
        {
            m_cardPairsOnScreen.EnsureNotNull("m_cardPairs");

            await scene.ManipulateSceneAsync((manipulator) =>
            {
                foreach (CardPairLogic actPair in m_cardPairsOnScreen)
                {
                    foreach (Card actCard in actPair.Cards)
                    {
                        manipulator.Remove(actCard);
                    }
                    manipulator.Remove(actPair);
                }

                manipulator.Clear(true);
            });
        }

        /// <summary>
        /// Builds all game objects for the given level.
        /// </summary>
        /// <param name="currentLevel">The current level.</param>
        /// <param name="scene">The scene to which to add all objects.</param>
        internal async Task BuildFirstScreenAsync(LevelData currentLevel, Scene scene)
        {
            m_scene = scene;
            m_currentLevel = currentLevel;

            int tilesX = currentLevel.Tilemap.TilesX;
            int tilesY = currentLevel.Tilemap.TilesY;
            float tileDistX = Constants.TILE_DISTANCE_X;
            float tileDistY = -Constants.TILE_DISTANCE_Y;
            Vector3 midPoint = new Vector3((tilesX - 1) * tileDistX / 2f, 0f, ((tilesY - 1) * tileDistY / 2f));

            m_cardMapOnScreen = new Card[tilesX, tilesY];
            m_cardPairsOnScreen = new List<CardPairLogic>();

            ScreenData currentScreen = currentLevel.Screens[0];
            m_actScreenIndex = 0;

            await scene.ManipulateSceneAsync((manipulator) =>
            {
                // Build background and define level-wide resources
                manipulator.BuildBackground(currentLevel.MainTextures.BackgroundTextureLink);
                m_resBackgroundMaterial1 = manipulator.AddSimpleColoredMaterial(
                    currentLevel.MainTextures.Tile1TextureLink);
                m_resBackgroundMaterial2 = manipulator.AddSimpleColoredMaterial(
                    currentLevel.MainTextures.Tile2TextureLink);

                // Build the current screen
                BuildScreen(manipulator, currentScreen);

                // Add all logic components to the scene
                manipulator.Add(new PairUncoverLogic());
                manipulator.Add(new VideoPlayLogic());
                manipulator.Add(new BackgroundMusicLogic(currentLevel));
                manipulator.Add(new EndGameLogic(currentLevel));
                manipulator.Add(this);
            });

            Messenger.BeginPublish<MainMemoryScreenEnteredMessage>();
        }

        /// <summary>
        /// Builds up the given screen on the given SceneManipulator.
        /// </summary>
        /// <param name="manipulator">The manipulator.</param>
        /// <param name="currentScreen">The screen to be build.</param>
        private void BuildScreen(SceneManipulator manipulator, ScreenData currentScreen)
        {
            int tilesX = m_currentLevel.Tilemap.TilesX;
            int tilesY = m_currentLevel.Tilemap.TilesY;
            float tileDistX = Constants.TILE_DISTANCE_X;
            float tileDistY = -Constants.TILE_DISTANCE_Y;
            Vector3 midPoint = new Vector3((tilesX - 1) * tileDistX / 2f, 0f, ((tilesY - 1) * tileDistY / 2f));

            foreach (CardPairData actPairData in currentScreen.MemoryPairs)
            {
                CardPairLogic actCardPair = new CardPairLogic(actPairData);

                // Define all resources needed for a card for this pair
                var resTitleMaterial = manipulator.AddSimpleColoredMaterial(actPairData.TitleFile);
                var resGeometry1 = manipulator.AddGeometry(new CardObjectType()
                {
                    FrontMaterial = resTitleMaterial,
                    BackMaterial = m_resBackgroundMaterial1
                });
                var resGeometry2 = manipulator.AddGeometry(new CardObjectType()
                {
                    FrontMaterial = resTitleMaterial,
                    BackMaterial = m_resBackgroundMaterial2
                });

                // Create both cards for this pair
                Card cardA = new Card(resGeometry1, actCardPair);
                Card cardB = new Card(resGeometry2, actCardPair);
                Tuple<int, int> slotA = SearchFreeCardSlot(m_currentLevel, m_cardMapOnScreen);
                m_cardMapOnScreen[slotA.Item1, slotA.Item2] = cardA;
                Tuple<int, int> slotB = SearchFreeCardSlot(m_currentLevel, m_cardMapOnScreen);
                m_cardMapOnScreen[slotB.Item1, slotB.Item2] = cardB;

                // Add both cards to the scene
                cardA.Position = new Vector3(slotA.Item1 * tileDistX, 0f, slotA.Item2 * tileDistY) - midPoint;
                cardA.AccentuationFactor = 1f;
                cardB.Position = new Vector3(slotB.Item1 * tileDistX, 0f, slotB.Item2 * tileDistY) - midPoint;
                cardB.AccentuationFactor = 1f;
                manipulator.Add(cardA);
                manipulator.Add(cardB);

                // Assigns the cards to the pair object
                actCardPair.Cards = new Card[] { cardA, cardB };
                manipulator.Add(actCardPair);

                m_cardPairsOnScreen.Add(actCardPair);
            }
        }

        /// <summary>
        /// Frees the current screen.
        /// </summary>
        /// <param name="manipulator">The manipulator.</param>
        private void FreeCurrentScreen(SceneManipulator manipulator)
        {
            foreach (CardPairLogic actCardPair in m_cardPairsOnScreen)
            {
                manipulator.Remove(actCardPair);
                manipulator.RemoveRange(actCardPair.Cards);
            }

            for (int loopX = 0; loopX < m_cardMapOnScreen.GetLength(0); loopX++)
            {
                for (int loopY = 0; loopY < m_cardMapOnScreen.GetLength(1); loopY++)
                {
                    m_cardMapOnScreen[loopX, loopY] = null;
                }
            }
        }

        /// <summary>
        /// Searches the next free slot in the card map.
        /// </summary>
        /// <param name="currentLevel">The current leveldata.</param>
        /// <param name="cardMap">The card map.</param>
        private static Tuple<int, int> SearchFreeCardSlot(LevelData currentLevel, Card[,] cardMap)
        {
            Tuple<int, int> result = null;

            while (result == null)
            {
                int xPos = ThreadSafeRandom.Next(0, cardMap.GetLength(0));
                int yPos = ThreadSafeRandom.Next(0, cardMap.GetLength(1));

                // Check whether the tile position is allowed
                if (!currentLevel.Tilemap[xPos, yPos]) { continue; }

                // Check whether the tile position is used already
                if (cardMap[xPos, yPos] != null) { continue; }

                result = Tuple.Create(xPos, yPos);
            }

            return result;
        }

        /// <summary>
        /// Called when a movie has finished playing.
        /// </summary>
        /// <param name="movieFinishedMessage">The movie finished message.</param>
        private async void OnMessage_Received(PlayMovieFinishedMessage movieFinishedMessage)
        {
            // Check whether we've finished the current screen
            bool isCurrentScreenFinished = true;
            foreach (CardPairLogic actCardPair in m_cardPairsOnScreen)
            {
                if (!actCardPair.IsUncovered)
                {
                    isCurrentScreenFinished = false;
                    break;
                }
            }

            // Call default screen fade-in if the current one has not finished yet
            if (!isCurrentScreenFinished)
            {
                Messenger.Publish<MainMemoryScreenEnteredMessage>();
                return;
            }

            if (m_currentLevel.Screens.Count - 1 <= m_actScreenIndex)
            {
                // Remove old object from the scene
                await m_scene.ManipulateSceneAsync((manipulator) =>
                {
                    FreeCurrentScreen(manipulator);
                });

                // We have finished with all screens, show ending animation
                Messenger.Publish<GameEndReachedMessage>();
            }
            else
            {
                // Build the new Screen
                m_actScreenIndex++;
                ScreenData nextScreen = m_currentLevel.Screens[m_actScreenIndex];
                await m_scene.ManipulateSceneAsync((manipulator) =>
                {
                    FreeCurrentScreen(manipulator);
                    BuildScreen(manipulator, nextScreen);
                });

                // Trigger scree fade-in
                Messenger.Publish<MainMemoryScreenEnteredMessage>();
            }
        }

        /// <summary>
        /// Gets the total count of pairs on the map.
        /// </summary>
        public int CountPairs
        {
            get
            {
                if (m_cardPairsOnScreen == null) { return 0; }
                return m_cardPairsOnScreen.Count;
            }
        }

        public List<CardPairLogic> Pairs
        {
            get { return m_cardPairsOnScreen; }
        }
    }
}