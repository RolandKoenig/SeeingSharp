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
using System.Text;
using System.Threading.Tasks;
using Windows.Gaming.Input;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Infrastructure;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.Multimedia.Input.GenericGamepadHandler),
    contractType: typeof(SeeingSharp.Multimedia.Input.IInputHandler))]

namespace SeeingSharp.Multimedia.Input
{
    internal class GenericGamepadHandler : IInputHandler
    {
        #region Constants
        private const int MAX_GAMEPAD_COUNT = 4;
        #endregion

        #region Resources
        private Gamepad[] m_gamepads;
        private GamepadState[] m_states;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericGamepadHandler"/> class.
        /// </summary>
        public GenericGamepadHandler()
        {
            m_gamepads = new Gamepad[MAX_GAMEPAD_COUNT];

            m_states = new GamepadState[m_gamepads.Length];
            for (int loop = 0; loop < m_gamepads.Length; loop++)
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

        public void Start(IInputEnabledView viewObject)
        {
            IReadOnlyList<Gamepad> gamepads = Gamepad.Gamepads;
            for(int loop=0; loop<MAX_GAMEPAD_COUNT; loop++)
            {
                if(gamepads.Count >= loop) { break; }
                m_gamepads[loop] = gamepads[loop];
            }

            Gamepad.GamepadAdded += OnGamepad_GamepadAdded;
            Gamepad.GamepadRemoved += OnGamepad_GamepadRemoved;
        }

        public void Stop()
        {
            Gamepad.GamepadAdded -= OnGamepad_GamepadAdded;
            Gamepad.GamepadRemoved -= OnGamepad_GamepadRemoved;

            for(int loop=0; loop<MAX_GAMEPAD_COUNT; loop++)
            {
                m_gamepads[loop] = null;
            }
        }

        /// <summary>
        /// Querries all current input states.
        /// </summary>
        public IEnumerable<InputStateBase> GetInputStates()
        {
            for(int loop=0; loop<MAX_GAMEPAD_COUNT; loop++)
            {
                Gamepad actGamepad = m_gamepads[loop];
                bool isConnected = actGamepad != null;

                // Handle connected state
                if(!isConnected)
                {
                    m_states[loop].NotifyConnected(false);
                    continue;
                }
                m_states[loop].NotifyConnected(true);

                GamepadReading gpReading = actGamepad.GetCurrentReading();
                m_states[loop].NotifyState(new GamepadReportedState()
                {
                    Buttons = (GamepadButton)gpReading.Buttons,
                    LeftThumbstickX = (float)gpReading.LeftThumbstickX,
                    LeftThumbstickY = (float)gpReading.LeftThumbstickY,
                    LeftTrigger = (float)gpReading.LeftTrigger,
                    RightThumbstickX = (float)gpReading.RightThumbstickX,
                    RightThumbstickY = (float)gpReading.RightThumbstickY,
                    RightTrigger = (float)gpReading.RightTrigger
                });
            }

            // Now return all input states
            for (int loop = 0; loop < m_states.Length; loop++)
            {
                yield return m_states[loop];
            }
        }

        private void OnGamepad_GamepadRemoved(object sender, Gamepad e)
        {
            for (int loop = 0; loop < MAX_GAMEPAD_COUNT; loop++)
            {
                if (m_gamepads[loop] == e)
                {
                    m_gamepads[loop] = null;
                    return;
                }
            }
        }

        private void OnGamepad_GamepadAdded(object sender, Gamepad e)
        {
            for (int loop = 0; loop < MAX_GAMEPAD_COUNT; loop++)
            {
                if (m_gamepads[loop] == null)
                {
                    m_gamepads[loop] = e;
                    return;
                }
            }
        }
    }
}