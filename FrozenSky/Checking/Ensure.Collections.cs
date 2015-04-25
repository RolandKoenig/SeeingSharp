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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Checking
{
    public static partial class Ensure
    {
        [Conditional("DEBUG")]
        public static void EnsureMoreThanZeroElements<T>(
            this IEnumerable<T> collection, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            // Get the collection count
            int collectionCount = -1;
            IReadOnlyCollection<T> readonlyCollection = collection as IReadOnlyCollection<T>;
            if(readonlyCollection != null)
            {
                collectionCount = readonlyCollection.Count;
            }
            else
            {
                ICollection simpleCollection = collection as ICollection;
                if(simpleCollection != null)
                {
                    collectionCount = simpleCollection.Count;
                }
            }

            // Check result
            if(collectionCount < 0)
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Collection {0} within method {1} could not be checked because of unsupported type!",
                    checkedVariableName, callerMethod));
            }
            else if (collectionCount == 0)
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Collection {0} within method {1} musst have more than zero elements!",
                    checkedVariableName, callerMethod));
            }
        }
    }
}
