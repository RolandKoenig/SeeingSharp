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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrozenSky.Checking;
using System.IO;
#if DESKTOP
using System.Windows;
#endif
#if UNIVERSAL
using Windows.Storage;
#endif

namespace FrozenSky.Util
{
    public class UriResourceLink : ResourceLink
    {
        private Uri m_resourceUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="UriResourceLink"/> class.
        /// </summary>
        /// <param name="uri">The uri from which to read.</param>
        public UriResourceLink(Uri uri)
        {
            uri.EnsureNotNull("uri");

            m_resourceUri = uri;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UriResourceLink"/> class.
        /// </summary>
        /// <param name="uriBuilder">The URI builder.</param>
        public UriResourceLink(AssemblyResourceUriBuilder uriBuilder)
            : this(uriBuilder.GetUri())
        {

        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Uri"/> to <see cref="UriResourceLink"/>.
        /// </summary>
        public static implicit operator UriResourceLink(Uri resourceUri)
        {
            return new UriResourceLink(resourceUri);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="AssemblyResourceUriBuilder"/> to <see cref="UriResourceLink"/>.
        /// </summary>
        public static implicit operator UriResourceLink(AssemblyResourceUriBuilder uriBuilder)
        {
            return new UriResourceLink(uriBuilder);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return "Uri-Resource: " + m_resourceUri; 
        }

        /// <summary>
        /// Gets an object pointing to a file at the same location (e. g. the same directory).
        /// </summary>
        /// <param name="newFileName">The new file name for which to get the ResourceLink object.</param>
        public override ResourceLink GetForAnotherFile(string newFileName)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Opens an output stream to the current stream source.
        /// </summary>
        public override Stream OpenOutputStream()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Opens the input stream to the described resource.
        /// </summary>
        public override async Task<Stream> OpenInputStreamAsync()
        {
#if DESKTOP
            return await Task.Factory.StartNew<Stream>(() => OpenInputStream());
#else
            var storageFile = await StorageFile.GetFileFromApplicationUriAsync(m_resourceUri);
            return await storageFile.OpenStreamForReadAsync();
#endif
        }

        /// <summary>
        /// Opens a stream to the resource.
        /// </summary>
        public override Stream OpenInputStream()
        {
#if DESKTOP
            return Application.GetResourceStream(m_resourceUri).Stream;
#else
            return OpenInputStreamAsync().Result;
#endif
        }

        /// <summary>
        /// Gets the file extension of the resource we target to.
        /// </summary>
        public override string FileExtension
        {
            get
            {
                return base.GetExtensionFromFileName(m_resourceUri.OriginalString);
            }
        }
    }
}
