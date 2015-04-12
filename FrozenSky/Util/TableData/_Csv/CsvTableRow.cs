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
    public class CsvTableRow : ITableRow
    {
        private static readonly Type TYPE_STRING = typeof(string);

        private CsvTableHeaderRow m_headerRow;
        private CsvTableFile m_parentFile;
        private string[] m_rowFields;
        
        internal CsvTableRow(CsvTableFile parentFile, string actRowString)
        {
            m_parentFile = parentFile;
            m_rowFields = actRowString.Split(parentFile.ImporterConfig.SeparationChar);
            m_headerRow = m_parentFile.CachedHeaderRow;
        }

        /// <summary>
        /// Reads the contents of the field with the given index.
        /// </summary>
        /// <typeparam name="T">The requested type.</typeparam>
        /// <param name="fieldIndex">The index of the field.</param>
        public T ReadField<T>(int fieldIndex) 
            where T : struct
        {
            return (T)Convert.ChangeType(m_rowFields[fieldIndex], typeof(T));
        }

        /// <summary>
        /// Reads the contents of the field with the given name.
        /// </summary>
        /// <typeparam name="T">The requested type.</typeparam>
        /// <param name="fieldName">The name of the field.</param>
        public T ReadField<T>(string fieldName) where T : struct
        {
            return (T)Convert.ChangeType(m_rowFields[m_headerRow.GetFieldIndex(fieldName)], typeof(T));
        }

        /// <summary>
        /// Reads the contents of the field with the given index.
        /// </summary>
        /// <param name="fieldIndex">The index of the field.</param>
        public string ReadFieldAsString(int fieldIndex)
        {
            return m_rowFields[fieldIndex];
        }

        /// <summary>
        /// Reads the contents of the field with the given name.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        public string ReadFieldAsString(string fieldName)
        {
            return m_rowFields[m_headerRow.GetFieldIndex(fieldName)];
        }
    }
}
