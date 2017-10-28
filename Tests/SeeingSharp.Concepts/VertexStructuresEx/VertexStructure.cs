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
using System.Numerics;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;

namespace SeeingSharp.Concepts.VertexStructuresEx
{
    /// <summary>
    /// Describes a 3D geometry in memory.
    /// </summary>
    public partial class VertexStructure
    {
        #region Description
        private string m_name;
        private string m_description;
        private Assembly m_resourceSourceAssembly;
        private ResourceLink m_resourceLink;
        #endregion

        #region Raw data
        private List<GeometryData> m_rawGeometry;
        private List<TextureData> m_rawTexture;
        private List<AnimationData> m_rawAnimation;
        #endregion

        #region Geometry
        private List<VertexStructureSurface> m_surfaces;
        private VertexCollection m_vertexCollection;
        private SurfaceCollection m_surfaceCollection;
        #endregion

        #region Members for build time transform
        private bool m_buildTimeTransformEnabled;
        private Matrix4x4 m_buildTransformMatrix;
        private Func<Vertex, Vertex> m_buildTimeTransformFunc;
        #endregion

        /// <summary>
        /// Creates a new Vertex structure object
        /// </summary>
        public VertexStructure(
            int verticesCapacity = 512, 
            bool supportsRawTexture = true, 
            bool supportsRawAnimation = false)
        {
            m_name = string.Empty;
            m_description = string.Empty;

            // Create raw data collections
            m_rawGeometry = new List<GeometryData>(verticesCapacity);
            if (supportsRawTexture) { m_rawTexture = new List<TextureData>(verticesCapacity); }
            if (supportsRawAnimation) { m_rawAnimation = new List<AnimationData>(verticesCapacity); }

            m_surfaces = new List<VertexStructureSurface>();

            m_vertexCollection = new VertexCollection(this);
            m_surfaceCollection = new SurfaceCollection(m_surfaces);
        }

        /// <summary>
        /// Creates the surface on this VertexStructure.
        /// </summary>
        /// <param name="triangleCapacity">The triangle capacity.</param>
        /// <param name="name">The internal name of the material.</param>
        public VertexStructureSurface CreateSurface(int triangleCapacity = 512, string name = "")
        {
            VertexStructureSurface newSurface = new VertexStructureSurface(this, triangleCapacity);
            newSurface.MaterialProperties.Name = name;

            m_surfaces.Add(newSurface);
            return newSurface;
        }

        /// <summary>
        /// Tries to get an existing surface using given name.
        /// If none exists, then a new surface is created.
        /// </summary>
        /// <param name="name">The internal name of the material.</param>
        /// <param name="triangleCapacity">The triangle capacity.</param>
        public VertexStructureSurface CreateOrGetExistingSurfaceByName(string name, int triangleCapacity = 512)
        {
            foreach (VertexStructureSurface actSurface in m_surfaces)
            {
                if (actSurface.MaterialProperties.Name == name) { return actSurface; }
            }

            return CreateSurface(triangleCapacity, name);
        }

        /// <summary>
        /// Tries to get an existing surface using given MaterialProperties.
        /// If none exists, then a new surface is created.
        /// </summary>
        /// <param name="matProperties">The material properties.</param>
        /// <param name="triangleCapacity">The triangle capacity.</param>
        public VertexStructureSurface CreateOrGetExistingSurface(MaterialProperties matProperties, int triangleCapacity = 512)
        {
            foreach(VertexStructureSurface actSurface in m_surfaces)
            {
                if(actSurface.MaterialProperties == matProperties) { return actSurface; }
            }

            VertexStructureSurface result = CreateSurface(triangleCapacity);
            result.MaterialProperties = matProperties;
            return result;
        }

        /// <summary>
        /// Tries to get an existing surface using given MaterialProperties.
        /// If none exists, then a new surface is created.
        /// </summary>
        /// <param name="name">The internal name of the material.</param>
        /// <param name="triangleCapacity">The triangle capacity.</param>
        public VertexStructureSurface CreateOrGetExistingSurface(string name, int triangleCapacity = 512)
        {
            foreach (VertexStructureSurface actSurface in m_surfaces)
            {
                if (actSurface.MaterialProperties.Name == name) { return actSurface; }
            }

            VertexStructureSurface result = CreateSurface(triangleCapacity);
            result.MaterialProperties.Name = name;
            return result;
        }

        /// <summary>
        /// Realigns all given structures to their center coordinate.
        /// </summary>
        public void RealignToCenter()
        {
            BoundingBox fullBoundingBox = this.GenerateBoundingBox();
            Vector3 fullCenter = fullBoundingBox.GetMiddleCenter();
            Vector3 targetCenter = new Vector3(0f, 0f, 0f);

            Vector3 moveToTargetCenter = targetCenter - fullCenter;

            this.UpdateVerticesUsingRelocationBy(moveToTargetCenter);
        }

        public void RealignToFloorCenter()
        {

            BoundingBox fullBoundingBox = this.GenerateBoundingBox();
            Vector3 fullCenter = fullBoundingBox.GetMiddleCenter();
            Vector3 targetCenter = new Vector3(0f, fullBoundingBox.GetSize().Y / 2f, 0f);

            Vector3 moveToTargetCenter = targetCenter - fullCenter;

            this.UpdateVerticesUsingRelocationBy(moveToTargetCenter);
        }

        ///// <summary>
        ///// Fits to centered cube.
        ///// </summary>
        //public void FitToCenteredCube()
        //{
        //    FitToCenteredCuboid(1f, 1f, 1f, FitToCuboidMode.MaintainAspectRatio, SpacialOriginLocation.Center);
        //}

        ///// <summary>
        ///// Fits to centered cube.
        ///// </summary>
        //public void FitToCenteredCube(float cubeSideLength)
        //{
        //    FitToCenteredCuboid(cubeSideLength, cubeSideLength, cubeSideLength, FitToCuboidMode.MaintainAspectRatio, SpacialOriginLocation.Center);
        //}

        ///// <summary>
        ///// Fits to centered cube.
        ///// </summary>
        //public void FitToCenteredCube(float cubeSideLength, FitToCuboidMode mode)
        //{
        //    FitToCenteredCuboid(cubeSideLength, cubeSideLength, cubeSideLength, mode, SpacialOriginLocation.Center);
        //}

        ///// <summary>
        ///// Fits to centered cube.
        ///// </summary>
        //public void FitToCenteredCube(float cubeSideLength, FitToCuboidMode mode, SpacialOriginLocation fitOrigin)
        //{
        //    FitToCenteredCuboid(cubeSideLength, cubeSideLength, cubeSideLength, mode, fitOrigin);
        //}

        ///// <summary>
        ///// Fits to centered cube.
        ///// </summary>
        //public void FitToCenteredCuboid(float cubeSideLengthX, float cubeSideLengthY, float cubeSideLengthZ, FitToCuboidMode fitMode, SpacialOriginLocation fitOrigin)
        //{
        //    //Get whole bounding box
        //    BoundingBox boundingBox = this.GenerateBoundingBox();
        //    Vector3 boundingBoxSize = boundingBox.GetSize();
        //    if (boundingBox.IsEmpty()) { return; }
        //    if (boundingBoxSize.X <= 0f) { return; }
        //    if (boundingBoxSize.Y <= 0f) { return; }
        //    if (boundingBoxSize.Z <= 0f) { return; }

        //    Vector3 targetCornerALocation = new Vector3(
        //        -boundingBoxSize.X / 2f,
        //        -boundingBoxSize.Y / 2f,
        //        -boundingBoxSize.Z / 2f);

        //    // Calculate resize factors
        //    float resizeFactorX = cubeSideLengthX / boundingBoxSize.X;
        //    float resizeFactorY = cubeSideLengthY / boundingBoxSize.Y;
        //    float resizeFactorZ = cubeSideLengthZ / boundingBoxSize.Z;
        //    if (fitMode == FitToCuboidMode.MaintainAspectRatio)
        //    {
        //        resizeFactorX = Math.Min(resizeFactorX, Math.Min(resizeFactorY, resizeFactorZ));
        //        resizeFactorY = resizeFactorX;
        //        resizeFactorZ = resizeFactorX;
        //    }

        //    targetCornerALocation.X = targetCornerALocation.X * resizeFactorX;
        //    targetCornerALocation.Y = targetCornerALocation.Y * resizeFactorY;
        //    targetCornerALocation.Z = targetCornerALocation.Z * resizeFactorZ;
        //    switch (fitOrigin)
        //    {
        //        case SpacialOriginLocation.LowerCenter:
        //            targetCornerALocation.Y = 0f;
        //            break;
        //    }

        //    //Bring the structure to origin based location and then scale it
        //    this.UpdateVerticesUsingRelocationBy(Vector3.Negate(boundingBox.CornerA));
        //    this.UpdateVerticesUsingRelocationFunc((actPosition) => new Vector3(
        //        actPosition.X * resizeFactorX,
        //        actPosition.Y * resizeFactorY,
        //        actPosition.Z * resizeFactorZ));
        //    this.UpdateVerticesUsingRelocationBy(targetCornerALocation);
        //}

        public void RemoveSurface(VertexStructureSurface surface)
        {
            m_surfaces.Remove(surface);
        }

        /// <summary>
        /// Ensures that there is a vertex at the given index and returns it.
        /// </summary>
        /// <param name="index">The index to get the vertex from.</param>
        public Vertex EnsureVertexAt(int index)
        {
            while (m_rawGeometry.Count <= index) { this.AddVertex(); }
            return new Vertex(this, index);
        }

        /// <summary>
        /// Enables build-time transform using the given matrix.
        /// </summary>
        /// <param name="transformMatrix">Transform matrix.</param>
        public void EnableBuildTimeTransform(Matrix4x4 transformMatrix)
        {
            m_buildTimeTransformEnabled = true;
            m_buildTransformMatrix = transformMatrix;
            m_buildTimeTransformFunc = null;
        }

        /// <summary>
        /// Enables build-time transform using given transform method.
        /// </summary>
        public void EnableBuildTimeTransform(Func<Vertex, Vertex> transformFunc)
        {
            m_buildTimeTransformEnabled = true;
            m_buildTransformMatrix = Matrix4x4.Identity;
            m_buildTimeTransformFunc = transformFunc;
        }

        /// <summary>
        /// Disables build-time transform.
        /// </summary>
        public void DisableBuildTimeTransform()
        {
            m_buildTimeTransformEnabled = false;
            m_buildTransformMatrix = Matrix4x4.Identity;
        }

        ///// <summary>
        ///// Performs a simple picking test against all triangles of this object.
        ///// </summary>
        ///// <param name="pickingRay">The picking ray.</param>
        ///// <param name="distance">Additional picking options.</param>
        ///// <param name="pickingOptions">The distance if picking succeeds.</param>
        //public bool Intersects(Ray pickingRay, PickingOptions pickingOptions, out float distance)
        //{
        //    int surfaceCount = m_surfaces.Count;
        //    for(int loop=0; loop<surfaceCount; loop++)
        //    {
        //        if(m_surfaces[loop].Intersects(pickingRay, pickingOptions, out distance))
        //        {
        //            return true;
        //        }
        //    }

        //    distance = 0f;
        //    return false;
        //}

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
            int baseIndex = (int)m_rawGeometry.Count;
            int newStructureLength = otherStructure.CountVertices;

            // Add all raw data collections
            m_rawGeometry.AddRange(otherStructure.m_rawGeometry);
            if (m_rawTexture != null)
            {
                if (otherStructure.m_rawTexture != null) { m_rawTexture.AddRange(otherStructure.m_rawTexture); }
                else
                {
                    for(int actNewIndex =0; actNewIndex<newStructureLength; actNewIndex++)
                    {
                        m_rawTexture.Add(default(TextureData));
                    }
                }
            }
            if (m_rawAnimation != null)
            {
                if(otherStructure.m_rawAnimation != null) { m_rawAnimation.AddRange(otherStructure.m_rawAnimation); }
                else
                {
                    for (int actNewIndex = 0; actNewIndex < newStructureLength; actNewIndex++)
                    {
                        m_rawAnimation.Add(default(AnimationData));
                    }
                }
            }

            // Copy all structures
            int otherStructureSurfaceCount = otherStructure.m_surfaces.Count;
            for (int loopSurface = 0; loopSurface < otherStructureSurfaceCount; loopSurface++)
            {
                m_surfaces.Add(otherStructure.m_surfaces[loopSurface].Clone(
                    this,
                    baseIndex: baseIndex));
            }
        }

        public Vertex AddVertex(
            int indexToCloneFrom = -1)
        {
            if(indexToCloneFrom > -1)
            {
                indexToCloneFrom.EnsureInRange(0, m_rawGeometry.Count - 1, nameof(indexToCloneFrom));

                GeometryData geoData = m_rawGeometry[indexToCloneFrom];
                TextureData texData = m_rawTexture != null ? m_rawTexture[indexToCloneFrom] : default(TextureData);
                AnimationData animData = m_rawAnimation != null ? m_rawAnimation[indexToCloneFrom] : default(AnimationData);

                m_rawGeometry.Add(geoData);
                m_rawTexture?.Add(texData);
                m_rawAnimation?.Add(animData);

                return new Vertex(this, m_rawGeometry.Count - 1);
            }
            else
            {
                m_rawGeometry.Add(new GeometryData());
                if (m_rawAnimation != null) { m_rawAnimation.Add(new AnimationData()); }
                if (m_rawTexture != null) { m_rawTexture.Add(new TextureData()); }
                return new Vertex(this, m_rawGeometry.Count - 1);
            }
        }

        /// <summary>
        /// Calculates tangents for all vectors.
        /// </summary>
        public void CalculateTangentsAndBinormals()
        {
            foreach (VertexStructureSurface actSurface in m_surfaces)
            {
                actSurface.CalculateTangentsAndBinormals();
            }
        }

        /// <summary>
        /// Recalculates all normals
        /// </summary>
        public void CalculateNormalsFlat()
        {
            foreach(VertexStructureSurface actSurface in m_surfaces)
            {
                actSurface.CalculateNormalsFlat();
            }
        }

        /// <summary>
        /// Calculates normals for all triangles of this structure.
        /// </summary>
        public void CalculateNormals()
        {
            CalculateNormals(0, m_rawGeometry.Count);
        }

        /// <summary>
        /// Calculates normals for all triangles specifyed by the given parameters.
        /// </summary>
        /// <param name="startVertex">The vertex index on which to start.</param>
        /// <param name="vertexCount">Total count of vertices to be updated.</param>
        public void CalculateNormals(int startVertex, int vertexCount)
        {
            if ((startVertex < 0) || (startVertex >= m_rawGeometry.Count)) { throw new ArgumentException("startVertex"); }
            if (vertexCount + startVertex > m_rawGeometry.Count) { throw new ArgumentException("vertexCount"); }

            for (int actVertexIndex = startVertex; actVertexIndex < startVertex + vertexCount; actVertexIndex++)
            {
                // Find all triangles connected to this vertex and get normals from them
                Vector3 finalNormalHelper = Vector3.Zero;
                Vector3 finalNormalHelper2 = Vector3.Zero;
                int normalCount = 0;

                int surfaceCount = m_surfaces.Count;
                for (int actSurfaceIndex = 0; actSurfaceIndex < surfaceCount; actSurfaceIndex++)
                {
                    VertexStructureSurface actSurface = m_surfaces[actSurfaceIndex];
                    int triangleCount = actSurface.CountTriangles;
                    for (int loopTriangle = 0; loopTriangle < triangleCount; loopTriangle++)
                    {
                        int triangleStartIndex = loopTriangle * 3;
                        if ((actSurface.IndicesInternal[triangleStartIndex] == actVertexIndex) ||
                            (actSurface.IndicesInternal[triangleStartIndex + 1] == actVertexIndex) ||
                            (actSurface.IndicesInternal[triangleStartIndex + 2] == actVertexIndex))
                        {
                            GeometryData geo1 = m_rawGeometry[actSurface.IndicesInternal[triangleStartIndex]];
                            GeometryData geo2 = m_rawGeometry[actSurface.IndicesInternal[triangleStartIndex + 1]];
                            GeometryData geo3 = m_rawGeometry[actSurface.IndicesInternal[triangleStartIndex + 2]];

                            finalNormalHelper += Vector3Ex.CalculateTriangleNormal(geo1.Position, geo2.Position, geo3.Position, false);

                            normalCount++;
                        }
                    }
                }

                // Calculate final normal
                if (normalCount > 0)
                {
                    GeometryData actGeometry = m_rawGeometry[actVertexIndex];
                    actGeometry.Normal = finalNormalHelper / finalNormalHelper.Length();
                    m_rawGeometry[actVertexIndex] = actGeometry;
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
            int vertexCount = m_rawGeometry.Count;
            VertexStructure result = new VertexStructure(
                vertexCount * capacityMultiplier);

            // Copy geometry
            if (copyGeometryData)
            {
                for (int loop = 0; loop < vertexCount; loop++)
                {
                    result.m_rawGeometry.Add(m_rawGeometry[loop]);

                }
            }

            // Copy surfaces
            foreach(VertexStructureSurface actSurface in m_surfaces)
            {
                result.m_surfaces.Add(actSurface.Clone(result, copyGeometryData, capacityMultiplier));
            }

            // Copy metadata
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

            foreach (GeometryData actRawGeometry in m_rawGeometry)
            {
                Vector3 actPosition = actRawGeometry.Position;

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
        /// <param name="newColor">The new color.</param>
        public void SetColorOnEachVertex(Color4 newColor)
        {
            for (int loop = 0; loop < m_rawGeometry.Count; loop++)
            {
                GeometryData rawGeo = m_rawGeometry[loop];
                rawGeo.Color = newColor;
                m_rawGeometry[loop] = rawGeo;
            }
        }

        ///// <summary>
        ///// Changes the index order of each triangle.
        ///// </summary>
        //public void ToggleTriangleIndexOrder()
        //{
        //    for (int loop = 0; loop < m_surfaces.Count; loop++)
        //    {
        //        m_surfaces[loop].ToggleTriangleIndexOrder();
        //    }
        //}

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

            // Now change index ordering
            foreach(VertexStructureSurface actSurface in m_surfaces)
            {
                actSurface.ToggleCoordinateSystemInternal();
            }
        }

        /// <summary>
        /// Transforms positions and normals of all vertices using the given transform matrix
        /// </summary>
        /// <param name="transformMatrix"></param>
        public void TransformVertices(Matrix4x4 transformMatrix)
        {
            int length = m_rawGeometry.Count;
            for (int loop = 0; loop < length; loop++)
            {
                var actGeoData = m_rawGeometry[loop];
                actGeoData.Position = Vector3.Transform(actGeoData.Position, transformMatrix);
                actGeoData.Normal = Vector3.TransformNormal(actGeoData.Normal, transformMatrix);
                m_rawGeometry[loop] = actGeoData;
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
            int length = m_rawGeometry.Count;
            for (int loop = 0; loop < length; loop++)
            {
                var actGeoData = m_rawGeometry[loop];
                actGeoData.Position = actGeoData.Position + relocateVector;
                m_rawGeometry[loop] = actGeoData;
            }
        }

        /// <summary>
        /// Relocates all vertices by the given relocation function (executed for each position vector).
        /// </summary>
        /// <param name="calculatePositionFunc">The function to be applied to each coordinate.</param>
        public void UpdateVerticesUsingRelocationFunc(Func<Vector3, Vector3> calculatePositionFunc)
        {
            int length = m_rawGeometry.Count;
            for (int loop = 0; loop < length; loop++)
            {
                var actGeoData = m_rawGeometry[loop];
                actGeoData.Position = calculatePositionFunc(actGeoData.Position);
                m_rawGeometry[loop] = actGeoData;
            }
        }

        public bool SupportsRawAnimation
        {
            get { return m_rawAnimation != null; }
        }

        public bool SupportsRawTexture
        {
            get { return m_rawTexture != null; }
        }

        /// <summary>
        /// Gets the first surface of  this structure.
        /// It there is no surface, then one gets created automatically.
        /// </summary>
        public VertexStructureSurface FirstSurface
        {
            get
            {
                if(m_surfaces.Count == 0) { CreateSurface(); }
                return m_surfaces[0];
            }
        }

        /// <summary>
        /// Retrieves total count of all vertices within this structure
        /// </summary>
        public int CountVertices
        {
            get { return m_rawGeometry.Count; }
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
            get { return (m_rawGeometry.Count == 0) && (m_surfaces.Count == 0); }
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
        /// Gets a collection of vertices.
        /// </summary>
        public VertexCollection Vertices
        {
            get { return m_vertexCollection; }
        }

        ///// <summary>
        ///// Gets a collection of vertices.
        ///// </summary>
        //internal List<Vertex> VerticesInternal
        //{
        //    get { return m_vertices; }
        //}

        public SurfaceCollection Surfaces
        {
            get { return m_surfaceCollection; }
        }

        public int CountSurfaces
        {
            get { return m_surfaces.Count; }
        }

        public int CountTriangles
        {
            get
            {
                int sum = 0; 
                for(int loop=0; loop<m_surfaces.Count; loop++)
                {
                    sum += m_surfaces[loop].CountTriangles;
                }
                return sum;
            }
        }

        public int CountIndices
        {
            get
            {
                int sum = 0;
                for (int loop = 0; loop < m_surfaces.Count; loop++)
                {
                    sum += m_surfaces[loop].CountIndices;
                }
                return sum;
            }
        }

        /// <summary>
        /// Gets or sets the resource source assembly.
        /// </summary>
        public Assembly ResourceSourceAssembly
        {
            get { return m_resourceSourceAssembly; }
            set { m_resourceSourceAssembly = value; }
        }

        /// <summary>
        /// Gets or sets the original source of this geometry.
        /// </summary>
        public ResourceLink ResourceLink
        {
            get { return m_resourceLink; }
            set { m_resourceLink = value; }
        }

        internal List<GeometryData> RawGeometry
        {
            get { return m_rawGeometry; }
        }

        internal List<TextureData> RawTexture
        {
            get { return m_rawTexture; }
        }

        internal List<AnimationData> RawAnimation
        {
            get { return m_rawAnimation; }
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        /// <summary>
        /// Contains all vertices of a VertexStructure object
        /// </summary>
        public class VertexCollection : IEnumerable<Vertex>
        {
            private VertexStructure m_host;

            internal VertexCollection(VertexStructure host)
            {
                m_host = host;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new VertexEnumerator(m_host);
            }

            /// <summary>
            ///
            /// </summary>
            public IEnumerator<Vertex> GetEnumerator()
            {
                return new VertexEnumerator(m_host);
            }

            /// <summary>
            /// Returns the vertex at ghe given index
            /// </summary>
            public Vertex this[int index]
            {
                get { return new Vertex(m_host, index); }
            }
        }

        /// <summary>
        /// Helper class for enumerating all vertices within a <see cref="VertexStructure"/>.
        /// </summary>
        class VertexEnumerator : IEnumerator<Vertex>
        {
            private VertexStructure m_host;
            private int m_actIndex;

            public VertexEnumerator(VertexStructure host)
            {
                m_host = host;
                m_actIndex = -1;
            }

            public bool MoveNext()
            {
                m_actIndex++;
                return m_host.CountVertices > m_actIndex;
            }

            public void Reset()
            {
                m_actIndex = -1;
            }

            public void Dispose()
            {
                m_host = null;
                m_actIndex = -1;
            }

            public Vertex Current => new Vertex(m_host, m_actIndex);

            object IEnumerator.Current => new Vertex(m_host, m_actIndex);
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        /// <summary>
        /// Contains all vertices of a VertexStructure object
        /// </summary>
        public class SurfaceCollection : IEnumerable<VertexStructureSurface>
        {
            private List<VertexStructureSurface> m_surfaces;

            internal SurfaceCollection(List<VertexStructureSurface> surfaces)
            {
                m_surfaces = surfaces;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return m_surfaces.GetEnumerator();
            }

            public void Add(VertexStructureSurface surface)
            {
                m_surfaces.Add(surface);
            }

            public IEnumerator<VertexStructureSurface> GetEnumerator()
            {
                return m_surfaces.GetEnumerator();
            }

            /// <summary>
            /// Returns the surface at ghe given index
            /// </summary>
            public VertexStructureSurface this[int index]
            {
                get { return m_surfaces[index]; }
                internal set { m_surfaces[index] = value; }
            }
        }
    }
}