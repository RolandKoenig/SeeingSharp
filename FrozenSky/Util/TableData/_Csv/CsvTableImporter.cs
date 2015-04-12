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
using FrozenSky;
using FrozenSky.Util;

namespace FrozenSky.Util.TableData
{
    public class CsvTableImporter : ITableImporter
    {
        /// <summary>
        /// Creates a default importer configuration object.
        /// </summary>
        public TableImporterConfig CreateDefaultConfig()
        {
            return new CsvImporterConfig();
        }

        /// <summary>
        /// Creates a default importer configuration object.
        /// </summary>
        /// <param name="sourceFile">The source file for which the default configuration should be created.</param>
        public TableImporterConfig CreateDefaultConfig(ResourceLink sourceFile)
        {
            switch(sourceFile.FileExtension.ToLower())
            {
                case "csv":
                    return new CsvImporterConfig();

                case "txt":
                    CsvImporterConfig result = new CsvImporterConfig();
                    result.SeparationChar = '\t';
                    return result;

                default:
                    return new CsvImporterConfig();
            }
        }

        /// <summary>
        /// Tries to laod the given table file.
        /// Null is returned if opening is not possible.
        /// </summary>
        /// <param name="tableFileSource">The file to be loaded.</param>
        /// <param name="importerConfig">Configuration for the import process.</param>
        public ITableFile OpenTableFile(ResourceLink tableFileSource, TableImporterConfig importerConfig)
        {
            CsvImporterConfig csvImporterConfig = importerConfig as CsvImporterConfig;
            if (csvImporterConfig == null) { throw new FrozenSkyException(string.Format("Invalid configuration object: {0}", importerConfig)); }

            return new CsvTableFile(tableFileSource, csvImporterConfig);
        }

        /// <summary>
        /// Gets a list containing supported file extensions.
        /// </summary>
        public IEnumerable<string> GetSupportedFileExtensions()
        {
            yield return ".csv";
            yield return ".txt";
        }
    }
}
