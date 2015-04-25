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
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKVideoMemory.Data
{
    public class CardPairData 
    {
        private List<ResourceLink> m_childImageFiles;
        private List<ResourceLink> m_childVideoFiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardPairData"/> class.
        /// </summary>
        /// <param name="title">The title of this pari.</param>
        internal CardPairData(string title)
        {
            this.Title = title;

            m_childImageFiles = new List<ResourceLink>();
            m_childVideoFiles = new List<ResourceLink>();
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.Title)) { return this.Title; }
            return base.ToString();
        }

        /// <summary>
        /// Determines whether this MemoryPairData is valid for the game.
        /// </summary>
        internal bool IsValidPair()
        {
            return this.TitleFile != null;
        }

        /// <summary>
        /// Processes the given image file.
        /// </summary>
        /// <param name="filePath">The path to the image file.</param>
        internal void ProcessImageFile(string filePath)
        {
            if (Path.GetFileNameWithoutExtension(filePath).Equals(Constants.TITLE_IMAGE_NAME, StringComparison.OrdinalIgnoreCase))
            {
                // We are loading the title image
                this.TitleFile = filePath;
            }
            else
            {
                // We are loading a image for the image sequence
                m_childImageFiles.Add(filePath);
            }
        }

        /// <summary>
        /// Processes the given video file.
        /// </summary>
        /// <param name="filePath">The path to the vide file.</param>
        internal void ProcessVideoFile(string filePath)
        {
            m_childVideoFiles.Add(filePath);
        }

        public string Title
        {
            get;
            private set;
        }

        public ResourceLink TitleFile
        {
            get;
            private set;
        }

        public List<ResourceLink> ChildImages
        {
            get { return m_childImageFiles; }
        }

        public List<ResourceLink> ChildVideos
        {
            get { return m_childVideoFiles; }
        }
    }
}
