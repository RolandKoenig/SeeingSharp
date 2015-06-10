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

namespace RKVideoMemory
{
    internal static class Constants
    {
        public const string GAME_SCENE_NAME = "VideoMemoryScene";

        #region folder names
        public const string DEFAULT_FOLDER_INITIAL_LEVEL = "MemoryData";
        public const string FOLDERNAME_MUSIC = "Music";
        public const string FOLDERPREFIX_SCREEN = "Screen";
        public const string FOLDERNAME_ENDING = "Ending";
        #endregion folder names

        #region file patterns
        public const string FILE_PATTERN_BACKGROUND = "Background.*";
        public const string FILE_PATTERN_TILE1 = "Tile1.*";
        public const string FILE_PATTERN_TILE2 = "Tile2.*";
        public const string FILENAME_TITLE_IMAGE = "_Title";
        public const string FILENAME_TILEMAP = "Tilemap.txt";
        public const string FILENAME_APPICON = "AppIcon.ico";
        public const string FILENAME_LEVELSETTINGS = "LevelSettings.xml";
        #endregion file patterns

        #region file extensions
        public static readonly string[] SUPPORTED_IMAGE_FORMATS = new string[] { ".jpg", ".jpeg", ".png", ".bmp" };
        public static readonly string[] SUPPORTED_VIDEO_FORMATS = new string[] { ".mp4", ".wmv" };
        public static readonly string[] SUPPORTED_MUSIC_FORMATS = new string[] { ".mp3", ".wav" };
        #endregion file extensions

        #region graphics layer settings
        public const string GFX_LAYER_BACKGROUND = "Background";
        public const int GFX_LAYER_BACKGROUND_ORDERID = -100;
        public const string GFX_LAYER_VIDEO_FOREGROUND = "VideoForeground";
        public const int GFX_LAYER_VIDEO_FOREGROUND_ORDERID = 100;
        #endregion graphics layer settings

        #region timing parameters
        public const int INITIAL_UNCOVER_SECONDS_MAX = 4;
        public const int INITIAL_UNCOVER_SECONDS_MIN = 3;
        public const int ROTATE_ANIM_DELAY_MILLIS_MAX = 600;
        public const int ROTATE_ANIM_DELAY_MILLIS_MIN = 200;
        public const int FADE_INOUT_ANIM_TIME = 400;
        #endregion timing parameters

        #region parameters for the tilemap
        public const int TILEMAP_DEFAULT_X_COUNT = 5;
        public const int TILEMAP_DEFAULT_Y_COUNT = 5;
        public const float TILE_WIDTH = 1.9f;
        public const float TILE_HEIGHT = 1.4f;
        public const float TILE_DISTANCE_X = 2.2f;
        public const float TILE_DISTANCE_Y = 1.6f;
        public const char TILE_CHAR_UNALLOWED = '_';
        public const char TILE_CHAR_ALLOWED = 'X';
        #endregion parameters for the tilemap
    }
}