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
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Multimedia.Core
{
    /// <summary>
    /// A helper class which is responsible for detecting the current platform.
    /// </summary>
    public static class PlatformDetector
    {
        private static SeeingSharpPlatform s_cachedValue;

        /// <summary>
        /// Initializes the <see cref="PlatformDetector"/> class.
        /// </summary>
        static PlatformDetector()
        {
#if DESKTOP
            s_cachedValue = SeeingSharpPlatform.Desktop;
#else
            // Platform detection logic is based on the following fact:
            // The property DisplayInformation.RawPixelsPerViewPixel is only
            // available on Windows Phone. 
            // see https://msdn.microsoft.com/en-us/library/windows.graphics.display.displayinformation.rawpixelsperviewpixel.aspx
            TypeInfo displayInfoType = typeof(Windows.Graphics.Display.DisplayInformation).GetTypeInfo();
            if(displayInfoType.GetDeclaredProperty("RawPixelsPerViewPixel") != null)
            {
                s_cachedValue = SeeingSharpPlatform.WindowsPhone;
            }
            else
            {
                s_cachedValue = SeeingSharpPlatform.ModernPCOrTabletApp;
            }
#endif
        }

        /// <summary>
        /// Checks on which platform we are running currently.
        /// </summary>
        public static SeeingSharpPlatform DetectPlatform()
        {
            return s_cachedValue;
        }
    }
}
