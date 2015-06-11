using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RKVideoMemory.Data;
using RKVideoMemory.Util;
using SeeingSharp;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Views;
using SeeingSharp.Util;

namespace RKVideoMemory.Game
{
    public class BackgroundMusicLogic : SceneLogicalObject
    {
        private MediaPlayerComponent m_mediaPlayer;
        private LevelData m_currentLevel;
        private int m_lastMusicIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundMusicLogic"/> class.
        /// </summary>
        public BackgroundMusicLogic(LevelData currentLevel)
        {
            m_currentLevel = currentLevel;
            m_lastMusicIndex = -1;
        }

        private string GetNextMusicFile()
        {
            List<string> musicFilePaths = m_currentLevel.MusicFilePaths.ToList();
            if (musicFilePaths.Count == 0) { return string.Empty; }
            else if (musicFilePaths.Count == 1) { return musicFilePaths[0]; }

            int nextMusicIndex = ThreadSafeRandom.Next(0, musicFilePaths.Count);
            while (nextMusicIndex != m_lastMusicIndex)
            {
                return musicFilePaths[nextMusicIndex];
            }

            return musicFilePaths[0];
        }

        /// <summary>
        /// Called when the user enters the main memory screen.
        /// </summary>
        private async void OnMessage_Received(MainMemoryScreenEnteredMessage message)
        {
            if (m_mediaPlayer == null)
            {
                string musicFile = GetNextMusicFile();
                if (string.IsNullOrEmpty(musicFile)) { return; }

                m_mediaPlayer = new MediaPlayerComponent();
                m_mediaPlayer.VideoClosed += OnMediaPlayer_VideoClosed;

                await m_mediaPlayer.OpenAndShowVideoFileAsync(musicFile);
                m_mediaPlayer.RestartWhenFinished = false;
            }

            // Fade in the video
            m_mediaPlayer.AudioVolume = 0f;
            while (m_mediaPlayer.AudioVolume < 0.3f)
            {
                await Task.Delay(50);
                m_mediaPlayer.AudioVolume = EngineMath.Clamp(m_mediaPlayer.AudioVolume + 0.02f, 0f, 1f);
            }
        }

        /// <summary>
        /// Called when the user found a correct memory pair.
        /// </summary>
        private async void OnMessage_Received(CardPairUncoveredByPlayerMessage message)
        {
            // Fade out the video
            while (m_mediaPlayer.AudioVolume > 0f)
            {
                await Task.Delay(50);
                m_mediaPlayer.AudioVolume = EngineMath.Clamp(m_mediaPlayer.AudioVolume - 0.02f, 0f, 1f);
            }
        }

        /// <summary>
        /// Called when we have to unload all referenced resources.
        /// </summary>
        public override void UnloadResources()
        {
            base.UnloadResources();

            // Clear reference to the media player
            MediaPlayerComponent mediaPlayer = m_mediaPlayer;
            m_mediaPlayer = null;

            mediaPlayer.AudioVolume = 0f;
            if (mediaPlayer.State != MediaPlayerState.NothingToDo)
            {
                mediaPlayer.CloseVideoAsync()
                    .ContinueWith((givenTask) =>
                    {
                        m_mediaPlayer = null;
                    });
            }
        }

        private async void OnMediaPlayer_VideoClosed(object sender, EventArgs e)
        {
            string musicFile = GetNextMusicFile();
            if (string.IsNullOrEmpty(musicFile)) { return; }

            await m_mediaPlayer.OpenAndShowVideoFileAsync(musicFile);
        }
    }
}