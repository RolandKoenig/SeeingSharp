using SeeingSharp.Multimedia.Core;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.RKKinectLounge.Modules.Kinect
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
