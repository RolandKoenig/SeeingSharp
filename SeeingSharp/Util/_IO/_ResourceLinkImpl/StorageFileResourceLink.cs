#region License information (SeeingSharp and all based games/applications)
/*
    SeeingSharp and all games/applications based on it (more info at http://www.rolandk.de/wp)
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
#if UNIVERSAL
using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using SeeingSharp.Checking;

namespace SeeingSharp.Util
{
    public class StorageFileResourceLink : ResourceLink
    {
        private StorageFile m_storageFile;
        private StorageFolder m_storageParentFolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageFileResourceLink"/> class.
        /// </summary>
        /// <param name="storageFile">The reference to the WinRT StorageFile object.</param>
        /// <param name="parentFolder">The parent folder which contains the given file.</param>
        public StorageFileResourceLink(
            StorageFile storageFile,
            StorageFolder parentFolder = null)
        {
            storageFile.EnsureNotNull("storageFile");

            m_storageFile = storageFile;
            m_storageParentFolder = parentFolder;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="StorageFile"/> to <see cref="StorageFileResourceLink"/>.
        /// </summary>
        public static implicit operator StorageFileResourceLink(StorageFile storageFile)
        {
            return new StorageFileResourceLink(storageFile);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(m_storageFile.Path)) { return "StorageFile: " + m_storageFile.Path; }
            else { return "StoageFile: " + m_storageFile.Name; }
        }

        /// <summary>
        /// Gets an object pointing to a file at the same location (e. g. the same directory).
        /// </summary>
        /// <param name="newFileName">The new file name for which to get the ResourceLink object.</param>
        public override ResourceLink GetForAnotherFile(string newFileName)
        {
            newFileName.EnsureNotNullOrEmptyOrWhiteSpace("newFileName");

            if(m_storageParentFolder != null)
            {
                return new StorageFileResourceLink(
                    m_storageParentFolder.GetFileAsync(newFileName).AsTask().Result,
                    m_storageParentFolder);
            }
            else
            {
                throw new SeeingSharpException("Unabel to query another file in the folder because no reference to the parent folder is given!");
            }
        }

        /// <summary>
        /// Opens a stream to the resource.
        /// </summary>
        public override Stream OpenInputStream()
        {
            return m_storageFile.OpenStreamForReadAsync().Result;
        }

        /// <summary>
        /// Opens the input stream to the described resource.
        /// </summary>
        /// <returns></returns>
        public override async Task<Stream> OpenInputStreamAsync()
        {
            return await m_storageFile.OpenStreamForReadAsync();
        }

        /// <summary>
        /// Opens an output stream to the current stream source.
        /// </summary>
        public override Stream OpenOutputStream()
        {
            return m_storageFile.OpenStreamForWriteAsync().Result;
        }

        /// <summary>
        /// Gets the file extension of the resource we target to.
        /// </summary>
        public override string FileExtension
        {
            get 
            {
                if (string.IsNullOrEmpty(m_storageFile.Path)) { return string.Empty; }

                return base.GetExtensionFromFileName(m_storageFile.Path);
            }
        }
    }
}
#endif