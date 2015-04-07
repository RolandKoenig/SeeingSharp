using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Util.TableData
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
            if (xslxImporterConfig == null) { throw new FrozenSkyException(string.Format("Invalid configuration object: {0}", importConfig)); }

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
