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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrozenSky.Util;
using RKVideoMemory.Graphics;
using RKVideoMemory.Data;
using FrozenSky;

namespace RKVideoMemory.Game
{
    public class GameCore
    {
        #region Game related members
        private LevelData m_currentLevel; 
        private GameMap m_gameMap;
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
            await m_scene.BuildBackgroundAsync();

            m_initialized = true;
            Messenger.BeginPublish<GameInitializedMessage>();
        }

        /// <summary>
        /// Loads a new level from the given data path.
        /// </summary>
        /// <param name="sourceDirectory">The data path.</param>
        public async Task LoadLevelAsync(string sourceDirectory)
        {
            if (m_currentLevel != null) { await UnloadCurrentLevel(); }

            // Load the level
            m_currentLevel = await LevelData.FromDirectory(sourceDirectory);

            // Load graphics for this level
            if(m_gameMap != null)
            {
                await m_gameMap.ClearAsync(m_scene);
            }
            m_gameMap = new GameMap();

            await m_gameMap.BuildLevelAsync(m_currentLevel, m_scene);

            m_camera.Position = new Vector3(0, 12f, 0f);
            m_camera.Target = new Vector3(0f, 0f, 0.1f);
            m_camera.UpdateCamera();

            Messenger.BeginPublish<MainMemoryScreenEnteredMessage>();
            Messenger.BeginPublish<LevelLoadedMessage>();
        }

        /// <summary>
        /// Unloads the currently loaded level.
        /// </summary>
        public async Task UnloadCurrentLevel()
        {
            if (m_currentLevel == null) { return; }

            // TODO
            await Task.Delay(100);

            Messenger.BeginPublish<LevelUnloadedMessage>();
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

        public FrozenSkyMessenger Messenger
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
