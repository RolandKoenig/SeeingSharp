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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Samples.Base
{
    /// <summary>
    /// Some constants parameters for the sample collection
    /// located in this assembly.
    /// </summary>
    internal static class Constants
    {
        #region Group parameters
        public const string SAMPLEGROUP_BASIC = "Basics";
        public const string SAMPLEGROUP_MF = "Media Foundation";
        public const string SAMPLEGROUP_DIRECT2D = "Direct2D";
        #endregion

        #region parameters for Basic samples
        public const int SAMPLE_BASICS_ROTATINGPALLET_ORDER = 1;
        public const int SAMPLE_BASICS_TEXTUREDPALLET_ORDER = 2;
        public const int SAMPLE_BASICS_SKYBOX_ORDER = 3;
        public const int SAMPLE_BASCIS_SINGLEMODEL_ORDER = 4;
        public const int SAMPLE_BASICS_ASYNCANIM_ORDER = 5;
        public const int SAMPLE_BASICS_TRANSPARENTPALLETS_ORDER = 6;
        public const int SAMPLE_BASICS_PALLETS_ORDER = 7;
        public const int SAMPLE_BASICS_PALLETS_ANIMATED_ORDER = 8;
        public const int SAMPLE_BASICS_PARENT_CHILD_ORDER = 9;
        #endregion

        #region parameters for Media Foundation samples
        public const int SAMPLE_MF_VIDEOTEXTURE_ORDER = 200;
        public const int SAMPLE_MF_CAMERACAPTURE_ORDER = 201;
        #endregion

        #region parameters for Direct2D samples
        public const int SAMPLE_DIRECT2D_D2DTEXTURE_ORDER = 101;
        public const int SAMPLE_DIRECT2D_D2DANIMTEXTURE_ORDER = 102;
        public const int SAMPLE_DIRECT2D_SCREEN_OVERLAY_ORDER = 103;
        public const int SAMPLE_DIRECT2D_SIMPLE_EFFECT = 104;
        #endregion

    }
}
