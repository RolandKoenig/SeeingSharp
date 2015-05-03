#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
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
using DocumentFormat.OpenXml;

namespace SeeingSharp.Util.TableData
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
