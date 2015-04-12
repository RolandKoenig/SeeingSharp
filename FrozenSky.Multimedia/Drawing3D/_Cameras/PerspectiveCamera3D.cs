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
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrozenSky.Multimedia.Drawing3D
{
    public class PerspectiveCamera3D : Camera3DBase
    {
        private float m_fov = (float)Math.PI / 4.0f;
        private float m_aspectRatio;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerspectiveCamera3D"/> class.
        /// </summary>
        public PerspectiveCamera3D()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerspectiveCamera3D"/> class.
        /// </summary>
        /// <param name="width">Width of the renderwindow.</param>
        /// <param name="height">Height of the renderwindow.</param>
        public PerspectiveCamera3D(int width, int height)
            : base(width, height)
        {

        }

        /// <summary>
        /// Calculates the view and projection matrix for this camera.
        /// </summary>
        /// <param name="position">The position of the camera.</param>
        /// <param name="target">The target point of the camera.</param>
        /// <param name="upVector">The current up vector.</param>
        /// <param name="zNear">Distance to the nearest rendered pixel.</param>
        /// <param name="zFar">Distance to the farest rendered pixel.</param>
        /// <param name="screenWidth">The current width of the screen in pixel.</param>
        /// <param name="screenHeight">The current height of the screen in pixel.</param>
        /// <param name="viewMatrix">The calculated view matrix.</param>
        /// <param name="projMatrix">The calculated projection matrix.</param>
        protected override void CalculateViewProjectionMatrices(
            Vector3 position, Vector3 target, Vector3 upVector, float zNear, float zFar, int screenWidth, int screenHeight,
            out Matrix viewMatrix, out Matrix projMatrix)
        {
            m_aspectRatio = (float)screenWidth / (float)screenHeight;
            viewMatrix = Matrix.LookAtLH(
                position,
                target,
                upVector);
            projMatrix = Matrix.PerspectiveFovLH(
                m_fov,
                m_aspectRatio,
                zNear, zFar);
        }

        /// <summary>
        /// Gets or sets the field of view value.
        /// </summary>
        public float FieldOfView
        {
            get { return m_fov; }
            set
            {
                m_fov = value;
                UpdateCamera();
            }
        }

        /// <summary>
        /// Gets the current aspect ratio.
        /// </summary>
        public float AspectRatio
        {
            get { return m_aspectRatio; }
        }
    }
}
