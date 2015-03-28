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
        public TableImporterConfig CreateDefaultConfig(ResourceSource sourceFile)
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
        public ITableFile OpenTableFile(ResourceSource tableFileSource, TableImporterConfig importerConfig)
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
