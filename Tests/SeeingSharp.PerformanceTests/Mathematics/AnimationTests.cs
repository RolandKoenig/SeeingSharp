using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Util;

namespace SeeingSharp.PerformanceTests.Mathematics
{
    public static class AnimationTests
    {
        private const int COUNT_TRIES = 1000;

        public static void PerformPerformanceTest()
        {
            Console.WriteLine("#################### Continous");
            Console.WriteLine("2_Secs:            " + CheckAnimation_2Secs_Continous().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("2_Anims_31_Secs:   " + CheckAnimation_2Anims_31Secs_Continous().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine();

            Console.WriteLine("#################### EventDriven");
            Console.WriteLine("2_Secs:            " + CheckAnimation_2Secs_EventDriven().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("2_Anims_31_Secs:   " + CheckAnimation_2Anims_31Secs_EventDriven().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine();
        }

        private static TimeSpan CheckAnimation_2Anims_31Secs_EventDriven()
        {
            // Prepare animations
            GenericObject[] preparedObjects = new GenericObject[COUNT_TRIES];
            for (int loop = 0; loop < COUNT_TRIES; loop++)
            {
                preparedObjects[loop] = DefineAnimation_2Anims_31Secs();
            }

            // Measure animations
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    preparedObjects[loop].AnimationHandler.CalculateEventDriven();
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan CheckAnimation_2Secs_EventDriven()
        {
            // Prepare animations
            GenericObject[] preparedObjects = new GenericObject[COUNT_TRIES];
            for (int loop = 0; loop < COUNT_TRIES; loop++)
            {
                preparedObjects[loop] = DefineAnimation_2Secs();
            }

            // Measure animations
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    preparedObjects[loop].AnimationHandler.CalculateEventDriven();
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan CheckAnimation_2Anims_31Secs_Continous()
        {
            // Prepare animations
            GenericObject[] preparedObjects = new GenericObject[COUNT_TRIES];
            for (int loop = 0; loop < COUNT_TRIES; loop++)
            {
                preparedObjects[loop] = DefineAnimation_2Anims_31Secs();
            }
            TimeSpan continousUpdateTime = TimeSpan.FromMilliseconds(50.0);

            // Measure animations
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    preparedObjects[loop].AnimationHandler.CalculateContinuous(continousUpdateTime);
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan CheckAnimation_2Secs_Continous()
        {
            // Prepare animations
            GenericObject[] preparedObjects = new GenericObject[COUNT_TRIES];
            for (int loop = 0; loop < COUNT_TRIES; loop++)
            {
                preparedObjects[loop] = DefineAnimation_2Secs();
            }
            TimeSpan continousUpdateTime = TimeSpan.FromMilliseconds(50.0);

            // Measure animations
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    preparedObjects[loop].AnimationHandler.CalculateContinuous(continousUpdateTime);
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static GenericObject DefineAnimation_2Anims_31Secs()
        {
            // Define animation parameters / objects
            GenericObject genObject = new GenericObject(NamedOrGenericKey.Empty);

            // Define animation sequence
            //  Same as above, just with a wait-finished event here (dynamic wait step)
            genObject.BuildAnimationSequence()
                .Move3DTo(new Vector3(6f, 0f, 0f), new MovementSpeed(0.3f))                      // 20,0 Secs
                .Scale3DTo(new Vector3(2f, 2f, 2f), TimeSpan.FromSeconds(2.0))                   // 02,0 Secs
                .WaitFinished()
                .Move3DTo(new Vector3(9f, 0f, 0f), new MovementSpeed(0.3f))                      // 10,0 Secs
                .RotateEulerAnglesBy(new Vector3(2f, 0f, 0f), TimeSpan.FromSeconds(12.0))        // 12,0 Secs
                .WaitFinished()
                .Apply();
            genObject.BuildAnimationSequence()
                .Move3DTo(new Vector3(10f, 0f, 0f), new MovementSpeed(0.4f))                     // 25,0 Secs
                .Scale3DTo(new Vector3(2f, 2f, 2f), TimeSpan.FromSeconds(30.0))                  // 30,0 Secs
                .WaitFinished()
                .Move3DTo(new Vector3(11f, 0f, 0f), new MovementSpeed(0.4f))                     //  2,5 Secs
                .RotateEulerAnglesBy(new Vector3(2f, 0f, 0f), TimeSpan.FromSeconds(1.0))         //  1,0 Secs
                .WaitFinished()
                .ApplyAsSecondary();

            return genObject;
        }

        private static GenericObject DefineAnimation_2Secs()
        {
            // Define animation parameters / objects
            GenericObject genObject = new GenericObject(NamedOrGenericKey.Empty);

            // Define animation sequence (very simple)
            genObject.BuildAnimationSequence()
                .Move3DTo(new Vector3(0.6f, 0f, 0f), new MovementSpeed(0.3f))                      // 20,0 Secs
                .Apply();

            return genObject;
        }
    }
}