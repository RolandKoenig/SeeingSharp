using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrozenSky.Util;

namespace FrozenSky.Util.TableData
{
    /// <summary>
    /// A generic importer interface which can import tables from a given datasource.
    /// </summary>
    public interface ITableImporter
    {
        /// <summary>
        /// Creates a default importer configuration object.
        /// </summary>
        TableImporterConfig CreateDefaultConfig();

        /// <summary>
        /// Creates a default importer configuration object.
        /// </summary>
        /// <param name="sourceFile">The source file for which the default configuration should be created.</param>
        TableImporterConfig CreateDefaultConfig(ResourceSource sourceFile);

        /// <summary>
        /// Tries to laod the given table file.
        /// Null is returned if opening is not possible.
        /// </summary>
        /// <param name="tableFile">The file to be loaded.</param>
        /// <param name="importConfig">The configuration for the importer.</param>
        ITableFile OpenTableFile(ResourceSource tableFile, TableImporterConfig importConfig);

        /// <summary>
        /// Gets a list containing supported file extensions.
        /// </summary>
        IEnumerable<string> GetSupportedFileExtensions();
    }
}
