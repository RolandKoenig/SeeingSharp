#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2014 Roland König (RolandK)

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

namespace FrozenSky.Util
{
    public class AssemblyResourceUriBuilder
    {
        private string m_assemblyName;
        private bool m_isMainAssembly;
        private string m_filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyResourceUriBuilder"/> class.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="isMainAssembly">if set to <c>true</c> [is main assembly].</param>
        /// <param name="filePath">The file path.</param>
        public AssemblyResourceUriBuilder(string assemblyName, bool isMainAssembly, string filePath)
        {
            m_assemblyName = assemblyName;
            m_isMainAssembly = isMainAssembly;
            m_filePath = filePath;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "Resource: [{0}] {1}",
                m_assemblyName, m_filePath);
        }

        /// <summary>
        /// Builds the Uri object.
        /// </summary>
        public Uri GetUri()
        {
#if UNIVERSAL
            return new Uri(string.Format("ms-appx:///{0}", m_filePath));
#elif DESKTOP
            return new Uri(string.Format("/{0};component/{1}", m_assemblyName, m_filePath), UriKind.Relative);
#else 
            throw new InvalidOperationException("Unable to generate resource uri: Platform not handled!");
#endif
        }
    }
}
