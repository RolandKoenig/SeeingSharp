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

using FrozenSky.Infrastructure;
using FrozenSky.Checking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FrozenSky.Util
{
    public abstract class ResourceLink
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceLink"/> class.
        /// </summary>
        protected ResourceLink()
        {

        }

        /// <summary>
        /// Reads the complete resource to a new string.
        /// </summary>
        public string ReadCompleteToString()
        {
            using(Stream inStream = this.OpenInputStream())
            using(StreamReader inStreamReader = new StreamReader(inStream))
            {
                return inStreamReader.ReadToEnd();
            }
        }

        /// <summary>
        /// Reads the complete resource to a new string.
        /// </summary>
        public async Task<string> ReadCompleteToStringAsync()
        {
            using (Stream inStream = await this.OpenInputStreamAsync())
            using (StreamReader inStreamReader = new StreamReader(inStream))
            {
                return await inStreamReader.ReadToEndAsync();
            }
        }

        /// <summary>
        /// Gets an object pointing to a file at the same location (e. g. the same directory).
        /// </summary>
        /// <param name="newFileName">The new file name for which to get the ResourceLink object.</param>
        public abstract ResourceLink GetForAnotherFile(string newFileName);

        /// <summary>
        /// Opens an output stream to the current stream source.
        /// </summary>
        public abstract Stream OpenOutputStream();

        /// <summary>
        /// Opens the input stream to the described resource.
        /// </summary>
        public abstract Task<Stream> OpenInputStreamAsync();

        /// <summary>
        /// Opens a stream to the resource.
        /// </summary>
        public abstract Stream OpenInputStream();

        /// <summary>
        /// Gets the name of the extension from the given file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        protected string GetExtensionFromFileName(string fileName)
        {
            fileName.EnsureNotNullOrEmptyOrWhiteSpace("fileName");

            // Try to read format out of the file name
            if (!string.IsNullOrEmpty(fileName))
            {
                int indexLastDot = fileName.LastIndexOf('.');
                if (indexLastDot < 0) { return string.Empty; }
                if (fileName.Length < indexLastDot + 1) { return string.Empty; }

                return fileName.Substring(indexLastDot + 1).ToLower();
            }
            else
            {
                return string.Empty;
            }
        }

        public static implicit operator ResourceLink(AssemblyResourceLink streamFactory)
        {
            return new AssemblyResourceLinkSource(streamFactory);
        }

        public static implicit operator ResourceLink(Func<Stream> streamFactory)
        {
            return new StreamFactoryResourceLink(streamFactory);
        }

#if UNIVERSAL
        /// <summary>
        /// Performs an implicit conversion from <see cref="StorageFile"/> to <see cref="ResourceLink"/>.
        /// </summary>
        public static implicit operator ResourceLink(Windows.Storage.StorageFile storageFile)
        {
            return new StorageFileResourceLink(storageFile);
        }
#endif

#if DESKTOP
        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="ResourceLink"/>.
        /// </summary>
        public static implicit operator ResourceLink(string fileName)
        {
            return new DesktopFileSystemResourceLink(fileName);
        }
#endif

        /// <summary>
        /// Performs an implicit conversion from <see cref="AssemblyResourceUriBuilder"/> to <see cref="ResourceLink"/>.
        /// </summary>
        public static implicit operator ResourceLink(AssemblyResourceUriBuilder uriBuilder)
        {
            return new UriResourceLink(uriBuilder);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Uri"/> to <see cref="ResourceLink"/>.
        /// </summary>
        public static implicit operator ResourceLink(Uri resourceUri)
        {
            return new UriResourceLink(resourceUri);
        }

        /// <summary>
        /// Gets the file extension of the resource we target to.
        /// </summary>
        public abstract string FileExtension
        {
            get;
        }
    }
}
