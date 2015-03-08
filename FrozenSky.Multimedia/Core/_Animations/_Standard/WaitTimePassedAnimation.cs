using System;

namespace FrozenSky.Multimedia.Core
{
    public class WaitTimePassedAnimation : AnimationBase
    {
        private TimeSpan m_timeToWait;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitFinishedAnimation" /> class.
        /// </summary>
        public WaitTimePassedAnimation(TimeSpan timeToWait)
            : base(null, AnimationType.FixedTime, timeToWait)
        {
            m_timeToWait = timeToWait;
        }

        /// <summary>
        /// Is this animation a blocking animation?
        /// If true, all following animation have to wait for finish-event.
        /// </summary>
        public override bool IsBlockingAnimation
        {
            get { return true; }
        }
    }
}