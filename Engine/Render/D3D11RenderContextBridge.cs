// ====================================================================================================
//  FILE: D3D11RenderContextBridge.cs
//  PATH: Engine/Render
//  MODULE: Render Core – D3D11 UI Bridge
//
//  ROLE:
//      Bridge between the engine Render subsystem and the ModernUIRenderer.
//      Forwards frame lifecycle operations and viewport configuration to both systems.
//
//  RESPONSIBILITIES:
//      - Forward BeginFrame / EndFrame calls to Renderer and ModernUIRenderer.
//      - Forward Clear() and Present() operations to the underlying Renderer.
//      - Synchronize viewport configuration between Renderer and ModernUIRenderer.
//      - Provide a render adapter when required by UI rendering paths.
//      - Maintain deterministic, thread-safe frame sequencing.
//
//  NON-RESPONSIBILITIES:
//      - Resource loading or caching.
//      - CPU-side composition or framebuffer drawing.
//      - Gameplay logic, UI layout, or diagnostics.
//      - GPU shader management or pipeline creation.
//
//  ARCHITECTURAL NOTES:
//      - Must be invoked from the main render thread.
//      - All UI rendering routed through ModernUIRenderer is GPU-accelerated.
//      - Bridge is thin: no batching, no caching, no state mutation beyond forwarding.
// ====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.UI.Rendering.Modern;

namespace SASZombieAssaultTD.Engine.Render
{
    public sealed class D3D11RenderContextBridge : IDisposable
    {
        private readonly Renderer _renderer;
        private readonly ModernUIRenderer _uiRenderer;
        private bool _disposed;
        private readonly D3D11Adapter_Core _renderContext;

        public Renderer Renderer => _renderer;
        public ModernUIRenderer UIRenderer => _uiRenderer;

        public D3D11RenderContextBridge(Renderer renderer, ModernUIRenderer uiRenderer, D3D11Adapter_Core adapterCore)
        {
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            _uiRenderer = uiRenderer ?? throw new ArgumentNullException(nameof(uiRenderer));
            _renderContext = adapterCore ?? throw new ArgumentNullException(nameof(adapterCore));
        }

        public void BeginFrame(float deltaSeconds)
        {
            if (_disposed)
                return;

            _renderer.BeginFrame();
            _uiRenderer.BeginFrame(this);
        }

        public void EndFrame(float deltaSeconds)
        {
            if (_disposed)
                return;

            _uiRenderer.EndFrame();
            _renderer.EndFrame();
        }

        public void Clear(System.Drawing.Color? color = null)
        {
            if (_disposed)
                return;

            if (color.HasValue)
            {
                var engineColor = Color.FromArgb(
                    color.Value.A,
                    color.Value.R,
                    color.Value.G,
                    color.Value.B);

                _renderer.Clear(engineColor);
            }
            else
            {
                _renderer.Clear(null);
            }
        }

        public void Present()
        {
            if (_disposed)
                return;

            _renderer.Present();
        }

        public void SetViewport(int width, int height)
        {
            if (_disposed)
                return;

            _renderer.SetViewport(width, height);
            _uiRenderer.SetViewport(width, height);
        }

        public D3D11Adapter_Core GetRenderContext() => _renderContext;

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
        }
    }
}
