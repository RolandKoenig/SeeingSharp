#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Multimedia.Core;

namespace SeeingSharp.Multimedia.Input
{
    public interface IInputHandler
    {
        /// <summary>
        /// Gets a list containing all supported view types.
        /// Null means that this handler is not bound to a view.
        /// </summary>
        Type[] GetSupportedViewTypes();

        /// <summary>
        /// Gets a list containing all supported camera types.
        /// Null means that this handler is not bound to a view.
        /// </summary>
        Type[] GetSupportedCameraTypes();

        /// <summary>
        /// Gets an array containing all supported input modes.
        /// Null means that all modes are supported.
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

        /// <summary>
        /// Querries all current input states.
        /// </summary>
        IEnumerable<InputStateBase> GetInputStates();
    }
}