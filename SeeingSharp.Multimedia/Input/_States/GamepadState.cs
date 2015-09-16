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
using System.Text;
using System.Threading.Tasks;

// Namespace mappings
using XI = SharpDX.XInput;

namespace SeeingSharp.Multimedia.Input
{
    public class GamepadState : InputStateBase
    {
        public static readonly GamepadState Dummy = new GamepadState(0);

        #region State variables
        private int m_controllerIndex;
        private XI.State m_prevState;
        private XI.State m_currentState;
        private bool m_isConnected;
        #endregion

        /// <summary>
        /// Prevents a default instance of the <see cref="GamepadState"/> class from being created.
        /// </summary>
        private GamepadState()
        {
            m_prevState = new XI.State();
            m_currentState = new XI.State();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GamepadState"/> class.
        /// </summary>
        internal GamepadState(int controllerIndex)
            : this()
        {
            m_controllerIndex = controllerIndex;
        }

        internal void NotifyConnected(bool isConnected)
        {
            m_isConnected = isConnected;
        }

        internal void NotifyState(XI.Controller controller)
        {
            m_prevState = m_currentState;
            controller.GetState(out m_currentState);
        }

        /// <summary>
        /// Copies this object and then resets it
        /// in preparation of the next update pass.
        /// Called by update-render loop.
        /// </summary>
        protected override InputStateBase CopyAndResetForUpdatePassInternal()
        {
            GamepadState result = new GamepadState();
            result.m_controllerIndex = this.m_controllerIndex;
            result.m_isConnected = this.m_isConnected;
            result.m_prevState = this.m_prevState;
            result.m_currentState = this.m_currentState;
            return result;
        }

        /// <summary>
        /// Is the given button down currently?
        /// </summary>
        /// <param name="button">The button to be checked.</param>
        public bool IsButtonDown(GamepadButton button)
        {
            if (!m_isConnected) { return false; }

            return ((short)m_currentState.Gamepad.Buttons & (short)button) == (short)button;
        }

        /// <summary>
        /// Is the given button hit exactly this frame?
        /// </summary>
        /// <param name="button">The button to be checked.</param>
        public bool IsButtonHit(GamepadButton button)
        {
            if (!m_isConnected) { return false; }

            bool prevDown = ((short)m_prevState.Gamepad.Buttons & (short)button) == (short)button;
            bool currentDown = ((short)m_currentState.Gamepad.Buttons & (short)button) == (short)button;

            
            return (!prevDown) && currentDown;
        }

        /// <summary>
        /// Do we have any controller connected on this point.
        /// </summary>
        public bool IsConnected
        {
            get { return m_isConnected; }
        }

        /// <summary>
        /// Gets the currently pressed buttons.
        /// </summary>
        public GamepadButton PressedButtons
        {
            get
            {
                if (!m_isConnected) { return GamepadButton.None; }
                return (GamepadButton)m_currentState.Gamepad.Buttons;
            }
        }

        /// <summary>
        /// Value from short.MinValue to short.MaxValue.
        /// </summary>
        public short LeftThumbX
        {
            get { return m_currentState.Gamepad.LeftThumbX; }
        }

        /// <summary>
        /// Value from short.MinValue to short.MaxValue.
        /// </summary>
        public short LeftThumbY
        {
            get { return m_currentState.Gamepad.LeftThumbY; }
        }

        public byte LeftTrigger
        {
            get { return m_currentState.Gamepad.LeftTrigger; }
        }

        /// <summary>
        /// Value from short.MinValue to short.MaxValue.
        /// </summary>
        public short RightThumbX
        {
            get { return m_currentState.Gamepad.RightThumbX; }
        }

        /// <summary>
        /// Value from short.MinValue to short.MaxValue.
        /// </summary>
        public short RightThumbY
        {
            get { return m_currentState.Gamepad.RightThumbY; }
        }

        public byte RightTrigger
        {
            get { return m_currentState.Gamepad.RightTrigger; }
        }
    }
}
