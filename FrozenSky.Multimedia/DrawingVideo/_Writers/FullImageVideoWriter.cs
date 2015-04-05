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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrozenSky.Multimedia.Core;
using FrozenSky.Util;

// Some namespace mappings
using D3D11 = SharpDX.Direct3D11;
#if DESKTOP
using GDI = System.Drawing;
#endif

namespace FrozenSky.Multimedia.DrawingVideo
{
    /// <summary>
    /// A VideoWriter which writes full image files per frame
    /// </summary>
    public class FullImageVideoWriter : FrozenSkyVideoWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FullImageVideoWriter"/> class.
        /// </summary>
        public FullImageVideoWriter()
        {
            this.FileNameTemplate = "VideoImage_{0}.png";
        }

        /// <summary>
        /// Starts rendering to the target.
        /// </summary>
        /// <param name="videoPixelSize">The pixel size of the video.</param>
        protected override void StartRenderingInternal(Size2 videoPixelSize)
        {
            // Nothing to be done here
        }

        /// <summary>
        /// Draws the given frame to the video.
        /// </summary>
        /// <param name="device">The device on which the given framebuffer is created.</param>
        /// <param name="uploadedTexture">The texture which should be added to the video.</param>
        protected override void DrawFrameInternal(EngineDevice device, MemoryMappedTexture32bpp uploadedTexture)
        {
            // Generate the bitmap and save it
#if DESKTOP
            using(GDI.Bitmap actBitmap = GraphicsHelper.LoadBitmapFromMappedTexture(uploadedTexture))
            {
                actBitmap.Save(base.GetNextFileName());
            }
#else
            throw new NotImplementedException("Not implemented for WinRT currently!");
#endif
        }

        /// <summary>
        /// Finishes rendering to the target (closes the video file).
        /// Video rendering can not be started again from this point on.
        /// </summary>
        protected override void FinishRenderingInternal()
        {
            // Nothing to be done here
        }
    }
}
