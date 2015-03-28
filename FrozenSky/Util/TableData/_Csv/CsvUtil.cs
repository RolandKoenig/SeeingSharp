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
        internal static StreamReader OpenReader(ResourceSource sourceFile, CsvImporterConfig importerConfig)
        {
            if (importerConfig.Encoding != null) { return new StreamReader(sourceFile.OpenInputStream(), importerConfig.Encoding); }
            else { return new StreamReader(sourceFile.OpenInputStream()); }
        }
    }
}
