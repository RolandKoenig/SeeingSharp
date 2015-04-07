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
    public class WicBitmapSource : IDisposable
    {
        private WIC.BitmapSource m_wicBitmapSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="WicBitmapSource"/> class.
        /// </summary>
        /// <param name="bitmapSource">The bitmap source.</param>
        private WicBitmapSource(WIC.BitmapSource bitmapSource)
        {
            m_wicBitmapSource = bitmapSource;
        }

        /// <summary>
        /// Creates a WIC BitmapSource object from the given source.
        /// </summary>
        /// <param name="resourceLink">The source of the resource.</param>
        public static async Task<WicBitmapSource> FromResourceSourceAsync(ResourceLink resourceLink)
        {
            WIC.BitmapSource wicBitmapSource = null;
            using (Stream inStream = await resourceLink.OpenInputStreamAsync())
            {
                wicBitmapSource = await CommonTools.CallAsync(() => GraphicsHelper.LoadBitmapSource(inStream));
            }

            return new WicBitmapSource(wicBitmapSource);
        }

        /// <summary>
        /// Führt anwendungsspezifische Aufgaben durch, die mit der Freigabe, der Zurückgabe oder dem Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
        /// </summary>
        public void Dispose()
        {
            GraphicsHelper.SafeDispose(ref m_wicBitmapSource);
        }

        public int Width
        {
            get 
            {
                if (m_wicBitmapSource == null) { throw new ObjectDisposedException("WicBitmapSource"); }
                return m_wicBitmapSource.Size.Width;
            }
        }

        public int Height
        {
            get
            {
                if (m_wicBitmapSource == null) { throw new ObjectDisposedException("WicBitmapSource"); }
                return m_wicBitmapSource.Size.Height;
            }
        }

        internal WIC.BitmapSource BitmapSource
        {
            get { return m_wicBitmapSource; }
        }
    }
}
