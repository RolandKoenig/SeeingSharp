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
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FrozenSky.Util;

namespace FrozenSky.Checking
{
    /// <summary>
    /// This class contains some helper methods which can be used
    /// to check method parameters.
    /// Note: All methods are only executing in DebugBuilds.
    /// </summary>
    public static partial class Ensure
    {
        [Conditional("DEBUG")]
        public static void EnsureNotNullOrDisposed(
            this ICheckDisposed disposable, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if ((disposable == null) ||
                (disposable.IsDisposed))
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Disposable onject {0} within method {1} must not be null or disposed!",
                    checkedVariableName, callerMethod));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureFalse(
            this bool boolValue, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if (boolValue)
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Boolean {0} within method {1} must be false!",
                    checkedVariableName, callerMethod));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureTrue(
            this bool boolValue, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if (!boolValue)
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Boolean {0} within method {1} must be true!",
                    checkedVariableName, callerMethod));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureNotNullOrEmpty<T>(
            this T[] array, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if ((array == null) ||
                (array.Length == 0))
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Array {0} within method {1} must not be null or empty!",
                    checkedVariableName, callerMethod));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureNotNullOrEmpty<T>(
            this ICollection<T> collection, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if ((collection == null) ||
                (collection.Count == 0))
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Collection {0} within method {1} must not be null or empty!",
                    checkedVariableName, callerMethod));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureNotNullOrEmpty<T>(
            this IReadOnlyCollection<T> collection, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if ((collection == null) ||
                (collection.Count == 0))
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Collection {0} within method {1} must not be null or empty!",
                    checkedVariableName, callerMethod));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureNotNullOrEmpty(
            this string stringParam, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if(string.IsNullOrEmpty(stringParam))
            {
                throw new FrozenSkyCheckException(string.Format(
                    "String {0} within method {1} must not be null or empty!",
                    checkedVariableName, callerMethod));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureNotNullOrEmptyOrWhiteSpace(
            this string stringParam, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if (string.IsNullOrWhiteSpace(stringParam))
            {
                throw new FrozenSkyCheckException(string.Format(
                    "String {0} within method {1} must not be null or empty!",
                    checkedVariableName, callerMethod));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureNotNull(
            this object objParam, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if (objParam == null)
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Object {0} within method {1} must not be null!",
                    checkedVariableName, callerMethod));
            }
        }
    }
}
