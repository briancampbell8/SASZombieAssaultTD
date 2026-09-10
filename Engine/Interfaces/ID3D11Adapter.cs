// =====================================================================================================
//  FILE: ID3D11Adapter.cs
//  PATH: Engine/Interface/D3D11Adapter.cs
//  SUBSYSTEM: Interfaces
//
//  ROLE:
//      Top‑level deterministic process controller for the D3D11 rendering pipeline.
//      Owns subsystem adapters and delegates all high‑level operations.
//      Maintains unified access to the backend RenderContextD3D11.
//
//  RESPONSIBILITIES:
//      - Deterministic delegation to subsystem adapters.
//      - Maintain unified access to the backend RenderContextD3D11.
//      - Enforce strict Option‑B architecture: management and process control only.
//      - Expose clean D3D11Adapter_Core surface.
//
//  NON‑RESPONSIBILITIES:
//      - Any rendering, drawing, resource, device, swap‑chain, or pipeline logic.
//      - Any direct GPU calls.
//      - Any backend RenderContextD3D11 construction.
// =====================================================================================================

using System.Numerics;
using SASZombieAssaultTD.Engine.Graphics.Software;

namespace SASZombieAssaultTD.Engine.Interfaces
{
    public interface ID3D11Adapter
    {
        // ---------------------------------------------------------------------------------------------
        // VIEWPORT PROPERTIES (READ‑ONLY)
        // ---------------------------------------------------------------------------------------------

        Vector2 ViewportSize { get; }
        float ScreenWidth { get; }
        int ScreenHeight { get; }
        int Width { get; }
        int Height { get; }

        // ---------------------------------------------------------------------------------------------
        // MANAGEMENT FUNCTIONS — PURE DELEGATION
        // ---------------------------------------------------------------------------------------------

        void Initialize();
        void Shutdown();
        void BeginFrame();
        void EndFrame();
        void Present();
        void DrawFrame(FramebufferDrawing framebuffer);
        void DrawCircle(int x, int y, int radius, uint pathColor);
    }
}
