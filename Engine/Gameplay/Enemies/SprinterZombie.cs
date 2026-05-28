/*
File:    SprinterZombie.cs
Purpose: High-speed pressure enemy.
         Represents a fast, aggressive enemy archetype.
         
Features: Sprinter enemy archetype with high mobility and pressure tactics.
          Placeholder implementation for enemy taxonomy and categorization.
          Used by enemy management, wave systems, and gameplay mechanics.

Created: Engine Gameplay Implementation
Notes:   This is a placeholder implementation for sprinter-type enemies.
         Pathfinding and attack logic will be implemented in separate systems.
*/

using System;

namespace SASZombieAssaultTD.Engine.Gameplay.Enemies
{
    /// <summary>
    /// High-speed pressure enemy.
    /// Represents fast, aggressive enemies that apply pressure through speed.
    /// </summary>
    public class SprinterZombie
    {
        ///  Properties
        
        /// <summary>Unique identifier for this enemy instance.</summary>
        public string Id { get; private set; }
        
        /// <summary>Type identifier for this enemy.</summary>
        public string EnemyType => "SprinterZombie";
        
        /// <summary>Category of this enemy (sprinter type).</summary>
        public string Category => "Sprinter";
        
        /// <summary>Display name for this enemy.</summary>
        public string DisplayName => "Sprinter Zombie";
        
        /// <summary>Base health value for this enemy type.</summary>
        public float BaseHealth => 75f;
        
        /// <summary>Base movement speed multiplier.</summary>
        public float BaseSpeed => 2.0f;
        
        /// <summary>Base damage value for this enemy type.</summary>
        public float BaseDamage => 15f;
        
        /// <summary>Recommended group size for this enemy type.</summary>
        public int RecommendedGroupSize => 3;
        
        /// <summary>Threat level rating (1-10).</summary>
        public int ThreatLevel => 6;
        
        /// <summary>Whether this enemy appears in groups.</summary>
        public bool AppearsInGroups => true;
        
        /// <summary>Preferred spawn pattern for this enemy.</summary>
        public string PreferredSpawnPattern => "Flanking";
        
        /// <summary>Weakness type for this enemy.</summary>
        public string Weakness => "SlowEffects";
        
        /// <summary>Resistance type for this enemy.</summary>
        public string Resistance => "QuickAttacks";
        
        /// 

        ///  Constructors
        
        /// <summary>
        /// Creates a new sprinter zombie instance.
        /// </summary>
        public SprinterZombie()
        {
            Id = GenerateId();
        }
        
        /// <summary>
        /// Creates a new sprinter zombie instance with specified ID.
        /// </summary>
        /// <param name="id">Unique identifier.</param>
        public SprinterZombie(string id)
        {
            Id = id ?? GenerateId();
        }
        
        /// 

        ///  Enemy Behavior (Placeholder)
        
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
                ["GroupBehavior"] = "Coordinated",
                ["AggressionLevel"] = "High",
                ["Intelligence"] = "Medium"
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
                ["MaxGroupSize"] = 5,
                ["SpawnDensity"] = "Medium",
                ["PreferredLocation"] = "FlankingRoutes",
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
                ["AttackSpeed"] = 2.5f,
                ["Accuracy"] = 0.8f,
                ["Evasion"] = 0.6f
            };
        }
        
        /// 

        ///  Utility Methods
        
        /// <summary>
        /// Generates a unique ID for this enemy instance.
        /// </summary>
        /// <returns>Unique identifier string.</returns>
        private static string GenerateId()
        {
            return $"SprinterZombie_{Guid.NewGuid():N}";
        }
        
        /// <summary>
        /// Creates a copy of this enemy instance.
        /// </summary>
        /// <returns>New sprinter zombie instance.</returns>
        public SprinterZombie Clone()
        {
            return new SprinterZombie();
        }
        
        /// <summary>
        /// Gets a summary of this enemy type.
        /// </summary>
        /// <returns>Summary string.</returns>
        public override string ToString()
        {
            return $"{DisplayName} (ID: {Id}, Threat: {ThreatLevel}, Speed: {BaseSpeed}x)";
        }
        
        /// 
    }
}