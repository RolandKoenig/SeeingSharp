using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Util.TableData
{
    /// <summary>
    /// Represents a row within the table.
    /// </summary>
    public interface ITableRow
    {
        /// <summary>
        /// Reads the contents of the field with the given index.
        /// </summary>
        /// <param name="fieldIndex">The index of the field.</param>
        T ReadField<T>(int fieldIndex)
            where T : struct;

        /// <summary>
        /// Reads the contents of the field with the given name.
        /// </summary>
        /// <param name="fieldName">The name of the field to read.</param>
        T ReadField<T>(string fieldName)
            where T : struct;

        /// <summary>
        /// Reads the contents of the field with the given index.
        /// </summary>
        /// <param name="fieldIndex">The index of the field.</param>
        string ReadFieldAsString(int fieldIndex);

        /// <summary>
        /// Reads the contents of the field with the given name.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        string ReadFieldAsString(string fieldName);
    }
}
