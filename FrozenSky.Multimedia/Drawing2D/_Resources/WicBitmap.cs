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
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Some namespace mappings
using WIC = SharpDX.WIC;

namespace FrozenSky.Multimedia.Drawing2D
{
    public class WicBitmap : IDisposable
    {
        private WIC.Bitmap m_wicBitmap;

        /// <summary>
        /// Initializes a new instance of the <see cref="WicBitmap"/> class.
        /// </summary>
        /// <param name="wicBitmap">The unmanaged bitmap data object.</param>
        private WicBitmap(WIC.Bitmap wicBitmap)
        {
            m_wicBitmap = wicBitmap;
        }

        public static async Task<WicBitmap> FromWicBitmapSourceAsync(WicBitmapSource bitmapSource, int width, int height)
        {
            WIC.Bitmap wicBitmap = null;
            await Task.Factory.StartNew(() =>
            {
                wicBitmap = new WIC.Bitmap(
                    GraphicsCore.Current.FactoryWIC,
                    bitmapSource.BitmapSource,
                    new SharpDX.Rectangle(0, 0, width, height));
            });
            return new WicBitmap(wicBitmap);
        }

        /// <summary>
        /// Creates a WIC bitmap from the given source.
        /// </summary>
        /// <param name="resourceSource">The source of the resource.</param>
        public static async Task<WicBitmap> FromResourceSourceAsync(ResourceSource resourceSource)
        {
            WIC.Bitmap wicBitmap = null;
            using (Stream inStream = await resourceSource.OpenInputStreamAsync())
            using (WIC.BitmapSource bitmapSource = await CommonTools.CallAsync(() => GraphicsHelper.LoadBitmapSource(inStream)))
            {
                wicBitmap = new WIC.Bitmap(
                    GraphicsCore.Current.FactoryWIC, bitmapSource, WIC.BitmapCreateCacheOption.CacheOnLoad);
            }

            return new WicBitmap(wicBitmap);
        }

        /// <summary>
        /// Gets an array containing all pixels of the underlying bitmap in Bgra format.
        /// </summary>
        public byte[] GetPixelsAsBgra()
        {
            if (m_wicBitmap == null) { throw new ObjectDisposedException("WicBitmap"); }

            byte[] result = new byte[m_wicBitmap.Size.Width * m_wicBitmap.Size.Height * 4];
            m_wicBitmap.CopyPixels(result, m_wicBitmap.Size.Width * 4);
            return result;
        }

        /// <summary>
        /// Führt anwendungsspezifische Aufgaben aus, die mit dem Freigeben, Zurückgeben oder Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
        /// </summary>
        public void Dispose()
        {
            GraphicsHelper.SafeDispose(ref m_wicBitmap);
        }

        public int Width
        {
            get 
            {
                if (m_wicBitmap == null) { throw new ObjectDisposedException("WicBitmap"); }
                return m_wicBitmap.Size.Width; 
            }
        }

        public int Height
        {
            get 
            {
                if (m_wicBitmap == null) { throw new ObjectDisposedException("WicBitmap"); }
                return m_wicBitmap.Size.Height; 
            }
        }
    }
}
