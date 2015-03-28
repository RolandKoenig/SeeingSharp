using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Util.TableData
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
