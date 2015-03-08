using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Multimedia.Core
{
    public class LazyAnimation : IAnimation
    {
        private Func<IAnimation> m_animationCreator;
        private IAnimation m_animation;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyAnimation"/> class.
        /// </summary>
        /// <param name="animationCreator">The animation creator.</param>
        public LazyAnimation(Func<IAnimation> animationCreator)
        {
            m_animationCreator = animationCreator;
            m_animation = null;
        }

        /// <summary>
        /// Checks if the given object is animated by this animation.
        /// </summary>
        /// <param name="targetObject">The object to check for.</param>
        public bool IsObjectAnimated(object targetObject)
        {
            if (m_animation == null) { m_animation = m_animationCreator(); }
            if (m_animation == null) { return false; }

            return m_animation.IsObjectAnimated(targetObject);
        }

        /// <summary>
        /// Called for each update step of this animation.
        /// </summary>
        /// <param name="updateState">The current state of the update pass.</param>
        /// <param name="animationState">The current state of the animation.</param>
        public AnimationUpdateResult Update(UpdateState updateState, AnimationState animationState)
        {
            if (m_animation == null) { m_animation = m_animationCreator(); }
            if (m_animation == null) { return AnimationUpdateResult.Empty; }

            return m_animation.Update(updateState, animationState); 
        }

        /// <summary>
        /// Resets this animation.
        /// </summary>
        public void Reset()
        {
            if (m_animation == null) { m_animation = m_animationCreator(); }
            if (m_animation == null) { return; }

            m_animation.Reset();
        }

        /// <summary>
        /// Gets the time in milliseconds till this animation is finished.
        /// This method is relevant for event-driven processing and tells the system by what amound the clock is to be increased next.
        /// </summary>
        /// <param name="previousMinFinishTime">The minimum TimeSpan previous animations take.</param>
        /// <param name="previousMaxFinishTime">The maximum TimeSpan previous animations take.</param>
        /// <param name="defaultCycleTime">The default cycle time if we would be in continous calculation mode.</param>
        public TimeSpan GetTimeTillNextEvent(TimeSpan previousMinFinishTime, TimeSpan previousMaxFinishTime, TimeSpan defaultCycleTime)
        {
            if (m_animation == null) { m_animation = m_animationCreator(); }
            if (m_animation == null) { return TimeSpan.Zero; }

            return m_animation.GetTimeTillNextEvent(previousMinFinishTime, previousMaxFinishTime, defaultCycleTime);
        }

        /// <summary>
        /// Is the animation finished?
        /// </summary>
        public bool Finished
        {
            get 
            {
                if (m_animation == null) { m_animation = m_animationCreator(); }
                if (m_animation == null) { return true; }
                return m_animation.Finished;
            }
        }

        /// <summary>
        /// Is this animation a blocking animation?
        /// </summary>
        public bool IsBlockingAnimation
        {
            get 
            {
                if (m_animation == null) { m_animation = m_animationCreator(); }
                if (m_animation == null) { return false; }
                return m_animation.IsBlockingAnimation;
            }
        }

        /// <summary>
        /// Is this animation canceled?
        /// </summary>
        public bool Canceled
        {
            get;
            set;
        }
    }
}
