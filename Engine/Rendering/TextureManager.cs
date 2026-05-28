// ============================================================================
// PROGRAM: TextureManager
// FILE PATH: Engine/Rendering/TextureManager.cs
// SUBSYSTEM: Rendering / CPU‑Side Texture Caching
//
// PURPOSE:
//   Provides centralized CPU‑side texture caching and retrieval. Ensures
//   deterministic reuse of Texture2D instances across UI, rendering, and
//   gameplay systems. Prevents redundant disk I/O and supports manual
//   registration of preloaded or procedurally generated textures.
//
// ARCHITECTURAL ROLE:
//   - Acts as the authoritative CPU‑side texture registry
//   - Resolves logical texture paths to absolute file paths
//   - Delegates actual file decoding to Texture2D.LoadFromFile()
//   - Provides deterministic cache behavior for all texture consumers
//
// DIAGNOSTICS:
//   - Emits forensic absolute‑path traces for debugging load failures
//   - No silent failures: returns null only when file does not exist
//   - No System.Diagnostics except for explicit forensic output
//
// INTEGRATION POINTS:
//   - Texture2D.LoadFromFile() for CPU‑side texture creation
//   - UI rendering subsystem (UISprite, UIAssetLoader)
//   - Static layout loader (StaticLayoutLoader)
//   - Gameplay systems requiring texture reuse
//
// NOTES:
//   - This manager does NOT perform GPU upload; GPU upload is handled by the
//     renderer backend.
//   - Cache keys are the *logical paths* provided by the caller, not absolute
//     paths.
// ============================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.UI.Rendering;
using System;
using System.Collections.Generic;
using System.IO;

namespace SASZombieAssaultTD.Engine.Rendering
{
    /// <summary>
    /// Centralized CPU‑side texture cache and loader.
    /// Ensures deterministic reuse and prevents redundant disk reads.
    /// </summary>
    public class TextureManager
    {
        // --------------------------------------------------------------------
        // INTERNAL STATE
        // --------------------------------------------------------------------

        /// <summary>
        /// Dictionary‑based texture cache keyed by logical path.
        /// Stores CPU‑side Texture2D instances for reuse.
        /// </summary>
        private readonly Dictionary<string, Texture2D> _textures =
            new Dictionary<string, Texture2D>();

        // --------------------------------------------------------------------
        // PUBLIC API — TEXTURE RETRIEVAL
        // --------------------------------------------------------------------

        /// <summary>
        /// Retrieves a texture by logical path.
        /// Loads from disk on cache miss and stores result in cache.
        /// </summary>
        /// <param name="path">Logical texture path relative to executable directory.</param>
        /// <returns>Texture2D instance or null if file does not exist.</returns>
        public Texture2D Get(string path)
        {
            // Cache hit: return immediately
            if (_textures.TryGetValue(path, out var texture))
                return texture;

            // Resolve absolute path relative to executable directory
            var fullPath = Path.Combine(AppContext.BaseDirectory, path);

            // Forensic trace: deterministic, grep‑friendly
            System.Diagnostics.Debug.WriteLine(
                "[FORENSIC] TextureManager.Get.AbsolutePath: " + fullPath);

            // Load from disk if file exists
            if (File.Exists(fullPath))
            {
                texture = Texture2D.LoadFromFile(fullPath);
                _textures[path] = texture;
                return texture;
            }

            // File does not exist — caller must handle null
            return null;
        }

        // --------------------------------------------------------------------
        // PUBLIC API — MANUAL REGISTRATION
        // --------------------------------------------------------------------

        /// <summary>
        /// Manually registers a texture in the cache.
        /// Used for procedurally generated textures or preloaded assets.
        /// </summary>
        /// <param name="path">Logical cache key.</param>
        /// <param name="texture">Texture instance to store.</param>
        public void Add(string path, Texture2D texture)
        {
            _textures[path] = texture;
        }

        // --------------------------------------------------------------------
        // FUTURE EXTENSION POINT
        // --------------------------------------------------------------------

        /// <summary>
        /// Retrieves a texture using an abstract texture ID.
        /// Placeholder for future renderer integration.
        /// </summary>
        internal ITexture2D GetTextureByPath(string textureId)
        {
            NI.Hit();
            return null;
        }
    }
}
