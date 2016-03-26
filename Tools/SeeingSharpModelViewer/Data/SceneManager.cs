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
#endregion License information (SeeingSharp and all based games/applications)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Checking;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Util;

namespace SeeingSharpModelViewer.Data
{
    public class SceneManager
    {
        #region main references
        private Scene m_scene;
        private Camera3DBase m_camera;
        #endregion main references

        #region Loaded data
        //private ResourceLink m_currentFile;
        //private ImportedModelContainer m_currentFileContent;
        #endregion Loaded data

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneManager"/> class.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="camera">The camera.</param>
        public SceneManager(Scene scene, Camera3DBase camera)
        {
            scene.EnsureNotNull(nameof(scene));
            camera.EnsureNotNull(nameof(camera));

            m_scene = scene;
            m_camera = camera;

            InitializeAsync()
                .FireAndForget();
        }

        public async Task<IEnumerable<SceneObject>> ImportFileAsync(ResourceLink resourceLink)
        {
            return await m_scene.ImportAsync(resourceLink);
        }

        public Task CloseAsync()
        {
            return m_scene.ManipulateSceneAsync(manipulator => manipulator.ClearLayer(Scene.DEFAULT_LAYER_NAME));
        }

        /// <summary>
        /// Initializes the scene for this model viewer.
        /// </summary>
        private async Task InitializeAsync()
        {
            await m_scene.ManipulateSceneAsync((manipulator) =>
            {
                SceneLayer bgLayer = manipulator.AddLayer("BACKGROUND");
                manipulator.SetLayerOrderID(bgLayer, 0);
                manipulator.SetLayerOrderID(Scene.DEFAULT_LAYER_NAME, 1);

                ResourceLink sourceWallTexture = new AssemblyResourceUriBuilder(
                    "SeeingSharpModelViewer", true,
                    "Assets/Textures/Background.dds");

                var resBackgroundTexture = manipulator.AddTexture(sourceWallTexture);
                manipulator.Add(new TexturePainter(resBackgroundTexture), bgLayer.Name);
            });
        }
    }
}