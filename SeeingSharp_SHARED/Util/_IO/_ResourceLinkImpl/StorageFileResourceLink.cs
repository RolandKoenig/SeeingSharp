#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
	Exception are projects where it is noted otherwhise.
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
            storageFile.EnsureNotNull(nameof(storageFile));

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

        public override bool Exists()
        {
            return m_storageFile != null;
        }

        /// <summary>
        /// Gets an object pointing to a file at the same location (e. g. the same directory).
        /// </summary>
        /// <param name="newFileName">The new file name for which to get the ResourceLink object.</param>
        /// <param name="subdirectories">The subdirectory path to the file (optional). This parameter may not be supported by all ResourceLink implementations!</param>
        public override ResourceLink GetForAnotherFile(string newFileName, params string[] subdirectories)
        {
            newFileName.EnsureNotNullOrEmptyOrWhiteSpace(nameof(newFileName));

            if(m_storageParentFolder != null)
            {
                StorageFolder actStorageFolder = m_storageParentFolder;
                for(int loop=0; loop<subdirectories.Length; loop++)
                {
                    actStorageFolder = actStorageFolder.GetFolderAsync(subdirectories[loop]).AsTask().Result;
                }

                return new StorageFileResourceLink(
                    actStorageFolder.GetFileAsync(newFileName).AsTask().Result,
                    actStorageFolder);
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
            // Bad construct to map asynchronous API to synchronous OpenInputStream function
            // .. but it works for now and don't create a Deadlock on the UI
            return Task.Factory.StartNew(async () =>
            {
                return await this.OpenInputStreamAsync().ConfigureAwait(false);
            }).Result.Result;
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

        /// <summary>
        /// Are async operations supported on this ResourceLink?
        /// </summary>
        public override bool SupportsAsync
        {
            get { return true; }
        }

        /// <summary>
        /// Are synchronous operations supported on this ResourceLink?
        /// </summary>
        public override bool SupportsSync
        {
            get { return false; }
        }
    }
}
#endif