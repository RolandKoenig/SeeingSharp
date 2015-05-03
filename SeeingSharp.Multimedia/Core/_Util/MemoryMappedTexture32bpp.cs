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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Multimedia.Core
{
    public unsafe class MemoryMappedTexture32bpp : IDisposable
    {
        #region The native structure, where we store all ObjectIDs uploaded from graphics hardware
        private IntPtr m_pointer;
        private int* m_pointerNative;
        private Size2 m_size;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryMappedTexture32bpp"/> class.
        /// </summary>
        /// <param name="size">The total size of the texture.</param>
        public MemoryMappedTexture32bpp(Size2 size)
        {
            m_pointer = Marshal.AllocHGlobal(size.Width * size.Height * 4);
            m_pointerNative = (int*)m_pointer.ToPointer();
            m_size = size;
        }

        /// <summary>
        /// Führt anwendungsspezifische Aufgaben aus, die mit dem Freigeben, Zurückgeben oder Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
        /// </summary>
        public void Dispose()
        {
            Marshal.FreeHGlobal(m_pointer);
            m_pointer = IntPtr.Zero;
            m_pointerNative = (int*)0;
            m_size = new Size2(0, 0);
        }

        /// <summary>
        /// Gets the value at the given (pixel) location.
        /// </summary>
        /// <param name="xPos">The x position.</param>
        /// <param name="yPos">The y position.</param>
        public int GetValue(int xPos, int yPos)
        {
            return m_pointerNative[xPos + (yPos * m_size.Width)];
        }

        /// <summary>
        /// Sets all alpha values to one.
        /// </summary>
        public void SetAllAlphaValuesToOne()
        {
            byte* pointerNativeByte = (byte*)m_pointerNative;
            for(int loopX=0 ; loopX<m_size.Width; loopX++)
            {
                for(int loopY=0 ; loopY<m_size.Height; loopY++)
                {
                    // Change alpha byte to 255
                    int index = loopX * 4 + (loopY * this.Pitch);
                    pointerNativeByte[index + 3] = 255;  
                }
            }
        }

        /// <summary>
        /// Gets the total size of the buffer in bytes.
        /// </summary>
        public uint SizeInBytes
        {
            get
            {
                return (uint)(m_size.Width * m_size.Height * 4);
            }
        }

        /// <summary>
        /// Gets the width of the buffer.
        /// </summary>
        public int Width
        {
            get { return m_size.Width; }
        }

        /// <summary>
        /// Gets the pitch of the underlying texture data.
        /// </summary>
        public int Pitch
        {
            get { return m_size.Width * 4; }
        }

        /// <summary>
        /// Gets the height of the buffer.
        /// </summary>
        public int Height
        {
            get { return m_size.Height; }
        }

        /// <summary>
        /// Gets the pixel size of this texture.
        /// </summary>
        public Size2 PixelSize
        {
            get
            {
                return m_size;
            }
        }

        /// <summary>
        /// Gets the pointer of the buffer.
        /// </summary>
        public IntPtr Pointer
        {
            get 
            {
                if (m_pointer == IntPtr.Zero) { throw new ObjectDisposedException("MemoryMappedTextureFloat"); }
                return m_pointer; 
            }
        }
    }
}
