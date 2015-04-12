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
        /// <param name="painter">The painter object which represents the view.</param>
        public async Task InitializeAsync(IFrozenSkyPainter painter)
        {
            // Set scene and camera
            painter.Scene = m_scene;
            painter.Camera = m_camera;

            // Initialize gaming logic
            await InitializeInternalAsync(painter);

            // Trigger main loop
            m_scene.PerformBeforeUpdateAsync(OnGameLoopTick)
                .FireAndForget();
        }

        protected abstract Task InitializeInternalAsync(IFrozenSkyPainter painter);

        protected abstract void PerformGameLoopTick();

        /// <summary>
        /// ´Main method of the game loop.
        /// </summary>
        private void OnGameLoopTick()
        {
            try
            {
                this.PerformGameLoopTick();
            }
            finally
            {
                m_scene.PerformBeforeUpdateAsync(OnGameLoopTick)
                    .FireAndForget();
            }
        }

        /// <summary>
        /// Gets the current randomizer object.
        /// </summary>
        public Random Randomizer
        {
            get { return m_randomizer; }
        }
    }
}
