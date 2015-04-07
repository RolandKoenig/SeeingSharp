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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

#if UNIVERSAL
using Windows.ApplicationModel;
using Windows.Storage;
#endif

namespace FrozenSky.Util
{
    public class ResourceLink
    {
        private Func<Stream> m_streamFactory;
        private string m_fileName;
        private AssemblyResourceLink m_resourceLink;
        private Uri m_resourceUri;
        private AssemblyResourceUriBuilder m_uriBuilder;
#if UNIVERSAL
        private StorageFile m_storageFile;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceLink" /> class.
        /// </summary>
        /// <param name="fileName">Name of the physical file.</param>
        public ResourceLink(string fileName)
        {
            m_fileName = fileName;
        }

#if UNIVERSAL
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceLink"/> class.
        /// </summary>
        /// <param name="storageFile">The reference to the WinRT StorageFile object.</param>
        public ResourceLink(StorageFile storageFile)
        {
            m_storageFile = storageFile;
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceLink" /> class.
        /// </summary>
        /// <param name="resourceLink">The assembly resource link.</param>
        public ResourceLink(AssemblyResourceLink resourceLink)
        {
            m_fileName = string.Empty;
            m_resourceLink = resourceLink;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceLink" /> class.
        /// </summary>
        /// <param name="resourceUri">The WPF/WinRT resource URI.</param>
        public ResourceLink(Uri resourceUri)
        {
            m_fileName = string.Empty;
            m_resourceUri = resourceUri;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceLink"/> class.
        /// </summary>
        /// <param name="streamFactory">A factory method which creates a stream.</param>
        public ResourceLink(Func<Stream> streamFactory)
        {
            m_streamFactory = streamFactory;
            m_fileName = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceLink"/> class.
        /// </summary>
        /// <param name="uriBuilder">The URI builder.</param>
        public ResourceLink(AssemblyResourceUriBuilder uriBuilder)
        {
            m_fileName = string.Empty;
            m_uriBuilder = uriBuilder;
            m_resourceUri = uriBuilder.GetUri();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(m_fileName))
            {
                return "File-Resource: " + m_fileName;
            }
            else if (m_resourceLink != null)
            {
                return "Assembly-Resource: " + m_resourceLink.ToString();
            }
            else if (m_resourceUri != null)
            {
                return "Uri-Resource: " + m_resourceUri;
            }
#if UNIVERSAL
            else if (m_storageFile != null)
            {
                if (!string.IsNullOrEmpty(m_storageFile.Path)) { return "StorageFile: " + m_storageFile.Path; }
                else { return "StoageFile: " + m_storageFile.Name; }
            }
#endif
            else
            {
                return "(Empty)";
            }
        }

        /// <summary>
        /// Gets an object pointing to a file at the same location (e. g. the same directory).
        /// </summary>
        /// <param name="newFileName">The new file name for which to get the ResourceLink object.</param>
        public ResourceLink GetForAnotherFile(string newFileName)
        {
            // Handle file path on hard disc
            if(!string.IsNullOrEmpty(m_fileName))
            {
                string directoryName = Path.GetDirectoryName(m_fileName);
                if(!string.IsNullOrEmpty(directoryName))
                {
                    return new ResourceLink(Path.Combine(directoryName, newFileName));
                }
                else
                {
                    return new ResourceLink(newFileName);
                }
            }

            // Handle AssemblyResourceLink objects
            if(m_resourceLink != null)
            {
                return new ResourceLink(m_resourceLink.GetForAnotherFile(newFileName));
            }

            if(m_resourceUri != null)
            {
                throw new NotImplementedException("This method is still not implemented for uri resources!");
            }

#if UNIVERSAL
            if(m_storageFile != null)
            {
                // TODO: Move to another file within the same folder?
            }
#endif

            throw new FrozenSkyException("Unable to get a new ResourceLink object for file " + newFileName + "!");
        }

        /// <summary>
        /// Opens an output stream to the current stream source.
        /// </summary>
        public Stream OpenOutputStream()
        {
            // Open stream from file
            if (!string.IsNullOrEmpty(m_fileName))
            {
#if DESKTOP || WINDOWS_PHONE
                return new FileStream(m_fileName, FileMode.Create, FileAccess.Write);
#else
            throw new FrozenSkyException("Writing to this ResourceLink not supported!");
#endif
            }

#if UNIVERSAL
            if(m_storageFile != null)
            {
                return m_storageFile.OpenStreamForWriteAsync().Result;
            }
#endif

            throw new FrozenSkyException("Writing to this ResourceLink not supported!");
        }

        /// <summary>
        /// Opens the input stream to the described resource.
        /// </summary>
        public async Task<Stream> OpenInputStreamAsync()
        {
            // Open stream from file
            if (!string.IsNullOrEmpty(m_fileName))
            {
#if DESKTOP 
                return File.OpenRead(m_fileName);
#else
                return await Package.Current.InstalledLocation.OpenStreamForReadAsync(m_fileName);
#endif
            }

            // Open stream from resource Uri
            if (m_resourceUri != null)
            {
#if DESKTOP 
                return Application.GetResourceStream(m_resourceUri).Stream;
#else
                var storageFile = await StorageFile.GetFileFromApplicationUriAsync(m_resourceUri);
                return await storageFile.OpenStreamForReadAsync();
#endif
            }

#if UNIVERSAL
            if (m_storageFile != null)
            {
                return await m_storageFile.OpenStreamForReadAsync();
            }
#endif

            // Open stream from assembly resource link
            if (m_resourceLink != null)
            {
                return m_resourceLink.OpenRead();
            }

            // This simple call should prevent the "missing await" on DESKTOP platform
            // .. not the best solution, but this code is only reached in error case
#if DESKTOP
            await Task.Delay(10);
#endif

            throw new FrozenSkyException("Unable to load resource stream:\n" + this.ToString()); 
        }

        /// <summary>
        /// Opens a stream to the resource.
        /// </summary>
        public Stream OpenInputStream()
        {
            return OpenInputStreamAsync().Result;
        }

        /// <summary>
        /// Tries to get the file extension of the resource we target to.
        /// </summary>
        public string FileExtension
        {
            get
            {
                // Try to get a valid file name
                string fileName = m_fileName;
                if (m_resourceUri != null)
                {
                    fileName = m_resourceUri.OriginalString;
                }
                else if(m_resourceLink != null)
                {
                    fileName = m_resourceLink.ResourcePath;
                }
#if UNIVERSAL
                else if(m_storageFile != null)
                {
                    fileName = m_storageFile.Path;
                }
#endif

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
        }


        public static implicit operator ResourceLink(string fileName)
        {
            return new ResourceLink(fileName);
        }

#if UNIVERSAL
        public static implicit operator ResourceLink(StorageFile storageFile)
        {
            return new ResourceLink(storageFile);
        }
#endif

        public static implicit operator ResourceLink(AssemblyResourceLink resourceLink)
        {
            return new ResourceLink(resourceLink);
        }

        public static implicit operator ResourceLink(Uri resourceUri)
        {
            return new ResourceLink(resourceUri);
        }

        public static implicit operator ResourceLink(Func<Stream> streamFactory)
        {
            return new ResourceLink(streamFactory);
        }

        public static implicit operator ResourceLink(AssemblyResourceUriBuilder uriBuilder)
        {
            return new ResourceLink(uriBuilder);
        }
    }
}
