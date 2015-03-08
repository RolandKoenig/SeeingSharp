using FrozenSky.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Tests
{
    [TestClass]
    public class ThreadingTests
    {
        public const string TEST_CATEGORY = "FrozenSky Threading ObjectThread";

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
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
            Assert.IsNull(occurredException);
            Assert.IsTrue(tickDurations.Count > 4);
            Assert.IsTrue(tickDurations.Count < 10);
            Assert.IsTrue(tickDurations
                .Count((actInt) => actInt > 450 && actInt < 550) > 4);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
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
            Assert.IsNull(occurredException);
            Assert.IsTrue(tickDurations.Count > 10);
            Assert.IsTrue(tickDurations.Count < 20);
            Assert.IsTrue(tickDurations.Count((actInt) => actInt > 80) > 8);
            Assert.IsTrue(tickDurations.Count((actInt) => actInt > 150) == 0);
        }
    }
}
