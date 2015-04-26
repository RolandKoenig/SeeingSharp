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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Checking;
using System.IO;

namespace SeeingSharp.Util
{
    public class AssemblyResourceLinkSource : ResourceLink
    {
        private AssemblyResourceLink m_resourceLink;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyResourceLinkSource" /> class.
        /// </summary>
        /// <param name="resourceLink">The link to the resource.</param>
        public AssemblyResourceLinkSource(AssemblyResourceLink resourceLink)
        {
            resourceLink.EnsureNotNull("resourceLink");

            m_resourceLink = resourceLink;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="AssemblyResourceLink"/> to <see cref="AssemblyResourceLinkSource"/>.
        /// </summary>
        public static implicit operator AssemblyResourceLinkSource(AssemblyResourceLink resourceLink)
        {
            return new AssemblyResourceLinkSource(resourceLink);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return "Assembly-Resource: " + m_resourceLink.ToString();
        }

        /// <summary>
        /// Gets an object pointing to a file at the same location (e. g. the same directory).
        /// </summary>
        /// <param name="newFileName">The new file name for which to get the ResourceLink object.</param>
        public override ResourceLink GetForAnotherFile(string newFileName)
        {
            newFileName.EnsureNotNullOrEmptyOrWhiteSpace("newFileName");

            return new AssemblyResourceLinkSource(
                m_resourceLink.GetForAnotherFile(newFileName));
        }

        /// <summary>
        /// Opens an output stream to the current stream source.
        /// </summary>
        public override Stream OpenOutputStream()
        {
            throw new SeeingSharpException("Unable to get an output stream to an assembly resource!");
        }

        /// <summary>
        /// Opens the input stream to the described resource.
        /// </summary>
        public override Task<Stream> OpenInputStreamAsync()
        {
            return Task.Factory.StartNew(() => OpenInputStream());
        }

        /// <summary>
        /// Opens a stream to the resource.
        /// </summary>
        public override Stream OpenInputStream()
        {
            return m_resourceLink.OpenRead();
        }

        /// <summary>
        /// Gets the file extension of the resource we target to.
        /// </summary>
        public override string FileExtension
        {
            get { return base.GetExtensionFromFileName(m_resourceLink.ResourceFile); }
        }
    }
}
