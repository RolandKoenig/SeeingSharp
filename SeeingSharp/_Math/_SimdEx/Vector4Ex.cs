using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp
{
    public static class Vector4Ex
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetValue(Vector4 vector, int index)
        {
            switch (index)
            {
                case 1: return vector.X;
                case 2: return vector.Y;
                case 3: return vector.Z;
                case 4: return vector.W;
                default: throw new ArgumentException("Invalid index!");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(Vector4 vector, int index, float value)
        {
            switch (index)
            {
                case 1: vector.X = value; break;
                case 2: vector.Y = value; break;
                case 3: vector.Z = value; break;
                case 4: vector.W = value; break;
                default: throw new ArgumentException("Invalid index!");
            }
        }
    }
}
