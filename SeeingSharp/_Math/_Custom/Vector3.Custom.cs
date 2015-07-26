//#region License information (SeeingSharp and all based games/applications)
///*
//    Seeing# and all games/applications distributed together with it. 
//    More info at 
//     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
//     - http://www.rolandk.de/wp (the autors homepage, german)
//    Copyright (C) 2015 Roland König (RolandK)

//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/.
//*/
//#endregion

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SeeingSharp
//{
//    public partial struct Vector3
//    {
//        public static readonly Vector3 MinValue = new Vector3(float.MinValue, float.MinValue, float.MinValue);
//        public static readonly Vector3 MaxValue = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

//        /// <summary>
//        /// Gets this vector with absolute x, y and z values.
//        /// </summary>
//        public Vector3 Abs()
//        {
//            return new Vector3(
//                Math.Abs(X),
//                Math.Abs(Y),
//                Math.Abs(Z));
//        }

//        /// <summary>
//        /// Transform this coordinate using the given transformation matrix.
//        /// </summary>
//        /// <param name="transform">The transformation matrix.</param>
//        public void TransformCoordinate(Matrix4x4 transform)
//        {
//            Vector3 transformed = Vector3.TransformCoordinate(this, transform);
//            this.X = transformed.X;
//            this.Y = transformed.Y;
//            this.Z = transformed.Z;
//        }

//        /// <summary>
//        /// Transform this normal using the given transformation matrix.
//        /// </summary>
//        /// <param name="transform">The transformation matrix.</param>
//        public void TransformNormal(Matrix4x4 transform)
//        {
//            Vector3 transformed = Vector3.TransformNormal(this, transform);
//            this.X = transformed.X;
//            this.Y = transformed.Y;
//            this.Z = transformed.Z;
//        }

//        /// <summary>
//        /// Converts the vector into a unit vector.
//        /// </summary>
//        /// <param name="length">The length value which is calculated during normalization.</param>
//        public void Normalize(out float length)
//        {
//            length = Length();
//            if (length > MathUtil.ZeroTolerance)
//            {
//                float inv = 1.0f / length;
//                X *= inv;
//                Y *= inv;
//                Z *= inv;
//            }
//        }

//        /// <summary>
//        /// Converts this vector to a vector containing horizontal and vertical rotation values.
//        /// </summary>
//        public Vector2 ToHVRotation()
//        {
//            Vector3 normal = Vector3.Normalize(this);

//            Vector2 result = new Vector2();
//            result.X = (float)Math.Atan2(normal.Z, normal.X);
//            result.Y = (float)Math.Atan2(normal.Y, new Vector2(normal.Z, normal.X).Length());
//            return result;
//        }

//        /// <summary>
//        /// Writes horizontal and vertical rotation values to given parameters.
//        /// </summary>
//        /// <param name="hRotation">Parameter for horizontal rotation.</param>
//        /// <param name="vRotation">Parameter for vertical rotation.</param>
//        public void ToHVRotation(out float hRotation, out float vRotation)
//        {
//            Vector3 normal = Vector3.Normalize(this);

//            hRotation = (float)Math.Atan2(normal.Z, normal.X);
//            vRotation = (float)Math.Atan2(normal.Y, new Vector2(normal.Z, normal.X).Length());
//        }

//        /// <summary>
//        /// Calculates the normal of the given triangle
//        /// </summary>
//        /// <param name="p0">First point of the triangle.</param>
//        /// <param name="p1">Second point of the triangle.</param>
//        /// <param name="p2">Third point of the triangle.</param>
//        public static Vector3 CalculateTriangleNormal(Vector3 p0, Vector3 p1, Vector3 p2)
//        {
//            return CalculateTriangleNormal(p0, p1, p2, true);
//        }

//        /// <summary>
//        /// Calculates the normal of the given triangle
//        /// </summary>
//        /// <param name="p0">First point of the triangle.</param>
//        /// <param name="p1">Second point of the triangle.</param>
//        /// <param name="p2">Third point of the triangle.</param>
//        /// <param name="doNormalize">Setting this parameter to false causes the result normal to be not normalized after calculation.</param>
//        public static Vector3 CalculateTriangleNormal(Vector3 p0, Vector3 p1, Vector3 p2, bool doNormalize)
//        {
//            Vector3 result = new Vector3();

//            // Calculation of the normal based on 'Mathematics for 3D Game Programming and Computer Graphics (Eric Lengyel, 2012)
//            //  Page 175: 7.7.1 Calculating Normal Vectors
//            //
//            //  We have two modes: Normalized and unnormalized form of the result
//            if (doNormalize)
//            {
//                Vector3 crossProductVector = Vector3.Cross(p1 - p0, p2 - p0);
//                result = crossProductVector / crossProductVector.Length();
//            }
//            else
//            {
//                result = Vector3.Cross(p1 - p0, p2 - p0);
//            }

//            return result;
//        }

//        /// <summary>
//        /// Is this vector empty?
//        /// </summary>
//        public bool IsEmpty()
//        {
//            return this.Equals(Vector3.Zero);
//        }

//        /// <summary>
//        /// Generates a normal out of given horizontal and vertical rotation.
//        /// </summary>
//        /// <param name="horizontalRotation">Horizontal rotation value.</param>
//        /// <param name="verticalRotation">Vertical rotation value.</param>
//        public static Vector3 NormalFromHVRotation(float horizontalRotation, float verticalRotation)
//        {
//            Vector3 result = Vector3.Zero;

//            //Generate vector
//            result.X = (float)(1f * Math.Cos(verticalRotation) * Math.Cos(horizontalRotation));
//            result.Y = (float)(1f * Math.Sin(verticalRotation));
//            result.Z = (float)(1f * Math.Cos(verticalRotation) * Math.Sin(horizontalRotation));

//            //Normalize the generated vector
//            result.Normalize();

//            return result;
//        }

//        /// <summary>
//        /// Generates a normal out of given horizontal and vertical rotation.
//        /// </summary>
//        /// <param name="rotation">Vector containing horizontal and vertical rotations.</param>
//        public static Vector3 NormalFromHVRotation(Vector2 rotation)
//        {
//            return NormalFromHVRotation(rotation.X, rotation.Y);
//        }

//        /// <summary>
//        /// Gets an average vector.
//        /// </summary>
//        public static Vector3 Average(params Vector3[] vectors)
//        {
//            if (vectors.Length == 0) { return Vector3.Zero; }
//            Vector3 result = Vector3.Sum(vectors);

//            result.X = result.X / (float)vectors.Length;
//            result.Y = result.Y / (float)vectors.Length;
//            result.Z = result.Z / (float)vectors.Length;

//            return result;
//        }

//        /// <summary>
//        /// Gets an average vector.
//        /// </summary>
//        public static Vector3 Average(List<Vector3> vectors)
//        {
//            if (vectors.Count == 0) { return Vector3.Zero; }
//            Vector3 result = Vector3.Sum(vectors);

//            result.X = result.X / (float)vectors.Count;
//            result.Y = result.Y / (float)vectors.Count;
//            result.Z = result.Z / (float)vectors.Count;

//            return result;
//        }

//        /// <summary>
//        /// Gets the a vector containing the sum of each given vector.
//        /// </summary>
//        /// <param name="vectors">The vectors to add one by one.</param>
//        public static Vector3 Sum(params Vector3[] vectors)
//        {
//            Vector3 result = Vector3.Zero;
//            for (int loop = 0; loop < vectors.Length; loop++)
//            {
//                result = result + vectors[loop];
//            }
//            return result;
//        }

//        /// <summary>
//        /// Gets the a vector containing the sum of each given vector.
//        /// </summary>
//        /// <param name="vectors">The vectors to add one by one.</param>
//        public static Vector3 Sum(List<Vector3> vectors)
//        {
//            Vector3 result = Vector3.Zero;
//            for (int loop = 0; loop < vectors.Count; loop++)
//            {
//                result = result + vectors[loop];
//            }
//            return result;
//        }

//#if DESKTOP
//        public System.Windows.Media.Media3D.Vector3D ToWpfVector()
//        {
//            return new System.Windows.Media.Media3D.Vector3D((double)this.X, (double)this.Y, (double)this.Z);
//        }
//#endif

//        public Vector2 XY
//        {
//            get { return new Vector2(this.X, this.Y); }
//        }
//    }
//}
