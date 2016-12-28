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
        #region State
        private string m_filePath;
        private string m_folderPath;
        #endregion

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

            m_filePath = storageFile.Path;
            if (parentFolder != null)
            {
                m_folderPath = parentFolder.Path;
            }
            else
            {
                m_folderPath = Path.GetDirectoryName(m_filePath);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageFileResourceLink"/> class.
        /// </summary>
        /// <param name="filePath">The full path to the file.</param>
        public StorageFileResourceLink(string filePath)
        {
            filePath.EnsureNotNullOrEmptyOrWhiteSpace(filePath);

            m_filePath = filePath;
            m_folderPath = Path.GetDirectoryName(m_filePath);
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
            return $"StorageFile: {m_filePath}, StorageFolder: {m_folderPath}";
        }

        public override bool Exists()
        {
            return File.Exists(m_filePath);
        }

        /// <summary>
        /// Gets an object pointing to a file at the same location (e. g. the same directory).
        /// </summary>
        /// <param name="newFileName">The new file name for which to get the ResourceLink object.</param>
        /// <param name="subdirectories">The subdirectory path to the file (optional). This parameter may not be supported by all ResourceLink implementations!</param>
        public override ResourceLink GetForAnotherFile(string newFileName, params string[] subdirectories)
        {
            newFileName.EnsureNotNullOrEmptyOrWhiteSpace(nameof(newFileName));

            if(!string.IsNullOrEmpty(m_folderPath))
            {
                string targetFolder = m_folderPath;
                for(int loop=0; loop<subdirectories.Length; loop++)
                {
                    if (string.IsNullOrEmpty(subdirectories[loop])) { continue; }

                    targetFolder = Path.Combine(targetFolder, subdirectories[loop]);
                }
                string targetFile = Path.Combine(targetFolder, newFileName);

                return new StorageFileResourceLink(
                    StorageFile.GetFileFromPathAsync(targetFile).GetResults(),
                    StorageFolder.GetFolderFromPathAsync(targetFolder).GetResults());
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
            return File.OpenRead(m_filePath);
        }

        /// <summary>
        /// Opens the input stream to the described resource.
        /// </summary>
        /// <returns></returns>
        public override async Task<Stream> OpenInputStreamAsync()
        {
            StorageFile storageFile = await StorageFile.GetFileFromPathAsync(m_filePath)
                .AsTask().ConfigureAwait(false);

            return await storageFile.OpenStreamForReadAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Opens an output stream to the current stream source.
        /// </summary>
        public override Stream OpenOutputStream()
        {
            return File.OpenWrite(m_filePath);
        }

        /// <summary>
        /// Gets the file extension of the resource we target to.
        /// </summary>
        public override string FileExtension
        {
            get 
            {
                if (string.IsNullOrEmpty(m_filePath)) { return string.Empty; }

                return base.GetExtensionFromFileName(m_filePath);
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