﻿using FrozenSky.Util;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.RKKinectLounge.Modules.Kinect
{
    [MessagePossibleSource(Constants.KINECT_THREAD_NAME)]
    public class MessageColorFrameArrived : FrozenSkyMessage
    {
        internal MessageColorFrameArrived(ColorFrameArrivedEventArgs colorFrameArgs)
        {
            this.ColorFrameArgs = colorFrameArgs;
        }

        public ColorFrameArrivedEventArgs ColorFrameArgs
        {
            get;
            private set;
        }
    }
}
