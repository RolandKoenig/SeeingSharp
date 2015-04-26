using SeeingSharp.Util;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.RKKinectLounge.Modules.Kinect
{
    [MessagePossibleSource(Constants.KINECT_THREAD_NAME)]
    public class MessageDepthFrameArrived : SeeingSharpMessage
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
