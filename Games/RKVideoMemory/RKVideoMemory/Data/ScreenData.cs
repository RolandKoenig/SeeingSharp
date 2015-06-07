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

namespace RKVideoMemory.Data
{
    public class ScreenData
    {
        public ScreenData(string directoryName)
        {
            // Search and process all memory pairs
            this.MemoryPairs = new List<CardPairData>();
            foreach (string actSubdirectory in Directory.GetDirectories(directoryName))
            {
                CardPairData actMemoryPair = new CardPairData(
                    Path.GetFileName(actSubdirectory));

                // Select all file names within the directory (order by filename)
                IEnumerable<string> fileNames =
                    from actFile in Directory.GetFiles(actSubdirectory)
                    orderby Path.GetFileName(actFile)
                    select actFile;
                foreach (string actFilePath in fileNames)
                {
                    string actFileExtension = Path.GetExtension(actFilePath);

                    // Handle image files
                    if (Constants.SUPPORTED_IMAGE_FORMATS.ContainsString(actFileExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        actMemoryPair.ProcessImageFile(actFilePath);
                        continue;
                    }

                    // Handle video files
                    if (Constants.SUPPORTED_VIDEO_FORMATS.ContainsString(actFileExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        actMemoryPair.ProcessVideoFile(actFilePath);
                        continue;
                    }
                }

                // Add the pair to the new object
                if (actMemoryPair.IsValidPair())
                {
                    this.MemoryPairs.Add(actMemoryPair);
                }
            }
        }

        /// <summary>
        /// Gets a collection containing all found MemoryPairs.
        /// </summary>
        public List<CardPairData> MemoryPairs
        {
            get;
            private set;
        }
    }
}
