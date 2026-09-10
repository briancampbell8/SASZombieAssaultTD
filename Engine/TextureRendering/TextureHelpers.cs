// ====================================================================================================
//  FILE: TextureHelpers.cs
//  PATH: Engine/Render/Textures/
//  MODULE: Render / Textures
//
//  ROLE:
//      Provides deterministic raw byte loading for texture files.
//      Acts as a lightweight helper for TextureLoader and GPU upload subsystems.
//
//  RESPONSIBILITIES:
//      - Load raw texture bytes from disk.
//      - Provide a minimal, deterministic API for higher-level loaders.
//      - Remain pure: no logging, no caching, no diagnostics.
//
//  NON-RESPONSIBILITIES:
//      - GPU upload or texture creation (handled by RenderDevice or TextureLoaderGPU).
//      - File format decoding (PNG/JPG decoding handled elsewhere).
//      - Resource caching or lifecycle management.
//
//  NOTES:
//      - Pure helper class.
//      - Compatible with Option‑B deterministic rendering.
// ====================================================================================================

using System;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.IO;

namespace SASZombieAssaultTD.Engine.TextureRendering
{
    /// <summary>
    /// Provides deterministic raw byte loading for texture files.
    /// </summary>
    public static class TextureHelpers
    {
        /// <summary>
        /// Loads raw bytes from a file path.
        /// </summary>
        public static byte[] LoadTextureBytes(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Texture path cannot be null or empty.", nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException($"Texture file not found: {path}", path);

            return File.ReadAllBytes(path);
        }
    }
}
