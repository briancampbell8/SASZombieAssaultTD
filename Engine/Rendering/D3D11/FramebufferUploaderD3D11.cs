// ====================================================================================================
//  FILE: FramebufferUploaderD3D11.cs
//  PATH: Engine/Rendering/ 
//  PROGRAM: FramebufferUploaderD3D11.cs
//  MODULE: Resource Management Framework
//  ROLE:
//      Defines the structures, loaders, and integration points responsible for discovering, validating, and providing engine resources in a deterministic manner.
//
//  RESPONSIBILITIES:
//      - Provide a unified API for loading, caching, and resolving engine resources.
//      - Enforce deterministic resource lookup and lifecycle rules.
//      - Abstract file formats, storage locations, and integration layers behind a stable interface.
//      - Ensure resource availability for all engine subsystems (Rendering, Audio, Gameplay, UI).
//
//  NON-RESPONSIBILITIES:
//      - Performing rendering or GPU upload operations.
//      - Managing gameplay logic or scene entities.
//      - Handling diagnostics, logging, or performance metrics.
//      - Encoding or authoring resource files.
//
//  ARCHITECTURAL NOTES:
//      - The Resource Management Framework acts as the central authority for all asset retrieval.
//      - Resource modules must remain pure: no side effects outside resource acquisition and validation.
//      - All resource types (textures, data files, definitions, metadata) must follow deterministic load rules.
//  ====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using Vortice.Direct3D11;

namespace SASZombieAssaultTD.Engine.Rendering.D3D11
{
    public sealed class FramebufferUploaderD3D11 : IDisposable
    {
        // ---------------------------------------------------------------------------------------------
        // Fields
        // ---------------------------------------------------------------------------------------------
        private readonly D3D11DeviceCore _deviceCore;

        private ID3D11Texture2D _gpuTexture;
        private ID3D11ShaderResourceView _gpuTextureView;

        private int _lastWidth;
        private int _lastHeight;

        private bool _disposed;

        // ---------------------------------------------------------------------------------------------
        // Construction
        // ---------------------------------------------------------------------------------------------
        public FramebufferUploaderD3D11(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));

            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Info,
                "D3D11",
                "FramebufferUploaderD3D11 created");
        }

        // ---------------------------------------------------------------------------------------------
        // Public API
        // ---------------------------------------------------------------------------------------------
        public void Upload(Framebuffer framebuffer)
        {
            if (framebuffer == null)
                throw new ArgumentNullException(nameof(framebuffer));

            ThrowIfDisposed();

            int w = framebuffer.Width;
            int h = framebuffer.Height;

            if (w <= 0 || h <= 0)
            {
                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogLevel.Warning,
                    "D3D11",
                    $"Invalid framebuffer size {w}x{h}");
                return;
            }

            // Detect size changes and recreate GPU resources
            if (_gpuTexture == null || w != _lastWidth || h != _lastHeight)
            {
                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogLevel.Info,
                    "D3D11",
                    $"Resizing GPU texture from {_lastWidth}x{_lastHeight} to {w}x{h}");

                DisposeGpuResources();

                _gpuTexture = _deviceCore.CreateTexture(w, h);
                _gpuTextureView = _deviceCore.CreateTextureView(_gpuTexture);

                _lastWidth = w;
                _lastHeight = h;

                if (_gpuTextureView == null)
                {
                    DLogger.Log(
                        LogSubsystems.Rendering,
                        LogLevel.Error,
                        "D3D11",
                        "GPU texture view is null after creation");
                    return;
                }

                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogLevel.Info,
                    "D3D11",
                    "GPU texture + SRV ready");
            }

            // Upload pixel data
            var pixels = framebuffer.Pixels;
            if (pixels == null)
            {
                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogLevel.Error,
                    "D3D11",
                    "Framebuffer.Pixels is null");
                return;
            }

            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "D3D11",
                $"Uploading framebuffer {w}x{h} ({pixels.Length} pixels)");

            _deviceCore.UpdateTexture(_gpuTexture, pixels, w, h);
        }

        public ID3D11ShaderResourceView GetTextureView()
        {
            ThrowIfDisposed();
            return _gpuTextureView;
        }

        // ---------------------------------------------------------------------------------------------
        // Disposal
        // ---------------------------------------------------------------------------------------------
        public void Dispose()
        {
            if (_disposed)
                return;

            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Info,
                "D3D11",
                "Disposing FramebufferUploaderD3D11");

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

            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "D3D11",
                "Disposed GPU texture + SRV");
        }

        // ---------------------------------------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------------------------------------
        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(FramebufferUploaderD3D11));
        }
    }
}
