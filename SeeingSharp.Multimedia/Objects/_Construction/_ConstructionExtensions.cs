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
using System.Numerics;
using System.Collections.Generic;

namespace SeeingSharp.Multimedia.Objects
{
    public static partial class ConstructionExtensions
    {
        /// <summary>
        /// Generates a bounding box out of given vertex structures.
        /// </summary>
        /// <param name="structures">The structures to buld the box for.</param>
        public static BoundingBox GenerateBoundingBox(this IEnumerable<VertexStructure> structures)
        {
            BoundingBox result = new BoundingBox();

            foreach (VertexStructure actStructure in structures)
            {
                BoundingBox actBoundingBox = actStructure.GenerateBoundingBox();
                result.MergeWith(actBoundingBox);
            }

            return result;
        }

        /// <summary>
        /// Calculates all normals in the given structures.
        /// </summary>
        /// <param name="structures">The structure where to calculate all normals.</param>
        public static void CalculateNormals(this IEnumerable<VertexStructure> structures)
        {
            foreach (VertexStructure actStructure in structures)
            {
                actStructure.CalculateNormalsFlat();
            }
        }

        /// <summary>
        /// Transforms positions and normals of all vertices using the given transform matrix
        /// </summary>
        /// <param name="transformMatrix"></param>
        public static void TransformVertices(this IEnumerable<VertexStructure> structures, Matrix4x4 matrix)
        {
            foreach (VertexStructure actStructure in structures)
            {
                actStructure.TransformVertices(matrix);
            }
        }

        /// <summary>
        /// Realigns all given structures to their center coordinate.
        /// </summary>
        /// <param name="structures">The structures to update.</param>
        public static void RealignToCenter(this IEnumerable<VertexStructure> structures)
        {
            BoundingBox fullBoundingBox = structures.GenerateBoundingBox();
            Vector3 fullCenter = fullBoundingBox.GetMiddleCenter();
            Vector3 targetCenter = new Vector3(0f, 0f, 0f);

            foreach (VertexStructure actStructure in structures)
            {
                BoundingBox currentBoundingBox = actStructure.GenerateBoundingBox();
                Vector3 currentCenter = currentBoundingBox.GetMiddleCenter();
                Vector3 moveToFullCenter = fullCenter - currentCenter;
                Vector3 moveToTargetCenter = moveToFullCenter - fullCenter;

                actStructure.UpdateVerticesUsingRelocationBy(moveToTargetCenter);
            }
        }

        public static void RealignToFloorCenter(this IEnumerable<VertexStructure> structures)
        {
            BoundingBox fullBoundingBox = structures.GenerateBoundingBox();
            Vector3 fullCenter = fullBoundingBox.GetMiddleCenter();
            Vector3 targetCenter = new Vector3(0f, fullBoundingBox.GetSize().Y / 2f, 0f);
            Vector3 moveVector = targetCenter - fullCenter;

            foreach (VertexStructure actStructure in structures)
            {
                //BoundingBox currentBoundingBox = actStructure.GenerateBoundingBox();
                //Vector3 currentCenter = currentBoundingBox.GetMiddleCenter();
                //Vector3 moveToFullCenter = fullCenter - currentCenter;
                //Vector3 moveToTargetCenter = moveToFullCenter + (targetCenter - fullCenter);

                actStructure.UpdateVerticesUsingRelocationBy(moveVector);
            }
        }

        /// <summary>
        /// Fits to centered cube.
        /// </summary>
        /// <param name="structures">The structures to perform the fit function on.</param>
        public static void FitToCenteredCube(this IEnumerable<VertexStructure> structures)
        {
            structures.FitToCenteredCuboid(1f, 1f, 1f, FitToCuboidMode.MaintainAspectRatio, SpacialOriginLocation.Center);
        }

        /// <summary>
        /// Fits to centered cube.
        /// </summary>
        /// <param name="structures">The structures to perform the fit function on.</param>
        /// <param name="cubeSideLength">Fixed cube side length for x, y and z.</param>
        public static void FitToCenteredCube(this IEnumerable<VertexStructure> structures, float cubeSideLength)
        {
            structures.FitToCenteredCuboid(cubeSideLength, cubeSideLength, cubeSideLength, FitToCuboidMode.MaintainAspectRatio, SpacialOriginLocation.Center);
        }

        public static void FitToCenteredCube(this IEnumerable<VertexStructure> structures, float cubeSideLength, FitToCuboidMode mode)
        {
            structures.FitToCenteredCuboid(cubeSideLength, cubeSideLength, cubeSideLength, mode, SpacialOriginLocation.Center);
        }

        public static void FitToCenteredCube(this IEnumerable<VertexStructure> structures, float cubeSideLength, FitToCuboidMode mode, SpacialOriginLocation fitOrigin)
        {
            structures.FitToCenteredCuboid(cubeSideLength, cubeSideLength, cubeSideLength, mode, fitOrigin);
        }

        public static void FitToCenteredCuboid(this IEnumerable<VertexStructure> structures, float cubeSideLengthX, float cubeSideLengthY, float cubeSideLengthZ, FitToCuboidMode fitMode, SpacialOriginLocation fitOrigin)
        {
            //Get whole bounding box
            BoundingBox wholeBoundingBox = structures.GenerateBoundingBox();
            Vector3 wholeBoundingBoxSize = wholeBoundingBox.GetSize();
            if (wholeBoundingBox.IsEmpty()) { return; }
            if (wholeBoundingBoxSize.X <= 0f) { return; }
            if (wholeBoundingBoxSize.Y <= 0f) { return; }
            if (wholeBoundingBoxSize.Z <= 0f) { return; }

            Vector3 targetCornerALocation = new Vector3(
                -wholeBoundingBoxSize.X / 2f,
                -wholeBoundingBoxSize.Y / 2f,
                -wholeBoundingBoxSize.Z / 2f);

            //Vector3 wholeRelocationVector = targetCornerALocation - wholeBoundingBox.CornerA;

            //Calculate resize factors
            float resizeFactorX = cubeSideLengthX / wholeBoundingBoxSize.X;
            float resizeFactorY = cubeSideLengthY / wholeBoundingBoxSize.Y;
            float resizeFactorZ = cubeSideLengthZ / wholeBoundingBoxSize.Z;
            if (fitMode == FitToCuboidMode.MaintainAspectRatio)
            {
                resizeFactorX = Math.Min(resizeFactorX, Math.Min(resizeFactorY, resizeFactorZ));
                resizeFactorY = resizeFactorX;
                resizeFactorZ = resizeFactorX;
            }

            targetCornerALocation.X = targetCornerALocation.X * resizeFactorX;
            targetCornerALocation.Y = targetCornerALocation.Y * resizeFactorY;
            targetCornerALocation.Z = targetCornerALocation.Z * resizeFactorZ;
            switch (fitOrigin)
            {
                case SpacialOriginLocation.LowerCenter:
                    targetCornerALocation.Y = 0f;
                    break;
            }

            //Transform each single structure
            foreach (VertexStructure actStructure in structures)
            {
                BoundingBox actPartBox = actStructure.GenerateBoundingBox();
                Vector3 localLocationOriginal = actPartBox.CornerA - wholeBoundingBox.CornerA;

                //Bring the structure to origin based location and then scale it
                actStructure.UpdateVerticesUsingRelocationBy(Vector3.Negate(actPartBox.CornerA));
                actStructure.UpdateVerticesUsingRelocationFunc((actPosition) => new Vector3(
                    actPosition.X * resizeFactorX,
                    actPosition.Y * resizeFactorY,
                    actPosition.Z * resizeFactorZ));

                Vector3 localLocation = new Vector3(
                    localLocationOriginal.X * resizeFactorX,
                    localLocationOriginal.Y * resizeFactorY,
                    localLocationOriginal.Z * resizeFactorZ);
                actStructure.UpdateVerticesUsingRelocationBy(targetCornerALocation + localLocation);
            }
        }
    }
}