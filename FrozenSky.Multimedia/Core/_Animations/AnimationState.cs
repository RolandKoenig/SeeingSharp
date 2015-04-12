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

namespace FrozenSky.Multimedia.Core
{
    public class AnimationState
    {
        private int m_runningAnimationsIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationState" /> class.
        /// </summary>
        public AnimationState()
        {
            m_runningAnimationsIndex = 0;
        }

        /// <summary>
        /// Gets the index within the collection of running animations.
        /// </summary>
        public int RunningAnimationsIndex
        {
            get { return m_runningAnimationsIndex; }
            set { m_runningAnimationsIndex = value; }
        }
    }
}