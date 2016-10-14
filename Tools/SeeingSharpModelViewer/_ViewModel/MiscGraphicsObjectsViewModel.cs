#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# Tools. More info at
     - https://github.com/RolandKoenig/SeeingSharp/tree/master/Tools (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)

	This program is distributed under the terms of the Microsoft Public License (Ms-PL)-
	More info at https://msdn.microsoft.com/en-us/library/ff647676.aspx
*/
#endregion License information (SeeingSharp and all based games/applications)

using SeeingSharp;
using SeeingSharp.Checking;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Util;
using System.Numerics;
using System.Threading.Tasks;

namespace SeeingSharpModelViewer
{
    public class MiscGraphicsObjectsViewModel : ViewModelBase
    {
        #region Scene
        private Scene m_scene;
        private SceneLayer m_bgLayer;
        #endregion Scene

        #region Configuration
        private int m_tilesPerSide;
        #endregion Configuration

        public MiscGraphicsObjectsViewModel(Scene scene)
        {
            scene.EnsureNotNull(nameof(scene));

            m_tilesPerSide = 16;

            m_scene = scene;
        }

        /// <summary>
        /// Initializes the scene for this model viewer.
        /// </summary>
        internal async Task InitializeAsync()
        {
            await m_scene.ManipulateSceneAsync((manipulator) =>
            {
                SceneLayer bgImageLayer = null;
                bool isBgImageCreated = false;
                if (manipulator.ContainsLayer(Constants.LAYER_BACKGROUND_FLAT))
                {
                    bgImageLayer = manipulator.GetLayer(Constants.LAYER_BACKGROUND_FLAT);
                    isBgImageCreated = true;
                }
                else
                {
                    bgImageLayer = manipulator.AddLayer(Constants.LAYER_BACKGROUND_FLAT);
                }

                SceneLayer bgLayer = manipulator.AddLayer(Constants.LAYER_BACKGROUND);
                manipulator.SetLayerOrderID(bgImageLayer, 0);
                manipulator.SetLayerOrderID(bgLayer, 1);
                manipulator.SetLayerOrderID(Scene.DEFAULT_LAYER_NAME, 2);

                var keyPostprocess = manipulator.AddResource<FocusPostprocessEffectResource>(
                    () => new FocusPostprocessEffectResource(false));
                if (!manipulator.ContainsLayer(Constants.LAYER_HOVER))
                {
                    SceneLayer layerHover = manipulator.AddLayer(Constants.LAYER_HOVER);
                    layerHover.PostprocessEffectKey = keyPostprocess;
                    layerHover.ClearDepthBufferBeforeRendering = true;
                    manipulator.SetLayerOrderID(layerHover, 3);
                }

                // Store a reference to the background layer
                m_bgLayer = bgLayer;

                // Define background texture
                if (!isBgImageCreated)
                {
                    ResourceLink linkBackgroundTexture = new AssemblyResourceUriBuilder(
                        "SeeingSharpModelViewer", true,
                        "Assets/Textures/Background.dds");
                    NamedOrGenericKey resBackgroundTexture = manipulator.AddTexture(linkBackgroundTexture);
                    manipulator.Add(new TexturePainter(resBackgroundTexture), bgImageLayer.Name);
                }

                // Define ground
                Grid3DType objTypeGrid = new Grid3DType();
                objTypeGrid.TilesX = m_tilesPerSide * 4;
                objTypeGrid.TilesZ = m_tilesPerSide * 4;
                objTypeGrid.HighlightXZLines = true;
                objTypeGrid.TileWidth = 0.25f;
                objTypeGrid.GroupTileCount = 4;
                objTypeGrid.GenerateGround = false;
                objTypeGrid.XLineHighlightColor = Color4.GreenColor;
                objTypeGrid.ZLineHighlightColor = Color4.BlueColor;

                NamedOrGenericKey resGridGeometry = manipulator.AddGeometry(objTypeGrid);
                manipulator.Add(new GenericObject(resGridGeometry), Constants.LAYER_BACKGROUND);

                TextGeometryOptions textXOptions = TextGeometryOptions.Default;
                textXOptions.SurfaceVertexColor = Color4.GreenColor;
                textXOptions.MakeVolumetricText = false;
                textXOptions.FontSize = 30;
                GenericObject textX = manipulator.Add3DText(
                    "X", textXOptions,
                    realignToCenter: true,
                    layer: Constants.LAYER_BACKGROUND);
                textX.Position = new Vector3((m_tilesPerSide / 2f) + 1f, 0, 0);

                TextGeometryOptions textZOptions = TextGeometryOptions.Default;
                textZOptions.SurfaceVertexColor = Color4.BlueColor;
                textZOptions.MakeVolumetricText = false;
                textZOptions.FontSize = 30;

                GenericObject textZ = manipulator.Add3DText(
                    "Z", textZOptions,
                    realignToCenter: true,
                    layer: Constants.LAYER_BACKGROUND);
                textZ.Position = new Vector3(0f, 0f, (m_tilesPerSide / 2f) + 1f);
            });
        }

        /// <summary>
        /// Reloads the background asynchronous.
        /// </summary>
        /// <returns></returns>
        internal async Task ReloadBackgroundAsync()
        {
            if (m_bgLayer != null)
            {
                await m_scene.ManipulateSceneAsync((manipulator) =>
                {
                    manipulator.RemoveLayer(m_bgLayer);
                });
            }

            await InitializeAsync();
        }

        public int TilesPerSide
        {
            get { return m_tilesPerSide; }
            set
            {
                if (m_tilesPerSide != value)
                {
                    m_tilesPerSide = value;
                    if (m_tilesPerSide < Constants.COUNT_TILES_MIN) { m_tilesPerSide = Constants.COUNT_TILES_MIN; }
                    if (m_tilesPerSide > Constants.COUNT_TILES_MAX) { m_tilesPerSide = Constants.COUNT_TILES_MAX; }

                    ReloadBackgroundAsync()
                        .FireAndForget();

                    RaisePropertyChanged(nameof(TilesPerSide));
                }
            }
        }

        public bool BackgroundVisible
        {
            get
            {
                if (m_bgLayer == null) { return false; }
                else { return m_bgLayer.IsRenderingEnabled; }
            }
            set
            {
                if (m_bgLayer == null) { return; }

                if (m_bgLayer.IsRenderingEnabled != value)
                {
                    m_bgLayer.IsRenderingEnabled = value;
                    RaisePropertyChanged(nameof(BackgroundVisible));
                }
            }
        }
    }
}