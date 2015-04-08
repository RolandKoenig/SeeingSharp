using FrozenSky.Infrastructure;
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.RKKinectLounge.Modules.Kinect
{
    public class KinectThread : ObjectThread
    {
        private KinectHandler m_kinectHandler;
        private PerformanceAnalyzer m_performanceAnalyzer;

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectThread"/> class.
        /// </summary>
        internal KinectThread(PerformanceAnalyzer performanceAnalyzer)
            : base(
                name: Constants.KINECT_THREAD_NAME, 
                heartBeat: 500, 
                createMessenger: true)
        {
            m_performanceAnalyzer = performanceAnalyzer;
        }

        /// <summary>
        /// Called when the thread is starting.
        /// Initialize kinect logic here.. so all frames arive on this thread.
        /// </summary>
        protected override void OnStarting(EventArgs eArgs)
        {
            base.OnStarting(eArgs);

            // Initialize Kinect handling (all done in constructor)
            m_kinectHandler = new KinectHandler(base.Messenger, m_performanceAnalyzer);
        }

        /// <summary>
        /// Called on each tick of this thread.
        /// </summary>
        protected override void OnTick(EventArgs eArgs)
        {
            base.OnTick(eArgs);
        }

        protected override void OnStopping(EventArgs eArgs)
        {
            base.OnStopping(eArgs);

            // TODO: Dispose all kinect handling objects here..
        }

    }
}
