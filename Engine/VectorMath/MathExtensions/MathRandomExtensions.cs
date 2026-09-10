// =====================================================================================================
//  FILE: MathRandomExtensions.cs
//  PATH: Engine/VectorMath/MathExtensions/MathRandomExtensions.cs
//  SUBSYSTEM: VectorMath MathExtensions Subsystem
//
//  ROLE:
//      Provides unified, static mathematical functions for random range generation, signs, and 
//      probability checks. Wraps the low-level engine RNG subsystem to offer a standard math API 
//      for procedural spawning, loot drops, weapon spreads, and stochastic game systems.
//
//  RESPONSIBILITIES:
//      - Provide boundary-safe random ranges for integer, float, and double types.
//      - Provide random state flags (booleans) optionally driven by specific weight probabilities.
//      - Provide randomized directional signs (+1 or -1) for directional logic.
//
//  NON-RESPONSIBILITIES:
//      - Initializing global RNG seeding values, tracking noise maps, or handling cryptographic keys.
//      - Managing localized item drop registries or generating specific enemy wave composition vectors.
//
//  ARCHITECTURAL NOTES:
//      - This class acts as a high-level extensions facade interacting with EngineRandom.
//      - All underlying method calls rely on the core engine's running simulation randomness subsystem.
// =====================================================================================================

using SASZombieAssaultTD.Engine.CoreSize.Random;

namespace SASZombieAssaultTD.Engine.VectorMath.MathExtensions
{
    public static class MathRandomExtensions
    {
        public static int Range(int min, int max)
        {
            // Correct inclusive-range behavior
            return EngineRandom.Range(min, max);
        }

        public static float Range(float min, float max)
        {
            return EngineRandom.Range(min, max);
        }

        public static double Range(double min, double max)
        {
            return EngineRandom.Range(min, max);
        }

        public static double Value()
        {
            return EngineRandom.Value();
        }

        public static float ValueFloat()
        {
            return EngineRandom.ValueFloat();
        }

        public static int Sign()
        {
            return EngineRandom.Sign();
        }

        public static bool Bool()
        {
            return EngineRandom.Bool();
        }

        public static bool Bool(float probability)
        {
            return EngineRandom.Bool(probability);
        }
    }
}
