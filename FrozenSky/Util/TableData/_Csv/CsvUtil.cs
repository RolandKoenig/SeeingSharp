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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Util.TableData
{
    internal static class CsvUtil
    {
        /// <summary>
        /// Opens a reader to the given source file.
        /// </summary>
        /// <param name="sourceFile">The source file to be loaded.</param>
        /// <param name="importerConfig">The configuration of the importer.</param>
        internal static StreamReader OpenReader(ResourceLink sourceFile, CsvImporterConfig importerConfig)
        {
            if (importerConfig.Encoding != null) { return new StreamReader(sourceFile.OpenInputStream(), importerConfig.Encoding); }
            else { return new StreamReader(sourceFile.OpenInputStream()); }
        }
    }
}
