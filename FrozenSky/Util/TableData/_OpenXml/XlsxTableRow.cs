#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2015 Roland König (RolandK)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
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
