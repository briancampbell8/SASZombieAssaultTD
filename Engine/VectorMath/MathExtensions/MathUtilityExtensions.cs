// =====================================================================================================
//  FILE: MathUtilityExtensions.cs
//  PATH: Engine/VectorMath/MathExtensions/MathUtilityExtensions.cs
//  SUBSYSTEM: VectorMath MathExtensions Subsystem
//
//  ROLE:
//      Provides unified, static mathematical functions for general engine-wide utility calculations.
//      Handles bitwise power-of-two optimizations, numeric sign evaluations, percentage ratio
//      fraction reductions, and human-readable string formats for diagnostics and user interfaces.
//
//  RESPONSIBILITIES:
//      - Perform fast bit-shifting calculations to determine next power-of-two boundaries.
//      - Verify binary power-of-two allocations for low-level textures or buffer alignment steps.
//      - Calculate safe progress metrics and convert them to formatted percentage text blocks.
//
//  NON-RESPONSIBILITIES:
//      - Parsing arbitrary localized user strings or manipulating complex text layouts.
//      - Direct hardware buffer allocations or tracking virtual graphics card limits.
//
//  ARCHITECTURAL NOTES:
//      - This class is part of the unified mathematical extensions module.
//      - All methods are pure, stateless, and safe to execute across concurrent thread pools.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.VectorMath.MathExtensions
{
    public static class MathUtilityExtensions
    {
        public static int NextPowerOfTwo(int value)
        {
            if (value <= 0)
                return 1;

            // Prevent overflow: if value is already too large, clamp to int.MaxValue
            if (value >= (1 << 30))
                return int.MaxValue;

            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value++;
            return value;
        }

        public static bool IsPowerOfTwo(int value)
        {
            return value > 0 && (value & (value - 1)) == 0;
        }

        public static int Sign(double value)
        {
            if (value > 0) return 1;
            if (value < 0) return -1;
            return 0;
        }

        public static int Sign(float value)
        {
            if (value > 0) return 1;
            if (value < 0) return -1;
            return 0;
        }

        public static double Percentage(double value, double total)
        {
            if (System.Math.Abs(total) < 1e-10)
                return 0.0;

            return System.Math.Clamp(value / total, 0.0, 1.0);
        }

        public static float Percentage(float value, float total)
        {
            if (System.Math.Abs(total) < 1e-6f)
                return 0f;

            return System.Math.Clamp(value / total, 0f, 1f);
        }

        public static string PercentageString(double value, double total, int decimalPlaces = 1)
        {
            var percentage = Percentage(value, total) * 100.0;
            return percentage.ToString($"F{decimalPlaces}") + "%";
        }

        public static string PercentageString(float value, float total, int decimalPlaces = 1)
        {
            var percentage = Percentage(value, total) * 100f;
            return percentage.ToString($"F{decimalPlaces}") + "%";
        }
    }
}
