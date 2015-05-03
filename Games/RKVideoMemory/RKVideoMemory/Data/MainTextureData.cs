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

using SeeingSharp;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKVideoMemory.Data
{
    public class MainTextureData
    {
        internal MainTextureData(string directoryPath)
        {
            string pathBackground = Directory.GetFiles(directoryPath, Constants.FILE_PATTERN_BACKGROUND).FirstOrDefault();
            string pathTile1 = Directory.GetFiles(directoryPath, Constants.FILE_PATTERN_TILE1).FirstOrDefault();
            string pathTile2 = Directory.GetFiles(directoryPath, Constants.FILE_PATTERN_TILE2).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(pathBackground)) { throw new SeeingSharpException("Unable to find file " + Constants.FILE_PATTERN_BACKGROUND + "!"); }
            if (string.IsNullOrWhiteSpace(pathTile1)) { throw new SeeingSharpException("Unable to find file " + Constants.FILE_PATTERN_TILE1 + "!"); }
            if (string.IsNullOrWhiteSpace(pathTile2)) { throw new SeeingSharpException("Unable to find file " + Constants.FILE_PATTERN_TILE2 + "!"); }

            this.BackgroundTextureLink = pathBackground;
            this.Tile1TextureLink = pathTile1;
            this.Tile2TextureLink = pathTile2;
        }

        public ResourceLink BackgroundTextureLink
        {
            get;
            private set;
        }

        public ResourceLink Tile1TextureLink
        {
            get;
            private set;
        }

        public ResourceLink Tile2TextureLink
        {
            get;
            private set;
        }
    }
}
