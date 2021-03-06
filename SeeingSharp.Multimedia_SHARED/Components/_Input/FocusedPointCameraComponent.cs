﻿#region License information (SeeingSharp and all based games/applications)
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
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Multimedia.Components
{
    public class FocusedPointCameraComponent : FocusedCameraComponent
    {
        private Vector3 m_focusedLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="FocusedPointCameraComponent"/> class.
        /// </summary>
        public FocusedPointCameraComponent()
        {
            m_focusedLocation = Vector3.Zero;
        }

        protected override Vector3 GetFocusedLocation()
        {
            return m_focusedLocation;
        }

#if DESKTOP
        [Browsable(true)]
        [Category(Constants.DESIGNER_CATEGORY_CAMERA)]
#endif
        public float FocusedPointX
        {
            get { return m_focusedLocation.X; }
            set { m_focusedLocation.X = value; }
        }

#if DESKTOP
        [Browsable(true)]
        [Category(Constants.DESIGNER_CATEGORY_CAMERA)]
#endif
        public float FocusedPointY
        {
            get { return m_focusedLocation.Y; }
            set { m_focusedLocation.Y = value; }
        }

#if DESKTOP
        [Browsable(true)]
        [Category(Constants.DESIGNER_CATEGORY_CAMERA)]
#endif
        public float FocusedPointZ
        {
            get { return m_focusedLocation.Z; }
            set { m_focusedLocation.Z = value; }
        }
    }
}
