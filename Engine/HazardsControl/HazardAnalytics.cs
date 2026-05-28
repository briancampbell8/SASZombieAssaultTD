/*
File:    HazardAnalytics.cs
Path:    Engine/HazardsControl/HazardAnalytics.cs
Purpose:  Metrics and summaries for hazard systems.
          Provides comprehensive analytics and performance data.

Role:     Analytics manager for hazard systems.
          - Tracks hazard counts and statistics
          - Calculates effectiveness metrics
          - Provides performance data
          - Supports difficulty scaling

Notes:    This file is used by Waves, AI, and difficulty scaling.
          All analytics logic is centralized here.
          Single responsibility: analytics management.
*/

using SASZombieAssaultTD.Engine.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;

namespace SASZombieAssaultTD.Engine.HazardsControl
{
    /// <summary>
    /// Breakdown of hazards by type.
    /// </summary>
    public class HazardTypeBreakdown
    {
        private static object TheType;
        private static object TheMember;

        public int FireHazards { get; set; }
        public int IceHazards { get; set; }
        public int ElectricHazards { get; set; }
        public int ExplosiveHazards { get; set; }
        public int TotalHazards => FireHazards + IceHazards + ElectricHazards + ExplosiveHazards;

        public HazardTypeBreakdown()
        {
            FireHazards = 0;
            IceHazards = 0;
            ElectricHazards = 0;
            ExplosiveHazards = 0;
        }

        public static implicit operator Dictionary<object, object>(HazardTypeBreakdown v)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Analytics manager for hazard systems.
    /// Provides metrics and summaries for hazard performance.
    /// </summary>
    public class HazardAnalytics
    {
        private readonly Dictionary<string, int> _typeCounts = new();
        private readonly List<HazardLifetimeRecord> _lifetimeRecords = new();
        private readonly Dictionary<int, HazardEffectivenessRecord> _effectivenessRecords = new();
        private bool _isInitialized;

        /// <summary>
        /// Initializes the hazard analytics system.
        /// </summary>
        public void Init()
        {
            _typeCounts.Clear();
            _lifetimeRecords.Clear();
            _effectivenessRecords.Clear();
            _isInitialized = true;
        }

        /// <summary>
        /// Gets the total hazard count.
        /// </summary>
        public int TotalHazardCount => _isInitialized ? _typeCounts.Values.Sum() : 0;

        /// <summary>
        /// Gets the active hazard count.
        /// </summary>
        public int ActiveHazardCount => _isInitialized ? _effectivenessRecords.Values.Count(r => r.IsActive) : 0;

        /// <summary>
        /// Gets the hazard type breakdown.
        /// </summary>
        public Dictionary<string, int> TypeBreakdown => _isInitialized
            ? new Dictionary<string, int>(_typeCounts)
            : new Dictionary<string, int>();

        /// <summary>
        /// Gets the average hazard lifetime.
        /// </summary>
        public float AverageHazardLifetime => _isInitialized && _lifetimeRecords.Any()
            ? _lifetimeRecords.Average(r => r.LifetimeSeconds)
            : 0f;

        /// <summary>
        /// Gets the average effectiveness score across all hazards.
        /// </summary>
        public float AverageEffectivenessScore => _isInitialized && _effectivenessRecords.Any()
            ? _effectivenessRecords.Values.Average(r => r.EffectivenessScore)
            : 0f;

        /// <summary>
        /// Records hazard creation for analytics.
        /// </summary>
        public void RecordHazardCreation(Hazard hazard)
        {
            if (!_isInitialized || hazard == null) return;

            _typeCounts[hazard.Type] = _typeCounts.GetValueOrDefault(hazard.Type, 0) + 1;

            _effectivenessRecords[hazard.Id] = new HazardEffectivenessRecord
            {
                HazardId = hazard.Id,
                HazardType = hazard.Type,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            OnHazardRecorded?.Invoke(hazard);
        }

        /// <summary>
        /// Records hazard destruction for analytics.
        /// </summary>
        public void RecordHazardDestruction(Hazard hazard)
        {
            if (!_isInitialized || hazard == null) return;

            var lifetime = (float)(DateTime.Now - hazard.CreatedAt).TotalSeconds;

            _lifetimeRecords.Add(new HazardLifetimeRecord
            {
                HazardId = hazard.Id,
                HazardType = hazard.Type,
                LifetimeSeconds = lifetime,
                DestructionReason = hazard.State == HazardState.Expired ? "Expired" : "Destroyed"
            });

            if (_effectivenessRecords.TryGetValue(hazard.Id, out var record))
            {
                record.IsActive = false;
                record.DestroyedAt = DateTime.Now;
                record.FinalLifetime = lifetime;
                record.EffectivenessScore = CalculateEffectivenessScore(record);
            }

            OnHazardDestructionRecorded?.Invoke(hazard);
        }

        /// <summary>
        /// Records hazard damage for analytics.
        /// </summary>
        public void RecordHazardDamage(int hazardId, float damageAmount, int enemiesAffected)
        {
            if (!_isInitialized || !_effectivenessRecords.TryGetValue(hazardId, out var record)) return;

            record.DamageDealt += damageAmount;
            record.EnemiesAffected += enemiesAffected;
            record.EffectivenessScore = CalculateEffectivenessScore(record);
        }

        /// <summary>
        /// Gets detailed analytics for a hazard type.
        /// </summary>
        public HazardTypeAnalytics GetHazardTypeAnalytics(string hazardType)
        {
            if (!_isInitialized) return new HazardTypeAnalytics();

            var typeRecords = _effectivenessRecords.Values.Where(r => r.HazardType == hazardType).ToList();
            var typeLifetimeRecords = _lifetimeRecords.Where(r => r.HazardType == hazardType).ToList();

            return new HazardTypeAnalytics
            {
                HazardType = hazardType,
                TotalCreated = _typeCounts.GetValueOrDefault(hazardType, 0),
                CurrentlyActive = typeRecords.Count(r => r.IsActive),
                AverageLifetime = typeLifetimeRecords.Any() ? typeLifetimeRecords.Average(r => r.LifetimeSeconds) : 0f,
                AverageEffectiveness = typeRecords.Any() ? typeRecords.Average(r => r.EffectivenessScore) : 0f,
                TotalDamageDealt = typeRecords.Sum(r => r.DamageDealt),
                TotalEnemiesAffected = typeRecords.Sum(r => r.EnemiesAffected)
            };
        }

        /// <summary>
        /// Gets comprehensive analytics summary.
        /// </summary>
        public HazardAnalyticsSummary GetComprehensiveSummary(Dictionary<string, int> hazardTypeBreakdown)
        {
            if (!_isInitialized) return new HazardAnalyticsSummary();

            return new HazardAnalyticsSummary
            {
                TotalHazardsCreated = TotalHazardCount,
                CurrentlyActiveHazards = ActiveHazardCount,
                TypeBreakdown = hazardTypeBreakdown,
                AverageLifetime = AverageHazardLifetime,
                AverageEffectiveness = AverageEffectivenessScore,
                MostEffectiveType = GetMostEffectiveType(),
                LeastEffectiveType = GetLeastEffectiveType(),
                TotalDamageDealt = _effectivenessRecords.Values.Sum(r => r.DamageDealt),
                TotalEnemiesAffected = _effectivenessRecords.Values.Sum(r => r.EnemiesAffected)
            };
        }

        /// <summary>
        /// Cleans up the hazard analytics system.
        /// </summary>
        public void Cleanup()
        {
            _typeCounts.Clear();
            _lifetimeRecords.Clear();
            _effectivenessRecords.Clear();
            _isInitialized = false;
        }

        /// <summary>
        /// Event triggered when a hazard is recorded.
        /// </summary>
        public event Action<Hazard> OnHazardRecorded;

        /// <summary>
        /// Event triggered when hazard destruction is recorded.
        /// </summary>
        public event Action<Hazard> OnHazardDestructionRecorded;

        ///  Private Methods

        private float CalculateEffectivenessScore(HazardEffectivenessRecord record)
        {
            if (record.EnemiesAffected == 0) return 0f;

            var enemyScore = System.Math.Min(1.0f, record.EnemiesAffected / 10.0f);
            var damageScore = System.Math.Min(1.0f, record.DamageDealt / 1000.0f);

            return (enemyScore * 0.7f) + (damageScore * 0.3f);
        }

        private string GetMostEffectiveType()
        {
            return _typeCounts.Any()
                ? _typeCounts.OrderByDescending(kvp => GetAverageEffectivenessForType(kvp.Key)).First().Key
                : "None";
        }

        private string GetLeastEffectiveType()
        {
            return _typeCounts.Any()
                ? _typeCounts.OrderBy(kvp => GetAverageEffectivenessForType(kvp.Key)).First().Key
                : "None";
        }

        private float GetAverageEffectivenessForType(string hazardType)
        {
            return _effectivenessRecords.Values
                .Where(r => r.HazardType == hazardType)
                .DefaultIfEmpty(new HazardEffectivenessRecord { EffectivenessScore = 0f })
                .Average(r => r.EffectivenessScore);
        }

        /// 
    }

    /// <summary>
    /// Record of hazard lifetime data.
    /// </summary>
    internal class HazardLifetimeRecord
    {
        public int HazardId { get; set; }
        public string HazardType { get; set; }
        public float LifetimeSeconds { get; set; }
        public string DestructionReason { get; set; }
    }

    /// <summary>
    /// Record of hazard effectiveness data.
    /// </summary>
    internal class HazardEffectivenessRecord
    {
        public int HazardId { get; set; }
        public string HazardType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DestroyedAt { get; set; }
        public bool IsActive { get; set; }
        public float EffectivenessScore { get; set; }
        public int EnemiesAffected { get; set; }
        public float DamageDealt { get; set; }
        public float FinalLifetime { get; set; }
    }

    /// <summary>
    /// Analytics data for a specific hazard type.
    /// </summary>
    public class HazardTypeAnalytics
    {
        public string HazardType { get; set; }
        public int TotalCreated { get; set; }
        public int CurrentlyActive { get; set; }
        public float AverageLifetime { get; set; }
        public float AverageEffectiveness { get; set; }
        public float TotalDamageDealt { get; set; }
        public int TotalEnemiesAffected { get; set; }
    }

    /// <summary>
    /// Comprehensive analytics summary.
    /// </summary>
    public class HazardAnalyticsSummary
    {
        public int TotalHazardsCreated { get; set; }
        public int CurrentlyActiveHazards { get; set; }
        public Dictionary<string, int> TypeBreakdown { get; set; }
        public float AverageLifetime { get; set; }
        public float AverageEffectiveness { get; set; }
        public string MostEffectiveType { get; set; }
        public string LeastEffectiveType { get; set; }
        public float TotalDamageDealt { get; set; }
        public int TotalEnemiesAffected { get; set; }
        
        // Missing properties
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public float EffectivenessScore { get; set; }
    }

    /// <summary>
    /// Hazard occupancy data for analytics.
    /// </summary>
    public class HazardOccupancyData
    {
        public bool IsOverloaded { get; set; }
        public List<string> ActiveHazards { get; set; } = new List<string>();
    }

    /// <summary>
    /// Hazard density data for analytics.
    /// </summary>
    public class HazardDensityData
    {
        private object TheType;
        private object TheMember;

        public float Area { get; set; }
        public int ClusterCount { get; set; }
        public List<string> NearestHazards { get; set; } = new List<string>();
        public bool IsHighRisk { get; set; }

        internal object CalculateHazardDensity(Rectangle area)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        internal void UpdateDensityForNewHazard(Hazard hazard)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }
    }
}
