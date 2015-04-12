#region License information (FrozenSky and all based games/applications)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Some namespace mappings
using DXGI = SharpDX.DXGI;

namespace FrozenSky.Multimedia.Drawing2D
{
    /// <summary>
    /// Represents enum from Direct2D.
    /// </summary>
    [Flags]
    public enum DrawTextOptions : int
    {
        /// <summary>
        /// Text is vertically snapped to pixel boundaries and is not clipped to the layout rectangle.
        /// </summary>
        None = 0,

        /// <summary>
        /// Text is not vertically snapped to pixel boundaries. This setting is recommended
        /// for text that is being animated.
        /// </summary>
        NoSnap = 1,

        /// <summary>
        /// Text is clipped to the layout rectangle.
        /// </summary>
        Clip = 2,
    }

    /// <summary>
    /// Represents enum from Direct2D.
    /// </summary>
    public enum MeasuringMode : int
    {
        /// <summary>
        /// Specifies that text is measured using glyph ideal metrics whose values are
        /// independent to the current display resolution.
        /// </summary>
        Natural = 0,

        /// <summary>
        /// Specifies that text is measured using glyph display-compatible metrics whose
        /// values tuned for the current display resolution.
        /// </summary>
        GdiClassic = 1,

        /// <summary>
        /// Specifies that text is measured using the same glyph display metrics as
        /// text measured by GDI using a font created with CLEARTYPE_NATURAL_QUALITY.
        /// </summary>
        GdiNatural = 2,
    }

    /// <summary>
    /// Represents enum from DirectWrite.
    /// </summary>
    public enum FontStyle : int
    {
        Normal = 0,
        Oblique = 1,
        Italic = 2,
    }

    /// <summary>
    /// Represents enum from DirectWrite.
    /// </summary>
    public enum FontWeight : int
    {
        Thin = 100,
        ExtraLight = 200,
        UltraLight = 200,
        Light = 300,
        Normal = 400,
        Regular = 400,
        Medium = 500,
        DemiBold = 600,
        SemiBold = 600,
        Bold = 700,
        UltraBold = 800,
        ExtraBold = 800,
        Heavy = 900,
        Black = 900,
        ExtraBlack = 950,
        UltraBlack = 950,
    }

    /// <summary>
    /// Represents enum from DirectWrite.
    /// </summary>
    public enum FontStretch : int
    {
        Undefined = 0,
        UltraCondensed = 1,
        ExtraCondensed = 2,
        Condensed = 3,
        SemiCondensed = 4,
        Normal = 5,
        Medium = 5,
        SemiExpanded = 6,
        Expanded = 7,
        ExtraExpanded = 8,
        UltraExpanded = 9,
    }

    /// <summary>
    /// Represents enum from DirectWrite.
    /// </summary>
    public enum ParagraphAlignment : int
    {
        Near = 0,
        Far = 1,
        Center = 2,
    }

    /// <summary>
    /// Represents enum from DirectWrite.
    /// </summary>
    public enum TextAlignment : int
    {
        Leading = 0,
        Trailing = 1,
        Center = 2,
    }

    /// <summary>
    /// Represents enum from DirectWrite.
    /// </summary>
    public enum WordWrapping : int
    {
        Wrap = 0,
        NoWrap = 1,
    }

    /// <summary>
    /// Represents enum from DirectWrite.
    /// </summary>
    public enum ReadingDirection : int
    {
        LeftToRight = 0,
        RightToLeft = 1,
    }

    /// <summary>
    /// Represents enum from Direct2D.
    /// </summary>
    public enum AlphaMode
    {
        /// <summary>
        /// The alpha value might not be meaningful.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The alpha value has been premultiplied. Each color is first scaled by the
        /// alpha value. The alpha value itself is the same in both straight and premultiplied
        /// alpha. Typically, no color channel value is greater than the alpha channel
        /// value. If a color channel value in a premultiplied format is greater than
        /// the alpha channel, the standard source-over blending math results in an additive
        /// blend.
        /// </summary>
        Premultiplied = 1,

        /// <summary>
        /// The alpha value has not been premultiplied. The alpha channel indicates
        /// the transparency of the color.
        /// </summary>
        Straight = 2,

        /// <summary>
        /// The alpha value is ignored.
        /// </summary>
        Ignore = 3,
    }

    /// <summary>
    /// Represents enum from Direct2D.
    /// </summary>
    public enum BitmapInterpolationMode : int
    {
        NearestNeighbor = 0,
        Linear = 1,
    }

    /// <summary>
    /// Some bitmap format for Direct2D rendering.
    /// Maps directly to a subset of DXGI formats.
    /// </summary>
    public enum BitmapFormat : int
    {
        Bgra = DXGI.Format.B8G8R8A8_UNorm
    }
}
