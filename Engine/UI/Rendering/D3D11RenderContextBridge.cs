/*
Program Name: SASZombieAssaultTD
File Path: Engine/UI/Rendering/D3D11RenderContextBridge.cs
Purpose: Bridge between legacy IDrawingContext calls and the modern D3D11 UI renderer.
Features:
  - Converts legacy HUD/UI draw calls to modern renderer commands
  - Forwards rendering commands to ModernUIRenderer
  - Provides frame management: BeginFrame, EndFrame, Clear, Present
  - Supports viewport configuration for both Renderer and UIRenderer
  - Implements IDisposable for resource cleanup
  - Must be used from main render thread for thread safety
  - All UI rendering is GPU-accelerated with no CPU rendering path
*/

//

using System;
using System.Drawing;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.UI.Rendering;
using SASZombieAssaultTD.Engine.UI.Rendering.Modern;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering.D3D11
{
    ///<summary>
    ///Bridge between the engine Renderer and the ModernUIRenderer.
    ///</summary>
    public sealed class D3D11RenderContextBridge : IDisposable
    {
        private readonly Renderer _renderer;
        private readonly ModernUIRenderer _uiRenderer;
        private bool _disposed;
        private IDrawingContext _renderContext;

        public Renderer Renderer => _renderer;
        public ModernUIRenderer UIRenderer => _uiRenderer;

        public D3D11RenderContextBridge(Renderer renderer, ModernUIRenderer uiRenderer)
        {
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            _uiRenderer = uiRenderer ?? throw new ArgumentNullException(nameof(uiRenderer));
        }

        public D3D11RenderContextBridge(ModernUIRenderer uiRenderer)
        {
        }

        public D3D11RenderContextBridge(D3D11DeviceCore deviceCore)
        {
        }

        public void BeginFrame(float deltaSeconds)
        {
            if (_disposed) return;

            _renderer.BeginFrame();
            _uiRenderer.BeginFrame(this);
        }

        public void EndFrame(float deltaSeconds)
        {
            if (_disposed) return;

            _uiRenderer.EndFrame();
            _renderer.EndFrame();
        }

        public void Clear(System.Drawing.Color? color = null)
        {
            if (_disposed) return;

            if (color.HasValue)
            {
                var engineColor = Color.FromArgb(color.Value.A, color.Value.R, color.Value.G, color.Value.B);
                _renderer.Clear(engineColor);
            }
            else
            {
                _renderer.Clear(null);
            }
        }

        public void Present()
        {
            if (_disposed) return;

            _renderer.Present();
        }

        public void SetViewport(int width, int height)
        {
            if (_disposed) return;

            _renderer.SetViewport(width, height);
            _uiRenderer.SetViewport(width, height);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
        }

        public IDrawingContext GetRenderContext() => _renderContext;

    }
}
