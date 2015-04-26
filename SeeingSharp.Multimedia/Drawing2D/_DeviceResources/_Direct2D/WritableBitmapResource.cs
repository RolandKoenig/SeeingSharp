#region License information (SeeingSharp and all based games/applications)
/*
    SeeingSharp and all games/applications based on it (more info at http://www.rolandk.de/wp)
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Some namespace mappings
using DXGI = SharpDX.DXGI;
using D2D = SharpDX.Direct2D1;

namespace SeeingSharp.Multimedia.Drawing2D
{
    public class WriteableBitmapResource : BitmapResource
    {
        private D2D.Bitmap[] m_loadedBitmaps;
        private Size2 m_bitmapSize;
        private D2D.PixelFormat m_pixelFormat;

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteableBitmapResource"/> class.
        /// </summary>
        public WriteableBitmapResource(Size2 bitmapSize, BitmapFormat format, AlphaMode alphaMode)
        {
            m_loadedBitmaps = new D2D.Bitmap[GraphicsCore.Current.DeviceCount];
            m_bitmapSize = bitmapSize;
            m_pixelFormat = new D2D.PixelFormat(
                (DXGI.Format)format,
                (D2D.AlphaMode)alphaMode);
        }

        /// <summary>
        /// Sets the bitmap's contents.
        /// </summary>
        /// <param name="graphics">The graphics object.</param>
        /// <param name="pointer">The pointer to the source data.</param>
        /// <param name="pitch"></param>
        public void SetBitmapContent(Graphics2D graphics, IntPtr pointer, int pitch)
        {
            D2D.Bitmap bitmap = this.GetBitmap(graphics.Device);

            bitmap.CopyFromMemory(pointer, pitch);
        }

        /// <summary>
        /// Gets the bitmap for the given device..
        /// </summary>
        /// <param name="engineDevice">The engine device.</param>
        internal override D2D.Bitmap GetBitmap(EngineDevice engineDevice)
        {
            // Check for disposed state
            if (base.IsDisposed) { throw new ObjectDisposedException(this.GetType().Name); }

            D2D.Bitmap result = m_loadedBitmaps[engineDevice.DeviceIndex];
            if (result == null)
            {
                // Load the bitmap initially
                result = new D2D.Bitmap(
                    engineDevice.FakeRenderTarget2D,
                    m_bitmapSize.ToDXSize2(), 
                    new D2D.BitmapProperties(m_pixelFormat));
                m_loadedBitmaps[engineDevice.DeviceIndex] = result;
            }

            return result;
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
    }
}
