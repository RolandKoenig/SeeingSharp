﻿#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SeeingSharp.Tests
{
    public class ThreadingTests
    {
        public const string TEST_CATEGORY = "SeeingSharp Threading ObjectThread";

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task CheckObjectThread_Heartbeat()
        {
            List<int> tickDurations = new List<int>();

            ObjectThread objThread = new ObjectThread("TestThread_Heartbeat", 500);
            Exception occurredException = null;
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                objThread.Tick += (sender, eArgs) =>
                {
                    tickDurations.Add((int)stopwatch.Elapsed.TotalMilliseconds);

                    stopwatch.Reset();
                    stopwatch.Start();
                };
                objThread.Start();

                // Wait some thime since we have some heartbeats
                await Task.Delay(2500);
            }
            catch (Exception ex) { occurredException = ex; }

            // Wait for thread finish
            await objThread.StopAsync(1000);

            // Check results
            Assert.Null(occurredException);
            Assert.True(tickDurations.Count > 4);
            Assert.True(tickDurations.Count < 10);
            Assert.True(tickDurations
                .Count((actInt) => actInt > 450 && actInt < 550) > 4);
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task CheckObjectThread_Invokes()
        {
            List<int> tickDurations = new List<int>();

            ObjectThread objThread = new ObjectThread("TestThread_Invokes", 5000);
            Exception occurredException = null;
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                objThread.Tick += (sender, eArgs) =>
                {
                    tickDurations.Add((int)stopwatch.Elapsed.TotalMilliseconds);

                    stopwatch.Reset();
                    stopwatch.Start();
                };
                objThread.Start();

                // Wait some thime since we have some heartbeats
                for(int loop=0; loop<10; loop++)
                {
                    await Task.Delay(100);
                    objThread.Trigger();
                }
            }
            catch (Exception ex) { occurredException = ex; }

            // Wait for thread finish
            await objThread.StopAsync(1000);

            // Check results
            Assert.Null(occurredException);
            Assert.True(tickDurations.Count > 10);
            Assert.True(tickDurations.Count < 20);
            Assert.True(tickDurations.Count((actInt) => actInt > 80) > 8);
            Assert.True(tickDurations.Count((actInt) => actInt > 150) == 0);
        }
    }
}
