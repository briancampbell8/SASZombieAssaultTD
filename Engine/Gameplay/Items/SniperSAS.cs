/*
File:    SniperSAS.cs
Purpose: Long-range precision soldier variant.
         Represents a sniper-type soldier with high accuracy.
         
Features: Sniper SAS archetype with long-range precision characteristics.
          Placeholder implementation for unit taxonomy and categorization.
          Used by unit management, squad systems, and gameplay mechanics.

Created: Engine Gameplay Implementation
Notes:   This is a placeholder implementation for sniper SAS units.
         Aiming and firing logic will be implemented in separate systems.
*/

using System;

namespace SASZombieAssaultTD.Engine.Gameplay.Items
{
    /// <summary>
    /// Long-range precision soldier variant.
    /// Represents a sniper-type soldier with high accuracy.
    /// </summary>
    public class SniperSAS
    {
        #region Properties
        
        /// <summary>Unique identifier for this soldier instance.</summary>
        public string Id { get; private set; }
        
        /// <summary>Type identifier for this unit.</summary>
        public string UnitType => "SniperSAS";
        
        /// <summary>Category of this unit.</summary>
        public string Category => "Sniper";
        
        /// <summary>Display name for this unit.</summary>
        public string DisplayName => "Sniper SAS";
        
        /// <summary>Base health value for this unit type.</summary>
        public float BaseHealth => 80f;
        
        /// <summary>Base movement speed multiplier.</summary>
        public float BaseSpeed => 0.8f;
        
        /// <summary>Base damage value for this unit type.</summary>
        public float BaseDamage => 75f;
        
        /// <summary>Base attack range for this unit.</summary>
        public float BaseRange => 12.0f;
        
        /// <summary>Base attack speed for this unit.</summary>
        public float BaseAttackSpeed => 0.5f;
        
        /// <summary>Whether this unit is player-controlled.</summary>
        public bool IsPlayerControlled { get; private set; }
        
        /// <summary>Current level of this unit.</summary>
        public int Level { get; private set; }
        
        /// <summary>Experience points for this unit.</summary>
        public int Experience { get; private set; }
        
        /// <summary>Current health of this unit.</summary>
        public float CurrentHealth { get; private set; }
        
        /// <summary>Whether this unit is currently active.</summary>
        public bool IsActive { get; private set; }
        
        /// <summary>Weapon type for this unit.</summary>
        public string WeaponType => "SniperRifle";
        
        /// <summary>Ammo capacity for this unit.</summary>
        public int AmmoCapacity => 5;
        
        /// <summary>Current ammo count.</summary>
        public int CurrentAmmo { get; private set; }
        
        /// <summary>Accuracy rating (0.0-1.0).</summary>
        public float Accuracy => 0.95f;
        
        /// <summary>Whether this unit can target air units.</summary>
        public bool CanTargetAir => false;
        
        /// <summary>Whether this unit can target ground units.</summary>
        public bool CanTargetGround => true;
        
        /// <summary>Zoom level for precision aiming.</summary>
        public float ZoomLevel => 4.0f;
        
        /// <summary>Steady aim time in seconds.</summary>
        public float SteadyAimTime => 2.0f;
        
        /// <summary>Whether this unit is currently aiming.</summary>
        public bool IsAiming { get; private set; }
        
        /// <summary>Current aim progress (0.0-1.0).</summary>
        public float AimProgress { get; private set; }
        
        #endregion

        #region Constructors
        
        /// <summary>
        /// Creates a new sniper SAS instance.
        /// </summary>
        public SniperSAS()
        {
            Id = GenerateId();
            Level = 1;
            Experience = 0;
            CurrentHealth = BaseHealth;
            CurrentAmmo = AmmoCapacity;
            IsActive = false;
            IsPlayerControlled = false;
            IsAiming = false;
            AimProgress = 0f;
        }
        
        /// <summary>
        /// Creates a new sniper SAS instance with specified control type.
        /// </summary>
        /// <param name="isPlayerControlled">Whether this unit is player-controlled.</param>
        public SniperSAS(bool isPlayerControlled)
        {
            Id = GenerateId();
            Level = 1;
            Experience = 0;
            CurrentHealth = BaseHealth;
            CurrentAmmo = AmmoCapacity;
            IsActive = false;
            IsPlayerControlled = isPlayerControlled;
            IsAiming = false;
            AimProgress = 0f;
        }
        
        /// <summary>
        /// Creates a new sniper SAS instance with specified ID and control type.
        /// </summary>
        /// <param name="id">Unique identifier.</param>
        /// <param name="isPlayerControlled">Whether this unit is player-controlled.</param>
        public SniperSAS(string id, bool isPlayerControlled = false)
        {
            Id = id ?? GenerateId();
            Level = 1;
            Experience = 0;
            CurrentHealth = BaseHealth;
            CurrentAmmo = AmmoCapacity;
            IsActive = false;
            IsPlayerControlled = isPlayerControlled;
            IsAiming = false;
            AimProgress = 0f;
        }
        
        #endregion

        #region Unit Operations (Placeholder)
        
        /// <summary>
        /// Activates this unit.
        /// </summary>
        public virtual void Activate()
        {
            IsActive = true;
        }
        
        /// <summary>
        /// Deactivates this unit.
        /// </summary>
        public virtual void Deactivate()
        {
            IsActive = false;
            StopAiming();
        }
        
        /// <summary>
        /// Applies damage to this unit.
        /// </summary>
        /// <param name="damage">Damage amount.</param>
        /// <returns>True if unit was destroyed.</returns>
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
        
        /// <summary>
        /// Heals this unit.
        /// </summary>
        /// <param name="amount">Heal amount.</param>
        /// <returns>Actual amount healed.</returns>
        public virtual float Heal(float amount)
        {
            var maxHealth = GetMaxHealthAtLevel(Level);
            var healAmount = System.Math.Min(amount, maxHealth - CurrentHealth);
            CurrentHealth += healAmount;
            return healAmount;
        }
        
        /// <summary>
        /// Reloads this unit's weapon.
        /// </summary>
        public virtual void Reload()
        {
            CurrentAmmo = AmmoCapacity;
            StopAiming();
        }
        
        /// <summary>
        /// Starts aiming for a precision shot.
        /// </summary>
        public virtual void StartAiming()
        {
            if (!IsActive || CurrentAmmo <= 0)
                return;
                
            IsAiming = true;
            AimProgress = 0f;
        }
        
        /// <summary>
        /// Stops aiming and resets aim progress.
        /// </summary>
        public virtual void StopAiming()
        {
            IsAiming = false;
            AimProgress = 0f;
        }
        
        /// <summary>
        /// Updates aim progress over time.
        /// </summary>
        /// <param name="deltaTime">Time since last frame.</param>
        public virtual void UpdateAim(float deltaTime)
        {
            if (!IsAiming)
                return;
                
            AimProgress = System.Math.Min(1f, AimProgress + (deltaTime / SteadyAimTime));
        }
        
        /// <summary>
        /// Fires this unit's weapon if aiming is complete.
        /// </summary>
        /// <returns>True if shot was fired successfully.</returns>
        public virtual bool Fire()
        {
            if (!IsActive || CurrentAmmo <= 0)
                return false;
                
            if (IsAiming && AimProgress < 1f)
                return false;
                
            CurrentAmmo--;
            StopAiming();
            return true;
        }
        
        /// <summary>
        /// Gets the current accuracy based on aim progress.
        /// </summary>
        /// <returns>Current accuracy (0.0-1.0).</returns>
        public virtual float GetCurrentAccuracy()
        {
            if (!IsAiming)
                return Accuracy * 0.5f; // Hip fire accuracy is lower
                
            return Accuracy * (0.5f + (AimProgress * 0.5f)); // Scale from 50% to 100%
        }
        
        /// <summary>
        /// Adds experience to this unit.
        /// </summary>
        /// <param name="amount">Experience amount.</param>
        /// <returns>True if unit leveled up.</returns>
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
        
        #endregion

        #region Unit Characteristics
        
        /// <summary>
        /// Gets the behavior characteristics for this unit type.
        /// </summary>
        /// <returns>Behavior characteristics dictionary.</returns>
        public virtual System.Collections.Generic.Dictionary<string, object> GetBehaviorCharacteristics()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["MovementPattern"] = "Stationary",
                ["TargetPriority"] = "HighValue",
                ["CombatStyle"] = "Precision",
                ["Intelligence"] = IsPlayerControlled ? "Player" : "AI",
                ["Cooperation"] = "Solo"
            };
        }
        
        /// <summary>
        /// Gets the combat characteristics for this unit type.
        /// </summary>
        /// <returns>Combat characteristics dictionary.</returns>
        public virtual System.Collections.Generic.Dictionary<string, object> GetCombatCharacteristics()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["AttackType"] = "Ballistic",
                ["AttackRange"] = GetRangeAtLevel(Level),
                ["AttackSpeed"] = GetAttackSpeedAtLevel(Level),
                ["Accuracy"] = GetCurrentAccuracy(),
                ["WeaponType"] = WeaponType,
                ["AmmoCapacity"] = AmmoCapacity,
                ["CanTargetAir"] = CanTargetAir,
                ["CanTargetGround"] = CanTargetGround,
                ["ZoomLevel"] = ZoomLevel,
                ["SteadyAimTime"] = SteadyAimTime,
                ["IsAiming"] = IsAiming,
                ["AimProgress"] = AimProgress
            };
        }
        
        /// <summary>
        /// Gets the precision characteristics for this unit type.
        /// </summary>
        /// <returns>Precision characteristics dictionary.</returns>
        public virtual System.Collections.Generic.Dictionary<string, object> GetPrecisionCharacteristics()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["BaseAccuracy"] = Accuracy,
                ["CurrentAccuracy"] = GetCurrentAccuracy(),
                ["ZoomLevel"] = ZoomLevel,
                ["SteadyAimTime"] = SteadyAimTime,
                ["AimProgress"] = AimProgress,
                ["IsAiming"] = IsAiming,
                ["CriticalHitChance"] = 0.15f,
                ["CriticalHitMultiplier"] = 2.0f
            };
        }
        
        /// <summary>
        /// Gets the current stats for this unit.
        /// </summary>
        /// <returns>Unit stats dictionary.</returns>
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
                ["IsPlayerControlled"] = IsPlayerControlled,
                ["Accuracy"] = GetCurrentAccuracy(),
                ["IsAiming"] = IsAiming,
                ["AimProgress"] = AimProgress
            };
        }
        
        #endregion

        #region Level-based Calculations
        
        /// <summary>
        /// Gets the maximum health at a specific level.
        /// </summary>
        /// <param name="level">Unit level.</param>
        /// <returns>Maximum health at specified level.</returns>
        protected virtual float GetMaxHealthAtLevel(int level)
        {
            return BaseHealth * (1f + (level - 1) * 0.08f);
        }
        
        /// <summary>
        /// Gets the damage at a specific level.
        /// </summary>
        /// <param name="level">Unit level.</param>
        /// <returns>Damage at specified level.</returns>
        protected virtual float GetDamageAtLevel(int level)
        {
            return BaseDamage * (1f + (level - 1) * 0.2f);
        }
        
        /// <summary>
        /// Gets the range at a specific level.
        /// </summary>
        /// <param name="level">Unit level.</param>
        /// <returns>Range at specified level.</returns>
        protected virtual float GetRangeAtLevel(int level)
        {
            return BaseRange * (1f + (level - 1) * 0.1f);
        }
        
        /// <summary>
        /// Gets the attack speed at a specific level.
        /// </summary>
        /// <param name="level">Unit level.</param>
        /// <returns>Attack speed at specified level.</returns>
        protected virtual float GetAttackSpeedAtLevel(int level)
        {
            return BaseAttackSpeed * (1f + (level - 1) * 0.08f);
        }
        
        /// <summary>
        /// Gets the movement speed at a specific level.
        /// </summary>
        /// <param name="level">Unit level.</param>
        /// <returns>Movement speed at specified level.</returns>
        protected virtual float GetSpeedAtLevel(int level)
        {
            return BaseSpeed * (1f + (level - 1) * 0.05f);
        }
        
        /// <summary>
        /// Gets the required experience for a specific level.
        /// </summary>
        /// <param name="level">Target level.</param>
        /// <returns>Required experience.</returns>
        protected virtual int GetRequiredExperienceForLevel(int level)
        {
            return level * 150; // Higher exp requirement for specialist unit
        }
        
        #endregion

        #region Utility Methods
        
        /// <summary>
        /// Generates a unique ID for this unit instance.
        /// </summary>
        /// <returns>Unique identifier string.</returns>
        private static string GenerateId()
        {
            return $"SniperSAS_{Guid.NewGuid():N}";
        }
        
        /// <summary>
        /// Creates a copy of this unit instance.
        /// </summary>
        /// <returns>New sniper SAS instance.</returns>
        public SniperSAS Clone()
        {
            return new SniperSAS(Id, IsPlayerControlled);
        }
        
        /// <summary>
        /// Gets a summary of this unit type.
        /// </summary>
        /// <returns>Summary string.</returns>
        public override string ToString()
        {
            var control = IsPlayerControlled ? "Player" : "AI";
            var status = IsAiming ? "Aiming" : "Ready";
            return $"{DisplayName} (ID: {Id}, Level: {Level}, Control: {control}, Status: {status})";
        }
        
        #endregion
    }
}