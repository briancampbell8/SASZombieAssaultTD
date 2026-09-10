// =====================================================================================================
//  FILE: DDC_PipelineStateBinder.cs
//  PATH: Engine/Render/D3D11/DeviceCore/DDC_PipelineStateBinder.cs
//  SUBSYSTEM: D3D11 Backend – Pipeline State Binder
//
//  ROLE:
//      Owns deterministic binding of the D3D11 graphics pipeline for fullscreen quad rendering.
//
//  RESPONSIBILITIES:
//      - Maintain input layout, vertex buffer, index buffer, shaders, sampler, and topology.
//      - Bind pipeline state to the D3D11 device context in a deterministic order.
//      - Issue draw calls for the fullscreen quad using a bound texture SRV.
//      - Expose a simple entry point for higher-level fullscreen quad programs.
//
//  NON-RESPONSIBILITIES:
//      - Device, swap-chain, or backbuffer creation.
//      - Render target or depth-stencil binding.
//      - Viewport or scissor rectangle management.
//      - Resource lifetime outside of owned pipeline objects.
//
//  ARCHITECTURAL NOTES:
//      - Standalone GPU program.
//      - No partials.
//      - No block comments.
//      - Deterministic Option-B formatting.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using Vortice.Direct3D;
using Vortice.Direct3D11;

namespace SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore
{
    internal sealed class DDCPipelineStateBinder : IDisposable
    {
        private readonly ID3D11Device _device;
        private readonly ID3D11DeviceContext _context;

        private ID3D11InputLayout _inputLayout;
        private ID3D11Buffer _vertexBuffer;
        private ID3D11Buffer _indexBuffer;
        private ID3D11VertexShader _vertexShader;
        private ID3D11PixelShader _pixelShader;
        private ID3D11SamplerState _samplerState;

        private int _indexCount;
        private PrimitiveTopology _topology = PrimitiveTopology.TriangleList;

        private bool _disposed;

        public DDCPipelineStateBinder(
            ID3D11Device device,
            ID3D11DeviceContext context)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
            _context = context ?? throw new ArgumentNullException(nameof(context));

            DLogger.Log(LogSubsystems.ResourcesPipeline, "DDC_PipelineStateBinder: initialized.");
        }

        public void ConfigurePipeline(
            ID3D11InputLayout inputLayout,
            ID3D11Buffer vertexBuffer,
            ID3D11Buffer indexBuffer,
            int indexCount,
            ID3D11VertexShader vertexShader,
            ID3D11PixelShader pixelShader,
            ID3D11SamplerState samplerState,
            PrimitiveTopology topology = PrimitiveTopology.TriangleList)
        {
            ThrowIfDisposed();

            _inputLayout = inputLayout ?? throw new ArgumentNullException(nameof(inputLayout));
            _vertexBuffer = vertexBuffer ?? throw new ArgumentNullException(nameof(vertexBuffer));
            _indexBuffer = indexBuffer ?? throw new ArgumentNullException(nameof(indexBuffer));
            _vertexShader = vertexShader ?? throw new ArgumentNullException(nameof(vertexShader));
            _pixelShader = pixelShader ?? throw new ArgumentNullException(nameof(pixelShader));
            _samplerState = samplerState ?? throw new ArgumentNullException(nameof(samplerState));

            if (indexCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(indexCount));

            _indexCount = indexCount;
            _topology = topology;

            DLogger.Log($"DDC_PipelineStateBinder.ConfigurePipeline: indexCount={_indexCount}, topology={_topology}");
        }

        public void BindAndDraw(ID3D11ShaderResourceView textureView)
        {
            ThrowIfDisposed();

            if (textureView == null)
                throw new ArgumentNullException(nameof(textureView));

            if (_inputLayout == null ||
                _vertexBuffer == null ||
                _indexBuffer == null ||
                _vertexShader == null ||
                _pixelShader == null ||
                _samplerState == null ||
                _indexCount <= 0)
            {
                throw new InvalidOperationException("DDC_PipelineStateBinder: pipeline not fully configured.");
            }

            uint stride = sizeof(float) * 5;
            uint offset = 0;

            _context.IASetInputLayout(_inputLayout);
            _context.IASetVertexBuffers(0, new[] { _vertexBuffer }, new[] { stride }, new[] { offset });
            _context.IASetIndexBuffer(_indexBuffer, Vortice.DXGI.Format.R32_UInt, 0);
            _context.IASetPrimitiveTopology(_topology);

            _context.VSSetShader(_vertexShader);
            _context.PSSetShader(_pixelShader);

            _context.PSSetShaderResources(0, new[] { textureView });
            _context.PSSetSamplers(0, new[] { _samplerState });

            _context.DrawIndexed((uint)_indexCount, 0, 0);

            DLogger.Log(LogSubsystems.ResourcesPipeline, "DDC_PipelineStateBinder.BindAndDraw: fullscreen quad drawn.");
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _inputLayout?.Dispose();
            _vertexBuffer?.Dispose();
            _indexBuffer?.Dispose();
            _vertexShader?.Dispose();
            _pixelShader?.Dispose();
            _samplerState?.Dispose();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "DDC_PipelineStateBinder: disposed.");
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DDCPipelineStateBinder));
        }
    }
}
