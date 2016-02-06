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

namespace SeeingSharp.Multimedia.Objects
{
    public class GenericObjectType : ObjectType
    {
        private VertexStructure[] m_vertexStructures;
        private VertexStructure[] m_vertexStructuresLowDetail;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericObjectType"/> class.
        /// </summary>
        /// <param name="vertexStructures">The vertex structures.</param>
        public GenericObjectType(VertexStructure[] vertexStructures)
        {
            m_vertexStructures = vertexStructures;
            m_vertexStructuresLowDetail = vertexStructures;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericObjectType"/> class.
        /// </summary>
        /// <param name="vertexStructures">The vertex structures.</param>
        /// <param name="vertexStructuresLowDetail">The vertex structures for low detail level.</param>
        public GenericObjectType(VertexStructure[] vertexStructures, VertexStructure[] vertexStructuresLowDetail)
        {
            m_vertexStructures = vertexStructures;
            m_vertexStructuresLowDetail = vertexStructuresLowDetail;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericObjectType"/> class.
        /// </summary>
        /// <param name="vertexStructure">The vertex structure.</param>
        public GenericObjectType(VertexStructure vertexStructure)
            : this(new VertexStructure[] { vertexStructure })
        {

        }

        /// <summary>
        /// Builds the structure.
        /// </summary>
        /// <param name="buildOptions">Some generic options for structure building</param>
        public override VertexStructure[] BuildStructure(StructureBuildOptions buildOptions)
        {
            if (buildOptions.IsHighDetail) { return m_vertexStructures; }
            else { return m_vertexStructuresLowDetail; }
        }

        /// <summary>
        /// Applies the given material to all contained vertex structures.
        /// </summary>
        /// <param name="materialToApply">The materials to apply.</param>
        public void ApplyMaterialForAll(NamedOrGenericKey materialToApply)
        {
            for (int loop = 0; loop < m_vertexStructures.Length; loop++)
            {
                foreach (VertexStructureSurface actSurface in m_vertexStructures[loop].Surfaces)
                {
                    actSurface.Material = materialToApply;
                }
            }

            for (int loop = 0; loop < m_vertexStructuresLowDetail.Length; loop++)
            {
                foreach (VertexStructureSurface actSurface in m_vertexStructuresLowDetail[loop].Surfaces)
                {
                    actSurface.Material = materialToApply;
                }
            }
        }

        /// <summary>
        /// Converts all occurrences of the given material to another material.
        /// </summary>
        /// <param name="materialNameOld">The material to be converted.</param>
        /// <param name="materialNameNew">The new material to be converted to.</param>
        public void ConvertMaterial(NamedOrGenericKey materialNameOld, NamedOrGenericKey materialNameNew)
        {
            for (int loop = 0; loop < m_vertexStructures.Length; loop++)
            {
                foreach (VertexStructureSurface actSurface in m_vertexStructures[loop].Surfaces)
                {
                    if (actSurface.Material == materialNameOld)
                    {
                        actSurface.Material = materialNameNew;
                    }
                }
            }

            for (int loop = 0; loop < m_vertexStructuresLowDetail.Length; loop++)
            {
                foreach (VertexStructureSurface actSurface in m_vertexStructuresLowDetail[loop].Surfaces)
                {
                    if (actSurface.Material == materialNameOld)
                    {
                        actSurface.Material = materialNameNew;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the array containing all loaded vertex structures.
        /// </summary>
        public VertexStructure[] VertexStructures
        {
            get { return m_vertexStructures; }
        }

        /// <summary>
        /// Gets an array containing all loaded vertex structures for low detail level.
        /// </summary>
        public VertexStructure[] VertexStructuresLowDetail
        {
            get { return m_vertexStructuresLowDetail; }
        }
    }
}