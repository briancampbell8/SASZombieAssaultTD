// =====================================================================================================
//  FILE: DDCQuad.cs
//  PATH: Engine/Platform/DDCQuad.cs
//  SUBSYSTEM: Platform Abstraction Layer – Screen-Space Quad Pipeline
//
//  ROLE:
//      Provides deterministic screen-space quad pipeline creation and draw-path support.
//      Owns the quad vertex buffer, input layout, and shaders used for UI/HUD quads.
//
//  RESPONSIBILITIES:
//      Create dynamic quad vertex buffer.
//      Create quad vertex shader and pixel shader.
//      Create quad input layout.
//      Provide draw methods for colored and textured screen-space quads.
//
//  NON-RESPONSIBILITIES:
//      Creating RTV or backbuffer resources.
//      Managing swap chain or presentation.
//      Managing device or context lifetime.
//      Pipeline state management.
//
//  ARCHITECTURAL NOTES:
//      Standalone program; no partials.
//      All GPU objects are injected explicitly.
//      No hidden fields or cross-file state.
//      Pure screen-space quad pipeline logic only.
// =====================================================================================================

using System.Runtime.InteropServices;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore
{
    public sealed class CoreQuad
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct QuadVertex
        {
            public float X;
            public float Y;
            public float U;
            public float V;
            public float R;
            public float G;
            public float B;
            public float A;

            public QuadVertex(float x, float y, float u, float v, Color color)
            {
                X = x;
                Y = y;
                U = u;
                V = v;
                R = color.R / 255f;
                G = color.G / 255f;
                B = color.B / 255f;
                A = color.A / 255f;
            }
        }

        private readonly ID3D11Device _device;
        private readonly ID3D11DeviceContext _context;
        private readonly byte[] _vsBytecode;
        private readonly byte[] _psBytecode;
        private readonly int _width;
        private readonly int _height;
        private readonly ID3D11SamplerState _sampler;

        private ID3D11Buffer _quadVertexBuffer;
        private ID3D11InputLayout _quadInputLayout;
        private ID3D11VertexShader _quadVS;
        private ID3D11PixelShader _quadPS;

        private bool _initialized;

        public CoreQuad(
            ID3D11Device device,
            ID3D11DeviceContext context,
            byte[] quadVS_Bytecode,
            byte[] quadPS_Bytecode,
            ID3D11SamplerState sampler,
            int width,
            int height)
        {
            _device = device;
            _context = context;
            _vsBytecode = quadVS_Bytecode;
            _psBytecode = quadPS_Bytecode;
            _sampler = sampler;
            _width = width;
            _height = height;
        }

        public void Initialize()
        {
            if (_initialized)
                return;

            DisposeResources();

            var vbDesc = new BufferDescription(
                (uint)(Marshal.SizeOf<QuadVertex>() * 4),
                BindFlags.VertexBuffer,
                ResourceUsage.Dynamic,
                CpuAccessFlags.Write,
                ResourceOptionFlags.None,
                0);

            _quadVertexBuffer = _device.CreateBuffer(vbDesc);

            _quadVS = _device.CreateVertexShader(_vsBytecode);
            _quadPS = _device.CreatePixelShader(_psBytecode);

            var inputElements = new[]
            {
                new InputElementDescription("POSITION", 0, Format.R32G32_Float, 0, 0),
                new InputElementDescription("TEXCOORD", 0, Format.R32G32_Float, 8, 0),
                new InputElementDescription("COLOR",    0, Format.R32G32B32A32_Float, 16, 0)
            };

            _quadInputLayout = _device.CreateInputLayout(inputElements, _vsBytecode);

            _initialized = true;
        }

        public void DrawScreenSpaceQuad(float x, float y, float width, float height, Color color)
        {
            if (!_initialized)
                Initialize();

            float left = (x / _width) * 2f - 1f;
            float right = ((x + width) / _width) * 2f - 1f;
            float top = 1f - (y / _height) * 2f;
            float bottom = 1f - ((y + height) / _height) * 2f;

            var quadVertices = new[]
            {
                new QuadVertex(left,  top,    0f, 0f, color),
                new QuadVertex(right, top,    1f, 0f, color),
                new QuadVertex(left,  bottom, 0f, 1f, color),
                new QuadVertex(right, bottom, 1f, 1f, color)
            };

            _context.UpdateSubresource(quadVertices, _quadVertexBuffer);

            _context.IASetInputLayout(_quadInputLayout);
            _context.IASetPrimitiveTopology(PrimitiveTopology.TriangleStrip);

            uint stride = (uint)Marshal.SizeOf<QuadVertex>();
            uint offset = 0;

            // FIXED CS1503: Wrapped references into a continuous array layout mapping to satisfy Vortice's context bindings
            _context.IASetVertexBuffers(0, new[] { _quadVertexBuffer }, new[] { stride }, new[] { offset });

            _context.VSSetShader(_quadVS);
            _context.PSSetShader(_quadPS);
            _context.PSSetSampler(0, _sampler);

            _context.Draw(4, 0);
        }

        public void DrawTexturedQuad(
            ID3D11ShaderResourceView textureView,
            float x,
            float y,
            float width,
            float height,
            Color color)
        {
            if (!_initialized)
                Initialize();

            if (textureView == null)
                return;

            float left = (x / _width) * 2f - 1f;
            float right = ((x + width) / _width) * 2f - 1f;
            float top = 1f - (y / _height) * 2f;
            float bottom = 1f - ((y + height) / _height) * 2f;

            var quadVertices = new[]
            {
                new QuadVertex(left,  top,    0f, 0f, color),
                new QuadVertex(right, top,    1f, 0f, color),
                new QuadVertex(left,  bottom, 0f, 1f, color),
                new QuadVertex(right, bottom, 1f, 1f, color)
            };

            _context.UpdateSubresource(quadVertices, _quadVertexBuffer);

            _context.IASetInputLayout(_quadInputLayout);
            _context.IASetPrimitiveTopology(PrimitiveTopology.TriangleStrip);

            uint stride = (uint)Marshal.SizeOf<QuadVertex>();
            uint offset = 0;

            // FIXED CS1503: Wrapped references into a continuous array layout mapping to satisfy Vortice's context bindings
            _context.IASetVertexBuffers(0, new[] { _quadVertexBuffer }, new[] { stride }, new[] { offset });

            _context.VSSetShader(_quadVS);
            _context.PSSetShader(_quadPS);
            _context.PSSetShaderResource(0, textureView);
            _context.PSSetSampler(0, _sampler);

            _context.Draw(4, 0);
        }

        public void DisposeResources()
        {
            _quadVertexBuffer?.Dispose();
            _quadInputLayout?.Dispose();
            _quadVS?.Dispose();
            _quadPS?.Dispose();

            _quadVertexBuffer = null;
            _quadInputLayout = null;
            _quadVS = null;
            _quadPS = null;

            _initialized = false;
        }
    }
}
