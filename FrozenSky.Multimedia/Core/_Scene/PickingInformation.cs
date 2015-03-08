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

namespace FrozenSky.Multimedia.Core
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
