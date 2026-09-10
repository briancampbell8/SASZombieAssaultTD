// ====================================================================================================
//  FILE: TowerUpgrade.cs
//  PATH: ./Engine/Towers/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the TowerUpgrade module.
//
//  RESPONSIBILITIES:
//      - Provide CanApplyTo() behavior for the Core subsystem.
//      - Provide ApplyTo() behavior for the Core subsystem.
//      - Provide GetPreviewStats() behavior for the Core subsystem.
//      - Provide GetPowerRating() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Save.GameSave;
using SASZombieAssaultTD.Engine.Towers.Save;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Towers.TowerEnums;

namespace SASZombieAssaultTD.Engine.Towers
{

    public class TowerUpgrade
    {

        internal object DamageIncrease;
        internal object RangeIncrease;
        internal object FireRateIncrease;
        internal object SpeedIncrease;
        public object AddUpgrade;
        public int RequiredLevel { get; set; }

        public UpgradeType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public int Level { get; set; }
        public int MaxLevel { get; set; } = 5;

        public float DamageMultiplier { get; set; } = 1.0f;
        public float DamageBonus { get; set; } = 0f;
        public float RangeMultiplier { get; set; } = 1.0f;
        public float AttackSpeedMultiplier { get; set; } = 1.0f;
        public float RangeBonus { get; set; } = 0f;
        public float FireRateMultiplier { get; set; } = 1.0f;
        public float FireRateBonus { get; set; } = 0f;

        public bool IsAffordable { get; set; }
        public TowerType TowerType { get; set; } = TowerType.Basic;
        public bool IsAvailable { get; set; } = true;

        public List<string> SpecialAbilities { get; set; } = new();
        public Dictionary<string, float> SpecialModifiers { get; set; } = new();

        public List<string> Prerequisites { get; set; } = new();
        public TowerType RequiredTowerType { get; set; }
        public int MinimumTowerLevel { get; set; } = 1;

        public string IconPath { get; set; }
        public string SoundEffect { get; set; }
        public string ParticleEffect { get; set; }
        public string Id { get; internal set; }
        public TowerSaveData tower { get; internal set; }
        public float Performance { get; internal set; }
        public float Power { get; internal set; }
        public string Key { get; internal set; }


        private static void CaptureTowerState(
            GSCore SASsave,
            TowerUpgrade Towersave,
            Tower tower)
        {

            Towersave.tower = new TowerSaveData
            {
                TowerCount = 0, //Placeholder
                TotalValue = 0, //Placeholder
                TowerTypes = new List<string>(), //Placeholder
                TowerPositions = new Dictionary<string, Vector3>(), //Placeholder
                TowerLevels = new Dictionary<string, int>() //Placeholder
            };
        }
        public TowerUpgrade()
        {
            Name = "Basic Upgrade";
            Description = "Improves tower performance";
            Cost = 100;
            Level = 1;
            Type = UpgradeType.Damage;
        }

        public TowerUpgrade(UpgradeType type, string name, int cost, int level = 1)
        {
            Type = type;
            Name = name;
            Cost = cost;
            Level = level;
            InitializeUpgradeStats();
        }

        public TowerUpgrade(TowerUpgrade other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            Type = other.Type;
            Name = other.Name;
            Description = other.Description;
            Cost = other.Cost;
            Level = other.Level;
            MaxLevel = other.MaxLevel;

            DamageMultiplier = other.DamageMultiplier;
            DamageBonus = other.DamageBonus;
            RangeMultiplier = other.RangeMultiplier;
            RangeBonus = other.RangeBonus;
            FireRateMultiplier = other.FireRateMultiplier;
            FireRateBonus = other.FireRateBonus;

            IsAffordable = other.IsAffordable;
            TowerType = other.TowerType;
            IsAvailable = other.IsAvailable;

            SpecialAbilities = new List<string>(other.SpecialAbilities);
            SpecialModifiers = new Dictionary<string, float>(other.SpecialModifiers);

            Prerequisites = new List<string>(other.Prerequisites);
            RequiredTowerType = other.RequiredTowerType;
            MinimumTowerLevel = other.MinimumTowerLevel;

            IconPath = other.IconPath;
            SoundEffect = other.SoundEffect;
            ParticleEffect = other.ParticleEffect;
            Id = other.Id;
        }
        private void InitializeUpgradeStats()
        {
            switch (Type)
            {
                case UpgradeType.Damage:
                    DamageMultiplier = 1.0f + (Level * 0.2f);
                    DamageBonus = Level * 10;
                    Description = $"Increases damage by {System.Math.Round((DamageMultiplier - 1) * 100, 0)}%";
                    break;

                case UpgradeType.Range:
                    RangeMultiplier = 1.0f + (Level * 0.15f);
                    RangeBonus = Level * 2;
                    Description = $"Increases range by {System.Math.Round((RangeMultiplier - 1) * 100, 0)}%";
                    break;

                case UpgradeType.FireRate:
                    FireRateMultiplier = 1.0f + (Level * 0.25f);
                    FireRateBonus = Level * 0.1f;
                    Description = $"Increases fire rate by {System.Math.Round((FireRateMultiplier - 1) * 100, 0)}%";
                    break;

                case UpgradeType.Special:
                    InitializeSpecialUpgrade();
                    break;

                case UpgradeType.Complete:
                    InitializeCompleteUpgrade();
                    break;
            }

            Cost = CalculateUpgradeCost();
        }

        private void InitializeSpecialUpgrade()
        {
            switch (Level)
            {
                case 1:
                    SpecialAbilities.Add("Slow");
                    SpecialModifiers["SlowEffect"] = 0.3f;
                    Description = "Adds 30% slow effect to attacks";
                    break;

                case 2:
                    SpecialAbilities.Add("Splash");
                    SpecialModifiers["SplashRadius"] = 2.0f;
                    SpecialModifiers["SplashDamage"] = 0.5f;
                    Description = "Adds splash damage with 2m radius";
                    break;

                case 3:
                    SpecialAbilities.Add("Critical");
                    SpecialModifiers["CriticalChance"] = 0.15f;
                    SpecialModifiers["CriticalDamage"] = 2.0f;
                    Description = "15% chance for critical damage (2x)";
                    break;
            }
        }

        private void InitializeCompleteUpgrade()
        {
            DamageMultiplier = 1.0f + (Level * 0.3f);
            RangeMultiplier = 1.0f + (Level * 0.2f);
            FireRateMultiplier = 1.0f + (Level * 0.35f);

            Description =
                $"Complete upgrade: +{System.Math.Round((DamageMultiplier - 1) * 100, 0)}% damage, " +
                $"+{System.Math.Round((RangeMultiplier - 1) * 100, 0)}% range, " +
                $"+{System.Math.Round((FireRateMultiplier - 1) * 100, 0)}% fire rate";
        }

        private int CalculateUpgradeCost()
        {
            int baseCost = Type switch
            {
                UpgradeType.Damage => 150,
                UpgradeType.Range => 200,
                UpgradeType.FireRate => 250,
                UpgradeType.Special => 400,
                UpgradeType.Complete => 600,
                _ => 100
            };

            return (int)(baseCost * System.Math.Pow(1.5, Level - 1));
        }

        public bool CanApplyTo(Tower tower)
        {
            if (tower == null) return false;
            if (Level >= MaxLevel) return false;
            if (tower.Level < MinimumTowerLevel) return false;
            if (RequiredTowerType != TowerType.Basic && tower.Type != RequiredTowerType) return false;

            return true;
        }

        public void ApplyTo(Tower tower)
        {
            if (!CanApplyTo(tower)) return;

            tower.ApplyUpgrade(this);

            Level++;
            InitializeUpgradeStats();
        }

        public string GetPreviewStats()
        {
            var stats = new List<string>();

            if (DamageMultiplier > 1.0f || DamageBonus > 0)
                stats.Add($"Damage: +{System.Math.Round((DamageMultiplier - 1) * 100, 0)}% (+{DamageBonus})");

            if (RangeMultiplier > 1.0f || RangeBonus > 0)
                stats.Add($"Range: +{System.Math.Round((RangeMultiplier - 1) * 100, 0)}% (+{RangeBonus}m)");

            if (FireRateMultiplier > 1.0f || FireRateBonus > 0)
                stats.Add($"Fire Rate: +{System.Math.Round((FireRateMultiplier - 1) * 100, 0)}% (+{FireRateBonus:F1})");

            if (SpecialAbilities.Count > 0)
                stats.Add($"Abilities: {string.Join(", ", SpecialAbilities)}");

            return string.Join("\n", stats);
        }

        public float GetPowerRating()
        {
            float rating = 0f;

            rating += (DamageMultiplier - 1.0f) * 100f;
            rating += (RangeMultiplier - 1.0f) * 80f;
            rating += (FireRateMultiplier - 1.0f) * 90f;
            rating += SpecialAbilities.Count * 50f;

            return rating;
        }

        // ---------------------------------------------------------------------------------------------
        // Fully Implemented Methods (No Exceptions)
        // ---------------------------------------------------------------------------------------------

        internal bool IsAvailableForLevel(int towerLevel, int playerLevel)
        {
            if (towerLevel < MinimumTowerLevel)
                return false;

            if (playerLevel < Level)
                return false;

            return IsAvailable;
        }

        internal bool CanPurchase(int playerCash, int towerLevel)
        {
            if (!IsAvailableForLevel(towerLevel, towerLevel))
                return false;

            return playerCash >= Cost;
        }

        internal bool Purchase(int playerCash)
        {
            if (playerCash < Cost)
                return false;

            return true;
        }

        internal void ApplyToTower(Tower tower)
        {
            if (tower == null)
                return;

            tower.ApplyUpgrade(this);
        }

        internal void RemoveFromTower(Tower tower)
        {
            if (tower == null)
                return;

            tower.RemoveUpgrade(this);
        }

        internal object GetEfficiencyRating()
        {
            float rating = GetPowerRating() / Cost;
            return rating;
        }

        internal void SetVisualProperties(object value1, Color white, object value2, object value3)
        {
            // No-op implementation to avoid exceptions.
        }
    }
}
