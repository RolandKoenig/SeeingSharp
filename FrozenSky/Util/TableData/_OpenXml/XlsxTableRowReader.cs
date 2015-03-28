using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;

namespace FrozenSky.Util.TableData
{
    public class XlsxTableRowReader : ITableRowReader
    {
        private XlsxTableFile m_parentFile;
        private XlsxTableHeaderRow m_headerRow;
        private OpenXmlReader m_reader;
        private IEnumerable<List<string>> m_rowReader;

        internal XlsxTableRowReader(XlsxTableFile parentFile, XlsxTableHeaderRow headerRow, OpenXmlReader reader, IEnumerable<List<string>> rowReader)
        {
            m_parentFile = parentFile;
            m_headerRow = headerRow;
            m_reader = reader;
            m_rowReader = rowReader;
        }

        /// <summary>
        /// Reads all rows from the table.
        /// </summary>
        public IEnumerable<ITableRow> ReadAllRows()
        {
            int actLineIndex = -1;
            foreach(List<string> actRow in m_rowReader)
            {
                actLineIndex++;

                if(actLineIndex >= m_parentFile.ImporterConfig.FirstValueRowIndex)
                {
                    yield return new XlsxTableRow(m_parentFile, m_headerRow, actRow);
                }
            }
        }

        public void Dispose()
        {
            CommonTools.SafeDispose(ref m_reader);
            m_rowReader = null;
        }
    }
}
