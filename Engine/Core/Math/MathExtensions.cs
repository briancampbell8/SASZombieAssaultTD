/*
File:    MathExtensions.cs
Purpose: Holds engine-wide math helpers (interpolation, approximate comparison, ranges).
         Provides comprehensive mathematical operations for animation, UI transitions, movement, and procedural systems.
         
Features: Interpolation methods, approximate comparison utilities, range generation, 
          and common mathematical operations used throughout the engine.
          Used by animation systems, UI transitions, movement calculations, and procedural generation.

Created: Engine Core Implementation
Notes:   This is the canonical math extension system for the entire engine.
         All advanced math operations should use this unified MathExtensions system.
*/

using SASZombieAssaultTD.Engine.Core.Random;
using System;

namespace SASZombieAssaultTD.Engine.Core.Math
{
    /// <summary>
    /// Engine-wide math helpers for interpolation, approximate comparison, ranges, and common operations.
    /// Used by animation, UI transitions, movement, and procedural systems throughout the engine.
    /// </summary>
    public static class MathExtensions
    {
        ///  Interpolation Methods
        
        /// <summary>
        /// Linearly interpolates between two values.
        /// </summary>
        /// <param name="a">Start value.</param>
        /// <param name="b">End value.</param>
        /// <param name="t">Interpolation factor (0.0 = a, 1.0 = b).</param>
        /// <returns>Interpolated value.</returns>
        public static double Lerp(double a, double b, double t)
        {
            t = System.Math.Clamp(t, 0.0, 1.0);
            return a + (b - a) * t;
        }
        
        /// <summary>
        /// Linearly interpolates between two values.
        /// </summary>
        /// <param name="a">Start value.</param>
        /// <param name="b">End value.</param>
        /// <param name="t">Interpolation factor (0.0 = a, 1.0 = b).</param>
        /// <returns>Interpolated value.</returns>
        public static float Lerp(float a, float b, float t)
        {
            t = System.Math.Clamp(t, 0f, 1f);
            return a + (b - a) * t;
        }
        
        /// <summary>
        /// Linearly interpolates between two values with unclamped factor.
        /// </summary>
        /// <param name="a">Start value.</param>
        /// <param name="b">End value.</param>
        /// <param name="t">Interpolation factor (not clamped).</param>
        /// <returns>Interpolated value.</returns>
        public static double LerpUnclamped(double a, double b, double t)
        {
            return a + (b - a) * t;
        }
        
        /// <summary>
        /// Linearly interpolates between two values with unclamped factor.
        /// </summary>
        /// <param name="a">Start value.</param>
        /// <param name="b">End value.</param>
        /// <param name="t">Interpolation factor (not clamped).</param>
        /// <returns>Interpolated value.</returns>
        public static float LerpUnclamped(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
        
        /// <summary>
        /// Smoothly interpolates between two values using smoothstep function.
        /// </summary>
        /// <param name="a">Start value.</param>
        /// <param name="b">End value.</param>
        /// <param name="t">Interpolation factor (0.0 = a, 1.0 = b).</param>
        /// <returns>Smoothly interpolated value.</returns>
        public static double SmoothStep(double a, double b, double t)
        {
            t = System.Math.Clamp(t, 0.0, 1.0);
            t = t * t * (3.0 - 2.0 * t); // Smoothstep formula
            return a + (b - a) * t;
        }
        
        /// <summary>
        /// Smoothly interpolates between two values using smoothstep function.
        /// </summary>
        /// <param name="a">Start value.</param>
        /// <param name="b">End value.</param>
        /// <param name="t">Interpolation factor (0.0 = a, 1.0 = b).</param>
        /// <returns>Smoothly interpolated value.</returns>
        public static float SmoothStep(float a, float b, float t)
        {
            t = System.Math.Clamp(t, 0f, 1f);
            t = t * t * (3f - 2f * t); // Smoothstep formula
            return a + (b - a) * t;
        }
        
        /// <summary>
        /// Smoothly interpolates between two values using smootherstep function.
        /// </summary>
        /// <param name="a">Start value.</param>
        /// <param name="b">End value.</param>
        /// <param name="t">Interpolation factor (0.0 = a, 1.0 = b).</param>
        /// <returns>Smoother interpolated value.</returns>
        public static double SmootherStep(double a, double b, double t)
        {
            t = System.Math.Clamp(t, 0.0, 1.0);
            t = t * t * t * (t * (t * 6.0 - 15.0) + 10.0); // Smootherstep formula
            return a + (b - a) * t;
        }
        
        /// <summary>
        /// Smoothly interpolates between two values using smootherstep function.
        /// </summary>
        /// <param name="a">Start value.</param>
        /// <param name="b">End value.</param>
        /// <param name="t">Interpolation factor (0.0 = a, 1.0 = b).</param>
        /// <returns>Smoother interpolated value.</returns>
        public static float SmootherStep(float a, float b, float t)
        {
            t = System.Math.Clamp(t, 0f, 1f);
            t = t * t * t * (t * (t * 6f - 15f) + 10f); // Smootherstep formula
            return a + (b - a) * t;
        }
        
        /// <summary>
        /// Inversely interpolates between two values.
        /// </summary>
        /// <param name="a">Start value.</param>
        /// <param name="b">End value.</param>
        /// <param name="value">Value to find interpolation factor for.</param>
        /// <returns>Interpolation factor (0.0 = a, 1.0 = b).</returns>
        public static double InverseLerp(double a, double b, double value)
        {
            if (System.Math.Abs(b - a) < 1e-10)
                return 0.0;
            return System.Math.Clamp((value - a) / (b - a), 0.0, 1.0);
        }
        
        /// <summary>
        /// Inversely interpolates between two values.
        /// </summary>
        /// <param name="a">Start value.</param>
        /// <param name="b">End value.</param>
        /// <param name="value">Value to find interpolation factor for.</param>
        /// <returns>Interpolation factor (0.0 = a, 1.0 = b).</returns>
        public static float InverseLerp(float a, float b, float value)
        {
            if (System.Math.Abs(b - a) < 1e-6f)
                return 0f;
            return System.Math.Clamp((value - a) / (b - a), 0f, 1f);
        }
        
        /// 

        ///  Approximate Comparison
        
        /// <summary>
        /// Determines if two floating-point values are approximately equal within a tolerance.
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <param name="tolerance">Tolerance for comparison (default: 1e-10).</param>
        /// <returns>True if values are approximately equal.</returns>
        public static bool Approximately(double a, double b, double tolerance = 1e-10)
        {
            return System.Math.Abs(a - b) < tolerance;
        }
        
        /// <summary>
        /// Determines if two floating-point values are approximately equal within a tolerance.
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <param name="tolerance">Tolerance for comparison (default: 1e-6).</param>
        /// <returns>True if values are approximately equal.</returns>
        public static bool Approximately(float a, float b, float tolerance = 1e-6f)
        {
            return System.Math.Abs(a - b) < tolerance;
        }
        
        /// <summary>
        /// Determines if two floating-point values are approximately equal using relative tolerance.
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <param name="relativeTolerance">Relative tolerance (default: 1e-8).</param>
        /// <returns>True if values are approximately equal.</returns>
        public static bool ApproximatelyRelative(double a, double b, double relativeTolerance = 1e-8)
        {
            if (a == b)
                return true;
                
            var absA = System.Math.Abs(a);
            var absB = System.Math.Abs(b);
            var diff = System.Math.Abs(a - b);
            
            return diff / System.Math.Max(absA, absB) < relativeTolerance;
        }
        
        /// <summary>
        /// Determines if two floating-point values are approximately equal using relative tolerance.
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <param name="relativeTolerance">Relative tolerance (default: 1e-5).</param>
        /// <returns>True if values are approximately equal.</returns>
        public static bool ApproximatelyRelative(float a, float b, float relativeTolerance = 1e-5f)
        {
            if (a == b)
                return true;
                
            var absA = System.Math.Abs(a);
            var absB = System.Math.Abs(b);
            var diff = System.Math.Abs(a - b);
            
            return diff / System.Math.Max(absA, absB) < relativeTolerance;
        }
        
        /// <summary>
        /// Determines if a value is approximately zero within a tolerance.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="tolerance">Tolerance for comparison (default: 1e-10).</param>
        /// <returns>True if value is approximately zero.</returns>
        public static bool ApproximatelyZero(double value, double tolerance = 1e-10)
        {
            return System.Math.Abs(value) < tolerance;
        }
        
        /// <summary>
        /// Determines if a value is approximately zero within a tolerance.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="tolerance">Tolerance for comparison (default: 1e-6).</param>
        /// <returns>True if value is approximately zero.</returns>
        public static bool ApproximatelyZero(float value, float tolerance = 1e-6f)
        {
            return System.Math.Abs(value) < tolerance;
        }
        
        /// 

        ///  Range Generation
        
        /// <summary>
        /// Returns a random integer in the specified range (inclusive).
        /// </summary>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <param name="max">Maximum value (inclusive).</param>
        /// <returns>Random integer in range [min, max].</returns>
        public static int Range(int min, int max)
        {
            return EngineRandom.Range(min, max + 1);
        }
        
        /// <summary>
        /// Returns a random floating-point number in the specified range.
        /// </summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>Random float in range [min, max].</returns>
        public static float Range(float min, float max)
        {
            return EngineRandom.Range(min, max);
        }
        
        /// <summary>
        /// Returns a random floating-point number in the specified range.
        /// </summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>Random double in range [min, max].</returns>
        public static double Range(double min, double max)
        {
            return EngineRandom.Range(min, max);
        }
        
        /// <summary>
        /// Returns a random value between 0.0 and 1.0.
        /// </summary>
        /// <returns>Random value in range [0.0, 1.0].</returns>
        public static double Value()
        {
            return EngineRandom.Value();
        }
        
        /// <summary>
        /// Returns a random value between 0.0 and 1.0.
        /// </summary>
        /// <returns>Random value in range [0.0, 1.0].</returns>
        public static float ValueFloat()
        {
            return EngineRandom.ValueFloat();
        }
        
        /// <summary>
        /// Returns a random sign (-1 or 1).
        /// </summary>
        /// <returns>Random sign.</returns>
        public static int Sign()
        {
            return EngineRandom.Sign();
        }
        
        /// <summary>
        /// Returns a random boolean value.
        /// </summary>
        /// <returns>Random true or false.</returns>
        public static bool Bool()
        {
            return EngineRandom.Bool();
        }
        
        /// <summary>
        /// Returns true with the specified probability.
        /// </summary>
        /// <param name="probability">Probability of returning true (0.0 to 1.0).</param>
        /// <returns>True with the specified probability.</returns>
        public static bool Bool(float probability)
        {
            return EngineRandom.Bool(probability);
        }
        
        /// 

        ///  Common Mathematical Operations
        
        /// <summary>
        /// Maps a value from one range to another.
        /// </summary>
        /// <param name="value">Value to map.</param>
        /// <param name="fromMin">Source range minimum.</param>
        /// <param name="fromMax">Source range maximum.</param>
        /// <param name="toMin">Target range minimum.</param>
        /// <param name="toMax">Target range maximum.</param>
        /// <returns>Mapped value.</returns>
        public static double Map(double value, double fromMin, double fromMax, double toMin, double toMax)
        {
            if (System.Math.Abs(fromMax - fromMin) < 1e-10)
                return toMin;
                
            var t = (value - fromMin) / (fromMax - fromMin);
            return toMin + t * (toMax - toMin);
        }
        
        /// <summary>
        /// Maps a value from one range to another.
        /// </summary>
        /// <param name="value">Value to map.</param>
        /// <param name="fromMin">Source range minimum.</param>
        /// <param name="fromMax">Source range maximum.</param>
        /// <param name="toMin">Target range minimum.</param>
        /// <param name="toMax">Target range maximum.</param>
        /// <returns>Mapped value.</returns>
        public static float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            if (System.Math.Abs(fromMax - fromMin) < 1e-6f)
                return toMin;
                
            var t = (value - fromMin) / (fromMax - fromMin);
            return toMin + t * (toMax - toMin);
        }
        
        /// <summary>
        /// Clamps a value between minimum and maximum values.
        /// </summary>
        /// <param name="value">Value to clamp.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>Clamped value.</returns>
        public static double Clamp(double value, double min, double max)
        {
            return System.Math.Clamp(value, min, max);
        }
        
        /// <summary>
        /// Clamps a value between minimum and maximum values.
        /// </summary>
        /// <param name="value">Value to clamp.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>Clamped value.</returns>
        public static float Clamp(float value, float min, float max)
        {
            return System.Math.Clamp(value, min, max);
        }
        
        /// <summary>
        /// Wraps a value between minimum and maximum values (circular clamp).
        /// </summary>
        /// <param name="value">Value to wrap.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>Wrapped value.</returns>
        public static double Wrap(double value, double min, double max)
        {
            var range = max - min;
            if (range <= 0)
                return min;
                
            var result = (value - min) % range;
            if (result < 0)
                result += range;
            return result + min;
        }
        
        /// <summary>
        /// Wraps a value between minimum and maximum values (circular clamp).
        /// </summary>
        /// <param name="value">Value to wrap.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>Wrapped value.</returns>
        public static float Wrap(float value, float min, float max)
        {
            var range = max - min;
            if (range <= 0)
                return min;
                
            var result = (value - min) % range;
            if (result < 0)
                result += range;
            return result + min;
        }
        
        /// <summary>
        /// Wraps an angle to the range [-π, π].
        /// </summary>
        /// <param name="angle">Angle in radians.</param>
        /// <returns>Wrapped angle in range [-π, π].</returns>
        public static double WrapAngle(double angle)
        {
            return Wrap(angle, -System.Math.PI, System.Math.PI);
        }
        
        /// <summary>
        /// Wraps an angle to the range [-π, π].
        /// </summary>
        /// <param name="angle">Angle in radians.</param>
        /// <returns>Wrapped angle in range [-π, π].</returns>
        public static float WrapAngle(float angle)
        {
            return Wrap(angle, -MathF.PI, MathF.PI);
        }
        
        /// <summary>
        /// Wraps an angle to the range [0, 2π].
        /// </summary>
        /// <param name="angle">Angle in radians.</param>
        /// <returns>Wrapped angle in range [0, 2π].</returns>
        public static double WrapAnglePositive(double angle)
        {
            return Wrap(angle, 0, 2 * System.Math.PI);
        }
        
        /// <summary>
        /// Wraps an angle to the range [0, 2π].
        /// </summary>
        /// <param name="angle">Angle in radians.</param>
        /// <returns>Wrapped angle in range [0, 2π].</returns>
        public static float WrapAnglePositive(float angle)
        {
            return Wrap(angle, 0f, 2f * MathF.PI);
        }
        
        /// 

        ///  Trigonometric Extensions
        
        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">Angle in degrees.</param>
        /// <returns>Angle in radians.</returns>
        public static double DegreesToRadians(double degrees)
        {
            return degrees * System.Math.PI / 180.0;
        }
        
        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">Angle in degrees.</param>
        /// <returns>Angle in radians.</returns>
        public static float DegreesToRadians(float degrees)
        {
            return degrees * MathF.PI / 180f;
        }
        
        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <returns>Angle in degrees.</returns>
        public static double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / System.Math.PI;
        }
        
        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <returns>Angle in degrees.</returns>
        public static float RadiansToDegrees(float radians)
        {
            return radians * 180f / MathF.PI;
        }
        
        /// <summary>
        /// Normalizes an angle to the range [-π, π].
        /// </summary>
        /// <param name="angle">Angle in radians.</param>
        /// <returns>Normalized angle in range [-π, π].</returns>
        public static double NormalizeAngle(double angle)
        {
            return WrapAngle(angle);
        }
        
        /// <summary>
        /// Normalizes an angle to the range [-π, π].
        /// </summary>
        /// <param name="angle">Angle in radians.</param>
        /// <returns>Normalized angle in range [-π, π].</returns>
        public static float NormalizeAngle(float angle)
        {
            return WrapAngle(angle);
        }
        
        /// <summary>
        /// Calculates the shortest angular difference between two angles.
        /// </summary>
        /// <param name="a">First angle in radians.</param>
        /// <param name="b">Second angle in radians.</param>
        /// <returns>Shortest angular difference in range [-π, π].</returns>
        public static double AngleDifference(double a, double b)
        {
            var diff = b - a;
            return WrapAngle(diff);
        }
        
        /// <summary>
        /// Calculates the shortest angular difference between two angles.
        /// </summary>
        /// <param name="a">First angle in radians.</param>
        /// <param name="b">Second angle in radians.</param>
        /// <returns>Shortest angular difference in range [-π, π].</returns>
        public static float AngleDifference(float a, float b)
        {
            var diff = b - a;
            return WrapAngle(diff);
        }
        
        /// 

        ///  Power and Root Extensions
        
        /// <summary>
        /// Safely calculates the square root of a value.
        /// </summary>
        /// <param name="value">Value to calculate square root for.</param>
        /// <returns>Square root, or 0 if value is negative.</returns>
        public static double SafeSqrt(double value)
        {
            return System.Math.Max(0.0, value);
        }
        
        /// <summary>
        /// Safely calculates the square root of a value.
        /// </summary>
        /// <param name="value">Value to calculate square root for.</param>
        /// <returns>Square root, or 0 if value is negative.</returns>
        public static float SafeSqrt(float value)
        {
            return System.Math.Max(0f, value);
        }
        
        /// <summary>
        /// Calculates the inverse square root (1/sqrt(x)).
        /// </summary>
        /// <param name="value">Value to calculate inverse square root for.</param>
        /// <returns>Inverse square root, or 0 if value is zero or negative.</returns>
        public static double InvSqrt(double value)
        {
            if (value <= 0.0)
                return 0.0;
            return 1.0 / System.Math.Sqrt(value);
        }
        
        /// <summary>
        /// Calculates the inverse square root (1/sqrt(x)).
        /// </summary>
        /// <param name="value">Value to calculate inverse square root for.</param>
        /// <returns>Inverse square root, or 0 if value is zero or negative.</returns>
        public static float InvSqrt(float value)
        {
            if (value <= 0f)
                return 0f;
            return 1f / MathF.Sqrt(value);
        }
        
        /// <summary>
        /// Raises a value to a power safely.
        /// </summary>
        /// <param name="value">Base value.</param>
        /// <param name="power">Exponent.</param>
        /// <returns>Value raised to power.</returns>
        public static double SafePow(double value, double power)
        {
            if (value < 0.0 && System.Math.Abs(power % 1.0) > 1e-10)
                return 0.0; // Can't raise negative to non-integer power
            return System.Math.Pow(value, power);
        }
        
        /// <summary>
        /// Raises a value to a power safely.
        /// </summary>
        /// <param name="value">Base value.</param>
        /// <param name="power">Exponent.</param>
        /// <returns>Value raised to power.</returns>
        public static float SafePow(float value, float power)
        {
            if (value < 0f && System.Math.Abs(power % 1f) > 1e-6f)
                return 0f; // Can't raise negative to non-integer power
            return MathF.Pow(value, power);
        }
        
        /// 

        ///  Utility Methods
        
        /// <summary>
        /// Returns the next power of two greater than or equal to the specified value.
        /// </summary>
        /// <param name="value">Value to find next power of two for.</param>
        /// <returns>Next power of two.</returns>
        public static int NextPowerOfTwo(int value)
        {
            if (value <= 0)
                return 1;
                
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value++;
            
            return value;
        }
        
        /// <summary>
        /// Checks if a value is a power of two.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True if value is a power of two.</returns>
        public static bool IsPowerOfTwo(int value)
        {
            return value > 0 && (value & (value - 1)) == 0;
        }
        
        /// <summary>
        /// Returns the sign of a value (-1, 0, or 1).
        /// </summary>
        /// <param name="value">Value to get sign for.</param>
        /// <returns>Sign of the value.</returns>
        public static int Sign(double value)
        {
            if (value > 0) return 1;
            if (value < 0) return -1;
            return 0;
        }
        
        /// <summary>
        /// Returns the sign of a value (-1, 0, or 1).
        /// </summary>
        /// <param name="value">Value to get sign for.</param>
        /// <returns>Sign of the value.</returns>
        public static int Sign(float value)
        {
            if (value > 0) return 1;
            if (value < 0) return -1;
            return 0;
        }
        
        /// <summary>
        /// Calculates the percentage of a value relative to a total.
        /// </summary>
        /// <param name="value">Value to calculate percentage for.</param>
        /// <param name="total">Total value.</param>
        /// <returns>Percentage (0.0 to 1.0).</returns>
        public static double Percentage(double value, double total)
        {
            if (System.Math.Abs(total) < 1e-10)
                return 0.0;
            return System.Math.Clamp(value / total, 0.0, 1.0);
        }
        
        /// <summary>
        /// Calculates the percentage of a value relative to a total.
        /// </summary>
        /// <param name="value">Value to calculate percentage for.</param>
        /// <param name="total">Total value.</param>
        /// <returns>Percentage (0.0 to 1.0).</returns>
        public static float Percentage(float value, float total)
        {
            if (System.Math.Abs(total) < 1e-6f)
                return 0f;
            return System.Math.Clamp(value / total, 0f, 1f);
        }
        
        /// <summary>
        /// Calculates the percentage of a value relative to a total as a percentage string.
        /// </summary>
        /// <param name="value">Value to calculate percentage for.</param>
        /// <param name="total">Total value.</param>
        /// <param name="decimalPlaces">Number of decimal places.</param>
        /// <returns>Percentage string (e.g., "75.0%").</returns>
        public static string PercentageString(double value, double total, int decimalPlaces = 1)
        {
            var percentage = Percentage(value, total) * 100f;
            return percentage.ToString($"F{decimalPlaces}") + "%";
        }
        
        /// <summary>
        /// Calculates the percentage of a value relative to a total as a percentage string.
        /// </summary>
        /// <param name="value">Value to calculate percentage for.</param>
        /// <param name="total">Total value.</param>
        /// <param name="decimalPlaces">Number of decimal places.</param>
        /// <returns>Percentage string (e.g., "75.0%").</returns>
        public static string PercentageString(float value, float total, int decimalPlaces = 1)
        {
            var percentage = Percentage(value, total) * 100f;
            return percentage.ToString($"F{decimalPlaces}") + "%";
        }
        
        /// 
    }
}