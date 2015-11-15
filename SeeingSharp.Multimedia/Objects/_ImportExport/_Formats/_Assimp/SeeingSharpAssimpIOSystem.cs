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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Util;

#if DESKTOP
namespace SeeingSharp.Multimedia.Objects
{
    internal class SeeingSharpAssimpIOSystem : Assimp.IOSystem
    {
        private ResourceLink m_originalSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeeingSharpAssimpIOSystem"/> class.
        /// </summary>
        /// <param name="originalSource">The original source.</param>
        public SeeingSharpAssimpIOSystem(ResourceLink originalSource)
        {
            m_originalSource = originalSource;
        }

        public override Assimp.IOStream OpenFile(string pathToFile, Assimp.FileIOMode fileMode)
        {
            throw new NotImplementedException();
        }
    }
}
#endif