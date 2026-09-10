// ====================================================================================================
//  FILE: IEngineTexture.cs
//  PATH: Engine/Interfaces/IEngineTexture.cs
//  MODULE: Resource Management Framework
//  SUBSYSTEM: Engine Interfaces
//  ROLE:
//      Defines the deterministic interface for GPU texture resources used by the engine.
//      Provides stable idECSEntityCore, metadata access, and lifecycle guarantees.
//
//  RESPONSIBILITIES:
//      - Expose immutable texture metadata (width, height, format, flags).
//      - Provide access to the underlying GPU texture resource.
//      - Integrate cleanly with resource loaders and caching layers.
//      - Support deterministic resource lookup via the Resource Management Framework.
//
//  NON-RESPONSIBILITIES:
//      - Performing GPU upload operations (handled by Texture2D).
//      - Performing rendering or sampling operations.
//      - Logging, diagnostics, or performance metrics.
//      - Encoding or authoring texture files.
//
//  ARCHITECTURAL NOTES:
//      - IEngineTexture is a pure descriptor interface.
//      - GPU upload is delegated to Texture2D.
//      - Resource idECSEntityCore must remain stable and deterministic.
// ====================================================================================================

using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.Interfaces
{
    public interface IEngineTexture
    {
        int Width { get; }
        int Height { get; }
        object Format { get; set; }
        Texture2D GPUTexture { get; }
        EngineTextureFlags Flags { get; }
        bool IsDisposed { get; }
    }
}
