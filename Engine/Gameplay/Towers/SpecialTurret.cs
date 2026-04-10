/*
File:    SpecialTurret.cs
Purpose: Unique or exotic turret type.
         Placeholder for special-case tower behaviors.
         
Features: Special turret archetype with unique or exotic characteristics.
          Placeholder implementation for tower taxonomy and categorization.
          Used by tower management, upgrade systems, and gameplay mechanics.

Created: Engine Gameplay Implementation
Notes:   This is a placeholder implementation for special-type towers.
         Special mechanics and unique abilities will be implemented in separate systems.
*/

using System;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Gameplay.Towers
{
    /// <summary>
    /// Unique or exotic turret type.
    /// Placeholder for special-case tower behaviors.
    /// </summary>
    public class SpecialTurret : BaseTower
    {
        #region Base Tower Properties
        
        /// <summary>Type identifier for this tower.</summary>
        public override string TowerType => "SpecialTurret";
        
        /// <summary>Category of this tower.</summary>
        public override string Category => "Special";
        
        /// <summary>Display name for this tower.</summary>
        public override string DisplayName => "Special Turret";
        
        /// <summary>Base cost to build this tower.</summary>
        public override int BaseCost => 300;

        private string currentMode;

        public string GetCurrentMode()
        {
            return currentMode;
        }

        private void SetCurrentMode(string value)
        {
            currentMode = value;
        }

        /// <summary>Base attack damage for this tower.</summary>
        public override float BaseDamage => 30f;
        
        /// <summary>Base attack range for this tower.</summary>
        public override float BaseRange => 5.0f;
        
        /// <summary>Base attack speed for this tower.</summary>
        public override float BaseAttackSpeed => 1.5f;
        
        /// <summary>Target priority for this tower type.</summary>
        public override string TargetPriority => "Special";
        
        /// <summary>Damage type for this tower.</summary>
        public override string DamageType => "Special";
        
        /// <summary>Whether this tower can target air units.</summary>
        public override bool CanTargetAir => true;
        
        /// <summary>Whether this tower can target ground units.</summary>
        public override bool CanTargetGround => true;
        
        #endregion

        #region Special Turret Specific Properties
        
        /// <summary>Special ability type for this tower.</summary>
        public string SpecialAbility => "Unknown";
        
        /// <summary>Energy consumption rate.</summary>
        public float EnergyConsumption => 5.0f;
        
        /// <summary>Special effect radius.</summary>
        public float EffectRadius => 3.0f;
        
        /// <summary>Cooldown time between special attacks.</summary>
        public float SpecialCooldown => 10.0f;
        
        /// <summary>Whether this tower has multiple modes.</summary>
        public bool HasMultipleModes => true;
        
        /// <summary>Available modes for this tower.</summary>
        public string[] AvailableModes => new[] { "Normal", "Special", "Ultimate" };

        /// <summary>Current active mode.</summary>
        public static string CurrentMode
        {
            get
            {
                return "Normal";
            }
        }

        /// <summary>Whether this tower requires manual activation.</summary>
        public bool RequiresManualActivation => false;
        
        /// <summary>Energy cost for special ability.</summary>
        public float SpecialEnergyCost => 50f;
        
        #endregion

        #region Constructors
        
        /// <summary>
        /// Creates a new special turret instance.
        /// </summary>
        public SpecialTurret() : base()
        {
        }
        
        /// <summary>
        /// Creates a new special turret instance with specified ID.
        /// </summary>
        /// <param name="id">Unique identifier.</param>
        public SpecialTurret(string id) : base(id)
        {
        }
        
        #endregion

        #region Tower Operations (Placeholder)
        
        /// <summary>
        /// Gets the behavior characteristics for this tower type.
        /// </summary>
        /// <returns>Behavior characteristics dictionary.</returns>
        public override System.Collections.Generic.Dictionary<string, object> GetBehaviorCharacteristics()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["FiringPattern"] = "Adaptive",
                ["TargetAcquisition"] = "Smart",
                ["TrackingSpeed"] = "VeryHigh",
                ["ReloadBehavior"] = "Auto",
                ["Priority"] = "SpecialTargets"
            };
        }
        
        /// <summary>
        /// Gets the combat characteristics for this tower type.
        /// </summary>
        /// <returns>Combat characteristics dictionary.</returns>
        public override System.Collections.Generic.Dictionary<string, object> GetCombatCharacteristics()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["AttackType"] = "Special",
                ["AttackRange"] = GetRangeAtLevel(Level),
                ["AttackSpeed"] = GetAttackSpeedAtLevel(Level),
                ["Accuracy"] = 0.9f,
                ["SpecialAbility"] = SpecialAbility,
                ["EffectRadius"] = EffectRadius,
                ["EnergyConsumption"] = EnergyConsumption,
                ["SpecialCooldown"] = SpecialCooldown,
                ["EnergyCost"] = SpecialEnergyCost
            };
        }
        
        /// <summary>
        /// Gets the upgrade characteristics for this tower type.
        /// </summary>
        /// <returns>Upgrade characteristics dictionary.</returns>
        public override System.Collections.Generic.Dictionary<string, object> GetUpgradeCharacteristics()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["UpgradePath"] = "SpecialAbility_Energy_Efficiency",
                ["SpecialUpgrades"] = new[] { "ModeUnlock", "EnergyEfficiency", "EffectRadius" },
                ["MaxLevel"] = MaxLevel
            };
        }
        
        /// <summary>
        /// Gets the special characteristics for this tower type.
        /// </summary>
        /// <returns>Special characteristics dictionary.</returns>
        public virtual System.Collections.Generic.Dictionary<string, object> GetSpecialCharacteristics()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["SpecialAbility"] = SpecialAbility,
                ["EffectRadius"] = EffectRadius,
                ["EnergyCost"] = SpecialEnergyCost,
                ["Cooldown"] = SpecialCooldown,
                ["Modes"] = AvailableModes,
                ["CurrentMode"] = GetCurrentMode(),
                ["ManualActivation"] = RequiresManualActivation
            };
        }
        
        #endregion

        #region Level-based Calculations
        
        /// <summary>
        /// Gets the damage at a specific level.
        /// </summary>
        /// <param name="level">Tower level.</param>
        /// <returns>Damage at specified level.</returns>
        protected override float GetDamageAtLevel(int level)
        {
            var baseDamage = base.GetDamageAtLevel(level);
            
            // Special towers have unique damage scaling
            return baseDamage * (1f + (level - 1) * 0.3f);
        }
        
        /// <summary>
        /// Gets the range at a specific level.
        /// </summary>
        /// <param name="level">Tower level.</param>
        /// <returns>Range at specified level.</returns>
        protected override float GetRangeAtLevel(int level)
        {
            var baseRange = base.GetRangeAtLevel(level);
            
            // Special towers have enhanced range scaling
            return baseRange * (1f + (level - 1) * 0.15f);
        }
        
        /// <summary>
        /// Gets the attack speed at a specific level.
        /// </summary>
        /// <param name="level">Tower level.</param>
        /// <returns>Attack speed at specified level.</returns>
        protected override float GetAttackSpeedAtLevel(int level)
        {
            var baseSpeed = base.GetAttackSpeedAtLevel(level);
            
            // Special towers have enhanced attack speed scaling
            return baseSpeed * (1f + (level - 1) * 0.2f);
        }
        
        /// <summary>
        /// Gets the upgrade benefits for the next level.
        /// </summary>
        /// <returns>Upgrade benefits list.</returns>
        protected override System.Collections.Generic.List<string> GetUpgradeBenefits()
        {
            var benefits = base.GetUpgradeBenefits();
            
            // Add special tower-specific benefits
            benefits.Add($"Special Ability: {SpecialAbility}");
            benefits.Add($"Effect Radius: {EffectRadius + (Level * 0.5f):F1}m");
            benefits.Add($"Energy Efficiency: {(EnergyConsumption - (Level * 0.5f)):F1}");
            
            return benefits;
        }
        
        #endregion

        #region Special Mode Management
        
        /// <summary>
        /// Switches to the specified mode if available.
        /// </summary>
        /// <param name="mode">Mode to switch to.</param>
        /// <returns>True if mode switch was successful.</returns>
        public virtual bool SwitchMode(string mode)
        {
            if (!AvailableModes.Contains(mode))
                return false;

            SetCurrentMode(mode);
            return true;
        }
        
        /// <summary>
        /// Activates the special ability if energy is available.
        /// </summary>
        /// <returns>True if ability was activated.</returns>
        public virtual bool ActivateSpecialAbility()
        {
            if (GetCurrentMode() != "Special")
                return false;
                
            if (TotalInvestment >= SpecialEnergyCost)
            {
                TotalInvestment -= (int)SpecialEnergyCost;
                return true;
            }
            
            return false;
        }
        
        #endregion

        #region Utility Methods
        
        /// <summary>
        /// Creates a copy of this tower instance.
        /// </summary>
        /// <returns>New special turret instance.</returns>
        public SpecialTurret Clone()
        {
            return new SpecialTurret(Id);
        }
        
        /// <summary>
        /// Gets a summary of this tower type.
        /// </summary>
        /// <returns>Summary string.</returns>
        public override string ToString()
        {
            return $"{DisplayName} (ID: {Id}, Level: {Level}/{MaxLevel}, Mode: {GetCurrentMode()})";
        }
        
        #endregion
    }
}