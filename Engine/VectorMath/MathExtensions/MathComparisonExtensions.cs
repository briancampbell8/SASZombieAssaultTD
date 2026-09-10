// =====================================================================================================
//  FILE: MathComparisonExtensions.cs
//  PATH: Engine/Extensions/MathComparisonExtensions.cs
//  SUBSYSTEM: Extensions Math Subsystem
//
//  ROLE:
//      Provides unified, static mathematical functions for absolute and relative floating-point 
//      approximate equality. Enables precise geometric, physics, and gameplay comparison checks 
//      without suffering from standard float/double precision drifting errors.
//
//  RESPONSIBILITIES:
//      - Provide standard absolute tolerance checks for double and single precision variables.
//      - Provide relative scale-invariant tolerance checks for extreme values.
//      - Provide specialized low-overhead zero-proximity check boundaries.
//
//  NON-RESPONSIBILITIES:
//      - Resolving gameplay collision state evaluations or spatial tree updates.
//      - Normalizing angular vector bounds or clamping geometric numbers.
//
//  ARCHITECTURAL NOTES:
//      - This class is part of the unified mathematical extensions module.
//      - All methods are pure, stateless, and safe to execute across concurrent thread pools.
// =====================================================================================================

using System.Runtime.CompilerServices;

namespace SASZombieAssaultTD.Engine.VectorMath.MathExtensions
{
    public static class MathComparisonExtensions
    {
        private const double DOUBLE_ZERO_TOLERANCE = 1e-10;
        private const float FLOAT_ZERO_TOLERANCE = 1e-6f;
        private const double DOUBLE_RELATIVE_TOLERANCE = 1e-8;
        private const float FLOAT_RELATIVE_TOLERANCE = 1e-5f;

        // -------------------------------------------------------------------------------------------------
        // ABSOLUTE APPROXIMATE EQUALITY
        // -------------------------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(double a, double b, double tolerance = DOUBLE_ZERO_TOLERANCE)
        {
            return System.Math.Abs(a - b) < tolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(float a, float b, float tolerance = FLOAT_ZERO_TOLERANCE)
        {
            return System.Math.Abs(a - b) < tolerance;
        }

        // -------------------------------------------------------------------------------------------------
        // RELATIVE APPROXIMATE EQUALITY (SCALE-INVARIANT)
        // -------------------------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproximatelyRelative(double a, double b, double relativeTolerance = DOUBLE_RELATIVE_TOLERANCE)
        {
            if (a == b)
                return true;

            var absA = System.Math.Abs(a);
            var absB = System.Math.Abs(b);
            var diff = System.Math.Abs(a - b);
            var max = System.Math.Max(absA, absB);

            // Zero-proximity fallback to avoid division by zero and handle near-zero values correctly.
            if (max < DOUBLE_ZERO_TOLERANCE)
                return diff < DOUBLE_ZERO_TOLERANCE;

            return diff / max < relativeTolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproximatelyRelative(float a, float b, float relativeTolerance = FLOAT_RELATIVE_TOLERANCE)
        {
            if (a == b)
                return true;

            var absA = System.Math.Abs(a);
            var absB = System.Math.Abs(b);
            var diff = System.Math.Abs(a - b);
            var max = System.Math.Max(absA, absB);

            // Zero-proximity fallback to avoid division by zero and handle near-zero values correctly.
            if (max < FLOAT_ZERO_TOLERANCE)
                return diff < FLOAT_ZERO_TOLERANCE;

            return diff / max < relativeTolerance;
        }

        // -------------------------------------------------------------------------------------------------
        // ZERO-PROXIMITY CHECKS
        // -------------------------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproximatelyZero(double value, double tolerance = DOUBLE_ZERO_TOLERANCE)
        {
            return System.Math.Abs(value) < tolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproximatelyZero(float value, float tolerance = FLOAT_ZERO_TOLERANCE)
        {
            return System.Math.Abs(value) < tolerance;
        }
    }
}
