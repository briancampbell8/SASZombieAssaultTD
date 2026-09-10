// ====================================================================================================
//  FILE: IPlatformRenderer.cs
//  PATH: Engine/Render
//  MODULE: Render Core – Platform GPU Abstraction
//
//  ROLE:
//      Defines the minimal deterministic GPU renderer contract used by platform-specific backends
//      (D3D11, BGFX, etc.). Provides initialization, frame rendering, viewport control, and capability
//      reporting.
//
//  RESPONSIBILITIES:
//      - Initialize the platform GPU renderer.
//      - Execute a single RenderFrame() call per frame.
//      - Clear the active render target.
//      - Configure viewport dimensions.
//      - Report GPU feature capabilities via RendererCapabilities.
//
//  NON-RESPONSIBILITIES:
//      - Game lifecycle management (startup/update/shutdown).
//      - Gameplay logic or UI layout.
//      - Resource loading or caching.
//      - CPU-side rendering or framebuffer operations.
//      - Composite operations or batching.
//
//  ARCHITECTURAL NOTES:
//      - Platform renderers must be deterministic and side-effect-free beyond GPU state changes.
//      - Capability reporting is enum-driven for stability and auditability.
//      - This interface is implemented by platform-specific GPU backends.
// ====================================================================================================

using static SASZombieAssaultTD.Engine.Render.RenderEnums;

namespace SASZombieAssaultTD.Engine.Interfaces
{
    public interface IPlatformRenderer
    {
        void Initialize();
        void RenderFrame();
        void SetViewport(int width, int height);
        void Clear();

        RendererCapabilities GetCapabilities();
    }

    // -------------------------------------------------------------------------
    //  ENUMS — deterministic, scalable, GPU‑friendly
    // -------------------------------------------------------------------------


    // -------------------------------------------------------------------------
    //  CAPABILITIES — enum‑driven, deterministic metadata
    // -------------------------------------------------------------------------

    public readonly struct RendererCapabilities
    {
        public TransparencySupport Transparency { get; }
        public BlendingSupport Blending { get; }
        public DepthSupport DepthBuffer { get; }

        public int MaxTextureSize { get; }
        public int MaxRenderTargets { get; }

        public RendererCapabilities(
            TransparencySupport transparency,
            BlendingSupport blending,
            DepthSupport depthBuffer,
            int maxTextureSize,
            int maxRenderTargets)
        {
            Transparency = transparency;
            Blending = blending;
            DepthBuffer = depthBuffer;
            MaxTextureSize = maxTextureSize;
            MaxRenderTargets = maxRenderTargets;
        }
    }
}
