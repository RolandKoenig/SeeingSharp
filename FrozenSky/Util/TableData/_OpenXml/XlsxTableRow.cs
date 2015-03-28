using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Util.TableData
{
    public class XlsxTableRow : ITableRow
    {
        private XlsxTableFile m_parentFile;
        private XlsxTableHeaderRow m_headerRow;
        private List<string> m_actRow;

        public XlsxTableRow(XlsxTableFile parentFile, XlsxTableHeaderRow headerRow, List<string> rowData)
        {
            m_parentFile = parentFile;
            m_headerRow = headerRow;
            m_actRow = rowData;
        }

        public T ReadField<T>(int fieldIndex) where T : struct
        {
            return (T)Convert.ChangeType(m_actRow[fieldIndex], typeof(T));
        }

        public T ReadField<T>(string fieldName) where T : struct
        {
            return (T)Convert.ChangeType(m_actRow[m_headerRow.GetFieldIndex(fieldName)], typeof(T));
        }

        public string ReadFieldAsString(int fieldIndex)
        {
            return m_actRow[fieldIndex];
        }

        public string ReadFieldAsString(string fieldName)
        {
            return m_actRow[m_headerRow.GetFieldIndex(fieldName)];
        }
    }
}
