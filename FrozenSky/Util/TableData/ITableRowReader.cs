using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Util.TableData
{
    /// <summary>
    /// This class reads the table's contents row by row.
    /// </summary>
    public interface ITableRowReader : IDisposable
    {
        /// <summary>
        /// Reads all rows from the table.
        /// </summary>
        IEnumerable<ITableRow> ReadAllRows();
    }
}
