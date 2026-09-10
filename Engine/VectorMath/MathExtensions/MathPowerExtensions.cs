// =====================================================================================================
//  FILE: MathPowerExtensions.cs
//  PATH: Engine/VectorMath/MathExtensions/MathPowerExtensions.cs
//  SUBSYSTEM: VectorMath MathExtensions Subsystem
//
//  ROLE:
//      Provides unified, static mathematical functions for safe root calculations and exponential
//      operations. Serves as a vital processing library for physics equations, vector length
//      normalizations, attenuation curves, and distance scaling functions across the engine.
//
//  RESPONSIBILITIES:
//      - Calculate domain-safe square roots that guard against negative values.
//      - Calculate inverse square roots (1/sqrt(x)) safely to avoid division-by-zero scenarios.
//      - Raise numeric bases to exponential powers while protecting against invalid negative-base configurations.
//
//  NON-RESPONSIBILITIES:
//      - Normalizing absolute directional game vectors or structures directly.
//      - Managing sound distance attenuation profiles or light falloff configurations.
//
//  ARCHITECTURAL NOTES:
//      - This class is part of the unified mathematical extensions module.
//      - All methods are pure, stateless, and safe to execute across concurrent thread pools.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.VectorMath.MathExtensions
{
    public static class MathPowerExtensions
    {
        // -------------------------------------------------------------------------------------------------
        // SAFE SQRT
        // -------------------------------------------------------------------------------------------------

        public static double SafeSqrt(double value)
        {
            if (value <= 0.0)
                return 0.0;

            return System.Math.Sqrt(value);
        }

        public static float SafeSqrt(float value)
        {
            if (value <= 0f)
                return 0f;

            return (float)System.Math.Sqrt(value);
        }

        // -------------------------------------------------------------------------------------------------
        // INVERSE SQRT
        // -------------------------------------------------------------------------------------------------

        public static double InvSqrt(double value)
        {
            if (value <= 0.0)
                return 0.0;

            return 1.0 / System.Math.Sqrt(value);
        }

        public static float InvSqrt(float value)
        {
            if (value <= 0f)
                return 0f;

            return 1f / (float)System.Math.Sqrt(value);
        }

        // -------------------------------------------------------------------------------------------------
        // SAFE POW
        // -------------------------------------------------------------------------------------------------

        public static double SafePow(double value, double power)
        {
            // If base is negative AND power is not an integer → invalid
            if (value < 0.0)
            {
                double rounded = System.Math.Round(power);
                if (System.Math.Abs(power - rounded) > 1e-10)
                    return 0.0;
            }

            return System.Math.Pow(value, power);
        }

        public static float SafePow(float value, float power)
        {
            if (value < 0f)
            {
                float rounded = (float)System.Math.Round(power);
                if (System.Math.Abs(power - rounded) > 1e-6f)
                    return 0f;
            }

            return (float)System.Math.Pow(value, power);
        }
    }
}
