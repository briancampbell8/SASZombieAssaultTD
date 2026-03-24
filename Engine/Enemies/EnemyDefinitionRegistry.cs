using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Enemies
{
    /// <summary>
    /// Represents a static registry for managing and retrieving enemy definitions.
    /// </summary>
    /// <remarks>
    /// This registry is initialized with predefined enemy definitions and provides methods to retrieve them by ID or enumerate all definitions.
    /// </remarks>
    public static class EnemyDefinitionRegistry
    {
        /// <summary>
        /// A read-only dictionary containing all enemy definitions.
        /// </summary>
        private static readonly IReadOnlyDictionary<string, EnemyDefinition> _defs;

        /// <summary>
        /// Initializes the <see cref="EnemyDefinitionRegistry"/> class with predefined enemy definitions.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when any predefined enemy definition is invalid.</exception>
        static EnemyDefinitionRegistry()
        {
            var definitions = new Dictionary<string, EnemyDefinition>
            {
                { "basic", CreateDefinition("basic", "Basic Zombie", 100, 1.0f, 10, "basic_sprite") },
                { "fast", CreateDefinition("fast", "Fast Zombie", 60, 2.5f, 15, "fast_sprite") }
            };

            _defs = definitions;

            foreach (var def in _defs.Values)
            {
                ModernLoggingSystem.Log("Info", $"[EnemyDefinitionRegistry] Loaded: {def.Id} ({def.Name})");
            }
        }

        /// <summary>
        /// Retrieves the enemy definition associated with the specified ID.
        /// </summary>
        /// <param name="id">The unique identifier of the enemy definition.</param>
        /// <returns>The <see cref="EnemyDefinition"/> if found; otherwise, <c>null</c>.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided ID is null or whitespace.</exception>
        public static EnemyDefinition? GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("ID cannot be null or whitespace.", nameof(id));
            }

            return _defs.TryGetValue(id, out var definition) ? definition : null;
        }

        /// <summary>
        /// Gets all enemy definitions in the registry.
        /// </summary>
        public static IReadOnlyCollection<EnemyDefinition> All => (IReadOnlyCollection<EnemyDefinition>)_defs.Values;

        /// <summary>
        /// Creates and validates an enemy definition.
        /// </summary>
        /// <param name="id">Unique identifier for the enemy.</param>
        /// <param name="name">Display name of the enemy.</param>
        /// <param name="maxHealth">Maximum health value.</param>
        /// <param name="speed">Movement speed.</param>
        /// <param name="reward">Reward for defeating the enemy.</param>
        /// <param name="spriteId">Sprite asset identifier.</param>
        /// <returns>A validated <see cref="EnemyDefinition"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when any property is invalid.</exception>
        private static EnemyDefinition CreateDefinition(string id, string name, int maxHealth, float speed, int reward, string spriteId)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("ID cannot be null or whitespace.", nameof(id));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
            if (maxHealth <= 0) throw new ArgumentOutOfRangeException(nameof(maxHealth), "MaxHealth must be positive.");
            if (speed <= 0) throw new ArgumentOutOfRangeException(nameof(speed), "Speed must be positive.");
            if (reward < 0) throw new ArgumentOutOfRangeException(nameof(reward), "Reward cannot be negative.");
            if (string.IsNullOrWhiteSpace(spriteId)) throw new ArgumentException("SpriteId cannot be null or whitespace.", nameof(spriteId));

            return new EnemyDefinition(id, name, maxHealth, speed, reward, spriteId);
        }
    }
}


