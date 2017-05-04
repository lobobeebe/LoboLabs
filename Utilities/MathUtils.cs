using System;

namespace LoboLabs.Utilities
{
    public static class MathUtils
    {
        private static Random sRandom = new Random();

        public static void Seed(int seed)
        {
            sRandom = new Random(seed);
        }

        public static double NextRand(double min = 0, double max = 1)
        {
            return min + sRandom.NextDouble() * (max - min);
        }

        public static uint NextRandUInt()
        {
            return (uint)sRandom.Next();
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0)
            {
                return min;
            }
            else if (val.CompareTo(max) > 0)
            {
                return max;
            }
            else
            {
                return val;
            }
        }
    }
}