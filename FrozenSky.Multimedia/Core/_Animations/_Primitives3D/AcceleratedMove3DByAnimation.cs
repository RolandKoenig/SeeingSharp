#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2014 Roland König (RolandK)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion

using System;

namespace FrozenSky.Multimedia.Core
{
    public class AcceleratedMove3DByAnimation : AnimationBase
    {
        // Parameters
        private SceneSpacialObject m_targetObject;
        private Vector3 m_moveNormal;
        private float m_acceleration;               //measured in m/s²
        private float m_initialSpeed;               //measured in m/s
        private TimeSpan m_duration;

        // Runtime values
        private Vector3 m_startPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="AcceleratedMove3DByAnimation" /> class.
        /// </summary>
        public AcceleratedMove3DByAnimation(SceneSpacialObject targetObject, Vector3 moveNormal, float acceleration, float initialSpeed, TimeSpan duration)
            : base(targetObject, AnimationType.FixedTime, duration)
        {
            m_targetObject = targetObject;
            m_moveNormal = moveNormal;
            m_acceleration = acceleration;
            m_initialSpeed = initialSpeed;
            m_duration = duration;
        }

        /// <summary>
        /// Called when animation starts.
        /// </summary>
        protected override void OnStartAnimation()
        {
            m_startPosition = m_targetObject.Position;
        }

        /// <summary>
        /// Called each time the CurrentTime value gets updated.
        /// </summary>
        protected override void OnCurrentTimeUpdated(UpdateState updateState, AnimationState animationState)
        {
            float currentSeconds = (float)base.CurrentTime.TotalSeconds;

            Vector3 moveVector = CalculateMoveVector(currentSeconds);
            m_targetObject.Position = m_startPosition + moveVector;
        }

        /// <summary>
        /// Called when the FixedTime animation has finished.
        /// </summary>
        protected override void OnFixedTimeAnimationFinished()
        {
            float currentSeconds = (float)base.FixedTime.TotalSeconds;

            Vector3 moveVector = CalculateMoveVector(currentSeconds);
            m_targetObject.Position = m_startPosition + moveVector;
        }

        /// <summary>
        /// Calculates the move vector for the given count of passed seconds.
        /// </summary>
        /// <param name="passedSeconds"></param>
        private Vector3 CalculateMoveVector(float passedSeconds)
        {
            if (passedSeconds <= 0f) { return Vector3.Zero; }

            float passedDistance = 0.5f * m_acceleration * (passedSeconds * passedSeconds) +
                                   m_initialSpeed * passedSeconds;

            return m_moveNormal * passedDistance;
        }
    }
}