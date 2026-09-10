// ====================================================================================================
//  FILE: D3D11Presentation.cs
//  PATH: Engine/Rendering/D3D11
//  SUBSYSTEM: D3D11 Presentation Layer
//
//  ROLE:
//      Owns deterministic DXGI swap-chain presentation.
//      Handles DXGI device-loss and presentation failure conditions.
//      Tracks frame presentation statistics.
//
//  RESPONSIBILITIES:
//      Present the swap-chain backbuffer.
//      Honor vsync configuration.
//      Log and classify DXGI presentation failures.
//      Maintain frame counters.
//
//  NON-RESPONSIBILITIES:
//      Rendering or GPU resource creation.
//      Swap-chain creation.
//      RTV creation or binding.
//      Fullscreen quad drawing.
//
//  ARCHITECTURAL NOTES:
//      Standalone program; no partials.
//      All GPU objects are injected explicitly.
//      No hidden fields or cross-file state.
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SharpGen.Runtime;
using Vortice.DXGI;

namespace SASZombieAssaultTD.Engine.Render.D3D11
{
    public sealed class D3D11Presentation : IDisposable
    {
        private readonly IDXGISwapChain1 _swapChain;
        private bool _vsyncEnabled;
        private int _framesPresented;
        private bool _disposed;

        public int FramesPresented => _framesPresented;
        public bool VSyncEnabled => _vsyncEnabled;

        public D3D11Presentation(
            IDXGISwapChain1 swapChain,
            bool vsyncEnabled)
        {
            _swapChain = swapChain ?? throw new ArgumentNullException(nameof(swapChain));
            _vsyncEnabled = vsyncEnabled;
        }

        public void Present()
        {
            if (_disposed)
                return;

            uint syncInterval = _vsyncEnabled ? 1u : 0u;

            Result result = _swapChain.Present(syncInterval, PresentFlags.None);

            if (result.Failure)
            {
                HandleDxgiError(result.Code);
                return;
            }

            _framesPresented++;
        }

        private static void HandleDxgiError(int hr)
        {
            string message = hr switch
            {
                unchecked((int)0x887A0005) => "DXGI_ERROR_DEVICE_REMOVED",
                unchecked((int)0x887A0007) => "DXGI_ERROR_DEVICE_RESET",
                unchecked((int)0x887A0006) => "DXGI_ERROR_DEVICE_HUNG",
                unchecked((int)0x887A0020) => "DXGI_ERROR_DRIVER_INTERNAL_ERROR",
                unchecked((int)0x887A0001) => "DXGI_ERROR_INVALID_CALL",
                _ => $"DXGI_PRESENT_FAILED (HRESULT=0x{hr:X8})"
            };

            DLogger.Log(LogSubsystems.D3D11, LogEnums.LogLevel.Error,
                $"D3D11Presentation: Present failure → {message}");
        }

        public void SetVSync(bool enabled)
        {
            _vsyncEnabled = enabled;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
        }
    }
}
