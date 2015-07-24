using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.PerformanceTests.Mathematics
{
    /// <summary>
    /// This class contains testmethods for performance comparison of SeeingSharp.Vector3 and System.Numerics.Vector3.
    /// </summary>
    public static class VectorFunctionTests
    {
        #region SeeingSharp
        public static TimeSpan Check_SeeingSharp_Vector_Multiplication()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                Vector3 dummyVector = new Vector3(2f, 3f, 4f);
                Vector3 dummyVector2 = new Vector3(1f, 2f, 3f);

                Vector3 resultVector = Vector3.Zero;
                for(int loop=0; loop<100000; loop++)
                {
                    resultVector = dummyVector * dummyVector2;
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static TimeSpan Check_SeeingSharp_Vector_Add()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                Vector3 dummyVector = new Vector3(2f, 3f, 4f);
                Vector3 dummyVector2 = new Vector3(1f, 2f, 3f);

                Vector3 resultVector = Vector3.Zero;
                for (int loop = 0; loop < 100000; loop++)
                {
                    resultVector = dummyVector + dummyVector2;
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static TimeSpan Check_SeeingSharp_Vector_Subtract()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                Vector3 dummyVector = new Vector3(2f, 3f, 4f);
                Vector3 dummyVector2 = new Vector3(1f, 2f, 3f);

                Vector3 resultVector = Vector3.Zero;
                for (int loop = 0; loop < 100000; loop++)
                {
                    resultVector = dummyVector - dummyVector2;
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static TimeSpan Check_SeeingSharp_Vector_Transform()
        {
            Matrix4x4 transformMatrix = Matrix4x4.RotationYawPitchRoll(1f, 1f, 1f);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                Vector3 dummyVector = new Vector3(2f, 3f, 4f);
                Vector3 dummyVector2 = new Vector3(1f, 2f, 3f);

                Vector3 resultVector = Vector3.Zero;
                for (int loop = 0; loop < 100000; loop++)
                {
                    resultVector = Vector3.Transform(dummyVector, transformMatrix).XYZ;
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
        #endregion
        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        #region System.Numerics
        public static TimeSpan Check_SystemNumerics_Vector_Multiplication()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                System.Numerics.Vector3 dummyVector = new System.Numerics.Vector3(2f, 3f, 4f);
                System.Numerics.Vector3 dummyVector2 = new System.Numerics.Vector3(1f, 2f, 3f);

                System.Numerics.Vector3 resultVector = System.Numerics.Vector3.Zero;
                for (int loop = 0; loop < 100000; loop++)
                {
                    resultVector = dummyVector * dummyVector2;
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static TimeSpan Check_SystemNumerics_Vector_Add()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                System.Numerics.Vector3 dummyVector = new System.Numerics.Vector3(2f, 3f, 4f);
                System.Numerics.Vector3 dummyVector2 = new System.Numerics.Vector3(1f, 2f, 3f);

                System.Numerics.Vector3 resultVector = System.Numerics.Vector3.Zero;
                for (int loop = 0; loop < 100000; loop++)
                {
                    resultVector = dummyVector + dummyVector2;
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static TimeSpan Check_SystemNumerics_Vector_Subtract()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                System.Numerics.Vector3 dummyVector = new System.Numerics.Vector3(2f, 3f, 4f);
                System.Numerics.Vector3 dummyVector2 = new System.Numerics.Vector3(1f, 2f, 3f);

                System.Numerics.Vector3 resultVector = System.Numerics.Vector3.Zero;
                for (int loop = 0; loop < 100000; loop++)
                {
                    resultVector = dummyVector - dummyVector2;
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static TimeSpan Check_SystemNumerics_Vector_Transform()
        {
            Matrix4x4 matrix = Matrix4x4.RotationYawPitchRoll(1f, 1f, 1f);
            System.Numerics.Matrix4x4 transformMatrix = new System.Numerics.Matrix4x4(
                matrix.M11, matrix.M12, matrix.M13, matrix.M14,
                matrix.M21, matrix.M22, matrix.M23, matrix.M24,
                matrix.M31, matrix.M32, matrix.M33, matrix.M34,
                matrix.M41, matrix.M42, matrix.M43, matrix.M44);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                System.Numerics.Vector3 dummyVector = new System.Numerics.Vector3(2f, 3f, 4f);
                System.Numerics.Vector3 dummyVector2 = new System.Numerics.Vector3(1f, 2f, 3f);

                System.Numerics.Vector3 resultVector = System.Numerics.Vector3.Zero;
                for (int loop = 0; loop < 100000; loop++)
                {
                    resultVector = System.Numerics.Vector3.Transform(dummyVector, transformMatrix);
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
        #endregion
    }
}
