using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrozenSky.Util;

namespace FrozenSky.Util.TableData
{
    public class CsvTableRowReader : ITableRowReader
    {
        private CsvTableFile m_parentFile;
        private StreamReader m_inStreamReader;

        internal CsvTableRowReader(CsvTableFile parentFile, ResourceSource csvFile)
        {
            // Open the csv file for reading
            m_parentFile = parentFile;
            m_inStreamReader = CsvUtil.OpenReader(csvFile, parentFile.ImporterConfig);

            // Read until we reach the starting line
            for(int loop=0; loop<m_parentFile.ImporterConfig.FirstValueRowIndex; loop++)
            {
                m_inStreamReader.ReadLine();
            }
        }

        public void Dispose()
        {
            CommonTools.SafeDispose(ref m_inStreamReader);
        }

        /// <summary>
        /// Reads all rows from the table.
        /// </summary>
        public IEnumerable<ITableRow> ReadAllRows()
        {
            string actLine = m_inStreamReader.ReadLine();
            while(actLine != null)
            {
                if (!string.IsNullOrEmpty(actLine))
                {
                    yield return new CsvTableRow(m_parentFile, actLine);
                }

                if (m_inStreamReader == null) { throw new ObjectDisposedException("CsvTableRow"); }

                actLine = m_inStreamReader.ReadLine();
            }
        }
    }
}
