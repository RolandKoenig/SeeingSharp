using FrozenSky.Util;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.RKKinectLounge.Modules.Kinect
{
    [MessagePossibleSource(Constants.KINECT_THREAD_NAME)]
    public class MessageDepthFrameArrived : FrozenSkyMessage
    {
        internal MessageDepthFrameArrived(DepthFrameArrivedEventArgs depth)
        {
            this.DepthFrameArgs = depth;
        }

        public DepthFrameArrivedEventArgs DepthFrameArgs
        {
            get;
            private set;
        }
    }
}
