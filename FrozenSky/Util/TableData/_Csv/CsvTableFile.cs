using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrozenSky;
using FrozenSky.Util;

namespace FrozenSky.Util.TableData
{
    public class CsvTableFile : ITableFile
    {
        internal const string TABLE_NAME = "CSV";

        // Configuration members
        #region
        private ResourceSource m_tableFileSource;
        private CsvImporterConfig m_importerConfig;
        #endregion

        // Caching
        #region
        private CsvTableHeaderRow m_headerRow;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvTableFile"/> class.
        /// </summary>
        /// <param name="tableFileSource">The file from which to load the table data.</param>
        /// <param name="importerConfig">The configuration for the import process.</param>
        internal CsvTableFile(ResourceSource tableFileSource, CsvImporterConfig importerConfig)
        {
            m_importerConfig = importerConfig;
            m_tableFileSource = tableFileSource;

            using(StreamReader inStreamReader = CsvUtil.OpenReader(tableFileSource, importerConfig))
            {
                // Read until we reach the header row
                for(int loop=0 ; loop<m_importerConfig.HeaderRowIndex; loop++)
                {
                    inStreamReader.ReadLine();
                }

                // Read the header row and ensure that we have something there
                string headerRow = inStreamReader.ReadLine();
                if (string.IsNullOrEmpty(headerRow)) { throw new FrozenSkyException(string.Format("No header row found in csv file{0}", tableFileSource)); }
                m_headerRow = new CsvTableHeaderRow(this, headerRow);
            }
        }

        /// <summary>
        /// Gets a collection containing all tables names defined
        /// </summary>
        public IEnumerable<string> GetTableNames()
        {
            yield return TABLE_NAME;
            yield break;
        }

        public void Dispose()
        {
            // Nothing to do here, because we don't hold any resources
        }

        /// <summary>
        /// Reads the header row from the given table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        public ITableHeaderRow ReadHeaderRow(string tableName)
        {
            if (tableName != TABLE_NAME) { throw new FrozenSkyException(string.Format("Invalid table name requested: {0}", tableName)); }
            return m_headerRow;
        }

        /// <summary>
        /// Opens a reader which reads all rows within the given table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns></returns>
        public ITableRowReader OpenReader(string tableName)
        {
            if (tableName != TABLE_NAME) { throw new FrozenSkyException(string.Format("Invalid table name requested: {0}", tableName)); }
            return new CsvTableRowReader(this, m_tableFileSource);
        }

        public CsvImporterConfig ImporterConfig
        {
            get { return m_importerConfig; }
        }

        public CsvTableHeaderRow CachedHeaderRow
        {
            get { return m_headerRow; }
        }
    }
}
