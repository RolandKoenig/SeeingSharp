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

#if DESKTOP
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Checking;

namespace SeeingSharp.Util
{
    public class DesktopFileSystemResourceLink : ResourceLink
    {
        private string m_filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopFileSystemResourceLink" /> class.
        /// </summary>
        /// <param name="filePath">The path to the physical file.</param>
        public DesktopFileSystemResourceLink(string filePath)
        {
            filePath.EnsureNotNullOrEmptyOrWhiteSpace("filePath");

            m_filePath = filePath;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="DesktopFileSystemResourceLink"/>.
        /// </summary>
        public static implicit operator DesktopFileSystemResourceLink(string fileName)
        {
            return new DesktopFileSystemResourceLink(fileName);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        public override string ToString()
        {
            return "File-Resource: " + m_filePath;
        }

        /// <summary>
        /// Gets an object pointing to a file at the same location (e. g. the same directory).
        /// </summary>
        /// <param name="newFileName">The new file name for which to get the ResourceLink object.</param>
        public override ResourceLink GetForAnotherFile(string newFileName)
        {
            newFileName.EnsureNotNullOrEmptyOrWhiteSpace("newFileName");

            string directoryName = Path.GetDirectoryName(m_filePath);
            if (!string.IsNullOrEmpty(directoryName))
            {
                return new DesktopFileSystemResourceLink(Path.Combine(directoryName, newFileName));
            }
            else
            {
                return new DesktopFileSystemResourceLink(newFileName);
            }
        }

        /// <summary>
        /// Opens an output stream to the current stream source.
        /// </summary>
        public override Stream OpenOutputStream()
        {
            return new FileStream(m_filePath, FileMode.Create, FileAccess.Write);
        }

        /// <summary>
        /// Opens the input stream to the described resource.
        /// </summary>
        public override Task<Stream> OpenInputStreamAsync()
        {
            return Task.Factory.StartNew(() => (Stream)File.OpenRead(m_filePath));
        }

        /// <summary>
        /// Opens a stream to the resource.
        /// </summary>
        public override Stream OpenInputStream()
        {
            return File.OpenRead(m_filePath);
        }

        /// <summary>
        /// Gets the file extension of the resource we target to.
        /// </summary>
        public override string FileExtension
        {
            get { return base.GetExtensionFromFileName(m_filePath); }
        }

        /// <summary>
        /// Gets the path to the file.
        /// </summary>
        public string FilePath
        {
            get { return m_filePath; }
        }
    }
}
#endif