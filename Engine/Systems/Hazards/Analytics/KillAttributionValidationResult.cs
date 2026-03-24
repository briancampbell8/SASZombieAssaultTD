using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Systems.Hazards.Analytics
{
    /// <summary>
    /// Base class for validation results in hazard analytics.
    /// </summary>
    public abstract class AnalyticsValidationResult
    {
        public bool IsValid { get; set; } = true;
        public TimeSpan ValidationDuration { get; set; }
        public DateTime ValidationTime { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Validation result for kill attribution analytics.
    /// </summary>
    public class KillAttributionValidationResult : AnalyticsValidationResult
    {
        public List<string> Warnings { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public float ValidationScore { get; set; }
        public int TotalKills { get; set; }
        public float TotalDamage { get; set; }
        public int UniqueHazards { get; set; }
        public List<KillContribution> Contributions { get; set; } = new();

        public void AddWarning(string message)
        {
            Warnings.Add(message);
            IsValid = false;
        }

        public void AddError(string message)
        {
            Errors.Add(message);
            IsValid = false;
        }

        public void CalculateScore()
        {
            ValidationScore = Errors.Count == 0 ? 100f : System.Math.Max(0f, 100f - (Errors.Count * 10f));
        }
    }

    /// <summary>
    /// Represents a single kill contribution in attribution analysis.
    /// </summary>
    public class KillContribution
    {
        public int HazardId { get; set; }
        public string HazardType { get; set; }
        public int KillCount { get; set; }
        public float DamageDealt { get; set; }
        public float ContributionPercentage { get; set; }
        public DateTime FirstKill { get; set; }
        public DateTime LastKill { get; set; }

        public KillContribution()
        {
            FirstKill = DateTime.Now;
            LastKill = DateTime.Now;
        }

        public KillContribution Clone()
        {
            return new KillContribution
            {
                HazardId = this.HazardId,
                HazardType = this.HazardType,
                KillCount = this.KillCount,
                DamageDealt = this.DamageDealt,
                ContributionPercentage = this.ContributionPercentage,
                FirstKill = this.FirstKill,
                LastKill = this.LastKill
            };
        }
    }

    /// <summary>
    /// Validation result for occupancy tracking analytics.
    /// </summary>
    public class OccupancyTrackingValidationResult : AnalyticsValidationResult
    {
        public List<string> Warnings { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public float ValidationScore { get; set; }
        public List<OccupancySample> Samples { get; set; } = new();
        public int TotalSamples { get; set; }
        public TimeSpan TrackingDuration { get; set; }

        public void AddWarning(string message)
        {
            Warnings.Add(message);
            IsValid = false;
        }

        public void AddError(string message)
        {
            Errors.Add(message);
            IsValid = false;
        }

        public void CalculateScore()
        {
            ValidationScore = Errors.Count == 0 ? 100f : System.Math.Max(0f, 100f - (Errors.Count * 10f));
        }
    }

    /// <summary>
    /// Represents a single occupancy sample in tracking analytics.
    /// </summary>
    public class OccupancySample
    {
        public DateTime SampleTime { get; set; } = DateTime.Now;
        public float OccupancyPercentage { get; set; }
        public int EnemyCount { get; set; }
        public int HazardCount { get; set; }
        public float TotalArea { get; set; }
        public float OccupiedArea { get; set; }

        public OccupancySample Clone()
        {
            return new OccupancySample
            {
                SampleTime = this.SampleTime,
                OccupancyPercentage = this.OccupancyPercentage,
                EnemyCount = this.EnemyCount,
                HazardCount = this.HazardCount,
                TotalArea = this.TotalArea,
                OccupiedArea = this.OccupiedArea
            };
        }
    }

    /// <summary>
    /// Validation result for effectiveness scoring analytics.
    /// </summary>
    public class EffectivenessScoringValidationResult : AnalyticsValidationResult
    {
        public List<string> Warnings { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public float ValidationScore { get; set; }
        public Dictionary<string, float> EffectivenessScores { get; set; } = new();
        public float OverallEffectiveness { get; set; }

        public void AddWarning(string message)
        {
            Warnings.Add(message);
            IsValid = false;
        }

        public void AddError(string message)
        {
            Errors.Add(message);
            IsValid = false;
        }

        public void CalculateScore()
        {
            ValidationScore = Errors.Count == 0 ? 100f : System.Math.Max(0f, 100f - (Errors.Count * 10f));
        }
    }

    /// <summary>
    /// Represents a hazard occupancy tracker for analytics.
    /// </summary>
    public class HazardOccupancyTracker
    {
        public int HazardId { get; set; }
        public string HazardType { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; }
        public float OccupiedArea { get; set; }
        public int EnemyCount { get; set; }
        public bool IsActive { get; set; } = true;

        public TimeSpan Duration => EndTime - StartTime;

        public HazardOccupancyTracker Clone()
        {
            return new HazardOccupancyTracker
            {
                HazardId = this.HazardId,
                HazardType = this.HazardType,
                StartTime = this.StartTime,
                EndTime = this.EndTime,
                OccupiedArea = this.OccupiedArea,
                EnemyCount = this.EnemyCount,
                IsActive = this.IsActive
            };
        }
    }
}
