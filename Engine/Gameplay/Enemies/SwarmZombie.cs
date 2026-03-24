/*
File:    SwarmZombie.cs
Purpose: Fast, weak enemy that appears in groups.
         Defines identity, category, and placeholder behavior for swarm-type enemies.
         
Features: Swarm enemy archetype with group behavior characteristics.
          Placeholder implementation for enemy taxonomy and categorization.
          Used by enemy management, wave systems, and gameplay mechanics.

Created: Engine Gameplay Implementation
Notes:   This is a placeholder implementation for swarm-type enemies.
         AI and movement logic will be implemented in separate systems.
*/

using System;

namespace SASZombieAssaultTD.Engine.Gameplay.Enemies
{
    /// <summary>
    /// Fast, weak enemy that appears in groups.
    /// Represents swarm-type enemies with high numbers but low individual threat.
    /// </summary>
    public class SwarmZombie
    {
        #region Properties
        
        /// <summary>Unique identifier for this enemy instance.</summary>
        public string Id { get; private set; }
        
        /// <summary>Type identifier for this enemy.</summary>
        public string EnemyType => "SwarmZombie";
        
        /// <summary>Category of this enemy (swarm type).</summary>
        public string Category => "Swarm";
        
        /// <summary>Display name for this enemy.</summary>
        public string DisplayName => "Swarm Zombie";
        
        /// <summary>Base health value for this enemy type.</summary>
        public float BaseHealth => 50f;
        
        /// <summary>Base movement speed multiplier.</summary>
        public float BaseSpeed => 1.2f;
        
        /// <summary>Base damage value for this enemy type.</summary>
        public float BaseDamage => 10f;
        
        /// <summary>Recommended group size for this enemy type.</summary>
        public int RecommendedGroupSize => 8;
        
        /// <summary>Threat level rating (1-10).</summary>
        public int ThreatLevel => 2;
        
        /// <summary>Whether this enemy appears in groups.</summary>
        public bool AppearsInGroups => true;
        
        /// <summary>Preferred spawn pattern for this enemy.</summary>
        public string PreferredSpawnPattern => "Cluster";
        
        /// <summary>Weakness type for this enemy.</summary>
        public string Weakness => "AreaDamage";
        
        /// <summary>Resistance type for this enemy.</summary>
        public string Resistance => "SingleTarget";
        
        #endregion

        #region Constructors
        
        /// <summary>
        /// Creates a new swarm zombie instance.
        /// </summary>
        public SwarmZombie()
        {
            Id = GenerateId();
        }
        
        /// <summary>
        /// Creates a new swarm zombie instance with specified ID.
        /// </summary>
        /// <param name="id">Unique identifier.</param>
        public SwarmZombie(string id)
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
                ["MovementPattern"] = "Flocking",
                ["TargetPriority"] = "Nearest",
                ["GroupBehavior"] = "Swarm",
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
                ["MinGroupSize"] = 4,
                ["MaxGroupSize"] = 12,
                ["SpawnDensity"] = "High",
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
                ["AttackRange"] = 1.0f,
                ["AttackSpeed"] = 1.5f,
                ["Accuracy"] = 0.7f,
                ["Evasion"] = 0.3f
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
            return $"SwarmZombie_{Guid.NewGuid():N}";
        }
        
        /// <summary>
        /// Creates a copy of this enemy instance.
        /// </summary>
        /// <returns>New swarm zombie instance.</returns>
        public SwarmZombie Clone()
        {
            return new SwarmZombie();
        }
        
        /// <summary>
        /// Gets a summary of this enemy type.
        /// </summary>
        /// <returns>Summary string.</returns>
        public override string ToString()
        {
            return $"{DisplayName} (ID: {Id}, Threat: {ThreatLevel}, Group: {RecommendedGroupSize})";
        }
        
        #endregion
    }
}