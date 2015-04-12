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

using System.Collections.Generic;

#if DESKTOP
using System.Windows.Media.Media3D;
#endif

namespace FrozenSky
{
    public static class MathExtensions
    {
        public static bool IsEmpty(this Vector2 vector)
        {
            return vector.Equals(Vector2.Zero);
        }

#if DESKTOP
        public static Vector3 ToRKVector(this Vector3D wpfVector)
        {
            return new Vector3((float)wpfVector.X, (float)wpfVector.Y, (float)wpfVector.Z);
        }

        public static Vector3 ToRKVector(this Point3D wpfVector)
        {
            return new Vector3((float)wpfVector.X, (float)wpfVector.Y, (float)wpfVector.Z);
        }

        public static Vector3D ToWpfVector(this Point3D wpfPoint)
        {
            return new Vector3D(wpfPoint.X, wpfPoint.Y, wpfPoint.Z);
        }

        public static System.Windows.Media.Color ToWpfColor(this Color4 color)
        {
            return System.Windows.Media.Color.FromArgb(
                (byte)(color.Alpha * 255),
                (byte)(color.Red * 255),
                (byte)(color.Green * 255),
                (byte)(color.Blue * 255));
        }
#endif

        /// <summary>
        /// Gets all points contained in given line collection.
        /// </summary>
        /// <param name="lines">A list containing all lines.</param>
        public static IEnumerable<Vector3> GetAllPoints(this IEnumerable<Line> lines)
        {
            Vector3 lastVector = Vector3.MinValue;
            foreach (Line actLine in lines)
            {
                if (lastVector != actLine.StartPosition) { yield return actLine.StartPosition; }

                yield return actLine.EndPosition;

                lastVector = actLine.EndPosition;
            }
        }
    }
}
