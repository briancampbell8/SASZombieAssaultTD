/*
File:    BaseTower.cs
Purpose: Parent class for all towers.
         Defines shared tower identity, category, and placeholder fields.
         
Features: Base tower archetype with shared properties and behaviors.
          Placeholder implementation for tower taxonomy and categorization.
          Used by tower management, upgrade systems, and gameplay mechanics.

Created: Engine Gameplay Implementation
Notes:   This is a placeholder implementation for the base tower class.
         Targeting and upgrade logic will be implemented in separate systems.
*/

using System;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Gameplay.Towers
{
    ///<summary>
    ///Parent class for all towers.
    ///Defines shared tower identity, category, and placeholder fields.
    ///</summary>
    public abstract class BaseTower
    {
        /// Properties
        
        ///<summary>Unique identifier for this tower instance.</summary>
        public string Id { get; protected set; }
        
        ///<summary>Type identifier for this tower.</summary>
        public abstract string TowerType { get; }
        
        ///<summary>Category of this tower.</summary>
        public abstract string Category { get; }
        
        ///<summary>Display name for this tower.</summary>
        public abstract string DisplayName { get; }
        
        ///<summary>Base cost to build this tower.</summary>
        public abstract int BaseCost { get; }
        
        ///<summary>Base attack damage for this tower.</summary>
        public abstract float BaseDamage { get; }
        
        ///<summary>Base attack range for this tower.</summary>
        public abstract float BaseRange { get; }
        
        ///<summary>Base attack speed for this tower.</summary>
        public abstract float BaseAttackSpeed { get; }
        
        ///<summary>Current level of this tower.</summary>
        public int Level { get; protected set; }
        
        ///<summary>Maximum level this tower can reach.</summary>
        public virtual int MaxLevel => 5;
        
        ///<summary>Whether this tower is currently active.</summary>
        public bool IsActive { get; protected set; }
        
        ///<summary>Current upgrade cost for this tower.</summary>
        public virtual int UpgradeCost => BaseCost * (Level + 1);
        
        ///<summary>Current total investment in this tower.</summary>
        public int TotalInvestment { get; protected set; }
        
        ///<summary>Target priority for this tower type.</summary>
        public abstract string TargetPriority { get; }
        
        ///<summary>Damage type for this tower.</summary>
        public abstract string DamageType { get; }
        
        ///<summary>Whether this tower can target air units.</summary>
        public virtual bool CanTargetAir => false;
        
        ///<summary>Whether this tower can target ground units.</summary>
        public virtual bool CanTargetGround => true;
        
        ///

        /// Constructors
        
        ///<summary>
        ///Creates a new base tower instance.
        ///</summary>
        protected BaseTower()
        {
            Id = GenerateId();
            Level = 1;
            IsActive = false;
            TotalInvestment = 0;
        }
        
        ///<summary>
        ///Creates a new base tower instance with specified ID.
        ///</summary>
        ///<param name="id">Unique identifier.</param>
        protected BaseTower(string id)
        {
            Id = id ?? GenerateId();
            Level = 1;
            IsActive = false;
            TotalInvestment = 0;
        }
        
        ///

        /// Tower Operations (Placeholder)
        
        ///<summary>
        ///Activates this tower.
        ///</summary>
        public virtual void Activate()
        {
            IsActive = true;
        }
        
        ///<summary>
        ///Deactivates this tower.
        ///</summary>
        public virtual void Deactivate()
        {
            IsActive = false;
        }
        
        ///<summary>
        ///Upgrades this tower to the next level.
        ///</summary>
        ///<returns>True if upgrade was successful.</returns>
        public virtual bool Upgrade()
        {
            if (Level >= MaxLevel)
                return false;
                
            Level++;
            TotalInvestment += UpgradeCost;
            return true;
        }
        
        ///<summary>
        ///Sells this tower and returns the refund amount.
        ///</summary>
        ///<returns>Refund amount.</returns>
        public virtual int Sell()
        {
            var refund = TotalInvestment / 2; //50% refund
            Deactivate();
            return refund;
        }
        
        ///<summary>
        ///Gets the current stats for this tower.
        ///</summary>
        ///<returns>Tower stats dictionary.</returns>
        public virtual System.Collections.Generic.Dictionary<string, object> GetCurrentStats()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["Level"] = Level,
                ["Damage"] = GetDamageAtLevel(Level),
                ["Range"] = GetRangeAtLevel(Level),
                ["AttackSpeed"] = GetAttackSpeedAtLevel(Level),
                ["Cost"] = TotalInvestment,
                ["UpgradeCost"] = UpgradeCost,
                ["IsActive"] = IsActive
            };
        }
        
        ///<summary>
        ///Gets the upgrade requirements for this tower.
        ///</summary>
        ///<returns>Upgrade requirements dictionary.</returns>
        public virtual System.Collections.Generic.Dictionary<string, object> GetUpgradeRequirements()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["CanUpgrade"] = Level < MaxLevel,
                ["UpgradeCost"] = UpgradeCost,
                ["RequiredLevel"] = Level + 1,
                ["UpgradeBenefits"] = GetUpgradeBenefits()
            };
        }
        
        ///<summary>
        ///Gets the targeting characteristics for this tower type.
        ///</summary>
        ///<returns>Targeting characteristics dictionary.</returns>
        public virtual System.Collections.Generic.Dictionary<string, object> GetTargetingCharacteristics()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["TargetPriority"] = TargetPriority,
                ["CanTargetAir"] = CanTargetAir,
                ["CanTargetGround"] = CanTargetGround,
                ["TargetingMode"] = "Closest",
                ["TargetSwitchSpeed"] = 1.0f
            };
        }
        
        ///<summary>
        ///Gets the behavior characteristics for this tower type.
        ///</summary>
        ///<returns>Behavior characteristics dictionary.</returns>
        public abstract System.Collections.Generic.Dictionary<string, object> GetBehaviorCharacteristics();
        
        ///<summary>
        ///Gets the combat characteristics for this tower type.
        ///</summary>
        ///<returns>Combat characteristics dictionary.</returns>
        public abstract System.Collections.Generic.Dictionary<string, object> GetCombatCharacteristics();
        
        ///<summary>
        ///Gets the upgrade characteristics for this tower type.
        ///</summary>
        ///<returns>Upgrade characteristics dictionary.</returns>
        public abstract System.Collections.Generic.Dictionary<string, object> GetUpgradeCharacteristics();
        
        ///

        /// Level-based Calculations
        
        ///<summary>
        ///Gets the damage at a specific level.
        ///</summary>
        ///<param name="level">Tower level.</param>
        ///<returns>Damage at specified level.</returns>
        protected virtual float GetDamageAtLevel(int level)
        {
            return BaseDamage * (1f + (level - 1) * 0.2f);
        }
        
        ///<summary>
        ///Gets the range at a specific level.
        ///</summary>
        ///<param name="level">Tower level.</param>
        ///<returns>Range at specified level.</returns>
        protected virtual float GetRangeAtLevel(int level)
        {
            return BaseRange * (1f + (level - 1) * 0.1f);
        }
        
        ///<summary>
        ///Gets the attack speed at a specific level.
        ///</summary>
        ///<param name="level">Tower level.</param>
        ///<returns>Attack speed at specified level.</returns>
        protected virtual float GetAttackSpeedAtLevel(int level)
        {
            return BaseAttackSpeed * (1f + (level - 1) * 0.15f);
        }
        
        ///<summary>
        ///Gets the upgrade benefits for the next level.
        ///</summary>
        ///<returns>Upgrade benefits list.</returns>
        protected virtual System.Collections.Generic.List<string> GetUpgradeBenefits()
        {
            var benefits = new System.Collections.Generic.List<string>();
            
            if (Level < MaxLevel)
            {
                benefits.Add($"Damage: {GetDamageAtLevel(Level + 1):F1}");
                benefits.Add($"Range: {GetRangeAtLevel(Level + 1):F1}");
                benefits.Add($"Attack Speed: {GetAttackSpeedAtLevel(Level + 1):F1}");
            }
            
            return benefits;
        }
        
        ///

        /// Utility Methods
        
        ///<summary>
        ///Generates a unique ID for this tower instance.
        ///</summary>
        ///<returns>Unique identifier string.</returns>
        private static string GenerateId()
        {
            return $"Tower_{Guid.NewGuid():N}";
        }
        
        ///<summary>
        ///Gets a summary of this tower type.
        ///</summary>
        ///<returns>Summary string.</returns>
        public override string ToString()
        {
            return $"{DisplayName} (ID: {Id}, Level: {Level}/{MaxLevel}, Cost: {BaseCost})";
        }
        
        ///
    }
}
