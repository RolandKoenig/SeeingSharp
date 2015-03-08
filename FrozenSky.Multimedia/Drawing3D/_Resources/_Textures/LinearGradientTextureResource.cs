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

#if DESKTOP
//Some namespace mappings
using GDI = System.Drawing;
using GDI2D = System.Drawing.Drawing2D;

namespace FrozenSky.Multimedia.Drawing3D
{
    public class LinearGradientTextureResource : DrawingBrushTextureResource
    {
        /// <summary>
        /// Creates a linear gradient texture.
        /// </summary>
        /// <param name="start">Starting color.</param>
        /// <param name="destination">Destination color.</param>
        /// <param name="gradientDirection">Direction of the gradient.</param>
        public LinearGradientTextureResource(
            Color4 start,
            Color4 destination,
            GradientDirection gradientDirection,
            int widht, int height)
            : base(
                new GDI2D.LinearGradientBrush(GetStartPoint(gradientDirection, widht, height), GetTargetPoint(gradientDirection, widht, height), start.ToGdiColor(), destination.ToGdiColor()),
                widht, height)
        {

        }

        /// <summary>
        /// Creates a linear gradient texture.
        /// </summary>
        /// <param name="start">Starting color.</param>
        /// <param name="destination">Destination color.</param>
        /// <param name="gradientDirection">Direction of the gradient.</param>
        public LinearGradientTextureResource(
            Color4 start,
            Color4 destination,
            GradientDirection gradientDirection)
            : base(
                new GDI2D.LinearGradientBrush(GetStartPoint(gradientDirection, 32, 32), GetTargetPoint(gradientDirection, 32, 32), start.ToGdiColor(), destination.ToGdiColor()),
                32, 32)
        {

        }

        /// <summary>
        /// Gets the start-point of the gradient.
        /// </summary>
        /// <param name="direction">Direction of the gradient.</param>
        private static GDI.Point GetStartPoint(GradientDirection direction, int width, int height)
        {
            switch (direction)
            {
                case GradientDirection.LeftToRight:
                    return new GDI.Point(0, 0);

                case GradientDirection.TopToBottom:
                    return new GDI.Point(0, 0);

                case GradientDirection.Directional:
                    return new GDI.Point(0, 0);
            }
            return new GDI.Point(0, 0);
        }

        /// <summary>
        /// Gets the target-point of the gradient.
        /// </summary>
        /// <param name="direction">Direction of the gradient.</param>
        private static GDI.Point GetTargetPoint(GradientDirection direction, int width, int height)
        {
            switch (direction)
            {
                case GradientDirection.LeftToRight:
                    return new GDI.Point(width, 0);

                case GradientDirection.TopToBottom:
                    return new GDI.Point(0, height);

                case GradientDirection.Directional:
                    return new GDI.Point(width, height);
            }
            return new GDI.Point(width, height);
        }
    }
}
#endif