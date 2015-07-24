using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.PerformanceTests.Mathematics;

namespace SeeingSharp.PerformanceTests
{
    public static class Program
    {
        public static void Main(string[] args)
        {

            Console.WriteLine("#################### SeeingSharp Vector");
            Console.WriteLine("Multiplication: " + VectorFunctionTests.Check_SeeingSharp_Vector_Multiplication().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("Add:            " + VectorFunctionTests.Check_SeeingSharp_Vector_Add().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("Subtract:       " + VectorFunctionTests.Check_SeeingSharp_Vector_Subtract().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("Transform:      " + VectorFunctionTests.Check_SeeingSharp_Vector_Transform().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine();

            Console.WriteLine("#################### System.Numerics Vector");
            Console.WriteLine("Multiplication: " + VectorFunctionTests.Check_SystemNumerics_Vector_Multiplication().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("Add:            " + VectorFunctionTests.Check_SystemNumerics_Vector_Add().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("Subtract:       " + VectorFunctionTests.Check_SystemNumerics_Vector_Subtract().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine("Transform:      " + VectorFunctionTests.Check_SystemNumerics_Vector_Transform().TotalMilliseconds.ToString("F2") + "ms");
            Console.WriteLine();
        }
    }
}
