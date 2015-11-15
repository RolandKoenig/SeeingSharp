using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Checking;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace SeeingSharpModelViewer.Data
{
    public class SceneManager
    {
        #region main references
        private Scene m_scene;
        private Camera3DBase m_camera;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneManager"/> class.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="camera">The camera.</param>
        public SceneManager(Scene scene, Camera3DBase camera)
        {
            scene.EnsureNotNull("scene");
            camera.EnsureNotNull("camera");

            m_scene = scene;
            m_camera = camera;

            InitializeAsync()
                .FireAndForget();
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
                ResourceLink sourceTileTexture = new AssemblyResourceUriBuilder(
                    "SeeingSharpModelViewer", true,
                    "Assets/Textures/Floor.dds");

                var resBackgroundTexture = manipulator.AddTexture(sourceWallTexture);
                manipulator.Add(new TexturePainter(resBackgroundTexture), bgLayer.Name);

                // Define textures and materials
                var resTileTexture = manipulator.AddResource(() => new StandardTextureResource(sourceTileTexture));
                var resTileMaterial = manipulator.AddResource(() => new SimpleColoredMaterialResource(resTileTexture));

                // Define floor geometry
                FloorType floorType = new FloorType(new Vector2(4f, 4f), 0f);
                floorType.BottomMaterial = resTileMaterial;
                floorType.DefaultFloorMaterial = resTileMaterial;
                floorType.SideMaterial = resTileMaterial;
                floorType.SetTilemap(25, 25);

                // Add floor to scene
                var resFloorGeometry = manipulator.AddResource((() => new GeometryResource(floorType)));
                var floorObject = manipulator.AddGeneric(resFloorGeometry);
            });
        }
    }
}
