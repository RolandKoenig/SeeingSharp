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

	********************************************************************************
    Additional permission under GNU GPL version 3 section 7

    If you modify this Program, or any covered work, by linking or combining it with 
	DirectX (or a modified version of that library), containing parts covered by the 
	terms of [name of library's license], the licensors of this Program grant you additional 
	permission to convey the resulting work. {Corresponding Source for a non-source form of 
	such a combination shall include the source code for the parts of DirectX used 
	as well as that of the covered work.}
*/
#endregion
using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
using FrozenSky.Multimedia.Views;
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Multimedia.Gaming
{
    public abstract class GameBase
    {
        // Objects for painting
        #region
        private Scene m_scene;
        private Camera3DBase m_camera;
        #endregion

        private Random m_randomizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameBase"/> class.
        /// </summary>
        public GameBase()
        {
            // Prepare scene and camera
            m_scene = new Scene();
            m_camera = new PerspectiveCamera3D();

            m_randomizer = new Random(Environment.TickCount);
        }

        /// <summary>
        /// Initializes this game object.
        /// </summary>
        internal async Task InitializeAsync(IFrozenSkyPainter view)
        {
            // Set scene and camera
            view.Scene = m_scene;
            view.Camera = m_camera;

#if UNIVERSAL
            App.Current.Suspending += OnApp_Suspending;
#endif
            //UpdateCameraLocation();

            //this.CurrentScore = 0;
            //this.MaximumReachedScore = 0;

            //// Perform all initializations on the scene
            //await m_scene.ManipulateSceneAsync((manipulator) =>
            //{
            //    manipulator.DefineTileResoures();
            //    manipulator.BuildBackground();
            //    manipulator.BuildGameField();
            //    manipulator.BuildGameFieldLabels();
            //});

            //// Perform start logic
            //await RestartGameAsync();

            m_scene.PerformBeforeUpdateAsync(OnGameLoopTick)
                .FireAndForget();
        }

        private void OnGameLoopTick()
        {
            try
            {

            }
            finally
            {
                m_scene.PerformBeforeUpdateAsync(OnGameLoopTick)
                    .FireAndForget();
            }
        }
    }
}
