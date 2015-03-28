using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Util.TableData
{
    public class CsvTableHeaderRow : ITableHeaderRow
    {
        private CsvTableFile m_parentFile;
        private Dictionary<string, int> m_columnIndices;
        private string[] m_headers;

        internal CsvTableHeaderRow(CsvTableFile parentFile, string rowString)
        {
            m_parentFile = parentFile;
            m_columnIndices = new Dictionary<string, int>();

            m_headers = rowString.Split(parentFile.ImporterConfig.SeparationChar);
            for (int loop = 0; loop < m_headers.Length; loop++)
            {
                if (m_headers[loop] != null)
                {
                    m_columnIndices[m_headers[loop]] = loop;
                }
            }
        }

        /// <summary>
        /// Gets the index for the given field name.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        public int GetFieldIndex(string fieldName)
        {
            return m_columnIndices[fieldName];
        }

        /// <summary>
        /// Gets the name of the field with the given index.
        /// </summary>
        /// <param name="fieldIndex">The index of the field.</param>
        public string GetFieldName(int fieldIndex)
        {
            return m_headers[fieldIndex];
        }

        /// <summary>
        /// Gets the total count of fields contained in the datasource.
        /// </summary>
        public int FieldCount
        {
            get { return m_headers.Length; }
        }
    }
}
