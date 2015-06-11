using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RKVideoMemory.Data;
using SeeingSharp.Checking;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Util;
using SeeingSharp.Multimedia.Views;
using SeeingSharp;

namespace RKVideoMemory.Game
{
    public class EndGameLogic : SceneLogicalObject
    {
        private LevelData m_levelData;

        #region variables for ending image
        private MediaPlayerComponent m_mediaPlayer;
        #endregion

        public EndGameLogic(LevelData levelData)
        {
            levelData.EnsureNotNull("levelData");

            m_levelData = levelData;
        }

        public override void UnloadResources()
        {
            base.UnloadResources();

            // Close the media player if it was opened
            if(m_mediaPlayer != null)
            {
                m_mediaPlayer.CloseVideoAsync()
                    .FireAndForget();
            }
        }

        /// <summary>
        /// Shows the ending video.
        /// </summary>
        private async Task ShowEndingVideo()
        {
            await Task.Delay(100);
        }

        /// <summary>
        /// Shows the ending image if available.
        /// </summary>
        private async Task ShowEndingImage()
        {
            if (string.IsNullOrEmpty(m_levelData.EndingImage)) { return; }

            // Start media player for background music
            if (!string.IsNullOrEmpty(m_levelData.EndingMusic))
            {
                m_mediaPlayer = new MediaPlayerComponent();
                m_mediaPlayer.RestartWhenFinished = true;
                m_mediaPlayer.AudioVolume = 0f;
                m_mediaPlayer.OpenAndShowVideoFileAsync(m_levelData.EndingMusic)
                    .ContinueWith(async (givenTask) =>
                    {
                        while(m_mediaPlayer.AudioVolume < 1f)
                        {
                            m_mediaPlayer.AudioVolume = EngineMath.Clamp(
                                m_mediaPlayer.AudioVolume + 0.1f,
                                0f, 1f);
                            await Task.Delay(100);
                        }
                    })
                    .FireAndForget();
            }

            // Prepare main variables
            NamedOrGenericKey resEndingImage = NamedOrGenericKey.Empty;
            TexturePainter objVideoPainter = null;

            // Attach the video texture to the scene
            Task imageAnimationTask = null;
            await base.Scene.ManipulateSceneAsync((manipulator) =>
            {
                // Create the layer (if necessary)
                if (!manipulator.ContainsLayer(Constants.GFX_LAYER_VIDEO_FOREGROUND))
                {
                    SceneLayer bgLayer = manipulator.AddLayer(Constants.GFX_LAYER_VIDEO_FOREGROUND);
                    bgLayer.ClearDepthBufferBefreRendering = true;
                    manipulator.SetLayerOrderID(
                        bgLayer,
                        Constants.GFX_LAYER_VIDEO_FOREGROUND_ORDERID);
                }

                // Load the texture painter
                resEndingImage = manipulator.AddResource(
                    () => new StandardTextureResource(m_levelData.EndingImage));
                objVideoPainter = new TexturePainter(resEndingImage);
                objVideoPainter.Scaling = 0.6f;
                objVideoPainter.AccentuationFactor = 1f;
                objVideoPainter.Opacity = 0.0f;
                imageAnimationTask = objVideoPainter.BuildAnimationSequence()
                    .Delay(300)
                    .WaitFinished()
                    .ScaleTo(0.9f, TimeSpan.FromMilliseconds(Constants.FADE_INOUT_ANIM_TIME))
                    .ChangeOpacityTo(1f, TimeSpan.FromMilliseconds(Constants.FADE_INOUT_ANIM_TIME))
                    .WaitFinished()
                    .Delay(10000)
                    .WaitFinished()
                    .ScaleTo(1.5f, TimeSpan.FromMilliseconds(Constants.FADE_INOUT_ANIM_TIME))
                    .ChangeOpacityTo(0f, TimeSpan.FromMilliseconds(Constants.FADE_INOUT_ANIM_TIME))
                    .ApplyAsync();

                manipulator.Add(
                    objVideoPainter,
                    Constants.GFX_LAYER_VIDEO_FOREGROUND);
            });

            // Close media-playing
            if(m_mediaPlayer != null)
            {
                imageAnimationTask.ContinueWith(async (givenTask) =>
                    {
                        while (m_mediaPlayer.AudioVolume > 0f)
                        {
                            m_mediaPlayer.AudioVolume = EngineMath.Clamp(
                                m_mediaPlayer.AudioVolume - 0.1f,
                                0f, 1f);
                            await Task.Delay(100);
                        }
                    })
                    .FireAndForget();
            }

            await imageAnimationTask;
        }

        private async void OnMessage_Received(GameEndReachedMessage message)
        {
            await ShowEndingVideo();

            await ShowEndingImage();
        }
    }
}