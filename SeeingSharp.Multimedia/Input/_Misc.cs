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

namespace SeeingSharp.Multimedia.Input
{
    public enum SeeingSharpInputMode
    {
        /// <summary>
        /// Free camera movement, e. g with keyboard an mouse.
        /// </summary>
        FreeCameraMovement,

        /// <summary>
        /// No default input processing at all.
        /// </summary>
        NoInput,
    }

    /// <summary>
    /// Describes the current state of the Xbox 360 Controller.
    /// This structure is used by the SharpDX.XInput.State structure when polling for
    /// changes in the state of the controller. The specific mapping of button to game
    /// function varies depending on the game type. The constant XINPUT_GAMEPAD_TRIGGER_THRESHOLD
    /// may be used as the value which bLeftTrigger and bRightTrigger must be greater
    /// than to register as pressed. This is optional, but often desirable. Xbox 360
    /// Controller buttons do not manifest crosstalk.
    /// </summary>
    [Flags]
    public enum GamepadButton : short
    {
        Y = short.MinValue,
        None = 0,
        DPadUp = 1,
        DPadDown = 2,
        DPadLeft = 4,
        DPadRight = 8,
        Start = 16,
        Back = 32,
        LeftThumb = 64,
        RightThumb = 128,
        LeftShoulder = 256,
        RightShoulder = 512,
        A = 4096,
        B = 8192,
        X = 16384
    }
}
