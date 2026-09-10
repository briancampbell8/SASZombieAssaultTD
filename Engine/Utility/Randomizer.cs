// ====================================================================================================
//  FILE: Randomizer.cs
//  PATH: ./Engine/Utility/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the Randomizer module.
//
//  RESPONSIBILITIES:
//      - Provide NextInt() behavior for the Core subsystem.
//      - Provide NextFloat() behavior for the Core subsystem.
//      - Provide NextBool() behavior for the Core subsystem.
//      - Provide NextInt() behavior for the Core subsystem.
//      - Provide NextFloat() behavior for the Core subsystem.
//      - Provide NextBool() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    Randomizer.cs
Purpose: NextInt, NextFloat; seeded constructor; NextBool.
*/
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

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

        public SeededRandomizer(int seed) => _random = new Random(seed);

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





