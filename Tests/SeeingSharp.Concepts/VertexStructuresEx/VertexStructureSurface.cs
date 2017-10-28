#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
	Exception are projects where it is noted otherwhise.
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
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Concepts.VertexStructuresEx
{
    /// <summary>
    /// A set of triangles of a VertexStructure which share the 
    /// same material settings.
    /// </summary>
    public partial class VertexStructureSurface
    {
        #region Common
        private VertexStructure m_owner;
        #endregion

        #region Geometry information
        private List<int> m_indices;
        private IndexCollection m_indexCollection;
        private TriangleCollection m_triangleCollection;
        #endregion

        #region Material information
        private MaterialProperties m_materialProperties;
        private Dictionary<Type, object> m_materialPropertiesExtended;
        #endregion

        internal VertexStructureSurface(VertexStructure owner, int triangleCapacity)
        {
            m_owner = owner;
            m_indices = new List<int>(triangleCapacity * 3);
            m_indexCollection = new IndexCollection(m_indices);
            m_triangleCollection = new TriangleCollection(m_indices);
            m_materialProperties = new MaterialProperties();
        }

        /// <summary>
        /// Clones this object.
        /// </summary>
        public VertexStructureSurface Clone(
            VertexStructure newOwner, 
            bool copyGeometryData = true, int capacityMultiplier = 1,
            int baseIndex = 0)
        {
            newOwner.EnsureNotNull(nameof(newOwner));

            // Create new VertexStructure object
            int indexCount = m_indices.Count;
            VertexStructureSurface result = new VertexStructureSurface(newOwner, (indexCount / 3) * capacityMultiplier);

            // Copy geometry
            if (copyGeometryData)
            {
                for (int loop = 0; loop < indexCount; loop++)
                {
                    result.m_indices.Add(m_indices[loop] + baseIndex);
                }
            }

            // Copy metadata
            result.m_materialProperties = m_materialProperties.Clone();

            return result;
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

        ///// <summary>
        ///// Adds a triangle
        ///// </summary>
        ///// <param name="v1">First vertex</param>
        ///// <param name="v2">Second vertex</param>
        ///// <param name="v3">Third vertex</param>
        //public void AddTriangle(Vertex v1, Vertex v2, Vertex v3)
        //{
        //    m_indices.Add(m_owner.AddVertex(v1));
        //    m_indices.Add(m_owner.AddVertex(v2));
        //    m_indices.Add(m_owner.AddVertex(v3));
        //}

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

        ///// <summary>
        ///// Adds a triangle
        ///// </summary>
        ///// <param name="v1">First vertex</param>
        ///// <param name="v2">Second vertex</param>
        ///// <param name="v3">Third vertex</param>
        //public void AddTriangleAndCalculateNormalsFlat(Vertex v1, Vertex v2, Vertex v3)
        //{
        //    int index1 = m_owner.AddVertex(v1);
        //    int index2 = m_owner.AddVertex(v2);
        //    int index3 = m_owner.AddVertex(v3);

        //    AddTriangleAndCalculateNormalsFlat(index1, index2, index3);
        //}

        ///// <summary>
        ///// Adds the given polygon using the cutting ears algorythm for triangulation.
        ///// </summary>
        ///// <param name="vertices">The vertices to add.</param>
        //public void AddPolygonByCuttingEars(IEnumerable<Vertex> vertices)
        //{
        //    //Add vertices first
        //    List<int> indices = new List<int>();
        //    foreach (Vertex actVertex in vertices)
        //    {
        //        indices.Add(m_owner.AddVertex(actVertex));
        //    }

        //    //Calculate cutting ears
        //    AddPolygonByCuttingEars(indices);
        //}

        /// <summary>
        /// Adds the given polygon using the cutting ears algorythm for triangulation.
        /// </summary>
        /// <param name="indices">The indices of the polygon's edges.</param>
        /// <param name="twoSided">The indiexes for front- and backside?</param>
        public void AddPolygonByCuttingEars(IEnumerable<int> indices, bool twoSided = false)
        {
            AddPolygonByCuttingEarsInternal(new List<int>(indices), twoSided);
        }

        ///// <summary>
        ///// Adds the given polygon using the cutting ears algorythm for triangulation.
        ///// </summary>
        ///// <param name="vertices">The vertices to add.</param>
        ///// <param name = "twoSided" > The indiexes for front- and backside?</param>
        //public void AddPolygonByCuttingEarsAndCalculateNormals(IEnumerable<Vertex> vertices, bool twoSided = false)
        //{
        //    //Add vertices first
        //    List<int> indices = new List<int>();
        //    foreach (Vertex actVertex in vertices)
        //    {
        //        indices.Add(m_owner.AddVertex(actVertex));
        //    }

        //    //Calculate cutting ears and normals
        //    AddPolygonByCuttingEarsAndCalculateNormals(indices, twoSided);
        //}

        /// <summary>
        /// Adds the given polygon using the cutting ears algorythm for triangulation.
        /// </summary>
        /// <param name="indices">The indices of the polygon's edges.</param>
        /// <param name="twoSided">The indiexes for front- and backside?</param>
        public void AddPolygonByCuttingEarsAndCalculateNormals(IEnumerable<int> indices, bool twoSided)
        {
            //Add the triangles using cutting ears algorithm
            IEnumerable<int> addedIndices = AddPolygonByCuttingEarsInternal(new List<int>(indices), twoSided);

            //Calculate all normals
            IEnumerator<int> indexEnumerator = addedIndices.GetEnumerator();
            while (indexEnumerator.MoveNext())
            {
                int index1 = indexEnumerator.Current;
                int index2 = 0;
                int index3 = 0;

                if (indexEnumerator.MoveNext()) { index2 = indexEnumerator.Current; } else { break; }
                if (indexEnumerator.MoveNext()) { index3 = indexEnumerator.Current; } else { break; }

                CalculateNormalsFlat(new Triangle(index1, index2, index3));
            }
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
                GeometryData geo1 = m_owner.RawGeometry[actTriangle.Index1];
                GeometryData geo2 = m_owner.RawGeometry[actTriangle.Index2];
                GeometryData geo3 = m_owner.RawGeometry[actTriangle.Index3];

                //Get average values for current face
                Vector3 averageBinormal = Vector3.Normalize(Vector3Ex.Average(geo1.Binormal, geo2.Binormal, geo3.Binormal));
                Vector3 averagePosition = Vector3Ex.Average(geo1.Position, geo2.Position, geo3.Position);
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
            foreach (var actTriangle in this.Triangles)
            {
                //Get all vertices of current face
                var geo1 = m_owner.RawGeometry[actTriangle.Index1];
                var geo2 = m_owner.RawGeometry[actTriangle.Index2];
                var geo3 = m_owner.RawGeometry[actTriangle.Index3];

                //Get average values for current face
                Vector3 averageNormal = Vector3.Normalize(Vector3Ex.Average(geo1.Normal, geo2.Normal, geo3.Normal));
                Vector3 averagePosition = Vector3Ex.Average(geo1.Position, geo2.Position, geo3.Position);
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
            foreach (var actTriangle in this.Triangles)
            {
                //Get all vertices of current face
                var geo1 = m_owner.RawGeometry[actTriangle.Index1];
                var geo2 = m_owner.RawGeometry[actTriangle.Index2];
                var geo3 = m_owner.RawGeometry[actTriangle.Index3];

                //Get average values for current face
                Vector3 averageTangent = Vector3.Normalize(Vector3Ex.Average(geo1.Tangent, geo2.Tangent, geo3.Tangent));
                Vector3 averagePosition = Vector3Ex.Average(geo1.Position, geo2.Position, geo3.Position);
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
            foreach (var actGeo in m_owner.RawGeometry)
            {
                if (actGeo.Binormal.Length() > 0.1f)
                {
                    result.Add(actGeo.Position);
                    result.Add(actGeo.Position + actGeo.Binormal * 0.2f);
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
            foreach (var actGeo in m_owner.RawGeometry)
            {
                if (actGeo.Normal.Length() > 0.1f)
                {
                    result.Add(actGeo.Position);
                    result.Add(actGeo.Position + actGeo.Normal * 0.2f);
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
            foreach (var actGeo in m_owner.RawGeometry)
            {
                if (actGeo.Tangent.Length() > 0.1f)
                {
                    result.Add(actGeo.Position);
                    result.Add(actGeo.Position + actGeo.Tangent * 0.2f);
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
                var geo1 = m_owner.RawGeometry[actTriangle.Index1];
                var geo2 = m_owner.RawGeometry[actTriangle.Index2];
                var geo3 = m_owner.RawGeometry[actTriangle.Index3];

                //first line (c)
                result.Add(geo1.Position);
                result.Add(geo2.Position);

                //second line (a)
                result.Add(geo2.Position);
                result.Add(geo3.Position);

                //third line (b)
                result.Add(geo3.Position);
                result.Add(geo1.Position);
            }

            return result;
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

        private IEnumerable<int> AddPolygonByCuttingEarsInternal(IList<int> vertexIndices, bool twoSided)
        {
            //Get all coordinates
            Vector3[] coordinates = new Vector3[vertexIndices.Count];
            for (int loop = 0; loop < vertexIndices.Count; loop++)
            {
                coordinates[loop] = m_owner.RawGeometry[vertexIndices[loop]].Position;
            }

            //Triangulate all data
            Polygon polygon = new Polygon(coordinates);
            IEnumerable<int> triangleIndices = polygon.TriangulateUsingCuttingEars();
            if (triangleIndices == null) { throw new SeeingSharpException("Unable to triangulate given polygon!"); }

            //Add all triangle data
            IEnumerator<int> indexEnumerator = triangleIndices.GetEnumerator();
            while (indexEnumerator.MoveNext())
            {
                int index1 = indexEnumerator.Current;
                int index2 = 0;
                int index3 = 0;

                if (indexEnumerator.MoveNext()) { index2 = indexEnumerator.Current; } else { break; }
                if (indexEnumerator.MoveNext()) { index3 = indexEnumerator.Current; } else { break; }

                this.AddTriangle(vertexIndices[index3], vertexIndices[index2], vertexIndices[index1]);
                if(twoSided)
                {
                    this.AddTriangle(vertexIndices[index1], vertexIndices[index2], vertexIndices[index3]);
                }

            }

            //Return found indices
            return triangleIndices;
        }

        /// <summary>
        /// Toggles all vertices and indexes from left to right handed or right to left handed system.
        /// </summary>
        internal void ToggleCoordinateSystemInternal()
        {
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
        /// Gets an index array
        /// </summary>
        public int[] GetIndexArray()
        {
            return m_indices.ToArray();
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
            var v1 = m_owner.RawGeometry[actTriangle.Index1];
            var v2 = m_owner.RawGeometry[actTriangle.Index2];
            var v3 = m_owner.RawGeometry[actTriangle.Index3];

            Vector3 normal = Vector3Ex.CalculateTriangleNormal(v1.Position, v2.Position, v3.Position);

            v1 = v1.Copy(v1.Position, normal);
            v2 = v2.Copy(v2.Position, normal);
            v3 = v3.Copy(v3.Position, normal);

            m_owner.RawGeometry[actTriangle.Index1] = v1;
            m_owner.RawGeometry[actTriangle.Index2] = v2;
            m_owner.RawGeometry[actTriangle.Index3] = v3;
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
        /// Calculates tangents for all vectors.
        /// </summary>
        public void CalculateTangentsAndBinormals()
        {
            m_owner.SupportsRawTexture.EnsureTrue(nameof(m_owner.SupportsRawTexture));

            for (int loop = 0; loop < this.CountTriangles; loop += 1)
            {
                Triangle actTriangle = this.Triangles[loop];

                //Get all vertices of current face
                var geo1 = m_owner.RawGeometry[actTriangle.Index1];
                var geo2 = m_owner.RawGeometry[actTriangle.Index2];
                var geo3 = m_owner.RawGeometry[actTriangle.Index3];
                var tex1 = m_owner.RawTexture[actTriangle.Index1];
                var tex2 = m_owner.RawTexture[actTriangle.Index2];
                var tex3 = m_owner.RawTexture[actTriangle.Index3];

                // Perform some precalculations
                Vector2 w1 = tex1.TextCoord1;
                Vector2 w2 = tex2.TextCoord1;
                Vector2 w3 = tex3.TextCoord1;
                float x1 = geo2.Position.X - geo1.Position.X;
                float x2 = geo3.Position.X - geo1.Position.X;
                float y1 = geo2.Position.Y - geo1.Position.Y;
                float y2 = geo3.Position.Y - geo1.Position.Y;
                float z1 = geo2.Position.Z - geo1.Position.Z;
                float z2 = geo3.Position.Z - geo1.Position.Z;
                float s1 = w2.X - w1.X;
                float s2 = w3.X - w1.X;
                float t1 = w2.Y - w1.Y;
                float t2 = w3.Y - w1.Y;
                float r = 1f / (s1 * t2 - s2 * t1);
                Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                // Create the tangent vector (assumes that each vertex normal within the face are equal)
                Vector3 tangent = Vector3.Normalize(sdir - geo1.Normal * Vector3.Dot(geo1.Normal, sdir));

                // Create the binormal using the tangent
                float tangentDir = (Vector3.Dot(Vector3.Cross(geo1.Normal, sdir), tdir) >= 0.0f) ? 1f : -1f;
                Vector3 binormal = Vector3.Cross(geo1.Normal, tangent) * tangentDir;

                // Seting binormals and tangents to each vertex of current face
                geo1.Tangent = tangent;
                geo1.Binormal = binormal;
                geo2.Tangent = tangent;
                geo2.Binormal = binormal;
                geo3.Tangent = tangent;
                geo3.Binormal = binormal;

                // Overtake changes made in vertex structures
                m_owner.RawGeometry[actTriangle.Index1] = geo1;
                m_owner.RawGeometry[actTriangle.Index2] = geo2;
                m_owner.RawGeometry[actTriangle.Index3] = geo3;
            }
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
            internal set { m_materialProperties = value; }
        }

        /// <summary>
        /// Retrieves a collection of triangles
        /// </summary>
        public TriangleCollection Triangles
        {
            get { return m_triangleCollection; }
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
        internal int CountIndices
        {
            get { return m_indices.Count; }
        }

        internal List<int> IndicesInternal
        {
            get { return m_indices; }
        }

        /// <summary>
        /// Retrieves total count of all triangles within this structure
        /// </summary>
        public int CountTriangles
        {
            get { return m_indices.Count / 3; }
        }

        /// <summary>
        /// Gets or sets the resource source assembly.
        /// </summary>
        public Assembly ResourceSourceAssembly
        {
            get { return m_owner.ResourceSourceAssembly; }
        }

        /// <summary>
        /// Gets or sets the original source of this geometry.
        /// </summary>
        public ResourceLink ResourceLink
        {
            get { return m_owner.ResourceLink; }
        }

        public VertexStructure Owner
        {
            get { return m_owner; }
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

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        /// <summary>
        /// Contains all triangles of a VertexStructure object
        /// </summary>
        public class TriangleCollection : IEnumerable<Triangle>
        {
            private List<int> m_indices;

            /// <summary>
            ///
            /// </summary>
            internal TriangleCollection(List<int> indices)
            {
                m_indices = indices;
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
    }
}