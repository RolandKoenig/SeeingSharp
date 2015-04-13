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

namespace FrozenSky.Multimedia.DrawingVideo
{
    /// <summary>
    /// Copy of the enum from SharpDX.MediaFoundation.
    /// Needed because the implementation of SharpDX didn't have the Flags attribute set (Version 2.6.3).
    /// </summary>
    [Flags]
    internal enum MediaSourceCharacteristics_Internal
    {
        IsLive = 1,
        CanSeek = 2,
        CanPause = 4,
        HasSlowSeek = 8,
        HasMultiplePresentations = 16,
        CanSkipforward = 32,
        CanSkipbackward = 64,
    }
}
