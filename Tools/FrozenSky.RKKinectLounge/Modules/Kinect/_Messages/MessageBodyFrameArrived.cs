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
    public class MessageBodyFrameArrived : FrozenSkyMessage
    {
        internal MessageBodyFrameArrived(BodyFrameArrivedEventArgs bodyFrameArgs)
        {
            this.BodyFrameArgs = bodyFrameArgs;
        }

        public BodyFrameArrivedEventArgs BodyFrameArgs
        {
            get;
            private set;
        }
    }
}
