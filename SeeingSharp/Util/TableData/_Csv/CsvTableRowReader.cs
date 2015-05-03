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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Util;

namespace SeeingSharp.Util.TableData
{
    public class CsvTableRowReader : ITableRowReader
    {
        private CsvTableFile m_parentFile;
        private StreamReader m_inStreamReader;

        internal CsvTableRowReader(CsvTableFile parentFile, ResourceLink csvFile)
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
