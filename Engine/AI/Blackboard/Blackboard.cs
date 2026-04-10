// ROLE: Key/value data store for AI systems.
// RESPONSIBILITY: Store and retrieve state data for AI behaviors enabling memory persistence 
//                  and state sharing across decision cycles.
// TRIGGERS: Instantiated by AIController for behavior state storage.
// INPUTS: Receives string keys and object values from AI behaviors.
// OUTPUTS: Returns stored values by key lookup.
// DEPENDENCIES: Self-contained with Dictionary-based storage.
// CONTENTS: Blackboard class with Set, Get, HasKey, Clear, Remove methods and _data Dictionary.

using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.AI.Blackboard
{
    /// <summary>
    /// Simple key/value blackboard for AI systems.
    /// Provides state persistence and data sharing across AI behaviors and decision cycles.
    /// </summary>
    public sealed class Blackboard
    {
        private readonly Dictionary<string, object> _data = new();

        /// <summary>
        /// Stores a value under the given key.
        /// Overwrites any existing value with the same key.
        /// </summary>
        /// <param name="key">The identifier for this data entry.</param>
        /// <param name="value">The data to store (any object type).</param>
        public void Set(string key, object value)
        {
            _data[key] = value;
        }

        /// <summary>
        /// Retrieves a value from the blackboard.
        /// Returns null if the key does not exist.
        /// </summary>
        /// <param name="key">The identifier of the data to retrieve.</param>
        /// <returns>The stored value, or null if key not found.</returns>
        public object? Get(string key)
        {
            if (_data.TryGetValue(key, out var value))
                return value;

            return null;
        }
    }
}



