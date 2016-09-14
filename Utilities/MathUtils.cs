using System;

namespace LoboLabs
{ 
namespace Utilities
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

}

}
}