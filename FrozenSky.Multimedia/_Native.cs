#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2014 Roland König (RolandK)

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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Multimedia
{
    static class NativeMethods
    {
        [DllImport("Mfreadwrite.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "MFCreateSourceReaderFromByteStream")]
        public static extern int MFCreateSourceReaderFromByteStream_Native(IntPtr pByteStream, IntPtr pAttributes, out IntPtr pSourceReader);

#if !UNIVERSAL
        /// <summary>
        /// Copies unmanaged memory from given source location to given target. 
        /// </summary>
        /// <param name="dest">The desitination address.</param>
        /// <param name="src">The source address.</param>
        /// <param name="count">Total count of bytes.</param>
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr MemCopy(IntPtr dest, IntPtr src, UIntPtr count);
#endif
    }
}
