﻿#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
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

using FrozenSky.Multimedia.Core;
using System.Collections.Generic;

// Some namespace mappings
using DWrite = SharpDX.DirectWrite;

namespace FrozenSky.Multimedia.Objects
{
    public partial class VertexStructure
    {
        /// <summary>
        /// Builds the text geometry for the given string.
        /// </summary>
        /// <param name="stringToBuild">The string to build within the geometry.</param>
        public void BuildTextGeometry(string stringToBuild)
        {
            BuildTextGeometry(stringToBuild, TextGeometryOptions.Default);
        }

        /// <summary>
        /// Builds the text geometry for the given string.
        /// </summary>
        /// <param name="stringToBuild">The string to build within the geometry.</param>
        /// <param name="geometryOptions">Some configuration for geometry creation.</param>
        public void BuildTextGeometry(string stringToBuild, TextGeometryOptions geometryOptions)
        {
            DWrite.Factory writeFactory = GraphicsCore.Current.FactoryDWrite;

            //TODO: Cache font objects

            //Get DirectWrite font weight
            DWrite.FontWeight fontWeight = DWrite.FontWeight.Normal;
            switch (geometryOptions.FontWeight)
            {
                case FontGeometryWeight.Bold:
                    fontWeight = DWrite.FontWeight.Bold;
                    break;

                default:
                    fontWeight = DWrite.FontWeight.Normal;
                    break;
            }

            //Get DirectWrite font style
            DWrite.FontStyle fontStyle = DWrite.FontStyle.Normal;
            switch (geometryOptions.FontStyle)
            {
                case FontGeometryStyle.Italic:
                    fontStyle = DWrite.FontStyle.Italic;
                    break;

                case FontGeometryStyle.Oblique:
                    fontStyle = DWrite.FontStyle.Oblique;
                    break;

                default:
                    fontStyle = DWrite.FontStyle.Normal;
                    break;
            }

            //Create the text layout object
            try
            {
                DWrite.TextLayout textLayout = new DWrite.TextLayout(
                    writeFactory, stringToBuild,
                    new DWrite.TextFormat(
                        writeFactory, geometryOptions.FontFamily, fontWeight, fontStyle, geometryOptions.FontSize),
                    float.MaxValue, float.MaxValue);

                //Render the text using the vertex structure text renderer
                using (VertexStructureTextRenderer textRenderer = new VertexStructureTextRenderer(this, geometryOptions))
                {
                    textLayout.Draw(textRenderer, 0f, 0f);
                }
            }
            catch (SharpDX.SharpDXException)
            {
                //TODO: Display some error
            }
        }

        /// <summary>
        /// Builds a plain polygon using the given coordinates.
        /// </summary>
        /// <param name="coordinates">The coordinates to build the polygon from.</param>
        public void BuildPlainPolygon(Vector3[] coordinates)
        {
            //Build the polygon
            Polygon polygon = new Polygon(coordinates);

            //Try to triangulate it
            IEnumerable<int> indices = polygon.TriangulateUsingCuttingEars();
            if (indices == null) { throw new FrozenSkyGraphicsException("Unable to triangulate given polygon!"); }

            //Append all vertices
            int baseIndex = (int)m_vertices.Count;
            for (int loopCoordinates = 0; loopCoordinates < coordinates.Length; loopCoordinates++)
            {
                this.AddVertex(new Vertex(coordinates[loopCoordinates]));
            }

            //Append all indices
            using (IEnumerator<int> indexEnumerator = indices.GetEnumerator())
            {
                while (indexEnumerator.MoveNext())
                {
                    int index1 = indexEnumerator.Current;
                    int index2 = 0;
                    int index3 = 0;

                    if (indexEnumerator.MoveNext()) { index2 = indexEnumerator.Current; } else { break; }
                    if (indexEnumerator.MoveNext()) { index3 = indexEnumerator.Current; } else { break; }

                    this.AddTriangle((int)(index1 + baseIndex), (int)(index2 + baseIndex), (int)(index3 + baseIndex));
                }
            }
        }
    }
}