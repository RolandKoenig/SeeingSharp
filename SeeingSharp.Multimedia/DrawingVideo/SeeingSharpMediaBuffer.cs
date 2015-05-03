#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
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

using SeeingSharp.Multimedia.Core;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Namespace mappings
using MF = SharpDX.MediaFoundation;

namespace SeeingSharp.Multimedia.DrawingVideo
{
    public class SeeingSharpMediaBuffer : IDisposable, ICheckDisposed
    {
        private MF.MediaBuffer m_buffer;

        internal SeeingSharpMediaBuffer(MF.MediaBuffer buffer)
        {
            m_buffer = buffer;
        }

        /// <summary>
        /// Creates a new object targeting the the same underlying COM object.
        /// </summary>
        public SeeingSharpMediaBuffer CopyPointer()
        {
            return new SeeingSharpMediaBuffer(
                new MF.MediaBuffer(m_buffer.NativePointer));
        }


        internal MF.MediaBuffer GetBuffer()
        {
            return m_buffer;
        }

        public void Dispose()
        {
            GraphicsHelper.SafeDispose(ref m_buffer);
        }

        public bool IsDisposed
        {
            get { return m_buffer == null; }
        }
    }
}
