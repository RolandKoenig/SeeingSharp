using System;

namespace FrozenSky.Multimedia.Core
{
    public class RotateQuaternionToAnimation : AnimationBase
    {
        private IAnimatableObjectQuaternion m_targetObject;
        private Quaternion m_startQuaternion;
        private Quaternion m_targetQuaternion;

        /// <summary>
        /// Initializes a new instance of the <see cref="RotateQuaternionToAnimation" /> class.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="targetQuaternion">The target quaternion.</param>
        /// <param name="timeSpan">The time span.</param>
        public RotateQuaternionToAnimation(IAnimatableObjectQuaternion targetObject, Quaternion targetQuaternion, TimeSpan timeSpan)
            : base(targetObject, AnimationType.FixedTime, timeSpan)
        {
            m_targetObject = targetObject;
            m_targetQuaternion = targetQuaternion;
        }

        /// <summary>
        /// Called when animation starts.
        /// </summary>
        protected override void OnStartAnimation()
        {
            m_startQuaternion = m_targetObject.RotationQuaternion;
        }

        /// <summary>
        /// Called each time the CurrentTime value gets updated.
        /// </summary>
        /// <param name="updateState"></param>
        /// <param name="animationState"></param>
        protected override void OnCurrentTimeUpdated(UpdateState updateState, AnimationState animationState)
        {
            //how does Slerp work: --> http://en.wikipedia.org/wiki/Slerp
            float changeFactor = (float)base.CurrentTime.Ticks / (float)base.FixedTime.Ticks;

            Quaternion slerpQuaternion;
            Quaternion.Slerp(ref m_startQuaternion, ref m_targetQuaternion, changeFactor, out slerpQuaternion);
            m_targetObject.RotationQuaternion = slerpQuaternion;
        }

        /// <summary>
        /// Called when the FixedTime animation has finished.
        /// (Sets final state to the target object and clears all runtime values).
        /// </summary>
        protected override void OnFixedTimeAnimationFinished()
        {
            m_targetObject.RotationQuaternion = m_targetQuaternion;
        }
    }
}