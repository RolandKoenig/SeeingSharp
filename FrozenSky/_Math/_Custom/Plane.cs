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
namespace FrozenSky
{
    public partial struct Plane
    {
        /// <summary>
        /// Calculates the distance from this plane to the given point.
        /// </summary>
        /// <param name="point">The point to calculate the distance to.</param>
        public float Distance(ref Vector3 point)
        {
            float distance;
            Vector3.Dot(ref this.Normal, ref point, out distance);
            distance += this.D;

            return distance;
        }
    }
}
