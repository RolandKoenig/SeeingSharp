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
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Namespace mappings
using D2D = SharpDX.Direct2D1;
using WIC = SharpDX.WIC;

namespace SeeingSharp.Multimedia.Drawing2D
{
    /// <summary>
    /// This object represents a inmemory chached bitmap which is 
    /// loaded from a ResourceLink (e. g. a file).
    /// </summary>
    public class StandardBitmapResource : BitmapResource
    {
        #region Bitmap resource and properties
        private Task<MemoryMappedTexture32bpp> m_loadResourceTask;
        private D2D.Bitmap[] m_loadedBitmaps;
        private MemoryMappedTexture32bpp m_cachedBitmap;
        private int m_pixelWidth;
        private int m_pixelHeight;
        private double m_dpiX;
        private double m_dpyY;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardBitmapResource"/> class.
        /// </summary>
        /// <param name="resource">The resource from w.</param>
        public StandardBitmapResource(ResourceLink resource)
        {
            m_loadedBitmaps = new D2D.Bitmap[GraphicsCore.Current.DeviceCount];

            // Set default values
            m_pixelWidth = 0;
            m_pixelHeight = 0;
            m_dpiX = 96.0;
            m_dpyY = 96.0;
            m_cachedBitmap = null;

            // Load the resource in a loading task
            m_loadResourceTask = AsyncResourceLoader.Current.EnqueueResourceLoadingTaskAsync(() =>
            {
                using (Stream inputStream = resource.OpenInputStream())
                using (WicBitmapSourceInternal bitmapSourceWrapper = GraphicsHelper.LoadBitmapSource_D2D(inputStream))
                {
                    WIC.BitmapSource bitmapSource = bitmapSourceWrapper.Converter;

                    m_pixelWidth = bitmapSource.Size.Width;
                    m_pixelHeight = bitmapSource.Size.Height;

                    m_cachedBitmap = new MemoryMappedTexture32bpp(new Size2(
                        m_pixelWidth, m_pixelHeight));
                    bitmapSource.CopyPixels(
                        m_cachedBitmap.Pitch,
                        new SharpDX.DataPointer(m_cachedBitmap.Pointer, (int)m_cachedBitmap.SizeInBytes));

                    bitmapSource.GetResolution(out m_dpiX, out m_dpyY);

                    return m_cachedBitmap;
                }
            });
        }

        public override string ToString()
        {
            return string.Format("Bitmap ({0}x{1} pixels)", m_pixelWidth, m_pixelHeight);
        }

        /// <summary>
        /// Gets the bitmap.
        /// </summary>
        /// <param name="engineDevice">The engine device.</param>
        internal override D2D.Bitmap GetBitmap(EngineDevice engineDevice)
        {
            if (m_cachedBitmap == null) { throw new ObjectDisposedException("StandardBitmapResource"); }
            if (base.IsDisposed) { throw new ObjectDisposedException(this.GetType().Name); }

            // Wait for caching task if it is still running
            if(m_loadResourceTask != null)
            {
                m_loadResourceTask.Wait();
                m_loadResourceTask = null;
            }

            D2D.Bitmap result = m_loadedBitmaps[engineDevice.DeviceIndex];
            if (result == null)
            {
                // Load the cached bitmap into the Direct2D resource
                result = new D2D.Bitmap(
                    engineDevice.FakeRenderTarget2D,
                    m_cachedBitmap.PixelSize.ToDXSize2(),
                    new SharpDX.DataPointer(m_cachedBitmap.Pointer, (int)m_cachedBitmap.SizeInBytes),
                    m_cachedBitmap.Pitch,
                    new D2D.BitmapProperties(new D2D.PixelFormat(
                        SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                        D2D.AlphaMode.Premultiplied)));

                // Register loaded bitmap
                m_loadedBitmaps[engineDevice.DeviceIndex] = result;
            }
            

            return result;
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            // Wait for caching task if it is still running
            if (m_loadResourceTask != null)
            {
                m_loadResourceTask.Wait();
                m_loadResourceTask = null;
            }

            // Dispose all created resources
            GraphicsHelper.SafeDispose(ref m_cachedBitmap);
        }

        /// <summary>
        /// Unloads all resources loaded on the given device.
        /// </summary>
        /// <param name="engineDevice">The device for which to unload the resource.</param>
        internal override void UnloadResources(EngineDevice engineDevice)
        {
            D2D.Bitmap brush = m_loadedBitmaps[engineDevice.DeviceIndex];
            if (brush != null)
            {
                GraphicsHelper.DisposeObject(brush);
                m_loadedBitmaps[engineDevice.DeviceIndex] = null;
            }
        }

        /// <summary>
        /// Gets the width of the bitmap in pixel´.
        /// </summary>
        public override int PixelWidth
        {
            get { return m_pixelWidth; }
        }

        /// <summary>
        /// Gets the height of the bitmap in pixel.
        /// </summary>
        public override int PixelHeight
        {
            get { return m_pixelHeight; }
        }
    }
}
