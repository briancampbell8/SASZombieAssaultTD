/*
File:    ShadowZombie.cs
Purpose: Stealth or visibility-based enemy.
         Represents enemies that interact with visibility or detection systems.
         
Features: Shadow enemy archetype with stealth and detection mechanics.
          Placeholder implementation for enemy taxonomy and categorization.
          Used by enemy management, wave systems, and gameplay mechanics.

Created: Engine Gameplay Implementation
Notes:   This is a placeholder implementation for shadow-type enemies.
         Stealth mechanics and detection logic will be implemented in separate systems.
*/

using System;

namespace SASZombieAssaultTD.Engine.Gameplay.Enemies
{
    /// <summary>
    /// Stealth or visibility-based enemy.
    /// Represents enemies that interact with visibility or detection systems.
    /// </summary>
    public class ShadowZombie
    {
        #region Properties
        
        /// <summary>Unique identifier for this enemy instance.</summary>
        public string Id { get; private set; }
        
        /// <summary>Type identifier for this enemy.</summary>
        public string EnemyType => "ShadowZombie";
        
        /// <summary>Category of this enemy (shadow type).</summary>
        public string Category => "Shadow";
        
        /// <summary>Display name for this enemy.</summary>
        public string DisplayName => "Shadow Zombie";
        
        /// <summary>Base health value for this enemy type.</summary>
        public float BaseHealth => 60f;
        
        /// <summary>Base movement speed multiplier.</summary>
        public float BaseSpeed => 1.0f;
        
        /// <summary>Base damage value for this enemy type.</summary>
        public float BaseDamage => 20f;
        
        /// <summary>Recommended group size for this enemy type.</summary>
        public int RecommendedGroupSize => 2;
        
        /// <summary>Threat level rating (1-10).</summary>
        public int ThreatLevel => 7;
        
        /// <summary>Whether this enemy appears in groups.</summary>
        public bool AppearsInGroups => true;
        
        /// <summary>Preferred spawn pattern for this enemy.</summary>
        public string PreferredSpawnPattern => "Stealth";
        
        /// <summary>Weakness type for this enemy.</summary>
        public string Weakness => "AreaDetection";
        
        /// <summary>Resistance type for this enemy.</summary>
        public string Resistance => "Stealth";
        
        #endregion

        #region Constructors
        
        /// <summary>
        /// Creates a new shadow zombie instance.
        /// </summary>
        public ShadowZombie()
        {
            Id = GenerateId();
        }
        
        /// <summary>
        /// Creates a new shadow zombie instance with specified ID.
        /// </summary>
        /// <param name="id">Unique identifier.</param>
        public ShadowZombie(string id)
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
                ["MovementPattern"] = "Stealth",
                ["TargetPriority"] = "WeakestTarget",
                ["GroupBehavior"] = "Coordinated",
                ["AggressionLevel"] = "High",
                ["Intelligence"] = "High"
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
                ["MinGroupSize"] = 1,
                ["MaxGroupSize"] = 3,
                ["SpawnDensity"] = "Low",
                ["PreferredLocation"] = "CoveredAreas",
                ["TimeOfDay"] = "Night"
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
                ["AttackRange"] = 1.5f,
                ["AttackSpeed"] = 1.8f,
                ["Accuracy"] = 0.9f,
                ["Evasion"] = 0.8f
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
            return $"ShadowZombie_{Guid.NewGuid():N}";
        }
        
        /// <summary>
        /// Creates a copy of this enemy instance.
        /// </summary>
        /// <returns>New shadow zombie instance.</returns>
        public ShadowZombie Clone()
        {
            return new ShadowZombie();
        }
        
        /// <summary>
        /// Gets a summary of this enemy type.
        /// </summary>
        /// <returns>Summary string.</returns>
        public override string ToString()
        {
            return $"{DisplayName} (ID: {Id}, Threat: {ThreatLevel}, Stealth: true)";
        }
        
        #endregion
    }
}