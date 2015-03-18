using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrozenSky;
using FrozenSky.Util;
using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
using FrozenSky.Multimedia.Objects;

namespace FrozenSky.Samples.VirtualTestRoom
{
    public class TestingRoomViewModel : ViewModelBase
    {
        private Scene m_scene;
        private PerspectiveCamera3D m_camera;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestingRoomViewModel"/> class.
        /// </summary>
        public TestingRoomViewModel()
        {
            m_scene = new Scene();
            m_scene.ManipulateSceneAsync(OnInitializeScene)
                .FireAndForget();
            m_camera = new PerspectiveCamera3D();

            m_camera = new PerspectiveCamera3D();
            m_camera.Position = new Vector3(0f, 2f, 0f);
            m_camera.Target = new Vector3(0f, 1.5f, 2f);
        }

        /// <summary>
        /// Called during the first update pass of the local scene object.
        /// </summary>
        /// <param name="manipulator">The object provided for scene manipulation.</param>
        private void OnInitializeScene(SceneManipulator manipulator)
        {
            Grid3DType gridTypeTopCube = new Grid3DType();
            gridTypeTopCube.GenerateGround = false;
            gridTypeTopCube.TilesX = 20;
            gridTypeTopCube.TilesZ = 20;
            gridTypeTopCube.TileWidth = 1f;
            gridTypeTopCube.GroupTileCount = 20;
            gridTypeTopCube.LineColor = Color.DarkBlue;
            var resGridTopCube = manipulator.AddGeometry(gridTypeTopCube);

            Grid3DType gridTypeFloor = new Grid3DType();
            gridTypeFloor.GenerateGround = false;
            gridTypeFloor.TilesX = 100;
            gridTypeFloor.TilesZ = 100;
            gridTypeFloor.TileWidth = 0.2f;
            gridTypeFloor.LineColor = Color.DarkBlue;
            var resGridFloor = manipulator.AddGeometry(gridTypeFloor);

            // Build floor
            {
                GenericObject objFloor = manipulator.AddGeneric(resGridFloor);
            }

            // Build top cube
            {
                GenericObject objFront = manipulator.AddGeneric(resGridTopCube);
                objFront.Position = new Vector3(0f, 10f, 10f);
                objFront.RotationEuler = new Vector3(-EngineMath.RAD_90DEG, 0f, 0f);

                GenericObject objBack = manipulator.AddGeneric(resGridTopCube);
                objBack.Position = new Vector3(0f, 10f, -10f);
                objBack.RotationEuler = new Vector3(EngineMath.RAD_90DEG, 0f, 0f);

                GenericObject objLeft = manipulator.AddGeneric(resGridTopCube);
                objLeft.Position = new Vector3(-10f, 10f, 0f);
                objLeft.RotationEuler = new Vector3(0f, 0f, -EngineMath.RAD_90DEG);
            }

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
