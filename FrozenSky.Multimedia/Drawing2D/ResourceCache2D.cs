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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Some namespace mappings
using D2D = SharpDX.Direct2D1;
using DWrite = SharpDX.DirectWrite;

namespace FrozenSky.Multimedia.Drawing2D
{
    public class ResourceCache2D 
    {
        private D2D.Factory m_factory2D;
        private DWrite.Factory m_factoryDWrite;
        private D2D.RenderTarget m_renderTarget;

        private Dictionary<Color4, D2D.SolidColorBrush> m_brushesSolidColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceCache2D"/> class.
        /// </summary>
        /// <param name="factory2D">The factory2 d.</param>
        /// <param name="factoryDWrite">The factory d write.</param>
        internal ResourceCache2D(D2D.Factory factory2D, DWrite.Factory factoryDWrite, D2D.RenderTarget renderTarget)
        {
            m_factory2D = factory2D;
            m_factoryDWrite = factoryDWrite;
            m_renderTarget = renderTarget;

            m_brushesSolidColor = new Dictionary<Color4, D2D.SolidColorBrush>();
        }

        /// <summary>
        /// Gets a solid color brush.
        /// </summary>
        /// <param name="color">The color for which to get the brush.</param>
        internal D2D.SolidColorBrush GetSolidBrush(Color4 color)
        {
            if (m_brushesSolidColor.ContainsKey(color)) { return m_brushesSolidColor[color]; }
            else
            {
                D2D.SolidColorBrush result = new D2D.SolidColorBrush(m_renderTarget, color.ToDXColor());
                m_brushesSolidColor[color] = result;
                return result;
            }
        }

    }
}
