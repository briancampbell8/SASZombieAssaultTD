// =====================================================================================================
//  FILE: DDC_RasterizerState.cs
//  PATH: Engine/Render/D3D11/DeviceCore/DDC_RasterizerState.cs
//  SUBSYSTEM: D3D11 Backend – Rasterizer State Program
//
//  ROLE:
//      Owns deterministic creation, binding, and disposal of the D3D11 rasterizer state.
//
//  RESPONSIBILITIES:
//      - Create ID3D11RasterizerState for the current pipeline configuration.
//      - Bind rasterizer state to the D3D11 device context.
//      - Expose rasterizer state to higher-level GPU programs.
//      - Dispose rasterizer state deterministically.
//
//  NON-RESPONSIBILITIES:
//      - Viewport creation or binding.
//      - Scissor rectangle creation or binding.
//      - Blend, depth-stencil, or sampler state management.
//      - Device/context lifetime management.
//      - Swap-chain resizing or presentation.
//
//  ARCHITECTURAL NOTES:
//      - Standalone GPU program.
//      - No partials.
//      - No block comments.
//      - Deterministic Option-B formatting.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics;
using Vortice.Direct3D11;

namespace SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore
{
    internal sealed class DDCRasterizerState : IDisposable
    {
        private readonly ID3D11Device _device;
        private readonly ID3D11DeviceContext _context;

        private ID3D11RasterizerState _rasterizerState;
        private bool _disposed;

        public ID3D11RasterizerState RasterizerState => _rasterizerState;

        public DDCRasterizerState(
            ID3D11Device device,
            ID3D11DeviceContext context)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
            _context = context ?? throw new ArgumentNullException(nameof(context));

            CreateRasterizerState();
        }

        private void CreateRasterizerState()
        {
            _rasterizerState?.Dispose();
            _rasterizerState = null;

            var desc = new RasterizerDescription
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.Back,
                FrontCounterClockwise = false,
                DepthBias = 0,
                DepthBiasClamp = 0.0f,
                SlopeScaledDepthBias = 0.0f,
                DepthClipEnable = true,
                ScissorEnable = false,
                MultisampleEnable = false,
                AntialiasedLineEnable = false
            };

            _rasterizerState = _device.CreateRasterizerState(desc);

            if (_rasterizerState == null)
                throw new InvalidOperationException("DDC_RasterizerState: failed to create rasterizer state.");

            DLogger.Log(LogSubsystems.ResourcesPipeline, "DDC_RasterizerState: rasterizer state created.");
        }

        public void Bind()
        {
            if (_disposed)
                return;

            if (_rasterizerState == null)
                throw new InvalidOperationException("DDC_RasterizerState: rasterizer state is null.");

            _context.RSSetState(_rasterizerState);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _rasterizerState?.Dispose();
            _rasterizerState = null;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "DDC_RasterizerState: disposed.");
        }
    }
}
