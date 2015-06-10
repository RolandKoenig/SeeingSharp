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

#if DESKTOP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Checking;
using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing2D;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Util;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.Samples.Base.Direct2D.Direct2DRemderDxfFile),
    contractType: typeof(SeeingSharp.Samples.Base.SampleBase))]
namespace SeeingSharp.Samples.Base.Direct2D
{
    [SampleInfo(
        Constants.SAMPLEGROUP_DIRECT2D, "Render Dxf File",
        Constants.SAMPLE_DIRECT2D_SCREEN_OVERLAY_ORDER,
        "https://github.com/RolandKoenig/SeeingSharp/blob/master/Samples/SeeingSharp.Samples.Base/_Samples/BasicSamples/RotatingPalletSample.cs")]
    public class Direct2DRemderDxfFile : SampleBase
    {
        private SolidBrushResource m_solidBrush;
        private TextFormatResource m_textFormat;
        private SolidBrushResource m_textBrush;

        /// <summary>
        /// Called when the sample has to startup.
        /// </summary>
        /// <param name="targetRenderLoop">The target render loop.</param>
        public override async Task OnStartupAsync(RenderLoop targetRenderLoop)
        {
            targetRenderLoop.EnsureNotNull("targetRenderLoop");

            // Build dummy scene
            Scene scene = targetRenderLoop.Scene;

            // Define 2D overlay
            m_solidBrush = new SolidBrushResource(Color4.LightSteelBlue.ChangeAlphaTo(0.7f));
            m_textFormat = new TextFormatResource("Arial", 36);
            m_textBrush = new SolidBrushResource(Color4.RedColor);
            Action<Graphics2D> draw2DAction = (graphics) =>
            {
                // 2D rendering is made here
                RectangleF d2dRectangle = new RectangleF(
                    10, 10,
                    graphics.ScreenSize.Width - 20,
                    graphics.ScreenSize.Height - 20);
                if (d2dRectangle.Width < 100) { return; }
                if (d2dRectangle.Height < 100) { return; }

                // Draw background rectangle
                graphics.FillRoundedRectangle(
                    d2dRectangle,
                    30, 30,
                    m_solidBrush);

                // Draw the text
                d2dRectangle.Inflate(-10, -10);
                d2dRectangle.Y = d2dRectangle.Y + (d2dRectangle.Height - 100);
                graphics.DrawText("Hello Direct2D!", m_textFormat, d2dRectangle, m_textBrush);
            };

            await targetRenderLoop.Scene.ManipulateSceneAsync((manipulator) =>
            {
                // Add the 2D layer to the scene
                manipulator.AddDrawingLayer(draw2DAction);
            });
        }

        public override void OnClosed()
        {
            base.OnClosed();

            CommonTools.SafeDispose(ref m_solidBrush);
            CommonTools.SafeDispose(ref m_textBrush);
            CommonTools.SafeDispose(ref m_textFormat);
        }
    }
}
#endif