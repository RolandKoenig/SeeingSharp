#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Util.TableData
{
    /// <summary>
    /// Represents a table file which may contain more tables.
    /// </summary>
    public interface ITableFile : IDisposable
    {
        /// <summary>
        /// Gets a collection containing all tables names defined 
        /// </summary>
        IEnumerable<string> GetTableNames();
    
        /// <summary>
        /// Reads the header row from the given table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        ITableHeaderRow ReadHeaderRow(string tableName);

        /// <summary>
        /// Opens a reader which reads all rows within the given table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        ITableRowReader OpenReader(string tableName);

    }
}
