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
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Linq;
using FrozenSky.Util;

#if UNIVERSAL
using Windows.Storage;
#endif

namespace RK2048.Data
{
    [XmlType]
    public class GameDataContainer
    {
        private const string GAME_DATA_FILE_NAME = "GameData.RK2048.xml";
        private const int GAME_SCORE_MAX = 20;

        private ObservableCollection<GameScore> m_gameScores;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameDataContainer" /> class.
        /// </summary>
        public GameDataContainer()
        {
            m_gameScores = new ObservableCollection<GameScore>();

            // Observe score collection
            m_gameScores.CollectionChanged += (sender, eArgs) =>
            {
                // Change score index
                int actScoreIndex = 1;
                foreach (var actItem in m_gameScores)
                {
                    actItem.ScoreIndex = actScoreIndex;
                    actScoreIndex++;
                }
            };
        }

        /// <summary>
        /// Posts a new score value.
        /// </summary>
        /// <param name="playerName">The name of the player.</param>
        /// <param name="totalScore">The value to be posted.</param>
        public GameScore TryPostNewScore(string playerName, int totalScore)
        {
            GameScore newScore = new GameScore(playerName, totalScore);

            // Try to insert the score on correct location
            bool added = false;
            for(int loop=0 ; loop<m_gameScores.Count; loop++)
            {
                if(m_gameScores[loop].TotalScore < totalScore)
                {
                    m_gameScores.Insert(loop, newScore);
                    added = true;
                    break;
                }
            }

            // Add the value to the end 
            if (!added && (m_gameScores.Count < GAME_SCORE_MAX)) 
            { 
                m_gameScores.Add(newScore);
                added = true;
            }

            // Cut out last entries if collection is too big
            while(m_gameScores.Count > GAME_SCORE_MAX)
            {
                m_gameScores.RemoveAt(GAME_SCORE_MAX);
            }

            return added ? newScore : null;
        }

        /// <summary>
        /// Saves this data container to the roaming folder.
        /// </summary>
        public async Task SaveToRoamingFolder()
        {
#if UNIVERSAL
            StorageFolder sourceFolder = ApplicationData.Current.RoamingFolder;

            //Save updated highscore to file again
            StorageFile highscoreFileOut = await sourceFolder.CreateFileAsync(GAME_DATA_FILE_NAME, CreationCollisionOption.ReplaceExisting);
            await CommonTools.SerializeToXmlFile(highscoreFileOut, this);
#endif

            await Task.Delay(10);
        }

        /// <summary>
        /// Loads the GameDataContainer from the roaming folder.
        /// </summary>
        public static async Task<GameDataContainer> LoadFromRoamingFolderAsync()
        {
#if UNIVERSAL
            StorageFolder sourceFolder = ApplicationData.Current.RoamingFolder;

            //Read the highscore file
            GameDataContainer loadedContainer = null;
            StorageFile highscoreFile = await sourceFolder.GetOrReturnNull("GameData.RK2048.xml");
            if (highscoreFile == null)
            {
                loadedContainer = new GameDataContainer();
            }
            else
            {
                try
                {
                    //Try to load current score from roaming file
                    GameDataContainer highScore = await CommonTools.DeserializeFromXmlFile<GameDataContainer>(highscoreFile);
                    if (highScore == null) { highScore = new GameDataContainer(); }
                    loadedContainer = highScore;
                }
                catch (Exception)
                {
                    //Any exception occurred while deserializing
                    loadedContainer = new GameDataContainer();
                }

            }

            return loadedContainer;
#endif
            await Task.Delay(10);

            return new GameDataContainer();
        }

        /// <summary>
        /// Gets a list containing all game scores.
        /// </summary>
        [XmlElement("GameScore")]
        public ObservableCollection<GameScore> GameScores
        {
            get { return m_gameScores; }
        }
    }
}