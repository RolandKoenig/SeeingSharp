﻿#region License information (FrozenSky and all based games/applications)
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
        public static void EnsureReadable(
            this Stream stream, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            try
            {
                if (!stream.CanRead)
                {
                    throw new FrozenSkyCheckException(string.Format(
                        "Stream {0} within method {1} must be readable!",
                        checkedVariableName, callerMethod));
                }
            }
            catch(ObjectDisposedException)
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Stream {0} within method {1} must not be disposed!",
                    checkedVariableName, callerMethod));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureWritable(
            this Stream stream, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            try
            {
                if (!stream.CanWrite)
                {
                    throw new FrozenSkyCheckException(string.Format(
                        "Stream {0} within method {1} must be writable!",
                        checkedVariableName, callerMethod));
                }
            }
            catch (ObjectDisposedException)
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Stream {0} within method {1} must not be disposed!",
                    checkedVariableName, callerMethod));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureSeekable(
            this Stream stream, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            try
            {
                if (!stream.CanSeek)
                {
                    throw new FrozenSkyCheckException(string.Format(
                        "Stream {0} within method {1} must be seekable!",
                        checkedVariableName, callerMethod));
                }
            }
            catch (ObjectDisposedException)
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Stream {0} within method {1} must not be disposed!",
                    checkedVariableName, callerMethod));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureFileExists(
            this string filePath, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

#if DESKTOP
            if (!File.Exists(filePath))
            {
                throw new FrozenSkyCheckException(string.Format(
                    "Filepath {0} within method {1} could not be resolved (value: {2})!",
                    checkedVariableName, callerMethod,
                    filePath));
            }
#else
            // Not possible on WinRT at all (no direct filesystem access)
            throw new FrozenSkyCheckException(string.Format(
                "Filepath {0} within method {1} could not be resolved (value: {2})!",
                checkedVariableName, callerMethod,
                filePath));
#endif
        }

    }
}
