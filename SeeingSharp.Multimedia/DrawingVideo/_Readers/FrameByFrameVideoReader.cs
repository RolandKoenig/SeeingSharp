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

using SeeingSharp.Multimedia.Core;
using SeeingSharp.Checking;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Namespace mappings
using MF = SharpDX.MediaFoundation;

namespace SeeingSharp.Multimedia.DrawingVideo
{
    public class FrameByFrameVideoReader : MediaFoundationVideoReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameByFrameVideoReader"/> class.
        /// </summary>
        /// <param name="videoSource">The source video file.</param>
        public FrameByFrameVideoReader(ResourceLink videoSource)
            : base(videoSource)
        {

        }

        /// <summary>
        /// Reads the next frame and puts it into a newly generated buffer.
        /// </summary>
        public MemoryMappedTexture32bpp ReadFrame()
        {
            this.EnsureNotNullOrDisposed("this");

            MemoryMappedTexture32bpp result = new MemoryMappedTexture32bpp(base.FrameSize);
            try
            {
                if (this.ReadFrame(result)) { return result; }
                else
                {
                    result.Dispose();
                    return null;
                }
            }
            catch (Exception)
            {
                result.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Reads the next frame and puts it into the provided buffer.
        /// </summary>
        /// <param name="targetBuffer">The target buffer to write to.</param>
        public bool ReadFrame(MemoryMappedTexture32bpp targetBuffer)
        {
            this.EnsureNotNullOrDisposed("this");
            targetBuffer.EnsureNotNull("targetBuffer");
            if ((targetBuffer.Width != base.FrameSize.Width) ||
               (targetBuffer.Height != base.FrameSize.Height))
            {
                throw new SeeingSharpGraphicsException("Size of the given buffer does not match the video size!");
            }

            // Read the frame
            SeeingSharpMediaBuffer mediaSharpManaged = base.ReadFrameInternal();
            if (mediaSharpManaged == null) { return false; }

            // Process the frame
            try
            {
                MF.MediaBuffer mediaBuffer = mediaSharpManaged.GetBuffer();

                int cbMaxLength;
                int cbCurrentLenght;
                IntPtr mediaBufferPointer = mediaBuffer.Lock(out cbMaxLength, out cbCurrentLenght);

#if DESKTOP
                // Performance optimization using MemCopy
                //  see http://code4k.blogspot.de/2010/10/high-performance-memcpy-gotchas-in-c.html
                NativeMethods.MemCopy(
                    targetBuffer.Pointer,
                    mediaBufferPointer,
                    new UIntPtr((uint)(base.FrameSize.Width * base.FrameSize.Height * 4)));
#else
                // TODO: Search a more performant way on WinRT platform (MemCopy not allowed there)
                unsafe
                {
                    Size2 frameSize = base.FrameSize;
                    int* mediaBufferPointerNative = (int*)mediaBufferPointer.ToPointer();
                    int* targetBufferPointerNative = (int*)targetBuffer.Pointer.ToPointer();
                    for (int loopY = 0; loopY < frameSize.Height; loopY++)
                    {
                        for (int loopX = 0; loopX < frameSize.Width; loopX++)
                        {
                            int actIndex = loopX + (loopY * frameSize.Width);
                            targetBufferPointerNative[actIndex] = mediaBufferPointerNative[actIndex];
                        }
                    }
                }
#endif

                return true;
            }
            finally
            {
                mediaSharpManaged.Dispose();
            }
        }
    }
}
