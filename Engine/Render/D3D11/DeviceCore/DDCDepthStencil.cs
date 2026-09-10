// =====================================================================================================
//  FILE: DDCDepthStencil.cs
//  PATH: Engine/Render/D3D11/DeviceCore/DDCDepthStencil.cs
//  SUBSYSTEM: D3D11 Backend – Depth-Stencil Program
//
//  ROLE:
//      Owns deterministic creation, management, and disposal of the depth-stencil buffer and view used
//      by the D3D11 pipeline.
//
//  RESPONSIBILITIES:
//      - Create ID3D11Texture2D depth buffer for the current swap-chain size.
//      - Create ID3D11DepthStencilView for binding to the output-merger stage.
//      - Expose depth-stencil view to higher-level GPU programs.
//      - Clear the depth-stencil buffer deterministically.
//      - Recreate depth resources on resize.
//      - Dispose all depth-related GPU resources deterministically.
//
//  NON-RESPONSIBILITIES:
//      - RTV creation or binding.
//      - Swap-chain resizing.
//      - Presentation.
//      - Texture or sampler creation.
//      - Device/context lifetime management.
//      - Depth-stencil state configuration (handled by a separate program if needed).
//
//  ARCHITECTURAL NOTES:
//      - Standalone GPU program.
//      - No partials.
//      - No block comments.
//      - Deterministic Option-B formatting.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using Vortice.Direct3D11;
using Vortice.DXGI;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore
{
    internal sealed class DDCDepthStencil : IDisposable
    {
        private readonly ID3D11Device _device;
        private readonly ID3D11DeviceContext _context;

        private ID3D11Texture2D _depthTexture;
        private ID3D11DepthStencilView _depthView;
        private bool _disposed;

        private int _width;
        private int _height;

        public ID3D11DepthStencilView DepthStencilView => _depthView;
        public int Width => _width;
        public int Height => _height;

        public DDCDepthStencil(
            ID3D11Device device,
            ID3D11DeviceContext context,
            int width,
            int height)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
            _context = context ?? throw new ArgumentNullException(nameof(context));

            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("DepthStencil dimensions must be positive.");

            _width = width;
            _height = height;

            CreateDepthResources(_width, _height);
        }

        private void CreateDepthResources(int width, int height)
        {
            _depthView?.Dispose();
            _depthView = null;

            _depthTexture?.Dispose();
            _depthTexture = null;

            var depthDesc = new Texture2DDescription
            {
                Width = (uint)width,
                Height = (uint)height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.D24_UNorm_S8_UInt,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CPUAccessFlags = CpuAccessFlags.None,
                MiscFlags = ResourceOptionFlags.None
            };

            _depthTexture = _device.CreateTexture2D(depthDesc);

            if (_depthTexture == null)
                throw new InvalidOperationException("DDC_DepthStencil: failed to create depth texture.");

            _depthView = _device.CreateDepthStencilView(_depthTexture);

            if (_depthView == null)
                throw new InvalidOperationException("DDC_DepthStencil: failed to create depth-stencil view.");

            DLogger.Log($"DDC_DepthStencil: depth resources created ({width}x{height}).");
        }

        public void Recreate(int width, int height)
        {
            if (_disposed)
                return;

            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("DepthStencil dimensions must be positive.");

            if (width == _width && height == _height)
                return;

            _width = width;
            _height = height;

            DLogger.Log($"DDC_DepthStencil: recreating depth resources {_width}x{_height}.");

            CreateDepthResources(_width, _height);
        }

        public void Clear(float depth = 1.0f, byte stencil = 0)
        {
            if (_disposed)
                return;

            if (_depthView == null)
                throw new InvalidOperationException("DDC_DepthStencil: depth-stencil view is null.");

            _context.ClearDepthStencilView(
                _depthView,
                DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil,
                depth,
                stencil);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _depthView?.Dispose();
            _depthView = null;

            _depthTexture?.Dispose();
            _depthTexture = null;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "DDC_DepthStencil: disposed.");
        }
    }
}
