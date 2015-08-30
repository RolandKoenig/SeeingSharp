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
#endregion License information (SeeingSharp and all based games/applications)

using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Checking;
using SeeingSharp.Multimedia;
using SeeingSharp.Multimedia.Core;

// Some namespace mappings
using D2D = SharpDX.Direct2D1;
using DWrite = SharpDX.DirectWrite;

namespace SeeingSharp.Multimedia.Drawing2D
{
    public class Graphics2D
    {
        #region Main view related properties
        private EngineDevice m_device;
        private D2D.RenderTarget m_renderTarget;
        private Size2F m_screenPixelSize;
#if UNIVERSAL
        private D2D.DeviceContext1 m_deviceContext;
#endif
        #endregion Main view related properties

        #region Transform settings
        private Graphics2DTransformSettings m_transformSettings;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Graphics2D"/> class.
        /// </summary>
        /// <param name="device">The hardware device which is used for rendering.</param>
        /// <param name="renderTarget">The render target which is used by this graphics object.</param>
        /// <param name="screenSize">The size of the screen in device independent pixels.</param>
        internal Graphics2D(EngineDevice device, D2D.RenderTarget renderTarget, Size2F screenSize)
        {
            m_transformSettings = Graphics2DTransformSettings.Default;

            m_device = device;
            m_renderTarget = renderTarget;
            m_screenPixelSize = screenSize;

#if UNIVERSAL
            m_deviceContext = m_renderTarget as D2D.DeviceContext1;
#endif
        }

        /// <summary>
        /// Sets current transform settings on this graphics object.
        /// (be carefull, the state is changed on device level!)
        /// </summary>
        /// <param name="transformSettings">The settings to be set.</param>
        internal void SetTransformSettings(Graphics2DTransformSettings transformSettings)
        {
            m_transformSettings = transformSettings;

            switch(transformSettings.TransformMode)
            {
                    // Overtake given scaling matrix
                case Graphics2DTransformMode.Custom:
                    m_renderTarget.Transform = transformSettings.CustomTransform.ToDXMatrix();
                    break;

                    // Calculate scaling matrix here 
                case Graphics2DTransformMode.AutoScaleToVirtualScreen:
                    float virtualWidth = m_transformSettings.VirtualScreenSize.Width;
                    float virtualHeight = m_transformSettings.VirtualScreenSize.Height;
                    if(virtualWidth == 0f) { virtualWidth = m_screenPixelSize.Width; }
                    if(virtualHeight == 0f) { virtualHeight = m_screenPixelSize.Height; }

                    float scaleFactorX = m_screenPixelSize.Width / virtualWidth;
                    float scaleFactorY = m_screenPixelSize.Height / virtualHeight;
                    float combinedScaleFactor = Math.Min(scaleFactorX, scaleFactorY);
                    float truePixelWidth = virtualWidth * combinedScaleFactor;
                    float truePixelHeight = virtualHeight * combinedScaleFactor;

                    m_renderTarget.Transform =
                        SharpDX.Matrix3x2.Scaling(combinedScaleFactor) *
                        SharpDX.Matrix3x2.Translation(
                            m_screenPixelSize.Width / 2f - truePixelWidth / 2f,
                            m_screenPixelSize.Height / 2f - truePixelHeight / 2f);
                    break;
            }
        }

        /// <summary>
        /// Resets the transform setting son this graphics object.
        /// (be carefull, the state is changed on device level!)
        /// </summary>
        /// <param name="transformSettings">The settings to be set.</param>
        internal void ResetTransformSettings()
        {
            this.SetTransformSettings(Graphics2DTransformSettings.Default);
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
        /// Draws the given rectangle with the given brush.
        /// </summary>
        public void DrawRectangle(RectangleF rectangle, BrushResource brush, float strokeWidth = 1f)
        {
            brush.EnsureNotNull("brush");
            rectangle.EnsureNotEmpty("rectangle");
            strokeWidth.EnsurePositive("strokeWidth");

            m_renderTarget.DrawRectangle(
                rectangle.ToDXRectangle(),
                brush.GetBrush(m_device),
                strokeWidth);
        }

        /// <summary>
        /// Draws the given rounded rectangle with the given brush.
        /// </summary>
        public void DrawRoundedRectangle(RectangleF rectangle, float radiusX, float radiusY, BrushResource brush, float strokeWidth = 1f)
        {
            rectangle.EnsureNotEmpty("rectangle");
            brush.EnsureNotNull("brush");
            radiusX.EnsurePositive("radiusX");
            radiusY.EnsurePositive("radiusY");
            strokeWidth.EnsurePositive("strokeWidth");

            D2D.RoundedRectangle roundedRect = new D2D.RoundedRectangle();
            roundedRect.Rect = rectangle.ToDXRectangle();
            roundedRect.RadiusX = radiusX;
            roundedRect.RadiusY = radiusY;

            m_renderTarget.DrawRoundedRectangle(
                roundedRect,
                brush.GetBrush(m_device),
                strokeWidth);
        }

        /// <summary>
        /// Draws a point at the given location.
        /// </summary>
        public void DrawPoint(Vector2 point, BrushResource brush)
        {
            this.DrawLine(
                point, new Vector2(point.X + 1f, point.Y),
                brush);
        }

        /// <summary>
        /// Draws the given line with the given brush.
        /// </summary>
        public void DrawLine(Vector2 start, Vector2 end, BrushResource brush, float strokeWidth = 1f)
        {
            brush.EnsureNotNull("brush");
            strokeWidth.EnsurePositiveAndNotZero("strokeWidth");

            m_renderTarget.DrawLine(
                start.ToDXVector(), end.ToDXVector(),
                brush.GetBrush(m_device),
                strokeWidth);
        }

        /// <summary>
        /// Fills the given rectangle with the given brush object.
        /// </summary>
        /// <param name="rectangle">The rectangle to be filled.</param>
        /// <param name="brush">The brush to be used.</param>
        public void FillRectangle(RectangleF rectangle, BrushResource brush)
        {
            rectangle.EnsureNotEmpty("rectangle");
            brush.EnsureNotNull("brush");

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
            rectangle.EnsureNotEmpty("rectangle");
            brush.EnsureNotNull("brush");
            radiusX.EnsurePositive("radiusX");
            radiusY.EnsurePositive("radiusY");

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
            textToDraw.EnsureNotNull("textToDraw");
            targetRectangle.EnsureNotEmpty("targetRectangle");
            brush.EnsureNotNull("brush");

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
            bitmap.EnsureNotNull("bitmap");
            opacity.EnsureInRange(0f, 1f, "opacity");

            m_renderTarget.DrawBitmap(
                bitmap.GetBitmap(m_device),
                opacity,
                (D2D.BitmapInterpolationMode)interpolationMode);
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
            bitmap.EnsureNotNull("bitmap");
            destinationRectangle.EnsureNotEmpty("destinationRectangle");
            opacity.EnsureInRange(0f, 1f, "opacity");

            m_renderTarget.DrawBitmap(
                bitmap.GetBitmap(m_device),
                destinationRectangle.ToDXRectangle(),
                opacity, 
                (D2D.BitmapInterpolationMode)interpolationMode);
        }

        /// <summary>
        /// Draws the given bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="destinationRectangle">The target rectangle where to draw the bitmap.</param>
        /// <param name="sourceRectangle">The area which to take from the bitmap.</param>
        /// <param name="opacity">The opacity.</param>
        /// <param name="interpolationMode">The interpolation mode.</param>
        public void DrawBitmap(
            BitmapResource bitmap,
            RectangleF destinationRectangle,
            RectangleF sourceRectangle,
            float opacity = 1f,
            BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.NearestNeighbor)
        {
            bitmap.EnsureNotNull("bitmap");
            destinationRectangle.EnsureNotEmpty("destinationRectangle");
            opacity.EnsureInRange(0f, 1f, "opacity");

            m_renderTarget.DrawBitmap(
                bitmap.GetBitmap(m_device),
                destinationRectangle.ToDXRectangle(),
                opacity,
                (D2D.BitmapInterpolationMode)interpolationMode,
                sourceRectangle.ToDXRectangle());
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
            Vector2 destinationOrigin,
            float opacity = 1f,
            BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.NearestNeighbor)
        {
            bitmap.EnsureNotNull("bitmap");
            opacity.EnsureInRange(0f, 1f, "opacity");

            m_renderTarget.DrawBitmap(
                bitmap.GetBitmap(m_device),
                new SharpDX.RectangleF(
                    destinationOrigin.X, destinationOrigin.Y,
                    bitmap.PixelWidth, bitmap.PixelHeight),
                opacity, 
                (D2D.BitmapInterpolationMode)interpolationMode);
        }

        /// <summary>
        /// Draws the given bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="destinationRectangle">The target rectangle where to draw the bitmap.</param>
        /// <param name="opacity">The opacity.</param>
        /// <param name="sourceRectangle">The area which to take from the bitmap.</param>
        /// <param name="interpolationMode">The interpolation mode.</param>
        public void DrawBitmap(
            BitmapResource bitmap,
            Vector2 destinationOrigin,
            RectangleF sourceRectangle,
            float opacity = 1f,
            BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.NearestNeighbor)
        {
            bitmap.EnsureNotNull("bitmap");
            opacity.EnsureInRange(0f, 1f, "opacity");

            m_renderTarget.DrawBitmap(
                bitmap.GetBitmap(m_device),
                new SharpDX.RectangleF(
                    destinationOrigin.X, destinationOrigin.Y,
                    bitmap.PixelWidth, bitmap.PixelHeight),
                opacity,
                (D2D.BitmapInterpolationMode)interpolationMode,
                sourceRectangle.ToDXRectangle());
        }

#if UNIVERSAL
        /// <summary>
        /// Draws the given image.
        /// </summary>
        /// <param name="image">The source of pixel data to be rendered.</param>
        /// <param name="destinationRectangle">The target rectangle where to draw the image.</param>
        public void DrawImage(
            IEffectInput image,
            Vector2 destinationOrigin)
        {
            image.EnsureNotNull("bitmap");

            IEffectInputInternal internalImage = image as IEffectInputInternal;
            internalImage.EnsureNotNull("internalImage");

            D2D.Image d2dImage = internalImage.GetInputObject(m_device) as D2D.Image;
            d2dImage.EnsureNotNull("d2dImage");

            m_deviceContext.DrawImage(
                d2dImage,
                destinationOrigin.ToDXVector(),
                null,
                D2D.InterpolationMode.Linear,
                D2D.CompositeMode.SourceOver);
        }

        /// <summary>
        /// Draws the given image.
        /// </summary>
        /// <param name="image">The source of pixel data to be rendered.</param>
        /// <param name="destinationRectangle">The target rectangle where to draw the image.</param>
        /// <param name="sourceRectangle">The area which to take from the bitmap.</param>
        public void DrawImage(
            IEffectInput image,
            Vector2 destinationOrigin,
            RectangleF sourceRectangle)
        {
            image.EnsureNotNull("bitmap");

            IEffectInputInternal internalImage = image as IEffectInputInternal;
            internalImage.EnsureNotNull("internalImage");

            D2D.Image d2dImage = internalImage.GetInputObject(m_device) as D2D.Image;
            d2dImage.EnsureNotNull("d2dImage");
     
            m_deviceContext.DrawImage(
                d2dImage,
                destinationOrigin.ToDXVector(),
                sourceRectangle.ToDXRectangle(),
                D2D.InterpolationMode.Linear,
                D2D.CompositeMode.SourceOver);
        }
#endif

        /// <summary>
        /// Gets the device which is used for rendering.
        /// </summary>
        public EngineDevice Device
        {
            get { return m_device; }
        }

        /// <summary>
        /// Gets the bounds of the screen.
        /// </summary>
        public RectangleF ScreenBounds
        {
            get
            {
                Size2F screenSize = this.ScreenSize;
                return new RectangleF(
                    0f, 0f,
                    screenSize.Width, screenSize.Height);
            }
        }

        /// <summary>
        /// Gets the total size of pixels (already scaled by DPI).
        /// </summary>
        public Size2F ScreenPixelSize
        {
            get { return m_screenPixelSize; }
        }

        /// <summary>
        /// Gets the total size of this screen.
        /// This value may be a virtual screen size (see TransformMode).
        /// </summary>
        public Size2F ScreenSize
        {
            get
            {
                switch (m_transformSettings.TransformMode)
                {
                    case Graphics2DTransformMode.AutoScaleToVirtualScreen:
                        return m_transformSettings.VirtualScreenSize;

                    default:
                        return m_screenPixelSize;
                }

            }
        }

        /// <summary>
        /// Gets the width of the screen.
        /// This value may be a virtual screen size (see TransformMode).
        /// </summary>
        public float ScreenWidth
        {
            get
            {
                switch(m_transformSettings.TransformMode)
                {
                    case Graphics2DTransformMode.AutoScaleToVirtualScreen:
                        return m_transformSettings.VirtualScreenSize.Width;

                    default:
                        return m_screenPixelSize.Width;
                }
                
            }
        }

        /// <summary>
        /// Gets the height of the screen.
        /// This value may be a virtual screen size (see TransformMode).
        /// </summary>
        public float ScreenHeight
        {
            get
            {
                switch (m_transformSettings.TransformMode)
                {
                    case Graphics2DTransformMode.AutoScaleToVirtualScreen:
                        return m_transformSettings.VirtualScreenSize.Height;

                    default:
                        return m_screenPixelSize.Height;
                }

            }
        }
    }
}