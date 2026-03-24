using System;

namespace SASZombieAssaultTD.Engine.Enemies
{
    /// <summary>
    /// Immutable data class representing an enemy definition.
    /// </summary>
    public sealed class EnemyDefinition : IEquatable<EnemyDefinition>
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
        /// <param name="id">Unique identifier for the enemy.</param>
        /// <param name="name">Display name of the enemy.</param>
        /// <param name="maxHealth">Maximum health value (must be positive).</param>
        /// <param name="speed">Movement speed (must be non-negative).</param>
        /// <param name="reward">Reward for defeating the enemy (must be non-negative).</param>
        /// <param name="spriteId">Sprite asset identifier.</param>
        public EnemyDefinition(string id, string name, int maxHealth, float speed, int reward, string spriteId)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            MaxHealth = maxHealth > 0 ? maxHealth : throw new ArgumentOutOfRangeException(nameof(maxHealth), "MaxHealth must be positive.");
            Speed = speed >= 0 ? speed : throw new ArgumentOutOfRangeException(nameof(speed), "Speed must be non-negative.");
            Reward = reward >= 0 ? reward : throw new ArgumentOutOfRangeException(nameof(reward), "Reward must be non-negative.");
            SpriteId = spriteId ?? throw new ArgumentNullException(nameof(spriteId));
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object? obj) => Equals(obj as EnemyDefinition);

        /// <summary>
        /// Determines whether the specified EnemyDefinition is equal to the current object.
        /// </summary>
        public bool Equals(EnemyDefinition? other)
        {
            if (other is null) return false;
            return Id == other.Id &&
                   Name == other.Name &&
                   MaxHealth == other.MaxHealth &&
                   Speed == other.Speed &&
                   Reward == other.Reward &&
                   SpriteId == other.SpriteId;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        public override int GetHashCode() => HashCode.Combine(Id, Name, MaxHealth, Speed, Reward, SpriteId);

        /// <summary>
        /// Returns a string representation of the enemy definition.
        /// </summary>
        public override string ToString() =>
            $"{Name} (ID: {Id}, Health: {MaxHealth}, Speed: {Speed}, Reward: {Reward}, Sprite: {SpriteId})";

        /// <summary>
        /// Creates a default enemy definition for testing or fallback purposes.
        /// </summary>
        public static EnemyDefinition CreateDefault() =>
            new EnemyDefinition("default", "Default Enemy", 100, 1.0f, 10, "default_sprite");
    }

    /// <summary>
    /// Enemy types available in the game.
    /// </summary>
    public enum EnemyType
    {
        BasicZombie,
        FastZombie,
        TankZombie,
        SpitterZombie,
        BossZombie,
        SwarmZombie,
        ArmoredZombie,
        SuicideZombie
    }
}


