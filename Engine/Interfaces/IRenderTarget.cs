// ====================================================================================================
//  FILE: IRenderTarget.cs
//  PATH: Engine/Render
//  MODULE: Rendering Subsystem
//
//  ROLE:
//      Minimal deterministic abstraction for a GPU-backed render target surface.
//      Provides stable metadata for off-screen rendering, post-processing, and compositing.
//
//  RESPONSIBILITIES:
//      - Represent a GPU renderable surface (color, depth, or depth-stencil).
//      - Expose immutable dimensions and usage classification.
//      - Ensure proper disposal of backend GPU resources.
//      - Integrate cleanly with IGraphicsDevice and platform-specific renderers.
//
//  NON-RESPONSIBILITIES:
//      - Performing draw operations (handled by D3D11Adapter_Core / D3D11Adapter_Core).
//      - Managing resource loading or caching (handled by Resource Management Framework).
//      - Encoding or authoring texture files.
//      - Performing gameplay or UI logic.
//
//  ARCHITECTURAL NOTES:
//      - Render targets are backend-owned GPU resources (D3D11, BGFX, Vulkan).
//      - Size and usage are immutable for the lifetime of the render target.
//      - Backend-specific handles (textures, views) are stored in implementation classes.
// ====================================================================================================

using System;

namespace SASZombieAssaultTD.Engine.Interfaces
{
    /// <summary>
    /// Classification of render target usage.
    /// </summary>


    /// <summary>
    /// Minimal abstraction for a GPU render target surface.
    /// </summary>
    public interface IRenderTarget : IDisposable
    {
        /// <summary>
        /// Logical name of the render target.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Width in pixels.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Height in pixels.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Usage classification (color, depth, etc.).
        /// </summary>
        string Usage { get; }
    }
}
