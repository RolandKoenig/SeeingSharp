#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
	Exception are projects where it is noted otherwhise.
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
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Multimedia.Core
{
    /// <summary>
    /// Helper interface for accessing transformation values from plc emulation
    /// </summary>
    public interface IEngineHostedSceneObject
    {
        /// <summary>
        /// Gets the current position of the object.
        /// </summary>
        Vector3 Position
        {
            get;
        }

        /// <summary>
        /// Gets the current rotation of the object.
        /// </summary>
        Vector3 RotationEuler
        {
            get;
        }

        /// <summary>
        /// Gets the current rotation of the parent.
        /// (only relevant when HostMode is set to ChildRotationMode).
        /// </summary>
        Vector3 ParentRotation
        {
            get;
        }

        /// <summary>
        /// Gets the current scaling of the object.
        /// </summary>
        Vector3 Scaling
        {
            get;
        }

        /// <summary>
        /// Gets the assoziated AnimationHandler object (if any).
        /// </summary>
        AnimationHandler AnimationHandler
        {
            get;
        }

        /// <summary>
        /// Gets the display color of the object.
        /// </summary>
        Color4 DisplayColor
        {
            get;
        }

        /// <summary>
        /// Gets the current visibility state of the object.
        /// </summary>
        bool IsVisibleGlobal
        {
            get;
        }
    }
}
