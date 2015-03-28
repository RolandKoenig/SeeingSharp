using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Util.TableData
{
    public class XlsxTableHeaderRow : ITableHeaderRow
    {
        private XlsxTableFile m_parentFile;
        private List<string> m_headers;

        internal XlsxTableHeaderRow(XlsxTableFile parentFile, List<string> headers)
        {
            m_parentFile = parentFile;
            m_headers = headers;
        }

        public int GetFieldIndex(string fieldName)
        {
            int fieldIndex = m_headers.IndexOf(fieldName);
            if (fieldIndex < 0) { throw new FrozenSkyException(string.Format("Field {0} not found!", fieldName)); }
            return fieldIndex;
        }

        public string GetFieldName(int fieldIndex)
        {
            return m_headers[fieldIndex];
        }

        public int FieldCount
        {
            get { return m_headers.Count; }
        }
    }
}
