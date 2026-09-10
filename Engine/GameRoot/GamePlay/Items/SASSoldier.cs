// ====================================================================================================
//  FILE: SASSoldier.cs
//  PATH: ./Engine/Gameplay/Items/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the SASSoldier module.
//
//  RESPONSIBILITIES:
//      - Provide Activate() behavior for the Core subsystem.
//      - Provide Deactivate() behavior for the Core subsystem.
//      - Provide TakeDamage() behavior for the Core subsystem.
//      - Provide Heal() behavior for the Core subsystem.
//      - Provide Reload() behavior for the Core subsystem.
//      - Provide Fire() behavior for the Core subsystem.
//      - Provide AddExperience() behavior for the Core subsystem.
//      - Provide GetMaxHealthAtLevel() behavior for the Core subsystem.
//      - Provide GetDamageAtLevel() behavior for the Core subsystem.
//      - Provide GetRangeAtLevel() behavior for the Core subsystem.
//      - Provide GetAttackSpeedAtLevel() behavior for the Core subsystem.
//      - Provide GetSpeedAtLevel() behavior for the Core subsystem.
//      - Provide GetRequiredExperienceForLevel() behavior for the Core subsystem.
//      - Provide Clone() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    SASSoldier.cs
Purpose: Player-controlled or AI-controlled soldier unit.
         Represents a general-purpose soldier archetype.
         
Features: SAS soldier archetype with general-purpose combat characteristics.
          Placeholder implementation for unit taxonomy and categorization.
          Used by unit management, squad systems, and gameplay mechanics.

Created: Engine Gameplay Implementation
Notes:   This is a placeholder implementation for SAS soldier units.
         Movement and combat logic will be implemented in separate systems.
*/

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.GameRoot.GamePlay.Items
{
    ///<summary>
    ///Player-controlled or AI-controlled soldier unit.
    ///Represents a general-purpose soldier archetype.
    ///</summary>
    public class SASSoldier
    {
        /// Properties
        
        ///<summary>Unique identifier for this soldier instance.</summary>
        public string Id { get; private set; }
        
        ///<summary>Type identifier for this unit.</summary>
        public string UnitType => "SASSoldier";
        
        ///<summary>Category of this unit.</summary>
        public string Category => "Infantry";
        
        ///<summary>Display name for this unit.</summary>
        public string DisplayName => "SAS Soldier";
        
        ///<summary>Base health value for this unit type.</summary>
        public float BaseHealth => 100f;
        
        ///<summary>Base movement speed multiplier.</summary>
        public float BaseSpeed => 1.0f;
        
        ///<summary>Base damage value for this unit type.</summary>
        public float BaseDamage => 25f;
        
        ///<summary>Base attack range for this unit.</summary>
        public float BaseRange => 3.0f;
        
        ///<summary>Base attack speed for this unit.</summary>
        public float BaseAttackSpeed => 1.2f;
        
        ///<summary>Whether this unit is player-controlled.</summary>
        public bool IsPlayerControlled { get; private set; }
        
        ///<summary>Current level of this unit.</summary>
        public int Level { get; private set; }
        
        ///<summary>Experience points for this unit.</summary>
        public int Experience { get; private set; }
        
        ///<summary>Current health of this unit.</summary>
        public float CurrentHealth { get; private set; }
        
        ///<summary>Whether this unit is currently active.</summary>
        public bool IsActive { get; private set; }
        
        ///<summary>Weapon type for this unit.</summary>
        public string WeaponType => "AssaultRifle";
        
        ///<summary>Ammo capacity for this unit.</summary>
        public int AmmoCapacity => 30;
        
        ///<summary>Current ammo count.</summary>
        public int CurrentAmmo { get; private set; }
        
        ///<summary>Accuracy rating (0.0-1.0).</summary>
        public float Accuracy => 0.8f;
        
        ///<summary>Whether this unit can target air units.</summary>
        public bool CanTargetAir => false;
        
        ///<summary>Whether this unit can target ground units.</summary>
        public bool CanTargetGround => true;
        
        ///

        /// Constructors
        
        ///<summary>
        ///Creates a new SAS soldier instance.
        ///</summary>
        public SASSoldier()
        {
            Id = GenerateId();
            Level = 1;
            Experience = 0;
            CurrentHealth = BaseHealth;
            CurrentAmmo = AmmoCapacity;
            IsActive = false;
            IsPlayerControlled = false;
        }
        
        ///<summary>
        ///Creates a new SAS soldier instance with specified control type.
        ///</summary>
        ///<param name="isPlayerControlled">Whether this unit is player-controlled.</param>
        public SASSoldier(bool isPlayerControlled)
        {
            Id = GenerateId();
            Level = 1;
            Experience = 0;
            CurrentHealth = BaseHealth;
            CurrentAmmo = AmmoCapacity;
            IsActive = false;
            IsPlayerControlled = isPlayerControlled;
        }
        
        ///<summary>
        ///Creates a new SAS soldier instance with specified ID and control type.
        ///</summary>
        ///<param name="id">Unique identifier.</param>
        ///<param name="isPlayerControlled">Whether this unit is player-controlled.</param>
        public SASSoldier(string id, bool isPlayerControlled = false)
        {
            Id = id ?? GenerateId();
            Level = 1;
            Experience = 0;
            CurrentHealth = BaseHealth;
            CurrentAmmo = AmmoCapacity;
            IsActive = false;
            IsPlayerControlled = isPlayerControlled;
        }
        
        ///

        /// Unit Operations (Placeholder)
        
        ///<summary>
        ///Activates this unit.
        ///</summary>
        public virtual void Activate()
        {
            IsActive = true;
        }
        
        ///<summary>
        ///Deactivates this unit.
        ///</summary>
        public virtual void Deactivate()
        {
            IsActive = false;
        }
        
        ///<summary>
        ///Applies damage to this unit.
        ///</summary>
        ///<param name="damage">Damage amount.</param>
        ///<returns>True if unit was destroyed.</returns>
        public virtual bool TakeDamage(float damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                Deactivate();
                return true;
            }
            return false;
        }
        
        ///<summary>
        ///Heals this unit.
        ///</summary>
        ///<param name="amount">Heal amount.</param>
        ///<returns>Actual amount healed.</returns>
        public virtual float Heal(float amount)
        {
            var maxHealth = GetMaxHealthAtLevel(Level);
            var healAmount = System.Math.Min(amount, maxHealth - CurrentHealth);
            CurrentHealth += healAmount;
            return healAmount;
        }
        
        ///<summary>
        ///Reloads this unit's weapon.
        ///</summary>
        public virtual void Reload()
        {
            CurrentAmmo = AmmoCapacity;
        }
        
        ///<summary>
        ///Fires this unit's weapon.
        ///</summary>
        ///<returns>True if shot was fired successfully.</returns>
        public virtual bool Fire()
        {
            if (CurrentAmmo <= 0)
                return false;
                
            CurrentAmmo--;
            return true;
        }
        
        ///<summary>
        ///Adds experience to this unit.
        ///</summary>
        ///<param name="amount">Experience amount.</param>
        ///<returns>True if unit leveled up.</returns>
        public virtual bool AddExperience(int amount)
        {
            Experience += amount;
            var requiredExp = GetRequiredExperienceForLevel(Level + 1);
            
            if (Experience >= requiredExp)
            {
                Level++;
                return true;
            }
            
            return false;
        }
        
        ///

        /// Unit Characteristics
        
        ///<summary>
        ///Gets the behavior characteristics for this unit type.
        ///</summary>
        ///<returns>Behavior characteristics dictionary.</returns>
        public virtual System.Collections.Generic.Dictionary<string, object> GetBehaviorCharacteristics()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["MovementPattern"] = "Tactical",
                ["TargetPriority"] = "ClosestThreat",
                ["CombatStyle"] = "Balanced",
                ["Intelligence"] = IsPlayerControlled ? "Player" : "AI",
                ["Cooperation"] = "Squad"
            };
        }
        
        ///<summary>
        ///Gets the combat characteristics for this unit type.
        ///</summary>
        ///<returns>Combat characteristics dictionary.</returns>
        public virtual System.Collections.Generic.Dictionary<string, object> GetCombatCharacteristics()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["AttackType"] = "Ballistic",
                ["AttackRange"] = GetRangeAtLevel(Level),
                ["AttackSpeed"] = GetAttackSpeedAtLevel(Level),
                ["Accuracy"] = Accuracy,
                ["WeaponType"] = WeaponType,
                ["AmmoCapacity"] = AmmoCapacity,
                ["CanTargetAir"] = CanTargetAir,
                ["CanTargetGround"] = CanTargetGround
            };
        }
        
        ///<summary>
        ///Gets the current stats for this unit.
        ///</summary>
        ///<returns>Unit stats dictionary.</returns>
        public virtual System.Collections.Generic.Dictionary<string, object> GetCurrentStats()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["Level"] = Level,
                ["Experience"] = Experience,
                ["Health"] = CurrentHealth,
                ["MaxHealth"] = GetMaxHealthAtLevel(Level),
                ["Damage"] = GetDamageAtLevel(Level),
                ["Range"] = GetRangeAtLevel(Level),
                ["AttackSpeed"] = GetAttackSpeedAtLevel(Level),
                ["Speed"] = GetSpeedAtLevel(Level),
                ["Ammo"] = CurrentAmmo,
                ["IsActive"] = IsActive,
                ["IsPlayerControlled"] = IsPlayerControlled
            };
        }
        
        ///

        /// Level-based Calculations
        
        ///<summary>
        ///Gets the maximum health at a specific level.
        ///</summary>
        ///<param name="level">Unit level.</param>
        ///<returns>Maximum health at specified level.</returns>
        protected virtual float GetMaxHealthAtLevel(int level)
        {
            return BaseHealth * (1f + (level - 1) * 0.1f);
        }
        
        ///<summary>
        ///Gets the damage at a specific level.
        ///</summary>
        ///<param name="level">Unit level.</param>
        ///<returns>Damage at specified level.</returns>
        protected virtual float GetDamageAtLevel(int level)
        {
            return BaseDamage * (1f + (level - 1) * 0.15f);
        }
        
        ///<summary>
        ///Gets the range at a specific level.
        ///</summary>
        ///<param name="level">Unit level.</param>
        ///<returns>Range at specified level.</returns>
        protected virtual float GetRangeAtLevel(int level)
        {
            return BaseRange * (1f + (level - 1) * 0.05f);
        }
        
        ///<summary>
        ///Gets the attack speed at a specific level.
        ///</summary>
        ///<param name="level">Unit level.</param>
        ///<returns>Attack speed at specified level.</returns>
        protected virtual float GetAttackSpeedAtLevel(int level)
        {
            return BaseAttackSpeed * (1f + (level - 1) * 0.1f);
        }
        
        ///<summary>
        ///Gets the movement speed at a specific level.
        ///</summary>
        ///<param name="level">Unit level.</param>
        ///<returns>Movement speed at specified level.</returns>
        protected virtual float GetSpeedAtLevel(int level)
        {
            return BaseSpeed * (1f + (level - 1) * 0.08f);
        }
        
        ///<summary>
        ///Gets the required experience for a specific level.
        ///</summary>
        ///<param name="level">Target level.</param>
        ///<returns>Required experience.</returns>
        protected virtual int GetRequiredExperienceForLevel(int level)
        {
            return level * 100; //Simple linear progression
        }
        
        ///

        /// Utility Methods
        
        ///<summary>
        ///Generates a unique ID for this unit instance.
        ///</summary>
        ///<returns>Unique identifier string.</returns>
        private static string GenerateId()
        {
            return $"SASSoldier_{Guid.NewGuid():N}";
        }
        
        ///<summary>
        ///Creates a copy of this unit instance.
        ///</summary>
        ///<returns>New SAS soldier instance.</returns>
        public SASSoldier Clone()
        {
            return new SASSoldier(Id, IsPlayerControlled);
        }
        
        ///<summary>
        ///Gets a summary of this unit type.
        ///</summary>
        ///<returns>Summary string.</returns>
        public override string ToString()
        {
            var control = IsPlayerControlled ? "Player" : "AI";
            return $"{DisplayName} (ID: {Id}, Level: {Level}, Control: {control})";
        }
        
        ///
    }
}

