/*
File:    VickersTurret.cs
Purpose: Rapid-fire ballistic turret.
         Defines the identity and category of a fast-shooting tower.
         
Features: Vickers turret archetype with rapid-fire ballistic characteristics.
          Placeholder implementation for tower taxonomy and categorization.
          Used by tower management, upgrade systems, and gameplay mechanics.

Created: Engine Gameplay Implementation
Notes:   This is a placeholder implementation for vickers-type towers.
         Firing logic and projectile systems will be implemented in separate systems.
*/

using System;

namespace SASZombieAssaultTD.Engine.Gameplay.Towers
{
    /// <summary>
    /// Rapid-fire ballistic turret.
    /// Defines the identity and category of a fast-shooting tower.
    /// </summary>
    public class VickersTurret : BaseTower
    {
        #region Base Tower Properties
        
        /// <summary>Type identifier for this tower.</summary>
        public override string TowerType => "VickersTurret";
        
        /// <summary>Category of this tower.</summary>
        public override string Category => "Ballistic";
        
        /// <summary>Display name for this tower.</summary>
        public override string DisplayName => "Vickers Turret";
        
        /// <summary>Base cost to build this tower.</summary>
        public override int BaseCost => 150;
        
        /// <summary>Base attack damage for this tower.</summary>
        public override float BaseDamage => 15f;
        
        /// <summary>Base attack range for this tower.</summary>
        public override float BaseRange => 4.0f;
        
        /// <summary>Base attack speed for this tower.</summary>
        public override float BaseAttackSpeed => 3.0f;
        
        /// <summary>Target priority for this tower type.</summary>
        public override string TargetPriority => "Closest";
        
        /// <summary>Damage type for this tower.</summary>
        public override string DamageType => "Ballistic";
        
        /// <summary>Whether this tower can target air units.</summary>
        public override bool CanTargetAir => false;
        
        /// <summary>Whether this tower can target ground units.</summary>
        public override bool CanTargetGround => true;
        
        #endregion

        #region Vickers Turret Specific Properties
        
        /// <summary>Fire rate in rounds per minute.</summary>
        public int FireRate => 180;
        
        /// <summary>Ammo capacity before reload.</summary>
        public int AmmoCapacity => 30;
        
        /// <summary>Reload time in seconds.</summary>
        public float ReloadTime => 2.0f;
        
        /// <summary>Accuracy rating (0.0-1.0).</summary>
        public float Accuracy => 0.85f;
        
        /// <summary>Projectile speed multiplier.</summary>
        public float ProjectileSpeed => 1.2f;
        
        /// <summary>Penetration power of projectiles.</summary>
        public float Penetration => 0.3f;
        
        #endregion

        #region Constructors
        
        /// <summary>
        /// Creates a new vickers turret instance.
        /// </summary>
        public VickersTurret() : base()
        {
        }
        
        /// <summary>
        /// Creates a new vickers turret instance with specified ID.
        /// </summary>
        /// <param name="id">Unique identifier.</param>
        public VickersTurret(string id) : base(id)
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
                ["FiringPattern"] = "Burst",
                ["TargetAcquisition"] = "Fast",
                ["TrackingSpeed"] = "High",
                ["ReloadBehavior"] = "Auto",
                ["Priority"] = "LowHealth"
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
                ["AttackType"] = "Ballistic",
                ["AttackRange"] = GetRangeAtLevel(Level),
                ["AttackSpeed"] = GetAttackSpeedAtLevel(Level),
                ["Accuracy"] = Accuracy,
                ["ProjectileSpeed"] = ProjectileSpeed,
                ["Penetration"] = Penetration,
                ["FireRate"] = FireRate,
                ["AmmoCapacity"] = AmmoCapacity,
                ["ReloadTime"] = ReloadTime
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
                ["UpgradePath"] = "Damage_FireRate_Range",
                ["SpecialUpgrades"] = new[] { "RapidFire", "ArmorPiercing", "ExtendedMagazine" },
                ["MaxLevel"] = MaxLevel
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
            return BaseDamage * (1f + (level - 1) * 0.15f);
        }
        
        /// <summary>
        /// Gets the range at a specific level.
        /// </summary>
        /// <param name="level">Tower level.</param>
        /// <returns>Range at specified level.</returns>
        protected override float GetRangeAtLevel(int level)
        {
            return BaseRange * (1f + (level - 1) * 0.08f);
        }
        
        /// <summary>
        /// Gets the attack speed at a specific level.
        /// </summary>
        /// <param name="level">Tower level.</param>
        /// <returns>Attack speed at specified level.</returns>
        protected override float GetAttackSpeedAtLevel(int level)
        {
            return BaseAttackSpeed * (1f + (level - 1) * 0.25f);
        }
        
        /// <summary>
        /// Gets the upgrade benefits for the next level.
        /// </summary>
        /// <returns>Upgrade benefits list.</returns>
        protected override System.Collections.Generic.List<string> GetUpgradeBenefits()
        {
            var benefits = base.GetUpgradeBenefits();
            
            // Add vickers-specific benefits
            benefits.Add($"Fire Rate: {FireRate + (Level * 10)} RPM");
            benefits.Add($"Accuracy: {Accuracy + (Level * 0.02f):F2}");
            benefits.Add($"Penetration: {Penetration + (Level * 0.05f):F2f}");
            
            return benefits;
        }
        
        #endregion

        #region Utility Methods
        
        /// <summary>
        /// Creates a copy of this tower instance.
        /// </summary>
        /// <returns>New vickers turret instance.</returns>
        public VickersTurret Clone()
        {
            return new VickersTurret(Id);
        }
        
        /// <summary>
        /// Gets a summary of this tower type.
        /// </summary>
        /// <returns>Summary string.</returns>
        public override string ToString()
        {
            return $"{DisplayName} (ID: {Id}, Level: {Level}/{MaxLevel}, Fire Rate: {FireRate} RPM)";
        }
        
        #endregion
    }
}