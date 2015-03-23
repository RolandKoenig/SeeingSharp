using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKVideoMemory.Data
{
    public class VideoMemoryLevel
    {
        private LevelData m_levelData;

        private Scene m_scene;
        private PerspectiveCamera3D m_camera;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoMemoryLevel"/> class.
        /// </summary>
        /// <param name="directoryName">Name of the directory.</param>
        private VideoMemoryLevel(string directoryName)
        {
            m_scene = new Scene();
            m_camera = new PerspectiveCamera3D();

            // Trigger first initialization of the scene
            m_scene.ManipulateSceneAsync(OnScene_Initialize);

            // Preload all level data
            m_levelData = new LevelData(directoryName);
        }

        /// <summary>
        /// Froms a new VideoMemoryLevel from the given directory.
        /// </summary>
        /// <param name="directoryName">Name of the directory.</param>
        public static VideoMemoryLevel FromDirectory(string directoryName)
        {
            return new VideoMemoryLevel(directoryName);
        }

        /// <summary>
        /// First first manipulate call of the local scene object.
        /// </summary>
        /// <param name="manipulator">The accessor for manipulation functions.</param>
        private void OnScene_Initialize(SceneManipulator manipulator)
        {
            
        }
    }
}
