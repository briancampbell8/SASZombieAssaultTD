// =====================================================================================================
//  FILE: DDCBackbuffer.cs
//  PATH: Engine/Render/D3D11/DeviceCore/DDCBackbuffer.cs
//  SUBSYSTEM: D3D11 Backend – Backbuffer Owner
//
//  ROLE:
//      Owns the swap-chain backbuffer texture and exposes it to higher‑level GPU programs.
//      Provides deterministic creation, retrieval, and disposal of the backbuffer.
//
//  RESPONSIBILITIES:
//      - Retrieve the backbuffer from IDXGISwapChain1.
//      - Create and maintain the ID3D11Texture2D instance.
//      - Dispose and recreate the backbuffer on resize.
//      - Guarantee no other subsystem touches or owns the backbuffer.
//
//  NON-RESPONSIBILITIES:
//      - Creating the swap chain (owned by DDC).
//      - Creating the render target view (handled by Core_RTV).
//      - Binding or clearing render targets (handled by Core_RenderTargets).
//      - Presentation (handled by D3D11Presentation).
//
//  ARCHITECTURAL NOTES:
//      - Standalone deterministic GPU program.
//      - No partials.
//      - No block comments.
//      - All GPU objects injected explicitly.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using Vortice.Direct3D11;
using Vortice.DXGI;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore_
{
    internal sealed class DDCBackbuffer : IDisposable
    {
        private readonly ID3D11Device _device;
        private readonly IDXGISwapChain1 _swapChain;

        private ID3D11Texture2D _backbuffer;
        private bool _disposed;

        public ID3D11Texture2D Backbuffer => _backbuffer;

        public DDCBackbuffer(ID3D11Device device, IDXGISwapChain1 swapChain)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
            _swapChain = swapChain ?? throw new ArgumentNullException(nameof(swapChain));

            CreateBackbuffer();
        }

        private void CreateBackbuffer()
        {
            _backbuffer?.Dispose();
            _backbuffer = _swapChain.GetBuffer<ID3D11Texture2D>(0);

            if (_backbuffer == null)
                throw new InvalidOperationException("Core_Backbuffer: failed to retrieve swap-chain backbuffer.");

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Core_Backbuffer: backbuffer created.");
        }

        public void Recreate()
        {
            if (_disposed)
                return;

            CreateBackbuffer();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _backbuffer?.Dispose();
            _backbuffer = null;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Core_Backbuffer: disposed.");
        }
    }
}
