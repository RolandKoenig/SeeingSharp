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
using SeeingSharp.Multimedia.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Multimedia.Core
{
    public class GenericInputEventArgs : EventArgs
    {
        private List<InputStateBase> m_inputStates;
        private bool m_anyRelevantState;

        internal GenericInputEventArgs(List<InputStateBase> unfilteredInputStates)
        {
            // Reset input states
            m_inputStates = new List<InputStateBase>(unfilteredInputStates.Count);
            this.DefaultGamepad = GamepadState.Dummy;

            // Update input states
            if (unfilteredInputStates != null)
            {
                int inputStateCount = unfilteredInputStates.Count;
                for (int loop = 0; loop < inputStateCount; loop++)
                {
                    InputStateBase actInputState = unfilteredInputStates[loop];
                    if (actInputState.RelatedView != null) { continue; }

                    m_inputStates.Add(actInputState);

                    // Register first Gamepad state as default
                    if (this.DefaultGamepad == GamepadState.Dummy)
                    {
                        GamepadState gamepadState = actInputState as GamepadState;
                        if (gamepadState != null)
                        {
                            m_anyRelevantState = true;
                            this.DefaultGamepad = gamepadState;
                            continue;
                        }
                    }
                }
            }
        }

        public bool AnyRelevantState
        {
            get { return m_anyRelevantState; }
        }

        /// <summary>
        /// Gets the current default gamepad state.
        /// </summary>
        public GamepadState DefaultGamepad
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a list containing all generic input states.
        /// </summary>
        public List<InputStateBase> InputStates
        {
            get { return m_inputStates; }
        }
    }
}
