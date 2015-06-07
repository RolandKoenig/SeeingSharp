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

using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Util;
using RKVideoMemory.Data;
using SeeingSharp;
using SeeingSharp.Multimedia.Objects;

namespace RKVideoMemory.Game
{
    public class GameCore : SceneLogicalObject
    {
        #region Game related members
        private LevelData m_currentLevel; 
        private GameScreenManagerLogic m_gameScreenManager;
        #endregion

        #region Graphics related members
        private bool m_initialized;
        private PerspectiveCamera3D m_camera;
        private Scene m_scene;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="GameCore"/> class.
        /// </summary>
        public GameCore()
        {
            m_camera = new PerspectiveCamera3D();
            m_scene = new Scene(
                name: Constants.GAME_SCENE_NAME,
                registerOnMessenger: true);
        }

        /// <summary>
        /// Initializes the game in an async way.
        /// </summary>
        public async Task InitializeAsync()
        {
            await m_scene.ManipulateSceneAsync((manipulator) =>
            {
                manipulator.Add(this);
            });

            m_initialized = true;
            Messenger.BeginPublish<GameInitializedMessage>();
        }

        /// <summary>
        /// Loads a new level from the given data path.
        /// </summary>
        /// <param name="sourceDirectory">The data path.</param>
        public async Task LoadLevelAsync(string sourceDirectory)
        {
            // Load the level
            m_currentLevel = await LevelData.FromDirectoryAsync(sourceDirectory);

            // Load graphics for this level
            if(m_gameScreenManager != null)
            {
                await m_gameScreenManager.ClearAsync(m_scene);
            }

            m_gameScreenManager = new GameScreenManagerLogic();

            // Build the first screen
            await m_gameScreenManager.BuildFirstScreenAsync(m_currentLevel, m_scene);

            m_camera.Position = new Vector3(0, 12f, 0f);
            m_camera.Target = new Vector3(0f, 0f, 0.1f);
            m_camera.UpdateCamera();

            m_scene.Messenger.BeginPublish(new LevelLoadedMessage(m_currentLevel));
        }
       
        public bool IsInitialized
        {
            get 
            { 
                return m_initialized; 
            }
        }

        /// <summary>
        /// Gets the currently loaded level.
        /// </summary>
        public LevelData CurrentLevel
        {
            get { return m_currentLevel; }
        }

        public GameScreenManagerLogic CurrentMap
        {
            get { return m_gameScreenManager; }
        }

        public SeeingSharpMessenger Messenger
        {
            get { return m_scene.Messenger; }
        }

        public Scene Scene
        {
            get { return m_scene; }
        }

        public Camera3DBase Camera
        {
            get { return m_camera; }
        }
    }
}
