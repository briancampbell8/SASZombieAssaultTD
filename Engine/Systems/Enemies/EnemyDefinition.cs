using System;

namespace SASZombieAssaultTD.Engine.Systems.Enemies
{
    /// <summary>
    /// Immutable data class representing an enemy definition.
    /// </summary>
    public sealed class EnemyDefinition
    {
        /// <summary>Unique identifier for the enemy.</summary>
        public string Id { get; }
        /// <summary>Display name of the enemy.</summary>
        public string Name { get; }
        /// <summary>Maximum health value.</summary>
        public int MaxHealth { get; }
        /// <summary>Movement speed.</summary>
        public float Speed { get; }
        /// <summary>Reward for defeating the enemy.</summary>
        public int Reward { get; }
        /// <summary>Sprite asset identifier.</summary>
        public string SpriteId { get; }

        /// <summary>
        /// Constructs a new immutable enemy definition.
        /// </summary>
        public EnemyDefinition(string id, string name, int maxHealth, float speed, int reward, string spriteId)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            MaxHealth = maxHealth;
            Speed = speed;
            Reward = reward;
            SpriteId = spriteId ?? throw new ArgumentNullException(nameof(spriteId));
        }
    }
}