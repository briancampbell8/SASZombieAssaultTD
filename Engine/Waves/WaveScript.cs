// ====================================================================================================
//  FILE: WaveScript.cs
//  PATH: ./Engine/Waves/
//  MODULE: WaveDirector
//
//  ROLE:
//      Load, validate, and construct wave definitions for the WaveDirector subsystem.
//
//  RESPONSIBILITIES:
//      - Provide GetTotalEnemyCount() behavior for the WaveDirector subsystem.
//      - Provide GetEnemyCount() behavior for the WaveDirector subsystem.
//      - Provide GetEnemyTypes() behavior for the WaveDirector subsystem.
//      - Provide HasBossEnemies() behavior for the WaveDirector subsystem.
//      - Provide GetEstimatedDuration() behavior for the WaveDirector subsystem.
//      - Provide GetDifficultyRating() behavior for the WaveDirector subsystem.
//      - Provide Validate() behavior for the WaveDirector subsystem.
//      - Provide Clone() behavior for the WaveDirector subsystem.
//      - Provide DeepCopy() behavior for the WaveDirector subsystem.
//      - Provide GetSummary() behavior for the WaveDirector subsystem.
//      - Provide GetDetailedInfo() behavior for the WaveDirector subsystem.
//      - Provide Clone() behavior for the WaveDirector subsystem.
//      - Provide Clone() behavior for the WaveDirector subsystem.
//      - Provide Clone() behavior for the WaveDirector subsystem.
//      - Provide Clone() behavior for the WaveDirector subsystem.
//      - Provide AreConditionsMet() behavior for the WaveDirector subsystem.
//      - Provide Clone() behavior for the WaveDirector subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
//
//  CHANGE LOG:
//      P11-08-06: Removed DifficultyMultiplier and replaced with WaveStatMultiplier for
//                 deterministic wave-local stat scaling. No structural changes made.
// ====================================================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Render.Validation;
using static SASZombieAssaultTD.Engine.Enemies.EnemiesEnums;
// using SASZombieAssaultTD.Engine.Extensions; // Extensions Removed

namespace SASZombieAssaultTD.Engine.Waves
{
    public class WaveScript
    {
        public struct DifficultyMultiplier
        {
            internal static float HealthMultiplier;

            public float Health { get; set; }
            public float Damage { get; set; }
            public float Speed { get; set; }
            public float Count { get; set; }
            public float SpawnDelay { get; set; }
            public static float SpeedMultiplier { get; internal set; }
            public static float DamageMultiplier { get; internal set; }
            public static int CountMultiplier { get; internal set; }

            public DifficultyMultiplier()
            {
                Health = 1f;
                Damage = 1f;
                Speed = 1f;
                Count = 1f;
                SpawnDelay = 1f;
            }
        }

        public int WaveNumber { get; set; }
        public string WaveName { get; set; }
        public string Description { get; set; }
        public List<WaveSpawnGroup> SpawnGroups { get; set; }
        public float InterWaveDelay { get; set; }

        // UPDATED: DifficultyMultiplier → WaveStatMultiplier
        public WaveStatMultiplier StatMultiplier { get; set; }

        public WaveModifiers Modifiers { get; set; }
        public WaveRewards Rewards { get; set; }
        public WaveEnvironment Environment { get; set; }
        public object Definition { get; internal set; }

        public WaveScript()
        {
            SpawnGroups = new List<WaveSpawnGroup>();
            InterWaveDelay = 10f;

            // UPDATED
            StatMultiplier = new WaveStatMultiplier();

            Modifiers = new WaveModifiers();
            Rewards = new WaveRewards();
            Environment = new WaveEnvironment();
        }

        public int GetTotalEnemyCount()
        {
            return SpawnGroups.Sum(group => group.Count);
        }

        public int GetEnemyCount(ZombieType enemyType)
        {
            return SpawnGroups
                .Where(group => group.EnemyType == (WaveSpawnGroup.ZombieType)enemyType)
                .Sum(group => group.Count);
        }

        public IEnumerable<ZombieType> GetEnemyTypes()
        {
            return SpawnGroups.Select(group => (ZombieType)group.EnemyType).Distinct();
        }

        public bool HasBossEnemies()
        {
            return SpawnGroups.Any(group => group.IsBoss);
        }

        public float GetEstimatedDuration()
        {
            var duration = 0f;

            foreach (var group in SpawnGroups)
            {
                var spawnTime = (group.Count - 1) * group.SpawnDelay;
                var totalTime = spawnTime + group.DelayAfterGroup;
                duration = System.Math.Max(duration, totalTime);
            }

            return duration;
        }

        public int GetDifficultyRating()
        {
            var rating = 1;
            rating += System.Math.Min(WaveNumber / 5, 5);

            var enemyCount = GetTotalEnemyCount();
            rating += System.Math.Min(enemyCount / 10, 2);

            if (HasBossEnemies())
                rating += 3;

            var specialTypes = GetEnemyTypes().Count(type =>
                type == ZombieType.Shadow ||
                type == ZombieType.Toxic ||
                type == ZombieType.Armored ||
                type == ZombieType.RobotClown ||
                type == ZombieType.Devastator);

            rating += System.Math.Min(specialTypes, 2);

            return System.Math.Min(rating, 10);
        }

        public ValidationResult Validate()
        {
            var result = new ValidationResult { IsValid = true };

            if (WaveNumber <= 0)
            {
                result.IsValid = false;
                result.AddError("Wave number must be positive");
            }

            if (SpawnGroups == null || SpawnGroups.Count == 0)
            {
                result.IsValid = false;
                result.AddError("Wave must have at least one spawn group");
            }
            else
            {
                foreach (var group in SpawnGroups)
                {
                    var groupResult = group.Validate();
                    if (!groupResult.IsValid)
                    {
                        result.IsValid = false;
                        result.AddError($"Invalid spawn group: {groupResult.ErrorMessage}");
                    }
                }
            }

            if (InterWaveDelay < 0)
            {
                result.IsValid = false;
                result.AddError("Inter-wave delay cannot be negative");
            }

            // UPDATED
            if (StatMultiplier == null)
            {
                result.IsValid = false;
                result.AddError("Wave stat multiplier is required");
            }

            return result;
        }

        public WaveScript Clone()
        {
            var clone = new WaveScript
            {
                WaveNumber = this.WaveNumber,
                WaveName = this.WaveName,
                Description = this.Description,
                InterWaveDelay = this.InterWaveDelay,

                // UPDATED
                StatMultiplier = this.StatMultiplier.Clone(),

                Modifiers = this.Modifiers.Clone(),
                Rewards = this.Rewards.Clone(),
                Environment = this.Environment.Clone()
            };

            clone.SpawnGroups = this.SpawnGroups.Select(group => group.Clone()).ToList();

            return clone;
        }

        public WaveScript DeepCopy()
        {
            return Clone();
        }

        public string GetSummary()
        {
            var summary = $"Wave {WaveNumber}: {WaveName}\n";
            summary += $"Enemies: {GetTotalEnemyCount()}\n";
            summary += $"Difficulty: {GetDifficultyRating()}/10\n";
            summary += $"Duration: ~{GetEstimatedDuration():F0}s\n";

            if (HasBossEnemies())
                summary += "Contains boss enemies\n";

            return summary.Trim();
        }

        public string GetDetailedInfo()
        {
            var info = GetSummary() + "\n\n";
            info += "Enemy Composition:\n";

            foreach (var group in SpawnGroups)
            {
                info += $"- {group.EnemyType} x{group.Count}";
                if (group.IsBoss)
                    info += " (BOSS)";
                info += $" - Pattern: {group.Pattern}\n";
            }

            info += $"\nModifiers:\n";
            info += $"- Health: x{StatMultiplier.HealthMultiplier:F2}\n";
            info += $"- Speed: x{StatMultiplier.SpeedMultiplier:F2}\n";
            info += $"- Damage: x{StatMultiplier.DamageMultiplier:F2}\n";

            if (Rewards.CashBonus > 0)
            {
                info += $"\nRewards:\n";
                info += $"- Cash Bonus: ${Rewards.CashBonus}\n";
            }

            return info.Trim();
        }
    }
}
