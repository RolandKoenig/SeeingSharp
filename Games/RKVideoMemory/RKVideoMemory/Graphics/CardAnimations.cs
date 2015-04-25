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
using FrozenSky;
using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
using RKVideoMemory.Game;
using RKVideoMemory.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKVideoMemory.Graphics
{
    internal static class CardAnimations
    {
        public static IAnimationSequenceBuilder<Card> MainScreen_WhenUncovered(
            this IAnimationSequenceBuilder<Card> sequenceBuilder)
        {
            return sequenceBuilder
                .CallAction(() => sequenceBuilder.TargetObject.RotationEuler = new FrozenSky.Vector3(0f, 0f, EngineMath.RAD_180DEG))
                .WaitFinished()
                .Delay(TimeSpan.FromSeconds(Constants.INITIAL_UNCOVER_SECONDS_MAX))
                .WaitFinished()
                .Delay(TimeSpan.FromMilliseconds(ThreadSafeRandom.Next(
                    Constants.ROTATE_ANIM_DELAY_MILLIS_MIN,
                    Constants.ROTATE_ANIM_DELAY_MILLIS_MAX)))
                .WaitFinished()
                .RotateEulerAnglesTo(
                    new Vector3(0f, 0f, 0f),
                    TimeSpan.FromMilliseconds(300))
                .WaitFinished();
        }

        public static IAnimationSequenceBuilder<Card> MainScreen_WhenCovered(
            this IAnimationSequenceBuilder<Card> sequenceBuilder)
        {
            return sequenceBuilder
                .CallAction(() => sequenceBuilder.TargetObject.RotationEuler = new FrozenSky.Vector3(0f, 0f, EngineMath.RAD_180DEG))
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
    }
}
