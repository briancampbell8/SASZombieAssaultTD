using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Rendering
{
    /// <summary>
    /// Cache for managing and retrieving sprites.
    /// </summary>
    public static class SpriteCache
    {
        private static readonly Dictionary<string, object> _sprites = new();

        /// <summary>
        /// Retrieves a sprite by name.
        /// </summary>
        /// <param name="name">The name of the sprite.</param>
        /// <returns>The sprite object, or null if not found.</returns>
        public static object GetSprite(string name)
        {
            return _sprites.TryGetValue(name, out var sprite) ? sprite : null;
        }

        /// <summary>
        /// Adds a sprite to the cache.
        /// </summary>
        /// <param name="name">The name of the sprite.</param>
        /// <param name="sprite">The sprite object.</param>
        public static void AddSprite(string name, object sprite)
        {
            _sprites[name] = sprite;
        }
    }
}
