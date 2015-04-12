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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Multimedia
{
    internal static class Extensions
    {
        internal static SharpDX.Color4 ToDXColor(this Color4 color)
        {
            return new SharpDX.Color4(color.Red, color.Green, color.Blue, color.Alpha);
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
