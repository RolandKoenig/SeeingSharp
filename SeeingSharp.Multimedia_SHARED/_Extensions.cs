#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Mathematics.Interop;

namespace SeeingSharp.Multimedia
{
    internal static class Extensions
    {
#if UNIVERSAL
        internal static Windows.UI.Color ToWindowsColor(this Color4 color)
        {
            return new Windows.UI.Color()
            {
                A = (byte)(color.Alpha * 255),
                R = (byte)(color.Red * 255),
                G = (byte)(color.Green * 255),
                B = (byte)(color.Blue * 255)
            };
        }
#endif

        internal static unsafe RawMatrix3x2 ToDXMatrix(this Matrix3x2 matrix)
        {
            return *(RawMatrix3x2*)&matrix;
        }

        internal static unsafe Matrix3x2 ToMatrix(this RawMatrix3x2 sdxMatrix)
        {
            return *(Matrix3x2*)&sdxMatrix;
        }

        internal static unsafe RawColor4 ToDXColor(this Color4 color)
        {
            return *(RawColor4*)&color;
        }

        internal static unsafe RawVector2 ToDXVector(this Vector2 vector)
        {
            return *(RawVector2*)&vector;
        }

        internal static unsafe RawRectangle ToDXRectangle(this Rectangle rectangle)
        {
            return *(RawRectangle*)&rectangle;
        }

        internal static unsafe RawRectangleF ToDXRectangle(this RectangleF rectangle)
        {
            return *(RawRectangleF*)&rectangle;
        }

        internal static unsafe SharpDX.Size2 ToDXSize2(this Size2 size)
        {
            return new SharpDX.Size2(size.Width, size.Height);
        }

        internal static SharpDX.Size2F ToDXSize2F(this Size2F size)
        {
            return new SharpDX.Size2F(size.Width, size.Height);
        }
    }
}