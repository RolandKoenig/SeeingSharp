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
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky
{
    [Flags]
    public enum FrozenSkyPointerButton
    {
        // Zusammenfassung:
        //     Es wurde keine Maustaste gedrückt.
        None = 0,
        //
        // Zusammenfassung:
        //     Die linke Maustaste wurde gedrückt.
        Left = 1048576,
        //
        // Zusammenfassung:
        //     Die rechte Maustaste wurde gedrückt.
        Right = 2097152,
        //
        // Zusammenfassung:
        //     Die mittlere Maustaste wurde gedrückt.
        Middle = 4194304,
        //
        // Zusammenfassung:
        //     XButton eins wurde gedrückt.
        XButton1 = 8388608,
        //
        // Zusammenfassung:
        //     XButton zwei wurde gedrückt.
        XButton2 = 16777216,
    }
}
