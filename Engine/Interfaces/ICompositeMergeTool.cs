// ====================================================================================================
//  FILE: ICompositeMergeTool.cs
//  PATH: Engine/Interfaces/ICompositeMergeTool.cs
//  SUBSYTEMS: Engine Interfaces
//
//  ROLE:
//      Defines the interface for deterministic CPU-side pixel merge operations.
//      Implementations perform RGBA8 merges from EngineTexture into CompositeSurface.
//
//  RESPONSIBILITIES:
//      - Declare merge operations used by CompositePipeline and CompositeBuilder.
//      - Provide a stable, deterministic API for CPU-side composition.
//      - Ensure consistent merge semantics across all implementations.
//
//  NON-RESPONSIBILITIES:
//      - Performing GPU upload or rendering (handled by Texture2D / IGraphicsDevice).
//      - Managing resource loading or caching (handled by Resource Management Framework).
//      - Diagnostics, logging, or performance metrics.
//      - Gameplay logic or UI layout.
//
//  ARCHITECTURAL NOTES:
//      - This interface is pure CPU-side contract definition.
//      - Implementations must be deterministic and side‑effect‑free.
//      - CompositeSurface is always RGBA8 (byte[]).
//      - EngineTexture provides CPU pixel data for composition.
// ====================================================================================================

using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.Interfaces
{
    /// <summary>
    /// Interface for deterministic CPU-side pixel merging.
    /// </summary>
    public interface ICompositeMergeTool
    {
        void Draw(ITextureSurface surface, IEngineTexture texture, int x, int y);
        void DrawTexture(ITextureSurface surface, EngineTexture texture, int x, int y);
    }

}
