// =====================================================================================================
//  FILE: D3D11RenderContext.cs
//  PATH: Engine/Render/D3D11/D3D11RenderContext.cs
//  SUBSYSTEM: D3D11 Render Backend
//
//  ROLE:
//      Deterministic Direct3D 11 render context.
//      Owns the GPU frame lifecycle: clear, upload framebuffer, fullscreen quad draw, present.
//
//  RESPONSIBILITIES:
//      - Bind and clear render targets.
//      - Upload framebuffer via presenter.
//      - Draw fullscreen quad via Core_FullscreenQuad.
//      - Present swap chain.
//      - Provide stable viewport properties.
//
//  NON-RESPONSIBILITIES:
//      - HUD/UI rendering (HUDRenderer, ModernUIRenderer).
//      - GPU pipeline binding (PipelineStateBinder).
//      - Texture loading (TextureManager).
//      - Device creation (DDC).
//
//  ARCHITECTURAL NOTES:
//      - Minimal, deterministic, no stubs.
//      - No immediate-mode drawing functions.
//      - No NotImplementedException.
// =====================================================================================================

using System;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Graphics.Software;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace SASZombieAssaultTD.Engine.Render.D3D11
{
    public sealed class D3D11RenderContext : IDisposable
    {
        private readonly CoreRenderTargets _renderTargets;
        private readonly D3D11FramebufferDrawingPresenter _presenter;
        private readonly Core_FullscreenQuad _fullscreenQuad;
        private readonly D3D11Presentation _presentation;

        private float _clearR = 0f;
        private float _clearG = 0f;
        private float _clearB = 0f;
        private float _clearA = 1f;

        private bool _disposed;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public float AspectRatio { get; private set; }
        public float FieldOfView { get; private set; }
        public float ZNear { get; private set; }
        public float ZFar { get; private set; }
        public bool VSyncEnabled { get; internal set; }
        public bool Fullscreen { get; internal set; }
        public bool Windowed { get; internal set; }
        public IDXGIFactory1 Factory { get; internal set; }

        public D3D11RenderContext(
            CoreRenderTargets renderTargets,
            D3D11FramebufferDrawingPresenter presenter,
            Core_FullscreenQuad fullscreenQuad,
            D3D11Presentation presentation,
            int width,
            int height)
        {
            _renderTargets = renderTargets ?? throw new ArgumentNullException(nameof(renderTargets));
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            _fullscreenQuad = fullscreenQuad ?? throw new ArgumentNullException(nameof(fullscreenQuad));
            _presentation = presentation ?? throw new ArgumentNullException(nameof(presentation));

            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;
            AspectRatio = width / (float)height;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "D3D11RenderContext: initialized.");
        }

        public void SetClearColor(float r, float g, float b, float a = 1f)
        {
            _clearR = r;
            _clearG = g;
            _clearB = b;
            _clearA = a;
        }

        public void BeginFrame()
        {
            ThrowIfDisposed();

            _renderTargets.Bind();
            _renderTargets.Clear(_clearR, _clearG, _clearB, _clearA);
        }

        public void DrawFrame(FramebufferDrawing framebuffer)
        {
            ThrowIfDisposed();

            if (framebuffer == null)
                throw new ArgumentNullException(nameof(framebuffer));

            _presenter.EnsureSize(framebuffer);
            _presenter.Upload(framebuffer);

            ID3D11ShaderResourceView textureView = _presenter.ShaderResourceView;
            if (textureView == null)
                return;

            _fullscreenQuad.Draw(textureView);
        }

        public void Present()
        {
            ThrowIfDisposed();
            _presentation.Present();
        }

        public void ClearScreen()
        {
            ThrowIfDisposed();
            _renderTargets.Bind();
            _renderTargets.Clear(_clearR, _clearG, _clearB, _clearA);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _presenter?.Dispose();
            _fullscreenQuad?.Dispose();
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(D3D11RenderContext));
        }

    }
}
