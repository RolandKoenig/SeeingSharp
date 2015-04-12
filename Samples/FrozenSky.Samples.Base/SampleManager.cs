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
*/
#endregion

using FrozenSky.Infrastructure;
using FrozenSky.Multimedia.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Samples.Base
{
    public static class SampleManager
    {
        private const string SAMPLE_ROT_PALLET = "Rotating Pallet";
        private const string SAMPLE_SKYBOX = "Skybox";
        private const string SAMPLE_SINGLE_MODEL = "Single Model";
        private const string SAMPLE_ASYNC_ANIM = "Async Animation";
        private const string SAMPLE_TRANS_PALLETS = "Transparent Pallets";
        private const string SAMPLE_PALLETS = "Pallets";
        private const string SAMPLE_PALLETS_TEXTURED = "Pallets (Textured)";

        /// <summary>
        /// Initializes the <see cref="SampleManager"/> class.
        /// </summary>
        static SampleManager()
        {
            // Create and register viewmodel for performance analyziation
            PerformanceAnalysisViewModel performanceAnalyzisViewModel = new PerformanceAnalysisViewModel();
            FrozenSkyApplication.Current.RegisterSingleton(performanceAnalyzisViewModel);
        }

        /// <summary>
        /// Gets the Names of all implemented samples.
        /// </summary>
        public static IEnumerable<string> GetSampleNames()
        {
            yield return SAMPLE_ROT_PALLET;
            yield return SAMPLE_SKYBOX;
            yield return SAMPLE_SINGLE_MODEL;
            yield return SAMPLE_ASYNC_ANIM;
            yield return SAMPLE_TRANS_PALLETS;
            yield return SAMPLE_PALLETS;
            yield return SAMPLE_PALLETS_TEXTURED;
        }

        /// <summary>
        /// Applies the sample with the given name to the given RenderLoop.
        /// </summary>
        /// <param name="renderLoop">The render loop.</param>
        /// <param name="sampleName">Name of the sample.</param>
        public static void ApplySample(this RenderLoop renderLoop, string sampleName)
        {
            switch(sampleName)
            {
                case SAMPLE_ROT_PALLET: renderLoop.BuildRotatingPalletDemo(); break;
                case SAMPLE_SKYBOX: renderLoop.BuildSkyboxRenderingDemo(); break;
                case SAMPLE_SINGLE_MODEL: renderLoop.BuildSingleModelDemo(); break;
                case SAMPLE_ASYNC_ANIM: renderLoop.BuildAsyncAnimationDemo(); break;
                case SAMPLE_TRANS_PALLETS: renderLoop.BuildTransparentPalletDemo(); break;
                case SAMPLE_PALLETS: renderLoop.BuildPalletsDemo(); break;
                case SAMPLE_PALLETS_TEXTURED: renderLoop.BuildPalletsDemoTextured(); break;
            }
        }
    }
}
