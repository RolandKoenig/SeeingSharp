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
#endregion License information (SeeingSharp and all based games/applications)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.RKKinectLounge
{
    internal static class Constants
    {
        public static readonly Random UI_RANDOMIZER = new Random(Environment.TickCount);

        #region Image sizes
        public const int THUMBNAIL_WDITH = 700;
        public const int THUMBNAIL_HEIGHT = 500;
        #endregion

        #region Viewtype names
        public const string VIEW_TYPE_BROWSING = "Browsing";
        public const string VIEW_TYPE_KINECT_DASHBOARD = "KinectDashboard";
        public const string VIEW_TYPE_IMAGE_SLIDER = "ImageSlider";
        #endregion

        #region All constants for browsing logic
        public const string BROWSING_FOLDER_CONFIG_FILE = "_Configuration.xml";
        public const int BROWSING_DELAY_TIME_PER_FOLDER_LOAD_MS = 100;
        public const int MAX_SUBITEM_COUNT_FOR_BIG_TILES = 4;
        #endregion

        #region All constants for Kinect handling
        public const string KINECT_THREAD_NAME = "Kinect-Thread";
        public const int KINECT_BOOT_ORDERID = 0;
        public const int KINECT_PERF_HISTORICAL_VALUE_COUNT = 600; // Remember performance values of last 10 minutes
        public const int KINECT_PERF_VALUE_INTERVAL_MS = 1000;     // Calculate one value each x seconds
        public const int KINECT_PERF_CALC_INTERVAL_MS = 1000;      // Take values from last x seconds for calculation
        public const string KINECT_PERF_ANALYZER_NAME = "KinectPerformance";
        public const string KINECT_PERF_FLOWRATE_INFRARED_FRAME = "Kinect.Frames.Infrared";
        public const string KINECT_PERF_FLOWRATE_LONG_EXPOSURE_INFRARED_FRAME = "Kinect.Frames.LongExposureInfrared";
        public const string KINECT_PERF_FLOWRATE_COLOR_FRAME = "Kinect.Frames.Color";
        public const string KINECT_PERF_FLOWRATE_DEPTH_FRAME = "Kinect.Frames.Depth";
        public const string KINECT_PERF_FLOWRATE_BODY_FRAME = "Kinect.Frames.Body";
        public const string KINECT_PERF_FLOWRATE_BODYINDEX_FRAME = "Kinect.Frames.BodyIndex";
        public static readonly Color4[] KINECT_BODY_INDEX_COLORS = new Color4[]
        {
            Color4.LightBlue, Color4.LightGreen, Color4.LightCyan, Color4.LightGoldenrodYellow, Color4.LightYellow
        };
        #endregion

        #region All constants for multimedia handling
        public static readonly string[] SUPPORTED_IMAGE_FORMATS = new string[] { ".jpg", ".jpeg", ".png", ".bmp" };
        public static readonly string[] SUPPORTED_VIDEO_FORMATS = new string[] { ".mpg", ".mpeg", ".avi", ".wmv", ".mp4" };
        public const string BROWSING_SEARCH_PATTERN_THUMBNAIL = "Thumbnail*.*";
        public const string VIEW_MODEL_EXT_MULTIMEDIA = "Multimedia";
        #endregion
    }
}