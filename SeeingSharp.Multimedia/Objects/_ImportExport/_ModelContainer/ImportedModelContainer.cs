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
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Util;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Threading;

namespace SeeingSharp.Multimedia.Objects
{
    public class ImportedModelContainer
    {
        #region Static id counter 
        private static int s_maxContainerID;
        #endregion

        #region All model data
        private int m_importID = 0;
        private ImportOptions m_importOptions;
        private List<SceneObject> m_objects;
        private List<Tuple<SceneObject, SceneObject>> m_parentChildRelationships;
        private List<ImportedResourceInfo> m_importedResources;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportedModelContainer" /> class.
        /// </summary>
        public ImportedModelContainer(ImportOptions importOptions)
        {
            m_importOptions = importOptions;
            m_objects = new List<SceneObject>();
            m_parentChildRelationships = new List<Tuple<SceneObject, SceneObject>>();
            m_importedResources = new List<ImportedResourceInfo>();

            m_importID = Interlocked.Increment(ref s_maxContainerID);
        }

        /// <summary>
        /// Creates and adds the root for all imported scene objects.
        /// </summary>
        public ScenePivotObject CreateAndAddRootObject()
        {
            // Append an object which transform the whole coordinate system
            ScenePivotObject rootObject = new ScenePivotObject();

            // Handle base transformation
            switch(m_importOptions.ResourceCoordinateSystem)
            {
                case CoordinateSystem.LeftHanded_UpY:
                    rootObject.TransformationType = SpacialTransformationType.None;
                    break;

                case CoordinateSystem.LeftHanded_UpZ:
                    rootObject.TransformationType = SpacialTransformationType.None;
                    break;

                case CoordinateSystem.RightHanded_UpY:
                    rootObject.TransformationType = SpacialTransformationType.None;
                    break;

                case CoordinateSystem.RightHanded_UpZ:
                    rootObject.Scaling = new Vector3(-1f, 1f, -1f);
                    rootObject.RotationEuler = new Vector3(EngineMath.RAD_90DEG, 0f, 0f);
                    rootObject.TransformationType = SpacialTransformationType.ScalingTranslationEulerAngles;
                    break;
            }

            // Add the object finally
            this.Objects.Add(rootObject);

            return rootObject;
        }

        /// <summary>
        /// Generates a key for a resource contained in an imported object graph.
        /// </summary>
        /// <param name="resourceClass">The type of the resource (defined by importer).</param>
        /// <param name="resourceID">The id of the resource (defined by importer)</param>
        public NamedOrGenericKey GetResourceKey(string resourceClass, string resourceID)
        {
            return new NamedOrGenericKey(
                "Imported." + m_importID + "." + resourceClass + "." + resourceID);
        }

        /// <summary>
        /// Gets a collection containing all objects.
        /// </summary>
        public List<SceneObject> Objects
        {
            get { return m_objects; }
        }

        /// <summary>
        /// Gets the hierarchy information of the loaded objects.
        /// </summary>
        public List<Tuple<SceneObject, SceneObject>> ParentChildRelationships
        {
            get { return m_parentChildRelationships; }
        }

        /// <summary>
        /// Gets a collection containing all resources.
        /// </summary>
        public List<ImportedResourceInfo> ImportedResources
        {
            get { return m_importedResources; }
        }

        /// <summary>
        /// Should triangle order be changes by the import logic?
        /// </summary>
        public bool ChangeTriangleOrder
        {
            get
            {
                switch(m_importOptions.ResourceCoordinateSystem)
                {
                    case CoordinateSystem.LeftHanded_UpY:
                    case CoordinateSystem.RightHanded_UpZ:
                        return false;

                    case CoordinateSystem.LeftHanded_UpZ:
                    case CoordinateSystem.RightHanded_UpY:
                        return true;

                    default:
                        throw new SeeingSharpGraphicsException(string.Format(
                            "Unknown coordinate system {0}!",
                            m_importOptions.ResourceCoordinateSystem));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float ResizeFactor 
        { 
            get 
            { 
                return m_importOptions.ResizeFactor;
            } 
        }
    }
}
