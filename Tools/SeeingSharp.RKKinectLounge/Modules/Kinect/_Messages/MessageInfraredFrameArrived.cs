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
    public class MessageInfraredFrameArrived : SeeingSharpMessage
    {
        internal MessageInfraredFrameArrived(InfraredFrameArrivedEventArgs infraredFrame)
        {
            this.InfraredFrameArgs = infraredFrame;
        }

        public InfraredFrameArrivedEventArgs InfraredFrameArgs
        {
            get;
            private set;
        }
    }
}
