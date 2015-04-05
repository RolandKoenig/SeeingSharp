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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if DESKTOP
using System.Windows.Media.Imaging;
#endif

// Some namespace mappings
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;

#if DESKTOP
using GDI = System.Drawing;
#endif

namespace FrozenSky.Multimedia.Core
{
    public class TextureUploader : IDisposable
    {
        // Given parameters
        private EngineDevice m_device;
        private D3D11.Texture2D m_texture;
        private int m_width;
        private int m_height;
        private DXGI.Format m_format;
        private bool m_isMultisampled;

        //Direct3D resources for rendertarget capturing
        // A staging texture for reading contents by Cpu
        // A standard texture for copying data from multisample texture to standard one
        // see http://www.rolandk.de/wp/2013/06/inhalt-der-rendertarget-textur-in-ein-bitmap-kopieren/
        private D3D11.Texture2D m_copyHelperTextureStaging;
        private D3D11.Texture2D m_copyHelperTextureStandard;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureUploader"/> class.
        /// </summary>
        /// <param name="device">The device on which the texture was created.</param>
        /// <param name="texture">The texture which is to be uploaded to system memory.</param>
        internal TextureUploader(EngineDevice device, D3D11.Texture2D texture)
        {
            var textureDesc = texture.Description;

            m_device = device;
            m_texture = texture;
            m_width = textureDesc.Width;
            m_height = textureDesc.Height;
            m_format = textureDesc.Format;
            m_isMultisampled = (textureDesc.SampleDescription.Count > 1) || (textureDesc.SampleDescription.Quality > 0);
        }

#if DESKTOP
        /// <summary>
        /// Takes a screenshot and returns it as a gdi bitmap.
        /// </summary>
        public GDI.Bitmap UploadToGdiBitmap()
        {
            // Check current format
            if ((m_format != GraphicsHelper.DEFAULT_TEXTURE_FORMAT) &&
               (m_format != GraphicsHelper.DEFAULT_TEXTURE_FORMAT_SHARING) &&
               (m_format != GraphicsHelper.DEFAULT_TEXTURE_FORMAT_SHARING_D2D))
            {
                throw new FrozenSkyGraphicsException("Invalid format for texture uploading to gdi bitmap (" + m_format + ")!");
            }

            // Upload the texture
            CopyTextureToStagingResource();

            // Load the bitmap
            GDI.Bitmap resultBitmap = GraphicsHelper.LoadBitmapFromStagingTexture(
                m_device,
                m_copyHelperTextureStaging, m_width, m_height);

            return resultBitmap;
        }
#endif

        /// <summary>
        /// Takes a color texture and uploads it to the given buffer.
        /// </summary>
        /// <param name="intBuffer">The target int buffer to which to copy all pixel data.</param>
        public unsafe void UploadToIntBuffer(MemoryMappedTexture32bpp intBuffer)
        {
            // Check current format
            if ((m_format != GraphicsHelper.DEFAULT_TEXTURE_FORMAT) &&
                (m_format != GraphicsHelper.DEFAULT_TEXTURE_FORMAT_SHARING) &&
                (m_format != GraphicsHelper.DEFAULT_TEXTURE_FORMAT_SHARING_D2D))
            {
                throw new FrozenSkyGraphicsException(string.Format("Invalid format for texture uploading to a color map ({0})!", m_format));
            }

            // Upload the texture
            CopyTextureToStagingResource();

            // Read the data into the .Net data block
            SharpDX.DataBox dataBox = m_device.DeviceImmediateContextD3D11.MapSubresource(
                m_copyHelperTextureStaging, 0, D3D11.MapMode.Read, D3D11.MapFlags.None);
            try
            {
                int rowPitchSource = dataBox.RowPitch;
                int rowPitchDestination = intBuffer.Width * 4;

#if !DESKTOP
                float* sourcePointer = (float*)dataBox.DataPointer.ToPointer();
                float* destPointer = (float*)intBuffer.Pointer.ToPointer();
#endif
                if ((rowPitchSource > 0) && (rowPitchSource < 20000) &&
                    (rowPitchDestination > 0) && (rowPitchDestination < 20000))
                {
                    for (int loopY = 0; loopY < m_height; loopY++)
                    {
#if DESKTOP
                        NativeMethods.MemCopy(
                            intBuffer.Pointer + loopY * rowPitchDestination,
                            dataBox.DataPointer + loopY * rowPitchSource,
                            new UIntPtr((uint)rowPitchDestination));
#else
                        float* memLocSource = sourcePointer + loopY * rowPitchSource;
                        float* memLocDest = destPointer + loopY * rowPitchDestination;
                        for(int loopX=0; loopX < m_width; loopX++)
                        {
                            memLocDest[loopX] = memLocSource[loopX];
                        }
#endif
                    }
                }
            }
            finally
            {
                m_device.DeviceImmediateContextD3D11.UnmapSubresource(m_copyHelperTextureStaging, 0);
            }
        }

        /// <summary>
        /// Upload a floatingpoint texture from the graphics hardware.
        /// This method is only valid for resources of type R32_Floatfloat.
        /// </summary>
        public unsafe MemoryMappedTextureFloat UploadToFloatBuffer()
        {
            MemoryMappedTextureFloat result = new MemoryMappedTextureFloat(
                new Size2(m_width, m_height));
            UploadToFloatBuffer(result);
            return result;
        }

        /// <summary>
        /// Upload a floatingpoint texture from the graphics hardware.
        /// This method is only valid for resources of type R32_Floatfloat.
        /// </summary>
        /// <param name="floatBuffer">The target float buffer to which to copy all ObjectIDs.</param>
        public unsafe void UploadToFloatBuffer(MemoryMappedTextureFloat floatBuffer)
        {
            // Check current format
            if (m_format != GraphicsHelper.DEFAULT_TEXTURE_FORMAT_OBJECT_ID)
            {
                throw new FrozenSkyGraphicsException("Invalid format for texture uploading to gdi bitmap (" + m_format + ")!");
            }
            if (floatBuffer.Width != m_width) { throw new FrozenSkyGraphicsException("The width of the textures during texture upload does not match!"); }
            if (floatBuffer.Height != m_height) { throw new FrozenSkyGraphicsException("The height of the textures during texture upload does not match!"); }

            // Upload the texture
            CopyTextureToStagingResource();

            // Read the data into the .Net data block
            SharpDX.DataBox dataBox = m_device.DeviceImmediateContextD3D11.MapSubresource(
                m_copyHelperTextureStaging, 0, D3D11.MapMode.Read, D3D11.MapFlags.None);
            try
            {

                int rowPitchSource = dataBox.RowPitch;
                int rowPitchDestination = floatBuffer.Width * 4;
#if !DESKTOP
                float* sourcePointer = (float*)dataBox.DataPointer.ToPointer();
                float* destPointer = (float*)floatBuffer.Pointer.ToPointer();
#endif
                if ((rowPitchSource > 0) && (rowPitchSource < 20000) &&
                    (rowPitchDestination > 0) && (rowPitchDestination < 20000))
                {
                    for (int loopY = 0; loopY < m_height; loopY++)
                    {
#if DESKTOP
                        NativeMethods.MemCopy(
                            floatBuffer.Pointer + loopY * rowPitchDestination,
                            dataBox.DataPointer + loopY * rowPitchSource,
                            new UIntPtr((uint)rowPitchDestination));
#else
                        float* memLocSource = sourcePointer + loopY * rowPitchSource;
                        float* memLocDest = destPointer + loopY * rowPitchDestination;
                        for(int loopX=0; loopX < m_width; loopX++)
                        {
                            memLocDest[loopX] = memLocSource[loopX];
                        }
#endif
                    }
                }
            }
            finally
            {
                m_device.DeviceImmediateContextD3D11.UnmapSubresource(m_copyHelperTextureStaging, 0);
            }
        }

#if DESKTOP
        /// <summary>
        /// Takes a NormalDepth texture and creates a normal- and a depth-bitmap
        /// </summary>
        /// <param name="normal">The normal-bitmap</param>
        /// <param name="depth">The depth-bitmap</param>
        public unsafe void UploadNormalDepthBitmapsWpf(out WriteableBitmap normal, out WriteableBitmap depth)
        {

            //Check current format
            if (m_format != GraphicsHelper.DEFAULT_TEXTURE_FORMAT_NORMAL_DEPTH)
            {
                throw new FrozenSkyGraphicsException("Invalid format for getting NormalDepth-Values (" + m_format + ")!");
            }

            //Upload the texture
            CopyTextureToStagingResource(true);

            //Read the data into the .Net data block
            SharpDX.DataBox dataBox = m_device.DeviceImmediateContextD3D11.MapSubresource(
                m_copyHelperTextureStaging, 0, D3D11.MapMode.Read, D3D11.MapFlags.None);

            //Create the output bitmaps
            normal = new WriteableBitmap(m_width, m_height, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null);
            depth = new WriteableBitmap(m_width, m_height, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null);

            try
            {
                //Read the values from Texture into bitmapsa
                SharpDX.Half* pointr = (SharpDX.Half*)dataBox.DataPointer.ToPointer();
                int rowPitchSource = dataBox.RowPitch / 2;

                int rowSource = normal.BackBufferStride / 4;

                normal.Lock();
                depth.Lock();

                int* normalBitmapBuffer = (int*)normal.BackBuffer.ToPointer();
                int* depthBitmapBuffer = (int*)depth.BackBuffer.ToPointer();

                for (int loopY = 0; loopY < m_height; loopY++)
                {
                    for (int loopX = 0; loopX < m_width; loopX++)
                    {
                        //Calculate the pointer
                        int position = (loopY * rowPitchSource) + (loopX * 4);

                        //Read values from texture
                        SharpDX.Half valueR = pointr[position];
                        SharpDX.Half valueG = pointr[position + 1];
                        SharpDX.Half valueB = pointr[position + 2];
                        SharpDX.Half valueDepth = pointr[position + 3];

                        //Calculate the Values for the Colors
                        int colorR = (int)Math.Abs(valueR * 255);
                        int colorG = (int)Math.Abs(valueG * 255);
                        int colorB = (int)Math.Abs(valueB * 255);
                        int colorDepthVal = (int)Math.Abs(valueDepth);
                        colorDepthVal = 255 - (colorDepthVal > 255 ? 255 : colorDepthVal < 0 ? 0 : colorDepthVal);
                        colorR = colorR > 255 ? 255 : colorR < 0 ? 0 : colorR;
                        colorG = colorG > 255 ? 255 : colorG < 0 ? 0 : colorG;
                        colorB = colorB > 255 ? 255 : colorB < 0 ? 0 : colorB;

                        //Create Colors
                        Color colorNormal = Color.FromArgb(255, colorR, colorG, colorB);
                        Color colorDepth = Color.FromArgb(255, colorDepthVal, colorDepthVal, colorDepthVal);

                        int normalColorData = colorNormal.A << 24;
                        normalColorData |= colorNormal.R << 16;
                        normalColorData |= colorNormal.G << 8;
                        normalColorData |= colorNormal.B << 0;

                        int depthColorData = colorDepth.A << 24;
                        depthColorData |= colorDepth.R << 16;
                        depthColorData |= colorDepth.G << 8;
                        depthColorData |= colorDepth.B << 0;

                        normalBitmapBuffer[loopY * rowSource + (loopX)] = normalColorData;
                        depthBitmapBuffer[loopY * rowSource + loopX] = depthColorData;
                    }
                }
                normal.AddDirtyRect(new System.Windows.Int32Rect(0, 0, m_width, m_height));
                depth.AddDirtyRect(new System.Windows.Int32Rect(0, 0, m_width, m_height));
                normal.Unlock();
                normal.Freeze();
                depth.Unlock();
                depth.Freeze();
            }
            finally
            {
                m_device.DeviceImmediateContextD3D11.UnmapSubresource(m_copyHelperTextureStaging, 0);
            }
        }
#endif

        /// <summary>
        /// Loads the target texture int a staging texture.
        /// </summary>
        private void CopyTextureToStagingResource(bool handleMultiSampling = true)
        {
            // Prepare needed textures
            if (m_copyHelperTextureStaging == null)
            {
                m_copyHelperTextureStaging = GraphicsHelper.CreateStagingTexture(m_device, m_width, m_height, m_format);
                if (m_isMultisampled && handleMultiSampling)
                {
                    m_copyHelperTextureStandard = GraphicsHelper.CreateTexture(m_device, m_width, m_height, m_format);
                }
            }

            // Copy contents of the texture
            //  .. execute a ResolveSubresource before if the source texture is multisampled
            if (m_isMultisampled && handleMultiSampling)
            {
                m_device.DeviceImmediateContextD3D11.ResolveSubresource(m_texture, 0, m_copyHelperTextureStandard, 0, m_format);
                m_device.DeviceImmediateContextD3D11.CopyResource(m_copyHelperTextureStandard, m_copyHelperTextureStaging);
            }
            else
            {
                m_device.DeviceImmediateContextD3D11.CopyResource(m_texture, m_copyHelperTextureStaging);
            }
        }

        /// <summary>
        /// Führt anwendungsspezifische Aufgaben aus, die mit dem Freigeben, Zurückgeben oder Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
        /// </summary>
        public void Dispose()
        {
            GraphicsHelper.SafeDispose(ref m_copyHelperTextureStaging);
            GraphicsHelper.SafeDispose(ref m_copyHelperTextureStandard);
        }
    }
}
