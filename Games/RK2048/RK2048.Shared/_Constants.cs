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
using System.Collections.Generic;
using System.Text;
using FrozenSky.Util;
using FrozenSky;

namespace RK2048
{
    internal static class Constants
    {
        // Some miscellaneous constants
        internal static readonly double TILE_ANIMATION_TIME_MS = 300.0;
        internal static readonly TimeSpan TILE_ANIMATION_TIME = TimeSpan.FromMilliseconds(TILE_ANIMATION_TIME_MS);
        internal static readonly TimeSpan TILE_ANIMATION_REMOVE_TIME = TimeSpan.FromMilliseconds(TILE_ANIMATION_TIME_MS);// * 1.5);
        internal static readonly int[] TILE_VALUE_BY_ID = new int[]
        {
            2,
            4,
            8,
            16,
            32,
            64,
            128,
            256,
            512,
            1024,
            2048
        };

        // Some measures
        internal static readonly float TILE_WIDTH = 2f;
        internal static readonly float TILE_PADDING = 0.2f;

        // Colors
        internal static readonly Color4 COLOR_TEXT = new Color4(0, 204, 255);
        internal static readonly Color4 COLOR_TILE_BASE = Color4.White;

        // Define resource keys for all tiles
        internal static readonly NamedOrGenericKey RES_GEO_TILE_2 = new NamedOrGenericKey("Resources/Tile_2");
        internal static readonly NamedOrGenericKey RES_GEO_TILE_4 = new NamedOrGenericKey("Resources/Tile_4");
        internal static readonly NamedOrGenericKey RES_GEO_TILE_8 = new NamedOrGenericKey("Resources/Tile_8");
        internal static readonly NamedOrGenericKey RES_GEO_TILE_16 = new NamedOrGenericKey("Resources/Tile_16");
        internal static readonly NamedOrGenericKey RES_GEO_TILE_32 = new NamedOrGenericKey("Resources/Tile_32");
        internal static readonly NamedOrGenericKey RES_GEO_TILE_64 = new NamedOrGenericKey("Resources/Tile_64");
        internal static readonly NamedOrGenericKey RES_GEO_TILE_128 = new NamedOrGenericKey("Resources/Tile_128");
        internal static readonly NamedOrGenericKey RES_GEO_TILE_256 = new NamedOrGenericKey("Resources/Tile_256");
        internal static readonly NamedOrGenericKey RES_GEO_TILE_512 = new NamedOrGenericKey("Resources/Tile_512");
        internal static readonly NamedOrGenericKey RES_GEO_TILE_1024 = new NamedOrGenericKey("Resources/Tile_1024");
        internal static readonly NamedOrGenericKey RES_GEO_TILE_2048 = new NamedOrGenericKey("Resources/Tile_2048");
        internal static readonly NamedOrGenericKey[] RES_GEO_TILES_BY_ID = new NamedOrGenericKey[]
        {
            RES_GEO_TILE_2,
            RES_GEO_TILE_4,
            RES_GEO_TILE_8,
            RES_GEO_TILE_16,
            RES_GEO_TILE_32,
            RES_GEO_TILE_64,
            RES_GEO_TILE_128, 
            RES_GEO_TILE_256, 
            RES_GEO_TILE_512, 
            RES_GEO_TILE_1024,
            RES_GEO_TILE_2048,
        };
    }
}
