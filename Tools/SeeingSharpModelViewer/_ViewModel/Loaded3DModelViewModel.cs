#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# Tools. More info at 
     - https://github.com/RolandKoenig/SeeingSharp/tree/master/Tools (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
	This program is distributed under the terms of the Microsoft Public License (Ms-PL)-
	More info at https://msdn.microsoft.com/en-us/library/ff647676.aspx
*/
#endregion
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp;
using SeeingSharp.Checking;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Infrastructure;

namespace SeeingSharpModelViewer
{
    public class Loaded3DModelViewModel : ViewModelBase
    {
        #region main references
        private Scene m_scene;
        private Camera3DBase m_camera;
        #endregion main references

        #region Loaded data
        private ResourceLink m_currentFile;
        private ImportOptions m_currentImportOptions;
        #endregion Loaded data

        /// <summary>
        /// Initializes a new instance of the <see cref="Loaded3DModelViewModel"/> class.
        /// </summary>
        public Loaded3DModelViewModel()
        {
            if (!SeeingSharpApplication.IsInitialized) { return; }

            m_scene = new Scene();
            m_scene.DiscardAutomaticUnload = true;

            m_camera = new PerspectiveCamera3D();
        }

        /// <summary>
        /// Creates the test data for the designer.
        /// </summary>
        public static Loaded3DModelViewModel CreateTestDataForDesigner()
        {
            return null;
        }


        private Task SetInitialCameraPositionAsync()
        {
            return Task.Delay(100);

          
            //await m_renderLoop.MoveCameraToDefaultLocationAsync(
            //    EngineMath.RAD_45DEG, EngineMath.RAD_45DEG);
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

            await m_scene.ImportAsync(m_currentFile, m_currentImportOptions);

            await m_scene.PerformBesideRenderingAsync(() => { });

            await SetInitialCameraPositionAsync();
        }

        public async Task ReloadCurrentFileAsync()
        {
            m_currentFile.EnsureNotNull(nameof(m_currentFile));
            m_currentImportOptions.EnsureNotNull(nameof(m_currentImportOptions));

            await CloseAsync(
                clearCurrentFileInfo: false);

            await m_scene.ImportAsync(m_currentFile, m_currentImportOptions);

            await m_scene.PerformBesideRenderingAsync(() => { });
            //await m_renderLoop.WaitForNextFinishedRenderAsync();

            await SetInitialCameraPositionAsync();
        }

        public async Task CloseAsync(bool clearCurrentFileInfo = true)
        {
            await m_scene.ManipulateSceneAsync(manipulator => manipulator.ClearLayer(Scene.DEFAULT_LAYER_NAME));

            if (clearCurrentFileInfo)
            {
                m_currentFile = null;
                m_currentImportOptions = null;
                RaisePropertyChanged(nameof(CurrentFile));
                RaisePropertyChanged(nameof(ImportOptions));
            }
        }

        public async Task SetBackgroundVisibility(bool isVisible)
        {
            await m_scene.ManipulateSceneAsync((manipulator) =>
            {
                var bgLayer = manipulator.TryGetLayer("BACKGROUND");
                if (bgLayer == null) { return; }

                bgLayer.IsRenderingEnabled = isVisible;
            });
        }

        /// <summary>
        /// Initializes the scene for this model viewer.
        /// </summary>
        internal async Task InitializeAsync()
        {
            await m_scene.ManipulateSceneAsync((manipulator) =>
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
