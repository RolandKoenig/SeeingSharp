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

	********************************************************************************
    Additional permission under GNU GPL version 3 section 7

    If you modify this Program, or any covered work, by linking or combining it with 
	DirectX (or a modified version of that library), containing parts covered by the 
	terms of [name of library's license], the licensors of this Program grant you additional 
	permission to convey the resulting work. {Corresponding Source for a non-source form of 
	such a combination shall include the source code for the parts of DirectX used 
	as well as that of the covered work.}
*/
#endregion
using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Some namespace mappings
using D2D = SharpDX.Direct2D1;
using DWrite = SharpDX.DirectWrite;

namespace FrozenSky.Multimedia.Drawing2D
{
    public class Graphics2D
    {
        private EngineDevice m_device;
        private D2D.RenderTarget m_renderTarget;
        private Size2F m_screenSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="Graphics2D"/> class.
        /// </summary>
        /// <param name="device">The hardware device which is used for rendering.</param>
        /// <param name="renderTarget">The render target which is used by this graphics object.</param>
        /// <param name="screenSize">The size of the screen in device independent pixels.</param>
        internal Graphics2D(EngineDevice device, D2D.RenderTarget renderTarget, Size2F screenSize)
        {
            m_device = device;
            m_renderTarget = renderTarget;
            m_screenSize = screenSize;
        }

        /// <summary>
        /// Clears the current view.
        /// </summary>
        /// <param name="clearColor">Color of the clear.</param>
        public void Clear(Color4 clearColor)
        {
            m_renderTarget.Clear(clearColor.ToDXColor());
        }

        /// <summary>
        /// Fills the given rectangle with the given brush object.
        /// </summary>
        /// <param name="rectangle">The rectangle to be filled.</param>
        /// <param name="brush">The brush to be used.</param>
        public void FillRectangle(RectangleF rectangle, BrushResource brush)
        {
            m_renderTarget.FillRectangle(
                rectangle.ToDXRectangle(),
                brush.GetBrush(m_device));
        }

        /// <summary>
        /// Fills the given rectangle with the given brush object.
        /// </summary>
        /// <param name="radiusX">The x radius of the rectangle's corners.</param>
        /// <param name="radiusY">The y radius of the rectangle's corners.</param>
        /// <param name="rectangle">The rectangle to be filled.</param>
        /// <param name="brush">The brush to be used.</param>
        public void FillRoundedRectangle(RectangleF rectangle, float radiusX, float radiusY, BrushResource brush)
        {
            D2D.RoundedRectangle roundedRect = new D2D.RoundedRectangle();
            roundedRect.Rect = rectangle.ToDXRectangle();
            roundedRect.RadiusX = radiusX;
            roundedRect.RadiusY = radiusY;

            m_renderTarget.FillRoundedRectangle(
                roundedRect,
                brush.GetBrush(m_device));
        }

        /// <summary>
        /// Draws the given text on the screen.
        /// </summary>
        /// <param name="textToDraw">The text to draw.</param>
        /// <param name="textFormat">The TextFormat to be used.</param>
        /// <param name="targetRectangle">The target rectangle.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="drawOptions">Some draw options to be passed to Direct2D.</param>
        /// <param name="measuringMode">Sets the measuring mode to be passed to Direct2D.</param>
        public void DrawText(
            string textToDraw, TextFormatResource textFormat, RectangleF targetRectangle, BrushResource brush,
            DrawTextOptions drawOptions = DrawTextOptions.None,
            MeasuringMode measuringMode = MeasuringMode.Natural)
        {
            D2D.DrawTextOptions drawOptionsD2D = (D2D.DrawTextOptions)drawOptions;
            D2D.MeasuringMode measuringModeD2D = (D2D.MeasuringMode)measuringMode;

            m_renderTarget.DrawText(
                textToDraw,
                textFormat.GetTextFormat(m_device),
                targetRectangle.ToDXRectangle(),
                brush.GetBrush(m_device),
                drawOptionsD2D);
        }

        /// <summary>
        /// Draws the given bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="opacity">The opacity.</param>
        /// <param name="interpolationMode">The interpolation mode.</param>
        public void DrawBitmap(
            BitmapResource bitmap, 
            float opacity = 1f, 
            BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.NearestNeighbor)
        {
            m_renderTarget.DrawBitmap(
                bitmap.GetBitmap(m_device),
                opacity, (D2D.BitmapInterpolationMode)interpolationMode);
        }

        /// <summary>
        /// Draws the given bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="destinationRectangle">The target rectangle where to draw the bitmap.</param>
        /// <param name="opacity">The opacity.</param>
        /// <param name="interpolationMode">The interpolation mode.</param>
        public void DrawBitmap(
            BitmapResource bitmap, 
            RectangleF destinationRectangle,
            float opacity = 1f, 
            BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.NearestNeighbor)
        {
            m_renderTarget.DrawBitmap(
                bitmap.GetBitmap(m_device),
                destinationRectangle.ToDXRectangle(),
                opacity, (D2D.BitmapInterpolationMode)interpolationMode);
        }

        /// <summary>
        /// Gets the device which is used for rendering.
        /// </summary>
        public EngineDevice Device
        {
            get { return m_device; }
        }

        /// <summary>
        /// Gets the total size of the screen (already scaled by DPI).
        /// </summary>
        public Size2F ScreenSize
        {
            get { return m_screenSize; }
        }
    }
}
