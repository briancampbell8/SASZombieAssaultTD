/* ====================================================================================================
 *  FILE: RenderContextD3D11.cs
 *  PATH: Engine/Rendering/D3D11/RenderContextD3D11.cs
 *  SUBSYSTEM: Rendering
 *  ROLE: High-level D3D11 render orchestrator. Bridges CPU-side framebuffer output
 *        to the GPU pipeline using D3D11DeviceCore and FramebufferUploaderD3D11.
 *
 *  RESPONSIBILITIES:
 *      - Maintain deterministic render lifecycle entry points (BeginFrame, DrawFrame, Present).
 *      - Clear the GPU backbuffer using the configured clear color.
 *      - Upload CPU framebuffer data to GPU textures via FramebufferUploaderD3D11.
 *      - Issue fullscreen textured quad draw calls through D3D11DeviceCore.
 *      - Serve as the engine’s single abstraction for framebuffer-driven rendering.
 *
 *  NON-RESPONSIBILITIES:
 *      - Asset loading, texture creation, or shader compilation.
 *      - Scene graph traversal, batching, or render ordering.
 *      - Gameplay logic, UI logic, or post-processing effects.
 *      - Swap chain creation, device initialization, or GPU resource lifetime management.
 *
 *  DEPENDENCIES:
 *      - D3D11DeviceCore
 *      - FramebufferUploaderD3D11
 *      - Framebuffer (CPU-side render target)
 *      - Vortice.Direct3D11 (ID3D11ShaderResourceView and related interfaces)
 *
 *  CALLED BY:
 *      - RenderManager
 *      - GameRoot (via IRenderContext abstraction)
 *
 *  CALLS INTO:
 *      - D3D11DeviceCore (clear, fullscreen quad draw, present)
 *      - FramebufferUploaderD3D11 (CPU→GPU framebuffer upload, SRV retrieval)
 *
 *  ARCHITECTURAL NOTES:
 *      - Must remain deterministic and free of gameplay or UI logic.
 *      - Must not perform asset management or shader pipeline configuration.
 *      - All GPU resource ownership resides in D3D11DeviceCore and FramebufferUploaderD3D11.
 *      - Provides a stable, minimal rendering interface for higher-level engine systems.
 * ==================================================================================================== */

using System;
//
using SASZombieAssaultTD.Engine.Rendering;
using Vortice.Direct3D11;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Rendering.D3D11
{
    public sealed class RenderContextD3D11 : IDisposable
    {
        //--------------------------------------------------------------------
        //Fields
        //--------------------------------------------------------------------

        private readonly D3D11DeviceCore _deviceCore;
        private readonly FramebufferUploaderD3D11 _uploader;

        private float _clearR = 0.0f;
        private float _clearG = 0.0f;
        private float _clearB = 0.0f;
        private float _clearA = 1.0f;

        private bool _disposed;

        //--------------------------------------------------------------------
        //Construction
        //--------------------------------------------------------------------

        public RenderContextD3D11(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
            _uploader = new FramebufferUploaderD3D11(deviceCore);

            DLogger.Log("RenderContextD3D11: created and bound to D3D11DeviceCore");
        }

        //--------------------------------------------------------------------
        //Configuration
        //--------------------------------------------------------------------

        public void SetClearColor(float r, float g, float b, float a = 1.0f)
        {
            _clearR = r;
            _clearG = g;
            _clearB = b;
            _clearA = a;

            DLogger.Log(
                $"RenderContextD3D11.SetClearColor: r={r:F3}, g={g:F3}, b={b:F3}, a={a:F3}");
        }

        //--------------------------------------------------------------------
        //Frame lifecycle
        //--------------------------------------------------------------------

        public void BeginFrame()
        {
            ThrowIfDisposed();

            DLogger.Log(
                $"RenderContextD3D11.BeginFrame: clearing GPU backbuffer to ({_clearR:F3}, {_clearG:F3}, {_clearB:F3}, {_clearA:F3})");

            _deviceCore.ClearRenderTarget(_clearR, _clearG, _clearB, _clearA);
        }

        public void DrawFrame(Framebuffer framebuffer)
        {
            if (framebuffer == null)
                throw new ArgumentNullException(nameof(framebuffer));

            ThrowIfDisposed();

            DLogger.Log(
                $"RenderContextD3D11.DrawFrame: uploading framebuffer {framebuffer.Width}x{framebuffer.Height}");

            //Upload CPU framebuffer → GPU texture
            _uploader.Upload(framebuffer);

            //Retrieve shader resource view for fullscreen quad
            ID3D11ShaderResourceView textureView = _uploader.GetTextureView();
            if (textureView == null)
            {
                DLogger.Log("RenderContextD3D11.DrawFrame: texture view is null, skipping draw");
                return;
            }

            DLogger.Log(
                "RenderContextD3D11.DrawFrame: drawing fullscreen quad using framebuffer texture");

            //Issue fullscreen quad draw call
            _deviceCore.DrawFullscreenTexturedQuad(textureView);
        }

        public void Present()
        {
            ThrowIfDisposed();

            DLogger.Log("RenderContextD3D11.Present: presenting swap chain");
            _deviceCore.Present();
        }

        //--------------------------------------------------------------------
        //Disposal
        //--------------------------------------------------------------------

        public void Dispose()
        {
            if (_disposed)
                return;

            DLogger.Log("RenderContextD3D11: disposing");
            _uploader.Dispose();

            _disposed = true;
        }

        //--------------------------------------------------------------------
        //Helpers
        //--------------------------------------------------------------------

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RenderContextD3D11));
        }
    }
}
