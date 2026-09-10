// =====================================================================================================
//  FILE: MathCommonExtensions.cs
//  PATH: Engine/Extensions/Math/MathCommonExtensions.cs
//  SUBSYSTEM: Extensions Math Subsystem
//
//  ROLE:
//      Provides unified, static mathematical functions for foundational number mapping, value
//      clamping boundaries, and cyclic domain wrapping. Serves as a core utility layer for general
//      gameplay systems, positioning limits, and progress modifiers throughout the engine.
//
//  RESPONSIBILITIES:
//      - Map numeric values linearly from an input domain range to a new target range.
//      - Clamp single and double precision variables strictly within a minimum and maximum window.
//      - Perform modulo-based circular wrapping on numeric domains safely avoiding negative balances.
//
//  NON-RESPONSIBILITIES:
//      - Managing stateful progression thresholds or UI progress bar animations.
//      - Tracking localized bounding-box collisions or handling spatial ECSEntityCore transformations.
//
//  ARCHITECTURAL NOTES:
//      - This class is part of the unified mathematical extensions module.
//      - All methods are pure, stateless, and safe to execute across concurrent thread pools.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.VectorMath.MathExtensions
{
    public static class MathCommonExtensions
    {
        private const double DOUBLE_TOLERANCE = 1e-10;
        private const float FLOAT_TOLERANCE = 1e-6f;

        // -------------------------------------------------------------------------------------------------
        // MAP
        // -------------------------------------------------------------------------------------------------

        public static double Map(double value, double fromMin, double fromMax, double toMin, double toMax)
        {
            if (System.Math.Abs(fromMax - fromMin) < DOUBLE_TOLERANCE)
                return toMin;

            var t = (value - fromMin) / (fromMax - fromMin);

            // Optional: clamp t to avoid floating-point drift
            if (t < 0.0) t = 0.0;
            if (t > 1.0) t = 1.0;

            return toMin + t * (toMax - toMin);
        }

        public static float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            if (System.Math.Abs(fromMax - fromMin) < FLOAT_TOLERANCE)
                return toMin;

            var t = (value - fromMin) / (fromMax - fromMin);

            if (t < 0f) t = 0f;
            if (t > 1f) t = 1f;

            return toMin + t * (toMax - toMin);
        }

        // -------------------------------------------------------------------------------------------------
        // CLAMP
        // -------------------------------------------------------------------------------------------------

        public static double Clamp(double value, double min, double max)
        {
            return System.Math.Clamp(value, min, max);
        }

        public static float Clamp(float value, float min, float max)
        {
            return System.Math.Clamp(value, min, max);
        }

        // -------------------------------------------------------------------------------------------------
        // WRAP (CYCLIC DOMAIN)
        // -------------------------------------------------------------------------------------------------

        public static double Wrap(double value, double min, double max)
        {
            var range = max - min;
            if (range <= 0)
                return min;

            var result = (value - min) % range;

            // Correct negative modulo
            if (result < 0)
                result += range;

            // Snap-to-boundary tolerance
            if (System.Math.Abs(result - range) < DOUBLE_TOLERANCE)
                return min;

            return result + min;
        }

        public static float Wrap(float value, float min, float max)
        {
            var range = max - min;
            if (range <= 0)
                return min;

            var result = (value - min) % range;

            if (result < 0)
                result += range;

            if (System.Math.Abs(result - range) < FLOAT_TOLERANCE)
                return min;

            return result + min;
        }
    }
}
