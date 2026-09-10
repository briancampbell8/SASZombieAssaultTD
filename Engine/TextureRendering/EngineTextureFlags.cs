// ====================================================================================================
//  FILE: EngineTextureFlags.cs
//  PATH: Engine/Render/Textures
//  MODULE: Resource Management Framework
//
//  ROLE:
//      Immutable, deterministic flag descriptor for engine texture resources.
//      Provides stable metadata used during texture creation and validation.
//
//  RESPONSIBILITIES:
//      - Represent texture creation flags in a pure, immutable form.
//      - Support deterministic resource lookup and caching.
//      - Integrate cleanly with EngineTexture and Texture2D loaders.
//
//  NON-RESPONSIBILITIES:
//      - Performing GPU upload operations.
//      - Managing composite surfaces.
//      - Logging, diagnostics, or performance metrics.
//      - Encoding or authoring texture files.
//
//  ARCHITECTURAL NOTES:
//      - Flags must be immutable and deterministic.
//      - No builders, no mutation, no side effects.
//      - Used by resource loaders and EngineTexture.
// ====================================================================================================

namespace SASZombieAssaultTD.Engine.TextureRendering
{
    /// <summary>
    /// Immutable flag descriptor for engine texture creation.
    /// </summary>
    public readonly struct EngineTextureFlags
    {
        public readonly bool IsSRGB;

        public EngineTextureFlags(bool isSRGB) => IsSRGB = isSRGB;

        public static object CpuToGpu { get; internal set; }
        public static object Srgb { get; internal set; }
        public static EngineTextureFlags None { get; internal set; }
    }


}
