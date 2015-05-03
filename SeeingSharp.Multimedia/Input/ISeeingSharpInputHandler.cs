#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
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

using SeeingSharp.Multimedia.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Multimedia.Input
{
    public interface ISeeingSharpInputHandler
    {
        /// <summary>
        /// Gets a list containing all supported view types.
        /// </summary>
        Type[] GetSupportedViewTypes();

        /// <summary>
        /// Gets a list containing all supported camera types.
        /// </summary>
        Type[] GetSupportedCameraTypes();

        /// <summary>
        /// Gets an array containing all supported input modes.
        /// </summary>
        SeeingSharpInputMode[] GetSupportedInputModes();

        /// <summary>
        /// Starts input handling.
        /// </summary>
        /// <param name="cameraObject">The camera object (e. g. PerspectiveCamera3D).</param>
        /// <param name="viewObject">The view object (e. g. Direct3D11Canvas).</param>
        void Start(object viewObject, object cameraObject);

        /// <summary>
        /// Generic method thet gets iteratively after this handler was started.
        /// </summary>
        void UpdateMovement();

        /// <summary>
        /// Stops input handling.
        /// </summary>
        void Stop();
    }
}
