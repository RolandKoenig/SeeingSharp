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

namespace SeeingSharp.Util.TableData
{
    public class XlsxTableImporter : ITableImporter
    {
        /// <summary>
        /// Creates a default importer configuration object.
        /// </summary>
        public TableImporterConfig CreateDefaultConfig()
        {
            return new XlsxImporterConfig();
        }

        /// <summary>
        /// Creates a default importer configuration object.
        /// </summary>
        /// <param name="sourceFile">The source file for which the default configuration should be created.</param>
        public TableImporterConfig CreateDefaultConfig(ResourceLink sourceFile)
        {
            return new XlsxImporterConfig();
        }

        /// <summary>
        /// Tries to laod the given table file.
        /// Null is returned if opening is not possible.
        /// </summary>
        /// <param name="tableFile">The file to be loaded.</param>
        /// <param name="importConfig">The configuration for the importer.</param>
        public ITableFile OpenTableFile(ResourceLink tableFile, TableImporterConfig importConfig)
        {
            XlsxImporterConfig xslxImporterConfig = importConfig as XlsxImporterConfig;
            if (xslxImporterConfig == null) { throw new SeeingSharpException(string.Format("Invalid configuration object: {0}", importConfig)); }

            return new XlsxTableFile(tableFile, xslxImporterConfig);
        }

        /// <summary>
        /// Gets a list containing supported file extensions.
        /// </summary>
        public IEnumerable<string> GetSupportedFileExtensions()
        {
            yield return "xslx";
        }
    }
}
