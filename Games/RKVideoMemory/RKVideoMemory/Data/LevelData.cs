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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Util;
using SeeingSharp.Multimedia.DrawingVideo;
using SeeingSharp.Multimedia.Core;

namespace RKVideoMemory.Data
{
    public class LevelData
    {
        private List<string> m_musicFilePaths;

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelData"/> class.
        /// </summary>
        /// <param name="directoryName">Name of the directory.</param>
        private LevelData(string directoryName)
        {
            m_musicFilePaths = new List<string>();

            // Search all main textures
            this.MainTextures = new MainTextureData(directoryName);

            // Handle tilemap file
            string tilemapPath = Path.Combine(directoryName, Constants.FILENAME_TILEMAP);
            if (File.Exists(tilemapPath))
            {
                this.Tilemap = TilemapData.FromFile(tilemapPath);
            }
            else
            {
                this.Tilemap = new TilemapData();
            }

            // Set reference to the main icon
            string appIconPath = Path.Combine(directoryName, Constants.FILENAME_APPICON);
            if (File.Exists(appIconPath))
            {
                this.AppIconPath = appIconPath;
            }

            // Load common level settings
            string levelSettingsPath = Path.Combine(directoryName, Constants.FILENAME_LEVELSETTINGS);
            if (File.Exists(levelSettingsPath))
            {
                this.LevelSettings = CommonTools.DeserializeFromXmlFile<LevelSettings>(levelSettingsPath);
            }
            else
            {
                this.LevelSettings = LevelSettings.Default;
            }

            // Load music folder
            string musicFolder = Path.Combine(directoryName, Constants.FOLDERNAME_MUSIC);
            if (Directory.Exists(musicFolder))
            {
                foreach (string actFileName in Directory.GetFiles(musicFolder))
                {
                    string actFileExtension = Path.GetExtension(actFileName);
                    if (!Constants.SUPPORTED_MUSIC_FORMATS.ContainsString(actFileExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    m_musicFilePaths.Add(actFileName);
                }
            }

            // Load ending folder
            string endingFolder = Path.Combine(directoryName, Constants.FOLDERNAME_ENDING);
            if (Directory.Exists(endingFolder))
            {
                foreach (string actFileName in Directory.GetFiles(endingFolder))
                {
                    if (Constants.SUPPORTED_VIDEO_FORMATS.Contains(Path.GetExtension(actFileName)))
                    {
                        this.EndingVideo = actFileName;

                        using (FrameByFrameVideoReader videoReader = new FrameByFrameVideoReader(actFileName))
                        {
                            // Read the first frame
                            this.EndingVideoFirstFrame = videoReader.ReadFrame();
                            this.EndingVideoFirstFrame.SetAllAlphaValuesToOne_ARGB();

                            // Read the last frame
                            videoReader.SetCurrentPosition(videoReader.Duration, false);

                            this.EndingVideoLastFrame = videoReader.ReadFrame();
                            this.EndingVideoLastFrame.SetAllAlphaValuesToOne_ARGB();
                        }

                        continue;
                    }

                    if (Constants.SUPPORTED_IMAGE_FORMATS.Contains(Path.GetExtension(actFileName)))
                    {
                        this.EndingImage = actFileName;
                        continue;
                    }

                    if (Constants.SUPPORTED_MUSIC_FORMATS.Contains(Path.GetExtension(actFileName)))
                    {
                        this.EndingMusic = actFileName;
                        continue;
                    }
                }
            }

            // Load all screens
            IEnumerable<string> screenDirectories =
                from actScreenDirectory in Directory.GetDirectories(directoryName)
                where Path.GetFileName(actScreenDirectory).StartsWith(Constants.FOLDERPREFIX_SCREEN, StringComparison.OrdinalIgnoreCase)
                orderby Path.GetFileName(actScreenDirectory)
                select actScreenDirectory;
            this.Screens = new List<ScreenData>();
            foreach (string actScreenDirectory in screenDirectories)
            {
                ScreenData actScreen = new ScreenData(actScreenDirectory);
                if (actScreen.MemoryPairs.Count > 0)
                {
                    this.Screens.Add(actScreen);
                }
            }
        }

        /// <summary>
        /// Loads the level from the given directory.
        /// </summary>
        /// <param name="directoryName">Name of the directory.</param>
        public static async Task<LevelData> FromDirectoryAsync(string directoryName)
        {
            LevelData result = null;
            await Task.Factory.StartNew(() => result = new LevelData(directoryName));
            return result;
        }

        public string AppIconPath
        {
            get;
            private set;
        }

        public string EndingImage
        {
            get;
            private set;
        }

        public string EndingVideo
        {
            get;
            private set;
        }

        public MemoryMappedTexture32bpp EndingVideoFirstFrame
        {
            get;
            private set;
        }

        public MemoryMappedTexture32bpp EndingVideoLastFrame
        {
            get;
            private set;
        }

        public string EndingMusic
        {
            get;
            private set;
        }

        public IEnumerable<string> MusicFilePaths
        {
            get { return m_musicFilePaths; }
        }

        public LevelSettings LevelSettings
        {
            get;
            private set;
        }

        public MainTextureData MainTextures
        {
            get;
            private set;
        }

        public TilemapData Tilemap
        {
            get;
            private set;
        }

        public List<ScreenData> Screens
        {
            get;
            private set;
        }
    }
}