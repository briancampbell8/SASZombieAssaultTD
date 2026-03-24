/*
File:    MamushkaZombie.cs
Purpose: Enemy that splits into smaller units on death.
         Represents multi-stage enemies with spawn-on-death behavior.
         
Features: Mamushka enemy archetype with multi-stage spawning mechanics.
          Placeholder implementation for enemy taxonomy and categorization.
          Used by enemy management, wave systems, and gameplay mechanics.

Created: Engine Gameplay Implementation
Notes:   This is a placeholder implementation for mamushka-type enemies.
         Spawning and split logic will be implemented in separate systems.
*/

using System;

namespace SASZombieAssaultTD.Engine.Gameplay.Enemies
{
    /// <summary>
    /// Enemy that splits into smaller units on death.
    /// Represents multi-stage enemies with spawn-on-death behavior.
    /// </summary>
    public class MamushkaZombie
    {
        #region Properties
        
        /// <summary>Unique identifier for this enemy instance.</summary>
        public string Id { get; private set; }
        
        /// <summary>Type identifier for this enemy.</summary>
        public string EnemyType => "MamushkaZombie";
        
        /// <summary>Category of this enemy (mamushka type).</summary>
        public string Category => "Mamushka";
        
        /// <summary>Display name for this enemy.</summary>
        public string DisplayName => "Mamushka Zombie";
        
        /// <summary>Base health value for this enemy type.</summary>
        public float BaseHealth => 150f;
        
        /// <summary>Base movement speed multiplier.</summary>
        public float BaseSpeed => 0.8f;
        
        /// <summary>Base damage value for this enemy type.</summary>
        public float BaseDamage => 25f;
        
        /// <summary>Recommended group size for this enemy type.</summary>
        public int RecommendedGroupSize => 2;
        
        /// <summary>Threat level rating (1-10).</summary>
        public int ThreatLevel => 7;
        
        /// <summary>Whether this enemy appears in groups.</summary>
        public bool AppearsInGroups => true;
        
        /// <summary>Preferred spawn pattern for this enemy.</summary>
        public string PreferredSpawnPattern => "Paired";
        
        /// <summary>Weakness type for this enemy.</summary>
        public string Weakness => "FireDamage";
        
        /// <summary>Resistance type for this enemy.</summary>
        public string Resistance => "Physical";
        
        /// <summary>Number of smaller units spawned on death.</summary>
        public int SplitCount => 3;
        
        /// <summary>Type of smaller units spawned.</summary>
        public string SplitUnitType => "MiniMamushka";
        
        /// <summary>Whether this enemy can split multiple times.</summary>
        public bool CanSplitMultiple => false;
        
        #endregion

        #region Constructors
        
        /// <summary>
        /// Creates a new mamushka zombie instance.
        /// </summary>
        public MamushkaZombie()
        {
            Id = GenerateId();
        }
        
        /// <summary>
        /// Creates a new mamushka zombie instance with specified ID.
        /// </summary>
        /// <param name="id">Unique identifier.</param>
        public MamushkaZombie(string id)
        {
            Id = id ?? GenerateId();
        }
        
        #endregion

        #region Enemy Behavior (Placeholder)
        
        /// <summary>
        /// Gets the behavior characteristics for this enemy type.
        /// </summary>
        /// <returns>Behavior characteristics dictionary.</returns>
        public virtual System.Collections.Generic.Dictionary<string, object> GetBehaviorCharacteristics()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["MovementPattern"] = "DirectPath",
                ["TargetPriority"] = "ClosestTarget",
                ["GroupBehavior"] = "Paired",
                ["AggressionLevel"] = "Medium",
                ["Intelligence"] = "Low"
            };
        }
        
        /// <summary>
        /// Gets the spawn requirements for this enemy type.
        /// </summary>
        /// <returns>Spawn requirements dictionary.</returns>
        public virtual System.Collections.Generic.Dictionary<string, object> GetSpawnRequirements()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["MinGroupSize"] = 2,
                ["MaxGroupSize"] = 4,
                ["SpawnDensity"] = "Medium",
                ["PreferredLocation"] = "OpenAreas",
                ["TimeOfDay"] = "Any"
            };
        }
        
        /// <summary>
        /// Gets the combat characteristics for this enemy type.
        /// </summary>
        /// <returns>Combat characteristics dictionary.</returns>
        public virtual System.Collections.Generic.Dictionary<string, object> GetCombatCharacteristics()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["AttackType"] = "Melee",
                ["AttackRange"] = 1.2f,
                ["AttackSpeed"] = 1.2f,
                ["Accuracy"] = 0.7f,
                ["Evasion"] = 0.2f
            };
        }
        
        /// <summary>
        /// Gets the split characteristics for this enemy type.
        /// </summary>
        /// <returns>Split characteristics dictionary.</returns>
        public virtual System.Collections.Generic.Dictionary<string, object> GetSplitCharacteristics()
        {
            return new System.Collections.Generic.Dictionary<string, object>
            {
                ["SplitCount"] = SplitCount,
                ["SplitUnitType"] = SplitUnitType,
                ["SplitTrigger"] = "OnDeath",
                ["SplitUnitHealth"] = BaseHealth / SplitCount,
                ["SplitUnitDamage"] = BaseDamage / SplitCount
            };
        }
        
        #endregion

        #region Utility Methods
        
        /// <summary>
        /// Generates a unique ID for this enemy instance.
        /// </summary>
        /// <returns>Unique identifier string.</returns>
        private static string GenerateId()
        {
            return $"MamushkaZombie_{Guid.NewGuid():N}";
        }
        
        /// <summary>
        /// Creates a copy of this enemy instance.
        /// </summary>
        /// <returns>New mamushka zombie instance.</returns>
        public MamushkaZombie Clone()
        {
            return new MamushkaZombie();
        }
        
        /// <summary>
        /// Gets a summary of this enemy type.
        /// </summary>
        /// <returns>Summary string.</returns>
        public override string ToString()
        {
            return $"{DisplayName} (ID: {Id}, Threat: {ThreatLevel}, Splits: {SplitCount})";
        }
        
        #endregion
    }
}