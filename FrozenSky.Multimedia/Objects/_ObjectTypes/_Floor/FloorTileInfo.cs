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

using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Multimedia.Objects
{
    public class FloorTileInfo
    {
        private NamedOrGenericKey m_material;

        /// <summary>
        /// Initializes a new instance of the <see cref="FloorTileInfo"/> class.
        /// </summary>
        /// <param name="material">The material to use for the tile (string.Empty or null to use default material).</param>
        public FloorTileInfo(NamedOrGenericKey material)
        {
            m_material = material;
        }

        /// <summary>
        /// Gets the material used for this tile.
        /// </summary>
        public NamedOrGenericKey Material
        {
            get { return m_material; }
        }
    }
}
