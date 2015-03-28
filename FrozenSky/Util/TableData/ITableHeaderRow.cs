using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Util.TableData
{
    /// <summary>
    /// Represents the header row within the table.
    /// </summary>
    public interface ITableHeaderRow
    {
        /// <summary>
        /// Gets the index for the given field name.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        int GetFieldIndex(string fieldName);

        /// <summary>
        /// Gets the name of the field with the given index.
        /// </summary>
        /// <param name="fieldIndex">The index of the field.</param>
        string GetFieldName(int fieldIndex);

        /// <summary>
        /// Gets the total count of fields contained in the datasource.
        /// </summary>
        int FieldCount
        {
            get;
        }
    }
}
