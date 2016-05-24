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
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Multimedia.Objects
{
    internal class ObjFileReader
    {
        #region Constants
        private static readonly char[] ARGUMENT_SPLITTER = new char[] { ' ' };
        private static readonly CultureInfo FILE_CULTURE = new CultureInfo("en-US");
        private static readonly NumberFormatInfo FILE_NUMBER_FORMAT = FILE_CULTURE.NumberFormat;
        #endregion

        #region Parameters
        private ResourceLink m_resource;
        private ImportedModelContainer m_targetContainer;
        #endregion

        #region Current object
        private VertexStructure m_currentVertexStructure;
        private VertexStructureSurface m_currentSurface;
        private VertexStructureSurface m_currentMaterialDefinition;
        #endregion

        #region Raw data
        private List<Vector3> m_rawVertices;
        private List<Vector3> m_rawNormals;
        private List<Vector2> m_rawTextureCoordinates;
        #endregion

        #region Common objects (to prevent high load on garbage collector)
        private float[] m_dummyFloatArguments_3 = new float[3];
        private int[] m_dummyIntArguments_3 = new int[3];
        private FaceIndices[] m_dummyFaceIndices_3 = new FaceIndices[3];
        private FaceIndices[] m_dummyFaceIndices_4 = new FaceIndices[4];
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjFileReader"/> class.
        /// </summary>
        /// <param name="resource">The resource to be loaded.</param>
        /// <param name="targetContainer">The target ModelContainer.</param>
        public ObjFileReader(ResourceLink resource, ImportedModelContainer targetContainer)
        {
            m_resource = resource;
            m_targetContainer = targetContainer;

            m_rawVertices = new List<Vector3>(1024);
            m_rawNormals = new List<Vector3>(1014);
            m_rawTextureCoordinates = new List<Vector2>(1024);

            m_currentVertexStructure = new VertexStructure();
        }

        /// <summary>
        /// Reads all contents of the Obj file.
        /// </summary>
        public void Read()
        {
            int actLineNumber = 0;
            try
            {
                using (StreamReader inStreamReader = new StreamReader(m_resource.OpenInputStream()))
                {
                    StringBuilder multiLineBuilder = new StringBuilder(32);
                    while (!inStreamReader.EndOfStream)
                    {
                        actLineNumber++;

                        string actLine = inStreamReader.ReadLine();
                        if (string.IsNullOrEmpty(actLine)) { continue; }

                        // Remove leading and ending spaces
                        actLine = actLine.Trim();

                        // Discard comments 
                        if (actLine[0] == '#') { continue; }

                        // Handle multiline entris (they have a \ at the end of the line)
                        if (actLine[actLine.Length - 1] == '\\')
                        {
                            multiLineBuilder.Append(actLine.TrimEnd('\\'));
                            continue;
                        }
                        if (multiLineBuilder.Length > 0)
                        {
                            multiLineBuilder.Append(actLine);
                            actLine = multiLineBuilder.ToString();
                            multiLineBuilder.Clear();
                        }

                        // Parse current keyword and arguments
                        string actKeyword = null;
                        string actArguments = null;
                        if (!TrySplitLine(actLine, out actKeyword, out actArguments))
                        {
                            continue;
                        }

                        // Handle current keyword
                        switch (actKeyword.ToLower())
                        {
                            case "mtllib":
                                HandleKeyword_Obj_Mtllib(actArguments);
                                break;

                            case "usemtl":
                                HandleKeyword_Obj_UseMtl(actArguments);
                                break;

                            case "o":
                                break;

                            case "v":
                                HandleKeyword_Obj_V(actArguments);
                                break;

                            case "vn":
                                HandleKeyword_Obj_VN(actArguments);
                                break;

                            case "vt":
                                HandleKeyword_Obj_VT(actArguments);
                                break;

                            case "vp":
                                // Parameter space is not supported
                                break;

                            case "f":
                                HandleKeyword_Obj_F(actArguments);
                                break;

                            case "p":
                            case "l":
                                // Points and lines not supported currently
                                break;
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                throw new SeeingSharpGraphicsException($"Unable to read obj file {m_resource}: Error at line {actLineNumber}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Reads all contents of the given material library.
        /// </summary>
        private void ReadMaterialLibrary(ResourceLink resource)
        {
            int actLineNumber = 0;
            try
            {
                using (StreamReader inStreamReader = new StreamReader(m_resource.OpenInputStream()))
                {
                    StringBuilder multiLineBuilder = new StringBuilder(32);
                    while (!inStreamReader.EndOfStream)
                    {
                        actLineNumber++;

                        string actLine = inStreamReader.ReadLine();
                        if (string.IsNullOrEmpty(actLine)) { continue; }

                        // Remove leading and ending spaces
                        actLine = actLine.Trim();

                        // Discard comments 
                        if (actLine[0] == '#') { continue; }

                        // Handle multiline entris (they have a \ at the end of the line)
                        if (actLine[actLine.Length - 1] == '\\')
                        {
                            multiLineBuilder.Append(actLine.TrimEnd('\\'));
                            continue;
                        }
                        if (multiLineBuilder.Length > 0)
                        {
                            multiLineBuilder.Append(actLine);
                            actLine = multiLineBuilder.ToString();
                            multiLineBuilder.Clear();
                        }

                        // Parse current keyword and arguments
                        string actKeyword = null;
                        string actArguments = null;
                        if (!TrySplitLine(actLine, out actKeyword, out actArguments))
                        {
                            continue;
                        }

                        switch(actKeyword.ToLower())
                        {
                            case "newmtl":
                                HandleKeyword_Mtl_NewMtl(actArguments);
                                break;

                            case "ka":
                                HandleKeyword_Mtl_Ka(actArguments);
                                break;

                            case "kd":
                                HandleKeyword_Mtl_Kd(actArguments);
                                break;

                            case "ks":
                                HandleKeyword_Mtl_Ks(actArguments);
                                break;

                            case "ke":
                                break;

                            case "ni":       // Optical density
                            case "ns":       // Specular exponent
                            case "d":        // Dissolve factor (transparency)
                            case "tf":       // Transmission filter
                            case "sharpness":
                            case "illum":    // Illumination model
                                // Not supported
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SeeingSharpGraphicsException($"Unable to read material library {m_resource}: Error at line {actLineNumber}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Starts reading all configured material libraries.
        /// </summary>
        private void HandleKeyword_Obj_Mtllib(string arguments)
        {
            string[] mtlFiles = arguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for(int loop=0; loop<mtlFiles.Length; loop++)
            {
                ResourceLink mtlLibResource = m_resource.GetForAnotherFile(mtlFiles[loop]);
                this.ReadMaterialLibrary(mtlLibResource);
            }
        }

        /// <summary>
        /// Reads the definition of a new material.
        /// </summary>
        /// <param name="arguments">Passed argumetns.</param>
        private void HandleKeyword_Mtl_NewMtl(string arguments)
        {
            string[] names = arguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if(names.Length != 1)
            {
                throw new SeeingSharpGraphicsException($"Invalid count of arguments for keyword 'newmtl', (expected=1, got={names.Length})!");
            }

            m_currentMaterialDefinition = m_currentVertexStructure.CreateSurface(name: names[0]);
        }

        /// <summary>
        /// Reads ambient component of a material.
        /// </summary>
        private void HandleKeyword_Mtl_Ka(string arguments)
        {
            m_currentMaterialDefinition.MaterialProperties.AmbientColor = this.ParseColor("Ka", arguments);
        }

        /// <summary>
        /// Reads diffuse component of a material.
        /// </summary>
        private void HandleKeyword_Mtl_Kd(string arguments)
        {
            m_currentMaterialDefinition.MaterialProperties.DiffuseColor = this.ParseColor("Kd", arguments);
        }

        /// <summary>
        /// Reads specular component of a material.
        /// </summary>
        private void HandleKeyword_Mtl_Ks(string arguments)
        {
            m_currentMaterialDefinition.MaterialProperties.SpecularColor = this.ParseColor("Ks", arguments);
        }

        /// <summary>
        /// Applies the material with the given name for following surfaces.
        /// </summary>
        /// <param name="arguments">Passed argumetns.</param>
        private void HandleKeyword_Obj_UseMtl(string arguments)
        {
            string[] names = arguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (names.Length != 1)
            {
                throw new SeeingSharpGraphicsException($"Invalid count of arguments for keyword 'usemtl', (expected=1, got={names.Length})!");
            }

            m_currentSurface = m_currentVertexStructure.CreateOrGetExistingSurfaceByName((string)names[0]);
        }

        /// <summary>
        /// Reads a line containing vertex information.
        /// </summary>
        private void HandleKeyword_Obj_V(string arguments)
        {
            // Split arguments
            string[] vertexArguments = arguments.Split(ARGUMENT_SPLITTER, StringSplitOptions.RemoveEmptyEntries);
            if((vertexArguments.Length < 3) || (vertexArguments.Length > 4))
            {
                throw new SeeingSharpGraphicsException($"Invalid count of arguments for keyword 'v', (expected=3, got={vertexArguments.Length})!");
            }

            // Parse vertex arguments (we don't support the w coordinate)
            if(!TryParseStringsToFloats(vertexArguments, m_dummyFloatArguments_3, 3))
            {
                throw new SeeingSharpGraphicsException($"Unable to parse vertex arguments: {arguments}!");
            }

            // Store vertex
            m_rawVertices.Add(new Vector3(m_dummyFloatArguments_3[0], m_dummyFloatArguments_3[1], m_dummyFloatArguments_3[2]));
        }

        /// <summary>
        /// Reads a line containing vertex normal information.
        /// </summary>
        private void HandleKeyword_Obj_VN(string arguments)
        {
            // Split arguments
            string[] vertexArguments = arguments.Split(ARGUMENT_SPLITTER, StringSplitOptions.RemoveEmptyEntries);
            if (vertexArguments.Length != 3)
            {
                throw new SeeingSharpGraphicsException($"Invalid count of arguments for keyword 'v', (expected=3, got={vertexArguments.Length})!");
            }

            // Parse normal arguments
            if (!TryParseStringsToFloats(vertexArguments, m_dummyFloatArguments_3, 3))
            {
                throw new SeeingSharpGraphicsException($"Unable to parse normal arguments: {arguments}!");
            }

            // Store vertex
            m_rawNormals.Add(new Vector3(m_dummyFloatArguments_3[0], m_dummyFloatArguments_3[1], m_dummyFloatArguments_3[2]));
        }

        /// <summary>
        /// Reads a line containing texture coordinate information.
        /// </summary>
        private void HandleKeyword_Obj_VT(string arguments)
        {
            // Split arguments
            string[] vertexArguments = arguments.Split(ARGUMENT_SPLITTER, StringSplitOptions.RemoveEmptyEntries);
            if ((vertexArguments.Length < 1) || (vertexArguments.Length > 3))
            {
                throw new SeeingSharpGraphicsException($"Invalid count of arguments for keyword 'v', (expected=3, got={vertexArguments.Length})!");
            }

            // We don't support 1D texture, so add a dummy coordinate here
            if(vertexArguments.Length == 1)
            {
                m_rawTextureCoordinates.Add(new Vector2(0f, 0f));
                return;
            }

            // Parse normal arguments 
            //  We don't support 3D texture, so the third coordinate is ignored
            if (!TryParseStringsToFloats(vertexArguments, m_dummyFloatArguments_3, 2))
            {
                throw new SeeingSharpGraphicsException($"Unable to parse normal arguments: {arguments}!");
            }

            // Store vertex
            m_rawTextureCoordinates.Add(new Vector2(m_dummyFloatArguments_3[0], m_dummyFloatArguments_3[1]));
        }

        /// <summary>
        /// Reads a line containing face information.
        /// </summary>
        private void HandleKeyword_Obj_F(string arguments)
        {
            // Split arguments
            string[] faceArguments = arguments.Split(ARGUMENT_SPLITTER, StringSplitOptions.RemoveEmptyEntries);
            if (faceArguments.Length < 3)
            {
                throw new SeeingSharpGraphicsException($"Invalid count of arguments for keyword 'f', (expected = >2, got={faceArguments.Length})!");
            }

            // Prepare FaceIndices array
            FaceIndices[] faceIndices = null;
            if (faceArguments.Length == 3) { faceIndices = m_dummyFaceIndices_3; }
            else if (faceArguments.Length == 4) { faceIndices = m_dummyFaceIndices_4; }
            else { faceIndices = new FaceIndices[faceArguments.Length]; }

            // Parse and preprocess all arumgents
            for (int loop = 0; loop < faceArguments.Length; loop++)
            {
                ParseFaceData(faceArguments[loop], m_dummyIntArguments_3);

                // Preprocess vertex index
                int actIndex = m_dummyIntArguments_3[0];
                if (actIndex < 0)
                {
                    int newIndex = m_rawVertices.Count - actIndex;
                    if (newIndex < 0)
                    {
                        throw new SeeingSharpGraphicsException($"Invalid vertex index: {actIndex} (we currently have {m_rawVertices.Count} vertices)!");
                    }
                    m_dummyIntArguments_3[0] = newIndex;
                }
                faceIndices[loop].VertexIndex = m_dummyIntArguments_3[0];

                // Preprocess texture coordinates
                actIndex = m_dummyIntArguments_3[1];
                if ((actIndex < 0) && (actIndex != Int32.MinValue))
                {
                    int newIndex = m_rawTextureCoordinates.Count - actIndex;
                    if (newIndex < 0)
                    {
                        throw new SeeingSharpGraphicsException($"Invalid vertex index: {actIndex} (we currently have {m_rawTextureCoordinates.Count} texture coordinates)!");
                    }
                    m_dummyIntArguments_3[1] = newIndex;
                }
                faceIndices[loop].TextureCoordinateIndex = m_dummyIntArguments_3[1];

                // Preprocess normal coordinates
                actIndex = m_dummyIntArguments_3[2];
                if ((actIndex < 0) && (actIndex != Int32.MinValue))
                {
                    int newIndex = m_rawNormals.Count - actIndex;
                    if (newIndex < 0)
                    {
                        throw new SeeingSharpGraphicsException($"Invalid vertex index: {actIndex} (we currently have {m_rawNormals.Count} normals)!");
                    }
                    m_dummyIntArguments_3[2] = newIndex;
                }
                faceIndices[loop].NormalIndex = m_dummyIntArguments_3[2];
            }
            
            // Generate vertices and triangles on current VertexStructure
            if(faceIndices.Length == 3)
            {
                GenerateFaceVertices(faceIndices);
                int highestVertexIndex = m_currentVertexStructure.CountVertices - 1;
                m_currentSurface.AddTriangle(
                    highestVertexIndex - 2,
                    highestVertexIndex - 1,
                    highestVertexIndex);
            }
            else
            {
                m_currentSurface.AddPolygonByCuttingEars(
                    GenerateFaceVertices(faceIndices));
            }
        }

        /// <summary>
        /// Generates all vertices for given FaceIndices.
        /// </summary>
        private IEnumerable<int> GenerateFaceVertices(FaceIndices[] faceIndices)
        {
            // Generate vertex
            for (int loop = 0; loop < faceIndices.Length; loop++)
            {
                FaceIndices actFaceIndices = faceIndices[loop];
                Vertex actVertex = new Vertex();

                actVertex.Position = m_rawVertices[actFaceIndices.VertexIndex];
                if (actFaceIndices.TextureCoordinateIndex > Int32.MinValue)
                {
                    actVertex.TexCoord = m_rawTextureCoordinates[actFaceIndices.TextureCoordinateIndex];
                }
                if (actFaceIndices.NormalIndex > Int32.MinValue)
                {
                    actVertex.Normal = m_rawNormals[actFaceIndices.NormalIndex];
                }

                yield return m_currentVertexStructure.AddVertex(actVertex);
            }
        }

        /// <summary>
        /// Tries to split the given line into keyword and arguments.
        /// Returns true if this is a valid line.
        /// </summary>
        /// <param name="line">The line to be splitted.</param>
        /// <param name="keyword">The keyword.</param>
        /// <param name="arguments">All arguments for the keyword.</param>
        private static bool TrySplitLine(string line, out string keyword, out string arguments)
        {
            int idx = line.IndexOf(' ');
            if (idx < 0)
            {
                keyword = line;
                arguments = null;
                return false;
            }

            keyword = line.Substring(0, idx);
            arguments = line.Substring(idx + 1);
            return true;
        }

        /// <summary>
        /// Tries to parse all given strings into the given float array..
        /// </summary>
        /// <param name="sourceStrings">The source strings.</param>
        /// <param name="targetFloats">The target floats.</param>
        /// <param name="countValuesToParse">Total count of values to parse.</param>
        private static bool TryParseStringsToFloats(string[] sourceStrings, float[] targetFloats, int countValuesToParse)
        {
            for (int loop = 0; loop < countValuesToParse; loop++)
            {
                float actParsedFloat = 0f;
                if(!float.TryParse(sourceStrings[loop], NumberStyles.Float, FILE_NUMBER_FORMAT, out actParsedFloat))
                {
                    return false;
                }
                targetFloats[loop] = actParsedFloat;
            }
            return true;
        }

        /// <summary>
        /// Tries to parse all given strings into the given int array..
        /// </summary>
        /// <param name="sourceStrings">The source strings.</param>
        /// <param name="targetInts">The target ints.</param>
        private static bool TryParseStringsToInts(string[] sourceStrings, int[] targetInts)
        {
            int length = sourceStrings.Length;
            for (int loop = 0; loop < length; loop++)
            {
                int actParsedInt = 0;
                if (!int.TryParse(sourceStrings[loop], NumberStyles.Integer, FILE_NUMBER_FORMAT, out actParsedInt))
                {
                    return false;
                }
                targetInts[loop] = actParsedInt;
            }
            return true;
        }

        /// <summary>
        /// Tries to parse all given strings into the given int array..
        /// </summary>
        /// <param name="sourceString">The string to be parsed.</param>
        /// <param name="targetInts">The target array where to put all values.</param>
        private static bool ParseFaceData(string sourceString, int[] targetInts)
        {
            string[] splitted = sourceString.Split('/');
            if((splitted.Length < 1) || (splitted.Length > 3))
            {
                throw new SeeingSharpGraphicsException($"Invalid face argument: {sourceString}! (invalid count of items)");
            }

            for (int loop = 0; loop < targetInts.Length; loop++)
            {
                if ((splitted.Length <= loop) ||
                    (string.IsNullOrEmpty(splitted[loop])))
                {
                    if(loop==0)
                    {
                        throw new SeeingSharpGraphicsException($"Invalid face argument: {sourceString} (first value missing)!");
                    }

                    // Missing value
                    targetInts[loop] = Int32.MinValue;
                }
                else
                {
                    int parsedValue = 0;
                    if(!Int32.TryParse(splitted[loop], NumberStyles.Integer, FILE_NUMBER_FORMAT, out parsedValue))
                    {
                        throw new SeeingSharpGraphicsException($"Invalid face argument: {sourceString} (unable to parse int)!");
                    }
                    targetInts[loop] = parsedValue;
                }
            }

            return true;
        }

        private Color4 ParseColor(string keyword, string arguments)
        {
            if (arguments.Contains("spectral") ||
               arguments.Contains("xyz"))
            {
                // We don't support spectral or xyz arguments
                return new Color4(1f, 1f, 1f, 1f);
            }

            // Split arguments
            string[] ambientArguments = arguments.Split(ARGUMENT_SPLITTER, StringSplitOptions.RemoveEmptyEntries);
            if ((ambientArguments.Length < 1) ||
                (ambientArguments.Length < 3))
            {
                throw new SeeingSharpGraphicsException($"Invalid count of arguments for keyword '{keyword}', (expected=1-3, got={ambientArguments.Length})!");
            }

            // Parse vertex arguments (we don't support the w coordinate)
            if (!TryParseStringsToFloats(ambientArguments, m_dummyFloatArguments_3, ambientArguments.Length))
            {
                throw new SeeingSharpGraphicsException($"Unable to parse arguments for keyword {keyword}: {arguments}!");
            }

            // G and B components are optional...
            for (int loop = 2; loop > 0; loop--)
            {
                if (ambientArguments.Length <= loop)
                {
                    m_dummyFaceIndices_3[loop] = m_dummyFaceIndices_3[0];
                }
            }

            // Return current color value
            return new Color4(m_dummyFloatArguments_3[0], m_dummyFloatArguments_3[1], m_dummyFloatArguments_3[2], 1f);
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        private struct FaceIndices
        {
            public int VertexIndex;
            public int TextureCoordinateIndex;
            public int NormalIndex;
        }
    }
}
