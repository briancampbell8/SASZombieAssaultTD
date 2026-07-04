/*
File:    HazardKillAttribution.cs
Path:    Engine/HazardsControl/HazardKillAttribution.cs
Purpose:  Tracking hazard contributions to kills.
          Manages damage attribution and kill credit.

Role:     Attribution manager for hazard systems.
          - Records hazard damage to targets
          - Tracks kill credit for hazards
          - Manages damage attribution
          - Supports scoring systems

Notes:    This file isolates scoring logic.
          All attribution logic is centralized here.
          Single responsibility: kill attribution.
*/

using System;
using System.Collections.Generic;
using System.Linq;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.HazardsControl
{
    ///<summary>
    ///Attribution manager for hazard systems.
    ///Tracks hazard contributions to kills and damage.
    ///</summary>
    public class HazardKillAttribution
    {
        private readonly Dictionary<int, HazardDamageRecord> _hazardDamageRecords = new();
        private readonly Dictionary<int, List<HazardKillCredit>> _targetKillRecords = new();
        private readonly Dictionary<int, HazardContributionData> _hazardContributions = new();
        private bool _isInitialized;

        ///<summary>
        ///Initializes the hazard kill attribution system.
        ///</summary>
        public void Init()
        {
            _hazardDamageRecords.Clear();
            _targetKillRecords.Clear();
            _hazardContributions.Clear();
            _isInitialized = true;
        }

        ///<summary>
        ///Records hazard damage to a target.
        ///</summary>
        public void RecordHazardDamage(Hazard hazard, int target, float amount)
        {
            if (!_isInitialized || hazard == null || amount <= 0) return;

            UpdateDamageRecord(hazard, target, amount);
            UpdateKillRecords(hazard, target, amount);
            UpdateHazardContribution(hazard.Id, amount, amount >= 100f); //Placeholder kill condition
        }

        ///<summary>
        ///Gets kill credit for a target.
        ///</summary>
        public List<HazardKillCredit> GetHazardKillCredit(int target) =>
            _isInitialized && _targetKillRecords.TryGetValue(target, out var killCredits)
                ? killCredits
                : new List<HazardKillCredit>();

        ///<summary>
        ///Clears kill records for a target.
        ///</summary>
        public void ClearKillRecords(int target)
        {
            if (!_isInitialized && _targetKillRecords.Remove(target))
            {
                OnKillRecordsCleared?.Invoke(target);
            }
        }

        ///<summary>
        ///Gets hazard contribution data.
        ///</summary>
        public HazardContributionData GetHazardContribution(int hazardId) =>
            _isInitialized && _hazardContributions.TryGetValue(hazardId, out var contribution)
                ? contribution
                : new HazardContributionData { HazardId = hazardId };

        ///<summary>
        ///Gets top contributing hazards.
        ///</summary>
        public List<HazardContributionData> GetTopContributingHazards(int limit = 10) =>
            _isInitialized
                ? _hazardContributions.Values
                    .OrderByDescending(c => c.TotalDamage)
                    .Take(limit)
                    .ToList()
                : new List<HazardContributionData>();

        ///<summary>
        ///Gets kill attribution summary.
        ///</summary>
        public KillAttributionSummary GetKillAttributionSummary()
        {
            if (!_isInitialized) return new KillAttributionSummary();

            var totalKills = _targetKillRecords.Values.Sum(list => list.Count(k => k.WasFinalBlow));
            var totalDamage = _hazardDamageRecords.Values.Sum(r => r.TotalDamage);
            var uniqueTargets = _targetKillRecords.Count;
            var uniqueHazards = _hazardDamageRecords.Count;

            return new KillAttributionSummary
            {
                TotalKills = totalKills,
                TotalDamage = totalDamage,
                UniqueTargetsAffected = uniqueTargets,
                UniqueHazardsUsed = uniqueHazards,
                AverageDamagePerHazard = uniqueHazards > 0 ? totalDamage / uniqueHazards : 0f,
                AverageKillsPerHazard = uniqueHazards > 0 ? (float)totalKills / uniqueHazards : 0f,
                TopHazardType = GetTopHazardType(),
                MostEffectiveHazard = GetMostEffectiveHazard()
            };
        }

        ///<summary>
        ///Gets damage statistics for a hazard.
        ///</summary>
        public HazardDamageStatistics GetHazardDamageStatistics(int hazardId)
        {
            if (!_isInitialized || !_hazardDamageRecords.TryGetValue(hazardId, out var damageRecord))
                return new HazardDamageStatistics();

            return new HazardDamageStatistics
            {
                HazardId = hazardId,
                HazardType = damageRecord.HazardType,
                TotalDamage = damageRecord.TotalDamage,
                TargetsHit = damageRecord.TargetsHit.Count,
                HitCount = damageRecord.HitCount,
                FirstHitTime = damageRecord.FirstHitTime,
                LastHitTime = damageRecord.LastHitTime,
                AverageDamagePerHit = damageRecord.HitCount > 0 ? damageRecord.TotalDamage / damageRecord.HitCount : 0f,
                DamagePerTarget = damageRecord.TargetsHit.Count > 0 ? damageRecord.TotalDamage / damageRecord.TargetsHit.Count : 0f
            };
        }

        ///<summary>
        ///Cleans up the hazard kill attribution system.
        ///</summary>
        public void Cleanup()
        {
            _hazardDamageRecords.Clear();
            _targetKillRecords.Clear();
            _hazardContributions.Clear();
            _isInitialized = false;
        }

        ///<summary>
        ///Event triggered when hazard deals damage.
        ///</summary>
        public event Action<Hazard, int, float> OnHazardDamage;

        ///<summary>
        ///Event triggered when hazard gets a kill.
        ///</summary>
        public event Action<Hazard, int, float> OnHazardKill;

        ///<summary>
        ///Event triggered when kill records are cleared.
        ///</summary>
        public event Action<int> OnKillRecordsCleared;

        /// Private Methods

        private void UpdateDamageRecord(Hazard hazard, int target, float amount)
        {
            if (!_hazardDamageRecords.TryGetValue(hazard.Id, out var damageRecord))
            {
                damageRecord = new HazardDamageRecord
                {
                    HazardId = hazard.Id,
                    HazardType = hazard.Type,
                    TargetsHit = new HashSet<int>(),
                    FirstHitTime = DateTime.Now
                };
                _hazardDamageRecords[hazard.Id] = damageRecord;
            }

            damageRecord.TotalDamage += amount;
            damageRecord.TargetsHit.Add(target);
            damageRecord.LastHitTime = DateTime.Now;
            damageRecord.HitCount++;
        }

        private void UpdateKillRecords(Hazard hazard, int target, float amount)
        {
            if (!_targetKillRecords.TryGetValue(target, out var killCredits))
            {
                killCredits = new List<HazardKillCredit>();
                _targetKillRecords[target] = killCredits;
            }

            var isKill = amount >= 100f; //Placeholder kill condition
            if (isKill)
            {
                killCredits.Add(new HazardKillCredit
                {
                    HazardId = hazard.Id,
                    HazardType = hazard.Type,
                    KillTime = DateTime.Now,
                    DamageAmount = amount,
                    WasFinalBlow = true
                });
                OnHazardKill?.Invoke(hazard, target, amount);
            }
            else
            {
                OnHazardDamage?.Invoke(hazard, target, amount);
            }
        }

        private void UpdateHazardContribution(int hazardId, float damageAmount, bool wasKill)
        {
            if (!_hazardContributions.TryGetValue(hazardId, out var contribution))
            {
                contribution = new HazardContributionData
                {
                    HazardId = hazardId,
                    TargetsHit = new HashSet<int>(),
                    FirstContribution = DateTime.Now
                };
                _hazardContributions[hazardId] = contribution;
            }

            contribution.TotalDamage += damageAmount;
            contribution.LastContribution = DateTime.Now;

            if (wasKill)
            {
                contribution.Kills++;
            }
        }

        private string GetTopHazardType() =>
            _hazardDamageRecords.Count == 0
                ? "None"
                : _hazardDamageRecords.Values
                    .GroupBy(r => r.HazardType)
                    .OrderByDescending(g => g.Sum(r => r.TotalDamage))
                    .First().Key;

        private HazardContributionData GetMostEffectiveHazard() =>
            _hazardContributions.Count == 0
                ? new HazardContributionData()
                : _hazardContributions.Values
                    .OrderByDescending(c => c.Kills > 0 ? (c.TotalDamage / c.Kills) : c.TotalDamage)
                    .First();

        ///
    }

    ///<summary>
    ///Complete hazard damage record for tracking damage attribution.
    ///Provides comprehensive tracking of hazard damage and kill contributions.
    ///</summary>
    public class HazardDamageRecord
    {
        public int HazardId { get; set; }
        public string HazardType { get; set; }
        public float TotalDamage { get; set; }
        public HashSet<int> TargetsHit { get; set; }
        public int HitCount { get; set; }
        public DateTime FirstHitTime { get; set; }
        public DateTime LastHitTime { get; set; }
        public float AverageDamagePerHit => HitCount > 0 ? TotalDamage / HitCount : 0f;
        public TimeSpan ActiveDuration => LastHitTime - FirstHitTime;
        public bool IsActive => DateTime.UtcNow - LastHitTime < TimeSpan.FromMinutes(5);

        ///<summary>
        ///Create a new hazard damage record.
        ///</summary>
        public HazardDamageRecord()
        {
            TargetsHit = new HashSet<int>();
            FirstHitTime = DateTime.UtcNow;
            LastHitTime = DateTime.UtcNow;
        }

        ///<summary>
        ///Create a new hazard damage record.
        ///</summary>
        public HazardDamageRecord(int hazardId, string hazardType)
        {
            HazardId = hazardId;
            HazardType = hazardType;
            TargetsHit = new HashSet<int>();
            FirstHitTime = DateTime.UtcNow;
            LastHitTime = DateTime.UtcNow;
        }

        ///<summary>
        ///Record damage dealt to a target.
        ///</summary>
        ///<param name="targetId">ID of the target hit.</param>
        ///<param name="damageAmount">Amount of damage dealt.</param>
        public void RecordDamage(int targetId, float damageAmount)
        {
            TotalDamage += damageAmount;
            TargetsHit.Add(targetId);
            HitCount++;
            LastHitTime = DateTime.UtcNow;
        }

        ///<summary>
        ///Reset the damage record.
        ///</summary>
        public void Reset()
        {
            TotalDamage = 0f;
            TargetsHit.Clear();
            HitCount = 0;
            FirstHitTime = DateTime.UtcNow;
            LastHitTime = DateTime.UtcNow;
        }

        ///<summary>
        ///Get a summary of this damage record.
        ///</summary>
        public string GetSummary()
        {
            return $"Hazard {HazardId} ({HazardType}): {TotalDamage:F1} total damage, " +
                   $"{HitCount} hits, {TargetsHit.Count} targets, " +
                   $"Avg: {AverageDamagePerHit:F1} per hit";
        }
    }

    public class HazardKillCredit
    {
        public int HazardId { get; set; }
        public string HazardType { get; set; }
        public DateTime KillTime { get; set; }
        public float DamageAmount { get; set; }
        public bool WasFinalBlow { get; set; }
    }

    public class HazardContributionData
    {
        public int HazardId { get; set; }
        public float TotalDamage { get; set; }
        public int Kills { get; set; }
        public HashSet<int> TargetsHit { get; set; }
        public DateTime FirstContribution { get; set; }
        public DateTime LastContribution { get; set; }
    }

    public class KillAttributionSummary
    {
        public int TotalKills { get; set; }
        public float TotalDamage { get; set; }
        public int UniqueTargetsAffected { get; set; }
        public int UniqueHazardsUsed { get; set; }
        public float AverageDamagePerHazard { get; set; }
        public float AverageKillsPerHazard { get; set; }
        public string TopHazardType { get; set; }
        public HazardContributionData MostEffectiveHazard { get; set; }
    }

    public class HazardDamageStatistics
    {
        public int HazardId { get; set; }
        public string HazardType { get; set; }
        public float TotalDamage { get; set; }
        public int TargetsHit { get; set; }
        public int HitCount { get; set; }
        public DateTime FirstHitTime { get; set; }
        public DateTime LastHitTime { get; set; }
        public float AverageDamagePerHit { get; set; }
        public float DamagePerTarget { get; set; }
    }
}

