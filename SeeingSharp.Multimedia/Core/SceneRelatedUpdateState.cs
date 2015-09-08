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
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Checking;
using SeeingSharp.Multimedia.Input;

namespace SeeingSharp.Multimedia.Core
{
    /// <summary>
    /// A UpdateState object which holds special variables for a Scene.
    /// </summary>
    public class SceneRelatedUpdateState : IAnimationUpdateState
    {
        #region members
        private UpdateState m_updateState;
        private Matrix4Stack m_world;
        private SceneLayer m_sceneLayer;
        private List<InputStateBase> m_inputStates;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneRelatedUpdateState"/> class.
        /// </summary>
        internal SceneRelatedUpdateState()
        {
            m_world = new Matrix4Stack(Matrix4x4.Identity);
            m_inputStates = new List<InputStateBase>(16);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneRelatedUpdateState"/> class.
        /// </summary>
        public SceneRelatedUpdateState(UpdateState updateState)
            : this()
        {
            m_updateState = updateState;
        }

        /// <summary>
        /// Called just before the update pass of a scene object starts.
        /// </summary>
        /// <param name="updateState">The update state.</param>
        /// <param name="unfilteredInputStates">A list containing all queried input states (still not filtered by scene!)</param>
        internal void OnStartSceneUpdate(UpdateState updateState, List<InputStateBase> unfilteredInputStates)
        {
            m_world.ResetStackToIdentity();

            m_updateState = updateState;
            m_sceneLayer = null;

            // Update input states
            m_inputStates.Clear();
        }

        /// <summary>
        /// Gets current update time.
        /// </summary>
        public TimeSpan UpdateTime
        {
            get
            {
                m_updateState.EnsureNotNull("m_updateState");

                return m_updateState.UpdateTime;
            }
        }

        /// <summary>
        /// Gets the current update time in milliseconds.
        /// </summary>
        public int UpdateTimeMilliseconds
        {
            get
            {
                m_updateState.EnsureNotNull("m_updateState");

                return m_updateState.UpdateTimeMilliseconds;
            }
        }

        public Matrix4Stack World
        {
            get { return m_world; }
        }

        public SceneLayer SceneLayer
        {
            get { return m_sceneLayer; }
            internal set { m_sceneLayer = value; }
        }

        public Scene Scene
        {
            get
            {
                if (m_sceneLayer == null) { return null; }
                else { return m_sceneLayer.Scene; }
            }
        }

        internal bool ForceTransformUpdatesOnChilds;
    }
}
