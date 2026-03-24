/*
File:    Math.cs
Purpose: Math namespace for SAS Zombie Assault TD.
Features: Provides Max, Min, Abs, Clamp, and Sqrt functions.

P11-04-07-B: Math namespace for engine mathematical operations.
*/

using System;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Math
{
    /// <summary>
    /// Mathematical utility functions for the engine.
    /// </summary>
    public static class Math
    {
        // Max operations
        public static float Max(float a, float b) => a > b ? a : b;
        public static int Max(int a, int b) => a > b ? a : b;
        public static double Max(double a, double b) => a > b ? a : b;

        // Min operations
        public static float Min(float a, float b) => a < b ? a : b;
        public static int Min(int a, int b) => a < b ? a : b;
        public static double Min(double a, double b) => a < b ? a : b;

        // Absolute value operations
        public static float Abs(float value) => System.MathF.Abs(value);
        public static int Abs(int value) => System.Math.Abs(value);
        public static double Abs(double value) => System.Math.Abs(value);

        // Clamp operations
        public static float Clamp(float value, float min, float max) => value < min ? min : value > max ? max : value;
        public static int Clamp(int value, int min, int max) => value < min ? min : value > max ? max : value;
        public static double Clamp(double value, double min, double max) => value < min ? min : value > max ? max : value;

        // Square root operations
        public static float Sqrt(float value) => System.MathF.Sqrt(value);
        public static double Sqrt(double value) => System.Math.Sqrt(value);

        // Power operations
        public static float Pow(float baseValue, float exponent) => System.MathF.Pow(baseValue, exponent);
        public static double Pow(double baseValue, double exponent) => System.Math.Pow(baseValue, exponent);
    }
}
