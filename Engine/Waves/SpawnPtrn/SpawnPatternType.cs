// ====================================================================================================
//  FILE: SpawnPatternType.cs
//  PATH: ./Engine/Waves/
//  MODULE: WaveDirector
//
//  ROLE:
//      Load, validate, and construct wave definitions for the WaveDirector subsystem.
//
//  RESPONSIBILITIES:
//      - Provide GenerateSpawnPositions() behavior for the WaveDirector subsystem.
//      - Provide GetSpawnDelay() behavior for the WaveDirector subsystem.
//      - Provide GetPatternDescription() behavior for the WaveDirector subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Waves
{
    ///<summary>
    ///Comprehensive spawn pattern types for enemy spawning strategies.
    ///Defines how enemies are spawned and positioned during waves.
    ///</summary>
    public enum SpawnPatternType
    {
        ///<summary>
        ///Enemies spawn in a single line from one spawn point.
        ///</summary>
        Line,
        
        ///<summary>
        ///Enemies spawn in a circular formation around a center point.
        ///</summary>
        Circle,
        
        ///<summary>
        ///Enemies spawn in a wave pattern with sinusoidal positioning.
        ///</summary>
        Wave,
        
        ///<summary>
        ///Enemies spawn randomly across available spawn points.
        ///</summary>
        Random,
        
        ///<summary>
        ///Enemies spawn in a cluster formation.
        ///</summary>
        Cluster,
        
        ///<summary>
        ///Enemies spawn in a spread formation across multiple points.
        ///</summary>
        Spread,
        
        ///<summary>
        ///Enemies spawn in a V-shaped formation.
        ///</summary>
        VFormation,
        
        ///<summary>
        ///Enemies spawn in a staggered pattern with time delays.
        ///</summary>
        Staggered,
        
        ///<summary>
        ///Enemies spawn in a pincer movement from multiple sides.
        ///</summary>
        Pincer,
        
        ///<summary>
        ///Enemies spawn in a flanking maneuver from the sides.
        ///</summary>
        Flanking,
        
        ///<summary>
        ///Enemies spawn in a spiral pattern.
        ///</summary>
        Spiral
    }

    ///<summary>
    ///Spawn pattern configuration and execution system.
    ///</summary>
    public static class SpawnPatternExecutor
    {
        ///<summary>
        ///Generate spawn positions based on pattern type.
        ///</summary>
        ///<param name="patternType">Type of spawn pattern</param>
        ///<param name="spawnPoints">Available spawn points</param>
        ///<param name="count">Number of enemies to spawn</param>
        ///<returns>List of spawn positions</returns>
        public static List<Vector3> GenerateSpawnPositions(SpawnPatternType patternType, List<Vector3> spawnPoints, int count)
        {
            if (spawnPoints == null || spawnPoints.Count == 0)
                return new List<Vector3> { Vector3.Zero };

            return patternType switch
            {
                SpawnPatternType.Line => GenerateLinePattern(spawnPoints, count),
                SpawnPatternType.Circle => GenerateCirclePattern(spawnPoints, count),
                SpawnPatternType.Wave => GenerateWavePattern(spawnPoints, count),
                SpawnPatternType.Random => GenerateRandomPattern(spawnPoints, count),
                SpawnPatternType.Cluster => GenerateClusterPattern(spawnPoints, count),
                SpawnPatternType.Spread => GenerateSpreadPattern(spawnPoints, count),
                SpawnPatternType.VFormation => GenerateVFormationPattern(spawnPoints, count),
                SpawnPatternType.Staggered => GenerateStaggeredPattern(spawnPoints, count),
                SpawnPatternType.Pincer => GeneratePincerPattern(spawnPoints, count),
                SpawnPatternType.Spiral => GenerateSpiralPattern(spawnPoints, count),
                _ => GenerateLinePattern(spawnPoints, count)
            };
        }

        ///<summary>
        ///Generate line pattern spawn positions.
        ///</summary>
        private static List<Vector3> GenerateLinePattern(List<Vector3> spawnPoints, int count)
        {
            var positions = new List<Vector3>();
            var basePoint = spawnPoints[new Random().Next(0, spawnPoints.Count)];
            
            for (int i = 0; i < count; i++)
            {
                var offset = new Vector3(i * 1.5f, 0, 0);
                positions.Add(basePoint + offset);
            }
            
            return positions;
        }

        ///<summary>
        ///Generate circular pattern spawn positions.
        ///</summary>
        private static List<Vector3> GenerateCirclePattern(List<Vector3> spawnPoints, int count)
        {
            var positions = new List<Vector3>();
            var center = spawnPoints[new Random().Next(0, spawnPoints.Count)];
            var radius = System.Math.Min(count * 0.8f, 8f);
            
            for (int i = 0; i < count; i++)
            {
                var angle = (2 * System.Math.PI * i) / count;
                var x = (float)(radius * System.Math.Cos(angle));
                var z = (float)(radius * System.Math.Sin(angle));
                positions.Add(center + new Vector3(x, 0, z));
            }
            
            return positions;
        }

        ///<summary>
        ///Generate wave pattern spawn positions.
        ///</summary>
        private static List<Vector3> GenerateWavePattern(List<Vector3> spawnPoints, int count)
        {
            var positions = new List<Vector3>();
            var basePoint = spawnPoints[new Random().Next(0, spawnPoints.Count)];
            
            for (int i = 0; i < count; i++)
            {
                var waveOffset = (float)System.Math.Sin(i * 0.5) * 2f;
                var offset = new Vector3(i * 1.2f, 0, waveOffset);
                positions.Add(basePoint + offset);
            }
            
            return positions;
        }

        ///<summary>
        ///Generate random pattern spawn positions.
        ///</summary>
        private static List<Vector3> GenerateRandomPattern(List<Vector3> spawnPoints, int count)
        {
            var positions = new List<Vector3>();
            var random = new Random();
            
            for (int i = 0; i < count; i++)
            {
                var spawnPoint = spawnPoints[random.Next(0, spawnPoints.Count)];
                var randomOffset = new Vector3(
                    (float)(random.NextDouble() * 4 - 2),
                    0,
                    (float)(random.NextDouble() * 4 - 2)
                );
                positions.Add(spawnPoint + randomOffset);
            }
            
            return positions;
        }

        ///<summary>
        ///Generate cluster pattern spawn positions.
        ///</summary>
        private static List<Vector3> GenerateClusterPattern(List<Vector3> spawnPoints, int count)
        {
            var positions = new List<Vector3>();
            var centerPoint = spawnPoints[new Random().Next(0, spawnPoints.Count)];
            var clusterRadius = System.Math.Min(System.Math.Sqrt(count) * 0.5f, 3f);
            
            for (int i = 0; i < count; i++)
            {
                var angle = random.NextDouble() * 2 * System.Math.PI;
                var distance = random.NextDouble() * clusterRadius;
                var x = (float)(distance * System.Math.Cos(angle));
                var z = (float)(distance * System.Math.Sin(angle));
                positions.Add(centerPoint + new Vector3(x, 0, z));
            }
            
            return positions;
        }

        ///<summary>
        ///Generate spread pattern spawn positions.
        ///</summary>
        private static List<Vector3> GenerateSpreadPattern(List<Vector3> spawnPoints, int count)
        {
            var positions = new List<Vector3>();
            var pointsPerSpawn = System.Math.Max(1, count / spawnPoints.Count);
            
            for (int i = 0; i < count; i++)
            {
                var spawnIndex = i % spawnPoints.Count;
                var spawnPoint = spawnPoints[spawnIndex];
                var spreadOffset = (float)(i / spawnPoints.Count) * 2f;
                positions.Add(spawnPoint + new Vector3(spreadOffset, 0, 0));
            }
            
            return positions;
        }

        ///<summary>
        ///Generate V-formation pattern spawn positions.
        ///</summary>
        private static List<Vector3> GenerateVFormationPattern(List<Vector3> spawnPoints, int count)
        {
            var positions = new List<Vector3>();
            var basePoint = spawnPoints[new Random().Next(0, spawnPoints.Count)];
            var random = new Random();
            
            for (int i = 0; i < count; i++)
            {
                var side = i % 2 == 0 ? 1 : -1;
                var distance = (i / 2) * 1.5f;
                var offset = new Vector3(distance, 0, side * distance * 0.5f);
                positions.Add(basePoint + offset);
            }
            
            return positions;
        }

        ///<summary>
        ///Generate staggered pattern spawn positions.
        ///</summary>
        private static List<Vector3> GenerateStaggeredPattern(List<Vector3> spawnPoints, int count)
        {
            var positions = new List<Vector3>();
            var basePoint = spawnPoints[new Random().Next(0, spawnPoints.Count)];
            
            for (int i = 0; i < count; i++)
            {
                var row = i / 3;
                var col = i % 3;
                var stagger = row % 2 == 1 ? 1.5f : 0f;
                var offset = new Vector3(col * 2f + stagger, 0, row * 1.5f);
                positions.Add(basePoint + offset);
            }
            
            return positions;
        }

        ///<summary>
        ///Generate pincer movement pattern spawn positions.
        ///</summary>
        private static List<Vector3> GeneratePincerPattern(List<Vector3> spawnPoints, int count)
        {
            var positions = new List<Vector3>();
            
            if (spawnPoints.Count < 2)
                return GenerateLinePattern(spawnPoints, count);
            
            var leftPoint = spawnPoints[0];
            var rightPoint = spawnPoints[spawnPoints.Count - 1];
            
            for (int i = 0; i < count; i++)
            {
                var point = i % 2 == 0 ? leftPoint : rightPoint;
                var offset = new Vector3((i / 2) * 1.5f, 0, 0);
                positions.Add(point + offset);
            }
            
            return positions;
        }

        ///<summary>
        ///Generate spiral pattern spawn positions.
        ///</summary>
        private static List<Vector3> GenerateSpiralPattern(List<Vector3> spawnPoints, int count)
        {
            var positions = new List<Vector3>();
            var center = spawnPoints[new Random().Next(0, spawnPoints.Count)];
            
            for (int i = 0; i < count; i++)
            {
                var angle = i * 0.5;
                var radius = i * 0.3f;
                var x = (float)(radius * System.Math.Cos(angle));
                var z = (float)(radius * System.Math.Sin(angle));
                positions.Add(center + new Vector3(x, 0, z));
            }
            
            return positions;
        }

        ///<summary>
        ///Get spawn delay pattern for timing between spawns.
        ///</summary>
        ///<param name="patternType">Spawn pattern type</param>
        ///<param name="index">Enemy index in spawn order</param>
        ///<param name="baseDelay">Base delay between spawns</param>
        ///<returns>Delay in seconds for this spawn</returns>
        public static float GetSpawnDelay(SpawnPatternType patternType, int index, float baseDelay = 1.0f)
        {
            return patternType switch
            {
                SpawnPatternType.Line => index * baseDelay,
                SpawnPatternType.Circle => index * baseDelay * 0.5f,
                SpawnPatternType.Wave => index * baseDelay * 0.8f,
                SpawnPatternType.Random => (float)(new Random().NextDouble() * baseDelay * 2),
                SpawnPatternType.Cluster => (index / 3) * baseDelay,
                SpawnPatternType.Spread => index * baseDelay * 0.6f,
                SpawnPatternType.VFormation => index * baseDelay * 0.7f,
                SpawnPatternType.Staggered => index * baseDelay * 1.2f,
                SpawnPatternType.Pincer => index * baseDelay * 0.9f,
                SpawnPatternType.Spiral => index * baseDelay * 0.4f,
                _ => index * baseDelay
            };
        }

        ///<summary>
        ///Get pattern description for UI display.
        ///</summary>
        ///<param name="patternType">Spawn pattern type</param>
        ///<returns>Human-readable description</returns>
        public static string GetPatternDescription(SpawnPatternType patternType)
        {
            return patternType switch
            {
                SpawnPatternType.Line => "Enemies spawn in a straight line",
                SpawnPatternType.Circle => "Enemies spawn in a circular formation",
                SpawnPatternType.Wave => "Enemies spawn in a wave pattern",
                SpawnPatternType.Random => "Enemies spawn randomly across the map",
                SpawnPatternType.Cluster => "Enemies spawn in tight clusters",
                SpawnPatternType.Spread => "Enemies spread across multiple spawn points",
                SpawnPatternType.VFormation => "Enemies spawn in a V-shaped formation",
                SpawnPatternType.Staggered => "Enemies spawn with staggered timing",
                SpawnPatternType.Pincer => "Enemies attack from multiple directions",
                SpawnPatternType.Spiral => "Enemies spawn in a spiral pattern",
                _ => "Unknown spawn pattern"
            };
        }

        private static readonly Random random = new Random();
    }
}

