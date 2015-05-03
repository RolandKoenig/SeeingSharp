﻿#region License information (SeeingSharp and all based games/applications)
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
using System.Text;

namespace RKVideoMemory.Util
{
    internal static class ThreadSafeRandom
    {
        private static Random s_random;
        private static object s_randomLock;

        static ThreadSafeRandom()
        {
            s_random = new Random(Environment.TickCount);
            s_randomLock = new object();
        }

        public static int Next(int min, int max)
        {
            lock(s_randomLock)
            {
                return s_random.Next(min, max);
            }
        }
        
    }
}
