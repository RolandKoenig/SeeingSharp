using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.PerformanceTests.Mathematics
{
    /// <summary>
    /// This class contains testmethods for performance comparison of SeeingSharp.Vector3 and System.Numerics.Vector3.
    /// </summary>
    public static class VectorFunctionTests
    {
        private const int COUNT_TRIES = 100000;

        public static void PerformPerformanceTest()
        {
            Console.WriteLine("#################### Seeing# Vector (default way)");
            Console.WriteLine("CrossProduct:   " + Check_SeeingSharp_Std_Vector_CrossProduct().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("Add:            " + Check_SeeingSharp_Std_Vector_Add().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("Subtract:       " + Check_SeeingSharp_Std_Vector_Subtract().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("Transform:      " + Check_SeeingSharp_Std_Vector_Transform().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("DotProduct:     " + Check_SeeingSharp_Std_Vector_DotProduct().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("Normalize:      " + Check_SeeingSharp_Std_Vector_Normalize().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("MatrixMultiply: " + Check_SeeingSharp_Std_Vector_MatrixMultiply().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine();

            Console.WriteLine("#################### Seeing# Vector (using ref/out parameters)");
            Console.WriteLine("CrossProduct:   " + Check_SeeingSharp_RefOut_Vector_CrossProduct().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("Add:            " + Check_SeeingSharp_RefOut_Vector_Add().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("Subtract:       " + Check_SeeingSharp_RefOut_Vector_Subtract().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("Transform:      " + Check_SeeingSharp_RefOut_Vector_Transform().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("DotProduct:     " + Check_SeeingSharp_RefOut_Vector_DotProduct().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("Normalize:      " + Check_SeeingSharp_RefOut_Vector_Normalize().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("MatrixMultiply: " + Check_SeeingSharp_RefOut_Vector_MatrixMultiply().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine();

            Console.WriteLine("#################### System.Numerics (new SIMD instructions)");
            Console.WriteLine("CrossProduct:   " + Check_SystemNumerics_Vector_CrossProduct().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("Add:            " + Check_SystemNumerics_Vector_Add().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("Subtract:       " + Check_SystemNumerics_Vector_Subtract().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("Transform:      " + Check_SystemNumerics_Vector_Transform().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("DotProduct:     " + Check_SystemNumerics_Vector_DotProduct().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("Normalize:      " + Check_SystemNumerics_Vector_Normalize().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("MatrixMultiply: " + Check_SystemNumerics_Vector_MatrixMultiply().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine();
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************

        #region SeeingSharp Standard

        private static TimeSpan Check_SeeingSharp_Std_Vector_CrossProduct()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                Vector3 dummyVector = new Vector3(2f, 3f, 4f);
                Vector3 dummyVector2 = new Vector3(1f, 2f, 3f);

                Vector3 resultVector = Vector3.Zero;
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    resultVector = Vector3.Cross(dummyVector, dummyVector2);
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan Check_SeeingSharp_Std_Vector_Add()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                Vector3 dummyVector = new Vector3(2f, 3f, 4f);
                Vector3 dummyVector2 = new Vector3(1f, 2f, 3f);

                Vector3 resultVector = Vector3.Zero;
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    resultVector = dummyVector + dummyVector2;
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan Check_SeeingSharp_Std_Vector_Subtract()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                Vector3 dummyVector = new Vector3(2f, 3f, 4f);
                Vector3 dummyVector2 = new Vector3(1f, 2f, 3f);

                Vector3 resultVector = Vector3.Zero;
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    resultVector = dummyVector - dummyVector2;
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan Check_SeeingSharp_Std_Vector_Transform()
        {
            Matrix4x4 transformMatrix = Matrix4x4.RotationYawPitchRoll(1f, 1f, 1f);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                Vector3 dummyVector = new Vector3(2f, 3f, 4f);

                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    dummyVector.TransformCoordinate(transformMatrix);
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan Check_SeeingSharp_Std_Vector_DotProduct()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                Vector3 dummyVector = new Vector3(2f, 3f, 4f);
                Vector3 dummyVector2 = new Vector3(1f, 2f, 3f);

                float resultValue = 0f;
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    resultValue = Vector3.Dot(dummyVector, dummyVector2);
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan Check_SeeingSharp_Std_Vector_Normalize()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                Vector3 dummyVector = new Vector3(2f, 3f, 4f);

                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    dummyVector.Normalize();
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan Check_SeeingSharp_Std_Vector_MatrixMultiply()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                Matrix4x4 dummyMatrix = Matrix4x4.Translation(10f, 10f, 10f);
                Matrix4x4 rotationMatrix = Matrix4x4.RotationY(1f);

                Matrix4x4 resultMatrix = Matrix4x4.Identity;
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    resultMatrix = dummyMatrix * rotationMatrix;
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        #endregion SeeingSharp Standard

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************

        #region SeeingSharp

        private static TimeSpan Check_SeeingSharp_RefOut_Vector_CrossProduct()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                Vector3 dummyVector = new Vector3(2f, 3f, 4f);
                Vector3 dummyVector2 = new Vector3(1f, 2f, 3f);

                Vector3 resultVector = Vector3.Zero;
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    Vector3.Cross(ref dummyVector, ref dummyVector2, out resultVector);
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan Check_SeeingSharp_RefOut_Vector_Add()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                Vector3 dummyVector = new Vector3(2f, 3f, 4f);
                Vector3 dummyVector2 = new Vector3(1f, 2f, 3f);

                Vector3 resultVector = Vector3.Zero;
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    Vector3.Add(ref dummyVector, ref dummyVector2, out resultVector);
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan Check_SeeingSharp_RefOut_Vector_Subtract()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                Vector3 dummyVector = new Vector3(2f, 3f, 4f);
                Vector3 dummyVector2 = new Vector3(1f, 2f, 3f);

                Vector3 resultVector = Vector3.Zero;
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    Vector3.Subtract(ref dummyVector, ref dummyVector2, out resultVector);
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan Check_SeeingSharp_RefOut_Vector_Transform()
        {
            Matrix4x4 transformMatrix = Matrix4x4.RotationYawPitchRoll(1f, 1f, 1f);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                Vector3 dummyVector = new Vector3(2f, 3f, 4f);
                Vector3 dummyVector2 = new Vector3(1f, 2f, 3f);

                Vector4 resultVector = Vector4.Zero;
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    Vector3.Transform(ref dummyVector, ref transformMatrix, out resultVector);
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan Check_SeeingSharp_RefOut_Vector_DotProduct()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                Vector3 dummyVector = new Vector3(2f, 3f, 4f);
                Vector3 dummyVector2 = new Vector3(1f, 2f, 3f);

                float resultValue = 0f;
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    Vector3.Dot(ref dummyVector, ref dummyVector2, out resultValue);
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan Check_SeeingSharp_RefOut_Vector_Normalize()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                Vector3 dummyVector = new Vector3(2f, 3f, 4f);

                Vector3 resultNormal = Vector3.Zero;
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    Vector3.Normalize(ref dummyVector, out resultNormal);
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan Check_SeeingSharp_RefOut_Vector_MatrixMultiply()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                Matrix4x4 dummyMatrix = Matrix4x4.Translation(10f, 10f, 10f);
                Matrix4x4 rotationMatrix = Matrix4x4.RotationY(1f);

                Matrix4x4 resultMatrix = Matrix4x4.Identity;
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    Matrix4x4.Multiply(ref dummyMatrix, ref rotationMatrix, out resultMatrix);
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        #endregion SeeingSharp

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************

        #region System.Numerics

        private static TimeSpan Check_SystemNumerics_Vector_CrossProduct()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                System.Numerics.Vector3 dummyVector = new System.Numerics.Vector3(2f, 3f, 4f);
                System.Numerics.Vector3 dummyVector2 = new System.Numerics.Vector3(1f, 2f, 3f);

                System.Numerics.Vector3 resultVector = System.Numerics.Vector3.Zero;
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    resultVector = System.Numerics.Vector3.Cross(dummyVector, dummyVector2);
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan Check_SystemNumerics_Vector_Add()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                System.Numerics.Vector3 dummyVector = new System.Numerics.Vector3(2f, 3f, 4f);
                System.Numerics.Vector3 dummyVector2 = new System.Numerics.Vector3(1f, 2f, 3f);

                System.Numerics.Vector3 resultVector = System.Numerics.Vector3.Zero;
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    resultVector = dummyVector + dummyVector2;
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan Check_SystemNumerics_Vector_Subtract()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                System.Numerics.Vector3 dummyVector = new System.Numerics.Vector3(2f, 3f, 4f);
                System.Numerics.Vector3 dummyVector2 = new System.Numerics.Vector3(1f, 2f, 3f);

                System.Numerics.Vector3 resultVector = System.Numerics.Vector3.Zero;
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    resultVector = dummyVector - dummyVector2;
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan Check_SystemNumerics_Vector_Transform()
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
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    resultVector = System.Numerics.Vector3.Transform(dummyVector, transformMatrix);
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan Check_SystemNumerics_Vector_DotProduct()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                System.Numerics.Vector3 dummyVector = new System.Numerics.Vector3(2f, 3f, 4f);
                System.Numerics.Vector3 dummyVector2 = new System.Numerics.Vector3(1f, 2f, 3f);

                float resultValue = 0f;
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    resultValue = System.Numerics.Vector3.Dot(dummyVector, dummyVector2);
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan Check_SystemNumerics_Vector_Normalize()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                System.Numerics.Vector3 dummyVector = new System.Numerics.Vector3(2f, 3f, 4f);

                System.Numerics.Vector3 resultNormal = System.Numerics.Vector3.Zero;
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    resultNormal = System.Numerics.Vector3.Normalize(dummyVector);
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static TimeSpan Check_SystemNumerics_Vector_MatrixMultiply()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                System.Numerics.Matrix4x4 dummyMatrix = System.Numerics.Matrix4x4.CreateTranslation(10f, 10f, 10f);
                System.Numerics.Matrix4x4 rotationMatrix = System.Numerics.Matrix4x4.CreateRotationY(1f);

                System.Numerics.Matrix4x4 resultMatrix = System.Numerics.Matrix4x4.Identity;
                for (int loop = 0; loop < COUNT_TRIES; loop++)
                {
                    resultMatrix = System.Numerics.Matrix4x4.Multiply(dummyMatrix, rotationMatrix);
                }
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        #endregion System.Numerics
    }
}