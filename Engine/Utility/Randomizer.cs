/*
File:    Randomizer.cs
Purpose: NextInt, NextFloat; seeded constructor; NextBool.
*/
using System;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Utility
{
    public static class Randomizer
    {
        private static readonly Random _rng = new Random();

        public static int NextInt(int min, int max)
        {
            return _rng.Next(min, max);
        }

        public static float NextFloat(float min, float max)
        {
            return min + (float)_rng.NextDouble() * (max - min);
        }

        public static bool NextBool()
        {
            return _rng.Next(2) == 1;
        }
    }

    public class SeededRandomizer
    {
        private readonly Random _random;

        public SeededRandomizer(int seed)
        {
            _random = new Random(seed);
        }

        public int NextInt(int min, int max)
        {
            return _random.Next(min, max);
        }

        public float NextFloat(float min, float max)
        {
            return min + (float)_random.NextDouble() * (max - min);
        }

        public bool NextBool()
        {
            return _random.Next(2) == 1;
        }
    }
}




