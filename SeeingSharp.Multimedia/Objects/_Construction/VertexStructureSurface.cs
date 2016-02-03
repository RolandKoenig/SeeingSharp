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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Multimedia.Objects
{
    /// <summary>
    /// A set of triangles of a VertexStructure which share the 
    /// same material settings.
    /// </summary>
    public class VertexStructureSurface
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

        private VertexStructureSurface(VertexStructure owner, int triangleCapacity)
        {
            m_owner = owner;
            m_indices = new List<int>(triangleCapacity * 3);
            m_indexCollection = new IndexCollection(m_indices);
            m_triangleCollection = new TriangleCollection(m_indices, m_owner.VerticesInternal);
            m_materialProperties = new MaterialProperties();
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