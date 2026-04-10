using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Audio;
using SASZombieAssaultTD.Engine.Towers.Upgrades;
using SASZombieAssaultTD.Engine.Economy;
using SASZombieAssaultTD.Engine.Extensions;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl
{
    /// <summary>
    /// Particle effect for visual effects.
    /// </summary>
    public class ParticleEffect
    {
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public Color Color { get; set; }
        public float Size { get; set; }
        public float Lifetime { get; set; }

        public ParticleEffect()
        {
            Position = Vector3.Zero;
            Velocity = Vector3.Zero;
            Color = Color.White;
            Size = 1f;
            Lifetime = 1f;
        }
    }

    /// <summary>
    /// Sound effect for audio playback.
    /// </summary>
    public class SoundEffect
    {
        public string Name { get; set; }
        public float Volume { get; set; }
        public bool IsLooping { get; set; }

        public SoundEffect()
        {
            Name = "";
            Volume = 1f;
            IsLooping = false;
        }
    }

    /// <summary>
    /// Tower upgrade system for SAS Zombie Assault TD.
    /// Manages tower enhancements, stat modifiers, and upgrade progression.
    /// </summary>
    public class NeuralTowerUpgrade : TowerUpgrade
    {
        private readonly Dictionary<string, float> _statModifiers;
        private readonly List<string> _specialAbilities;
        private readonly List<UpgradeEffect> _visualEffects;
        private readonly List<UpgradeRequirement> _requirements;
        private readonly List<UpgradeReward> _rewards;

        // Basic properties
        public int Level { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Cost { get; private set; }
        public TowerType TowerType { get; private set; }
        public UpgradeType UpgradeType { get; private set; }

        // Upgrade effects
        public float DamageMultiplier { get; private set; }
        public float RangeMultiplier { get; private set; }
        public float FireRateMultiplier { get; private set; }
        public float SpeedMultiplier { get; private set; }
        public float ArmorMultiplier { get; private set; }
        public float AccuracyMultiplier { get; private set; }
        public float CriticalChance { get; private set; }
        public float CriticalMultiplier { get; private set; }

        // Visual properties
        public Sprite UpgradeSprite { get; private set; }
        public Color UpgradeColor { get; private set; }
        public ParticleEffect UpgradeEffect { get; private set; }
        public SoundEffect UpgradeSound { get; private set; }

        // Upgrade state
        public bool IsPurchased { get; private set; }
        public bool IsMaxLevel { get; private set; }
        public bool IsPrerequisiteMet { get; private set; }
        public DateTime PurchaseTime { get; private set; }

        // Events
        public event Action<TowerUpgrade> OnUpgradePurchased;
        public event Action<TowerUpgrade> OnUpgradeApplied;
        public event Action<TowerUpgrade> OnUpgradeRemoved;

        public NeuralTowerUpgrade()
        {
            _statModifiers = new Dictionary<string, float>();
            _specialAbilities = new List<string>();
            _visualEffects = new List<UpgradeEffect>();
            _requirements = new List<UpgradeRequirement>();
            _rewards = new List<UpgradeReward>();

            Level = 1;
            Name = "Basic Upgrade";
            Description = "Basic tower upgrade";
            Cost = 100;
            TowerType = TowerType.Basic; // TODO: VickersTurret doesn't exist in enum
            UpgradeType = UpgradeType.Damage;

            DamageMultiplier = 1.0f;
            RangeMultiplier = 1.0f;
            FireRateMultiplier = 1.0f;
            SpeedMultiplier = 1.0f;
            ArmorMultiplier = 1.0f;
            AccuracyMultiplier = 1.0f;
            CriticalChance = 0.0f;
            CriticalMultiplier = 1.0f;

            UpgradeColor = Color.White;
        }

        /// <summary>
        /// Create a new tower upgrade.
        /// </summary>
        /// <param name="level">Upgrade level.</param>
        /// <param name="name">Upgrade name.</param>
        /// <param name="description">Upgrade description.</param>
        /// <param name="cost">Upgrade cost.</param>
        /// <param name="towerType">Tower type.</param>
        /// <param name="upgradeType">Upgrade type.</param>
        /// <returns>Created upgrade.</returns>
        public static TowerUpgrade Create(int level, string name, string description, int cost, TowerType towerType, UpgradeType upgradeType)
        {
            var upgrade = new NeuralTowerUpgrade
            {
                Level = level,
                Name = name,
                Description = description,
                Cost = cost,
                TowerType = towerType,
                UpgradeType = upgradeType
            };

            // Set default values based on upgrade type
            upgrade.SetDefaultValues(upgradeType);

            return upgrade;
        }

        /// <summary>
        /// Set default values based on upgrade type.
        /// </summary>
        /// <param name="upgradeType">Upgrade type.</param>
        private void SetDefaultValues(UpgradeType upgradeType)
        {
            switch (upgradeType)
            {
                case UpgradeType.Damage:
                    AddSpecialAbility("damage_boost");
                    break;

                case UpgradeType.Range:
                    RangeMultiplier = 1.3f;
                    AccuracyMultiplier = 1.1f;
                    break;

                case UpgradeType.FireRate:
                    SpeedMultiplier = 1.5f;
                    FireRateMultiplier = 1.4f;
                    break;

                case UpgradeType.Speed:
                    FireRateMultiplier = 1.2f;
                    break;

                case UpgradeType.Armor:
                    ArmorMultiplier = 1.3f;
                    DamageMultiplier = 1.1f;
                    break;

                case UpgradeType.Accuracy:
                    AccuracyMultiplier = 1.5f;
                    CriticalChance = 0.15f;
                    break;

                case UpgradeType.Special:
                    // Complete upgrade for special abilities
                    AddSpecialAbility("area_damage");
                    AddSpecialAbility("chain_lightning");
                    AddSpecialAbility("slow_aura");
                    break;

                }
        }

        /// <summary>
        /// Add a stat modifier.
        /// </summary>
        /// <param name="stat">Stat name.</param>
        /// <param name="multiplier">Stat multiplier.</param>
        public void AddStatModifier(string stat, float multiplier)
        {
            _statModifiers[stat] = multiplier;
        }

        /// <summary>
        /// Add a special ability.
        /// </summary>
        /// <param name="ability">Ability name.</param>
        public void AddSpecialAbility(string ability)
        {
            if (!_specialAbilities.Contains(ability))
            {
                _specialAbilities.Add(ability);
            }
        }

        /// <summary>
        /// Add a visual effect.
        /// </summary>
        /// <param name="effect">Visual effect.</param>
        public void AddVisualEffect(UpgradeEffect effect)
        {
            _visualEffects.Add(effect);
        }

        /// <summary>
        /// Add a requirement.
        /// </summary>
        /// <param name="requirement">Upgrade requirement.</param>
        public void AddRequirement(UpgradeRequirement requirement)
        {
            _requirements.Add(requirement);
        }

        /// <summary>
        /// Add a reward.
        /// </summary>
        /// <param name="reward">Upgrade reward.</param>
        public void AddReward(UpgradeReward reward)
        {
            _rewards.Add(reward);
        }

        /// <summary>
        /// Set visual properties.
        /// </summary>
        /// <param name="sprite">Upgrade sprite.</param>
        /// <param name="color">Upgrade color.</param>
        /// <param name="effect">Particle effect.</param>
        /// <param name="sound">Sound effect.</param>
        public void SetVisualProperties(Sprite sprite, Color color, ParticleEffect effect, SoundEffect sound)
        {
            UpgradeSprite = sprite;
            UpgradeColor = color;
            UpgradeEffect = effect;
            UpgradeSound = sound;
        }

        /// <summary>
        /// Check if upgrade can be purchased.
        /// </summary>
        /// <param name="playerCash">Player cash amount.</param>
        /// <param name="towerLevel">Current tower level.</param>
        /// <returns>True if upgrade can be purchased.</returns>
        public bool CanPurchase(int playerCash, int towerLevel)
        {
            if (IsPurchased || IsMaxLevel) return false;
            if (playerCash < Cost) return false;
            if (towerLevel < Level - 1) return false;
            if (!IsPrerequisiteMet) return false;

            return true;
        }

        /// <summary>
        /// Purchase the upgrade.
        /// </summary>
        /// <param name="playerCash">Player cash amount.</param>
        /// <returns>True if upgrade was purchased.</returns>
        public bool Purchase(int playerCash)
        {
            if (!CanPurchase(playerCash, Level)) return false;

            // Deduct cost
            EconomyManager.Spend(Cost);

            // Mark as purchased
            IsPurchased = true;
            PurchaseTime = DateTime.Now;

            // Apply upgrade effects
            ApplyUpgradeEffects();

            // Trigger events
            OnUpgradePurchased?.Invoke(this);
            OnUpgradeApplied?.Invoke(this);

            // Play purchase sound
            AudioSystem.PlaySound("upgrade_purchased");

            Console.WriteLine($"Purchased upgrade: {Name} for ${Cost}");
            return true;
        }

        /// <summary>
        /// Apply upgrade effects to a tower.
        /// </summary>
        /// <param name="tower">Tower to apply upgrade to.</param>
        public void ApplyToTower(Tower tower)
        {
            if (tower == null) return;

            // Apply stat modifiers using the new tower methods
            foreach (var modifier in _statModifiers)
            {
                tower.SetStatModifier(modifier.Key, modifier.Value);
            }

            // Apply special abilities using the new tower methods
            foreach (var ability in _specialAbilities)
            {
                tower.AddSpecialAbility(ability);
            }

            // Apply visual effects (simplified - would need tower visual effect system)
            foreach (var effect in _visualEffects)
            {
                // tower.AddVisualEffect(effect); // Would need to implement this method
                Console.WriteLine($"Applied visual effect: {effect}");
            }

            // Update tower level
            tower.Level = Level;

            // Update tower visual properties using the new SetVisualProperties method
            if (UpgradeSprite != null)
            {
                // Apply visual properties with upgrade-specific colors
                var upgradeColor = GetUpgradeColor();
                tower.SetVisualProperties(tower,
                                          primary: upgradeColor,
                                          secondary: upgradeColor,
                                          v1: 1.0f,
                                          v2: 0.5f);
            ///    SetVisualProperties(tower, upgradeColor.primary, upgradeColor.secondary, 1.0f, 0.5f);
            }           

            // Trigger upgrade applied event
            OnUpgradeApplied?.Invoke(this);
        }

        private void SetVisualProperties(Tower tower, object primary, object secondary, float v1, float v2)
        {
            throw new NotImplementedException();
        }

        private object GetUpgradeColor()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove upgrade effects from a tower.
        /// </summary>
        /// <param name="tower">Tower to remove upgrade from.</param>
        public void RemoveFromTower(Tower tower)
        {
            if (tower == null) return;

            // Remove stat modifiers using the new tower methods
            foreach (var modifier in _statModifiers)
            {
                tower.RemoveStatModifier(modifier.Key);
            }

            // Remove special abilities using the new tower methods
            foreach (var ability in _specialAbilities)
            {
                tower.RemoveSpecialAbility(ability);
            }

            // Remove visual effects (simplified - would need tower visual effect system)
            foreach (var effect in _visualEffects)
            {
                // tower.RemoveVisualEffect(effect); // Would need to implement this method
                Console.WriteLine($"Removed visual effect: {effect}");
            }

            // Reset tower level (simplified - full implementation would track original level)
            tower.Level = 1;

            // Reset tower visual properties (simplified)
            // if (OriginalSprite != null)
            // {
            //     tower.SetSprite(OriginalSprite);
            // }

            // if (OriginalColor != Color.White)
            // {
            //     tower.SetTintColor(OriginalColor);
            // }

            // Trigger upgrade removed event
            OnUpgradeRemoved?.Invoke(this);
        }

        /// <summary>
        /// Check if upgrade is available.
        /// </summary>
        /// <param name="towerLevel">Current tower level.</param>
        /// <param name="playerLevel">Player level.</param>
        /// <returns>True if upgrade is available.</returns>
        public bool IsAvailable(int towerLevel, int playerLevel)
        {
            if (IsPurchased || IsMaxLevel) return false;
            if (towerLevel < Level - 1) return false;
            if (playerLevel < GetRequiredPlayerLevel()) return false;

            return true;
        }

        /// <summary>
        /// Get required player level.
        /// </summary>
        /// <returns>Required player level.</returns>
        public int GetRequiredPlayerLevel()
        {
            return Level * 2; // Example: Level 5 upgrade requires level 10 player
        }

        /// <summary>
        /// Get upgrade description with stats.
        /// </summary>
        /// <returns>Detailed description.</returns>
        public string GetDetailedDescription()
        {
            var description = Description + "\n\n";
            description += "Effects:\n";

            if (DamageMultiplier != 1.0f)
                description += $"Damage: x{DamageMultiplier:F1}\n";

            if (RangeMultiplier != 1.0f)
                description += $"Range: x{RangeMultiplier:F1}\n";

            if (FireRateMultiplier != 1.0f)
                description += $"Fire Rate: x{FireRateMultiplier:F1}\n";

            if (SpeedMultiplier != 1.0f)
                description += $"Speed: x{SpeedMultiplier:F1}\n";

            if (ArmorMultiplier != 1.0f)
                description += $"Armor: x{ArmorMultiplier:F1}\n";

            if (AccuracyMultiplier != 1.0f)
                description += $"Accuracy: x{AccuracyMultiplier:F1}\n";

            if (CriticalChance > 0f)
                description += $"Critical Chance: {CriticalChance:P1}\n";

            if (CriticalMultiplier != 1.0f)
                description += $"Critical Damage: x{CriticalMultiplier:F1}\n";

            if (_specialAbilities.Count > 0)
            {
                description += "Special Abilities:\n";
                foreach (var ability in _specialAbilities)
                {
                    description += $"- {ability}\n";
                }
            }

            return description.Trim();
        }

        /// <summary>
        /// Get upgrade summary.
        /// </summary>
        /// <returns>Upgrade summary.</returns>
        public string GetSummary()
        {
            var summary = $"{Name} (Level {Level})\n";
            summary += $"Cost: ${Cost}\n";
            summary += $"Type: {UpgradeType}\n";

            if (_specialAbilities.Count > 0)
            {
                summary += $"Abilities: {string.Join(", ", _specialAbilities)}\n";
            }

            return summary.Trim();
        }

        /// <summary>
        /// Clone this upgrade.
        /// </summary>
        /// <returns>Cloned upgrade.</returns>
        public NeuralTowerUpgrade Clone()
        {
            var clone = new NeuralTowerUpgrade
            {
                Level = this.Level,
                Name = this.Name,
                Description = this.Description,
                Cost = this.Cost,
                TowerType = this.TowerType,
                UpgradeType = this.UpgradeType,
                DamageMultiplier = this.DamageMultiplier,
                RangeMultiplier = this.RangeMultiplier,
                FireRateMultiplier = this.FireRateMultiplier,
                SpeedMultiplier = this.SpeedMultiplier,
                ArmorMultiplier = this.ArmorMultiplier,
                AccuracyMultiplier = this.AccuracyMultiplier,
                CriticalChance = this.CriticalChance,
                CriticalMultiplier = this.CriticalMultiplier,
                UpgradeSprite = this.UpgradeSprite,
                UpgradeColor = this.UpgradeColor,
                UpgradeEffect = this.UpgradeEffect,
                UpgradeSound = this.UpgradeSound
            };

            // Clone collections
            foreach (var modifier in _statModifiers)
            {
                clone._statModifiers[modifier.Key] = modifier.Value;
            }

            clone._specialAbilities.AddRange(this._specialAbilities);
            clone._visualEffects.AddRange(this._visualEffects);
            clone._requirements.AddRange(this._requirements);
            clone._rewards.AddRange(this._rewards);

            return clone;
        }

        /// <summary>
        /// Apply upgrade effects.
        /// </summary>
        private void ApplyUpgradeEffects()
        {
            // Apply visual effects using simplified particle system
            if (UpgradeEffect != null)
            {
                // Create particle effect for upgrade
                Console.WriteLine($"Creating upgrade effect: {UpgradeEffect}");
                // In full implementation, would call:
                // ParticleSystem.Instance?.CreateEffect(UpgradeEffect);
                // For now, we'll just log it
            }

            // Play sound effect
            if (UpgradeSound != null)
            {
                AudioSystem.PlaySound(UpgradeSound.Name);
            }
        }

        /// <summary>
        /// Check if upgrade is max level.
        /// </summary>
        /// <param name="maxLevel">Maximum upgrade level.</param>
        /// <returns>True if upgrade is max level.</returns>
        public bool IsMaxLevelUpgrade(int maxLevel)
        {
            return Level >= maxLevel;
        }

        /// <summary>
        /// Get upgrade power rating.
        /// </summary>
        /// <returns>Power rating (1-10).</returns>
        public int GetPowerRating()
        {
            var rating = 1;

            // Calculate rating based on stat modifiers
            if (DamageMultiplier > 1.0f) rating += (int)((DamageMultiplier - 1.0f) * 3);
            if (RangeMultiplier > 1.0f) rating += (int)((RangeMultiplier - 1.0f) * 2);
            if (FireRateMultiplier > 1.0f) rating += (int)((FireRateMultiplier - 1.0f) * 2);
            if (SpeedMultiplier > 1.0f) rating += (int)((SpeedMultiplier - 1.0f) * 1);
            if (AccuracyMultiplier > 1.0f) rating += (int)((AccuracyMultiplier - 1.0f) * 1);

            // Add rating for special abilities
            rating += _specialAbilities.Count;

            // Add rating for critical chance
            rating += (int)(CriticalChance * 5);

            return System.Math.Min(10, System.Math.Max(1, rating));
        }

        /// <summary>
        /// Get upgrade cost scaling based on level.
        /// </summary>
        /// <param name="baseCost">Base cost.</param>
        /// <param name="level">Upgrade level.</param>
        /// <returns>Scaled cost.</returns>
        public static int GetScaledCost(int baseCost, int level)
        {
            var scaling = 1.5f; // Cost increases by 50% per level
            return (int)(baseCost * MathF.Pow(scaling, level - 1));
        }

        /// <summary>
        /// Get upgrade efficiency rating.
        /// </summary>
        /// <returns>Efficiency rating (0-1).</returns>
        public float GetEfficiencyRating()
        {
            var powerRating = GetPowerRating();
            var costEfficiency = 10.0f / Cost; // Higher rating for lower cost
            return System.Math.Clamp((powerRating * costEfficiency) / 10f, 0f, 1f);
        }
    }

    /// <summary>
    /// Upgrade types for towers.
    /// </summary>
    public enum UpgradeType
    {
        Damage,
        Range,
        FireRate,
        Speed,
        Armor,
        Accuracy,
        Special,
        Elemental
    }

    /// <summary>
    /// Upgrade visual effect.
    /// </summary>
    public class UpgradeEffect
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public float Duration { get; set; }
        public float Intensity { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public UpgradeEffect()
        {
            Parameters = new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Upgrade requirement.
    /// </summary>
    public class UpgradeRequirement
    {
        public RequirementType Type { get; set; }
        public string Target { get; set; }
        public int Amount { get; set; }
        public bool IsMet { get; set; }

        public bool CheckRequirement()
        {
            return Type switch
            {
                RequirementType.TowerLevel => CheckTowerLevel(),
                RequirementType.PlayerLevel => CheckPlayerLevel(),
                RequirementType.WaveComplete => CheckWaveComplete(),
                RequirementType.EnemiesKilled => CheckEnemiesKilled(),
                RequirementType.TowerCount => CheckTowerCount(),
                _ => false
            };
        }

        private bool CheckTowerLevel()
        {
            // Implementation would check tower level
            return true;
        }

        private bool CheckPlayerLevel()
        {
            // Implementation would check player level
            return true;
        }

        private bool CheckWaveComplete()
        {
            // Implementation would check wave completion
            return true;
        }

        private bool CheckEnemiesKilled()
        {
            // Implementation would check enemy kills
            return true;
        }

        private bool CheckTowerCount()
        {
            // Implementation would check tower count
            return true;
        }
    }

    /// <summary>
    /// Upgrade reward.
    /// </summary>
    public class UpgradeReward
    {
        public RewardType Type { get; set; }
        public string Target { get; set; }
        public int Amount { get; set; }
        public bool IsClaimed { get; set; }

        public void ClaimReward()
        {
            if (IsClaimed) return;

            switch (Type)
            {
                case RewardType.Cash:
                    EconomyManager.Earn(Amount);
                    break;
                case RewardType.Experience:
                    // Implementation would add experience
                    break;
                case RewardType.Unlock:
                    // Implementation would unlock content
                    break;
            }

            IsClaimed = true;
        }
    }

    /// <summary>
    /// Requirement types.
    /// </summary>
    public enum RequirementType
    {
        TowerLevel,
        PlayerLevel,
        WaveComplete,
        EnemiesKilled,
        TowerCount
    }

    /// <summary>
    /// Reward types.
    /// </summary>
    public enum RewardType
    {
        Cash,
        Experience,
        Unlock
    }
}
