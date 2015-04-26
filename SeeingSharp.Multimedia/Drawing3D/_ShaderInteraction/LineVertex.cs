#region License information (SeeingSharp and all based games/applications)
/*
    SeeingSharp and all games/applications based on it (more info at http://www.rolandk.de/wp)
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

//Some namespace mappings
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;

namespace SeeingSharp.Multimedia.Drawing3D
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct LineVertex
    {
        public Vector3 Position;

        // Create InputElements descriptior for Direct3D 11 api
        public static readonly int Size = Marshal.SizeOf<LineVertex>();

        // Structure describing all elements of one vertex
        public static readonly D3D11.InputElement[] InputElements = new D3D11.InputElement[]
        {
            new D3D11.InputElement("POSITION", 0, DXGI.Format.R32G32B32_Float, 0, 0)
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="LineVertex" /> struct.
        /// </summary>
        /// <param name="position">The position.</param>
        public LineVertex(Vector3 position)
        {
            this.Position = position;
        }
    }
}
