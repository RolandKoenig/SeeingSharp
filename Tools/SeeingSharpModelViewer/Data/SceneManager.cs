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
using SeeingSharp;

namespace SeeingSharpModelViewer.Data
{
    /// <summary>
    /// The SceneManger acts as the ViewModel and therefore contains all logic
    /// relevant for the ModelViewer application.
    /// </summary>
    public class SceneManager : ViewModelBase
    {
        #region main references
        private RenderLoop m_renderLoop;
        #endregion main references

        #region Loaded data
        private ResourceLink m_currentFile;
        private ImportOptions m_currentImportOptions;
        #endregion Loaded data

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneManager"/> class.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="camera">The camera.</param>
        public SceneManager(RenderLoop renderLoop)
        {
            m_renderLoop = renderLoop;

            InitializeAsync()
                .FireAndForget();
        }

        private async Task SetInitialCameraPositionAsync()
        {
            await m_renderLoop.MoveCameraToDefaultLocationAsync(
                EngineMath.RAD_45DEG, EngineMath.RAD_45DEG);
        }

        /// <summary>
        /// Imports a new file by the given <see cref="ResourceLink"/>.
        /// </summary>
        /// <param name="resourceLink">The <see cref="ResourceLink"/> from which to load the resource.</param>
        public async Task ImportNewFileAsync(ResourceLink resourceLink)
        {
            m_currentFile = resourceLink;
            m_currentImportOptions = GraphicsCore.Current.ImportersAndExporters.CreateImportOptions(m_currentFile);
            base.RaisePropertyChanged(nameof(CurrentFile));
            base.RaisePropertyChanged(nameof(CurrentImportOptions));

            await m_renderLoop.Scene.ImportAsync(m_currentFile, m_currentImportOptions);

            await m_renderLoop.WaitForNextFinishedRenderAsync();

            await SetInitialCameraPositionAsync();
        }

        public async Task ReloadCurrentFileAsync()
        {
            m_currentFile.EnsureNotNull(nameof(m_currentFile));
            m_currentImportOptions.EnsureNotNull(nameof(m_currentImportOptions));

            await CloseAsync(
                clearCurrentFileInfo: false);

            await m_renderLoop.Scene.ImportAsync(m_currentFile, m_currentImportOptions);

            await m_renderLoop.WaitForNextFinishedRenderAsync();

            await SetInitialCameraPositionAsync();
        }

        public async Task CloseAsync(bool clearCurrentFileInfo = true)
        {
            await m_renderLoop.Scene.ManipulateSceneAsync(manipulator => manipulator.ClearLayer(Scene.DEFAULT_LAYER_NAME));

            if(clearCurrentFileInfo)
            {
                m_currentFile = null;
                m_currentImportOptions = null;
                RaisePropertyChanged(nameof(CurrentFile));
                RaisePropertyChanged(nameof(ImportOptions));
            }
        }

        public async Task SetBackgroundVisibility(bool isVisible)
        {
            await m_renderLoop.Scene.ManipulateSceneAsync((manipulator) =>
            {
                var bgLayer = manipulator.TryGetLayer("BACKGROUND");
                if(bgLayer == null) { return; }

                bgLayer.IsRenderingEnabled = isVisible;
            });
        }

        /// <summary>
        /// Initializes the scene for this model viewer.
        /// </summary>
        private async Task InitializeAsync()
        {
            await m_renderLoop.Scene.ManipulateSceneAsync((manipulator) =>
            {
                SceneLayer bgImageLayer = manipulator.AddLayer("BACKGROUND_FLAT");
                SceneLayer bgLayer = manipulator.AddLayer("BACKGROUND");
                manipulator.SetLayerOrderID(bgImageLayer, 0);
                manipulator.SetLayerOrderID(bgLayer, 1);
                manipulator.SetLayerOrderID(Scene.DEFAULT_LAYER_NAME, 2);

                // Define background texture
                ResourceLink linkBackgroundTexture = new AssemblyResourceUriBuilder(
                    "SeeingSharpModelViewer", true,
                    "Assets/Textures/Background.dds");
                NamedOrGenericKey resBackgroundTexture = manipulator.AddTexture(linkBackgroundTexture);
                manipulator.Add(new TexturePainter(resBackgroundTexture), bgImageLayer.Name);

                // Define ground
                Grid3DType objTypeGrid = new Grid3DType();
                objTypeGrid.TilesX = 64;
                objTypeGrid.TilesZ = 64;
                objTypeGrid.HighlightXZLines = true;
                objTypeGrid.TileWidth = 0.25f;
                objTypeGrid.GroupTileCount = 4;
                objTypeGrid.GenerateGround = false;

                NamedOrGenericKey resGridGeometry = manipulator.AddGeometry(objTypeGrid);
                manipulator.Add(new GenericObject(resGridGeometry), "BACKGROUND");
            });

            await m_renderLoop.WaitForNextFinishedRenderAsync();
            await m_renderLoop.WaitForNextFinishedRenderAsync();

            await SetInitialCameraPositionAsync();
        }

        public ResourceLink CurrentFile
        {
            get { return m_currentFile; }
        }

        public ImportOptions CurrentImportOptions
        {
            get { return m_currentImportOptions; }
        }
    }
}