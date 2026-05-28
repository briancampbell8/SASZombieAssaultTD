/*
Program Name: SASZombieAssaultTD
File Path: Engine\Rendering\D3D11\D3D11Presentation.cs
Purpose: Rendering system for D3D11 graphics, sprites, text, and visual effects.
Features: Render command queuing, sprite batching, D3D11 presentation, and zombie rendering.
*/

// ============================================================================
// File:        D3D11Presentation.cs
// Author:      BDC
// Created:     2026-05-14
// Purpose:     Handles DXGI swap-chain presentation and frame submission.
// ============================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using SharpGen.Runtime;
using System;
using Vortice.DXGI;

namespace SASZombieAssaultTD.Engine.Rendering.D3D11
{
    /// <summary>
    /// Handles swap-chain presentation and DXGI error management.
    /// </summary>
    public sealed class D3D11Presentation : IDisposable
    {
        private readonly D3D11DeviceCore _deviceCore;

        private int _framesPresented;
        private int _vsyncMode;
        private bool _disposed;

        public int FramesPresented => _framesPresented;
        public bool VSyncEnabled => _deviceCore.VSyncEnabled;

        public D3D11Presentation(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
            _vsyncMode = deviceCore.VSyncEnabled ? 1 : 0;
        }

        /// <summary>
        /// Presents the backbuffer to the display.
        /// </summary>
        public void Present()
        {
            if (_disposed || _deviceCore.SwapChain == null)
                return;

            // Vortice expects uint for sync interval
            Result result = _deviceCore.SwapChain.Present((uint)_vsyncMode, PresentFlags.None);

            if (result.Failure)
            {
                HandleDxgiError(result);
                return;
            }

            _framesPresented++;
        }

        /// <summary>
        /// Handles DXGI device removal, reset, or other presentation failures.
        /// </summary>
        private static void HandleDxgiError(Result result)
        {
            int hr = result.Code;   // raw HRESULT

            string message = hr switch
            {
                unchecked((int)0x887A0005) => "D3D11Presentation: Device removed.", // DXGI_ERROR_DEVICE_REMOVED
                unchecked((int)0x887A0007) => "D3D11Presentation: Device reset.",   // DXGI_ERROR_DEVICE_RESET
                unchecked((int)0x887A0006) => "D3D11Presentation: Device hung.",    // DXGI_ERROR_DEVICE_HUNG

                _ => $"D3D11Presentation: Present failed (HRESULT=0x{hr:X8})."
            };

            DebugLogger.Log("ERROR", message);
        }





        /// <summary>
        /// Updates vsync mode dynamically.
        /// </summary>
        public void SetVSync(bool enabled)
        {
            _vsyncMode = enabled ? 1 : 0;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
        }
    }
}
