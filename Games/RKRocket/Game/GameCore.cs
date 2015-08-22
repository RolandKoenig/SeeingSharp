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
#endregion
using SeeingSharp;
using SeeingSharp.Util;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Drawing2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace RKRocket.Game
{
    /// <summary>
    /// The central coontrol logic of the game.
    /// </summary>
    public class GameCore : SceneLogicalObject
    {
        #region Level objects
        private PlayerRocket m_playerRocket;
        private List<Block> m_blocks;
        #endregion

        #region Graphics related members
        private PerspectiveCamera3D m_camera;
        private Scene m_gameScene;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="GameCore"/> class.
        /// </summary>
        public GameCore()
        {
            m_blocks = new List<Block>();

            // Create  main scene objects
            m_camera = new PerspectiveCamera3D();
            m_gameScene = new Scene(
                name: Constants.GAME_SCENE_NAME,
                registerOnMessenger: true);

            // Configure 2D screen
            m_gameScene.TransformMode2D = Graphics2DTransformMode.AutoScaleToVirtualScreen;
            m_gameScene.VirtualScreenSize2D = new Size2F(
                Constants.GFX_SCREEN_VPIXEL_WIDTH,
                Constants.GFX_SCREEN_VPIXEL_HEIGHT);

            // Register initial manipulate method
            m_gameScene.ManipulateSceneAsync(OnInitializeGame)
                .FireAndForget();
        }

        /// <summary>
        /// Called before the first frame of the game is rendered on the screen.
        /// </summary>
        /// <param name="manipulator">The manipulator object.</param>
        private void OnInitializeGame(SceneManipulator manipulator)
        {
            // Append this object to the scene
            manipulator.Add(this);

            // Append the background
            manipulator.Add(new Background());

            // Create and append the player
            m_playerRocket = new PlayerRocket();
            manipulator.Add(m_playerRocket);

            // Load first level (create all blocks)
            float cellWidth = Constants.GFX_SCREEN_VPIXEL_WIDTH / Constants.BLOCKS_COUNT_X;
            float cellHeight = Constants.BLOCK_VPIXEL_HEIGHT + 10f;
            for (int loopRow =0; loopRow < 3; loopRow++)
            {
                for(int loopXBlock = 0; loopXBlock < Constants.BLOCKS_COUNT_X; loopXBlock++)
                {
                    Vector2 actBlockPosition = new Vector2(
                        cellWidth * loopXBlock + (cellWidth / 2f),
                        cellHeight * loopRow + (cellHeight / 2f));

                    Block newBlock = new Block();
                    newBlock.Position = actBlockPosition;
                    manipulator.Add(newBlock);
                }
            }
        }

        public Scene GameScene
        {
            get { return m_gameScene; }
        }

        public Camera3DBase Camera
        {
            get { return m_camera; }
        }
    }
}
