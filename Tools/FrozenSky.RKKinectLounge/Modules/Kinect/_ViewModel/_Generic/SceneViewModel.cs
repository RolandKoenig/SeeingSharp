using FrozenSky.Multimedia.Core;
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.RKKinectLounge.Modules.Kinect
{
    public class SceneViewModel : ViewModelBase
    {
        public SceneViewModel(Scene scene)
        {
            this.Scene = scene;
        }

        public Scene Scene
        {
            get;
            private set;
        }
    }
}
