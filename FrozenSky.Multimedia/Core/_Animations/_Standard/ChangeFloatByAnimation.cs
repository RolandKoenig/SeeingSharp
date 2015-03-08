using System;

namespace FrozenSky.Multimedia.Core
{
    public class ChangeFloatByAnimation : AnimationBase
    {
        //Configuration members
        private Func<float> m_getValueFunc;

        private Action<float> m_setValueAction;
        private float m_increaseTotal;
        private TimeSpan m_timeSpan;

        //Members for running animation
        private float m_alreadyIncreased;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeFloatByAnimation" /> class.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="getValueFunc">The get value func.</param>
        /// <param name="setValueAction">The set value action.</param>
        /// <param name="increaseTotal">The increase total.</param>
        /// <param name="timeSpan">The timespan.</param>
        public ChangeFloatByAnimation(object targetObject, Func<float> getValueFunc, Action<float> setValueAction, float increaseTotal, TimeSpan timeSpan)
            : base(targetObject, AnimationType.FixedTime, timeSpan)
        {
            m_getValueFunc = getValueFunc;
            m_setValueAction = setValueAction;
            m_increaseTotal = increaseTotal;
            m_timeSpan = timeSpan;
        }

        /// <summary>
        /// Called when animation starts.
        /// </summary>
        protected override void OnStartAnimation()
        {
            m_alreadyIncreased = 0f;
        }

        /// <summary>
        /// Called each time the CurrentTime value gets updated.
        /// </summary>
        protected override void OnCurrentTimeUpdated(UpdateState updateState, AnimationState animationState)
        {
            float currentLocationPercent = (float)(base.CurrentTime.TotalMilliseconds / base.FixedTime.TotalMilliseconds);
            float toIncreaseTotal = m_increaseTotal * currentLocationPercent;
            float toIncrease = toIncreaseTotal - m_alreadyIncreased;

            m_setValueAction(m_getValueFunc() + toIncrease);

            m_alreadyIncreased = toIncreaseTotal;
        }

        /// <summary>
        /// Is this animation a blocking animation?
        /// If true, all following animation have to wait for finish-event.
        /// </summary>
        public override bool IsBlockingAnimation
        {
            get { return false; }
        }
    }
}