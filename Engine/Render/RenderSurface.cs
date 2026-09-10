// ====================================================================================================
//  FILE: RenderSurface.cs
//  PATH: Engine/Rendering/
//  MODULE: Rendering / Surface
//
//  ROLE:
//      Represents a deterministic software render target backed by a FramebufferDrawing. Provides
//      binding, clearing, and disposal logic for off‑screen rendering surfaces.
//
//  RESPONSIBILITIES:
//      - Allocate and own an off‑screen framebuffer.
//      - Provide Bind() to prepare the surface for rendering.
//      - Clear the framebuffer deterministically.
//      - Dispose framebuffer resources safely.
//
//  NON‑RESPONSIBILITIES:
//      - Resource loading or asset management.
//      - GPU rendering or SpriteBatch submission.
//      - Gameplay logic or diagnostics.
//
//  NOTES:
//      This module belongs to the Rendering subsystem, not the Resource Management Framework.
// ====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
// using SASZombieAssaultTD.Engine.Extensions; // Extensions Removed
using SASZombieAssaultTD.Engine.Graphics.Software;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Render
{
    public sealed class RenderSurface : IDisposable
    {
        public int Width { get; }
        public int Height { get; }

        public FramebufferDrawing? OffscreenBuffer => _offscreenBuffer;

        private FramebufferDrawing? _offscreenBuffer;
        private bool _disposed;

        public RenderSurface(int width, int height)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "Info", $"[RenderSurface] Creating surface {width}x{height}");

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;

            _offscreenBuffer = new FramebufferDrawing(width, height);
        }

        public void Bind(D3D11Adapter_Core renderContext)
        {
            if (renderContext == null)
                throw new ArgumentNullException(nameof(renderContext));

            // Clear the off‑screen buffer before rendering
            _offscreenBuffer?.ClearScreen(Color.Black);

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Info",
                $"[RenderSurface] Bound off‑screen surface ({Width}x{Height}).");
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _offscreenBuffer = null;
            _disposed = true;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Info", "[RenderSurface] Disposed.");
        }
    }
}
