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
namespace SeeingSharp.Multimedia.Core
{
    public class PickingInformation
    {
        private SceneObject m_pickedObject;
        private float m_distance;

        /// <summary>
        /// Initializes a new instance of the <see cref="PickingInformation" /> class.
        /// </summary>
        public PickingInformation()
        {
            m_pickedObject = null;
            m_distance = float.NaN;
        }

        /// <summary>
        /// Notifies a pick for the given object with the given distance.
        /// </summary>
        /// <param name="pickedObject">The object that was picked.</param>
        /// <param name="distance">The distance from the origin to the picked point.</param>
        public void NotifyPick(SceneObject pickedObject, float distance)
        {
            if ((float.IsNaN(m_distance)) ||
                (distance < m_distance))
            {
                m_distance = distance;
                m_pickedObject = pickedObject;
            }
        }

        /// <summary>
        /// The picked object.
        /// </summary>
        public SceneObject PickedObject
        {
            get { return m_pickedObject; }
        }


        /// <summary>
        /// Gets the distance to the picked object.
        /// </summary>
        public float Distance
        {
            get { return m_distance; }
        }
    }
}
