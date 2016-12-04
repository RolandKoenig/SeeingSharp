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
using SeeingSharp.Infrastructure;

// Namespace mappings
using XI = SharpDX.XInput;
using SeeingSharp.Multimedia.Core;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.Multimedia.Input.GenericXInputHandler),
    contractType: typeof(SeeingSharp.Multimedia.Input.IInputHandler))]

namespace SeeingSharp.Multimedia.Input
{
    internal class GenericXInputHandler : IInputHandler
    {
        #region Resources
        private XI.Controller[] m_controllers;
        private GamepadState[] m_states;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericXInputHandler"/> class.
        /// </summary>
        public GenericXInputHandler()
        {
            m_controllers = new XI.Controller[4];
            m_controllers[0] = new XI.Controller(XI.UserIndex.One);
            m_controllers[1] = new XI.Controller(XI.UserIndex.Two);
            m_controllers[2] = new XI.Controller(XI.UserIndex.Three);
            m_controllers[3] = new XI.Controller(XI.UserIndex.Four);

            m_states = new GamepadState[m_controllers.Length];
            for(int loop=0; loop<m_controllers.Length; loop++)
            {
                m_states[loop] = new GamepadState(loop);
            }
        }

        /// <summary>
        /// Gets a list containing all supported view types.
        /// Null means that this handler is not bound to a view.
        /// </summary>
        public Type[] GetSupportedViewTypes()
        {
            return null;
        }

        /// <summary>
        /// Starts input handling.
        /// </summary>
        /// <param name="viewObject">The view object (e. g. Direct3D11Canvas).</param>
        public void Start(IInputEnabledView viewObject)
        {

        }

        public void Stop()
        {

        }

        /// <summary>
        /// Querries all current input states.
        /// </summary>
        public IEnumerable<InputStateBase> GetInputStates()
        {
            // Update connected states first
            for (int loop = 0; loop < m_controllers.Length; loop++)
            {
                bool isConnected = m_controllers[loop].IsConnected;

                if (!isConnected)
                {
                    m_states[loop].NotifyConnected(false);
                    continue;
                }

                // Query all state values
                m_states[loop].NotifyConnected(true);
                m_states[loop].NotifyState(m_controllers[loop]);
            }

            // Now return all input states
            for (int loop=0; loop<m_states.Length; loop++)
            {
                yield return m_states[loop];
            }
        }
    }
}