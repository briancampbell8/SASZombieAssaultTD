/*
File:    EngineCompatibility.cs
Purpose: Compatibility layer for missing engine APIs and methods.
*/

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Rendering;

namespace SASZombieAssaultTD.Engine.Utilities
{
    /// <summary>
    /// Compatibility utilities for engine APIs.
    /// </summary>
    public static class EngineCompatibility
    {
        /// <summary>
        /// Math functions compatibility.
        /// </summary>
        public static class MathFunctions
        {
            public static float Clamp(float value, float min, float max)
            {
                return System.Math.Max(min, System.Math.Min(max, value));
            }

            public static float Lerp(float a, float b, float t)
            {
                return a + (b - a) * t;
            }

            public static float Distance(Vector3 a, Vector3 b)
            {
                return (float)System.Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y) + (a.Z - b.Z) * (a.Z - b.Z));
            }
        }

        /// <summary>
        /// Vector3 compatibility helpers.
        /// </summary>
        public static class Vector3Helpers
        {
            public static Vector3 Zero => new Vector3(0f, 0f, 0f);
            public static Vector3 One => new Vector3(1f, 1f, 1f);
            public static Vector3 UnitX => new Vector3(1f, 0f, 0f);
            public static Vector3 UnitY => new Vector3(0f, 1f, 0f);
            public static Vector3 UnitZ => new Vector3(0f, 0f, 1f);

            public static float Distance(Vector3 a, Vector3 b)
            {
                return MathFunctions.Distance(a, b);
            }

            public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
            {
                return new Vector3(
                    MathFunctions.Lerp(a.X, b.X, t),
                    MathFunctions.Lerp(a.Y, b.Y, t),
                    MathFunctions.Lerp(a.Z, b.Z, t)
                );
            }
        }

        /// <summary>
        /// Color compatibility helpers.
        /// </summary>
        public static class ColorHelpers
        {
            public static Color FromArgb(int r, int g, int b) => Color.FromArgb(255, r, g, b);
            public static Color FromArgb(int a, int r, int g, int b) => Color.FromArgb(a, r, g, b);
            public static Color FromRgb(int r, int g, int b) => Color.FromArgb(255, r, g, b);
        }

        /// <summary>
        /// Common missing properties and methods.
        /// </summary>
        public static class CommonProperties
        {
            public static float GetDefaultRange() => 100f;
            public static float GetDefaultDamage() => 10f;
            public static float GetDefaultFireRate() => 1f;
            public static int GetDefaultCost() => 100;
            public static float GetDefaultHealth() => 100f;
            public static float GetDefaultSpeed() => 1f;
        }
    }

    /// <summary>
    /// Extensions for common types.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Safe ToString() for potentially null objects.
        /// </summary>
        public static string SafeToString(this object obj) => obj?.ToString() ?? string.Empty;

        /// <summary>
        /// Get value or default for nullable types.
        /// </summary>
        public static T ValueOrDefault<T>(this T? nullable, T defaultValue = default) where T : struct
        {
            return nullable ?? defaultValue;
        }

        /// <summary>
        /// Clamp a value between min and max.
        /// </summary>
        public static float Clamp(this float value, float min, float max)
        {
            return EngineCompatibility.MathFunctions.Clamp(value, min, max);
        }
    }
}
