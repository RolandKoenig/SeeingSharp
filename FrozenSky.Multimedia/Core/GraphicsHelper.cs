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

using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;
using System.Text;
using FrozenSky;
using FrozenSky.Multimedia.Drawing3D;
using FrozenSky.Util;

//Some namespace mappings
using Buffer = SharpDX.Direct3D11.Buffer;
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;
using TKGFX = SharpDX.Toolkit.Graphics;
#if !WINDOWS_PHONE
using D2D = SharpDX.Direct2D1;
using WIC = SharpDX.WIC;
#endif
#if DESKTOP
using D3D10 = SharpDX.Direct3D10;
using GDI = System.Drawing;
using WinForms = System.Windows.Forms;
#endif

namespace FrozenSky.Multimedia.Core
{
    public static class GraphicsHelper
    {
        // All default texture formats
        internal const DXGI.Format DEFAULT_TEXTURE_FORMAT = DXGI.Format.B8G8R8A8_UNorm;
        internal const DXGI.Format DEFAULT_TEXTURE_FORMAT_SHARING = DXGI.Format.B8G8R8A8_UNorm; 
        internal const DXGI.Format DEFAULT_TEXTURE_FORMAT_SHARING_D2D = DXGI.Format.B8G8R8A8_UNorm;
        internal const DXGI.Format DEFAULT_TEXTURE_FORMAT_NORMAL_DEPTH = DXGI.Format.R16G16B16A16_Float;
        internal const DXGI.Format DEFAULT_TEXTURE_FORMAT_OBJECT_ID = DXGI.Format.R32_Float;
        internal const DXGI.Format DEFAULT_TEXTURE_FORMAT_DEPTH = DXGI.Format.D32_Float_S8X24_UInt;
        internal static readonly Guid DEFAULT_WIC_BITMAP_FORMAT = WIC.PixelFormat.Format32bppBGRA; 

#if DESKTOP
        /// <summary>
        /// Creates a default SwapChain for the given target control.
        /// </summary>
        /// <param name="targetControl">Target control of the swap chain.</param>
        /// <param name="factory">Factory for SwapChain creation.</param>
        /// <param name="device">Graphics device.</param>
        internal static DXGI.SwapChain CreateDefaultSwapChain(WinForms.Control targetControl, EngineDevice device, GraphicsViewConfiguration gfxConfig)
        {
            // Create the swap chain description
            DXGI.SwapChainDescription swapChainDesc = new DXGI.SwapChainDescription();
            if (gfxConfig.AntialiasingEnabled && device.IsStandardAntialiasingPossible)
            {
                swapChainDesc.OutputHandle = targetControl.Handle;
                swapChainDesc.IsWindowed = true;
                swapChainDesc.BufferCount = 1;
                swapChainDesc.Flags = DXGI.SwapChainFlags.AllowModeSwitch;
                swapChainDesc.ModeDescription = new DXGI.ModeDescription(
                    targetControl.Width,
                    targetControl.Height,
                    new DXGI.Rational(60, 1),
                    DEFAULT_TEXTURE_FORMAT);
                swapChainDesc.SampleDescription = device.GetSampleDescription(gfxConfig.AntialiasingQuality);
                swapChainDesc.SwapEffect = DXGI.SwapEffect.Discard;
                swapChainDesc.Usage = DXGI.Usage.RenderTargetOutput;
            }
            else
            {
                swapChainDesc.OutputHandle = targetControl.Handle;
                swapChainDesc.IsWindowed = true;
                swapChainDesc.BufferCount = 1;
                swapChainDesc.Flags = DXGI.SwapChainFlags.AllowModeSwitch;
                swapChainDesc.ModeDescription = new DXGI.ModeDescription(
                    targetControl.Width,
                    targetControl.Height,
                    new DXGI.Rational(60, 1),
                    DEFAULT_TEXTURE_FORMAT);
                swapChainDesc.SampleDescription = new DXGI.SampleDescription(1, 0);
                swapChainDesc.SwapEffect = DXGI.SwapEffect.Discard;
                swapChainDesc.Usage = DXGI.Usage.RenderTargetOutput;
            }

            // Create and return the swap chain and the render target
            return new DXGI.SwapChain(device.FactoryDxgi, device.DeviceD3D11, swapChainDesc);
        }

        /// <summary>
        /// Converts a System.Drawing.Bitmap to a DirectX 11 texture object.
        /// </summary>
        /// <param name="device">Device on wich the resource should be created.</param>
        /// <param name="bitmap">The source bitmap.</param>
        internal static D3D11.Texture2D LoadTextureFromBitmap(EngineDevice device, GDI.Bitmap bitmap)
        {
            return LoadTextureFromBitmap(device, bitmap, 1);
        }

        /// <summary>
        /// Converts a System.Drawing.Bitmap to a DirectX 11 texture object.
        /// </summary>
        /// <param name="device">Device on wich the resource should be created.</param>
        /// <param name="bitmap">The source bitmap.</param>
        /// <param name="mipLevels">Total count of levels for mipmapping.</param>
        internal static D3D11.Texture2D LoadTextureFromBitmap(EngineDevice device, GDI.Bitmap bitmap, int mipLevels)
        {
            D3D11.Texture2D result = null;

            // Lock bitmap so it can be accessed for texture loading
            System.Drawing.Imaging.BitmapData bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            try
            {
                // Convert pixel format form GDI to Direct3D
                ConvertPixelFormatGdiToDirect3DOrReverse(
                    bitmapData.Scan0, bitmapData.Stride, bitmap.Height,
                    PixelConvertMode.GdiToDirect3D);

                // Open a reading stream for bitmap memory
                DataRectangle dataRectangle = new DataRectangle(bitmapData.Scan0, bitmap.Width * 4);

                // Load the texture
                result = new D3D11.Texture2D(device.DeviceD3D11, new D3D11.Texture2DDescription()
                {
                    BindFlags = D3D11.BindFlags.ShaderResource | D3D11.BindFlags.RenderTarget,
                    CpuAccessFlags = D3D11.CpuAccessFlags.None,
                    Format = DEFAULT_TEXTURE_FORMAT,
                    OptionFlags = D3D11.ResourceOptionFlags.None | D3D11.ResourceOptionFlags.GenerateMipMaps,
                    MipLevels = 0,
                    Usage = D3D11.ResourceUsage.Default,
                    Width = bitmap.Width,
                    Height = bitmap.Height,
                    ArraySize = 1,
                    SampleDescription = new DXGI.SampleDescription(1, 0)
                }, new DataRectangle[] { dataRectangle, dataRectangle, dataRectangle, dataRectangle, dataRectangle, dataRectangle, dataRectangle, dataRectangle, dataRectangle, dataRectangle, dataRectangle, dataRectangle });

                // Workaround for now... auto generate mip-levels
                using (D3D11.ShaderResourceView shaderResourceView = new D3D11.ShaderResourceView(device.DeviceD3D11, result))
                {
                    device.DeviceImmediateContextD3D11.GenerateMips(shaderResourceView);
                }
            }
            finally
            {
                // Free bitmap-access resources
                bitmap.UnlockBits(bitmapData);
            }

            return result;
        }

        /// <summary>
        /// Loads a bitmap from the given texture. Be careful: The texture musst have CPU read access and this only matches for staging textures.
        /// </summary>
        /// <param name="device">The device on which the texture is created.</param>
        /// <param name="stagingTexture">The texture to be loaded into the bitmap.</param>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        internal static GDI.Bitmap LoadBitmapFromStagingTexture(EngineDevice device, D3D11.Texture2D stagingTexture, int width, int height)
        {
            //Prepare target bitmap 
            GDI.Bitmap resultBitmap = new GDI.Bitmap(width, height);
            SharpDX.DataBox dataBox = device.DeviceImmediateContextD3D11.MapSubresource(stagingTexture, 0, D3D11.MapMode.Read, D3D11.MapFlags.None);
            try
            {
                //Lock bitmap so it can be accessed for texture loading
                System.Drawing.Imaging.BitmapData bitmapData = resultBitmap.LockBits(
                    new System.Drawing.Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                try
                {
                    //Copy data row by row
                    // => Rows form datasource may have more pixels because driver changes the size of textures
                    UIntPtr rowPitch = new UIntPtr((uint)(width * 4));
                    for (int loopRow = 0; loopRow < height; loopRow++)
                    {
                        // Copy bitmap data
                        int rowPitchSource = dataBox.RowPitch;
                        int rowPitchDestination = width * 4;
                        NativeMethods.MemCopy(
                            bitmapData.Scan0 + loopRow * rowPitchDestination,
                            dataBox.DataPointer + loopRow * rowPitchSource,
                            rowPitch);

                        // Convert pixel data
                        GraphicsHelper.ConvertPixelFormatGdiToDirect3DOrReverse(
                            bitmapData.Scan0 + loopRow * rowPitchDestination,
                            rowPitchDestination, 1,
                            PixelConvertMode.Direct3DToGdi);
                    }
                }
                finally
                {
                    resultBitmap.UnlockBits(bitmapData);
                }
            }
            finally
            {
                device.DeviceImmediateContextD3D11.UnmapSubresource(stagingTexture, 0);
            }
            return resultBitmap;
        }
#endif

#if WINRT || UNIVERSAL
        /// <summary>
        /// Creates the SwapChain object that is used on WinRT platforms.
        /// </summary>
        /// <param name="device">The device on which to create the SwapChain.</param>
        /// <param name="width">Width of the screen in pixels.</param>
        /// <param name="height">Height of the screen in pixels.</param>
        /// <param name="gfxConfig">Current graphics configuration.</param>
        internal static DXGI.SwapChain1 CreateSwapChainForComposition(EngineDevice device, int width, int height, GraphicsViewConfiguration gfxConfig)
        {
            DXGI.SwapChainDescription1 desc = new SharpDX.DXGI.SwapChainDescription1()
            {
                Width = width,
                Height = height,
                Format = DEFAULT_TEXTURE_FORMAT,
                Stereo = false,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = SharpDX.DXGI.Usage.BackBuffer | SharpDX.DXGI.Usage.RenderTargetOutput,
                BufferCount = 2,
                Scaling = SharpDX.DXGI.Scaling.Stretch,
                SwapEffect = SharpDX.DXGI.SwapEffect.FlipSequential,
            };
            
            //Creates the swap chain for XAML composition
            return new DXGI.SwapChain1(device.FactoryDxgi, device.DeviceD3D11, ref desc);
        }
#endif

        /// <summary>
        /// Switches the pixel format between GDI and Direct3D.
        /// </summary>
        /// <param name="scan0">Pointer to the beginning of the bitmap data in local memory.</param>
        /// <param name="stride">Size in bytes of one line of pixels.</param>
        /// <param name="height">The total height of the bitmap in pixels.</param>
        internal static void ConvertPixelFormatGdiToDirect3DOrReverse(IntPtr scan0, int stride, int height, PixelConvertMode convertMode)
        {
            //switch(convertMode)
            //{
            //    case PixelConvertMode.GdiToDirect3D:
            //        unsafe
            //        {
            //            byte* startPointer = (byte*)scan0;
            //            for (int loop = 0; loop < (stride / 4) * height; loop++)
            //            {
            //                // Be careful: This is format ARGB in reverse order
            //                //   .. don't know exactly why, but testet with debugger and some images
            //                byte blueValue = startPointer[loop * 4];
            //                byte greenValue = startPointer[loop * 4 + 1];
            //                byte redValue = startPointer[loop * 4 + 2];
            //                byte alphaValue = startPointer[loop * 4 + 3];

            //                // Apply new format as in constant DEFAULT_TEXTURE_FORMAT
            //                startPointer[loop * 4] = redValue;
            //                startPointer[loop * 4 + 1] = greenValue;
            //                startPointer[loop * 4 + 2] = blueValue;
            //                startPointer[loop * 4 + 3] = alphaValue;
            //            }
            //        }
            //        break;

            //    case PixelConvertMode.Direct3DToGdi:
            //        unsafe
            //        {
            //            byte* startPointer = (byte*)scan0;
            //            for (int loop = 0; loop < (stride / 4) * height; loop++)
            //            {
            //                // Be careful: This is format ARGB in reverse order
            //                //   .. don't know exactly why, but testet with debugger and some images
            //                byte redValue = startPointer[loop * 4];
            //                byte greenValue = startPointer[loop * 4 + 1];
            //                byte blueValue = startPointer[loop * 4 + 2];
            //                byte alphaValue = startPointer[loop * 4 + 3];

            //                // Apply new format as in constant DEFAULT_TEXTURE_FORMAT
            //                startPointer[loop * 4] = blueValue;
            //                startPointer[loop * 4 + 1] = greenValue;
            //                startPointer[loop * 4 + 2] = redValue;
            //                startPointer[loop * 4 + 3] = alphaValue;
            //            }
            //        }
            //        break;
            //}
        }

#if WINDOWS_PHONE
        /// <summary>
        /// Creates a Direct3D 11 texture that can be shared between more devices.
        /// </summary>
        /// <param name="device">The Direct3D 11 device.</param>
        /// <param name="width">The width of the generated texture.</param>
        /// <param name="height">The height of the generated texture.</param>
        internal static D3D11.Texture2D CreateSharedTextureWP8Xaml(EngineDevice device, int width, int height)
        {
            D3D11.Texture2DDescription textureDescription = new D3D11.Texture2DDescription
            {
                Format = DEFAULT_TEXTURE_FORMAT_SHARING,
                Width = width,
                Height = height,
                ArraySize = 1,
                MipLevels = 1,
                BindFlags = D3D11.BindFlags.RenderTarget | D3D11.BindFlags.ShaderResource,
                Usage = D3D11.ResourceUsage.Default,
                CpuAccessFlags = D3D11.CpuAccessFlags.None,
                OptionFlags = D3D11.ResourceOptionFlags.SharedKeyedmutex | D3D11.ResourceOptionFlags.SharedNthandle,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0)
            };
            return new D3D11.Texture2D(device.DeviceD3D11, textureDescription);
        }
#endif

        /// <summary>
        /// Creates a Direct3D 11 texture that can be shared between more devices.
        /// </summary>
        /// <param name="device">The Direct3D 11 device.</param>
        /// <param name="width">The width of the generated texture.</param>
        /// <param name="height">The height of the generated texture.</param>
        internal static D3D11.Texture2D CreateSharedTexture(EngineDevice device, int width, int height)
        {
            D3D11.Texture2DDescription textureDescription = new D3D11.Texture2DDescription
            {
                BindFlags = D3D11.BindFlags.RenderTarget | D3D11.BindFlags.ShaderResource,
                Format = DEFAULT_TEXTURE_FORMAT_SHARING,
                Width = width,
                Height = height,
                MipLevels = 1,
                SampleDescription = new DXGI.SampleDescription(1, 0),
                Usage = D3D11.ResourceUsage.Default,
                OptionFlags = D3D11.ResourceOptionFlags.Shared,
                CpuAccessFlags = D3D11.CpuAccessFlags.None,
                ArraySize = 1
            };
            return new D3D11.Texture2D(device.DeviceD3D11, textureDescription);
        }

#if DESKTOP
        /// <summary>
        /// Creates a Direct3D 10 texture that can be shared between more devices.
        /// </summary>
        /// <param name="device">The Direct3D 10 device.</param>
        /// <param name="width">The width of the generated texture.</param>
        /// <param name="height">The height of the generated texture.</param>
        internal static D3D10.Texture2D CreateSharedTexture10(EngineDevice device, int width, int height)
        {
            D3D10.Texture2DDescription textureDescription = new D3D10.Texture2DDescription
            {
                BindFlags = D3D10.BindFlags.RenderTarget | D3D10.BindFlags.ShaderResource,
                Format = DEFAULT_TEXTURE_FORMAT,
                Width = width,
                Height = height,
                MipLevels = 1,
                SampleDescription = new DXGI.SampleDescription(1, 0),
                Usage = D3D10.ResourceUsage.Default,
                OptionFlags = D3D10.ResourceOptionFlags.Shared,
                CpuAccessFlags = D3D10.CpuAccessFlags.None,
                ArraySize = 1
            };
            return new D3D10.Texture2D(device.DeviceD3D10, textureDescription);
        }
#endif

#if !WINDOWS_PHONE
        /// <summary>
        /// Loads the texture2 D from stream.
        /// </summary>
        /// <param name="device">The device on wich to create the texture.</param>
        /// <param name="inStream">The source stream.</param>
        /// <returns></returns>
        internal static D3D11.Texture2D LoadTexture2D(EngineDevice device, Stream inStream)
        {
            return LoadTexture2DFromBitmap(
                device,
                LoadBitmapSource(inStream));
        }

        /// <summary>
        /// Loads a new texture from the given file path.
        /// </summary>
        /// <param name="device">The device on wich to create the texture.</param>
        /// <param name="fileName">The source file</param>
        /// <returns></returns>
        internal static D3D11.Texture2D LoadTexture2D(EngineDevice device, string fileName)
        {
            return LoadTexture2DFromBitmap(
                device,
                LoadBitmap(fileName));
        }

        /// <summary>
        /// Loads a bitmap using WIC.
        /// </summary>
        /// <param name="inStream">The stream from wich to load the texture file.</param>
        internal static WIC.BitmapSource LoadBitmapSource(Stream inStream)
        {
            var bitmapDecoder = new SharpDX.WIC.BitmapDecoder(
                GraphicsCore.Current.FactoryWIC,
                inStream,
                SharpDX.WIC.DecodeOptions.CacheOnDemand
                );
            var formatConverter = new WIC.FormatConverter(GraphicsCore.Current.FactoryWIC);

            formatConverter.Initialize(
                bitmapDecoder.GetFrame(0),
                DEFAULT_WIC_BITMAP_FORMAT,
                WIC.BitmapDitherType.None,
                null,
                0.0,
                WIC.BitmapPaletteType.Custom);

            return formatConverter;
        }

        /// <summary>
        /// Loads a bitmap using WIC.
        /// </summary>
        /// <param name="inStream">The file from wich to load the texture.</param>
        internal static WIC.BitmapSource LoadBitmap(string filename)
        {
            var bitmapDecoder = new SharpDX.WIC.BitmapDecoder(
                GraphicsCore.Current.FactoryWIC,
                filename,
                SharpDX.WIC.DecodeOptions.CacheOnDemand
                );

            var formatConverter = new WIC.FormatConverter(GraphicsCore.Current.FactoryWIC);

            formatConverter.Initialize(
                bitmapDecoder.GetFrame(0),
                DEFAULT_WIC_BITMAP_FORMAT,
                WIC.BitmapDitherType.None,
                null,
                0.0,
                WIC.BitmapPaletteType.Custom);

            return formatConverter;
        }

        /// <summary>
        /// Creates a <see cref="SharpDX.Direct3D11.Texture2D"/> from a WIC <see cref="SharpDX.WIC.BitmapSource"/>
        /// </summary>
        /// <param name="device">The Direct3D11 device</param>
        /// <param name="bitmapSource">The WIC bitmap source</param>
        /// <returns>A Texture2D</returns>
        internal static D3D11.Texture2D LoadTexture2DFromBitmap(EngineDevice device, WIC.BitmapSource bitmapSource)
        {
            // Allocate DataStream to receive the WIC image pixels
            int stride = bitmapSource.Size.Width * 4;
            using (var buffer = new SharpDX.DataStream(bitmapSource.Size.Height * stride, true, true))
            {
                // Copy the content of the WIC to the buffer
                bitmapSource.CopyPixels(stride, buffer);

                //Create the texture
                var dataRectangle = new SharpDX.DataRectangle(buffer.DataPointer, stride);
                D3D11.Texture2D result = new D3D11.Texture2D(device.DeviceD3D11, new SharpDX.Direct3D11.Texture2DDescription()
                {
                    Width = bitmapSource.Size.Width,
                    Height = bitmapSource.Size.Height,
                    ArraySize = 1,
                    BindFlags = SharpDX.Direct3D11.BindFlags.ShaderResource | D3D11.BindFlags.RenderTarget,
                    Usage = SharpDX.Direct3D11.ResourceUsage.Default,
                    CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                    Format = DEFAULT_TEXTURE_FORMAT,
                    MipLevels = 0,
                    OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.None | D3D11.ResourceOptionFlags.GenerateMipMaps,
                    SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                }, new DataRectangle[] { dataRectangle, dataRectangle, dataRectangle, dataRectangle, dataRectangle, dataRectangle, dataRectangle, dataRectangle, dataRectangle, dataRectangle, dataRectangle, dataRectangle });

                //Workaround for now... auto generate mip-levels
                // TODO: Dispatch this call to render-thread..
                using (D3D11.ShaderResourceView shaderResourceView = new D3D11.ShaderResourceView(device.DeviceD3D11, result))
                {
                    device.DeviceImmediateContextD3D11.GenerateMips(shaderResourceView);
                }

                //Return the generated texture
                return result;
            }
        }
#endif

#if DESKTOP
        /// <summary>
        /// Loads a bitmap from the given mapped texture.
        /// </summary>
        /// <param name="mappedTexture">The mapped texture from which to read all pixel data.</param>
        internal static GDI.Bitmap LoadBitmapFromMappedTexture(MemoryMappedTexture32bpp mappedTexture)
        {
            GDI.Bitmap resultBitmap = new GDI.Bitmap(mappedTexture.Width, mappedTexture.Height);

            //Lock bitmap so it can be accessed for texture loading
            System.Drawing.Imaging.BitmapData bitmapData = resultBitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height),
                System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            try
            {
                //Copy data row by row
                // => Rows form datasource may have more pixels because driver changes the size of textures
                UIntPtr rowPitch = new UIntPtr((uint)(mappedTexture.Width * 4));
                for (int loopRow = 0; loopRow < mappedTexture.Height; loopRow++)
                {
                    // Copy bitmap data
                    int rowPitchSource = mappedTexture.Pitch;
                    int rowPitchDestination = mappedTexture.Width * 4;
                    NativeMethods.MemCopy(
                        bitmapData.Scan0 + loopRow * rowPitchDestination,
                        mappedTexture.Pointer + loopRow * rowPitchSource,
                        rowPitch);
                }
            }
            finally
            {
                resultBitmap.UnlockBits(bitmapData);
            }

            return resultBitmap;
        }
#endif

        /// <summary>
        /// Creates a default viewport for the given width and height
        /// </summary>
        /// <param name="targetControl">Target control object.</param>
        internal static SharpDX.ViewportF CreateDefaultViewport(int width, int height)
        {
            SharpDX.ViewportF result = new SharpDX.ViewportF(
                0f, 0f,
                (float)width, (float)height,
                0f, 1f);
            return result;
        }

        /// <summary>
        /// Creates a standard texture with the given width and height.
        /// </summary>
        /// <param name="device">Graphics device.</param>
        /// <param name="width">Width of generated texture.</param>
        /// <param name="height">Height of generated texture.</param>
        /// <param name="format">The format which is used to create the texture.</param>
        internal static D3D11.Texture2D CreateTexture(EngineDevice device, int width, int height, DXGI.Format format = DEFAULT_TEXTURE_FORMAT)
        {
            D3D11.Texture2DDescription textureDescription = new D3D11.Texture2DDescription();
            textureDescription.Width = width;
            textureDescription.Height = height;
            textureDescription.MipLevels = 1;
            textureDescription.ArraySize = 1;
            textureDescription.Format = format;
            textureDescription.Usage = D3D11.ResourceUsage.Default;
            textureDescription.SampleDescription = new DXGI.SampleDescription(1, 0);
            textureDescription.BindFlags = D3D11.BindFlags.ShaderResource;
            textureDescription.CpuAccessFlags = D3D11.CpuAccessFlags.None;
            textureDescription.OptionFlags = D3D11.ResourceOptionFlags.None;

            return new D3D11.Texture2D(device.DeviceD3D11, textureDescription);
        }

        /// <summary>
        /// Creates a standard texture with the given width and height.
        /// </summary>
        /// <param name="device">Graphics device.</param>
        /// <param name="width">Width of generated texture.</param>
        /// <param name="height">Height of generated texture.</param>
        /// <param name="rawData">Raw data to be loaded into the texture.</param>
        internal static D3D11.Texture2D CreateTexture(EngineDevice device, int width, int height, SharpDX.DataBox[] rawData)
        {
            D3D11.Texture2DDescription textureDescription = new D3D11.Texture2DDescription();
            textureDescription.Width = width;
            textureDescription.Height = height;
            textureDescription.MipLevels = 1;
            textureDescription.ArraySize = 1;
            textureDescription.Format = DEFAULT_TEXTURE_FORMAT;
            textureDescription.Usage = D3D11.ResourceUsage.Default;
            textureDescription.SampleDescription = new DXGI.SampleDescription(1, 0);
            textureDescription.BindFlags = D3D11.BindFlags.ShaderResource;
            textureDescription.CpuAccessFlags = D3D11.CpuAccessFlags.None;
            textureDescription.OptionFlags = D3D11.ResourceOptionFlags.None;

            return new D3D11.Texture2D(device.DeviceD3D11, textureDescription, rawData);
        }

        /// <summary>
        /// Creates a new image based on the given raw image data.
        /// </summary>
        /// <param name="device">Graphics device.</param>
        /// <param name="rawImage">Raw image data.</param>
        internal static D3D11.Texture2D CreateTexture(EngineDevice device, TKGFX.Image rawImage)
        {
            D3D11.Texture2DDescription textureDescription = new D3D11.Texture2DDescription();
            textureDescription.Width = rawImage.Description.Width;
            textureDescription.Height = rawImage.Description.Height;
            textureDescription.MipLevels = rawImage.Description.MipLevels;
            textureDescription.ArraySize = rawImage.Description.ArraySize;
            textureDescription.Format = rawImage.Description.Format;
            textureDescription.Usage = D3D11.ResourceUsage.Default;
            textureDescription.SampleDescription = new DXGI.SampleDescription(1, 0);
            textureDescription.BindFlags = D3D11.BindFlags.ShaderResource;
            textureDescription.CpuAccessFlags = D3D11.CpuAccessFlags.None;
            textureDescription.OptionFlags = D3D11.ResourceOptionFlags.None;

            // Special handling for cube textures
            if (rawImage.Description.Dimension == TKGFX.TextureDimension.TextureCube)
            {
                textureDescription.OptionFlags = D3D11.ResourceOptionFlags.TextureCube;
            }

            return new D3D11.Texture2D(device.DeviceD3D11, textureDescription, rawImage.ToDataBox());
        }

        /// <summary>
        /// Creates a staging texture which enables copying data from gpu to cpu memory.
        /// </summary>
        /// <param name="device">Graphics device.</param>
        /// <param name="width">Width of generated texture.</param>
        /// <param name="height">Height of generated texture.</param>
        /// <param name="format">The format used to create the texture.</param>
        internal static D3D11.Texture2D CreateStagingTexture(EngineDevice device, int width, int height, DXGI.Format format = DEFAULT_TEXTURE_FORMAT)
        {
            //For handling of staging resource see
            // http://msdn.microsoft.com/en-us/library/windows/desktop/ff476259(v=vs.85).aspx

            D3D11.Texture2DDescription textureDescription = new D3D11.Texture2DDescription();
            textureDescription.Width = width;
            textureDescription.Height = height;
            textureDescription.MipLevels = 1;
            textureDescription.ArraySize = 1;
            textureDescription.Format = format;
            textureDescription.Usage = D3D11.ResourceUsage.Staging;
            textureDescription.SampleDescription = new DXGI.SampleDescription(1, 0);
            textureDescription.BindFlags = D3D11.BindFlags.None;
            textureDescription.CpuAccessFlags = D3D11.CpuAccessFlags.Read;
            textureDescription.OptionFlags = D3D11.ResourceOptionFlags.None;

            return new D3D11.Texture2D(device.DeviceD3D11, textureDescription);
        }

        /// <summary>
        /// Creates a staging texture which enables copying data from gpu to cpu memory.
        /// </summary>
        /// <param name="device">Graphics device.</param>
        /// <param name="width">Width of generated texture.</param>
        /// <param name="height">Height of generated texture.</param>
        internal static D3D11.Texture2D CreateStagingTexture(EngineDevice device, int width, int height)
        {
            //For handling of staging resource see
            // http://msdn.microsoft.com/en-us/library/windows/desktop/ff476259(v=vs.85).aspx

            D3D11.Texture2DDescription textureDescription = new D3D11.Texture2DDescription();
            textureDescription.Width = width;
            textureDescription.Height = height;
            textureDescription.MipLevels = 1;
            textureDescription.ArraySize = 1;
            textureDescription.Format = DEFAULT_TEXTURE_FORMAT;
            textureDescription.Usage = D3D11.ResourceUsage.Staging;
            textureDescription.SampleDescription = new DXGI.SampleDescription(1, 0);
            textureDescription.BindFlags = D3D11.BindFlags.None;
            textureDescription.CpuAccessFlags = D3D11.CpuAccessFlags.Read;
            textureDescription.OptionFlags = D3D11.ResourceOptionFlags.None;

            return new D3D11.Texture2D(device.DeviceD3D11, textureDescription);
        }

        /// <summary>
        /// Creates a render target texture with the given width and height.
        /// This texture is used to receive normal and depth data (xyzw components) and stores data in floating point format.
        /// </summary>
        /// <param name="device">Graphics device.</param>
        /// <param name="width">Width of generated texture.</param>
        /// <param name="height">Height of generated texture.</param>
        /// <param name="gfxConfig">The GFX configuration.</param>
        internal static D3D11.Texture2D CreateRenderTargetTextureNormalDepth(
            EngineDevice device, int width, int height, GraphicsViewConfiguration gfxConfig)
        {
            D3D11.Texture2DDescription textureDescription = new D3D11.Texture2DDescription();

            if ((gfxConfig.AntialiasingEnabled) &&
                (device.IsStandardAntialiasingPossible))
            {
                textureDescription.Width = width;
                textureDescription.Height = height;
                textureDescription.MipLevels = 1;
                textureDescription.ArraySize = 1;
                textureDescription.Format = DEFAULT_TEXTURE_FORMAT_NORMAL_DEPTH;
                textureDescription.Usage = D3D11.ResourceUsage.Default;
                textureDescription.SampleDescription = device.GetSampleDescription(gfxConfig.AntialiasingQuality);
                textureDescription.BindFlags = D3D11.BindFlags.ShaderResource | D3D11.BindFlags.RenderTarget;
                textureDescription.CpuAccessFlags = D3D11.CpuAccessFlags.None;
                textureDescription.OptionFlags = D3D11.ResourceOptionFlags.None;
            }
            else
            {
                textureDescription.Width = width;
                textureDescription.Height = height;
                textureDescription.MipLevels = 1;
                textureDescription.ArraySize = 1;
                textureDescription.Format = DEFAULT_TEXTURE_FORMAT_NORMAL_DEPTH;
                textureDescription.Usage = D3D11.ResourceUsage.Default;
                textureDescription.SampleDescription = new DXGI.SampleDescription(1, 0);
                textureDescription.BindFlags = D3D11.BindFlags.ShaderResource | D3D11.BindFlags.RenderTarget;
                textureDescription.CpuAccessFlags = D3D11.CpuAccessFlags.None;
                textureDescription.OptionFlags = D3D11.ResourceOptionFlags.None;
            }

            return new D3D11.Texture2D(device.DeviceD3D11, textureDescription);
        }

        /// <summary>
        /// Creates a render target texture with the given width and height.
        /// This texture is used to receive ObjectIDs and stores data as single unsigned integers (32-Bit).
        /// </summary>
        /// <param name="device">Graphics device.</param>
        /// <param name="width">Width of generated texture.</param>
        /// <param name="height">Height of generated texture.</param>
        /// <param name="gfxConfig">The GFX configuration.</param>
        internal static D3D11.Texture2D CreateRenderTargetTextureObjectIDs(
            EngineDevice device, int width, int height, GraphicsViewConfiguration gfxConfig)
        {
            D3D11.Texture2DDescription textureDescription = new D3D11.Texture2DDescription();

            if ((gfxConfig.AntialiasingEnabled) &&
                (device.IsStandardAntialiasingPossible))
            {
                textureDescription.Width = width;
                textureDescription.Height = height;
                textureDescription.MipLevels = 1;
                textureDescription.ArraySize = 1;
                textureDescription.Format = DEFAULT_TEXTURE_FORMAT_OBJECT_ID;
                textureDescription.Usage = D3D11.ResourceUsage.Default;
                textureDescription.SampleDescription = device.GetSampleDescription(gfxConfig.AntialiasingQuality);
                textureDescription.BindFlags = D3D11.BindFlags.ShaderResource | D3D11.BindFlags.RenderTarget;
                textureDescription.CpuAccessFlags = D3D11.CpuAccessFlags.None;
                textureDescription.OptionFlags = D3D11.ResourceOptionFlags.None;
            }
            else
            {
                textureDescription.Width = width;
                textureDescription.Height = height;
                textureDescription.MipLevels = 1;
                textureDescription.ArraySize = 1;
                textureDescription.Format = DEFAULT_TEXTURE_FORMAT_OBJECT_ID;
                textureDescription.Usage = D3D11.ResourceUsage.Default;
                textureDescription.SampleDescription = new DXGI.SampleDescription(1, 0);
                textureDescription.BindFlags = D3D11.BindFlags.ShaderResource | D3D11.BindFlags.RenderTarget;
                textureDescription.CpuAccessFlags = D3D11.CpuAccessFlags.None;
                textureDescription.OptionFlags = D3D11.ResourceOptionFlags.None;
            }

            return new D3D11.Texture2D(device.DeviceD3D11, textureDescription);
        }

        /// <summary>
        /// Creates a render target texture with the given width and height.
        /// </summary>
        /// <param name="device">Graphics device.</param>
        /// <param name="width">Width of generated texture.</param>
        /// <param name="height">Height of generated texture.</param>
        /// <param name="gfxConfig">The GFX configuration.</param>
        internal static D3D11.Texture2D CreateRenderTargetTexture(
            EngineDevice device, int width, int height, GraphicsViewConfiguration gfxConfig)
        {
            D3D11.Texture2DDescription textureDescription = new D3D11.Texture2DDescription();

            if ((gfxConfig.AntialiasingEnabled) &&
                (device.IsStandardAntialiasingPossible))
            {
                textureDescription.Width = width;
                textureDescription.Height = height;
                textureDescription.MipLevels = 1;
                textureDescription.ArraySize = 1;
                textureDescription.Format = DEFAULT_TEXTURE_FORMAT;
                textureDescription.Usage = D3D11.ResourceUsage.Default;
                textureDescription.SampleDescription = device.GetSampleDescription(gfxConfig.AntialiasingQuality);
                textureDescription.BindFlags = D3D11.BindFlags.ShaderResource | D3D11.BindFlags.RenderTarget;
                textureDescription.CpuAccessFlags = D3D11.CpuAccessFlags.None;
                textureDescription.OptionFlags = D3D11.ResourceOptionFlags.None;
            }
            else
            {
                textureDescription.Width = width;
                textureDescription.Height = height;
                textureDescription.MipLevels = 1;
                textureDescription.ArraySize = 1;
                textureDescription.Format = DEFAULT_TEXTURE_FORMAT;
                textureDescription.Usage = D3D11.ResourceUsage.Default;
                textureDescription.SampleDescription = new DXGI.SampleDescription(1, 0);
                textureDescription.BindFlags = D3D11.BindFlags.ShaderResource | D3D11.BindFlags.RenderTarget;
                textureDescription.CpuAccessFlags = D3D11.CpuAccessFlags.None;
                textureDescription.OptionFlags = D3D11.ResourceOptionFlags.None;
            }

            return new D3D11.Texture2D(device.DeviceD3D11, textureDescription);
        }

        /// <summary>
        /// Creates a depth buffer texture with given width and height.
        /// </summary>
        /// <param name="device">Graphics device.</param>
        /// <param name="width">Width of generated texture.</param>
        /// <param name="height">Height of generated texture.</param>
        internal static D3D11.Texture2D CreateDepthBufferTexture(EngineDevice device, int width, int height, GraphicsViewConfiguration gfxConfig)
        {
            D3D11.Texture2DDescription textureDescription = new D3D11.Texture2DDescription();

            if ((gfxConfig.AntialiasingEnabled) &&
                (device.IsStandardAntialiasingPossible))
            {
                textureDescription.Width = width;
                textureDescription.Height = height;
                textureDescription.MipLevels = 1;
                textureDescription.ArraySize = 1;
                textureDescription.Usage = D3D11.ResourceUsage.Default;
                textureDescription.SampleDescription = device.GetSampleDescription(gfxConfig.AntialiasingQuality);
                textureDescription.BindFlags = D3D11.BindFlags.DepthStencil;
                textureDescription.CpuAccessFlags = D3D11.CpuAccessFlags.None;
                textureDescription.OptionFlags = D3D11.ResourceOptionFlags.None;
            }
            else
            {
                textureDescription.Width = width;
                textureDescription.Height = height;
                textureDescription.MipLevels = 1;
                textureDescription.ArraySize = 1;
                textureDescription.Usage = D3D11.ResourceUsage.Default;
                textureDescription.SampleDescription = new DXGI.SampleDescription(1, 0);
                textureDescription.BindFlags = D3D11.BindFlags.DepthStencil;
                textureDescription.CpuAccessFlags = D3D11.CpuAccessFlags.None;
                textureDescription.OptionFlags = D3D11.ResourceOptionFlags.None;
            }

            // Set buffer format
            switch (device.DriverLevel)
            {
                case HardwareDriverLevel.Direct3D11:
                case HardwareDriverLevel.Direct3D10:
                    textureDescription.Format = DXGI.Format.D32_Float_S8X24_UInt;
                    break;

                // This would be for Direct3D 9 hardware
                case HardwareDriverLevel.Direct3D9_1:
                case HardwareDriverLevel.Direct3D9_2:
                case HardwareDriverLevel.Direct3D9_3:
                    textureDescription.Format = DXGI.Format.D24_UNorm_S8_UInt;
                    break;

                default:
                    throw new FrozenSkyGraphicsException("Unable to create depth buffer texture because of unsupported DriverLevel: " + device.DriverLevel);
            }

            // Create the texture finally
            return new D3D11.Texture2D(device.DeviceD3D11, textureDescription);
        }

        /// <summary>
        /// Create a new DepthBuffer view to bind the given depth buffer to the rendering device.
        /// </summary>
        /// <param name="device">The device on which to create the view.</param>
        /// <param name="depthBuffer">The target resource.</param>
        internal static D3D11.DepthStencilView CreateDepthBufferView(EngineDevice device, D3D11.Texture2D depthBuffer)
        {
            return new D3D11.DepthStencilView(device.DeviceD3D11, depthBuffer);
        }

        /// <summary>
        /// Calculate the Bias value depending on the given value to be added to the depth buffer.
        /// </summary>
        /// <param name="zValue">The z value to be added.</param>
        public static int GetDepthBiasValue(EngineDevice device, float zValue)
        {
            // Calulation depends on depth buffer format
            // see http://msdn.microsoft.com/de-de/library/windows/desktop/cc308048(v=vs.85).aspx
            // see Book "3D Game Programming With Direct3D 11, Frank D. Luna, 2012" Page 678

            switch (device.DriverLevel)
            {
                case HardwareDriverLevel.Direct3D11:
                case HardwareDriverLevel.Direct3D10:
                    return (int)(zValue / (1 / Math.Pow(2, 23)));

                case HardwareDriverLevel.Direct3D9_1:
                case HardwareDriverLevel.Direct3D9_2:
                case HardwareDriverLevel.Direct3D9_3:
                    return (int)(Math.Floor(zValue * (1f / (float)(2 ^ 24))));

                default:
                    throw new FrozenSkyGraphicsException("Unable to calculate depth bias value: Target hardware unknown!");
            }
        }

        /// <summary>
        /// Creates a dynamic vertex buffer for the given vertex type and maximum capacity.
        /// </summary>
        /// <typeparam name="T">Type of the vertices.</typeparam>
        /// <param name="device">Graphics device.</param>
        /// <param name="vertexCount">Maximum count of vertices within the buffer.</param>
        internal static D3D11.Buffer CreateDynamicVertexBuffer<T>(EngineDevice device, int vertexCount)
            where T : struct
        {
            Type vertexType = typeof(T);
            int vertexSize = Marshal.SizeOf<T>();

            D3D11.BufferDescription bufferDescription = new D3D11.BufferDescription();
            bufferDescription.BindFlags = D3D11.BindFlags.VertexBuffer;
            bufferDescription.CpuAccessFlags = D3D11.CpuAccessFlags.Write;
            bufferDescription.OptionFlags = D3D11.ResourceOptionFlags.None;
            bufferDescription.SizeInBytes = vertexCount * vertexSize;
            bufferDescription.Usage = D3D11.ResourceUsage.Dynamic;
            bufferDescription.StructureByteStride = vertexCount * vertexSize;

            return new D3D11.Buffer(device.DeviceD3D11, bufferDescription);
        }

        /// <summary>
        /// Creates an immutable vertex buffer from the given vertex array.
        /// </summary>
        /// <typeparam name="T">Type of a vertex.</typeparam>
        /// <param name="device">Graphics device.</param>
        /// <param name="vertices">The vertex array.</param>
        internal static D3D11.Buffer CreateImmutableVertexBuffer<T>(EngineDevice device, params T[][] vertices)
            where T : struct
        {
            Type vertexType = typeof(T);
            int vertexCount = vertices.Sum((actArray) => actArray.Length);
            int vertexSize = Marshal.SizeOf<T>();
            DataStream outStream = new DataStream(
                vertexCount * vertexSize,
                true, true);

            foreach (T[] actArray in vertices)
            {
                outStream.WriteRange(actArray);
            }
            outStream.Position = 0;

            D3D11.BufferDescription bufferDescription = new D3D11.BufferDescription();
            bufferDescription.BindFlags = D3D11.BindFlags.VertexBuffer;
            bufferDescription.CpuAccessFlags = D3D11.CpuAccessFlags.None;
            bufferDescription.OptionFlags = D3D11.ResourceOptionFlags.None;
            bufferDescription.SizeInBytes = vertexCount * vertexSize;
            bufferDescription.Usage = D3D11.ResourceUsage.Immutable;
            bufferDescription.StructureByteStride = vertexSize;

            D3D11.Buffer result = new D3D11.Buffer(device.DeviceD3D11, outStream, bufferDescription);
            outStream.Dispose();

            return result;
        }

        /// <summary>
        /// Creates an immutable index buffer from the given index array.
        /// </summary>
        /// <param name="device">Graphics device.</param>
        /// <param name="indices">Source index array.</param>
        internal static D3D11.Buffer CreateImmutableIndexBuffer(EngineDevice device, params int[][] indices)
        {
            int countIndices = indices.Sum((actArray) => actArray.Length);
            int bytesPerIndex = device.SupportsOnly16BitIndexBuffer ? Marshal.SizeOf<ushort>() : Marshal.SizeOf<uint>();


            DataStream outStreamIndex = new DataStream(
                countIndices *
                bytesPerIndex, true, true);

            // Write all instance data to the target stream
            foreach (int[] actArray in indices)
            {
                int actArrayLength = actArray.Length;
                for (int loop = 0; loop < actArrayLength; loop++)
                {
                    if (device.SupportsOnly16BitIndexBuffer) { outStreamIndex.Write((ushort)actArray[loop]); }
                    else { outStreamIndex.Write((uint)actArray[loop]); }
                }
            }
            outStreamIndex.Position = 0;

            // Configure index buffer
            D3D11.BufferDescription bufferDescriptionIndex = new D3D11.BufferDescription();
            bufferDescriptionIndex.BindFlags = D3D11.BindFlags.IndexBuffer;
            bufferDescriptionIndex.CpuAccessFlags = D3D11.CpuAccessFlags.None;
            bufferDescriptionIndex.OptionFlags = D3D11.ResourceOptionFlags.None;
            bufferDescriptionIndex.SizeInBytes = countIndices * bytesPerIndex;
            bufferDescriptionIndex.Usage = D3D11.ResourceUsage.Immutable;

            // Load the index buffer
            D3D11.Buffer result = new D3D11.Buffer(device.DeviceD3D11, outStreamIndex, bufferDescriptionIndex);

            outStreamIndex.Dispose();

            return result;
        }

#if DESKTOP
        /// <summary>
        /// Resizes the given bitmap to given size.
        /// </summary>
        /// <param name="bitmapToResize">The bitmap to resize.</param>
        /// <param name="newWidth">Width of the generated bitmap.</param>
        /// <param name="newHeight">Height of the genrated bitmap.</param>
        internal static GDI.Bitmap ResizeGdiBitmap(GDI.Bitmap bitmapToResize, int newWidth, int newHeight)
        {
            GDI.Bitmap result = new GDI.Bitmap(newWidth, newHeight);
            using (GDI.Graphics g = GDI.Graphics.FromImage((GDI.Image)result))
            {
                g.DrawImage(bitmapToResize, 0, 0, newWidth, newHeight);
            }
            return result;
        }
#endif

        ///// <summary>
        ///// Loads a Direct2D bitmap from the given gdi resource.
        ///// </summary>
        ///// <param name="drawingBitmap">The source gdi bitmap.</param>
        ///// <param name="renderTarget">The RenderTarget object for wich to create the resource.</param>
        //internal static D2D.Bitmap LoadBitmap(D2D.RenderTarget renderTarget, GDI.Bitmap drawingBitmap)
        //{
        //    D2D.Bitmap result = null;

        //    //Lock the gdi resource
        //    System.Drawing.Imaging.BitmapData drawingBitmapData = drawingBitmap.LockBits(
        //        new GDI.Rectangle(0, 0, drawingBitmap.Width, drawingBitmap.Height),
        //        System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

        //    //Prepare loading the image from gdi resource
        //    DataStream dataStream = new DataStream(
        //        drawingBitmapData.Scan0,
        //        drawingBitmapData.Stride * drawingBitmapData.Height,
        //        true, false);
        //    D2D.BitmapProperties properties = new D2D.BitmapProperties();
        //    properties.PixelFormat = new D2D.PixelFormat(
        //        DEFAULT_TEXTURE_FORMAT,
        //        D2D.AlphaMode.Premultiplied);

        //    //Load the image from the gdi resource
        //    result = new D2D.Bitmap(
        //        renderTarget,
        //        new SharpDX.Size2(drawingBitmap.Width, drawingBitmap.Height),
        //        dataStream, drawingBitmapData.Stride,
        //        properties);

        //    //Unlock the gdi resource
        //    drawingBitmap.UnlockBits(drawingBitmapData);

        //    return result;
        //}

#if DESKTOP
        /// <summary>
        /// Copies all contents of the given gdi bitmap into the given Direct2D bitmap.
        /// </summary>
        /// <param name="targetBitmap">Target Direct2D bitmap.</param>
        /// <param name="drawingBitmap">The source gdi bitmap.</param>
        internal static void SetBitmapContents(D2D.Bitmap targetBitmap, GDI.Bitmap drawingBitmap)
        {
            //Lock the gdi resource
            System.Drawing.Imaging.BitmapData drawingBitmapData = drawingBitmap.LockBits(
                new GDI.Rectangle(0, 0, drawingBitmap.Width, drawingBitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            targetBitmap.CopyFromMemory(drawingBitmapData.Scan0, drawingBitmapData.Stride);
            drawingBitmap.UnlockBits(drawingBitmapData);
        }
#endif

        /// <summary>
        /// Disposes the given object.
        /// </summary>
        internal static void SafeDispose<T>(ref T toDispose)
            where T : class, IDisposable
        {
            toDispose = DisposeObject(toDispose);
        }

        /// <summary>
        /// Disposes the given object and returns null.
        /// </summary>
        internal static T DisposeObject<T>(T objectToDispose)
            where T : class, IDisposable
        {
            if (objectToDispose == null) { return null; }

            try { objectToDispose.Dispose(); }
            catch (Exception)
            {
                // Generic exception handling..
            }
            return null;
        }

        /// <summary>
        /// Disposes the given object.
        /// </summary>
        internal static void SafeDisposeLazy<T>(ref Lazy<T> toDispose)
            where T : class, IDisposable
        {
            toDispose = DisposeObjectLazy(toDispose);
        }

        /// <summary>
        /// Disposes the given lazy object (if created already).
        /// </summary>
        /// <param name="objectToDispose">The object to be disposed.</param>
        internal static Lazy<T> DisposeObjectLazy<T>(Lazy<T> objectToDispose)
            where T : class, IDisposable
        {
            if (objectToDispose == null) { return null; }
            if (!objectToDispose.IsValueCreated) { return null; }

            DisposeObject(objectToDispose.Value);
            return null;
        }

        /// <summary>
        /// Disposes all objects within the given enumeration.
        /// </summary>
        /// <param name="enumeration">Enumeration containing all disposable objects.</param>
        internal static void DisposeObjects<T>(IEnumerable<T> enumeration)
            where T : class, IDisposable
        {
            if (enumeration == null) { throw new ArgumentNullException("enumeration"); }

            foreach (T actItem in enumeration)
            {
                DisposeObject(actItem);
            }
        }

        /// <summary>
        /// Gets the resource file for the given shader.
        /// </summary>
        /// <param name="device">The target graphics device.</param>
        /// <param name="subdirectory">The subdirectory where the shader is located.</param>
        /// <param name="shaderNameWithoutExt">The name of the shader without extension.</param>
        /// <param name="isMinimalistic">Was minimalistic shader chosen?</param>
        /// <returns>A tuple containing namespace and filename.</returns>
        internal static Tuple<string, string> GetShaderResourcePath(
            EngineDevice device,
            string subdirectory, string shaderNameWithoutExt,
            string shaderModel)
        {
            // Build raw shader resource name
            StringBuilder resultBuilderNamespace = new StringBuilder(100);
            resultBuilderNamespace.Append("FrozenSky.Multimedia.Resources.Shaders.");
            resultBuilderNamespace.Append(subdirectory);

            // Append shader name itself
            StringBuilder resultBuilderFileName = new StringBuilder(100);
            resultBuilderFileName.Append(shaderNameWithoutExt);

            // Append shader model name
            resultBuilderFileName.Append('.');
            resultBuilderFileName.Append(shaderModel);

            // Load debug shaders if debugging is enabled
            if (GraphicsCore.Current.IsDebugEnabled)
            {
                resultBuilderFileName.Append(".Debug");
            }

            // Append extension
            resultBuilderFileName.Append(".cso");

            return Tuple.Create(resultBuilderNamespace.ToString(), resultBuilderFileName.ToString());
        }

        /// <summary>
        /// Gets the resource link to the given shader.
        /// </summary>
        /// <param name="subdirectory">The subdirectory where the shader is located.</param>
        /// <param name="shaderNameWithoutExt">The name of the shader without extension.</param>
        /// <param name="isMinimalistic">Was minimalistic shader chosen?</param>
        internal static AssemblyResourceLink GetShaderResourceLink(EngineDevice device, string subdirectory, string shaderNameWithoutExt, string shaderModel)
        {
            var shaderResourcePath = GetShaderResourcePath(device, subdirectory, shaderNameWithoutExt, shaderModel);

            return new AssemblyResourceLink(
                typeof(GraphicsHelper).GetTypeInfo().Assembly,
                shaderResourcePath.Item1,
                shaderResourcePath.Item2);
        }

        /// <summary>
        /// Gets a vertex shader resource pointing to given shader file.
        /// </summary>
        /// <param name="subdirectory">The subdirectory where the shader is located.</param>
        /// <param name="shaderNameWithoutExt">The name of the shader without extension.</param>
        internal static VertexShaderResource GetVertexShaderResource(EngineDevice device, string subdirectory, string shaderNameWithoutExt)
        {
            string shaderModel = device.DefaultVertexShaderModel;

            AssemblyResourceLink resourceLink = GetShaderResourceLink(device, subdirectory, shaderNameWithoutExt, shaderModel);

            return new VertexShaderResource(shaderModel, resourceLink);
        }

        /// <summary>
        /// Gets a pixel shader resource pointing to given shader file.
        /// </summary>
        /// <param name="subdirectory">The subdirectory where the shader is located.</param>
        /// <param name="shaderNameWithoutExt">The name of the shader without extension.</param>
        internal static PixelShaderResource GetPixelShaderResource(EngineDevice device, string subdirectory, string shaderNameWithoutExt)
        {
            string shaderModel = device.DefaultPixelShaderModel;

            AssemblyResourceLink resourceLink = GetShaderResourceLink(device, subdirectory, shaderNameWithoutExt, shaderModel);

            return new PixelShaderResource(shaderModel, resourceLink);
        }

        /// <summary>
        /// Creates a default exture sampler state.
        /// </summary>
        /// <param name="device">The device to create the state for.</param>
        /// <param name="samplerQualityLevel">The target sampler quality</param>
        internal static D3D11.SamplerState CreateDefaultTextureSampler(EngineDevice device, TextureSamplerQualityLevel samplerQualityLevel)
        {
            // Set state parameters
            var samplerDesk = D3D11.SamplerStateDescription.Default();
            switch (device.DriverLevel)
            {
                case HardwareDriverLevel.Direct3D11:
                case HardwareDriverLevel.Direct3D10:
                    switch (samplerQualityLevel)
                    {
                        case TextureSamplerQualityLevel.High:
                            if (!device.IsHighDetailSupported) { goto case TextureSamplerQualityLevel.Low; }
                            samplerDesk.AddressU = D3D11.TextureAddressMode.Wrap;
                            samplerDesk.AddressV = D3D11.TextureAddressMode.Wrap;
                            samplerDesk.Filter = D3D11.Filter.Anisotropic;
                            samplerDesk.MaximumAnisotropy = 16;
                            break;

                        case TextureSamplerQualityLevel.Medium:
                            if (!device.IsHighDetailSupported) { goto case TextureSamplerQualityLevel.Low; }
                            samplerDesk.AddressU = D3D11.TextureAddressMode.Wrap;
                            samplerDesk.AddressV = D3D11.TextureAddressMode.Wrap;
                            samplerDesk.Filter = D3D11.Filter.Anisotropic;
                            samplerDesk.MaximumAnisotropy = 8;
                            break;

                        case TextureSamplerQualityLevel.Low:
                            samplerDesk.AddressU = D3D11.TextureAddressMode.Wrap;
                            samplerDesk.AddressV = D3D11.TextureAddressMode.Wrap;
                            samplerDesk.Filter = D3D11.Filter.MinMagMipLinear;
                            break;
                    }
                    break;

                default:
                    samplerDesk.AddressU = D3D11.TextureAddressMode.Wrap;
                    samplerDesk.AddressV = D3D11.TextureAddressMode.Wrap;
                    samplerDesk.Filter = D3D11.Filter.MinMagMipLinear;
                    break;
            }

            // Create the state object finally
            return new D3D11.SamplerState(device.DeviceD3D11, samplerDesk);
        }

        /// <summary>
        /// Creates a render target texture with the given width and height.
        /// </summary>
        /// <param name="device">Graphics device.</param>
        /// <param name="width">Width of generated texture.</param>
        /// <param name="height">Height of generated texture.</param>
        internal static D3D11.Texture2D CreateRenderTargetTextureDummy(D3D11.Device device, int width, int height)
        {
            D3D11.Texture2DDescription textureDescription = new D3D11.Texture2DDescription();

            textureDescription.Width = width;
            textureDescription.Height = height;
            textureDescription.MipLevels = 1;
            textureDescription.ArraySize = 1;
            textureDescription.Format = DEFAULT_TEXTURE_FORMAT;
            textureDescription.Usage = D3D11.ResourceUsage.Default;
            textureDescription.SampleDescription = new DXGI.SampleDescription(1, 0);
            textureDescription.BindFlags = D3D11.BindFlags.ShaderResource | D3D11.BindFlags.RenderTarget;
            textureDescription.CpuAccessFlags = D3D11.CpuAccessFlags.None;
            textureDescription.OptionFlags = D3D11.ResourceOptionFlags.None;

            return new D3D11.Texture2D(device, textureDescription);
        }
    }
}
