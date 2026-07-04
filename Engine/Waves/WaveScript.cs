using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Enemies;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Waves
{
    ///<summary>
    ///Wave script for SAS Zombie Assault TD.
    ///Defines enemy composition, timing, and patterns for a single wave.
    ///</summary>
    public class WaveScript
    {
        public int WaveNumber { get; set; }
        public string WaveName { get; set; }
        public string Description { get; set; }
        public List<WaveSpawnGroup> SpawnGroups { get; set; }
        public float InterWaveDelay { get; set; }
        public DifficultyMultiplier DifficultyMultiplier { get; set; }
        public WaveModifiers Modifiers { get; set; }
        public WaveRewards Rewards { get; set; }
        public WaveEnvironment Environment { get; set; }

        public WaveScript()
        {
            SpawnGroups = new List<WaveSpawnGroup>();
            InterWaveDelay = 10f;
            DifficultyMultiplier = new DifficultyMultiplier();
            Modifiers = new WaveModifiers();
            Rewards = new WaveRewards();
            Environment = new WaveEnvironment();
        }

        ///<summary>
        ///Get total enemy count for this wave.
        ///</summary>
        ///<returns>Total number of enemies.</returns>
        public int GetTotalEnemyCount()
        {
            return SpawnGroups.Sum(group => group.Count);
        }

        ///<summary>
        ///Get total count of specific enemy type.
        ///</summary>
        ///<param name="enemyType">Enemy type to count.</param>
        ///<returns>Number of enemies of specified type.</returns>
        public int GetEnemyCount(ZombieType enemyType)
        {
            return SpawnGroups
                .Where(group => group.EnemyType == (WaveSpawnGroup.ZombieType)enemyType)
                .Sum(group => group.Count);
        }

        ///<summary>
        ///Get all enemy types in this wave.
        ///</summary>
        ///<returns>Collection of enemy types.</returns>
        public IEnumerable<ZombieType> GetEnemyTypes()
        {
            return SpawnGroups.Select(group => (SASZombieAssaultTD.Engine.Enemies.ZombieType)group.EnemyType).Distinct();
        }

        ///<summary>
        ///Check if wave contains boss enemies.
        ///</summary>
        ///<returns>True if wave contains boss enemies.</returns>
        public bool HasBossEnemies()
        {
            return SpawnGroups.Any(group => group.IsBoss);
        }

        ///<summary>
        ///Get estimated wave duration in seconds.
        ///</summary>
        ///<returns>Estimated duration.</returns>
        public float GetEstimatedDuration()
        {
            var duration = 0f;

            foreach (var group in SpawnGroups)
            {
                //Time to spawn all enemies in group
                var spawnTime = (group.Count - 1) * group.SpawnDelay;

                //Add delay after group
                var totalTime = spawnTime + group.DelayAfterGroup;

                duration = System.Math.Max(duration, totalTime);
            }

            return duration;
        }

        ///<summary>
        ///Get wave difficulty rating.
        ///</summary>
        ///<returns>Difficulty rating (1-10).</returns>
        public int GetDifficultyRating()
        {
            var rating = 1;

            //Base rating from wave number
            rating += System.Math.Min(WaveNumber / 5, 5);

            //Add rating for enemy count
            var enemyCount = GetTotalEnemyCount();
            rating += System.Math.Min(enemyCount / 10, 2);

            //Add rating for boss enemies
            if (HasBossEnemies())
            {
                rating += 3;
            }

            //Add rating for special enemy types
            var specialTypes = GetEnemyTypes().Count(type =>
                type == ZombieType.Shadow ||
                type == ZombieType.Toxic ||
                type == ZombieType.Armored ||
                type == ZombieType.RobotClown ||
                type == ZombieType.Devastator);

            rating += System.Math.Min(specialTypes, 2);

            return System.Math.Min(rating, 10);
        }

        ///<summary>
        ///Validate wave script.
        ///</summary>
        ///<returns>Validation result.</returns>
        public ValidationResult Validate()
        {
            var result = new ValidationResult { IsValid = true };

            //Check wave number
            if (WaveNumber <= 0)
            {
                result.IsValid = false;
                result.AddError("Wave number must be positive");
            }

            //Check spawn groups
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

            //Check inter-wave delay
            if (InterWaveDelay < 0)
            {
                result.IsValid = false;
                result.AddError("Inter-wave delay cannot be negative");
            }

            //Check difficulty multiplier
            if (DifficultyMultiplier == null)
            {
                result.IsValid = false;
                result.AddError("Difficulty multiplier is required");
            }

            return result;
        }

        ///<summary>
        ///Clone this wave script.
        ///</summary>
        ///<returns>Cloned wave script.</returns>
        public WaveScript Clone()
        {
            var clone = new WaveScript
            {
                WaveNumber = this.WaveNumber,
                WaveName = this.WaveName,
                Description = this.Description,
                InterWaveDelay = this.InterWaveDelay,
                DifficultyMultiplier = new DifficultyMultiplier(this.DifficultyMultiplier.Clone()),
                Modifiers = this.Modifiers.Clone(),
                Rewards = this.Rewards.Clone(),
                Environment = this.Environment.Clone()
            };

            //Clone spawn groups
            clone.SpawnGroups = this.SpawnGroups.Select(group => group.Clone()).ToList();

            return clone;
        }

        ///<summary>
        ///Create a deep copy of this wave script for modification.
        ///</summary>
        ///<returns>Deep copied wave script.</returns>
        public WaveScript DeepCopy()
        {
            return Clone();
        }

        ///<summary>
        ///Get wave summary for display.
        ///</summary>
        ///<returns>Wave summary string.</returns>
        public string GetSummary()
        {
            var summary = $"Wave {WaveNumber}: {WaveName}\n";
            summary += $"Enemies: {GetTotalEnemyCount()}\n";
            summary += $"Difficulty: {GetDifficultyRating()}/10\n";
            summary += $"Duration: ~{GetEstimatedDuration():F0}s\n";

            if (HasBossEnemies())
            {
                summary += "Contains boss enemies\n";
            }

            return summary.Trim();
        }

        ///<summary>
        ///Get detailed wave information.
        ///</summary>
        ///<returns>Detailed wave information.</returns>
        public string GetDetailedInfo()
        {
            var info = GetSummary() + "\n\n";
            info += "Enemy Composition:\n";

            foreach (var group in SpawnGroups)
            {
                info += $"- {group.EnemyType} x{group.Count}";
                if (group.IsBoss)
                {
                    info += " (BOSS)";
                }
                info += $" - Pattern: {group.Pattern}\n";
            }

            info += $"\nModifiers:\n";
            info += $"- Health: x{DifficultyMultiplier.HealthModifier:F2}\n";
            info += $"- Speed: x{DifficultyMultiplier.SpeedModifier:F2}\n";
            info += $"- Damage: x{DifficultyMultiplier.DamageMultiplier:F2}\n";

            if (Rewards.CashBonus > 0)
            {
                info += $"\nRewards:\n";
                info += $"- Cash Bonus: ${Rewards.CashBonus}\n";
            }

            return info.Trim();
        }
    }

    ///<summary>
    ///Wave modifiers for special effects.
    ///</summary>
    public class WaveModifiers
    {
        public bool FastSpawn { get; set; }
        public bool ArmoredEnemies { get; set; }
        public bool RegeneratingEnemies { get; set; }
        public bool StealthEnemies { get; set; }
        public bool ExplosiveEnemies { get; set; }
        public float GlobalSpeedModifier { get; set; } = 1.0f;
        public float GlobalHealthModifier { get; set; } = 1.0f;
        public List<string> DisabledTowers { get; set; }
        public List<string> RestrictedUpgrades { get; set; }

        public WaveModifiers()
        {
            DisabledTowers = new List<string>();
            RestrictedUpgrades = new List<string>();
        }

        ///<summary>
        ///Clone this wave modifier.
        ///</summary>
        ///<returns>Cloned modifier.</returns>
        public WaveModifiers Clone()
        {
            return new WaveModifiers
            {
                FastSpawn = this.FastSpawn,
                ArmoredEnemies = this.ArmoredEnemies,
                RegeneratingEnemies = this.RegeneratingEnemies,
                StealthEnemies = this.StealthEnemies,
                ExplosiveEnemies = this.ExplosiveEnemies,
                GlobalSpeedModifier = this.GlobalSpeedModifier,
                GlobalHealthModifier = this.GlobalHealthModifier,
                DisabledTowers = new List<string>(this.DisabledTowers),
                RestrictedUpgrades = new List<string>(this.RestrictedUpgrades)
            };
        }
    }

    ///<summary>
    ///Wave rewards for completing the wave.
    ///</summary>
    public class WaveRewards
    {
        public int CashBonus { get; set; }
        public int ExperienceBonus { get; set; }
        public List<string> UnlockTowers { get; set; }
        public List<string> UnlockUpgrades { get; set; }
        public string SpecialReward { get; set; }

        public WaveRewards()
        {
            UnlockTowers = new List<string>();
            UnlockUpgrades = new List<string>();
        }

        ///<summary>
        ///Clone this wave reward.
        ///</summary>
        ///<returns>Cloned reward.</returns>
        public WaveRewards Clone()
        {
            return new WaveRewards
            {
                CashBonus = this.CashBonus,
                ExperienceBonus = this.ExperienceBonus,
                UnlockTowers = new List<string>(this.UnlockTowers),
                UnlockUpgrades = new List<string>(this.UnlockUpgrades),
                SpecialReward = this.SpecialReward
            };
        }
    }

    ///<summary>
    ///Wave environment settings.
    ///</summary>
    public class WaveEnvironment
    {
        public string Weather { get; set; }
        public string TimeOfDay { get; set; }
        public float VisibilityModifier { get; set; } = 1.0f;
        public List<string> EnvironmentalHazards { get; set; }

        public WaveEnvironment()
        {
            EnvironmentalHazards = new List<string>();
        }

        ///<summary>
        ///Clone this wave environment.
        ///</summary>
        ///<returns>Cloned environment.</returns>
        public WaveEnvironment Clone()
        {
            return new WaveEnvironment
            {
                Weather = this.Weather,
                TimeOfDay = this.TimeOfDay,
                VisibilityModifier = this.VisibilityModifier,
                EnvironmentalHazards = new List<string>(this.EnvironmentalHazards)
            };
        }
    }

    ///<summary>
    ///Enemy modifier for special abilities.
    ///</summary>
    public class EnemyModifier
    {
        public string ModifierType { get; set; }
        public float Value { get; set; }
        public float Duration { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public EnemyModifier()
        {
            Parameters = new Dictionary<string, object>();
        }

        ///<summary>
        ///Clone this enemy modifier.
        ///</summary>
        ///<returns>Cloned modifier.</returns>
        public EnemyModifier Clone()
        {
            return new EnemyModifier
            {
                ModifierType = this.ModifierType,
                Value = this.Value,
                Duration = this.Duration,
                Parameters = new Dictionary<string, object>(this.Parameters)
            };
        }
    }

    ///<summary>
    ///Spawn conditions for enemy groups.
    ///</summary>
    public class SpawnConditions
    {
        public int MinimumWaveNumber { get; set; }
        public int MaximumWaveNumber { get; set; }
        public float MinimumPlayerLevel { get; set; }
        public List<string> RequiredTowers { get; set; }
        public List<string> ForbiddenTowers { get; set; }
        public bool RequireBossDefeated { get; set; }

        public SpawnConditions()
        {
            RequiredTowers = new List<string>();
            ForbiddenTowers = new List<string>();
        }

        ///<summary>
        ///Check if conditions are met.
        ///</summary>
        ///<param name="currentWave">Current wave number.</param>
        ///<param name="playerLevel">Player level.</param>
        ///<param name="builtTowers">List of built towers.</param>
        ///<returns>True if conditions are met.</returns>
        public bool AreConditionsMet(int currentWave, int playerLevel, List<string> builtTowers)
        {
            //Check wave number range
            if (MinimumWaveNumber > 0 && currentWave < MinimumWaveNumber)
                return false;

            if (MaximumWaveNumber > 0 && currentWave > MaximumWaveNumber)
                return false;

            //Check player level
            if (MinimumPlayerLevel > 0 && playerLevel < MinimumPlayerLevel)
                return false;

            //Check required towers
            if (RequiredTowers.Count > 0)
            {
                foreach (var requiredTower in RequiredTowers)
                {
                    if (!builtTowers.Contains(requiredTower))
                        return false;
                }
            }

            //Check forbidden towers
            if (ForbiddenTowers.Count > 0)
            {
                foreach (var forbiddenTower in ForbiddenTowers)
                {
                    if (builtTowers.Contains(forbiddenTower))
                        return false;
                }
            }

            return true;
        }

        ///<summary>
        ///Clone this spawn condition.
        ///</summary>
        ///<returns>Cloned condition.</returns>
        public SpawnConditions Clone()
        {
            return new SpawnConditions
            {
                MinimumWaveNumber = this.MinimumWaveNumber,
                MaximumWaveNumber = this.MaximumWaveNumber,
                MinimumPlayerLevel = this.MinimumPlayerLevel,
                RequiredTowers = new List<string>(this.RequiredTowers),
                ForbiddenTowers = new List<string>(this.ForbiddenTowers),
                RequireBossDefeated = this.RequireBossDefeated
            };
        }
    }
}
