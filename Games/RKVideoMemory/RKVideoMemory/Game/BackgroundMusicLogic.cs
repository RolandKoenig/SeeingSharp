using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RKVideoMemory.Data;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Views;

namespace RKVideoMemory.Game
{
    public class BackgroundMusicLogic : SceneLogicalObject
    {
        private MediaPlayerComponent m_mediaPlayer;
        private LevelData m_currentLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundMusicLogic"/> class.
        /// </summary>
        public BackgroundMusicLogic(LevelData currentLevel)
        {
            m_mediaPlayer = new MediaPlayerComponent();
            m_currentLevel = currentLevel;
        }

        private async void OnMessage_Received(MainMemoryScreenEnteredMessage message)
        {
            await m_mediaPlayer.OpenAndShowVideoFileAsync(
                m_currentLevel.MusicFilePaths.First());
        }

        private void OnMessage_Received(CardPairUncoveredByPlayerMessage message)
        {
        }
    }
}