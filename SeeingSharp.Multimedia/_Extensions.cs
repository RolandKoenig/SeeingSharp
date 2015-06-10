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
#endregion License information (SeeingSharp and all based games/applications)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        internal static SharpDX.Color4 ToDXColor(this Color4 color)
        {
            return new SharpDX.Color4(color.Red, color.Green, color.Blue, color.Alpha);
        }

        internal static SharpDX.Vector2 ToDXVector(this Vector2 vector)
        {
            return new SharpDX.Vector2(vector.X, vector.Y);
        }

        internal static SharpDX.Rectangle ToDXRectangle(this Rectangle rectangle)
        {
            return new SharpDX.Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        internal static SharpDX.RectangleF ToDXRectangle(this RectangleF rectangle)
        {
            return new SharpDX.RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        internal static SharpDX.Size2 ToDXSize2(this Size2 size)
        {
            return new SharpDX.Size2(size.Width, size.Height);
        }

        internal static SharpDX.Size2F ToDXSize2F(this Size2F size)
        {
            return new SharpDX.Size2F(size.Width, size.Height);
        }
    }
}