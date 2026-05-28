// ============================================================================
// File:    D3D11DeviceCoreRTV.cs
// Path:    Engine/Rendering/D3D11/D3D11DeviceCore.RTV.cs
// Author:  BDC
// Purpose: Vortice-based RTV + viewport + ClearRenderTarget implementation
//          for D3D11DeviceCore.
// ============================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using System;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace SASZombieAssaultTD.Engine.Rendering.D3D11
{
    public sealed partial class D3D11DeviceCore : IDisposable
    {
        // --------------------------------------------------------------------
        // Fields
        // --------------------------------------------------------------------

        private ID3D11RenderTargetView? _backbufferRTV;
        private bool _rtvInitialized;

        // --------------------------------------------------------------------
        // Public API
        // --------------------------------------------------------------------

        /// <summary>
        /// Initializes the backbuffer RTV and viewport.
        /// Call this once after the swap chain is created or resized.
        /// </summary>
        public void InitializeRenderTargets(int width, int height)
        {
            DebugLogger.LogInfo(
                $"D3D11DeviceCore.InitializeRenderTargets: creating RTV + viewport for {width}x{height}");

            _backbufferRTV?.Dispose();
            _backbufferRTV = null;
            _rtvInitialized = false;

            // 1. Get backbuffer texture from swap chain
            using ID3D11Texture2D backbuffer = SwapChain.GetBuffer<ID3D11Texture2D>(0);

            // 2. Create RTV
            _backbufferRTV = Device.CreateRenderTargetView(backbuffer);

            // 3. Bind RTV
            ImmediateContext.OMSetRenderTargets(_backbufferRTV, null);

            // 4. Set viewport
            var viewport = new Viewport(0, 0, width, height, 0.0f, 1.0f);
            ImmediateContext.RSSetViewport(viewport);

            _rtvInitialized = true;

            DebugLogger.LogInfo("D3D11DeviceCore.InitializeRenderTargets: RTV + viewport initialized");
        }

        /// <summary>
        /// Clears the GPU backbuffer to the specified color.
        /// </summary>
        public void ClearRenderTarget(float r, float g, float b, float a)
        {
            if (!_rtvInitialized || _backbufferRTV is null)
            {
                DebugLogger.LogWarning(
                    "D3D11DeviceCore.ClearRenderTarget: RTV not initialized, skipping clear");
                return;
            }

            var color = new Color4(r, g, b, a);

            DebugLogger.LogDebug(
                $"D3D11DeviceCore.ClearRenderTarget: clearing to ({r:F3}, {g:F3}, {b:F3}, {a:F3})");

            ImmediateContext.ClearRenderTargetView(_backbufferRTV, color);
        }

        // --------------------------------------------------------------------
        // Disposal
        // --------------------------------------------------------------------

        private void DisposeRtvResources()
        {
            _backbufferRTV?.Dispose();
            _backbufferRTV = null;
            _rtvInitialized = false;
        }
    }
}
