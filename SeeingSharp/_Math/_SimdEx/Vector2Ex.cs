using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp
{
    public static class Vector2Ex
    {
        public static Vector2 FromSize2(Size2 size)
        {
            return new Vector2(size.Width, size.Height);
        }

        public static Vector2 FromSize2(Size2F size)
        {
            return new Vector2(size.Width, size.Height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetValue(Vector2 vector, int index)
        {
            switch (index)
            {
                case 1: return vector.X;
                case 2: return vector.Y;
                default: throw new ArgumentException("Invalid index!");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(Vector2 vector, int index, float value)
        {
            switch (index)
            {
                case 1: vector.X = value; break;
                case 2: vector.Y = value; break;
                default: throw new ArgumentException("Invalid index!");
            }
        }

    }
}
