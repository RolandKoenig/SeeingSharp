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

using FrozenSky.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(typeof(FrozenSky.Translatables))]

namespace FrozenSky
{
    [TranslateableClass("FrozenSky")]
    internal static class Translatables
    {
        public static string BOOTSTRAP_TRANSLATION = "Loading Translation..";

        public static string LOADERROR_NO_HARDWARE_DEVICE_AVAILABLE = "No hardware graphics device available!";

        public static string ERROR_ROOT = "Root";
    }
}
