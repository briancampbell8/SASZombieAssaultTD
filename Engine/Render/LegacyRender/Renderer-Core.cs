// =====================================================================================================
//  FILE: Renderer-Core.cs
//  PATH: Engine/Render/LegacyRender/Renderer-Core.cs
//  SUBSYSTEM: Rendering (Legacy Driver Program)
//
//  ROLE:
//      Provides the core idECSEntityCore and diagnostic surface for the Renderer driver program.
//      This partial anchors all other Renderer-* partials and exposes deterministic metadata
//      describing the renderer’s current configuration and runtime state.
//
//  RESPONSIBILITIES:
//      - Define the public diagnostic ToString() surface.
//      - Report stable renderer state for debugging and logging.
//      - Anchor the Renderer partial class structure.
//      - Maintain deterministic reporting of initialization, viewport, color grading,
//        render scale, VSync, and GPU context.
//
//  NON-RESPONSIBILITIES:
//      - Performing rendering operations.
//      - Managing GPU resources or issuing draw calls.
//      - Handling initialization, viewport changes, frame pacing, screenshots,
//        color grading, or GPU timing.
//      - Implementing subsystem logic (delegated to other Renderer-* partials).
//
//  ARCHITECTURAL NOTES:
//      - Renderer.cs (Engine/Render/Renderer.cs) is the driver program; this partial extends it.
//      - All operational logic is isolated into Renderer-* partials under LegacyRender.
//      - This partial must remain lightweight and free of functional logic.
//      - ToString() must remain deterministic and safe for logging at any engine stage.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Render
{
    public partial class Renderer
    {
        /// <summary>
        /// Provides deterministic diagnostic information describing the renderer’s current state.
        /// </summary>
        public override string ToString()
        {
            return $"Renderer: Initialized={_isInitialized}, " +
                   $"Viewport={_viewportSize.X}x{_viewportSize.Y}, " +
                   $"ClearColor={_clearColor}, " +
                   $"VSync={_vsyncEnabled}, " +
                   $"RenderScale={_renderScale:F2}, " +
                   $"ColorGrading={_colorGradingEnabled}, " +
                   $"GPUContext={_gpuContext}";
        }
    }
}
