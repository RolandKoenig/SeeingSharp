﻿#region License information (SeeingSharp and all based games/applications)
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

using SeeingSharp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(typeof(RK2048.Translatables))]

namespace RK2048
{
    [TranslateableClass("RK2048")]
    internal static class Translatables
    {
        public static string MAIN_PAGE_TITLE = "RK 2048";
        public static string MAIN_PAGE_SUBTITLE = "Join the numbers and get to the 2048 tile!";

        public static string SCORE_MAXIMUM = "Maximum";
        public static string SCORE_CURRENT = "Current";

        public static string GAME_RESTART = "Restart";

        public static string MENU_RESTART = "Restart";
        public static string MENU_WEB = "Web";
        public static string MENU_HELP = "Help";
        public static string MENU_CONFIG = "Configuration";

        public static string TITLE_VERSION = "{0} (Version {1})";

        public static string RESTART_RENDERING = "Restart Rendering";

    }
}