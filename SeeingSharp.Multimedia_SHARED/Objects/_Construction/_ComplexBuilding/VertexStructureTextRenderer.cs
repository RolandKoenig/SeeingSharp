﻿#region License information (SeeingSharp and all based games/applications)
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
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SeeingSharp.Multimedia.Core;

// Namespace mappings
using SDX = SharpDX;

namespace SeeingSharp.Multimedia.Objects
{
    internal class VertexStructureTextRenderer : TextRendererBase
    {
        private VertexStructureSurface m_targetSurface;
        private TextGeometryOptions m_geometryOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexStructureTextRenderer" /> class.
        /// </summary>
        public VertexStructureTextRenderer(VertexStructureSurface targetSurface, TextGeometryOptions textGeometryOptions)
            : base()
        {
            m_targetSurface = targetSurface;
            m_geometryOptions = textGeometryOptions;
        }

        /// <summary>
        /// IDWriteTextLayout::Draw calls this function to instruct the client to render a run of glyphs.
        /// </summary>
        /// <param name="clientDrawingContext">The application-defined drawing context passed to  <see cref="M:SharpDX.DirectWrite.TextLayout.Draw_(System.IntPtr,System.IntPtr,System.Single,System.Single)" />.</param>
        /// <param name="baselineOriginX">The pixel location (X-coordinate) at the baseline origin of the glyph run.</param>
        /// <param name="baselineOriginY">The pixel location (Y-coordinate) at the baseline origin of the glyph run.</param>
        /// <param name="measuringMode">The measuring method for glyphs in the run, used with the other properties to determine the rendering mode.</param>
        /// <param name="glyphRun">Pointer to the glyph run instance to render.</param>
        /// <param name="glyphRunDescription">A pointer to the optional glyph run description instance which contains properties of the characters  associated with this run.</param>
        /// <param name="clientDrawingEffect">Application-defined drawing effects for the glyphs to render. Usually this argument represents effects such as the foreground brush filling the interior of text.</param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
        /// </returns>
        /// <unmanaged>HRESULT DrawGlyphRun([None] void* clientDrawingContext,[None] FLOAT baselineOriginX,[None] FLOAT baselineOriginY,[None] DWRITE_MEASURING_MODE measuringMode,[In] const DWRITE_GLYPH_RUN* glyphRun,[In] const DWRITE_GLYPH_RUN_DESCRIPTION* glyphRunDescription,[None] IUnknown* clientDrawingEffect)</unmanaged>
        /// <remarks>
        /// The <see cref="M:SharpDX.DirectWrite.TextLayout.Draw_(System.IntPtr,System.IntPtr,System.Single,System.Single)" /> function calls this callback function with all the information about glyphs to render. The application implements this callback by mostly delegating the call to the underlying platform's graphics API such as {{Direct2D}} to draw glyphs on the drawing context. An application that uses GDI can implement this callback in terms of the <see cref="M:SharpDX.DirectWrite.BitmapRenderTarget.DrawGlyphRun(System.Single,System.Single,SharpDX.Direct2D1.MeasuringMode,SharpDX.DirectWrite.GlyphRun,SharpDX.DirectWrite.RenderingParams,SharpDX.Color4)" /> method.
        /// </remarks>
        public override SDX.Result DrawGlyphRun(
            object clientDrawingContext, float baselineOriginX, float baselineOriginY,
            MeasuringMode measuringMode, GlyphRun glyphRun, GlyphRunDescription glyphRunDescription, SDX.ComObject clientDrawingEffect)
        {
            if ((glyphRun.Indices == null) ||
               (glyphRun.Indices.Length == 0))
            {
                return SDX.Result.Ok; ;
            }

            SharpDX.DirectWrite.Factory dWriteFactory = GraphicsCore.Current.FactoryDWrite;
            SharpDX.Direct2D1.Factory d2DFactory = GraphicsCore.Current.FactoryD2D;

            // Extrude geometry data out of given glyph run
            SimplePolygon2DGeometrySink geometryExtruder = new SimplePolygon2DGeometrySink(new Vector2(baselineOriginX, baselineOriginY));
            using (PathGeometry pathGeometry = new PathGeometry(d2DFactory))
            {
                // Write all geometry data into a standard PathGeometry object
                using (GeometrySink geoSink = pathGeometry.Open())
                {
                    glyphRun.FontFace.GetGlyphRunOutline(
                        glyphRun.FontSize,
                        glyphRun.Indices,
                        glyphRun.Advances,
                        glyphRun.Offsets,
                        glyphRun.IsSideways,
                        glyphRun.BidiLevel % 2 == 1,
                        geoSink);
                    geoSink.Close();
                }

                // Simplify written geometry and write it into own structure
                pathGeometry.Simplify(GeometrySimplificationOption.Lines, m_geometryOptions.SimplificationFlatternTolerance, geometryExtruder);
            }

            // Structure for caching the result
            VertexStructure tempStructure = new VertexStructure();
            VertexStructureSurface tempSurface = tempStructure.CreateSurface();

            // Create the text surface
            if (m_geometryOptions.MakeSurface)
            {
                // Separate polygons by clock direction
                // Order polygons as needed for further hole finding algorithm
                IEnumerable<Polygon2D> fillingPolygons = geometryExtruder.GeneratedPolygons
                    .Where(actPolygon => actPolygon.EdgeOrder == EdgeOrder.CounterClockwise)
                    .OrderBy(actPolygon => actPolygon.BoundingBox.Size.X * actPolygon.BoundingBox.Size.Y);
                List<Polygon2D> holePolygons = geometryExtruder.GeneratedPolygons
                    .Where(actPolygon => actPolygon.EdgeOrder == EdgeOrder.Clockwise)
                    .OrderByDescending(actPolygon => actPolygon.BoundingBox.Size.X * actPolygon.BoundingBox.Size.Y)
                    .ToList();

                // Build geometry for all polygons
                int loopPolygon = 0;
                foreach (Polygon2D actFillingPolygon in fillingPolygons)
                {
                    // Find all corresponding holes
                    BoundingBox2D actFillingPolygonBounds = actFillingPolygon.BoundingBox;
                    IEnumerable<Polygon2D> correspondingHoles = holePolygons
                        .Where(actHolePolygon => actHolePolygon.BoundingBox.IsContainedBy(actFillingPolygonBounds))
                        .ToList();

                    // Two steps here:
                    // - Merge current filling polygon and all its holes.
                    // - Remove found holes from current hole list
                    Polygon2D polygonForRendering = actFillingPolygon;
                    Polygon2D polygonForTriangulation = actFillingPolygon.Clone();
                    List<Vector2> cutPoints = new List<Vector2>();
                    foreach (Polygon2D actHole in correspondingHoles)
                    {
                        holePolygons.Remove(actHole);
                        polygonForRendering = polygonForRendering.MergeWithHole(actHole, Polygon2DMergeOptions.Default, cutPoints);
                        polygonForTriangulation = polygonForTriangulation.MergeWithHole(actHole, new Polygon2DMergeOptions() { MakeMergepointSpaceForTriangulation = true });
                    }

                    loopPolygon++;
                    int actBaseIndex = (int)tempStructure.CountVertices;

                    EdgeOrder edgeOrder = polygonForRendering.EdgeOrder;
                    float edgeSize = edgeOrder == EdgeOrder.CounterClockwise ? 0.1f : 0.4f;

                    // Append all vertices to temporary VertexStructure
                    for (int loop = 0; loop < polygonForRendering.Vertices.Count; loop++)
                    {
                        // Calculate 3d location and texture coordinate
                        Vector3 actVertexLocation = new Vector3(
                            polygonForRendering.Vertices[loop].X,
                            0f,
                            polygonForRendering.Vertices[loop].Y);
                        Vector2 actTexCoord = new Vector2(
                            (polygonForRendering.Vertices[loop].X - polygonForRendering.BoundingBox.Location.X) / polygonForRendering.BoundingBox.Size.X,
                            (polygonForRendering.Vertices[loop].Y - polygonForRendering.BoundingBox.Location.Y) / polygonForRendering.BoundingBox.Size.Y);
                        if (float.IsInfinity(actTexCoord.X) || float.IsNaN(actTexCoord.X)) { actTexCoord.X = 0f; }
                        if (float.IsInfinity(actTexCoord.Y) || float.IsNaN(actTexCoord.Y)) { actTexCoord.Y = 0f; }

                        // Append the vertex to the result
                        tempStructure.AddVertex(
                            new Vertex(
                                actVertexLocation,
                                m_geometryOptions.SurfaceVertexColor,
                                actTexCoord,
                                new Vector3(0f, 1f, 0f)));
                    }

                    // Generate cubes on each vertex if requested
                    if (m_geometryOptions.GenerateCubesOnVertices)
                    {
                        for (int loop = 0; loop < polygonForRendering.Vertices.Count; loop++)
                        {
                            Color4 colorToUse = Color4.GreenColor;
                            float pointRenderSize = 0.1f;
                            if (cutPoints.Contains(polygonForRendering.Vertices[loop]))
                            {
                                colorToUse = Color4.RedColor;
                                pointRenderSize = 0.15f;
                            }

                            Vector3 actVertexLocation = new Vector3(
                                polygonForRendering.Vertices[loop].X,
                                0f,
                                polygonForRendering.Vertices[loop].Y);
                            tempSurface.BuildCube24V(actVertexLocation, pointRenderSize, colorToUse);
                        }
                    }

                    // Triangulate the polygon
                    IEnumerable<int> triangleIndices = polygonForTriangulation.TriangulateUsingCuttingEars();
                    if (triangleIndices == null) { continue; }
                    if (triangleIndices == null) { throw new SeeingSharpGraphicsException("Unable to triangulate given PathGeometry object!"); }

                    // Append all triangles to the temporary structure
                    using (IEnumerator<int> indexEnumerator = triangleIndices.GetEnumerator())
                    {
                        while (indexEnumerator.MoveNext())
                        {
                            int index1 = indexEnumerator.Current;
                            int index2 = 0;
                            int index3 = 0;

                            if (indexEnumerator.MoveNext()) { index2 = indexEnumerator.Current; } else { break; }
                            if (indexEnumerator.MoveNext()) { index3 = indexEnumerator.Current; } else { break; }

                            tempSurface.AddTriangle(
                                (int)(actBaseIndex + index3),
                                (int)(actBaseIndex + index2),
                                (int)(actBaseIndex + index1));
                        }
                    }
                }
            }

            // Make volumetric outlines
            int triangleCountWithoutSide = tempSurface.CountTriangles;
            if (m_geometryOptions.MakeVolumetricText)
            {
                float volumetricTextDepth = m_geometryOptions.VolumetricTextDepth;
                if(m_geometryOptions.VerticesScaleFactor > 0f)
                {
                    volumetricTextDepth = volumetricTextDepth / m_geometryOptions.VerticesScaleFactor;
                }

                // Add all side surfaces
                    foreach (Polygon2D actPolygon in geometryExtruder.GeneratedPolygons)
                {
                    foreach (Line2D actLine in actPolygon.Lines)
                    {
                        tempSurface.BuildRect4V(
                            new Vector3(actLine.StartPosition.X, -volumetricTextDepth, actLine.StartPosition.Y),
                            new Vector3(actLine.EndPosition.X, -volumetricTextDepth, actLine.EndPosition.Y),
                            new Vector3(actLine.EndPosition.X, 0f, actLine.EndPosition.Y),
                            new Vector3(actLine.StartPosition.X, 0f, actLine.StartPosition.Y),
                            m_geometryOptions.VolumetricSideSurfaceVertexColor);
                    }
                }
            }

            // Do also make back surface?
            if (m_geometryOptions.MakeBackSurface)
            {
                for (int loop = 0; loop < triangleCountWithoutSide; loop++)
                {
                    Triangle triangle = tempSurface.Triangles[loop];
                    Vertex vertex0 = tempStructure.Vertices[triangle.Index1];
                    Vertex vertex1 = tempStructure.Vertices[triangle.Index2];
                    Vertex vertex2 = tempStructure.Vertices[triangle.Index3];
                    Vector3 changeVector = new Vector3(0f, -m_geometryOptions.VolumetricTextDepth, 0f);

                    tempSurface.AddTriangle(
                        vertex2.Copy(vertex2.Position - changeVector, Vector3.Negate(vertex2.Normal)),
                        vertex1.Copy(vertex1.Position - changeVector, Vector3.Negate(vertex1.Normal)),
                        vertex0.Copy(vertex0.Position - changeVector, Vector3.Negate(vertex0.Normal)));
                }
            }

            // TODO: Make this configurable
            tempStructure.ToggleCoordinateSystem();

            // Scale the text using given scale factor
            if (m_geometryOptions.VerticesScaleFactor > 0f)
            {
                Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(
                    m_geometryOptions.VerticesScaleFactor,
                    m_geometryOptions.VerticesScaleFactor,
                    m_geometryOptions.VerticesScaleFactor);

                Matrix4Stack transformMatrix = new Matrix4Stack(scaleMatrix);
                transformMatrix.TransformLocal(m_geometryOptions.VertexTransform);

                tempStructure.UpdateVerticesUsingRelocationFunc((actVector) => Vector3.Transform(actVector, transformMatrix.Top));
            }

            // Calculate all normals before adding to target structure
            if (m_geometryOptions.CalculateNormals)
            {
                tempStructure.CalculateNormalsFlat();
            }

            // Merge temporary structure to target structure
            m_targetSurface.AddStructure(tempStructure);

            return SDX.Result.Ok;
        }

        /// <summary>
        /// IDWriteTextLayout::Draw calls this application callback when it needs to draw an inline object.
        /// </summary>
        /// <param name="clientDrawingContext">The application-defined drawing context passed to IDWriteTextLayout::Draw.</param>
        /// <param name="originX">X-coordinate at the top-left corner of the inline object.</param>
        /// <param name="originY">Y-coordinate at the top-left corner of the inline object.</param>
        /// <param name="inlineObject">The application-defined inline object set using IDWriteTextFormat::SetInlineObject.</param>
        /// <param name="isSideways">A Boolean flag that indicates whether the object's baseline runs alongside the baseline axis of the line.</param>
        /// <param name="isRightToLeft">A Boolean flag that indicates whether the object is in a right-to-left context, hinting that the drawing may want to mirror the normal image.</param>
        /// <param name="clientDrawingEffect">Application-defined drawing effects for the glyphs to render. Usually this argument represents effects such as the foreground brush filling the interior of a line.</param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
        /// </returns>
        /// <unmanaged>HRESULT DrawInlineObject([None] void* clientDrawingContext,[None] FLOAT originX,[None] FLOAT originY,[None] IDWriteInlineObject* inlineObject,[None] BOOL isSideways,[None] BOOL isRightToLeft,[None] IUnknown* clientDrawingEffect)</unmanaged>
        public override SDX.Result DrawInlineObject(object clientDrawingContext, float originX, float originY, InlineObject inlineObject, bool isSideways, bool isRightToLeft, SDX.ComObject clientDrawingEffect)
        {
            return SDX.Result.Ok;
        }

        /// <summary>
        /// IDWriteTextLayout::Draw calls this function to instruct the client to draw a strikethrough.
        /// </summary>
        /// <param name="clientDrawingContext">The application-defined drawing context passed to  IDWriteTextLayout::Draw.</param>
        /// <param name="baselineOriginX">The pixel location (X-coordinate) at the baseline origin of the run where strikethrough applies.</param>
        /// <param name="baselineOriginY">The pixel location (Y-coordinate) at the baseline origin of the run where strikethrough applies.</param>
        /// <param name="strikethrough">Pointer to  a structure containing strikethrough logical information.</param>
        /// <param name="clientDrawingEffect">Application-defined effect to apply to the strikethrough.  Usually this argument represents effects such as the foreground brush filling the interior of a line.</param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
        /// </returns>
        /// <unmanaged>HRESULT DrawStrikethrough([None] void* clientDrawingContext,[None] FLOAT baselineOriginX,[None] FLOAT baselineOriginY,[In] const DWRITE_STRIKETHROUGH* strikethrough,[None] IUnknown* clientDrawingEffect)</unmanaged>
        /// <remarks>
        /// A single strikethrough can be broken into multiple calls, depending on how the formatting changes attributes. Strikethrough is not averaged across font sizes/styles changes. To get an appropriate starting pixel position, add strikethrough::offset to the baseline. Like underlines, the x coordinate will always be passed as the left side, regardless of text directionality.
        /// </remarks>
        public override SDX.Result DrawStrikethrough(object clientDrawingContext, float baselineOriginX, float baselineOriginY, ref Strikethrough strikethrough, SDX.ComObject clientDrawingEffect)
        {
            return SDX.Result.Ok;
        }

        /// <summary>
        /// IDWriteTextLayout::Draw calls this function to instruct the client to draw an underline.
        /// </summary>
        /// <param name="clientDrawingContext">The application-defined drawing context passed to  IDWriteTextLayout::Draw.</param>
        /// <param name="baselineOriginX">The pixel location (X-coordinate) at the baseline origin of the run where underline applies.</param>
        /// <param name="baselineOriginY">The pixel location (Y-coordinate) at the baseline origin of the run where underline applies.</param>
        /// <param name="underline">Pointer to  a structure containing underline logical information.</param>
        /// <param name="clientDrawingEffect">Application-defined effect to apply to the underline. Usually this argument represents effects such as the foreground brush filling the interior of a line.</param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
        /// </returns>
        /// <unmanaged>HRESULT DrawUnderline([None] void* clientDrawingContext,[None] FLOAT baselineOriginX,[None] FLOAT baselineOriginY,[In] const DWRITE_UNDERLINE* underline,[None] IUnknown* clientDrawingEffect)</unmanaged>
        /// <remarks>
        /// A single underline can be broken into multiple calls, depending on how the formatting changes attributes. If font sizes/styles change within an underline, the thickness and offset will be averaged weighted according to characters. To get an appropriate starting pixel position, add underline::offset to the baseline. Otherwise there will be no spacing between the text. The x coordinate will always be passed as the left side, regardless of text directionality. This simplifies drawing and reduces the problem of round-off that could potentially cause gaps or a double stamped alpha blend. To avoid alpha overlap, round the end points to the nearest device pixel.
        /// </remarks>
        public override SDX.Result DrawUnderline(object clientDrawingContext, float baselineOriginX, float baselineOriginY, ref Underline underline, SDX.ComObject clientDrawingEffect)
        {
            return SDX.Result.Ok;
        }
    }
}