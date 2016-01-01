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

namespace SeeingSharp.Multimedia.Objects
{
    internal static class AssimpExtensions
    {
        internal static Vector3 ToSeeingSharpVector(this Assimp.Vector3D vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        internal static Vector2 ToSeeingSharpVector(this Assimp.Vector2D vector)
        {
            return new Vector2(vector.X, vector.Y);
        }

        internal static Color4 ToSeeingSharpColor(this Assimp.Color4D color)
        {
            return new Color4(color.R, color.G, color.B, color.A);
        }

        internal static Matrix4x4 ToSeeingSharpMatrix(this Assimp.Matrix4x4 matrix)
        {
            return new Matrix4x4(
                matrix.A1, matrix.A2, matrix.A3, matrix.A4,
                matrix.B1, matrix.B2, matrix.B3, matrix.B4,
                matrix.C1, matrix.C2, matrix.C3, matrix.C4,
                matrix.D1, matrix.D2, matrix.D3, matrix.D4);
        }

        internal static Quaternion ToSeeingSharpQuaternion(this Assimp.Quaternion quaternion)
        {
            return new Quaternion(
                quaternion.X, quaternion.Y, quaternion.Z,
                quaternion.W);
        }
    }
}
