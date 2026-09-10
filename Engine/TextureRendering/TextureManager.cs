// =====================================================================================================
//  FILE: TextureManager.cs
//  PATH: Engine/Render/Textures/
//  SUBSYSTEM: GPU Texture Management
//
//  ROLE:
//      Deterministic GPU texture manager responsible for creating and owning ID3D11Texture2D instances,
//      backed by the engine’s D3D11DeviceCore and TextureLoaderGPU PNG → GPU pipeline.
//
//  RESPONSIBILITIES:
//      - Provide deterministic methods to create textures from PNG byte data.
//      - Maintain an internal registry of GPU-resident textures keyed by deterministic IDs.
//      - Expose lookup methods for downstream systems (UI, scenes, HUD) to retrieve textures.
//      - Remain side-effect free beyond GPU allocation and registry mutation.
//
//  NON-RESPONSIBILITIES:
//      - Shader resource view (SRV) creation (handled by higher-level render systems).
//      - Texture lifetime beyond registry ownership (SystemRegistry / engine shutdown handles disposal).
//      - File I/O, path resolution, or asset discovery (asset pipeline provides PNG bytes).
//
//  ARCHITECTURAL NOTES:
//      - Uses TextureLoaderGPU.CreateTextureFromPng() for PNG → ID3D11Texture2D creation.
//      - Registry is keyed by string IDs (e.g., "MeanStreets.Map.Main", "HUD.Panel.Left").
//      - All methods are deterministic: no lazy initialization, no hidden global state.
// =====================================================================================================

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using D3DTexture2D = Vortice.Direct3D11.ID3D11Texture2D;

namespace SASZombieAssaultTD.Engine.TextureRendering
{
    public sealed class TextureManager
    {
        private readonly D3D11DeviceCore _deviceCore;
        private readonly Dictionary<string, D3DTexture2D> _textures;

        /// <summary>
        /// Constructs a deterministic TextureManager bound to the engine's D3D11 device core.
        /// </summary>
        public TextureManager(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
            _textures = new Dictionary<string, D3DTexture2D>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Creates a GPU texture from PNG bytes and registers it under the given ID.
        /// If the ID already exists, the existing texture is replaced.
        /// </summary>
        public D3DTexture2D AddFromPng(string id, byte[] pngBytes)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Texture ID is null or whitespace.", nameof(id));

            if (pngBytes == null || pngBytes.Length == 0)
                throw new ArgumentException("PNG byte buffer is null or empty.", nameof(pngBytes));

            D3DTexture2D texture = TextureLoaderGPU.CreateTextureFromPng(_deviceCore, pngBytes);
            _textures[id] = texture;
            return texture;
        }

        /// <summary>
        /// Registers an existing GPU texture under the given ID.
        /// </summary>
        public void AddRaw(string id, D3DTexture2D texture)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Texture ID is null or whitespace.", nameof(id));

            if (texture == null)
                throw new ArgumentNullException(nameof(texture));

            _textures[id] = texture;
        }

        /// <summary>
        /// Attempts to retrieve a texture by ID. Returns null if not found.
        /// </summary>
        public D3DTexture2D? GetTextureOrNull(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            return _textures.TryGetValue(id, out var tex) ? tex : null;
        }

        /// <summary>
        /// Retrieves a texture by ID. Throws if not found.
        /// </summary>
        public D3DTexture2D GetTexture(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Texture ID is null or whitespace.", nameof(id));

            if (!_textures.TryGetValue(id, out var tex))
                throw new InvalidOperationException($"Texture with ID '{id}' is not registered.");

            return tex;
        }

        /// <summary>
        /// Wraps the raw ID3D11Texture2D into the engine's Texture2D abstraction.
        /// </summary>
        internal Texture2D Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Texture ID is null or whitespace.", nameof(id));

            if (!_textures.TryGetValue(id, out var tex))
                throw new InvalidOperationException($"Texture with ID '{id}' is not registered.");

            return new Texture2D(tex, tex.Description.Width, tex.Description.Height);
        }
    }
}
