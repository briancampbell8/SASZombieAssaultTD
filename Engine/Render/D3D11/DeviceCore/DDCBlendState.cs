// =====================================================================================================
//  FILE: DDCBlendState.cs
//  PATH: Engine/Render/D3D11/DeviceCore/DDCBlendState.cs
//  SUBSYSTEM: D3D11 Backend – Blend State Program
//
//  ROLE:
//      Owns the deterministic alpha‑blend state used by all D3D11 rendering programs.
//      Provides creation, binding, and disposal of the blend state.
//
//  RESPONSIBILITIES:
//      - Create a single ID3D11BlendState instance.
//      - Bind the blend state to the output‑merger stage.
//      - Dispose the blend state deterministically.
//
//  NON-RESPONSIBILITIES:
//      - Creating device, context, or swap chain.
//      - Managing render targets or backbuffer.
//      - Performing draw calls.
//      - Pipeline orchestration.
//
//  ARCHITECTURAL NOTES:
//      - Standalone GPU program.
//      - No partials.
//      - No block comments.
//      - All GPU objects injected explicitly.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using Vortice.Direct3D11;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.Render.RenderEnums;

namespace SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore_
{
    internal sealed class DDCBlendState : IDisposable
    {
        private readonly ID3D11Device _device;
        private readonly ID3D11DeviceContext _context;

        private ID3D11BlendState _blendState;
        private bool _disposed;

        public ID3D11BlendState BlendState => _blendState;

        public DDCBlendState(ID3D11Device device, ID3D11DeviceContext context)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
            _context = context ?? throw new ArgumentNullException(nameof(context));

            CreateBlendState();
        }

        private void CreateBlendState()
        {
            var desc = new BlendDescription
            {
                AlphaToCoverageEnable = false,
                IndependentBlendEnable = false
            };

            desc.RenderTarget[0] = new RenderTargetBlendDescription
            {
                BlendEnable = true,
                SourceBlend = Blend.One,
                DestinationBlend = Blend.InverseSourceAlpha,
                BlendOperation = BlendOperation.Add,
                SourceBlendAlpha = Blend.One,
                DestinationBlendAlpha = Blend.InverseSourceAlpha,
                BlendOperationAlpha = BlendOperation.Add,
                RenderTargetWriteMask = (ColorWriteEnable)ColorWriteMaskFlags.All
            };

            _blendState = _device.CreateBlendState(desc);

            if (_blendState == null)
                throw new InvalidOperationException("Core_BlendState: failed to create blend state.");

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Core_BlendState: blend state created.");
        }

        public void Bind()
        {
            if (_disposed)
                return;

            _context.OMSetBlendState(_blendState);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _blendState?.Dispose();
            _blendState = null;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Core_BlendState: disposed.");
        }
    }
}
