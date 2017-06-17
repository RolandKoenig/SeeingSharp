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
using System.Runtime.InteropServices;
using System.Numerics;
using SeeingSharp.Checking;

namespace SeeingSharp.Concepts.VertexStructuresEx
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        private int m_vertexIndex;
        private VertexStructure m_hostStructure;

        /// <summary>
        /// Creates a new vertex
        /// </summary>
        internal Vertex(VertexStructure hostStructure, int vertexIndex)
        {
            hostStructure.EnsureNotNull(nameof(hostStructure));
            vertexIndex.EnsurePositive(nameof(vertexIndex));

            m_hostStructure = hostStructure;
            m_vertexIndex = vertexIndex;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (!this.IsValid) { return "Invalid vertex"; }

            var lstGeo = m_hostStructure.RawGeometry;
            return lstGeo[m_vertexIndex].ToString();
        }

        /// <summary>
        /// Copies this vertex and sets the new position
        /// </summary>
        public Vertex Copy(Vector3 newPosition)
        {
            Vertex result = m_hostStructure.AddVertex(
                indexToCloneFrom: m_vertexIndex);

            result.Position = newPosition; 

            return result;
        }

        /// <summary>
        /// Copies this vertex and changes the texture coordinate of the result.
        /// </summary>
        /// <param name="actTexCoord">The texture coordinate to be set.</param>
        public Vertex Copy(Vector2 actTexCoord)
        {
            Vertex result = this;
            result.TextCoord1 = actTexCoord;
            return result;
        }

        internal Vertex Copy(Color4 newColor)
        {
            Vertex result = this;
            result.Color = newColor;
            return result;
        }

        /// <summary>
        /// Copies this vertex and sets a new position and texture coordinate
        /// </summary>
        public Vertex Copy(Vector3 newPosition, Vector2 newTexCoord1)
        {
            Vertex result = this;
            result.Position = newPosition;
            result.TextCoord1 = newTexCoord1;
            return result;
        }

        /// <summary>
        /// Copies this vertex and sets a new position, normal and texture coordinate
        /// </summary>
        public Vertex Copy(Vector3 newPosition, Vector3 newNormal, Vector2 newTexCoord1)
        {
            Vertex result = this;
            result.Position = newPosition;
            result.Normal = newNormal;
            result.TextCoord1 = newTexCoord1;
            return result;
        }

        /// <summary>
        /// Copies this vertex and sets the new values
        /// </summary>
        public Vertex Copy(Vector3 newPosition, Vector3 newNormal)
        {
            Vertex result = this;
            result.Position = newPosition;
            result.Normal = newNormal;
            return result;
        }

        /// <summary>
        /// Is set to false when the vertex was deleted on the <see cref="VertexStructure"/>.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if(m_hostStructure == null) { return false; }

                var lstGeo = m_hostStructure.RawGeometry;
                return lstGeo.Count > m_vertexIndex;
            }
        }

        public VertexStructure HostStructure
        {
            get { return m_hostStructure; }
        }

        public int Index
        {
            get { return m_vertexIndex; }
        }

        /// <summary>
        /// Does this <see cref="Vertex"/> support animation data?
        /// </summary>
        public bool SupportsAnimation
        {
            get
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));

                return m_hostStructure.RawAnimation != null;
            }
        }

        /// <summary>
        /// Does this <see cref="Vertex"/> support texture data?
        /// </summary>
        public bool SupportsTexture
        {
            get
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));

                return m_hostStructure.RawTexture != null;
            }
        }

        /// <summary>
        /// Retrieves or sets geometry data
        /// </summary>
        public GeometryData Geometry
        {
            get
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));

                return m_hostStructure.RawGeometry[m_vertexIndex];
            }
            set
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));

                m_hostStructure.RawGeometry[m_vertexIndex] = value;
            }
        }

        /// <summary>
        /// Gets or sets all animation related data of the vertex.
        /// </summary>
        public AnimationData Animation
        {
            get
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));
                this.SupportsAnimation.EnsureTrue(nameof(this.SupportsAnimation));

                return m_hostStructure.RawAnimation[m_vertexIndex];
            }
            set
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));
                this.SupportsAnimation.EnsureTrue(nameof(this.SupportsAnimation));

                m_hostStructure.RawAnimation[m_vertexIndex] = value;
            }
        }

        /// <summary>
        /// Retrieves or sets texture data
        /// </summary>
        public TextureData Texture
        {
            get
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));
                this.SupportsTexture.EnsureTrue(nameof(this.SupportsTexture));

                return m_hostStructure.RawTexture[m_vertexIndex];
            }
            set
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));
                this.SupportsTexture.EnsureTrue(nameof(this.SupportsTexture));

                m_hostStructure.RawTexture[m_vertexIndex] = value;
            }
        }

        /// <summary>
        /// Gets or sets the position
        /// </summary>
        public Vector3 Position
        {
            get
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));

                return m_hostStructure.RawGeometry[m_vertexIndex].Position;
            }
            set
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));

                var rawGeometry = m_hostStructure.RawGeometry[m_vertexIndex];
                rawGeometry.Position = value;
                m_hostStructure.RawGeometry[m_vertexIndex] = rawGeometry;
            }
        }

        /// <summary>
        /// Gets or sets the normal
        /// </summary>
        public Vector3 Normal
        {
            get
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));

                return m_hostStructure.RawGeometry[m_vertexIndex].Normal;
            }
            set
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));

                var rawGeometry = m_hostStructure.RawGeometry[m_vertexIndex];
                rawGeometry.Normal = value;
                m_hostStructure.RawGeometry[m_vertexIndex] = rawGeometry;
            }
        }

        /// <summary>
        /// Gets or sets the tangent vector.
        /// </summary>
        public Vector3 Tangent
        {
            get
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));

                return m_hostStructure.RawGeometry[m_vertexIndex].Tangent;
            }
            set
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));

                var rawGeometry = m_hostStructure.RawGeometry[m_vertexIndex];
                rawGeometry.Tangent = value;
                m_hostStructure.RawGeometry[m_vertexIndex] = rawGeometry;
            }
        }

        /// <summary>
        /// Gets or sets the binormal vector.
        /// </summary>
        public Vector3 Binormal
        {
            get
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));

                return m_hostStructure.RawGeometry[m_vertexIndex].Binormal;
            }
            set
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));

                var rawGeometry = m_hostStructure.RawGeometry[m_vertexIndex];
                rawGeometry.Binormal = value;
                m_hostStructure.RawGeometry[m_vertexIndex] = rawGeometry;
            }
        }

        /// <summary>
        /// Gets or sets the texture coordinate
        /// </summary>
        public Vector2 TextCoord1
        {
            get
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));

                return m_hostStructure.RawTexture[m_vertexIndex].TextCoord1;
            }
            set
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));

                var rawTexture = m_hostStructure.RawTexture[m_vertexIndex];
                rawTexture.TextCoord1 = value;
                m_hostStructure.RawTexture[m_vertexIndex] = rawTexture;
            }
        }

        /// <summary>
        /// Gets or sets the texture factor.
        /// This value decides wether a texture is displayed on this vertex or not.
        /// A value greater or equal 0 will show the texture, all negatives will hide it.
        /// </summary>
        public float TextureFactor
        {
            get
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));

                return m_hostStructure.RawTexture[m_vertexIndex].TextureFactor;
            }
            set
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));

                var rawTexture = m_hostStructure.RawTexture[m_vertexIndex];
                rawTexture.TextureFactor = value;
                m_hostStructure.RawTexture[m_vertexIndex] = rawTexture;
            }
        }

        /// <summary>
        /// Gets or sets the diffuse color
        /// </summary>
        public Color4 Color
        {
            get
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));

                return m_hostStructure.RawGeometry[m_vertexIndex].Color;
            }
            set
            {
                this.IsValid.EnsureTrue(nameof(this.IsValid));

                var rawGeometry = m_hostStructure.RawGeometry[m_vertexIndex];
                rawGeometry.Color = value;
                m_hostStructure.RawGeometry[m_vertexIndex] = rawGeometry;
            }
        }
    }
}