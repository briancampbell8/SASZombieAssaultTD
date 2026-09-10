// =====================================================================================================
//  FILE: UITextureAtlasManager.cs
//  PATH: Engine/UI/Rendering/Modern/UITextureAtlasManager.cs
//  SUBSYSTEM: Modern UI Rendering — Texture Atlas Management
//
//  ROLE:
//      Deterministic texture/material lookup for ModernUIRendererP2.
//      Provides GPU-bound materials for UI elements (sprites, images, buttons).
//
//  RESPONSIBILITIES:
//      - Maintain a lookup table of UI materials.
//      - Bind GPU textures from TextureManager.
//      - Provide deterministic fallback material.
//      - Support non-atlas textures (MainMenu.png, button backgrounds, icons).
//
//  NON-RESPONSIBILITIES:
//      - GPU resource creation (handled by TextureManager).
//      - Render command submission (handled by ModernUIRendererP2).
//
//  ARCHITECTURAL NOTES:
//      - Option-B deterministic subsystem.
//      - No dynamic atlas packing.
//      - Every UI texture is treated as a standalone material.
// =====================================================================================================

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.UI.Rendering.Modern
{
    /// <summary>
    /// Deterministic texture/material manager used by ModernUIRendererP2.
    /// </summary>
    public sealed class UITextureAtlasManager
    {
        private readonly Dictionary<string, UIAtlasMaterial> _materials =
            new Dictionary<string, UIAtlasMaterial>(StringComparer.Ordinal);

        private readonly TextureManager _textureManager;

        public UITextureAtlasManager(TextureManager textureManager)
        {
            _textureManager = textureManager ?? throw new ArgumentNullException(nameof(textureManager));
        }

        public UITextureAtlasManager()
        {
        }

        /// <summary>
        /// Default material returned when a lookup fails.
        /// </summary>
        public UIAtlasMaterial DefaultMaterial { get; private set; }

        // ----------------------------------------------------------------------------------------------
        // MATERIAL REGISTRATION
        // ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Registers a GPU texture as a UI material.
        /// </summary>
        public void RegisterMaterial(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var gpuTexture = _textureManager.GetTexture(name);
            if (gpuTexture == null)
                throw new InvalidOperationException(
                    $"UITextureAtlasManager: TextureManager has no GPU texture for key '{name}'.");

            var material = new UIAtlasMaterial(
                name: name,
                gpuHandle: gpuTexture,
                u1: 0f,
                v1: 0f,
                u2: 1f,
                v2: 1f
            );

            _materials[name] = material;
        }

        /// <summary>
        /// Registers a prebuilt material (atlas or custom).
        /// </summary>
        public void RegisterMaterial(string name, UIAtlasMaterial material)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (material == null)
                throw new ArgumentNullException(nameof(material));

            _materials[name] = material;
        }

        // ----------------------------------------------------------------------------------------------
        // MATERIAL LOOKUP
        // ----------------------------------------------------------------------------------------------

        public UIAtlasMaterial GetMaterialForElement(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return DefaultMaterial;

            if (_materials.TryGetValue(name, out var mat))
                return mat;

            return DefaultMaterial;
        }

        // ----------------------------------------------------------------------------------------------
        // DEFAULT MATERIAL
        // ----------------------------------------------------------------------------------------------

        public void SetDefaultMaterial(string textureName)
        {
            var gpuTexture = _textureManager.GetTexture(textureName);
            if (gpuTexture == null)
                throw new InvalidOperationException(
                    $"UITextureAtlasManager: Cannot set default material. Texture '{textureName}' not found.");

            DefaultMaterial = new UIAtlasMaterial(
                name: textureName,
                gpuHandle: gpuTexture,
                u1: 0f,
                v1: 0f,
                u2: 1f,
                v2: 1f
            );
        }
    }

    /// <summary>
    /// Represents a GPU-bound UI material.
    /// </summary>
    public sealed class UIAtlasMaterial
    {
        public string Name { get; }
        public object GpuHandle { get; }
        public float U1 { get; }
        public float V1 { get; }
        public float U2 { get; }
        public float V2 { get; }

        public UIAtlasMaterial(string name, object gpuHandle, float u1, float v1, float u2, float v2)
        {
            Name = name;
            GpuHandle = gpuHandle;
            U1 = u1;
            V1 = v1;
            U2 = u2;
            V2 = v2;
        }
    }
}
