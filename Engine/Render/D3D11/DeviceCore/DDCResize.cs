// =====================================================================================================
//  FILE: DDC_Resize.cs
//  PATH: Engine/Render/D3D11/DeviceCore/DDC_Resize.cs
//  SUBSYSTEM: D3D11 Backend – Swap Chain Resize Program
//
//  ROLE:
//      Provides deterministic resizing of the swap chain and triggers recreation of dependent GPU
//      resources such as the backbuffer and RTV.
//
//  RESPONSIBILITIES:
//      - Resize IDXGISwapChain1 buffers.
//      - Track and expose updated width/height.
//      - Notify caller to recreate backbuffer and RTV after resize.
//      - Dispose deterministically.
//
//  NON-RESPONSIBILITIES:
//      - Backbuffer creation (Core_Backbuffer).
//      - RTV creation (Core_RTV).
//      - Render‑target binding or clearing.
//      - Presentation.
//      - Device/context creation.
//
//  ARCHITECTURAL NOTES:
//      - Standalone GPU program.
//      - No partials.
//      - No block comments.
//      - Deterministic Option‑B formatting.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics;
using Vortice.DXGI;

namespace SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore
{
    internal sealed class DDCResize : IDisposable
    {
        private readonly IDXGISwapChain1 _swapChain;

        private int _width;
        private int _height;
        private bool _disposed;

        public int Width => _width;
        public int Height => _height;

        public DDCResize(IDXGISwapChain1 swapChain, int initialWidth, int initialHeight)
        {
            _swapChain = swapChain ?? throw new ArgumentNullException(nameof(swapChain));

            _width = initialWidth;
            _height = initialHeight;

            DLogger.Log($"Core_Resize: initialized with {_width}x{_height}.");
        }

        public void Resize(int width, int height)
        {
            if (_disposed)
                return;

            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Resize dimensions must be positive.");

            if (width == _width && height == _height)
                return;

            _width = width;
            _height = height;

            DLogger.Log($"Core_Resize: resizing swap chain to {_width}x{_height}.");

            _swapChain.ResizeBuffers(
                2,
                (uint)_width,
                (uint)_height,
                Vortice.DXGI.Format.B8G8R8A8_UNorm,
                SwapChainFlags.None);

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Core_Resize: resize complete. Backbuffer/RTV must be recreated by caller.");
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Core_Resize: disposed.");
        }
    }
}
