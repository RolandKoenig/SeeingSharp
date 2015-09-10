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
        #region parameters
        private Scene m_owner;
        #endregion

        #region parameters for single update step
        private UpdateState m_updateState;
        private Matrix4Stack m_world;
        private SceneLayer m_sceneLayer;
        private List<InputStateBase> m_inputStates;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneRelatedUpdateState"/> class.
        /// </summary>
        internal SceneRelatedUpdateState(Scene owner)
        {
            m_owner = owner;
            m_world = new Matrix4Stack(Matrix4x4.Identity);
            m_inputStates = new List<InputStateBase>(16);
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

            // Reset input states
            m_inputStates.Clear();
            this.MouseOrPointer = MouseOrPointerState.Dummy;

            // Update input states
            if (unfilteredInputStates != null)
            {
                int inputStateCount = unfilteredInputStates.Count;
                for (int loop = 0; loop < inputStateCount; loop++)
                {
                    InputStateBase actInputState = unfilteredInputStates[loop];

                    // TODO: Move this call to another location because
                    // we have a conflict with the UI thread which may register/deregister
                    // a view
                    if (m_owner.IsViewRegistered(actInputState.RelatedView))
                    {
                        m_inputStates.Add(actInputState);

                        // Handle MouseOrPointer states
                        MouseOrPointerState mouseOrPointer = actInputState as MouseOrPointerState;
                        if (mouseOrPointer != null)
                        {
                            this.MouseOrPointer = mouseOrPointer;
                            continue;
                        }
                    }
                }
            }
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

        /// <summary>
        /// Gets a collection containing all gathered input states.
        /// </summary>
        public IEnumerable<InputStateBase> InputStates
        {
            get { return m_inputStates; }
        }

        public MouseOrPointerState MouseOrPointer
        {
            get;
            private set;
        }

        internal bool ForceTransformUpdatesOnChilds;
    }
}
