using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrozenSky.Multimedia.Objects;

namespace FrozenSky.Multimedia.Core
{
    public class ChangeOpacityToAnimation : AnimationBase
    {
        private float m_startOpacity;
        private TimeSpan m_duration;
        private float m_moveOpacity;
        private float m_targetOpacity;
        private GenericObject m_targetObject;

        /// <summary>
        /// Initialize a new Instance of the <see cref="Move3DByAnimation" /> class.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="targetOpacity">The target opacity.</param>
        /// <param name="duration">The duration.</param>
        /// <exception cref="System.Exception">Opacity value can be between 0 and 1, not greater than 1 and not lower than 0!</exception>
        public ChangeOpacityToAnimation(GenericObject targetObject, float targetOpacity, TimeSpan duration)
            : base(targetObject, AnimationType.FixedTime, duration)
        {
            m_targetObject = targetObject;
            m_duration = duration;
            m_targetOpacity = targetOpacity;

            if (targetOpacity < 0f || targetOpacity > 1f)
            {
                throw new Exception("Opacity value can be between 0 and 1, not greater than 1 and not lower than 0!");
            }
        }

        /// <summary>
        /// Called when animation starts.
        /// </summary>
        protected override void OnStartAnimation()
        {
            m_startOpacity = m_targetObject.Opacity;
            m_moveOpacity = m_targetOpacity - m_startOpacity;
        }

        /// <summary>
        /// Called each time the CurrentTime value gets updated.
        /// </summary>
        protected override void OnCurrentTimeUpdated(UpdateState updateState, AnimationState animationState)
        {
            float changeFactor = (float)base.CurrentTime.Ticks / (float)base.FixedTime.Ticks;
            m_targetObject.Opacity = m_startOpacity + m_moveOpacity * changeFactor;
        }

        /// <summary>
        /// Called when the FixedTime animation has finished.
        /// (Sets final state to the target object and clears all runtime values).
        /// </summary>
        protected override void OnFixedTimeAnimationFinished()
        {
            m_targetObject.Opacity = m_targetOpacity;

            m_moveOpacity = 0;
            m_startOpacity = 1;
        }
    }
}