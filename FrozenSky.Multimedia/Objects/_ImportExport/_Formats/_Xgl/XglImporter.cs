#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2014 Roland König (RolandK)

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
using FrozenSky.Infrastructure;
using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

// Some namespace mappings
using DXGI = SharpDX.DXGI;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(FrozenSky.Multimedia.Objects.XglImporter),
    contractType: typeof(FrozenSky.Multimedia.Objects.IModelImporter))]

namespace FrozenSky.Multimedia.Objects
{
    /// <summary>
    /// An importer which loads files in xgl format.
    /// See http://vizstream.aveva.com/release/vsplatform/XGLSpec.htm
    /// </summary>
    [SupportedFileFormat("xgl", "SGI OpenGL render library format")]
    [SupportedFileFormat("zgl", "SGI OpenGL render library format (compressed)")]
    public class XglImporter : IModelImporter
    {
        private const string RES_CLASS_TEXTURE = "Tex";
        private const string RES_CLASS_MESH = "Mesh";

        // All common nodes
        private const string NODE_NAME_DATA = "DATA";
        private const string NODE_NAME_BACKGROUND = "BACKGROUND";
        private const string NODE_NAME_LIGHTING = "LIGHTING";
        private const string NODE_NAME_TEXTURE = "TEXTURE";
        private const string NODE_NAME_MESH = "MESH";
        private const string NODE_NAME_OBJECT = "OBJECT";

        // All nodes containing texture information
        private const string NODE_NAME_TEXTURE_RGBA = "TEXTURERGBA";
        private const string NODE_NAME_TEXTURE_RGB = "TEXTURERGB";
        private const string NODE_NAME_TEXTURE_MODULATE = "MODULATE";
        private const string NODE_NAME_TEXTURE_REPEAT = "REPEAT";

        // All nodes containing mesh information
        private const string NODE_NAME_MAT = "MAT";
        private const string NODE_NAME_MAT_DIFFUSE = "DIFF";
        private const string NODE_NAME_NORMAL = "N";
        private const string NODE_NAME_POINT = "P";
        private const string NODE_NAME_FACE = "F";
        private const string NODE_NAME_FACE_VERTEX_1 = "FV1";
        private const string NODE_NAME_FACE_VERTEX_2 = "FV2";
        private const string NODE_NAME_FACE_VERTEX_3 = "FV3";
        private const string NODE_NAME_FACE_MATERIAL_REF = "MATREF";
        private const string NODE_NAME_FACE_POINT_REF = "PREF";
        private const string NODE_NAME_FACE_NORMAL_REF = "NREF";
        private const string NODE_NAME_PATCH = "PATCH";

        // All nodes containing object information
        private const string NODE_NAME_MESHREF = "MESHREF";
        private const string NODE_NAME_TRANS_FORWARD = "FORWARD";
        private const string NODE_NAME_TRANS_UP = "UP";
        private const string NODE_NAME_TRANS_POSITION = "POSITION";
        
        /// <summary>
        /// Imports a model from the given file.
        /// </summary>
        /// <param name="sourceFile">The source file to be loaded.</param>
        /// <param name="importOptions">Some configuration for the importer.</param>
        public ImportedModelContainer ImportModel(ResourceSource sourceFile, ImportOptions importOptions)
        {
            ImportedModelContainer result = new ImportedModelContainer(importOptions);

            // Append an object which transform the whole coordinate system
            ScenePivotObject rootObject = result.CreateAndAddRootObject();

            // Load current model by walking through xml nodes
            using (Stream inStream = sourceFile.OpenInputStream())
            using (XmlReader inStreamXml = XmlReader.Create(inStream, new XmlReaderSettings() { CloseInput = false }))
            {
                while (inStreamXml.Read())
                {
                    try
                    {
                        if (inStreamXml.NodeType != XmlNodeType.Element) { continue; }

                        switch (inStreamXml.Name)
                        {
                            case NODE_NAME_DATA:
                                break;

                            case NODE_NAME_BACKGROUND:
                                break;

                            case NODE_NAME_LIGHTING:
                                break;

                            case NODE_NAME_TEXTURE:
                                ImportTexture(inStreamXml, result);
                                break;

                            case NODE_NAME_MESH:
                                ImportMesh(inStreamXml, result);
                                break;

                            case NODE_NAME_OBJECT:
                                ImportObject(inStreamXml, result, rootObject);
                                break;
                        }
                    }
                    catch(Exception ex)
                    {
                        // Get current line and column
                        int currentLine = 0;
                        int currentColumn = 0;
                        IXmlLineInfo lineInfo = inStreamXml as IXmlLineInfo;
                        if(lineInfo != null)
                        {
                            currentLine = lineInfo.LineNumber;
                            currentColumn = lineInfo.LinePosition;
                        }

                        // Throw an exception with more detail where the error was raised originally
                        throw new FrozenSkyGraphicsException(string.Format(
                            "Unable to read file {0} because of an error while reading xml at {1}",
                            sourceFile,
                            currentLine + ", " + currentColumn), ex);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a default import options object for this importer.
        /// </summary>
        public ImportOptions CreateDefaultImportOptions()
        {
            ImportOptions result = new ImportOptions();
            result.ResourceCoordinateSystem = CoordinateSystem.RightHanded_UpZ;
            return result;
        }

        /// <summary>
        /// Imports the texture node where the xml reader is currently located.
        /// </summary>
        /// <param name="inStreamXml">The xml reader object.</param>
        /// <param name="container">The container where to import to.</param>
        private void ImportTexture(XmlReader inStreamXml, ImportedModelContainer container)
        {
            string id = inStreamXml.GetAttribute("ID");
            int width = 0;
            int height = 0;
            byte[] textureBuffer = null;
            DXGI.Format format = DXGI.Format.R8G8B8A8_UNorm;

            while(inStreamXml.Read())
            {
                // Ending condition
                if ((inStreamXml.NodeType == XmlNodeType.EndElement) &&
                   (inStreamXml.Name == NODE_NAME_TEXTURE))
                {
                    break;
                }

                // Continue condition
                if (inStreamXml.NodeType != XmlNodeType.Element) { continue; }

                // Read all texture data
                switch(inStreamXml.Name)
                {
                    case NODE_NAME_TEXTURE_RGBA:
                        width = Int32.Parse(inStreamXml.GetAttribute("WIDTH"));
                        height = Int32.Parse(inStreamXml.GetAttribute("HEIGHT"));
                        textureBuffer = new byte[width * height * 4];
                        inStreamXml.Read();
                        inStreamXml.ReadContentAsBinHex(textureBuffer, 0, textureBuffer.Length);
                        format = DXGI.Format.R8G8B8A8_UNorm;
                        break;
                   
                    case NODE_NAME_TEXTURE_RGB:
                        width = Int32.Parse(inStreamXml.GetAttribute("WIDTH"));
                        height = Int32.Parse(inStreamXml.GetAttribute("HEIGHT"));
                        byte[] textureBufferDummy = new byte[width * height * 3];
                        textureBuffer = new byte[width * height * 4];
                        inStreamXml.Read();
                        inStreamXml.ReadContentAsBinHex(textureBufferDummy, 0, textureBufferDummy.Length);
                        int loop2 = 0;
                        for (int loop = 0; loop < textureBufferDummy.Length; loop+= 3)
                        {
                            textureBuffer[loop2] = textureBufferDummy[loop];
                            textureBuffer[loop2 + 1] = textureBufferDummy[loop + 1];
                            textureBuffer[loop2 + 2] = textureBufferDummy[loop + 2];
                            textureBuffer[loop2 + 3] = 255;

                            loop2 += 4;
                        }
                        format = DXGI.Format.R8G8B8A8_UNorm;
                        break;

                    case NODE_NAME_TEXTURE_MODULATE:
                        break;

                    case NODE_NAME_TEXTURE_REPEAT:
                        break;

                    default:
                        throw new FrozenSkyGraphicsException(string.Format(
                            "Unknown element {0} in xgl file!",
                            inStreamXml.Name));
                }
            }

            // Check values
            if (width < 0) { throw new FrozenSkyGraphicsException("Unable to read with of a texture!"); }
            if (height < 0) { throw new FrozenSkyGraphicsException("Unable to read height of a texture!"); }
            if (textureBuffer == null) { throw new FrozenSkyGraphicsException("Unable to read the contents of a texture!"); }

            // Add the imported texture
            container.ImportedResources.Add(new ImportedResourceInfo(
                container.GetResourceKey(RES_CLASS_TEXTURE, id),
                () => new StandardTextureResource(textureBuffer, width, height, format)));
        }

        /// <summary>
        /// Imports the texture node where the xml reader is currently located.
        /// </summary>
        /// <param name="inStreamXml">The xml reader object.</param>
        /// <param name="container">The container where to import to.</param>
        private void ImportMesh(XmlReader inStreamXml, ImportedModelContainer container)
        {
            string id = inStreamXml.GetAttribute("ID");
            VertexStructure actVertexStructure = new VertexStructure();
            int minVertexID = int.MaxValue;
            int actVertexIndex = -1;
            int actNormalIndex = -1;
            Vertex actTempVertex = Vertex.Empty;
            int[] actFaceReferences = new int[3];
            Dictionary<int, XglMaterialInfo> localMaterialInfos = new Dictionary<int, XglMaterialInfo>();
            XglMaterialInfo currentMaterial = null;

            while (inStreamXml.Read())
            {
                // Ending condition
                if ((inStreamXml.NodeType == XmlNodeType.EndElement) &&
                    (inStreamXml.Name == NODE_NAME_MESH))
                {
                    break;
                }

                // Continue condition
                if (inStreamXml.NodeType != XmlNodeType.Element) { continue; }

                switch(inStreamXml.Name)
                {
                        // Handle new material node
                    case NODE_NAME_MAT:
                        currentMaterial = new XglMaterialInfo();
                        currentMaterial.ID = Int32.Parse(inStreamXml.GetAttribute("ID"));
                        localMaterialInfos[currentMaterial.ID] = currentMaterial;
                        break;

                    case NODE_NAME_MAT_DIFFUSE:
                        if (currentMaterial == null) { continue; }
                        inStreamXml.Read();
                        currentMaterial.Diffuse = new Color4(inStreamXml.ReadContentAsVector3(), 1f);
                        break;

                        // Read next normal
                    case NODE_NAME_NORMAL:
                        if (minVertexID == int.MaxValue) { minVertexID = Int32.Parse(inStreamXml.GetAttribute("ID")); }
                        actNormalIndex++;
                        actTempVertex = actVertexStructure.EnsureVertexAt(actNormalIndex);
                        inStreamXml.Read();
                        actTempVertex.Normal = inStreamXml.ReadContentAsVector3();
                        actVertexStructure.Vertices[actNormalIndex] = actTempVertex;
                        break;

                        // Read next point
                    case NODE_NAME_POINT:
                        if (minVertexID == int.MaxValue) { minVertexID = Int32.Parse(inStreamXml.GetAttribute("ID")); }
                        actVertexIndex++;
                        actTempVertex = actVertexStructure.EnsureVertexAt(actVertexIndex);
                        inStreamXml.Read();
                        actTempVertex.Position = inStreamXml.ReadContentAsVector3() / 1000f;
                        actVertexStructure.Vertices[actVertexIndex] = actTempVertex;
                        break;

                        // Read next face
                    case NODE_NAME_FACE:
                        actFaceReferences[0] = 0;
                        actFaceReferences[1] = 0;
                        actFaceReferences[2] = 0;
                        int loop = 0;
                        int referencedMat = -1;
                        while(inStreamXml.Read())
                        {
                            // Ending condition
                            if ((inStreamXml.NodeType == XmlNodeType.EndElement) &&
                                (inStreamXml.Name == NODE_NAME_FACE))
                            {
                                break;
                            }
                            if (inStreamXml.NodeType != XmlNodeType.Element) { continue; }

                            // Read next face index
                            if(inStreamXml.Name == NODE_NAME_FACE_MATERIAL_REF)
                            {
                                inStreamXml.Read();
                                referencedMat = inStreamXml.ReadContentAsInt();
                            }
                            else if(inStreamXml.Name == NODE_NAME_FACE_POINT_REF)
                            {
                                if (loop >= 3) { throw new FrozenSkyGraphicsException("Invalid face index count!"); }
                                inStreamXml.Read();
                                actFaceReferences[loop] = inStreamXml.ReadContentAsInt() - minVertexID;
                                loop++;
                            }
                        }
                        if (loop != 3) { throw new FrozenSkyGraphicsException("Invalid face index count!"); }
                        actVertexStructure.AddTriangle(actFaceReferences[0], actFaceReferences[1], actFaceReferences[2]);
                        
                        // Apply material 
                        XglMaterialInfo faceMaterial = null;
                        if(localMaterialInfos.TryGetValue(referencedMat, out faceMaterial))
                        {
                            for(int actFaceLoc=0 ; actFaceLoc<3; actFaceLoc++)
                            {
                                int actVertexIndexInner = actFaceReferences[actFaceLoc];
                                actTempVertex = actVertexStructure.Vertices[actVertexIndexInner];
                                actTempVertex.Color = faceMaterial.Diffuse;
                                actVertexStructure.Vertices[actVertexIndexInner] = actTempVertex;
                            }
                        }
                        break;

                    default:
                        //throw new FrozenSkyGraphicsException(string.Format(
                        //    "Unknown element {0} in xgl file!",
                        //    inStreamXml.Name));
                        break;
                }
            }

            // Add the geometry resource
            container.ImportedResources.Add(new ImportedResourceInfo(
                container.GetResourceKey(RES_CLASS_MESH, id),
                () => new GeometryResource(actVertexStructure)));
        }

        /// <summary>
        /// Imports the texture node where the xml reader is currently located.
        /// </summary>
        /// <param name="inStreamXml">The xml reader object.</param>
        /// <param name="container">The container where to import to.</param>
        private void ImportObject(XmlReader inStreamXml, ImportedModelContainer container, SceneObject parentObject)
        {
            Vector3 upVector = Vector3.UnitY;
            Vector3 forwardVector = Vector3.UnitZ;
            Vector3 positionVector = Vector3.UnitX;
            string meshID = string.Empty;

            // Define a action which finally create the new object
            //  (may be called on two locations here.. so this action was defined
            SceneSpacialObject newObject = null;
            Action actionFinalizeObject = () =>
            {
                if (newObject != null) { return; }

                if (string.IsNullOrEmpty(meshID))
                {
                    // Generate a Pivot on which other objects are orientated
                    newObject = new ScenePivotObject();
                }
                else
                {
                    // Generate an instance of a 3d mesh
                    newObject = new GenericObject(
                        container.GetResourceKey(RES_CLASS_MESH, meshID));
                }
                newObject.Position = positionVector;
                newObject.TransformationType = SpacialTransformationType.TranslationDirection;
                newObject.RotationForward = forwardVector;
                newObject.RotationUp = upVector;
                container.Objects.Add(newObject);

                // Add dependency (parent => child)
                if (parentObject != null)
                {
                    container.ObjectDependencies.Add(Tuple.Create<SceneObject, SceneObject>(
                        parentObject, newObject));
                }
            };

            // Read xml contents
            while (inStreamXml.Read())
            {
                // Ending condition
                if ((inStreamXml.NodeType == XmlNodeType.EndElement) &&
                   (inStreamXml.Name == NODE_NAME_OBJECT))
                {
                    break;
                }

                // Continue condition
                if (inStreamXml.NodeType != XmlNodeType.Element) { continue; }

                switch(inStreamXml.Name)
                {
                    case NODE_NAME_OBJECT:
                        actionFinalizeObject();
                        ImportObject(inStreamXml, container, newObject);
                        break;

                    case NODE_NAME_TRANS_FORWARD:
                        inStreamXml.Read();
                        forwardVector = inStreamXml.ReadContentAsVector3();
                        break;

                    case NODE_NAME_TRANS_UP:
                        inStreamXml.Read();
                        upVector = inStreamXml.ReadContentAsVector3();
                        break;

                    case NODE_NAME_TRANS_POSITION:
                        inStreamXml.Read();
                        positionVector = inStreamXml.ReadContentAsVector3() / 1000f;
                        break;

                    case NODE_NAME_MESHREF:
                        inStreamXml.Read();
                        meshID = inStreamXml.ReadContentAsString();
                        break;
                }
            }

            // finalize object here
            actionFinalizeObject();
        }
    }
}
#endif