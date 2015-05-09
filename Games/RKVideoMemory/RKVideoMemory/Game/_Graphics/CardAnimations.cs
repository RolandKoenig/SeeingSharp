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
#endregion

using SeeingSharp;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using RKVideoMemory.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKVideoMemory.Game
{
    internal static class CardAnimations
    {
        public static IAnimationSequenceBuilder<Card> MainScreen_PerformCover(
            this IAnimationSequenceBuilder<Card> sequenceBuilder)
        {
            return sequenceBuilder
                .RotateEulerAnglesTo(
                    new Vector3(0f, 0f, EngineMath.RAD_180DEG),
                    TimeSpan.FromMilliseconds(300));
        }

        public static IAnimationSequenceBuilder<Card> MainScreen_PerformUncover(
            this IAnimationSequenceBuilder<Card> sequenceBuilder)
        {
            return sequenceBuilder
                .RotateEulerAnglesTo(
                    new Vector3(0f, 0f, 0f),
                    TimeSpan.FromMilliseconds(300));
        }

        public static IAnimationSequenceBuilder<Card> MainScreenLeave(
            this IAnimationSequenceBuilder<Card> sequenceBuilder)
        {
            return sequenceBuilder
                .ResetCard_BeforeLeave()
                .ChangeOpacityTo(0f, TimeSpan.FromMilliseconds(300));
        }

        public static IAnimationSequenceBuilder<Card> MainScreenStart_WhenUncovered(
            this IAnimationSequenceBuilder<Card> sequenceBuilder)
        {
            return sequenceBuilder
                .ResetCard_BeforeStart()
                .ChangeOpacityTo(1f, TimeSpan.FromMilliseconds(300))
                .WaitFinished()
                .Delay(TimeSpan.FromMilliseconds(ThreadSafeRandom.Next(
                    Constants.ROTATE_ANIM_DELAY_MILLIS_MIN,
                    Constants.ROTATE_ANIM_DELAY_MILLIS_MAX)))
                .WaitFinished()
                .RotateEulerAnglesTo(
                    new Vector3(0f, 0f, 0f),
                    TimeSpan.FromMilliseconds(300))
                .WaitFinished()
                .Delay(TimeSpan.FromSeconds(ThreadSafeRandom.Next(
                    Constants.INITIAL_UNCOVER_SECONDS_MIN,
                    Constants.INITIAL_UNCOVER_SECONDS_MAX)))
                .WaitFinished()
                .Scale3DTo(new Vector3(0.8f, 0.8f, 0.8f), TimeSpan.FromMilliseconds(300))
                .ChangeOpacityTo(0.5f, TimeSpan.FromMilliseconds(300))
                .ChangeAccentuationFactorTo(0.5f, TimeSpan.FromMilliseconds(300));
        }

        public static IAnimationSequenceBuilder<Card> MainScreenStart_WhenCovered(
            this IAnimationSequenceBuilder<Card> sequenceBuilder)
        {
            return sequenceBuilder
                .ResetCard_BeforeStart()
                .ChangeOpacityTo(1f, TimeSpan.FromMilliseconds(300))
                .WaitFinished()
                .Delay(TimeSpan.FromMilliseconds(ThreadSafeRandom.Next(
                    Constants.ROTATE_ANIM_DELAY_MILLIS_MIN, 
                    Constants.ROTATE_ANIM_DELAY_MILLIS_MAX)))
                .WaitFinished()
                .RotateEulerAnglesTo(
                    new Vector3(0f, 0f, 0f),
                    TimeSpan.FromMilliseconds(300))
                .WaitFinished()
                .Delay(TimeSpan.FromSeconds(ThreadSafeRandom.Next(
                    Constants.INITIAL_UNCOVER_SECONDS_MIN, 
                    Constants.INITIAL_UNCOVER_SECONDS_MAX)))
                .WaitFinished()
                .RotateEulerAnglesTo(
                    new Vector3(0f, 0f, EngineMath.RAD_180DEG),
                    TimeSpan.FromMilliseconds(300))
                .WaitFinished();
        }

        public static IAnimationSequenceBuilder<Card> ResetCard_BeforeStart(
            this IAnimationSequenceBuilder<Card> sequenceBuilder)
        {
            return sequenceBuilder.CallAction(() =>
            {
                sequenceBuilder.TargetObject.RotationEuler = new SeeingSharp.Vector3(0f, 0f, EngineMath.RAD_180DEG);
                sequenceBuilder.TargetObject.Opacity = 0f;
                sequenceBuilder.TargetObject.AccentuationFactor = 1f;
                sequenceBuilder.TargetObject.Scaling = Vector3.One;
            });
        }

        public static IAnimationSequenceBuilder<Card> ResetCard_BeforeLeave(
            this IAnimationSequenceBuilder<Card> sequenceBuilder)
        {
            return sequenceBuilder.CallAction(() =>
                {
                    sequenceBuilder.TargetObject.Opacity = 1f;
                    sequenceBuilder.TargetObject.Scaling = Vector3.One;
                });
        }
    }
}
