using SASZombieAssaultTD.Engine.Towers;
using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Economy;

namespace SASZombieAssaultTD.Engine.Towers
{
    /// <summary>
    /// Tower upgrade system for SAS Zombie Assault TD.
    /// Manages tower enhancements, stat modifiers, and upgrade progression.
    /// </summary>
    public enum UpgradeType
    {
        /// <summary>
        /// Increases tower damage output.
        /// </summary>
        Damage,
        
        /// <summary>
        /// Increases tower attack range.
        /// </summary>
        Range,
        
        /// <summary>
        /// Increases tower fire rate.
        /// </summary>
        FireRate,
        
        /// <summary>
        /// Adds special abilities or effects.
        /// </summary>
        Special,
        
        /// <summary>
        /// Complete upgrade package with multiple improvements.
        /// </summary>
        Complete
    }

    /// <summary>
    /// Comprehensive tower upgrade data with progression system.
    /// </summary>
    public class TowerUpgrade
    {
        public UpgradeType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public int Level { get; set; }
        public int MaxLevel { get; set; } = 5;
        
        // Stat modifiers
        public float DamageMultiplier { get; set; } = 1.0f;
        public float DamageBonus { get; set; } = 0f;
        public float RangeMultiplier { get; set; } = 1.0f;
        public float RangeBonus { get; set; } = 0f;
        public float FireRateMultiplier { get; set; } = 1.0f;
        public float FireRateBonus { get; set; } = 0f;
        
        /// <summary>
        /// Whether this upgrade can be afforded.
        /// </summary>
        public bool IsAffordable { get; set; }

        /// <summary>
        /// Tower type this upgrade applies to.
        /// </summary>
        public TowerType TowerType { get; set; } = TowerType.Basic;

        /// <summary>
        /// Whether this upgrade is available for purchase.
        /// </summary>
        public bool IsAvailable { get; set; } = true;
        
        // Special abilities
        public List<string> SpecialAbilities { get; set; } = new();
        public Dictionary<string, float> SpecialModifiers { get; set; } = new();
        
        // Requirements
        public List<string> Prerequisites { get; set; } = new();
        public TowerType RequiredTowerType { get; set; }
        public int MinimumTowerLevel { get; set; } = 1;
        
        // Visual and audio
        public string IconPath { get; set; }
        public string SoundEffect { get; set; }
        public string ParticleEffect { get; set; }
        public string Id { get; internal set; }

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

        /// <summary>
        /// Copy constructor for TowerUpgrade.
        /// </summary>
        /// <param name="other">The upgrade to copy.</param>
        public TowerUpgrade(TowerUpgrade other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            
            Type = other.Type;
            Name = other.Name;
            Description = other.Description;
            Cost = other.Cost;
            Level = other.Level;
            MaxLevel = other.MaxLevel;
            
            // Copy stat modifiers
            DamageMultiplier = other.DamageMultiplier;
            DamageBonus = other.DamageBonus;
            RangeMultiplier = other.RangeMultiplier;
            RangeBonus = other.RangeBonus;
            FireRateMultiplier = other.FireRateMultiplier;
            FireRateBonus = other.FireRateBonus;
            
            IsAffordable = other.IsAffordable;
            TowerType = other.TowerType;
            IsAvailable = other.IsAvailable;
            
            // Copy special abilities
            SpecialAbilities = new List<string>(other.SpecialAbilities);
            SpecialModifiers = new Dictionary<string, float>(other.SpecialModifiers);
            
            // Copy requirements
            Prerequisites = new List<string>(other.Prerequisites);
            RequiredTowerType = other.RequiredTowerType;
            MinimumTowerLevel = other.MinimumTowerLevel;
            
            // Copy visual and audio
            IconPath = other.IconPath;
            SoundEffect = other.SoundEffect;
            ParticleEffect = other.ParticleEffect;
            Id = other.Id;
        }

        /// <summary>
        /// Initialize upgrade stats based on type.
        /// </summary>
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

        /// <summary>
        /// Initialize special ability upgrades.
        /// </summary>
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

        /// <summary>
        /// Initialize complete upgrade package.
        /// </summary>
        private void InitializeCompleteUpgrade()
        {
            DamageMultiplier = 1.0f + (Level * 0.3f);
            RangeMultiplier = 1.0f + (Level * 0.2f);
            FireRateMultiplier = 1.0f + (Level * 0.35f);
            Description = $"Complete upgrade: +{System.Math.Round((DamageMultiplier - 1) * 100, 0)}% damage, +{System.Math.Round((RangeMultiplier - 1) * 100, 0)}% range, +{System.Math.Round((FireRateMultiplier - 1) * 100, 0)}% fire rate";
        }

        /// <summary>
        /// Calculate upgrade cost based on level and type.
        /// </summary>
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
            
            // Exponential cost scaling
            return (int)(baseCost * System.Math.Pow(1.5, Level - 1));
        }

        /// <summary>
        /// Check if upgrade can be applied to a tower.
        /// </summary>
        public bool CanApplyTo(Tower tower)
        {
            if (tower == null) return false;
            if (Level >= MaxLevel) return false;
            if (tower.Level < MinimumTowerLevel) return false;
            if (RequiredTowerType != TowerType.Basic && tower.Type != RequiredTowerType) return false;
            
            return true;
        }

        /// <summary>
        /// Apply upgrade to tower.
        /// </summary>
        public void ApplyTo(Tower tower)
        {
            if (!CanApplyTo(tower)) return;

            // Apply stat modifications
            if (Type == UpgradeType.Damage || Type == UpgradeType.Complete)
            {
                // Tower would have damage properties that get modified here
            }
            
            if (Type == UpgradeType.Range || Type == UpgradeType.Complete)
            {
                // Tower range would be modified here
            }
            
            if (Type == UpgradeType.FireRate || Type == UpgradeType.Complete)
            {
                // Tower fire rate would be modified here
            }
            
            // Apply special abilities
            foreach (var ability in SpecialAbilities)
            {
                // Add abilities to tower
            }
            
            Level++;
            InitializeUpgradeStats(); // Recalculate for next level
        }

        /// <summary>
        /// Get upgrade preview stats.
        /// </summary>
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

        /// <summary>
        /// Gets the power rating of this upgrade.
        /// </summary>
        /// <returns>Power rating value.</returns>
        public float GetPowerRating()
        {
            float rating = 0f;
            
            // Calculate power rating based on stat modifiers
            rating += (DamageMultiplier - 1.0f) * 100f; // Damage contribution
            rating += (RangeMultiplier - 1.0f) * 80f;   // Range contribution
            rating += (FireRateMultiplier - 1.0f) * 90f; // Fire rate contribution
            rating += SpecialAbilities.Count * 50f;      // Special abilities contribution
            
            return rating;
        }

        /// <summary>
        /// Checks if this upgrade is available for the specified tower and player levels.
        /// Validates level requirements and prerequisites for upgrade availability.
        /// </summary>
        /// <param name="towerLevel">Current level of the tower.</param>
        /// <param name="playerLevel">Current player level.</param>
        /// <returns>True if upgrade is available, false otherwise.</returns>
        internal bool IsAvailableForLevel(int towerLevel, int playerLevel)
        {
            // Check if upgrade level is within allowed range
            if (Level > MaxLevel)
                return false;
                
            // Check minimum tower level requirement
            if (towerLevel < MinimumTowerLevel)
                return false;
                
            // Check if player level is sufficient (assuming player must be at least upgrade level)
            if (playerLevel < Level)
                return false;
                
            // Check if upgrade is marked as available
            if (!IsAvailable)
                return false;
                
            // Check prerequisites (simplified - in full implementation would check actual upgrade ownership)
            if (Prerequisites.Count > 0)
            {
                // For now, assume prerequisites are not met
                // In full implementation, would check if player owns prerequisite upgrades
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Checks if this upgrade can be purchased with the available cash and tower level.
        /// Validates both financial and level requirements for purchase.
        /// </summary>
        /// <param name="playerCash">Amount of cash the player has.</param>
        /// <param name="towerLevel">Current level of the tower.</param>
        /// <returns>True if purchase is possible, false otherwise.</returns>
        internal bool CanPurchase(int playerCash, int towerLevel)
        {
            // Check if player has enough cash
            if (playerCash < Cost)
                return false;
                
            // Check if tower meets minimum level requirement
            if (towerLevel < MinimumTowerLevel)
                return false;
                
            // Check if upgrade is available for purchase
            if (!IsAvailable)
                return false;
                
            // Check if upgrade level is within allowed range
            if (Level > MaxLevel)
                return false;
                
            return true;
        }

        /// <summary>
        /// Processes the purchase of this upgrade.
        /// Deducts cost from player cash and marks upgrade as purchased.
        /// </summary>
        /// <param name="playerCash">Reference to player cash amount.</param>
        /// <returns>True if purchase was successful, false if insufficient funds.</returns>
        internal bool Purchase(ref int playerCash)
        {
            // Check if purchase is possible
            if (!CanPurchase(playerCash, 1)) // Using level 1 as default for cash check
                return false;
                
            // Deduct cost from player cash
            playerCash -= Cost;
            
            // Mark upgrade as unavailable for future purchases (upgrades are typically one-time)
            IsAvailable = false;
            
            return true;
        }

        /// <summary>
        /// Applies this upgrade's effects to the specified tower.
        /// Modifies tower stats based on upgrade type and modifiers.
        /// </summary>
        /// <param name="tower">The tower to apply upgrades to.</param>
        internal void ApplyToTower(Tower tower)
        {
            if (tower == null)
                throw new ArgumentNullException(nameof(tower));
                
            // Apply stat modifications based on upgrade type
            switch (Type)
            {
                case UpgradeType.Damage:
                    tower.Data.Damage = (tower.Data.Damage + DamageBonus) * DamageMultiplier;
                    break;
                    
                case UpgradeType.Range:
                    tower.Data.Range = (tower.Data.Range + RangeBonus) * RangeMultiplier;
                    break;
                    
                case UpgradeType.FireRate:
                    tower.Data.FireRate = (tower.Data.FireRate + FireRateBonus) * FireRateMultiplier;
                    // Convert fire rate to attacks per second if needed
                    if (tower.Data.FireRate > 0)
                        tower.Data.AttackSpeed = 1.0f / tower.Data.FireRate;
                    break;
                    
                case UpgradeType.Special:
                    // Apply special abilities and modifiers
                    ApplySpecialAbilities(tower);
                    break;
                    
                case UpgradeType.Complete:
                    // Apply all stat modifications
                    tower.Data.Damage = (tower.Data.Damage + DamageBonus) * DamageMultiplier;
                    tower.Data.Range = (tower.Data.Range + RangeBonus) * RangeMultiplier;
                    tower.Data.FireRate = (tower.Data.FireRate + FireRateBonus) * FireRateMultiplier;
                    if (tower.Data.FireRate > 0)
                        tower.Data.AttackSpeed = 1.0f / tower.Data.FireRate;
                    ApplySpecialAbilities(tower);
                    break;
            }
            
            // Update tower level to match upgrade level
            tower.Level = Level;
        }

        /// <summary>
        /// Removes this upgrade's effects from the specified tower.
        /// Reverts tower stats to their pre-upgrade state.
        /// </summary>
        /// <param name="tower">The tower to remove upgrades from.</param>
        internal void RemoveFromTower(Tower tower)
        {
            if (tower == null)
                throw new ArgumentNullException(nameof(tower));
                
            // Revert stat modifications (inverse of ApplyToTower)
            switch (Type)
            {
                case UpgradeType.Damage:
                    // Revert damage modifications
                    tower.Data.Damage = tower.Data.Damage / DamageMultiplier - DamageBonus;
                    break;
                    
                case UpgradeType.Range:
                    // Revert range modifications
                    tower.Data.Range = tower.Data.Range / RangeMultiplier - RangeBonus;
                    break;
                    
                case UpgradeType.FireRate:
                    // Revert fire rate modifications
                    tower.Data.FireRate = tower.Data.FireRate / FireRateMultiplier - FireRateBonus;
                    if (tower.Data.FireRate > 0)
                        tower.Data.AttackSpeed = 1.0f / tower.Data.FireRate;
                    break;
                    
                case UpgradeType.Special:
                case UpgradeType.Complete:
                    // Remove special abilities
                    RemoveSpecialAbilities(tower);
                    
                    // For Complete type, also revert basic stats
                    if (Type == UpgradeType.Complete)
                    {
                        tower.Data.Damage = tower.Data.Damage / DamageMultiplier - DamageBonus;
                        tower.Data.Range = tower.Data.Range / RangeMultiplier - RangeBonus;
                        tower.Data.FireRate = tower.Data.FireRate / FireRateMultiplier - FireRateBonus;
                        if (tower.Data.FireRate > 0)
                            tower.Data.AttackSpeed = 1.0f / tower.Data.FireRate;
                    }
                    break;
            }
            
            // Reset tower level (this is a simplification - full implementation would track original level)
            tower.Level = 1;
        }

        /// <summary>
        /// Calculates the efficiency rating of this upgrade.
        /// Higher values indicate more cost-effective upgrades.
        /// </summary>
        /// <returns>Numerical efficiency rating.</returns>
        internal object GetEfficiencyRating()
        {
            // Calculate base efficiency as stat improvement per cost
            float efficiency = 0f;
            
            // Calculate damage efficiency
            if (DamageMultiplier > 1.0f || DamageBonus > 0)
            {
                float damageImprovement = (DamageMultiplier - 1.0f) * 100f + DamageBonus;
                efficiency += damageImprovement / Cost * 1000f; // Scale for meaningful values
            }
            
            // Calculate range efficiency
            if (RangeMultiplier > 1.0f || RangeBonus > 0)
            {
                float rangeImprovement = (RangeMultiplier - 1.0f) * 100f + RangeBonus;
                efficiency += rangeImprovement / Cost * 500f; // Range is less valuable than damage
            }
            
            // Calculate fire rate efficiency
            if (FireRateMultiplier > 1.0f || FireRateBonus > 0)
            {
                float fireRateImprovement = (FireRateMultiplier - 1.0f) * 100f + FireRateBonus;
                efficiency += fireRateImprovement / Cost * 750f; // Fire rate value between damage and range
            }
            
            // Add bonus for special abilities
            efficiency += SpecialAbilities.Count * 50f / Cost * 100f;
            
            // Apply level bonus (higher level upgrades are generally better)
            efficiency *= (1.0f + Level * 0.1f);
            
            return System.Math.Round(efficiency, 2);
        }

        /// <summary>
        /// Applies special abilities to the tower based on upgrade configuration.
        /// Processes special modifiers and abilities defined in the upgrade.
        /// </summary>
        /// <param name="tower">The tower to apply special abilities to.</param>
        private void ApplySpecialAbilities(Tower tower)
        {
            if (tower == null || SpecialAbilities.Count == 0)
                return;
                
            // Apply each special ability
            foreach (var ability in SpecialAbilities)
            {
                switch (ability.ToLower())
                {
                    case "splash":
                        // Add splash damage capability
                        // tower.AddSplashDamage(SpecialModifiers.GetValueOrDefault("splashRadius", 1.0f));
                        break;
                        
                    case "slow":
                        // Add slowing effect
                        // tower.AddSlowEffect(SpecialModifiers.GetValueOrDefault("slowAmount", 0.5f));
                        break;
                        
                    case "poison":
                        // Add poison damage over time
                        // tower.AddPoisonEffect(SpecialModifiers.GetValueOrDefault("poisonDamage", 5.0f));
                        break;
                        
                    case "critical":
                        // Add critical hit chance
                        // tower.AddCriticalChance(SpecialModifiers.GetValueOrDefault("criticalChance", 0.1f));
                        break;
                        
                    case "multishot":
                        // Add multi-shot capability
                        // tower.SetMultiShot((int)SpecialModifiers.GetValueOrDefault("shotCount", 2));
                        break;
                        
                    default:
                        // Handle unknown abilities
                        Console.WriteLine($"Unknown special ability: {ability}");
                        break;
                }
            }
            
            // Apply general special modifiers
            foreach (var modifier in SpecialModifiers)
            {
                // Apply modifier to tower stats
                // tower.SetSpecialModifier(modifier.Key, modifier.Value);
            }
        }

        /// <summary>
        /// Removes special abilities from the tower.
        /// Reverts any special effects applied by this upgrade.
        /// </summary>
        /// <param name="tower">The tower to remove special abilities from.</param>
        private void RemoveSpecialAbilities(Tower tower)
        {
            if (tower == null || SpecialAbilities.Count == 0)
                return;
                
            // Remove each special ability
            foreach (var ability in SpecialAbilities)
            {
                switch (ability.ToLower())
                {
                    case "splash":
                        // Remove splash damage
                        // tower.RemoveSplashDamage();
                        break;
                        
                    case "slow":
                        // Remove slowing effect
                        // tower.RemoveSlowEffect();
                        break;
                        
                    case "poison":
                        // Remove poison effect
                        // tower.RemovePoisonEffect();
                        break;
                        
                    case "critical":
                        // Remove critical hit chance
                        // tower.RemoveCriticalChance();
                        break;
                        
                    case "multishot":
                        // Remove multi-shot capability
                        // tower.SetMultiShot(1);
                        break;
                        
                    default:
                        // Handle unknown abilities
                        Console.WriteLine($"Cannot remove unknown special ability: {ability}");
                        break;
                }
            }
            
            // Clear special modifiers
            // tower.ClearSpecialModifiers();
        }

        /// <summary>
        /// Sets visual properties for the tower when this upgrade is applied.
        /// Updates tower appearance to reflect upgrade status.
        /// </summary>
        /// <param name="tower">The tower being upgraded.</param>
        /// <param name="primaryColor">Primary color for upgrade effects.</param>
        /// <param name="secondaryColor">Secondary color for upgrade effects.</param>
        /// <param name="effectIntensity">Intensity of visual effects.</param>
        /// <param name="glowRadius">Radius of glow effect.</param>
        internal void SetVisualProperties(Tower tower, System.Drawing.Color primaryColor, System.Drawing.Color secondaryColor, float effectIntensity, float glowRadius)
        {
            if (tower == null)
                throw new ArgumentNullException(nameof(tower));
                
            // Store visual properties on the tower (simplified implementation)
            // In full implementation, would update actual rendering components
            
            // Apply color tint based on upgrade type
            switch (Type)
            {
                case UpgradeType.Damage:
                    // Red tint for damage upgrades
                    break;
                    
                case UpgradeType.Range:
                    // Blue tint for range upgrades
                    break;
                    
                case UpgradeType.FireRate:
                    // Yellow tint for fire rate upgrades
                    break;
                    
                case UpgradeType.Special:
                    // Purple tint for special upgrades
                    break;
                    
                case UpgradeType.Complete:
                    // Gold tint for complete upgrades
                    break;
            }
            
            // Store effect parameters for rendering system
            // This would be used by the tower's rendering component
            // tower.SetUpgradeVisuals(primaryColor, secondaryColor, effectIntensity, glowRadius);
        }

        internal void SetVisualProperties(object value1, Color white, object value2, float v)
        {
            throw new NotImplementedException();
        }
    }
}
