#region License information (SeeingSharp and all based games/applications)
/*
    SeeingSharp and all games/applications based on it (more info at http://www.rolandk.de/wp)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SeeingSharp.Tests
{
    public class MathTests
    {
        public class MovementCalculationTests
        {
            public const string TEST_CATEGORY = "SeeingSharp Mathematics";

            [Fact]
            [Trait("Category", TEST_CATEGORY)]
            public void Movemenet_SpeedBased()
            {
                MovementSpeed palletConveyorSpeed = new MovementSpeed(0.3f);
                MovementSpeed trayConveyorSpeed = new MovementSpeed(0.8f);

                MovementAnimationHelper animHelper1 = new MovementAnimationHelper(palletConveyorSpeed, new Vector3(5f, 0f, 0f));
                MovementAnimationHelper animHelper2 = new MovementAnimationHelper(palletConveyorSpeed, new Vector3(3f, 4f, 2f));
                MovementAnimationHelper animHelper3 = new MovementAnimationHelper(trayConveyorSpeed, new Vector3(5f, 0f, 0f));

                Assert.True((int)animHelper1.MovementTime.TotalMilliseconds == 16667);
                Assert.True((int)animHelper2.MovementTime.TotalMilliseconds == 17951);
                Assert.True((int)animHelper3.MovementTime.TotalMilliseconds == 6250);
            }

            [Fact]
            [Trait("Category", TEST_CATEGORY)]
            public void Movement_SpeedBased_FixedTime()
            {
                Vector3 movementVector = new Vector3(10f, 0f, 8f);
                MovementSpeed movementSpeed = new MovementSpeed(movementVector, TimeSpan.FromSeconds(5.0));

                MovementAnimationHelper animHelper = new MovementAnimationHelper(movementSpeed, movementVector);

                Assert.True(animHelper.MovementTime == TimeSpan.FromSeconds(5.0));
                Assert.True(animHelper.AccelerationTime == TimeSpan.Zero);
                Assert.True(animHelper.DecelerationTime == TimeSpan.Zero);
            }

            [Fact]
            [Trait("Category", TEST_CATEGORY)]
            public void Movement_AccDecFull_ShortRun()
            {
                // Configuration
                MovementSpeed movementSpeed = new MovementSpeed(1.5f, 0.8f, -0.4f);
                Vector3 movementVector = new Vector3(0f, 0f, -0.14f);

                // Action
                MovementAnimationHelper animHelper = new MovementAnimationHelper(movementSpeed, movementVector);

                Vector3 pos = animHelper.GetPartialMoveDistance(animHelper.MovementTime);

                // Asserts
                Assert.True(animHelper.AccelerationTime > TimeSpan.Zero);
                Assert.True(animHelper.DecelerationTime > TimeSpan.Zero);
                Assert.True(animHelper.DecelerationTime > animHelper.AccelerationTime);
                Assert.True(animHelper.FullSpeedTime == TimeSpan.Zero);
                Assert.True(animHelper.MovementTime < TimeSpan.FromSeconds(15.0));
            }

            [Fact]
            [Trait("Category", TEST_CATEGORY)]
            public void Movement_AccDecFull_LongRun()
            {
                // Configuration
                MovementSpeed movementSpeed = new MovementSpeed(1.5f, 0.6f, -0.5f);
                Vector3 movementVector = new Vector3(10f, 0f, 8f);

                // Action
                MovementAnimationHelper animHelper = new MovementAnimationHelper(movementSpeed, movementVector);

                // Asserts
                Assert.True(animHelper.AccelerationTime > TimeSpan.Zero);
                Assert.True(animHelper.DecelerationTime > TimeSpan.Zero);
                Assert.True(animHelper.DecelerationTime > animHelper.AccelerationTime);
                Assert.True(animHelper.FullSpeedTime > animHelper.AccelerationTime);
                Assert.True(animHelper.FullSpeedTime > animHelper.DecelerationTime);
                Assert.True(animHelper.MovementTime < TimeSpan.FromSeconds(15.0));
            }

        }
    }
}
