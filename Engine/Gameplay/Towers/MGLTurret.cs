/*
File:    MGLTurret.cs
Purpose: Grenade launcher turret with splash damage.
         Represents an explosive-area turret archetype.
         
Features: MGL turret archetype with grenade launcher and splash damage characteristics.
          Placeholder implementation for tower taxonomy and categorization.
          Used by tower management, upgrade systems, and gameplay mechanics.

Created: Engine Gameplay Implementation
Notes:   This is a placeholder implementation for MGL-type towers.
         Projectile and splash logic will be implemented in separate systems.
*/

using System;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Gameplay.Towers
{
    ///<summary>
    ///Grenade launcher turret with splash damage.
    ///Represents an explosive-area turret archetype.
    ///</summary>
    public class MGLTurret : BaseTower
    {
        /// Base Tower Properties
        
        ///<summary>Type identifier for this tower.</summary>
        public override string TowerType => "MGLTurret";
        
        ///<summary>Category of this tower.</summary>
        public override string Category => "Explosive";
        
        ///<summary>Display name for this tower.</summary>
        public override string DisplayName => "MGL Turret";
        
        ///<summary>Base cost to build this tower.</summary>
        public override int BaseCost => 250;
        
        ///<summary>Base attack damage for this tower.</summary>
        public override float BaseDamage => 40f;
        
        ///<summary>Base attack range for this tower.</summary>
        public override float BaseRange => 6.0f;
        
        ///<summary>Base attack speed for this tower.</summary>
        public override float BaseAttackSpeed => 0.8f;
        
        ///<summary>Target priority for this tower type.</summary>
        public override string TargetPriority => "Groups";
        
        ///<summary>Damage type for this tower.</summary>
        public override string DamageType => "Explosive";
        
        ///<summary>Whether this tower can target air units.</summary>
        public override bool CanTargetAir => true;
        
        ///<summary>Whether this tower can target ground units.</summary>
        public override bool CanTargetGround => true;
        
        ///

        /// MGL Turret Specific Properties
        
        ///<summary>Splash damage radius of grenades.</summary>
        public float SplashRadius = 2.5f;
        
        ///<summary>Splash damage falloff multiplier.</summary>
        public float SplashDamageFalloff = 0.5f;
        
        ///<summary>Projectile arc angle in degrees.</summary>
        public float ProjectileArc = 45f;
        
        ///<summary>Grenade travel speed multiplier.</summary>
        public float ProjectileSpeed = 0.8f;
        
        ///<summary>Reload time between shots.</summary>
        public float ReloadTime = 3.0f;
        
        ///<summary>Maximum grenade count before reload.</summary>
        public int MagazineCapacity = 6;
        
        ///<summary>Accuracy rating (0.0-1.0).</summary>
        public float Accuracy = 0.7f;
        
        ///<summary>Whether grenades bounce.</summary>
        public bool CanBounce = true;
        
        ///<summary>Maximum bounce count.</summary>
        public int MaxBounceCount = 1;
        
        ///

        /// Constructors
        
        ///<summary>
        ///Creates a new MGL turret instance.
        ///</summary>
        public MGLTurret() : base()
        {
        }
        
        ///<summary>
        ///Creates a new MGL turret instance with specified ID.
        ///</summary>
        ///<param name="id">Unique identifier.</param>
        public MGLTurret(string id) : base(id)
        {
        }
        
        ///

        /// Tower Operations (Placeholder)
        
        ///<summary>
        ///Gets the behavior characteristics for this tower type.
        ///</summary>
        ///<returns>Behavior characteristics dictionary.</returns>
        public override System.Collections.Generic.Dictionary<string, object> GetBehaviorCharacteristics()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["FiringPattern"] = "AreaTargeting",
                ["TargetAcquisition"] = "AreaScan",
                ["TrackingSpeed"] = "Medium",
                ["ReloadBehavior"] = "Manual",
                ["Priority"] = "DenseGroups"
            };
        }
        
        ///<summary>
        ///Gets the combat characteristics for this tower type.
        ///</summary>
        ///<returns>Combat characteristics dictionary.</returns>
        public override System.Collections.Generic.Dictionary<string, object> GetCombatCharacteristics()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["AttackType"] = "Explosive",
                ["AttackRange"] = GetRangeAtLevel(Level),
                ["AttackSpeed"] = GetAttackSpeedAtLevel(Level),
                ["Accuracy"] = Accuracy,
                ["SplashRadius"] = SplashRadius,
                ["SplashDamage"] = BaseDamage * SplashDamageFalloff,
                ["ProjectileArc"] = ProjectileArc,
                ["ProjectileSpeed"] = ProjectileSpeed,
                ["MagazineCapacity"] = MagazineCapacity,
                ["ReloadTime"] = ReloadTime,
                ["CanBounce"] = CanBounce,
                ["MaxBounceCount"] = MaxBounceCount
            };
        }
        
        ///<summary>
        ///Gets the upgrade characteristics for this tower type.
        ///</summary>
        ///<returns>Upgrade characteristics dictionary.</returns>
        public override System.Collections.Generic.Dictionary<string, object> GetUpgradeCharacteristics()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["UpgradePath"] = "Damage_SplashRadius_Range",
                ["SpecialUpgrades"] = new[] { "Napalm", "Airburst", "MultiShot" },
                ["MaxLevel"] = MaxLevel
            };
        }
        
        ///<summary>
        ///Gets the area effect characteristics for this tower type.
        ///</summary>
        ///<returns>Area effect characteristics dictionary.</returns>
        public virtual System.Collections.Generic.Dictionary<string, object> GetAreaEffectCharacteristics()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["EffectType"] = "Explosion",
                ["Radius"] = SplashRadius,
                ["DamageFalloff"] = SplashDamageFalloff,
                ["DamageType"] = "Explosive",
                ["CanBounce"] = CanBounce,
                ["MaxBounceCount"] = MaxBounceCount
            };
        }
        
        ///

        /// Level-based Calculations
        
        ///<summary>
        ///Gets the damage at a specific level.
        ///</summary>
        ///<param name="level">Tower level.</param>
        ///<returns>Damage at specified level.</returns>
        protected override float GetDamageAtLevel(int level)
        {
            return BaseDamage * (1f + (level - 1) * 0.25f);
        }
        
        ///<summary>
        ///Gets the range at a specific level.
        ///</summary>
        ///<param name="level">Tower level.</param>
        ///<returns>Range at specified level.</returns>
        protected override float GetRangeAtLevel(int level)
        {
            return BaseRange * (1f + (level - 1) * 0.12f);
        }
        
        ///<summary>
        ///Gets the attack speed at a specific level.
        ///</summary>
        ///<param name="level">Tower level.</param>
        ///<returns>Attack speed at specified level.</returns>
        protected override float GetAttackSpeedAtLevel(int level)
        {
            return BaseAttackSpeed * (1f + (level - 1) * 0.1f);
        }
        
        ///<summary>
        ///Gets the upgrade benefits for the next level.
        ///</summary>
        ///<returns>Upgrade benefits list.</returns>
        protected override System.Collections.Generic.List<string> GetUpgradeBenefits()
        {
            var benefits = base.GetUpgradeBenefits();
            
            //Add MGL-specific benefits
            benefits.Add($"Splash Radius: {SplashRadius + (Level * 0.5f):F1}m");
            benefits.Add($"Magazine Capacity: {MagazineCapacity + (Level)}");
            benefits.Add($"Bounce Count: {MaxBounceCount + (Level > 2 ? 1 : 0)}");
            
            return benefits;
        }
        
        ///

        /// Utility Methods
        
        ///<summary>
        ///Creates a copy of this tower instance.
        ///</summary>
        ///<returns>New MGL turret instance.</returns>
        public MGLTurret Clone()
        {
            return new MGLTurret(Id);
        }
        
        ///<summary>
        ///Gets a summary of this tower type.
        ///</summary>
        ///<returns>Summary string.</returns>
        public override string ToString()
        {
            return $"{DisplayName} (ID: {Id}, Level: {Level}/{MaxLevel}, Splash: {SplashRadius:F1}m)";
        }
        
        ///
    }
}
