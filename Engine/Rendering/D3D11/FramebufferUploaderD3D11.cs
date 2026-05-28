// ============================================================================
// File:    FramebufferUploaderD3D11.cs
// Path:    Engine/Rendering/D3D11/FramebufferUploaderD3D11.cs
// Author:  BDC
// Purpose: Bridge between CPU-side Framebuffer (map + HUD) and GPU texture.
//          Uploads the CPU framebuffer pixels into a GPU Texture2D +
//          ShaderResourceView using D3D11DeviceCore.
// ============================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Rendering;          // Framebuffer
using SASZombieAssaultTD.Engine.Rendering.D3D11;    // D3D11DeviceCore
using Vortice.Direct3D11;

namespace SASZombieAssaultTD.Engine.Rendering.D3D11
{
    /// <summary>
    /// Uploads the CPU-side framebuffer (map + HUD) into a GPU texture.
    /// This is the bridge between your existing CPU rendering pipeline and
    /// the new D3D11 backend.
    /// 
    /// RenderContextD3D11 will call:
    ///     _uploader.Upload(framebuffer);
    /// 
    /// This class:
    /// - Detects framebuffer size changes.
    /// - Creates/resizes a GPU Texture2D.
    /// - Creates a ShaderResourceView.
    /// - Uploads pixel data each frame.
    /// - Exposes the SRV for fullscreen quad rendering.
    /// </summary>
    public sealed class FramebufferUploaderD3D11 : IDisposable
    {
        // --------------------------------------------------------------------
        // Fields
        // --------------------------------------------------------------------

        private readonly D3D11DeviceCore _deviceCore;

        // Cached GPU texture + SRV
        private ID3D11Texture2D _gpuTexture;
        private ID3D11ShaderResourceView _gpuTextureView;

        private int _lastWidth;
        private int _lastHeight;

        private bool _disposed;

        // --------------------------------------------------------------------
        // Construction
        // --------------------------------------------------------------------

        /// <summary>
        /// Creates a new framebuffer uploader bound to the given D3D11 device.
        /// </summary>
        public FramebufferUploaderD3D11(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
            DebugLogger.LogInfo("FramebufferUploaderD3D11: created");
        }

        // --------------------------------------------------------------------
        // Public API
        // --------------------------------------------------------------------

        /// <summary>
        /// Uploads the CPU framebuffer to the GPU texture.
        /// Detects size changes, recreates the texture/SRV if needed,
        /// and uploads the uint[] BGRA pixel buffer.
        /// </summary>
        public void Upload(Framebuffer framebuffer)
        {
            if (framebuffer == null)
                throw new ArgumentNullException(nameof(framebuffer));

            ThrowIfDisposed();

            int w = framebuffer.Width;
            int h = framebuffer.Height;

            if (w <= 0 || h <= 0)
            {
                DebugLogger.LogError($"FramebufferUploaderD3D11.Upload: invalid framebuffer size {w}x{h}");
                return;
            }

            // Create or resize GPU resources if dimensions changed
            if (_gpuTexture == null || w != _lastWidth || h != _lastHeight)
            {
                DebugLogger.LogInfo(
                    $"FramebufferUploaderD3D11: creating/resizing GPU texture from {_lastWidth}x{_lastHeight} to {w}x{h}");

                DisposeGpuResources();

                _gpuTexture = _deviceCore.CreateTexture(w, h);
                _gpuTextureView = _deviceCore.CreateTextureView(_gpuTexture);

                _lastWidth = w;
                _lastHeight = h;

                if (_gpuTextureView == null)
                {
                    DebugLogger.LogError("FramebufferUploaderD3D11: GPU texture view is null after creation");
                    return;
                }

                DebugLogger.LogInfo("FramebufferUploaderD3D11: GPU texture + SRV ready");
            }

            // Upload pixel data (uint[] BGRA)
            var pixels = framebuffer.Pixels;
            if (pixels == null)
            {
                DebugLogger.LogError("FramebufferUploaderD3D11.Upload: framebuffer.Pixels is null");
                return;
            }

            DebugLogger.LogDebug(
                $"FramebufferUploaderD3D11.Upload: uploading framebuffer {w}x{h} ({pixels.Length} pixels) to GPU texture");

            _deviceCore.UpdateTexture(_gpuTexture, pixels, w, h);
        }

        /// <summary>
        /// Returns the GPU ShaderResourceView for the uploaded framebuffer.
        /// </summary>
        public ID3D11ShaderResourceView GetTextureView()
        {
            ThrowIfDisposed();
            return _gpuTextureView;
        }

        // --------------------------------------------------------------------
        // Disposal
        // --------------------------------------------------------------------

        public void Dispose()
        {
            if (_disposed)
                return;

            DebugLogger.LogInfo("FramebufferUploaderD3D11: disposing");

            DisposeGpuResources();

            _disposed = true;
        }

        private void DisposeGpuResources()
        {
            if (_gpuTextureView != null)
            {
                _gpuTextureView.Dispose();
                _gpuTextureView = null;
            }

            if (_gpuTexture != null)
            {
                _gpuTexture.Dispose();
                _gpuTexture = null;
            }

            _lastWidth = 0;
            _lastHeight = 0;

            DebugLogger.LogInfo("FramebufferUploaderD3D11: disposed GPU texture + SRV");
        }

        // --------------------------------------------------------------------
        // Helpers
        // --------------------------------------------------------------------

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(FramebufferUploaderD3D11));
        }
    }
}
