// =====================================================================================================
//  FILE: IClearableDrawingContext.cs
//  PATH: Engine/Interfaces/IClearableDrawingContext.cs
//  SUBSYSTEM: Rendering Interfaces
//
//  ROLE:
//      Contract for drawing contexts that can clear the current render target (and optionally depth).
//      Implemented by GPU-backed contexts (e.g., D3D11) that own or can access RTV/DSV surfaces.
//
//  RESPONSIBILITIES:
//      - Provide a deterministic Clear(Color) operation for the active render target.
//      - Allow high-level systems (RenderSystem, adapters) to request screen clears without GPU knowledge.
//      - Integrate cleanly with existing IDrawingContext-based rendering pipelines.
//
//  NON-RESPONSIBILITIES:
//      - Pixel-level rasterization (handled by GPU and draw calls).
//      - Resource discovery, caching, or asset management.
//      - Diagnostics, logging, or performance metrics.
//      - Swap-chain, device, or lifetime management.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Interfaces
{
    /// <summary>
    /// Defines a drawing context that supports clearing the render target with a specified color.
    /// </summary>
    internal interface IClearableDrawingContext
    {
        /// <summary>
        /// Clears the current render target (and optionally depth-stencil) with the given color.
        /// </summary>
        /// <param name="color">The color to clear the render target with.</param>
        void Clear(Color color);
    }
}
