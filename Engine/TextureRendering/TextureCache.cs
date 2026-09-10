// ====================================================================================================
//  FILE: TextureCache.cs
//  PATH: Engine/Render/Textures/
//  MODULE: Render / Textures
//
//  ROLE:
//      Deterministic registry for GPU Texture2D objects used by the rendering subsystem.
//      Provides fast lookup and storage of textures by name or key.
//
//  RESPONSIBILITIES:
//      - Store Texture2D instances by string key.
//      - Provide TryGet(), Add(), Remove(), and Clear() operations.
//      - Remain deterministic and side‑effect free.
//
//  NON-RESPONSIBILITIES:
//      - Loading image files or performing resource management.
//      - Tracking texture age, usage, or disposal state.
//      - Logging, diagnostics, or fallback behavior.
//      - File persistence or serialization.
//
//  NOTES:
//      - Pure GPU texture registry.
//      - Compatible with Option‑B deterministic rendering.
//      - Works with TextureLoader, SpriteBatch, TextRenderer, and UI subsystems.
// ====================================================================================================

using System.Collections.Generic;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.TextureRendering
{
    /// <summary>
    /// Deterministic cache for GPU Texture2D objects.
    /// </summary>
    public sealed class TextureCache
    {
        private readonly Dictionary<string, Texture2D> _textures = new();

        /// <summary>
        /// Number of textures stored in the cache.
        /// </summary>
        public int Count => _textures.Count;

        /// <summary>
        /// Attempts to retrieve a texture by key.
        /// </summary>
        public bool TryGet(string key, out Texture2D texture)
        {
            return _textures.TryGetValue(key, out texture);
        }

        /// <summary>
        /// Adds or replaces a texture in the cache.
        /// </summary>
        public void Add(string key, Texture2D texture)
        {
            _textures[key] = texture;
        }

        /// <summary>
        /// Removes a texture from the cache.
        /// </summary>
        public bool Remove(string key)
        {
            return _textures.Remove(key);
        }

        /// <summary>
        /// Clears all cached textures.
        /// </summary>
        public void Clear()
        {
            _textures.Clear();
        }
    }
}
