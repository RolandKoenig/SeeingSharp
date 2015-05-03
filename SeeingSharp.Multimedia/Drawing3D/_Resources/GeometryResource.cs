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

using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Util;
using System;
using System.Linq;
using System.Collections.Generic;

// Some namespace mappings
using D3D = SharpDX.Direct3D;
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;

namespace SeeingSharp.Multimedia.Drawing3D
{
    public class GeometryResource : Resource
    {
        private const int INSTANCE_BUFFER_MAX_SIZE = 1024 * 8;

        //Resources for Direct3D 11 rendering
        private LoadedStructureInfo[] m_loadedStructures;

        //Generic resources
        private BoundingBox m_boundingBox;
        private ObjectType m_objectType;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryResource"/> class.
        /// </summary>
        /// <param name="name">The name of the resource.</param>
        public GeometryResource(ObjectType objectType)
        {
            m_objectType = objectType;

            m_loadedStructures = new LoadedStructureInfo[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryResource"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="vertexStructures">The vertex structures.</param>
        public GeometryResource(params VertexStructure[] vertexStructures)
            : this(new GenericObjectType(vertexStructures))
        {

        }

        /// <summary>
        /// Performs an intersection test using given picking ray and picking options.
        /// </summary>
        /// <param name="pickingRay">The given picking ray.</param>
        /// <param name="pickingOptions">The picking options.</param>
        /// <param name="distance">The distance from origin to the picking point.</param>
        public bool Intersects(Ray pickingRay, PickingOptions pickingOptions, out float distance)
        {
            distance = float.MaxValue;
            bool result = false;

            for (int loop = 0; loop < m_loadedStructures.Length; loop++)
            {
                foreach (VertexStructure actLoadedStructure in m_loadedStructures[loop].VertexStructures)
                {
                    float currentDistance = float.NaN;
                    if (actLoadedStructure.Intersects(pickingRay, pickingOptions, out currentDistance))
                    {
                        result = true;
                        if (currentDistance < distance) { distance = currentDistance; }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Redefines the content of this geometry resource.
        /// </summary>
        /// <param name="vertexStructures">The new ObjectType to be applied.</param>
        public void Redefine(ResourceDictionary resources, ObjectType objectType)
        {
            //Unload resource first if it was loaded
            bool wasLoaded = this.IsLoaded;
            if (wasLoaded)
            {
                this.UnloadResource();
            }

            //Update members
            m_objectType = objectType;
            m_boundingBox = new BoundingBox();

            //Reload resources again if they where loaded before
            if (wasLoaded)
            {
                this.LoadResource();
            }
        }

        /// <summary>
        /// Redefines the content of this geometry resource.
        /// </summary>
        /// <param name="vertexStructures">An array containing all new vertex structures.</param>
        public void Redefine(ResourceDictionary resources, params VertexStructure[] vertexStructures)
        {
            this.Redefine(resources, new GenericObjectType(vertexStructures));
        }

        /// <summary>
        /// Performs a simple picking-test.
        /// </summary>
        /// <param name="pickInformation">The that gathers picking information.</param>
        /// <param name="pickingRay">The picking ray.</param>
        public void Pick(PickingInformation pickInformation, Ray pickingRay)
        {
            if (pickingRay.Intersects(m_boundingBox))
            {

            }
        }

        /// <summary>
        /// Renders this GeometryResource.
        /// </summary>
        /// <param name="renderState">Current render state.</param>
        public void Render(RenderState renderState)
        {
            D3D11.DeviceContext deviceContext = renderState.Device.DeviceImmediateContextD3D11;
            DXGI.Format indexBufferFormat = renderState.Device.SupportsOnly16BitIndexBuffer ? DXGI.Format.R16_UInt : DXGI.Format.R32_UInt;

            for (int loop = 0; loop < m_loadedStructures.Length; loop++)
            {
                LoadedStructureInfo structureToDraw = m_loadedStructures[loop];

                if (renderState.LastRenderBlockID != structureToDraw.RenderBlockID)
                {
                    // Apply new vertex buffer
                    deviceContext.InputAssembler.InputLayout = structureToDraw.InputLayout;
                    deviceContext.InputAssembler.SetIndexBuffer(structureToDraw.IndexBuffer, indexBufferFormat, 0);
                    deviceContext.InputAssembler.SetVertexBuffers(0, new D3D11.VertexBufferBinding(structureToDraw.VertexBuffer, structureToDraw.SizePerVertex, 0));

                    // Register current render block
                    renderState.LastRenderBlockID = structureToDraw.RenderBlockID;
                }

                // Apply material
                renderState.ApplyMaterial(structureToDraw.Material);
                D3D11.InputLayout newInputLayout = null;
                if(renderState.ForcedMaterial != null)
                {
                    newInputLayout = renderState.ForcedMaterial.GenerateInputLayout(
                        renderState.Device,
                        StandardVertex.InputElements,
                        MaterialApplyInstancingMode.SingleObject);
                    deviceContext.InputAssembler.InputLayout = newInputLayout;
                }
                try
                {
                    // Draw current rener block
                    deviceContext.DrawIndexed(
                        structureToDraw.IndexCount,
                        structureToDraw.StartIndex,
                        0);
                }
                finally
                {
                    if (newInputLayout != null)
                    {
                        deviceContext.InputAssembler.InputLayout = null;
                        GraphicsHelper.SafeDispose(ref newInputLayout);
                    }
                }
            }
        }

        /// <summary>
        /// Loads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void LoadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            //Build structures
            VertexStructure[] structures = m_objectType.BuildStructure(new StructureBuildOptions(device.SupportedDetailLevel));

            //Load materials
            List<Vector3> vertexLocations = new List<Vector3>();
            for (int loop = 0; loop < structures.Length; loop++)
            {
                foreach (Vertex actVertex in structures[loop].Vertices)
                {
                    vertexLocations.Add(actVertex.Position);
                }
            }
            m_boundingBox = new BoundingBox(vertexLocations);

            //Load specialized data
            List<VertexStructure> currentStructures = new List<VertexStructure>();
            int actVertexCount = 0;
            int actIndexCount = 0;
            for (int loop = 0; loop < structures.Length; loop++)
            {
                //Load VertexBuffer and IndexBuffer if we are unable to group further
                VertexStructure actStructure = structures[loop];
                if (actVertexCount + actStructure.CountVertices >= (int)int.MaxValue)
                {
                    VertexStructure[] structuresToLoad = currentStructures.ToArray();
                    currentStructures.Clear();
                }

                //Append current structure to current cache
                actVertexCount += actStructure.CountVertices;
                actIndexCount += actStructure.CountIndexes;
                currentStructures.Add(actStructure);
            }
            if (currentStructures.Count > 0)
            {
                VertexStructure[] structuresToLoad = currentStructures.ToArray();
                currentStructures.Clear();
            }

            //Build geometry
            m_loadedStructures = BuildBuffers(device, structures, resources);
        }

        /// <summary>
        /// Unloads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void UnloadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            for (int loop = 0; loop < m_loadedStructures.Length; loop++)
            {
                m_loadedStructures[loop].InputLayout = GraphicsHelper.DisposeObject(m_loadedStructures[loop].InputLayout);
                m_loadedStructures[loop].VertexBuffer = GraphicsHelper.DisposeObject(m_loadedStructures[loop].VertexBuffer);
                m_loadedStructures[loop].IndexBuffer = GraphicsHelper.DisposeObject(m_loadedStructures[loop].IndexBuffer);
            }
            m_loadedStructures = new LoadedStructureInfo[0];

            device = null;
        }

        /// <summary>
        /// Builds vertex and index buffers.
        /// </summary>
        /// <param name="device">The device on which to build all buffers.</param>
        /// <param name="vertexBuffers">An array containing all vertex buffers.</param>
        /// <param name="indexBuffers">An array containing all index buffers.</param>
        /// <param name="resources">The current resource dictionary</param>
        protected virtual LoadedStructureInfo[] BuildBuffers(EngineDevice device, VertexStructure[] structures, ResourceDictionary resources)
        {
            List<LoadedStructureInfo> result = new List<LoadedStructureInfo>();

            // Get all used materials and associate corresponding vertex structures
            List<Tuple<MaterialResource, VertexStructure>> structureMaterialTuples = new List<Tuple<MaterialResource, VertexStructure>>();
            foreach (var actStructure in structures)
            {
                MaterialResource actMaterialResource = resources.GetOrCreateMaterialResourceAndEnsureLoaded(actStructure);
                structureMaterialTuples.Add(Tuple.Create(actMaterialResource, actStructure));
            }
            int structureCount = structureMaterialTuples.Count;

            // Sort structures by material
            structureMaterialTuples.Sort((left, right) => left.Item1.GetHashCode().CompareTo(right.Item1.GetHashCode()));

            // Define variables for VertexBuffer building
            int countVertices = 0;
            int actBaseVertex = 0;
            List<StandardVertex[]> allCurrentVertices = new List<StandardVertex[]>();
            List<int[]> allCurrentIndices = new List<int[]>();
            int currentRenderBlockStart = 0;
            for (int loop = 0; loop < structureCount; loop++)
            {
                VertexStructure actStructure = structureMaterialTuples[loop].Item2;

                // Update index and vertex collections
                actBaseVertex = countVertices;
                countVertices += actStructure.CountVertices;

                // Append vertex data to current buffer
                allCurrentVertices.Add(StandardVertex.FromVertexStructure(actStructure));

                // Append index data to current buffer
                int[] indexArray = actStructure.GetIndexArray();
                if (actBaseVertex > 0)
                {
                    for (int loopInner = 0; loopInner < indexArray.Length; loopInner++)
                    {
                        indexArray[loopInner] = (int)(indexArray[loopInner] + actBaseVertex);
                    }
                }
                allCurrentIndices.Add(indexArray);

                // Finish buffers / finish loop
                int nextIndex = loop + 1;
                if ((nextIndex >= structureCount) ||
                    (countVertices + structureMaterialTuples[nextIndex].Item2.CountVertices > int.MaxValue - 100))
                {
                    countVertices = 0;
                    int actRenderBlockID = resources.GetNextRenderBlockID();

                    // Build vertex and index buffers
                    D3D11.Buffer vertexBuffer = GraphicsHelper.CreateImmutableVertexBuffer(device, allCurrentVertices.ToArray());
                    D3D11.Buffer indexBuffer = GraphicsHelper.CreateImmutableIndexBuffer(device, allCurrentIndices.ToArray());

                    // Build render blocks per contained material
                    MaterialResource actMaterial = null;
                    List<VertexStructure> localStructures = new List<VertexStructure>();
                    int countIndicesInner = 0;
                    int countVerticesInner = 0;
                    int lastVertexCount = 0;
                    int lastIndexCount = 0;
                    for (int loopBlock = currentRenderBlockStart; loopBlock <= loop; loopBlock++)
                    {
                        VertexStructure actStructureBlock = structureMaterialTuples[loopBlock].Item2;
                        localStructures.Add(actStructureBlock);

                        countIndicesInner += actStructureBlock.CountIndexes;
                        countVerticesInner += actStructureBlock.CountVertices;

                        actMaterial = structureMaterialTuples[loopBlock].Item1;
                        if ((loopBlock == loop) ||
                           (actMaterial != structureMaterialTuples[loopBlock + 1].Item1))
                        {
                            // Create some information about the loaded structures
                            LoadedStructureInfo newStructureInfo = new LoadedStructureInfo();
                            newStructureInfo.RenderBlockID = actRenderBlockID;
                            newStructureInfo.SizePerVertex = StandardVertex.Size;
                            newStructureInfo.VertexStructures = localStructures.ToArray();
                            newStructureInfo.IndexCount = countIndicesInner;
                            newStructureInfo.StartIndex = lastIndexCount;
                            newStructureInfo.Material = actMaterial;
                            newStructureInfo.VertexBuffer = vertexBuffer;
                            newStructureInfo.IndexBuffer = indexBuffer;
                            newStructureInfo.InputLayout = newStructureInfo.Material.GenerateInputLayout(
                                device, StandardVertex.InputElements, MaterialApplyInstancingMode.SingleObject);
                            result.Add(newStructureInfo);

                            // Reset counter variables
                            lastIndexCount += countIndicesInner;
                            lastVertexCount += countVerticesInner;
                            countIndicesInner = 0;
                            countVerticesInner = 0;
                        }
                    }

                    currentRenderBlockStart = loop + 1;
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Is the resource loaded?
        /// </summary>
        public override bool IsLoaded
        {
            get { return m_loadedStructures.Length > 0; }
        }

        /// <summary>
        /// Gets the source of geometry data.
        /// </summary>
        public ObjectType ObjectType
        {
            get { return m_objectType; }
        }

        /// <summary>
        /// Gets the bounding box sourounding this object.
        /// </summary>
        public BoundingBox BoundingBox
        {
            get { return m_boundingBox; }
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        protected class LoadedStructureInfo
        {
            public int RenderBlockID;
            public VertexStructure[] VertexStructures;
            public D3D11.Buffer VertexBuffer;
            public D3D11.Buffer IndexBuffer;
            public int SizePerVertex;
            public int IndexCount;
            public int StartIndex;
            public MaterialResource Material;
            public D3D11.InputLayout InputLayout;
        }
    }
}