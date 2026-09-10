// ====================================================================================================
//  FILE: EngineRandom.cs
//  PATH: ./Engine/Core/Random/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the EngineRandom module.
//
//  RESPONSIBILITIES:
//      - Provide Value() behavior for the Core subsystem.
//      - Provide ValueFloat() behavior for the Core subsystem.
//      - Provide Range() behavior for the Core subsystem.
//      - Provide Range() behavior for the Core subsystem.
//      - Provide Range() behavior for the Core subsystem.
//      - Provide Bool() behavior for the Core subsystem.
//      - Provide Bool() behavior for the Core subsystem.
//      - Provide Sign() behavior for the Core subsystem.
//      - Provide Normal() behavior for the Core subsystem.
//      - Provide Normal() behavior for the Core subsystem.
//      - Provide Exponential() behavior for the Core subsystem.
//      - Provide Exponential() behavior for the Core subsystem.
//      - Provide Poisson() behavior for the Core subsystem.
//      - Provide Binomial() behavior for the Core subsystem.
//      - Provide Direction() behavior for the Core subsystem.
//      - Provide Rotation() behavior for the Core subsystem.
//      - Provide EnableDeterministic() behavior for the Core subsystem.
//      - Provide DisableDeterministic() behavior for the Core subsystem.
//      - Provide ResetDeterministic() behavior for the Core subsystem.
//      - Provide CreateInstance() behavior for the Core subsystem.
//      - Provide CreateInstance() behavior for the Core subsystem.
//      - Provide TestRandomness() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    EngineRandom.cs
Purpose: Unified random number generator for the entire engine with deterministic behavior support.
         Provides comprehensive random utilities for gameplay, procedural generation, and simulations.

Features: Unified random state, deterministic behavior support, various distribution methods,
          seed management, and performance optimization.
          Used by AI, procedural generation, gameplay mechanics, and testing systems.

Created: Engine Core Implementation
Notes:   This is the canonical random system for the entire engine.
         All random operations should use this unified EngineRandom system.
*/

//
using System;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.CoreSize.Random
{
    /// <summary>
    /// Unified random number generator for the entire engine with deterministic behavior support. Used by AI,
    /// procedural generation, gameplay mechanics, and testing systems.
    /// </summary>
    public static class EngineRandom
    {
        /// Private Fields

        private static readonly System.Random _globalRandom = new System.Random();
        private static System.Random? _deterministicRandom;
        private static int? _deterministicSeed;
        private static readonly System.Random _rng = new System.Random();
        private static bool _useDeterministic = false;

        //Thread-local random for performance
        [ThreadStatic] private static System.Random? _threadLocalRandom;

        private static object TheType;
        private static object TheMember;

        ///

        /// Public Properties

        /// <summary>
        /// Whether deterministic random generation is currently enabled.
        /// </summary>
        public static bool IsDeterministic => _useDeterministic;

        /// <summary>
        /// Current deterministic seed (null if not deterministic).
        /// </summary>
        public static int? DeterministicSeed => _deterministicSeed;

        ///

        /// Basic Random Methods

        /// <summary>
        /// Returns a random floating-point number between 0.0 and 1.0 (inclusive).
        /// </summary>
        /// <returns>Random value in range [0.0, 1.0].</returns>
        public static double Value()
        {
            return GetCurrentRandom().NextDouble();
        }

        /// <summary>
        /// Returns a random floating-point number between 0.0 and 1.0 (inclusive).
        /// </summary>
        /// <returns>Random value in range [0.0, 1.0].</returns>
        public static float ValueFloat()
        {
            return (float)GetCurrentRandom().NextDouble();
        }

        /// <summary>
        /// Returns a random integer between minInclusive and maxExclusive.
        /// </summary>
        /// <param name="minInclusive">Inclusive minimum value.</param>
        /// <param name="maxExclusive">Exclusive maximum value.</param>
        /// <returns>Random integer in range [minInclusive, maxExclusive).</returns>
        public static int Range(int minInclusive, int maxExclusive)
        {
            return GetCurrentRandom().Next(minInclusive, maxExclusive);
        }

        /// <summary>
        /// Returns a random floating-point number between minInclusive and maxExclusive.
        /// </summary>
        /// <param name="minInclusive">Inclusive minimum value.</param>
        /// <param name="maxExclusive">Exclusive maximum value.</param>
        /// <returns>Random float in range [minInclusive, maxExclusive).</returns>
        public static float Range(float minInclusive, float maxExclusive)
        {
            return (float)(minInclusive + GetCurrentRandom().NextDouble() * (maxExclusive - minInclusive));
        }

        /// <summary>
        /// Returns a random floating-point number between minInclusive and maxExclusive.
        /// </summary>
        /// <param name="minInclusive">Inclusive minimum value.</param>
        /// <param name="maxExclusive">Exclusive maximum value.</param>
        /// <returns>Random double in range [minInclusive, maxExclusive).</returns>
        public static double Range(double minInclusive, double maxExclusive)
        {
            return minInclusive + GetCurrentRandom().NextDouble() * (maxExclusive - minInclusive);
        }

        /// <summary>
        /// Returns a random boolean value.
        /// </summary>
        /// <returns>Random true or false.</returns>
        public static bool Bool()
        {
            return GetCurrentRandom().Next(0, 2) == 1;
        }

        /// <summary>
        /// Returns true with the specified probability.
        /// </summary>
        /// <param name="probability">Probability of returning true (0.0 to 1.0).</param>
        /// <returns>True with the specified probability.</returns>
        public static bool Bool(float probability)
        {
            return ValueFloat() < System.Math.Clamp(probability, 0f, 1f);
        }

        /// <summary>
        /// Returns a random sign (-1 or 1).
        /// </summary>
        /// <returns>Random sign.</returns>
        public static int Sign()
        {
            return GetCurrentRandom().Next(0, 2) == 0 ? -1 : 1;
        }

        ///

        /// Distribution Methods

        /// <summary>
        /// Returns a random value from a normal (Gaussian) distribution.
        /// </summary>
        /// <param name="mean">Mean of the distribution.</param>
        /// <param name="standardDeviation">Standard deviation of the distribution.</param>
        /// <returns>Random value from normal distribution.</returns>
        public static double Normal(double mean = 0.0, double standardDeviation = 1.0)
        {
            //Box-Muller transform
            var u1 = GetCurrentRandom().NextDouble();
            var u2 = GetCurrentRandom().NextDouble();
            var randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Sin(2.0 * System.Math.PI * u2);
            return mean + randStdNormal * standardDeviation;
        }

        /// <summary>
        /// Returns a random value from a normal (Gaussian) distribution.
        /// </summary>
        /// <param name="mean">Mean of the distribution.</param>
        /// <param name="standardDeviation">Standard deviation of the distribution.</param>
        /// <returns>Random value from normal distribution.</returns>
        public static float Normal(float mean = 0f, float standardDeviation = 1f)
        {
            return (float)Normal((double)mean, (double)standardDeviation);
        }

        /// <summary>
        /// Returns a random value from an exponential distribution.
        /// </summary>
        /// <param name="lambda">Rate parameter (inverse of mean).</param>
        /// <returns>Random value from exponential distribution.</returns>
        public static double Exponential(double lambda = 1.0)
        {
            if (lambda <= 0.0)
                throw new ArgumentException("Lambda must be positive", nameof(lambda));

            return -System.Math.Log(1.0 - GetCurrentRandom().NextDouble()) / lambda;
        }

        /// <summary>
        /// Returns a random value from an exponential distribution.
        /// </summary>
        /// <param name="lambda">Rate parameter (inverse of mean).</param>
        /// <returns>Random value from exponential distribution.</returns>
        public static float Exponential(float lambda = 1f)
        {
            return (float)Exponential((double)lambda);
        }

        /// <summary>
        /// Returns a random value from a Poisson distribution.
        /// </summary>
        /// <param name="lambda">Expected value (rate parameter).</param>
        /// <returns>Random value from Poisson distribution.</returns>
        public static int Poisson(double lambda)
        {
            if (lambda <= 0.0)
                throw new ArgumentException("Lambda must be positive", nameof(lambda));

            var L = System.Math.Exp(-lambda);
            var k = 0;
            var p = 1.0;

            do
            {
                k++;
                p *= GetCurrentRandom().NextDouble();
            } while (p > L);

            return k - 1;
        }

        /// <summary>
        /// Returns a random value from a binomial distribution.
        /// </summary>
        /// <param name="trials">Number of trials.</param>
        /// <param name="probability">Probability of success in each trial.</param>
        /// <returns>Random value from binomial distribution.</returns>
        public static int Binomial(int trials, double probability)
        {
            if (trials < 0)
                throw new ArgumentException("Trials must be non-negative", nameof(trials));
            if (probability < 0.0 || probability > 1.0)
                throw new ArgumentException("Probability must be between 0 and 1", nameof(probability));

            var successes = 0;
            for (int i = 0; i < trials; i++)
            {
                if (GetCurrentRandom().NextDouble() < probability)
                    successes++;
            }

            return successes;
        }

        ///

        /// Selection Methods

        /// <summary>
        /// Returns a random element from the specified array.
        /// </summary>
        /// <typeparam name="T">Type of elements.</typeparam>
        /// <param name="array">Array to select from.</param>
        /// <returns>Random element from the array.</returns>
        public static T Select<T>(T[] array)
        {
            if (array == null || array.Length == 0)
                throw new ArgumentException("Array cannot be null or empty", nameof(array));

            return array[Range(0, array.Length)];
        }

        /// <summary>
        /// Returns a random element from the specified list.
        /// </summary>
        /// <typeparam name="T">Type of elements.</typeparam>
        /// <param name="list">List to select from.</param>
        /// <returns>Random element from the list.</returns>
        public static T Select<T>(System.Collections.Generic.IList<T> list)
        {
            if (list == null || list.Count == 0)
                throw new ArgumentException("List cannot be null or empty", nameof(list));

            return list[Range(0, list.Count)];
        }

        /// <summary>
        /// Returns a random weighted element from the specified weights and values.
        /// </summary>
        /// <typeparam name="T">Type of elements.</typeparam>
        /// <param name="weights">Weights for each element.</param>
        /// <param name="values">Values corresponding to weights.</param>
        /// <returns>Random weighted element.</returns>
        public static T WeightedSelect<T>(float[] weights, T[] values)
        {
            if (weights == null || values == null)
                throw new ArgumentException("Weights and values cannot be null");
            if (weights.Length != values.Length)
                throw new ArgumentException("Weights and values must have the same length");
            if (weights.Length == 0)
                throw new ArgumentException("Weights and values cannot be empty");

            var totalWeight = 0f;
            for (int i = 0; i < weights.Length; i++)
            {
                if (weights[i] < 0f)
                    throw new ArgumentException("Weights must be non-negative");
                totalWeight += weights[i];
            }

            if (totalWeight <= 0f)
                return values[Range(0, values.Length)]; //Fallback to uniform selection

            var randomValue = Range(0f, totalWeight);
            var currentWeight = 0f;

            for (int i = 0; i < weights.Length; i++)
            {
                currentWeight += weights[i];
                if (randomValue < currentWeight)
                    return values[i];
            }

            return values[values.Length - 1]; //Fallback
        }

        /// <summary>
        /// Shuffles the specified array in place.
        /// </summary>
        /// <typeparam name="T">Type of elements.</typeparam>
        /// <param name="array">Array to shuffle.</param>
        public static void Shuffle<T>(T[] array)
        {
            if (array == null)
                return;

            var random = GetCurrentRandom();
            for (int i = array.Length - 1; i > 0; i--)
            {
                var j = random.Next(0, i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }

        /// <summary>
        /// Shuffles the specified list in place.
        /// </summary>
        /// <typeparam name="T">Type of elements.</typeparam>
        /// <param name="list">List to shuffle.</param>
        public static void Shuffle<T>(System.Collections.Generic.IList<T> list)
        {
            if (list == null)
                return;

            var random = GetCurrentRandom();
            for (int i = list.Count - 1; i > 0; i--)
            {
                var j = random.Next(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        /// <summary>
        /// Returns a shuffled copy of the specified array.
        /// </summary>
        /// <typeparam name="T">Type of elements.</typeparam>
        /// <param name="array">Array to shuffle.</param>
        /// <returns>Shuffled copy of the array.</returns>
        public static T[] Shuffled<T>(T[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            var result = new T[array.Length];
            Array.Copy(array, result, array.Length);
            Shuffle(result);
            return result;
        }

        /// <summary>
        /// Returns a shuffled copy of the specified list.
        /// </summary>
        /// <typeparam name="T">Type of elements.</typeparam>
        /// <param name="list">List to shuffle.</param>
        /// <returns>Shuffled copy of the list.</returns>
        public static System.Collections.Generic.List<T> Shuffled<T>(System.Collections.Generic.IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            var result = new System.Collections.Generic.List<T>(list);
            Shuffle(result);
            return result;
        }

        ///

        /// Geometric Methods

        /// <summary>
        /// Returns a random point on a unit circle.
        /// </summary>
        /// <returns>Random point (x, y) on unit circle.</returns>
        public static (float X, float Y) OnUnitCircle()
        {
            var angle = Range(0f, 2f * (float)System.Math.PI);
            return ((float)System.Math.Cos(angle), (float)System.Math.Sin(angle));
        }

        /// <summary>
        /// Returns a random point inside a unit circle.
        /// </summary>
        /// <returns>Random point (x, y) inside unit circle.</returns>
        public static (float X, float Y) InsideUnitCircle()
        {
            var angle = Range(0f, 2f * (float)System.Math.PI);
            var radius = (float)System.Math.Sqrt(ValueFloat()); //Square root for uniform distribution
            return (radius * (float)System.Math.Cos(angle), radius * (float)System.Math.Sin(angle));
        }

        /// <summary>
        /// Returns a random point on a unit sphere.
        /// </summary>
        /// <returns>Random point (x, y, z) on unit sphere.</returns>
        public static (float X, float Y, float Z) OnUnitSphere()
        {
            var theta = Range(0f, 2f * (float)System.Math.PI);
            var phi = (float)System.Math.Acos(2f * ValueFloat() - 1f);

            var x = (float)(System.Math.Sin(phi) * System.Math.Cos(theta));
            var y = (float)(System.Math.Sin(phi) * System.Math.Sin(theta));
            var z = (float)System.Math.Cos(phi);

            return (x, y, z);
        }

        /// <summary>
        /// Returns a random point inside a unit sphere.
        /// </summary>
        /// <returns>Random point (x, y, z) inside unit sphere.</returns>
        public static (float X, float Y, float Z) InsideUnitSphere()
        {
            var theta = Range(0f, 2f * (float)System.Math.PI);
            var phi = (float)System.Math.Acos(2f * ValueFloat() - 1f);
            var radius = (float)System.Math.Pow(ValueFloat(), 1f / 3f); //Cube root for uniform distribution

            var x = radius * (float)(System.Math.Sin(phi) * System.Math.Cos(theta));
            var y = radius * (float)(System.Math.Sin(phi) * System.Math.Sin(theta));
            var z = radius * (float)System.Math.Cos(phi);

            return (x, y, z);
        }

        /// <summary>
        /// Returns a random direction vector (normalized).
        /// </summary>
        /// <returns>Random normalized direction vector.</returns>
        public static VectorMath.Vector3 Direction()
        {
            var (x, y, z) = OnUnitSphere();
            return new VectorMath.Vector3(x, y, z);
        }

        /// <summary>
        /// Returns a random rotation quaternion.
        /// </summary>
        /// <returns>Random rotation quaternion.</returns>
        public static VectorMath.Vector4 Rotation()
        {
            var u1 = ValueFloat();
            var u2 = ValueFloat();
            var u3 = ValueFloat();

            var sqrt1 = (float)System.Math.Sqrt(1f - u1);
            var sqrt2 = (float)System.Math.Sqrt(u1);

            var theta1 = 2f * (float)System.Math.PI * u2;
            var theta2 = 2f * (float)System.Math.PI * u3;

            var w = sqrt1 * (float)System.Math.Sin(theta1);
            var x = sqrt1 * (float)System.Math.Cos(theta1);
            var y = sqrt2 * (float)System.Math.Sin(theta2);
            var z = sqrt2 * (float)System.Math.Cos(theta2);

            return new VectorMath.Vector4(x, y, z, w);
        }

        ///

        /// Deterministic Control

        /// <summary>
        /// Enables deterministic random generation with the specified seed.
        /// </summary>
        /// <param name="seed">Seed for deterministic generation.</param>
        public static void EnableDeterministic(int seed)
        {
            _deterministicSeed = seed;
            _deterministicRandom = new System.Random(seed);
            _useDeterministic = true;
        }

        /// <summary>
        /// Disables deterministic random generation.
        /// </summary>
        public static void DisableDeterministic()
        {
            _useDeterministic = false;
            _deterministicRandom = null;
            _deterministicSeed = null;
        }

        /// <summary>
        /// Resets the deterministic random generator with the current seed.
        /// </summary>
        public static void ResetDeterministic()
        {
            if (_useDeterministic && _deterministicSeed.HasValue)
            {
                _deterministicRandom = new System.Random(_deterministicSeed.Value);
            }
        }

        /// <summary>
        /// Creates a new independent random instance with the specified seed.
        /// </summary>
        /// <param name="seed">Seed for the new random instance.</param>
        /// <returns>New independent random instance.</returns>
        public static System.Random CreateInstance(int seed)
        {
            return new System.Random(seed);
        }

        /// <summary>
        /// Creates a new independent random instance with a random seed.
        /// </summary>
        /// <returns>New independent random instance.</returns>
        public static System.Random CreateInstance()
        {
            return new System.Random();
        }

        ///

        /// Utility Methods

        /// <summary>
        /// Returns the current random instance based on deterministic settings.
        /// </summary>
        /// <returns>Current random instance.</returns>
        private static System.Random GetCurrentRandom()
        {
            if (_useDeterministic && _deterministicRandom != null)
                return _deterministicRandom;

            //Use thread-local random for better performance
            if (_threadLocalRandom == null)
                _threadLocalRandom = new System.Random();

            return _threadLocalRandom;
        }

        /// <summary>
        /// Tests the randomness quality of the current generator.
        /// </summary>
        /// <param name="sampleSize">Number of samples to generate.</param>
        /// <returns>Statistics about the randomness quality.</returns>
        public static RandomnessStatistics TestRandomness(int sampleSize = 10000)
        {
            var values = new double[sampleSize];
            for (int i = 0; i < sampleSize; i++)
            {
                values[i] = Value();
            }

            //Calculate statistics
            var mean = 0.0;
            var variance = 0.0;
            var min = 1.0;
            var max = 0.0;

            for (int i = 0; i < sampleSize; i++)
            {
                mean += values[i];
                min = System.Math.Min(min, values[i]);
                max = System.Math.Max(max, values[i]);
            }
            mean /= sampleSize;

            for (int i = 0; i < sampleSize; i++)
            {
                variance += (values[i] - mean) * (values[i] - mean);
            }
            variance /= sampleSize;

            return new RandomnessStatistics
            {
                SampleSize = sampleSize,
                Mean = mean,
                Variance = variance,
                StandardDeviation = System.Math.Sqrt(variance),
                Min = min,
                Max = max,
                Range = max - min
            };
        }

        internal static System.Numerics.Vector2 RangeVector2(float v1, float v2)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        ///

        /// Nested Classes

        /// <summary>
        /// Statistics about randomness quality.
        /// </summary>
        public class RandomnessStatistics
        {
            public int SampleSize { get; set; }
            public double Mean { get; set; }
            public double Variance { get; set; }
            public double StandardDeviation { get; set; }
            public double Min { get; set; }
            public double Max { get; set; }
            public double Range { get; set; }

            public override string ToString()
            {
                return $"Randomness Statistics (n={SampleSize}):\n" +
                       $"  Mean: {Mean:F6} (ideal: 0.5)\n" +
                       $"  Variance: {Variance:F6} (ideal: 0.083333)\n" +
                       $"  StdDev: {StandardDeviation:F6} (ideal: 0.288675)\n" +
                       $"  Min: {Min:F6} (ideal: 0.0)\n" +
                       $"  Max: {Max:F6} (ideal: 1.0)\n" +
                       $"  Range: {Range:F6} (ideal: 1.0)";
            }
        }

        ///
    }
}