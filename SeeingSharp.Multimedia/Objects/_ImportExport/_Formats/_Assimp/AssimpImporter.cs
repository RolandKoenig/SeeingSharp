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

#if DESKTOP
using SeeingSharp.Infrastructure;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using System.Numerics;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.Multimedia.Objects.AssimpImporter),
    contractType: typeof(SeeingSharp.Multimedia.Objects.IModelImporter))]

namespace SeeingSharp.Multimedia.Objects
{
    [SupportedFileFormat("3ds", "3D Studio Max format")]
    [SupportedFileFormat("obj", "Wavefront Technologies format")]
    [SupportedFileFormat("x", "DirectX format")]
    [SupportedFileFormat("dae", "Collada")]
    [SupportedFileFormat("stl", "Stereolithography")]
    public class AssimpImporter : IModelImporter
    {
        private const string RES_CLASS_MATERIAL = "Mat";
        private const string RES_CLASS_TEXTURE = "Tex";
        private const string RES_CLASS_MESH = "Mesh";

        /// <summary>
        /// Imports a model from the given file.
        /// </summary>
        /// <param name="sourceFile">The source file to be loaded.</param>
        /// <param name="importOptions">Some configuration for the importer.</param>
        /// <returns></returns>
        public ImportedModelContainer ImportModel(ResourceLink sourceFile, ImportOptions importOptions)
        {
            ImportedModelContainer result = new ImportedModelContainer(importOptions);

            // Append an object which transform the whole coordinate system
            ScenePivotObject rootObject = result.CreateAndAddRootObject();

            using (Stream inStream = sourceFile.OpenInputStream())
            using (Assimp.AssimpContext importer = new Assimp.AssimpContext())
            {
                // Set custom io system
                //  => This is needed to load additional dependencies
                importer.SetIOSystem(new SeeingSharpAssimpIOSystem(sourceFile));

                // Load all contents of the file to assimp's DOM model
                Assimp.Scene importedScene = importer.ImportFileFromStream(inStream, "." + sourceFile.FileExtension);

                // Now import step-by-step
                ImportGeometry(result, importedScene, sourceFile);
                ImportMaterials(result, importedScene, sourceFile);
                ImportObject(result, importedScene, importedScene.RootNode, rootObject);
            }

            return result;
        }

        /// <summary>
        /// Creates a default import options object for this importer.
        /// </summary>
        public ImportOptions CreateDefaultImportOptions()
        {
            return new ImportOptions();
        }

        /// <summary>
        /// Imports all meshes (=> geometry) from the given scene.
        /// </summary>
        /// <param name="container">The target container object.</param>
        /// <param name="importedScene">The scene to be imported.</param>
        /// <param name="sourceFile">The original source file.</param>
        private void ImportGeometry(ImportedModelContainer container, Assimp.Scene importedScene, ResourceLink sourceFile)
        {
            for (int actMeshID = 0; actMeshID < importedScene.MeshCount; actMeshID++)
            {
                Assimp.Mesh actMesh = importedScene.Meshes[actMeshID];
                int actVertexCount = actMesh.VertexCount;
                int actFaceCount = actMesh.FaceCount;

                // Get vertex colors from the model
                List<Assimp.Color4D> vertexColors = null;
                if (actMesh.HasVertexColors(0))
                {
                    vertexColors = actMesh.VertexColorChannels[0];
                }

                // Get texture coordinates from the model
                List<Assimp.Vector3D> textureCoords = null;
                if (actMesh.HasTextureCoords(0))
                {
                    textureCoords = actMesh.TextureCoordinateChannels[0];
                }

                // Prepare structure
                VertexStructure vStructure = new VertexStructure(actVertexCount, actFaceCount);
                vStructure.Material = container.GetResourceKey(RES_CLASS_MATERIAL, actMesh.MaterialIndex.ToString());

                // Create all vertices
                float resizeFactor = container.ResizeFactor;
                for (int actVertexID = 0; actVertexID < actVertexCount; actVertexID++)
                {
                    Vertex newVertex = new Vertex();
                    newVertex.Position = actMesh.Vertices[actVertexID].ToSeeingSharpVector() * resizeFactor;
                    if (actMesh.HasNormals) { newVertex.Normal = actMesh.Normals[actVertexID].ToSeeingSharpVector(); }
                    if (vertexColors != null) { newVertex.Color = vertexColors[actVertexID].ToSeeingSharpColor(); }
                    if (textureCoords != null)
                    {
                        // Mapping of Y coordinate may be confusing here
                        //  .. idea comes from http://stackoverflow.com/questions/7943137/incorrect-texture-in-3d-model-loading-using-assimp-opengl
                        Vector2 actTexCoord = Vector3Ex.GetXY(textureCoords[actVertexID].ToSeeingSharpVector());
                        actTexCoord.Y = 1 - actTexCoord.Y;
                        newVertex.TexCoord = actTexCoord;
                    }
                    vStructure.AddVertex(newVertex);
                }

                // Create all faces
                for (int actFaceID = 0; actFaceID < actFaceCount; actFaceID++)
                {
                    List<int> actFaceIndices = actMesh.Faces[actFaceID].Indices;
                    if (actFaceIndices == null) { continue; }

                    // Convert indices to integer format
                    int[] actFaceIndicesInt = new int[actFaceIndices.Count];
                    for (int loop = 0; loop < actFaceIndices.Count; loop++)
                    {
                        actFaceIndicesInt[loop] = (int)actFaceIndices[loop];
                    }

                    // Append current face to VertexStructure
                    switch (actFaceIndices.Count)
                    {
                        case 0:
                        case 1:
                        case 2:
                            break;

                        case 3:
                            vStructure.AddTriangle((int)actFaceIndices[0], (int)actFaceIndices[1], (int)actFaceIndices[2]);
                            break;

                        default:
                            vStructure.AddPolygonByCuttingEars(actFaceIndicesInt);
                            break;
                    }
                }

                // Create geometry resource
                container.ImportedResources.Add(new ImportedResourceInfo(
                    container.GetResourceKey(RES_CLASS_MESH, actMeshID.ToString()),
                    () => new GeometryResource(vStructure)));
            }
        }

        /// <summary>
        /// Imports all materials from the given scene.
        /// </summary>
        /// <param name="container">The target container object.</param>
        /// <param name="importedScene">The scene to be imported.</param>
        /// <param name="sourceFile">The original source file</param>
        private void ImportMaterials(ImportedModelContainer container, Assimp.Scene importedScene, ResourceLink sourceFile)
        {
            Assimp.TextureSlot textureInfo;
            for (int actMaterialID = 0; actMaterialID < importedScene.MaterialCount; actMaterialID++)
            {
                Assimp.Material actMaterial = importedScene.Materials[actMaterialID];

                // Load the texture if it is available
                int difTextureCount = actMaterial.GetMaterialTextureCount(Assimp.TextureType.Diffuse);
                NamedOrGenericKey textureKey = NamedOrGenericKey.Empty;
                if (difTextureCount > 0)
                {
                    textureKey = container.GetResourceKey(RES_CLASS_TEXTURE, actMaterialID.ToString());
                    actMaterial.GetMaterialTexture(Assimp.TextureType.Diffuse, 0, out textureInfo);
                    ResourceLink textureFile = sourceFile.GetForAnotherFile(textureInfo.FilePath);
                    container.ImportedResources.Add(new ImportedResourceInfo(
                        textureKey,
                        () => new StandardTextureResource(textureFile)));
                }

                // Generate the material
                container.ImportedResources.Add(new ImportedResourceInfo(
                    container.GetResourceKey(RES_CLASS_MATERIAL, actMaterialID.ToString()),
                    () => new SimpleColoredMaterialResource(textureKey)));
            }
        }

        /// <summary>
        /// Imports this given node (=> object) from the given scene.
        /// </summary>
        /// <param name="container">The target container object.</param>
        /// <param name="importedScene">The scene to be imported.</param>
        /// <param name="currentNode"></param>
        /// <param name="parentObject"></param>
        private void ImportObject(ImportedModelContainer container, Assimp.Scene importedScene, Assimp.Node currentNode, SceneObject parentObject)
        {
            Assimp.Vector3D location;
            Assimp.Vector3D scaling;
            Assimp.Quaternion rotation;
            currentNode.Transform.Decompose(out scaling, out rotation, out location);

            // Generate the pivot on which all mesh instances are oriented
            ScenePivotObject currentPivot = new ScenePivotObject();
            currentPivot.TransformationType = SpacialTransformationType.ScalingTranslationQuaternion;
            currentPivot.Position = location.ToSeeingSharpVector() * container.ResizeFactor;
            currentPivot.Scaling = scaling.ToSeeingSharpVector();
            currentPivot.RotationQuaternion = rotation.ToSeeingSharpQuaternion();
            currentPivot.Tag1 = "Node: " + currentNode.Name;
            container.Objects.Add(currentPivot);
            container.ObjectDependencies.Add(Tuple.Create<SceneObject, SceneObject>(parentObject, currentPivot));

            // Add all meshes to the scene
            if (currentNode.HasMeshes)
            {
                int meshCount = currentNode.MeshCount;
                for (int actMeshIndex = 0; actMeshIndex < meshCount; actMeshIndex++)
                {
                    GenericObject actGenericObject = new GenericObject(container.GetResourceKey(RES_CLASS_MESH, currentNode.MeshIndices[actMeshIndex].ToString()));
                    actGenericObject.TransformationType = SpacialTransformationType.None;
                    container.Objects.Add(actGenericObject);

                    container.ObjectDependencies.Add(Tuple.Create<SceneObject, SceneObject>(currentPivot, actGenericObject));
                }
            }

            // Add all children to the scene
            if (currentNode.HasChildren)
            {
                int childCount = currentNode.ChildCount;
                for (int actChildIndex = 0; actChildIndex < childCount; actChildIndex++)
                {
                    ImportObject(container, importedScene, currentNode.Children[actChildIndex], currentPivot);
                }
            }
        }
    }
}
#endif