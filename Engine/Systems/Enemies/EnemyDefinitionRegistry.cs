using SASZombieAssaultTD.Engine.Systems.Diagnostics;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Systems.Enemies
{
    /// <summary>
    /// Static registry for all enemy definitions.
    /// </summary>
    public static class EnemyDefinitionRegistry
    {
        private static readonly Dictionary<string, EnemyDefinition> _defs = new();

        static EnemyDefinitionRegistry()
        {
            // Minimal placeholder definitions
            _defs["basic"] = new EnemyDefinition("basic", "Basic Zombie", 100, 1.0f, 10, "basic_sprite");
            _defs["fast"] = new EnemyDefinition("fast", "Fast Zombie", 60, 2.5f, 15, "fast_sprite");

            foreach (var def in _defs.Values)
            {
                DebugLogger.Log("Info", $"[EnemyDefinitionRegistry] Loaded: {def.Id} ({def.Name})");
            }
        }

        /// <summary>
        /// Returns the enemy definition for the given ID, or null if not found.
        /// </summary>
        public static EnemyDefinition? GetById(string id)
        {
            return _defs.TryGetValue(id, out var def) ? def : null;
        }

        /// <summary>
        /// Enumerates all registered enemy definitions.
        /// </summary>
        public static IEnumerable<EnemyDefinition> All => _defs.Values;
    }
}