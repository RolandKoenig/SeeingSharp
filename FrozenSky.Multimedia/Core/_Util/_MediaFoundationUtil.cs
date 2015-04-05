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

namespace FrozenSky.Multimedia.Core
{
    ///// <summary>
    ///// All container formats supported by media foundation.
    /////  .. see mfapi.h (Guids with name MFTranscodeContainerType*).
    ///// </summary>
    //internal static class MFContainerTypes
    //{
    //    public static readonly Guid CONTAINER_ASF = new Guid(0x430f6f6e, (short)0xb6bf, (short)0x4fc1, 0xa0, 0xbd, 0x9e, 0xe4, 0x6e, 0xee, 0x2a, 0xfb);
    //    public static readonly Guid CONTAINER_MPEG4 = new Guid(0x430f6f6e, (short)0xb6bf, (short)0x4fc1, 0xa0, 0xbd, 0x9e, 0xe4, 0x6e, 0xee, 0x2a, 0xfb);
    //    public static readonly Guid CONTAINER_MP3 = new Guid(0x430f6f6e, (short)0xb6bf, (short)0x4fc1, 0xa0, 0xbd, 0x9e, 0xe4, 0x6e, 0xee, 0x2a, 0xfb);
    //    public static readonly Guid CONTAINER_3GP = new Guid(0x430f6f6e, (short)0xb6bf, (short)0x4fc1, 0xa0, 0xbd, 0x9e, 0xe4, 0x6e, 0xee, 0x2a, 0xfb);
    //    public static readonly Guid CONTAINER_AC3 = new Guid(0x430f6f6e, (short)0xb6bf, (short)0x4fc1, 0xa0, 0xbd, 0x9e, 0xe4, 0x6e, 0xee, 0x2a, 0xfb);
    //    public static readonly Guid CONTAINER_ADTS = new Guid(0x430f6f6e, (short)0xb6bf, (short)0x4fc1, 0xa0, 0xbd, 0x9e, 0xe4, 0x6e, 0xee, 0x2a, 0xfb);
    //    public static readonly Guid CONTAINER_MPEG2 = new Guid(0x430f6f6e, (short)0xb6bf, (short)0x4fc1, 0xa0, 0xbd, 0x9e, 0xe4, 0x6e, 0xee, 0x2a, 0xfb);
    //    public static readonly Guid CONTAINER_WAVE = new Guid(0x430f6f6e, (short)0xb6bf, (short)0x4fc1, 0xa0, 0xbd, 0x9e, 0xe4, 0x6e, 0xee, 0x2a, 0xfb);
    //    public static readonly Guid CONTAINER_AVI = new Guid(0x430f6f6e, (short)0xb6bf, (short)0x4fc1, 0xa0, 0xbd, 0x9e, 0xe4, 0x6e, 0xee, 0x2a, 0xfb);
    //    public static readonly Guid CONTAINER_FMPEG4 = new Guid(0x430f6f6e, (short)0xb6bf, (short)0x4fc1, 0xa0, 0xbd, 0x9e, 0xe4, 0x6e, 0xee, 0x2a, 0xfb);
    //}

    /// <summary>
    /// Raw video formats as described in mfapi.h
    /// </summary>
    internal static class MFRawFormats
    {
        public const int D3DFMT_R8G8B8 = 20;
        public const int D3DFMT_A8R8G8B8 = 21;
        public const int D3DFMT_X8R8G8B8 = 22;
        public const int D3DFMT_R5G6B5 = 23;
        public const int D3DFMT_X1R5G5B5 = 24;
        public const int D3DFMT_P8 = 41;
    }

    /// <summary>
    /// All video formats supported by media foundation (excluding Direct3D formats).
    ///  ..see mfapi.h (Guids with name MFVideoFormat_*).
    /// </summary>
    internal static class MFVideoFormats
    {
        public static readonly Guid FORMAT_RBG32 = MFHelper.BuildVideoSubtypeGuid(MFRawFormats.D3DFMT_X8R8G8B8);

        public static readonly Guid FORMAT_AI44 = MFHelper.BuildVideoSubtypeGuid("AI44");
        public static readonly Guid FORMAT_AYUV = MFHelper.BuildVideoSubtypeGuid("AYUV");
        public static readonly Guid FORMAT_YUY2 = MFHelper.BuildVideoSubtypeGuid("YUY2");
        public static readonly Guid FORMAT_YVYU = MFHelper.BuildVideoSubtypeGuid("YVYU");
        public static readonly Guid FORMAT_YVU9 = MFHelper.BuildVideoSubtypeGuid("YVU9");
        public static readonly Guid FORMAT_UYVY = MFHelper.BuildVideoSubtypeGuid("UYVY");
        public static readonly Guid FORMAT_NV11 = MFHelper.BuildVideoSubtypeGuid("NV11");
        public static readonly Guid FORMAT_NV12 = MFHelper.BuildVideoSubtypeGuid("NV12");
        public static readonly Guid FORMAT_YV12 = MFHelper.BuildVideoSubtypeGuid("YV12");
        public static readonly Guid FORMAT_I420 = MFHelper.BuildVideoSubtypeGuid("I420");
        public static readonly Guid FORMAT_IYUV = MFHelper.BuildVideoSubtypeGuid("IYUV");
        public static readonly Guid FORMAT_Y210 = MFHelper.BuildVideoSubtypeGuid("Y210");
        public static readonly Guid FORMAT_Y216 = MFHelper.BuildVideoSubtypeGuid("Y216");
        public static readonly Guid FORMAT_Y410 = MFHelper.BuildVideoSubtypeGuid("Y410");
        public static readonly Guid FORMAT_Y416 = MFHelper.BuildVideoSubtypeGuid("Y416");
        public static readonly Guid FORMAT_Y41P = MFHelper.BuildVideoSubtypeGuid("Y41P");
        public static readonly Guid FORMAT_Y41T = MFHelper.BuildVideoSubtypeGuid("Y41T");
        public static readonly Guid FORMAT_Y42T = MFHelper.BuildVideoSubtypeGuid("Y42T");
        public static readonly Guid FORMAT_P210 = MFHelper.BuildVideoSubtypeGuid("P210");
        public static readonly Guid FORMAT_P216 = MFHelper.BuildVideoSubtypeGuid("P216");
        public static readonly Guid FORMAT_P010 = MFHelper.BuildVideoSubtypeGuid("P010");
        public static readonly Guid FORMAT_P016 = MFHelper.BuildVideoSubtypeGuid("P016");
        public static readonly Guid FORMAT_v210 = MFHelper.BuildVideoSubtypeGuid("v210");
        public static readonly Guid FORMAT_v216 = MFHelper.BuildVideoSubtypeGuid("v216");
        public static readonly Guid FORMAT_v410 = MFHelper.BuildVideoSubtypeGuid("v410");
        public static readonly Guid FORMAT_MP43 = MFHelper.BuildVideoSubtypeGuid("MP43");
        public static readonly Guid FORMAT_MP4S = MFHelper.BuildVideoSubtypeGuid("MP4S");
        public static readonly Guid FORMAT_M4S2 = MFHelper.BuildVideoSubtypeGuid("M4S2");
        public static readonly Guid FORMAT_MP4V = MFHelper.BuildVideoSubtypeGuid("MP4V");
        public static readonly Guid FORMAT_WMV1 = MFHelper.BuildVideoSubtypeGuid("WMV1");
        public static readonly Guid FORMAT_WMV2 = MFHelper.BuildVideoSubtypeGuid("WMV2");
        public static readonly Guid FORMAT_WMV3 = MFHelper.BuildVideoSubtypeGuid("WMV3");
        public static readonly Guid FORMAT_WVC1 = MFHelper.BuildVideoSubtypeGuid("WVC1");
        public static readonly Guid FORMAT_MSS1 = MFHelper.BuildVideoSubtypeGuid("MSS1");
        public static readonly Guid FORMAT_MSS2 = MFHelper.BuildVideoSubtypeGuid("MSS2");
        public static readonly Guid FORMAT_MPG1 = MFHelper.BuildVideoSubtypeGuid("MPG1");
        public static readonly Guid FORMAT_dvsl = MFHelper.BuildVideoSubtypeGuid("dvsl");
        public static readonly Guid FORMAT_dvsd = MFHelper.BuildVideoSubtypeGuid("dvsd");
        public static readonly Guid FORMAT_dvhd = MFHelper.BuildVideoSubtypeGuid("dvhd");
        public static readonly Guid FORMAT_dv25 = MFHelper.BuildVideoSubtypeGuid("dv25");
        public static readonly Guid FORMAT_dv50 = MFHelper.BuildVideoSubtypeGuid("dv50");
        public static readonly Guid FORMAT_dvh1 = MFHelper.BuildVideoSubtypeGuid("dvh1");
        public static readonly Guid FORMAT_dvc = MFHelper.BuildVideoSubtypeGuid("dvc");
        public static readonly Guid FORMAT_H264 = MFHelper.BuildVideoSubtypeGuid("H264");
        public static readonly Guid FORMAT_MJPG = MFHelper.BuildVideoSubtypeGuid("MJPG");
        public static readonly Guid FORMAT_420O = MFHelper.BuildVideoSubtypeGuid("420O");
        public static readonly Guid FORMAT_HEVC = MFHelper.BuildVideoSubtypeGuid("HEVC");
        public static readonly Guid FORMAT_HEVS = MFHelper.BuildVideoSubtypeGuid("HEVS");
    }
}
