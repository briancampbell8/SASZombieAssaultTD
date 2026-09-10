// ====================================================================================================
//  FILE: IColorRGBA.cs
//  PATH: Engine/Interfaces/IColorRGBA.cs
//  MODULE: Render Core – High‑Level Drawing Context
//
//  ROLE:
//      Deterministic high‑level GPU drawing interface.
//      Emits stable draw commands consumed by the RenderDevice and GPU backend.
//
//  RESPONSIBILITIES:
//      - Provide pure draw‑command emission for lines, rectangles, circles, sprites, textures, and text.
//      - Abstract the underlying GPU backend (D3D11, Vulkan, BGFX, etc.).
//      - Supply a consistent API for UI, HUD, gameplay, and engine subsystems.
//      - Expose frame lifecycle operations (BeginFrame, EndFrame, Present).
//      - Provide viewport information for layout and rendering.
//
//  NON-RESPONSIBILITIES:
//      - Resource loading or caching.
//      - CPU‑side rasterization (handled by FramebufferDrawing).
//      - Composite operations (handled by CompositePipeline).
//      - Diagnostics, logging, or performance metrics.
//      - Texture encoding or file authoring.
//
//  ARCHITECTURAL NOTES:
//      - All operations must be pure command emission with no CPU rendering.
//      - Legacy System.Drawing overloads will be removed.
//      - API must remain minimal, stable, and deterministic.
// ====================================================================================================

namespace SASZombieAssaultTD.Engine.Interfaces
{
    public interface IColorRGBA
    {
        float A { get; set; }

        float B { get; set; }

        float G { get; set; }

        float R { get; set; }
    }
}
