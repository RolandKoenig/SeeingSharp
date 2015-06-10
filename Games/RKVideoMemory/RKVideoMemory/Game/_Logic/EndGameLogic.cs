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

namespace RKVideoMemory.Game
{
    public class EndGameLogic : SceneLogicalObject
    {
        private LevelData m_levelData;

        public EndGameLogic(LevelData levelData)
        {
            levelData.EnsureNotNull("levelData");

            m_levelData = levelData;
        }

        private async Task ShowEndingVideo()
        {
            await Task.Delay(100);
        }

        private async Task ShowEndingImage()
        {
            if (string.IsNullOrEmpty(m_levelData.EndingImage)) { return; }

            // Prepare main variables
            NamedOrGenericKey resEndingImage = NamedOrGenericKey.Empty;
            TexturePainter objVideoPainter = null;

            // Attach the video texture to the scene
            Task startAnimationTask = null;
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
                startAnimationTask = objVideoPainter.BuildAnimationSequence()
                    .Delay(300)
                    .WaitFinished()
                    .ScaleTo(0.9f, TimeSpan.FromMilliseconds(Constants.FADE_INOUT_ANIM_TIME))
                    .ChangeOpacityTo(1f, TimeSpan.FromMilliseconds(Constants.FADE_INOUT_ANIM_TIME))
                    .ApplyAsync();

                manipulator.Add(
                    objVideoPainter,
                    Constants.GFX_LAYER_VIDEO_FOREGROUND);
            });

            await startAnimationTask;
        }

        private async void OnMessage_Received(GameEndReachedMessage message)
        {
            await ShowEndingVideo();

            await ShowEndingImage();
        }
    }
}