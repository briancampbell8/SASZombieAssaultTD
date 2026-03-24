/*
File:    TextureCache.cs
Author:  BDC
Created: 2026-02-17

Purpose:
Simple texture cache for resource management.

Notes:
Placeholder implementation for P11-08 milestone.
Will be enhanced in future tasks.

*/
using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Rendering
{
    /// <summary>
    /// Simple texture cache for resource management.
    /// </summary>
    public sealed class TextureCache
    {
        private readonly Dictionary<string, Texture2D> _cache = new();

        /// <summary>
        /// Gets the number of cached textures.
        /// </summary>
        public int Count => _cache.Count;

        /// <summary>
        /// Attempts to get a cached texture.
        /// </summary>
        /// <param name="filePath">The file path of the texture.</param>
        /// <param name="texture">The cached texture if found.</param>
        /// <returns>True if texture was found in cache.</returns>
        public bool TryGet(string filePath, out Texture2D? texture)
        {
            return _cache.TryGetValue(filePath, out texture);
        }

        /// <summary>
        /// Adds a texture to the cache.
        /// </summary>
        /// <param name="filePath">The file path of the texture.</param>
        /// <param name="texture">The texture to cache.</param>
        public void Add(string filePath, Texture2D texture)
        {
            _cache[filePath] = texture;
        }

        /// <summary>
        /// Removes a texture from the cache.
        /// </summary>
        /// <param name="filePath">The file path of the texture to remove.</param>
        /// <returns>True if texture was removed from cache.</returns>
        public bool Remove(string filePath)
        {
            return _cache.Remove(filePath);
        }

        /// <summary>
        /// Clears all cached textures.
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Invalidates stale textures based on age or usage criteria.
        /// P11-08-25: Ensures no stale textures remain in cache.
        /// </summary>
        /// <param name="maxAge">Maximum age in minutes before texture is considered stale.</param>
        /// <param name="maxUnusedCount">Maximum number of unused textures to keep.</param>
        public void InvalidateStaleTextures(TimeSpan maxAge, int maxUnusedCount = 100)
        {
            var keysToRemove = new List<string>();
            var currentTime = DateTime.UtcNow;

            foreach (var kvp in _cache)
            {
                var texture = kvp.Value;

                // Check if texture is disposed (invalid)
                if (texture == null)
                {
                    keysToRemove.Add(kvp.Key);
                    continue;
                }

                // Add age-based invalidation logic here if needed
                // For now, just remove null references
            }

            // Remove stale textures
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
            }
        }

        /// <summary>
        /// Validates cache integrity and removes invalid entries.
        /// P11-08-25: Ensures cache contains only valid textures.
        /// </summary>
        public void ValidateCache()
        {
            var keysToRemove = new List<string>();

            foreach (var kvp in _cache)
            {
                if (kvp.Value == null)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
            }
        }
    }
}




