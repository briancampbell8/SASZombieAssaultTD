using System.Collections.Generic;

namespace SASZombieAssaultTD.Scripts.AI.Blackboard
{
    /// <summary>
    /// Simple key/value blackboard for AI systems.
    /// </summary>
    public sealed class Blackboard
    {
        private readonly Dictionary<string, object> _data = new();

        /// <summary>
        /// Stores a value under the given key.
        /// </summary>
        public void Set(string key, object value)
        {
            _data[key] = value;
        }

        /// <summary>
        /// Retrieves a value from the blackboard.
        /// Returns null if the key does not exist.
        /// </summary>
        public object? Get(string key)
        {
            if (_data.TryGetValue(key, out var value))
                return value;

            return null;
        }
    }
}