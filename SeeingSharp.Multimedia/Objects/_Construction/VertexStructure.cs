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
using SeeingSharp.Util;
using SeeingSharp.Checking;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Reflection;

namespace SeeingSharp.Multimedia.Objects
{
    public partial class VertexStructure
    {
        private string m_name;
        private string m_description;
        private MaterialProperties m_materialProperties;
        private Dictionary<Type, object> m_materialPropertiesExtended;
        private List<int> m_indices;
        private List<Vertex> m_vertices;
        private IndexCollection m_indexCollection;
        private TriangleCollection m_triangleCollection;
        private VertexCollection m_vertexCollection;

        /// <summary>
        /// Creates a new Vertex structure object
        /// </summary>
        public VertexStructure()
            : this(512, 512)
        {
            
        }

        /// <summary>
        /// Creates a new Vertex structure object
        /// </summary>
        public VertexStructure(int verticesCapacity, int triangleCapacity)
        {
            m_materialProperties = new MaterialProperties();

            m_vertices = new List<Vertex>(verticesCapacity);
            m_indices = new List<int>(triangleCapacity * 3);

            m_vertexCollection = new VertexCollection(m_vertices);
            m_triangleCollection = new TriangleCollection(m_indices, m_vertices);
            m_indexCollection = new IndexCollection(m_indices);

            m_name = string.Empty;
            m_description = string.Empty;
        }

        /// <summary>
        /// Ensures that there is a vertex at the given index and returns it.
        /// </summary>
        /// <param name="index">The index to get the vertex from.</param>
        public Vertex EnsureVertexAt(int index)
        {
            while (m_vertices.Count <= index) { this.AddVertex(); }
            return m_vertices[index];
        }

        /// <summary>
        /// Generates a bounding box surrounding all given structures.
        /// </summary>
        /// <param name="structures">Structures to generate a bounding box for.</param>
        public static BoundingBox GenerateBoundingBox(params VertexStructure[] structures)
        {
            return structures.GenerateBoundingBox();
        }

        /// <summary>
        /// Gets a vector to the bottom center of given structures.
        /// </summary>
        public static Vector3 GetBottomCenter(VertexStructure[] structures)
        {
            BoundingBox box = GetBoundingBox(structures);
            return box.GetBottomCenter();
        }

        /// <summary>
        /// Gets a bounding box for given vertex structure array.
        /// </summary>
        /// <param name="structures">Array of structures.</param>
        public static BoundingBox GetBoundingBox(VertexStructure[] structures)
        {
            BoundingBox result = new BoundingBox();

            if (structures != null)
            {
                for (int loop = 0; loop < structures.Length; loop++)
                {
                    if (structures[loop] != null)
                    {
                        result.MergeWith(structures[loop].GenerateBoundingBox());
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a vector to the middle center of given structures.
        /// </summary>
        public static Vector3 GetMiddleCenter(VertexStructure[] structures)
        {
            BoundingBox box = GetBoundingBox(structures);
            return box.GetMiddleCenter();
        }

        /// <summary>
        /// Adds all triangles of the given VertexStructure to this one.
        /// </summary>
        /// <param name="otherStructure">The structure to add to this one.</param>
        public void AddStructure(VertexStructure otherStructure)
        {
            int baseIndex = (int)m_vertices.Count;

            m_vertices.AddRange(otherStructure.Vertices);
            for (int loopIndex = 0; loopIndex < otherStructure.m_indices.Count; loopIndex++)
            {
                m_indices.Add((int)(baseIndex + otherStructure.m_indices[loopIndex]));
            }
        }

        /// <summary>
        /// Adds the given polygon using the cutting ears algorythm for triangulation.
        /// </summary>
        /// <param name="vertices">The vertices to add.</param>
        public void AddPolygonByCuttingEars(IEnumerable<Vertex> vertices)
        {
            //Add vertices first
            List<int> indices = new List<int>();
            foreach (Vertex actVertex in vertices)
            {
                indices.Add(this.AddVertex(actVertex));
            }

            //Calculate cutting ears
            AddPolygonByCuttingEars(indices);
        }

        /// <summary>
        /// Adds the given polygon using the cutting ears algorythm for triangulation.
        /// </summary>
        /// <param name="indices">The indices of the polygon's edges.</param>
        public void AddPolygonByCuttingEars(IEnumerable<int> indices)
        {
            AddPolygonByCuttingEarsInternal(new List<int>(indices));
        }

        /// <summary>
        /// Adds the given polygon using the cutting ears algorythm for triangulation.
        /// </summary>
        /// <param name="vertices">The vertices to add.</param>
        public void AddPolygonByCuttingEarsAndCalculateNormals(IEnumerable<Vertex> vertices)
        {
            //Add vertices first
            List<int> indices = new List<int>();
            foreach (Vertex actVertex in vertices)
            {
                indices.Add(this.AddVertex(actVertex));
            }

            //Calculate cutting ears and normals
            AddPolygonByCuttingEarsAndCalculateNormals(indices);
        }

        /// <summary>
        /// Adds the given polygon using the cutting ears algorythm for triangulation.
        /// </summary>
        /// <param name="indices">The indices of the polygon's edges.</param>
        public void AddPolygonByCuttingEarsAndCalculateNormals(IEnumerable<int> indices)
        {
            //Add the triangles using cutting ears algorithm
            IEnumerable<int> addedIndices = AddPolygonByCuttingEarsInternal(new List<int>(indices));

            //Calculate all normals
            IEnumerator<int> indexEnumerator = addedIndices.GetEnumerator();
            while (indexEnumerator.MoveNext())
            {
                int index1 = indexEnumerator.Current;
                int index2 = 0;
                int index3 = 0;

                if (indexEnumerator.MoveNext()) { index2 = indexEnumerator.Current; } else { break; }
                if (indexEnumerator.MoveNext()) { index3 = indexEnumerator.Current; } else { break; }

                this.CalculateNormalsFlat(new Triangle(index1, index2, index3));
            }
        }

        /// <summary>
        /// Adds a triangle
        /// </summary>
        /// <param name="index1">Index of the first vertex</param>
        /// <param name="index2">Index of the second vertex</param>
        /// <param name="index3">Index of the third vertex</param>
        public void AddTriangle(int index1, int index2, int index3)
        {
            m_indices.Add(index1);
            m_indices.Add(index2);
            m_indices.Add(index3);
        }

        /// <summary>
        /// Adds a triangle
        /// </summary>
        /// <param name="v1">First vertex</param>
        /// <param name="v2">Second vertex</param>
        /// <param name="v3">Third vertex</param>
        public void AddTriangle(Vertex v1, Vertex v2, Vertex v3)
        {
            m_indices.Add(AddVertex(v1));
            m_indices.Add(AddVertex(v2));
            m_indices.Add(AddVertex(v3));
        }

        /// <summary>
        /// Adds a triangle
        /// </summary>
        /// <param name="v1">First vertex</param>
        /// <param name="v2">Second vertex</param>
        /// <param name="v3">Third vertex</param>
        public void AddTriangleAndCalculateNormalsFlat(Vertex v1, Vertex v2, Vertex v3)
        {
            int index1 = AddVertex(v1);
            int index2 = AddVertex(v2);
            int index3 = AddVertex(v3);

            AddTriangleAndCalculateNormalsFlat(index1, index2, index3);
        }

        /// <summary>
        /// Adds a triangle and calculates normals for it.
        /// </summary>
        /// <param name="index1">Index of the first vertex</param>
        /// <param name="index2">Index of the second vertex</param>
        /// <param name="index3">Index of the third vertex</param>
        public void AddTriangleAndCalculateNormalsFlat(int index1, int index2, int index3)
        {
            AddTriangle(index1, index2, index3);
            CalculateNormalsFlat(new Triangle(index1, index2, index3));
        }

        /// <summary>
        /// Adds a vertex to the structure
        /// </summary>
        public int AddVertex()
        {
            return AddVertex(Vertex.Empty);
        }

        /// <summary>
        /// Adds a vertex to the structure
        /// </summary>
        /// <param name="vertex"></param>
        public int AddVertex(Vertex vertex)
        {
            //Transform vertex on build-time
            if (m_buildTimeTransformEnabled)
            {
                if (m_buildTimeTransformFunc != null) { vertex = m_buildTimeTransformFunc(vertex); }
                else
                {
                    vertex.Position = Vector3.Transform(vertex.Position, m_buildTransformMatrix);
                    vertex.Normal = Vector3.TransformNormal(vertex.Normal, m_buildTransformMatrix);
                }
            }

            //Add the vertex and return the index
            m_vertices.Add(vertex);
            return (int)(m_vertices.Count - 1);
        }

        /// <summary>
        /// Builds a line list containing a line for each face binormal.
        /// </summary>
        public List<Vector3> BuildLineListForFaceBinormals()
        {
            List<Vector3> result = new List<Vector3>();

            //Generate all lines
            foreach (Triangle actTriangle in this.Triangles)
            {
                //Get all vertices of current face
                Vertex vertex1 = m_vertices[actTriangle.Index1];
                Vertex vertex2 = m_vertices[actTriangle.Index2];
                Vertex vertex3 = m_vertices[actTriangle.Index3];

                //Get average values for current face
                Vector3 averageBinormal = Vector3.Normalize(Vector3Ex.Average(vertex1.Binormal, vertex2.Binormal, vertex3.Binormal));
                Vector3 averagePosition = Vector3Ex.Average(vertex1.Position, vertex2.Position, vertex3.Position);
                averageBinormal *= 0.2f;

                //Generate a line
                if (averageBinormal.Length() > 0.1f)
                {
                    result.Add(averagePosition);
                    result.Add(averagePosition + averageBinormal);
                }
            }

            return result;
        }

        /// <summary>
        /// Builds a line list containing a line for each face normal.
        /// </summary>
        public List<Vector3> BuildLineListForFaceNormals()
        {
            List<Vector3> result = new List<Vector3>();

            //Generate all lines
            foreach (Triangle actTriangle in this.Triangles)
            {
                //Get all vertices of current face
                Vertex vertex1 = m_vertices[actTriangle.Index1];
                Vertex vertex2 = m_vertices[actTriangle.Index2];
                Vertex vertex3 = m_vertices[actTriangle.Index3];

                //Get average values for current face
                Vector3 averageNormal = Vector3.Normalize(Vector3Ex.Average(vertex1.Normal, vertex2.Normal, vertex3.Normal));
                Vector3 averagePosition = Vector3Ex.Average(vertex1.Position, vertex2.Position, vertex3.Position);
                averageNormal *= 0.2f;

                //Generate a line
                if (averageNormal.Length() > 0.1f)
                {
                    result.Add(averagePosition);
                    result.Add(averagePosition + averageNormal);
                }
            }

            return result;
        }

        /// <summary>
        /// Builds a line list containing a line for each face tangent.
        /// </summary>
        public List<Vector3> BuildLineListForFaceTangents()
        {
            List<Vector3> result = new List<Vector3>();

            //Generate all lines
            foreach (Triangle actTriangle in this.Triangles)
            {
                //Get all vertices of current face
                Vertex vertex1 = m_vertices[actTriangle.Index1];
                Vertex vertex2 = m_vertices[actTriangle.Index2];
                Vertex vertex3 = m_vertices[actTriangle.Index3];

                //Get average values for current face
                Vector3 averageTangent = Vector3.Normalize(Vector3Ex.Average(vertex1.Tangent, vertex2.Tangent, vertex3.Tangent));
                Vector3 averagePosition = Vector3Ex.Average(vertex1.Position, vertex2.Position, vertex3.Position);
                averageTangent *= 0.2f;

                //Generate a line
                if (averageTangent.Length() > 0.1f)
                {
                    result.Add(averagePosition);
                    result.Add(averagePosition + averageTangent);
                }
            }

            return result;
        }

        /// <summary>
        /// Builds a list list containing a list for each vertex binormal.
        /// </summary>
        public List<Vector3> BuildLineListForVertexBinormals()
        {
            List<Vector3> result = new List<Vector3>();

            //Generate all lines
            foreach (Vertex actVertex in m_vertices)
            {
                if (actVertex.Binormal.Length() > 0.1f)
                {
                    result.Add(actVertex.Position);
                    result.Add(actVertex.Position + actVertex.Binormal * 0.2f);
                }
            }

            return result;
        }

        /// <summary>
        /// Builds a list list containing a list for each vertex normal.
        /// </summary>
        public List<Vector3> BuildLineListForVertexNormals()
        {
            List<Vector3> result = new List<Vector3>();

            //Generate all lines
            foreach (Vertex actVertex in m_vertices)
            {
                if (actVertex.Normal.Length() > 0.1f)
                {
                    result.Add(actVertex.Position);
                    result.Add(actVertex.Position + actVertex.Normal * 0.2f);
                }
            }

            return result;
        }

        /// <summary>
        /// Builds a list list containing a list for each vertex tangent.
        /// </summary>
        public List<Vector3> BuildLineListForVertexTangents()
        {
            List<Vector3> result = new List<Vector3>();

            //Generate all lines
            foreach (Vertex actVertex in m_vertices)
            {
                if (actVertex.Tangent.Length() > 0.1f)
                {
                    result.Add(actVertex.Position);
                    result.Add(actVertex.Position + actVertex.Tangent * 0.2f);
                }
            }

            return result;
        }

        /// <summary>
        /// Build a line list containing all lines for wireframe display.
        /// </summary>
        public List<Vector3> BuildLineListForWireframeView()
        {
            List<Vector3> result = new List<Vector3>();

            //Generate all lines
            foreach (Triangle actTriangle in this.Triangles)
            {
                //Get all vertices of current face
                Vertex vertex1 = m_vertices[actTriangle.Index1];
                Vertex vertex2 = m_vertices[actTriangle.Index2];
                Vertex vertex3 = m_vertices[actTriangle.Index3];

                //first line (c)
                result.Add(vertex1.Position);
                result.Add(vertex2.Position);

                //second line (a)
                result.Add(vertex2.Position);
                result.Add(vertex3.Position);

                //third line (b)
                result.Add(vertex3.Position);
                result.Add(vertex1.Position);
            }

            return result;
        }

        /// <summary>
        /// Calculates tangents for all vectors.
        /// </summary>
        public void CalculateTangentsAndBinormals()
        {
            for (int loop = 0; loop < this.CountTriangles; loop += 1)
            {
                Triangle actTriangle = this.Triangles[loop];

                //Get all vertices of current face
                Vertex vertex1 = m_vertices[actTriangle.Index1];
                Vertex vertex2 = m_vertices[actTriangle.Index2];
                Vertex vertex3 = m_vertices[actTriangle.Index3];

                // Perform some precalculations
                Vector2 w1 = vertex1.TexCoord;
                Vector2 w2 = vertex2.TexCoord;
                Vector2 w3 = vertex3.TexCoord;
                float x1 = vertex2.Position.X - vertex1.Position.X;
                float x2 = vertex3.Position.X - vertex1.Position.X;
                float y1 = vertex2.Position.Y - vertex1.Position.Y;
                float y2 = vertex3.Position.Y - vertex1.Position.Y;
                float z1 = vertex2.Position.Z - vertex1.Position.Z;
                float z2 = vertex3.Position.Z - vertex1.Position.Z;
                float s1 = w2.X - w1.X;
                float s2 = w3.X - w1.X;
                float t1 = w2.Y - w1.Y;
                float t2 = w3.Y - w1.Y;
                float r = 1f / (s1 * t2 - s2 * t1);
                Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                // Create the tangent vector (assumes that each vertex normal within the face are equal)
                Vector3 tangent = Vector3.Normalize(sdir - vertex1.Normal * Vector3.Dot(vertex1.Normal, sdir));

                // Create the binormal using the tangent
                float tangentDir = (Vector3.Dot(Vector3.Cross(vertex1.Normal, sdir), tdir) >= 0.0f) ? 1f : -1f;
                Vector3 binormal = Vector3.Cross(vertex1.Normal, tangent) * tangentDir;

                // Seting binormals and tangents to each vertex of current face
                vertex1.Tangent = tangent;
                vertex1.Binormal = binormal;
                vertex2.Tangent = tangent;
                vertex2.Binormal = binormal;
                vertex3.Tangent = tangent;
                vertex3.Binormal = binormal;

                // Overtake changes made in vertex structures
                m_vertices[actTriangle.Index1] = vertex1;
                m_vertices[actTriangle.Index2] = vertex2;
                m_vertices[actTriangle.Index3] = vertex3;
            }
        }

        /// <summary>
        /// Recalculates all normals
        /// </summary>
        public void CalculateNormalsFlat()
        {
            foreach (Triangle actTriangle in m_triangleCollection)
            {
                CalculateNormalsFlat(actTriangle);
            }
        }

        /// <summary>
        /// Calculates normals for the given treangle.
        /// </summary>
        /// <param name="actTriangle">The triangle for which to calculate the normal (flat).</param>
        public void CalculateNormalsFlat(Triangle actTriangle)
        {
            Vertex v1 = m_vertices[actTriangle.Index1];
            Vertex v2 = m_vertices[actTriangle.Index2];
            Vertex v3 = m_vertices[actTriangle.Index3];

            Vector3 normal = Vector3Ex.CalculateTriangleNormal(v1.Geometry.Position, v2.Geometry.Position, v3.Geometry.Position);

            v1 = v1.Copy(v1.Geometry.Position, normal);
            v2 = v2.Copy(v2.Geometry.Position, normal);
            v3 = v3.Copy(v3.Geometry.Position, normal);

            m_vertices[actTriangle.Index1] = v1;
            m_vertices[actTriangle.Index2] = v2;
            m_vertices[actTriangle.Index3] = v3;
        }

        /// <summary>
        /// Calculates normals for the given treangle.
        /// </summary>
        /// <param name="countTriangles">Total count of triangles.</param>
        /// <param name="startTriangleIndex">The triangle on which to start.</param>
        public void CalculateNormalsFlat(int startTriangleIndex, int countTriangles)
        {
            int startIndex = startTriangleIndex * 3;
            int indexCount = countTriangles * 3;

            if (startIndex < 0) { throw new ArgumentException("startTriangleIndex"); }
            if (startIndex >= m_indices.Count) { throw new ArgumentException("startTriangleIndex"); }
            if (startIndex + indexCount > m_indices.Count) { throw new ArgumentException("countTriangles"); }

            for (int loop = 0; loop < indexCount; loop += 3)
            {
                CalculateNormalsFlat(new Triangle(
                    m_indices[(int)(startIndex + loop)],
                    m_indices[(int)(startIndex + loop + 1)],
                    m_indices[(int)(startIndex + loop + 2)]));
            }
        }

        /// <summary>
        /// Calculates normals for all triangles of this structure.
        /// </summary>
        public void CalculateNormals()
        {
            CalculateNormals(0, m_vertices.Count);
        }

        /// <summary>
        /// Calculates normals for all triangles specifyed by the given parameters.
        /// </summary>
        /// <param name="startVertex">The vertex index on which to start.</param>
        /// <param name="vertexCount">Total count of vertices to be updated.</param>
        public void CalculateNormals(int startVertex, int vertexCount)
        {
            if ((startVertex < 0) || (startVertex >= m_vertices.Count)) { throw new ArgumentException("startVertex"); }
            if (vertexCount + startVertex > m_vertices.Count) { throw new ArgumentException("vertexCount"); }

            int triangleCount = m_indices.Count / 3;
            for (int actVertexIndex = startVertex; actVertexIndex < startVertex + vertexCount; actVertexIndex++)
            {
                // Find all triangles connected to this vertex and get normals from them
                Vector3 finalNormalHelper = Vector3.Zero;
                Vector3 finalNormalHelper2 = Vector3.Zero;
                int normalCount = 0;
                for (int loopTriangle = 0; loopTriangle < triangleCount; loopTriangle++)
                {
                    int triangleStartIndex = loopTriangle * 3;
                    if ((m_indices[triangleStartIndex] == actVertexIndex) ||
                        (m_indices[triangleStartIndex + 1 ] == actVertexIndex) ||
                        (m_indices[triangleStartIndex + 2] == actVertexIndex))
                    {
                        Vertex v1 = m_vertices[m_indices[triangleStartIndex]];
                        Vertex v2 = m_vertices[m_indices[triangleStartIndex + 1]];
                        Vertex v3 = m_vertices[m_indices[triangleStartIndex + 2]];

                        finalNormalHelper += Vector3Ex.CalculateTriangleNormal(v1.Geometry.Position, v2.Geometry.Position, v3.Geometry.Position, false);

                        normalCount++;
                    }
                }

                // Calculate final normal
                if (normalCount > 0)
                {
                    Vertex actVertex = m_vertices[actVertexIndex];
                    actVertex.Normal = finalNormalHelper / finalNormalHelper.Length();
                    m_vertices[actVertexIndex] = actVertex;
                    normalCount = 0;
                }
            }
        }

        /// <summary>
        /// Clones this object
        /// </summary>
        public VertexStructure Clone(bool copyGeometryData = true, int capacityMultiplier = 1)
        {
            capacityMultiplier.EnsurePositiveAndNotZero(nameof(capacityMultiplier));

            // Create new VertexStructure object
            int vertexCount = m_vertices.Count;
            int indexCount = m_indices.Count;
            VertexStructure result = new VertexStructure(
                vertexCount * capacityMultiplier, 
                (indexCount / 3) * capacityMultiplier);

            // Copy geometry
            if (copyGeometryData)
            {
                for (int loop = 0; loop < vertexCount; loop++)
                {
                    result.m_vertices.Add(m_vertices[loop]);
                }
                for (int loop = 0; loop < indexCount; loop++)
                {
                    result.m_indices.Add(m_indices[loop]);
                }
            }

            // Copy metadata
            result.m_materialProperties = m_materialProperties.Clone();
            result.m_description = m_description;
            result.m_name = m_name;

            return result;
        }

        /// <summary>
        /// Generates a boundbox around this structure
        /// </summary>
        public BoundingBox GenerateBoundingBox()
        {
            Vector3 maximum = Vector3Ex.MinValue;
            Vector3 minimum = Vector3Ex.MaxValue;

            foreach (Vertex actVertex in m_vertices)
            {
                Vector3 actPosition = actVertex.Position;

                //Update minimum vector
                if (actPosition.X < minimum.X) { minimum.X = actPosition.X; }
                if (actPosition.Y < minimum.Y) { minimum.Y = actPosition.Y; }
                if (actPosition.Z < minimum.Z) { minimum.Z = actPosition.Z; }

                //Update maximum vector
                if (actPosition.X > maximum.X) { maximum.X = actPosition.X; }
                if (actPosition.Y > maximum.Y) { maximum.Y = actPosition.Y; }
                if (actPosition.Z > maximum.Z) { maximum.Z = actPosition.Z; }
            }

            return new BoundingBox(minimum, maximum);
        }

        /// <summary>
        /// Changes the color on each vertex.
        /// </summary>
        /// <param name="Color4">The new color.</param>
        public void SetColorOnEachVertex(Color4 Color4)
        {
            for (int loop = 0; loop < m_vertices.Count; loop++)
            {
                m_vertices[loop] = m_vertices[loop].Copy(Color4);
            }
        }

        /// <summary>
        /// Toggles all vertices and indexes from left to right handed or right to left handed system.
        /// </summary>
        public void ToggleCoordinateSystem()
        {
            //Calculate the center coordinate of this structure
            BoundingBox boundingBox = this.GenerateBoundingBox();
            Vector3 centerCoord = boundingBox.GetMiddleCenter();

            //Update each vertex coordinate
            this.UpdateVerticesUsingRelocationFunc((givenVector) =>
            {
                return new Vector3(
                    givenVector.X, givenVector.Y,
                    centerCoord.Z + (centerCoord.Z - givenVector.Z));
            });

            //Now update all triangle indices
            for (int loopTriangle = 0; loopTriangle + 3 <= m_indices.Count; loopTriangle += 3)
            {
                int index1 = m_indices[loopTriangle];
                int index2 = m_indices[loopTriangle + 1];
                int index3 = m_indices[loopTriangle + 2];
                m_indices[loopTriangle] = index3;
                m_indices[loopTriangle + 1] = index2;
                m_indices[loopTriangle + 2] = index1;
            }
        }

        /// <summary>
        /// Gets extended material properties of the given type.
        /// </summary>
        /// <typeparam name="T">The type of properties to get.</typeparam>
        public T GetExtendedMaterialProperties<T>()
            where T : class
        {
            if (m_materialPropertiesExtended == null) { return null; }

            Type propertiesType = typeof(T);
            if (m_materialPropertiesExtended.ContainsKey(propertiesType)) { return m_materialPropertiesExtended[propertiesType] as T; }

            return null;
        }

        /// <summary>
        /// Gets an index array
        /// </summary>
        public int[] GetIndexArray()
        {
            return m_indices.ToArray();
        }

        /// <summary>
        /// Sets the extended material properties.
        /// </summary>
        /// <typeparam name="T">The type of properties to set.</typeparam>
        /// <param name="properties">The properties to set.</param>
        public void SetExtendedMaterialProperties<T>(T properties)
            where T : class
        {
            if (m_materialPropertiesExtended == null) { m_materialPropertiesExtended = new Dictionary<Type, object>(); }

            if (properties == null)
            {
                Type propertiesType = typeof(T);
                if (m_materialPropertiesExtended.ContainsKey(propertiesType)) { m_materialPropertiesExtended.Remove(propertiesType); }
                if (m_materialPropertiesExtended.Count == 0) { m_materialPropertiesExtended = null; }
            }
            else
            {
                m_materialPropertiesExtended[typeof(T)] = properties;
            }
        }

        /// <summary>
        /// Transforms positions and normals of all vertices using the given transform matrix
        /// </summary>
        /// <param name="transformMatrix"></param>
        public void TransformVertices(Matrix4x4 transformMatrix)
        {
            int length = m_vertices.Count;
            for (int loop = 0; loop < length; loop++)
            {
                m_vertices[loop] = m_vertices[loop].Copy(
                    Vector3.Transform(m_vertices[loop].Position, transformMatrix),
                    Vector3.TransformNormal(m_vertices[loop].Normal, transformMatrix));
            }
        }

        /// <summary>
        /// Gets an array with this object as a single item.
        /// </summary>
        public VertexStructure[] ToSingleItemArray()
        {
            return new VertexStructure[] { this };
        }

        /// <summary>
        /// Relocates all vertices by the given vector
        /// </summary>
        public void UpdateVerticesUsingRelocationBy(Vector3 relocateVector)
        {
            int length = m_vertices.Count;
            for (int loop = 0; loop < length; loop++)
            {
                m_vertices[loop] = m_vertices[loop].Copy(Vector3.Add(m_vertices[loop].Geometry.Position, relocateVector));
            }
        }

        /// <summary>
        /// Relocates all vertices by the given relocation function (executed for each position vector).
        /// </summary>
        /// <param name="calculatePositionFunc">The function to be applied to each coordinate.</param>
        public void UpdateVerticesUsingRelocationFunc(Func<Vector3, Vector3> calculatePositionFunc)
        {
            int length = m_vertices.Count;
            for (int loop = 0; loop < length; loop++)
            {
                m_vertices[loop] = m_vertices[loop].Copy(calculatePositionFunc(m_vertexCollection[loop].Position));
            }
        }

        private IEnumerable<int> AddPolygonByCuttingEarsInternal(IList<int> vertexIndices)
        {
            //Get all coordinates
            Vector3[] coordinates = new Vector3[vertexIndices.Count];
            for (int loop = 0; loop < vertexIndices.Count; loop++)
            {
                coordinates[loop] = m_vertices[loop].Position;
            }

            //Triangulate all data
            Polygon polygon = new Polygon(coordinates);
            IEnumerable<int> triangleIndices = polygon.TriangulateUsingCuttingEars();
            if (triangleIndices == null) { throw new SeeingSharpGraphicsException("Unable to triangulate given polygon!"); }

            //Add all triangle data
            IEnumerator<int> indexEnumerator = triangleIndices.GetEnumerator();
            while (indexEnumerator.MoveNext())
            {
                int index1 = indexEnumerator.Current;
                int index2 = 0;
                int index3 = 0;

                if (indexEnumerator.MoveNext()) { index2 = indexEnumerator.Current; } else { break; }
                if (indexEnumerator.MoveNext()) { index3 = indexEnumerator.Current; } else { break; }

                this.AddTriangle(index3, index2, index1);
                this.AddTriangle(index1, index2, index3);
            }

            //Return found indices
            return triangleIndices;
        }

        /// <summary>
        /// Retrieves total count of all triangles within this structure
        /// </summary>
        public int CountTriangles
        {
            get { return m_indices.Count / 3; }
        }

        /// <summary>
        /// Retrieves total count of all vertices within this structure
        /// </summary>
        public int CountVertices
        {
            get { return m_vertices.Count; }
        }

        /// <summary>
        /// A short description for the use of this structure
        /// </summary>
        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        /// <summary>
        /// Is this structure empty?
        /// </summary>
        public bool IsEmpty
        {
            get { return (m_vertices.Count == 0) && (m_indices.Count == 0); }
        }

        /// <summary>
        /// Gets the name of the material.
        /// </summary>
        public NamedOrGenericKey Material
        {
            get { return m_materialProperties.Key; }
            set { m_materialProperties.Key = value; }
        }

        /// <summary>
        /// Gets or sets the diffuse color of the material.
        /// </summary>
        public Color4 DiffuseColor
        {
            get { return m_materialProperties.DiffuseColor; }
            set { m_materialProperties.DiffuseColor = value; }
        }

        /// <summary>
        /// Gets or sets the resource source assembly.
        /// </summary>
        public Assembly ResourceSourceAssembly
        {
            get { return m_materialProperties.ResourceSourceAssembly; }
            set { m_materialProperties.ResourceSourceAssembly = value; }
        }
    
        /// <summary>
        /// Gets or sets the original source of this geometry.
        /// </summary>
        public ResourceLink ResourceLink
        {
            get { return m_materialProperties.ResourceLink; }
            set { m_materialProperties.ResourceLink = value; }
        }

        /// <summary>
        /// Gets or sets the name of the texture (used for the NamedOrGenericKey structure behind).
        /// </summary>
        public string TextureName
        {
            get { return m_materialProperties.TextureKey.NameKey; }
            set 
            {
                if (string.IsNullOrEmpty(value)) { m_materialProperties.TextureKey = NamedOrGenericKey.Empty; }
                else { m_materialProperties.TextureKey = new NamedOrGenericKey(value); }
            }
        }

        /// <summary>
        /// Gets or sets the key of the texture.
        /// </summary>
        public NamedOrGenericKey TextureKey
        {
            get { return m_materialProperties.TextureKey; }
            set { m_materialProperties.TextureKey = value; }
        }

        /// <summary>
        /// Gets or sets the material properties object.
        /// </summary>
        public MaterialProperties MaterialProperties
        {
            get { return m_materialProperties; }
        }

        /// <summary>
        /// The name of this structure
        /// </summary>
        public string Name
        {
            get { return m_name; }
            set
            {
                m_name = value;
                if (m_name == null) { m_name = string.Empty; }
            }
        }

        /// <summary>
        /// Retrieves a collection of triangles
        /// </summary>
        public TriangleCollection Triangles
        {
            get { return m_triangleCollection; }
        }

        /// <summary>
        /// Retrieves a collection of vertices
        /// </summary>
        public VertexCollection Vertices
        {
            get { return m_vertexCollection; }
        }

        /// <summary>
        /// Gets a collection containing all indices.
        /// </summary>
        public IndexCollection Indices
        {
            get { return m_indexCollection; }
        }

        /// <summary>
        /// Retrieves total count of all indexes within this structure
        /// </summary>
        internal int CountIndexes
        {
            get { return m_indices.Count; }
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        /// <summary>
        /// Contains all triangles of a VertexStructure object
        /// </summary>
        public class TriangleCollection : IEnumerable<Triangle>
        {
            private List<int> m_indices;
            private List<Vertex> m_vertices;

            /// <summary>
            ///
            /// </summary>
            internal TriangleCollection(List<int> indices, List<Vertex> vertices)
            {
                m_indices = indices;
                m_vertices = vertices;
            }

            /// <summary>
            ///
            /// </summary>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new Enumerator(m_indices);
            }

            /// <summary>
            /// Adds a treangle to this vertex structure
            /// </summary>
            /// <param name="index1">Index of the first vertex</param>
            /// <param name="index2">Index of the second vertex</param>
            /// <param name="index3">Index of the third vertex</param>
            public int Add(int index1, int index2, int index3)
            {
                int result = m_indices.Count / 3;

                m_indices.Add(index1);
                m_indices.Add(index2);
                m_indices.Add(index3);

                return result;
            }

            /// <summary>
            /// Adds a treangle to this vertex structure
            /// </summary>
            /// <param name="triangle"></param>
            public int Add(Triangle triangle)
            {
                return this.Add(triangle.Index1, triangle.Index2, triangle.Index3);
            }

            /// <summary>
            ///
            /// </summary>
            public IEnumerator<Triangle> GetEnumerator()
            {
                return new Enumerator(m_indices);
            }

            /// <summary>
            /// Gets an array containing all indices
            /// </summary>
            public int[] ToIndexArray()
            {
                return m_indices.ToArray();
            }

            /// <summary>
            /// Gets an array containing all indices
            /// </summary>
            public int[] ToIndexArray(int baseIndex)
            {
                int[] result = m_indices.ToArray();
                for (int loop = 0; loop < result.Length; loop++)
                {
                    result[loop] = (int)(result[loop] + baseIndex);
                }
                return result;
            }

            /// <summary>
            /// Retrieves the triangle at the given index
            /// </summary>
            public Triangle this[int index]
            {
                get
                {
                    int startIndex = index * 3;
                    return new Triangle(m_indices[startIndex], m_indices[startIndex + 1], m_indices[startIndex + 2]);
                }
            }

            //*****************************************************************
            //*****************************************************************
            //*****************************************************************
            /// <summary>
            /// Enumerator of TriangleCollection
            /// </summary>
            private class Enumerator : IEnumerator<Triangle>
            {
                private List<int> m_indices;
                private int m_maxIndex;
                private int m_startIndex;

                /// <summary>
                ///
                /// </summary>
                public Enumerator(List<int> indices)
                {
                    m_startIndex = -3;
                    m_maxIndex = indices.Count - 3;
                    m_indices = indices;
                }

                /// <summary>
                ///
                /// </summary>
                public void Dispose()
                {
                    m_startIndex = -3;
                    m_indices = null;
                }

                /// <summary>
                ///
                /// </summary>
                public bool MoveNext()
                {
                    m_startIndex += 3;
                    return m_startIndex <= m_maxIndex;
                }

                /// <summary>
                ///
                /// </summary>
                public void Reset()
                {
                    m_startIndex = -3;
                    m_maxIndex = m_indices.Count - 3;
                }

                /// <summary>
                ///
                /// </summary>
                object System.Collections.IEnumerator.Current
                {
                    get { return new Triangle(m_indices[m_startIndex], m_indices[m_startIndex + 1], m_indices[m_startIndex + 2]); }
                }

                /// <summary>
                ///
                /// </summary>
                public Triangle Current
                {
                    get { return new Triangle(m_indices[m_startIndex], m_indices[m_startIndex + 1], m_indices[m_startIndex + 2]); }
                }
            }
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        /// <summary>
        /// Contains all indices of a VertexStructure object.
        /// </summary>
        public class IndexCollection : IEnumerable<int>
        {
            private List<int> m_indices;

            internal IndexCollection(List<int> indices)
            {
                m_indices = indices;
            }

            /// <summary>
            ///
            /// </summary>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return m_indices.GetEnumerator();
            }

            /// <summary>
            ///
            /// </summary>
            public IEnumerator<int> GetEnumerator()
            {
                return m_indices.GetEnumerator();
            }

            /// <summary>
            /// Returns the index at ghe given index
            /// </summary>
            public int this[int index]
            {
                get { return m_indices[index]; }
            }
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        /// <summary>
        /// Contains all vertices of a VertexStructure object
        /// </summary>
        public class VertexCollection : IEnumerable<Vertex>
        {
            private List<Vertex> m_vertices;

            /// <summary>
            ///
            /// </summary>
            internal VertexCollection(List<Vertex> vertices)
            {
                m_vertices = vertices;
            }

            /// <summary>
            ///
            /// </summary>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return m_vertices.GetEnumerator();
            }

            /// <summary>
            /// Adds a vertex to the structure
            /// </summary>
            public void Add(Vertex vertex)
            {
                m_vertices.Add(vertex);
            }

            /// <summary>
            ///
            /// </summary>
            public IEnumerator<Vertex> GetEnumerator()
            {
                return m_vertices.GetEnumerator();
            }

            /// <summary>
            /// Returns the vertex at ghe given index
            /// </summary>
            public Vertex this[int index]
            {
                get { return m_vertices[index]; }
                internal set { m_vertices[index] = value; }
            }
        }
    }
}