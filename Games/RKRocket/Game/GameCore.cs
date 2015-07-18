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
using SeeingSharp.Util;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKRocket.Game
{
    public class GameCore : SceneLogicalObject
    {
        //#region Game related members
        //private LevelData m_currentLevel;
        //private GameScreenManagerLogic m_gameScreenManager;
        //#endregion

        #region Graphics related members
        private bool m_initialized;
        private PerspectiveCamera3D m_camera;
        private Scene m_gameScene;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="GameCore"/> class.
        /// </summary>
        public GameCore()
        {
            // Create  main scene objects
            m_camera = new PerspectiveCamera3D();
            m_gameScene = new Scene(
                name: Constants.GAME_SCENE_NAME,
                registerOnMessenger: true);

            // Register initial manipulate method
            m_gameScene.ManipulateSceneAsync(OnInitializeGame)
                .FireAndForget();
        }

        private void OnInitializeGame(SceneManipulator manipulator)
        {
            manipulator.Add(this);
            manipulator.Add(new Background());
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
