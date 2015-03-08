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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Multimedia.Drawing3D
{
    public interface ICamera3D
    {
        /// <summary>
        /// Sets the size of the view this camera is hosted in.
        /// </summary>
        /// <param name="width">The width in pixels.</param>
        /// <param name="height">The height in pixels.</param>
        void SetScreenSize(int width, int height);

        /// <summary>
        /// Gets the current screen size.
        /// </summary>
        Vector2 GetScreenSize();

        /// <summary>
        /// Updates the camera's transformation.
        /// </summary>
        void UpdateCamera();

        /// <summary>
        /// Gets the current position of the camera.
        /// </summary>
        Vector3 Position { get;}

        /// <summary>
        /// Gets the current view matrix.
        /// </summary>
        Matrix View { get; }

        /// <summary>
        /// Gets the current projection matrix.
        /// </summary>
        Matrix Projection { get; }

        /// <summary>
        /// Gets the current view-projection matrix.
        /// </summary>
        Matrix ViewProjection { get; }

        /// <summary>
        /// Did the state of the camera change last time?
        /// Set this flag to false to reset the value.
        /// </summary>
        bool StateChanged
        {
            get;
            set;
        }
    }
}
