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
using RKRocket.ViewModel;
using SeeingSharp.Gaming;
using SeeingSharp.Infrastructure;

namespace RKRocket.Game
{
    /// <summary>
    /// The central coontrol logic of the game.
    /// </summary>
    public class GameCore : SceneLogicalObject
    {
        #region Level objects
        private PlayerRocketEntity m_playerRocket;
        private List<BlockEntity> m_blocks;
        #endregion

        #region Graphics related members
        private PerspectiveCamera3D m_camera;
        private Scene m_gameScene;
        #endregion

        #region Game states
        private bool m_isGameOver;
        private bool m_isMenuOpened;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="GameCore"/> class.
        /// </summary>
        public GameCore()
        {
            m_blocks = new List<BlockEntity>();

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

            // Initialize the scene
            m_gameScene.ManipulateSceneAsync(OnInitializeGame)
                .FireAndForget();
        }

        /// <summary>
        /// Updates all global states of the scene.
        /// </summary>
        private void UpdateStates()
        {
            m_gameScene.IsPaused = m_isGameOver || m_isMenuOpened;
        }

        /// <summary>
        /// Called before the first frame of the game is rendered on the screen.
        /// </summary>
        /// <param name="manipulator">The manipulator object.</param>
        private void OnInitializeGame(SceneManipulator manipulator)
        {
            // Append this object to the scene
            manipulator.Add(this);

            // Append systems
            manipulator.Add(new CollisionSystem());
            manipulator.Add(new FragmentSystem());
            manipulator.Add(new LevelSystem());
            manipulator.Add(new ScoreSystem());
            manipulator.Add(new HealthSystem());
            manipulator.Add(new AudioSystem());

            // Append the background
            manipulator.Add(new BackgroundEntity());

            // Create and append the player
            m_playerRocket = new PlayerRocketEntity();
            manipulator.Add(m_playerRocket);
        }

        private void OnMessage_Received(MessageGameOver message)
        {
            m_isGameOver = true;

            this.UpdateStates();
        }

        private void OnMessage_Received(MessageNewGame message)
        {
            m_isGameOver = false;

            this.UpdateStates();
        }

        private void OnMessage_Received(MessageMenuOpened message)
        {
            m_isMenuOpened = true;

            this.UpdateStates();
        }

        private void OnMessage_Received(MessageMenuClosed message)
        {
            m_isMenuOpened = false;

            this.UpdateStates();
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
