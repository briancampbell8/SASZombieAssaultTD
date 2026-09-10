// =====================================================================================================
//  FILE: MathInterpolationExtensions.cs
//  PATH: Engine/VectorMath/MathExtensions/MathInterpolationExtensions.cs
//  SUBSYSTEM: VectorMath MathExtensions Subsystem
//
//  ROLE:
//      Provides unified, static mathematical functions for single and double precision transitions.
//      Enables deterministic calculation steps for game animations, user interface transitions,
//      camera movements, and procedural pacing nodes across the engine.
//
//  RESPONSIBILITIES:
//      - Provide standard and unclamped Linear Interpolation (Lerp) calculations.
//      - Provide analytical SmoothStep and SmootherStep non-linear blending functions.
//      - Calculate inverse interpolation fractions safely to protect against division-by-zero errors.
//
//  NON-RESPONSIBILITIES:
//      - Maintaining running transformation durations, interpolation states, or frame timers.
//      - Rendering graphics or applying vector translations to spatial components directly.
//
//  ARCHITECTURAL NOTES:
//      - This class is part of the unified mathematical extensions module.
//      - All methods are pure, stateless, and safe to execute across concurrent thread pools.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.VectorMath.MathExtensions
{
    public static class MathInterpolationExtensions
    {
        // -------------------------------------------------------------------------------------------------
        // LERP
        // -------------------------------------------------------------------------------------------------

        public static double Lerp(double a, double b, double t)
        {
            t = System.Math.Clamp(t, 0.0, 1.0);
            return a + (b - a) * t;
        }

        public static float Lerp(float a, float b, float t)
        {
            t = System.Math.Clamp(t, 0f, 1f);
            return a + (b - a) * t;
        }

        public static double LerpUnclamped(double a, double b, double t)
        {
            return a + (b - a) * t;
        }

        public static float LerpUnclamped(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        // -------------------------------------------------------------------------------------------------
        // SMOOTHSTEP
        // -------------------------------------------------------------------------------------------------

        public static double SmoothStep(double a, double b, double t)
        {
            t = System.Math.Clamp(t, 0.0, 1.0);
            t = t * t * (3.0 - 2.0 * t);

            // Clamp final output to avoid overshoot when a > b
            var result = a + (b - a) * t;
            return System.Math.Clamp(result, System.Math.Min(a, b), System.Math.Max(a, b));
        }

        public static float SmoothStep(float a, float b, float t)
        {
            t = System.Math.Clamp(t, 0f, 1f);
            t = t * t * (3f - 2f * t);

            var result = a + (b - a) * t;
            return System.Math.Clamp(result, System.Math.Min(a, b), System.Math.Max(a, b));
        }

        // -------------------------------------------------------------------------------------------------
        // SMOOTHERSTEP
        // -------------------------------------------------------------------------------------------------

        public static double SmootherStep(double a, double b, double t)
        {
            t = System.Math.Clamp(t, 0.0, 1.0);
            t = t * t * t * (t * (t * 6.0 - 15.0) + 10.0);

            var result = a + (b - a) * t;
            return System.Math.Clamp(result, System.Math.Min(a, b), System.Math.Max(a, b));
        }

        public static float SmootherStep(float a, float b, float t)
        {
            t = System.Math.Clamp(t, 0f, 1f);
            t = t * t * t * (t * (t * 6f - 15f) + 10f);

            var result = a + (b - a) * t;
            return System.Math.Clamp(result, System.Math.Min(a, b), System.Math.Max(a, b));
        }

        // -------------------------------------------------------------------------------------------------
        // INVERSE LERP
        // -------------------------------------------------------------------------------------------------

        public static double InverseLerp(double a, double b, double value)
        {
            if (System.Math.Abs(b - a) < 1e-10)
                return 0.0;

            // Unclamped fraction (correct for procedural systems)
            return (value - a) / (b - a);
        }

        public static float InverseLerp(float a, float b, float value)
        {
            if (System.Math.Abs(b - a) < 1e-6f)
                return 0f;

            return (value - a) / (b - a);
        }
    }
}
