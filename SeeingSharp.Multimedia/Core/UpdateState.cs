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

using System;
using SeeingSharp.Util;

namespace SeeingSharp.Multimedia.Core
{
    public class UpdateState
    {
        private int m_updateTimeMilliseconds;
        private TimeSpan m_updateTime;
        private Matrix4Stack m_world;
        private SceneLayer m_sceneLayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateState"/> class.
        /// </summary>
        /// <param name="updateTime">The update time.</param>
        public UpdateState(TimeSpan updateTime)
        {
            m_updateTime = updateTime;
            m_updateTimeMilliseconds = (int)updateTime.TotalMilliseconds;
            m_world = new Matrix4Stack(Matrix4x4.Identity);
        }

        /// <summary>
        /// Resets this UpdateState to the given update time.
        /// </summary>
        /// <param name="updateTime">The update time.</param>
        internal void Reset(TimeSpan updateTime)
        {
            m_updateTime = updateTime;
            m_updateTimeMilliseconds = (int)updateTime.TotalMilliseconds;
            m_world.ResetStackToIdentity();
        }

        /// <summary>
        /// Gets current update time.
        /// </summary>
        public TimeSpan UpdateTime
        {
            get { return m_updateTime; }
        }

        /// <summary>
        /// Gets the current update time in milliseconds.
        /// </summary>
        public int UpdateTimeMilliseconds
        {
            get { return m_updateTimeMilliseconds; }
        }

        /// <summary>
        /// Gets current world transform.
        /// </summary>
        public Matrix4Stack World
        {
            get { return m_world; }
        }

        /// <summary>
        /// The scene layer the currently updated object belongs to.
        /// </summary>
        public SceneLayer SceneLayer
        {
            get { return m_sceneLayer; }
            internal set { m_sceneLayer = value; }
        }

        /// <summary>
        /// The scene the currently updated object belongs to.
        /// </summary>
        public Scene Scene
        {
            get
            {
                if (m_sceneLayer == null) { return null; }
                else { return m_sceneLayer.Scene; }
            }
        }

        internal bool ForceTransformUpdatesOnChilds;
        internal bool IsEventDrivenUpdate;
    }
}
