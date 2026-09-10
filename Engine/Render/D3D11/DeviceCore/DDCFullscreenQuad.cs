// =====================================================================================================
//  FILE: DDCFullscreenQuad.cs
//  PATH: Engine/Render/D3D11/DeviceCore/DDCFullscreenQuad.cs
//  SUBSYSTEM: D3D11 Backend – Fullscreen Quad Pipeline
//
//  ROLE:
//      Provides deterministic fullscreen-quad pipeline creation and draw-path support.
//      Owns the quad vertex buffer, index buffer, input layout, shaders, and sampler used for fullscreen
//      rendering, and delegates all pipeline binding to DDC_PipelineStateBinder.
//
//  RESPONSIBILITIES:
//      - Create fullscreen quad vertex buffer.
//      - Create fullscreen quad index buffer.
//      - Create quad vertex shader and pixel shader.
//      - Create quad input layout.
//      - Create quad sampler.
//      - Configure DDC_PipelineStateBinder with these GPU objects.
//      - Provide a draw method for fullscreen textured quads.
//
//  NON-RESPONSIBILITIES:
//      - Creating RTV or backbuffer resources.
//      - Managing swap chain or presentation.
//      - Managing device or context lifetime.
//      - Creating screen-space quads or UI quads.
//      - Direct pipeline state binding (handled by PipelineStateBinder).
//
//  ARCHITECTURAL NOTES:
//      - Standalone program; no partials.
//      - All GPU objects are injected explicitly.
//      - No hidden fields or cross-file state.
//      - Pure fullscreen-quad pipeline logic only.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Numerics;
using System.Runtime.InteropServices;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;


namespace SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FullscreenVertex
    {
        public Vector2 Position;
        public Vector2 TexCoord;
    }

    public sealed class Core_FullscreenQuad : IDisposable
    {
        private readonly ID3D11Device _device;
        private readonly DDCPipelineStateBinder _pipelineBinder;
        private readonly byte[] _vsBytecode;
        private readonly byte[] _psBytecode;

        private ID3D11Buffer _vertexBuffer;
        private ID3D11Buffer _indexBuffer;
        private ID3D11VertexShader _vs;
        private ID3D11PixelShader _ps;
        private ID3D11InputLayout _inputLayout;
        private ID3D11SamplerState _sampler;

        private bool _initialized;
        private bool _disposed;


        internal Core_FullscreenQuad(
            ID3D11Device device,
            DDCPipelineStateBinder pipelineBinder,
            byte[] quadVS_Bytecode,
            byte[] quadPS_Bytecode)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
            _pipelineBinder = pipelineBinder ?? throw new ArgumentNullException(nameof(pipelineBinder));
            _vsBytecode = quadVS_Bytecode ?? throw new ArgumentNullException(nameof(quadVS_Bytecode));
            _psBytecode = quadPS_Bytecode ?? throw new ArgumentNullException(nameof(quadPS_Bytecode));

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Core_FullscreenQuad: constructed.");
        }



        public void Initialize()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Core_FullscreenQuad));

            if (_initialized)
                return;

            DisposeResources();

            var quadVertices = new[]
            {
                new FullscreenVertex { Position = new Vector2(-1f,  1f), TexCoord = new Vector2(0f, 0f) },
                new FullscreenVertex { Position = new Vector2( 1f,  1f), TexCoord = new Vector2(1f, 0f) },
                new FullscreenVertex { Position = new Vector2(-1f, -1f), TexCoord = new Vector2(0f, 1f) },
                new FullscreenVertex { Position = new Vector2( 1f, -1f), TexCoord = new Vector2(1f, 1f) }
            };

            int vertexSize = Marshal.SizeOf<FullscreenVertex>();
            int totalSize = quadVertices.Length * vertexSize;

            var vbDesc = new BufferDescription
            {
                ByteWidth = (uint)totalSize,
                BindFlags = BindFlags.VertexBuffer,
                Usage = ResourceUsage.Immutable,
                CPUAccessFlags = CpuAccessFlags.None,
                MiscFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            };

            unsafe
            {
                fixed (FullscreenVertex* pVertices = quadVertices)
                {
                    var initData = new SubresourceData((IntPtr)pVertices);
                    _vertexBuffer = _device.CreateBuffer(vbDesc, initData);
                }
            }

            // Index buffer for fullscreen quad (triangle strip: 0,1,2,3)
            uint[] indices = { 0u, 1u, 2u, 3u };
            int indexSize = sizeof(uint) * indices.Length;

            var ibDesc = new BufferDescription
            {
                ByteWidth = (uint)indexSize,
                BindFlags = BindFlags.IndexBuffer,
                Usage = ResourceUsage.Immutable,
                CPUAccessFlags = CpuAccessFlags.None,
                MiscFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            };

            unsafe
            {
                fixed (uint* pIndices = indices)
                {
                    var initData = new SubresourceData((IntPtr)pIndices);
                    _indexBuffer = _device.CreateBuffer(ibDesc, initData);
                }
            }

            _vs = _device.CreateVertexShader(_vsBytecode);
            _ps = _device.CreatePixelShader(_psBytecode);

            var inputElements = new[]
            {
                new InputElementDescription("POSITION", 0, Format.R32G32_Float, 0, 0),
                new InputElementDescription("TEXCOORD", 0, Format.R32G32_Float, 8, 0)
            };

            _inputLayout = _device.CreateInputLayout(inputElements, _vsBytecode);

            var samplerDesc = new SamplerDescription
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                ComparisonFunc = ComparisonFunction.Never,
                MinLOD = 0f,
                MaxLOD = float.MaxValue,
                MipLODBias = 0f,
                MaxAnisotropy = 1
            };

            _sampler = _device.CreateSamplerState(samplerDesc);

            _pipelineBinder.ConfigurePipeline(
                _inputLayout,
                _vertexBuffer,
                _indexBuffer,
                indexCount: indices.Length,
                _vs,
                _ps,
                _sampler,
                PrimitiveTopology.TriangleStrip);

            _initialized = true;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Core_FullscreenQuad.Initialize: fullscreen quad pipeline configured.");
        }

        public void Draw(ID3D11ShaderResourceView textureView)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Core_FullscreenQuad));

            if (!_initialized)
                Initialize();

            if (textureView == null)
                return;

            _pipelineBinder.BindAndDraw(textureView);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            DisposeResources();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Core_FullscreenQuad.Dispose: resources disposed.");
        }

        private void DisposeResources()
        {
            _vertexBuffer?.Dispose();
            _indexBuffer?.Dispose();
            _vs?.Dispose();
            _ps?.Dispose();
            _inputLayout?.Dispose();
            _sampler?.Dispose();

            _vertexBuffer = null;
            _indexBuffer = null;
            _vs = null;
            _ps = null;
            _inputLayout = null;
            _sampler = null;

            _initialized = false;
        }
    }
}
