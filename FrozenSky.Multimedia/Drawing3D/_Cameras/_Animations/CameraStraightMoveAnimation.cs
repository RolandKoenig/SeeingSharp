#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2015 Roland König (RolandK)

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

	********************************************************************************
    Additional permission under GNU GPL version 3 section 7

    If you modify this Program, or any covered work, by linking or combining it with 
	DirectX (or a modified version of that library), containing parts covered by the 
	terms of [name of library's license], the licensors of this Program grant you additional 
	permission to convey the resulting work. {Corresponding Source for a non-source form of 
	such a combination shall include the source code for the parts of DirectX used 
	as well as that of the covered work.}
*/
#endregion
using FrozenSky.Multimedia.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Multimedia.Drawing3D
{
    public class CameraStraightMoveAnimation : AnimationBase
    {
        private Camera3DBase m_camera;
        private PerspectiveCamera3D m_cameraPerspective;
        private OrthographicCamera3D m_cameraOrthographic;

        private Camera3DViewPoint m_viewPointSource;
        private Camera3DViewPoint m_viewPointTarget;

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraStraightMoveAnimation"/> class.
        /// </summary>
        /// <param name="targetCamera">The camera to be animated.</param>
        /// <param name="targetViewPoint">The target view point.</param>
        /// <param name="animationTime">The animation time.</param>
        public CameraStraightMoveAnimation(Camera3DBase targetCamera, Camera3DViewPoint targetViewPoint, TimeSpan animationTime)
            : base(targetCamera, AnimationType.FixedTime, animationTime)
        {
            m_camera = targetCamera;
            m_cameraPerspective = m_camera as PerspectiveCamera3D;
            m_cameraOrthographic = m_camera as OrthographicCamera3D;

            m_viewPointSource = m_camera.GetViewPoint();
            m_viewPointTarget = targetViewPoint;
        }

        /// <summary>
        /// Called each time the CurrentTime value gets updated.
        /// </summary>
        /// <param name="updateState">The current state of update processing.</param>
        /// <param name="animationState">The current state of the animation.</param>
        protected override void OnCurrentTimeUpdated(UpdateState updateState, AnimationState animationState)
        {
            base.OnCurrentTimeUpdated(updateState, animationState);

            // Calculate factor by what to transform all coordinates
            double maxMilliseconds = base.FixedTime.TotalMilliseconds;
            double currentMillis = base.CurrentTime.TotalMilliseconds;
            float actFrameFactor = (float)(currentMillis / maxMilliseconds);

            // Transform position and rotation
            Vector3 moveVector = m_viewPointTarget.Position - m_viewPointSource.Position;
            Vector2 rotationVector = m_viewPointTarget.Rotation - m_viewPointSource.Rotation;
            m_camera.Position = m_viewPointSource.Position + (moveVector * actFrameFactor);
            m_camera.TargetRotation = m_viewPointSource.Rotation + (rotationVector * actFrameFactor);

            // Special handling for orthographics cameras
            if (m_cameraOrthographic != null)
            {
                float zoomValue = m_viewPointTarget.OrthographicZoomFactor - m_viewPointSource.OrthographicZoomFactor;
                m_cameraOrthographic.ZoomFactor = m_viewPointSource.OrthographicZoomFactor + (zoomValue * actFrameFactor);
            }
        }

        /// <summary>
        /// Called when the FixedTime animation has finished.
        /// (Sets final state to the target object and clears all runtime values).
        /// </summary>
        protected override void OnFixedTimeAnimationFinished()
        {
            base.OnFixedTimeAnimationFinished();

            m_camera.ApplyViewPoint(m_viewPointTarget);
        }
    }
}
