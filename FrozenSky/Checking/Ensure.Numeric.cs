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

namespace FrozenSky.Checking
{
    public static partial class Ensure
    {
        //---------------------------------------------------------------------
        // Method 'EnsureInRange' for all common numeric variables
        #region
        [Conditional("DEBUG")]
        public static void EnsureInRange(
            this byte numValue, byte min, byte max, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if ((numValue < min) ||
                (numValue > max))
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Value {0} within method {1} must be between {2} and {3} (given value is {4}!",
                    checkedVariableName, callerMethod,
                    min, max, numValue));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureInRange(
            this short numValue, short min, short max, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if ((numValue < min) ||
                (numValue > max))
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Value {0} within method {1} must be between {2} and {3} (given value is {4}!",
                    checkedVariableName, callerMethod,
                    min, max, numValue));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureInRange(
            this int numValue, int min, int max, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if ((numValue < min) ||
                (numValue > max))
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Value {0} within method {1} must be between {2} and {3} (given value is {4}!",
                    checkedVariableName, callerMethod,
                    min, max, numValue));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureInRange(
            this long numValue, long min, long max, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if ((numValue < min) ||
                (numValue > max))
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Value {0} within method {1} must be between {2} and {3} (given value is {4}!",
                    checkedVariableName, callerMethod,
                    min, max, numValue));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureInRange(
            this ushort numValue, ushort min, ushort max, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if ((numValue < min) ||
                (numValue > max))
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Value {0} within method {1} must be between {2} and {3} (given value is {4}!",
                    checkedVariableName, callerMethod,
                    min, max, numValue));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureInRange(
            this uint numValue, uint min, uint max, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if ((numValue < min) ||
                (numValue > max))
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Value {0} within method {1} must be between {2} and {3} (given value is {4}!",
                    checkedVariableName, callerMethod,
                    min, max, numValue));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureInRange(
            this ulong numValue, ulong min, ulong max, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if ((numValue < min) ||
                (numValue > max))
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Value {0} within method {1} must be between {2} and {3} (given value is {4}!",
                    checkedVariableName, callerMethod,
                    min, max, numValue));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureInRange(
            this float numValue, float min, float max, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if ((numValue < min) ||
                (numValue > max))
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Value {0} within method {1} must be between {2} and {3} (given value is {4}!",
                    checkedVariableName, callerMethod,
                    min, max, numValue));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureInRange(
            this double numValue, double min, double max, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if ((numValue < min) ||
                (numValue > max))
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Value {0} within method {1} must be between {2} and {3} (given value is {4}!",
                    checkedVariableName, callerMethod,
                    min, max, numValue));
            }
        }

        #endregion

        //---------------------------------------------------------------------
        // Method 'EnsurePositive' for all common numeric variables
        #region
        [Conditional("DEBUG")]
        public static void EnsurePositive(
            this int numValue, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if (numValue < 0)
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Value {0} within method {1} must be positive (value: {2})!",
                    checkedVariableName, callerMethod, numValue));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsurePositive(
            this short numValue, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if (numValue < 0)
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Value {0} within method {1} must be positive (value: {2})!",
                    checkedVariableName, callerMethod, numValue));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsurePositive(
            this long numValue, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if (numValue < 0)
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Value {0} within method {1} must be positive (value: {2})!",
                    checkedVariableName, callerMethod, numValue));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsurePositive(
            this float numValue, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if (numValue < 0)
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Value {0} within method {1} must be positive (value: {2})!",
                    checkedVariableName, callerMethod, numValue));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsurePositive(
            this double numValue, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if (numValue < 0)
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Value {0} within method {1} must be positive (value: {2})!",
                    checkedVariableName, callerMethod, numValue));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsurePositive(
            this decimal numValue, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if (numValue < 0)
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Value {0} within method {1} must be positive (value: {2})!",
                    checkedVariableName, callerMethod, numValue));
            }
        }
        #endregion
    }
}
