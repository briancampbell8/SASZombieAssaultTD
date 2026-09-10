/* ====================================================================================================
   FILE: DDC_Draw.cs
   PATH: Engine/Rendering/D3D11
   TYPE: D3D11 Device Core (Draw Path)

   ROLE:
       - Provides fullscreen-quad draw path for framebuffer presentation.
       - Binds framebuffer SRV, render target, and viewport.
       - Issues deterministic draw call against the swap chain backbuffer.

   RESPONSIBILITIES:
       - Bind RTV, viewport, input layout, vertex buffer, shaders, sampler, and blend state.
       - Bind framebuffer SRV to pixel shader.
       - Issue Draw(4) for fullscreen quad.

   NON-RESPONSIBILITIES:
       - Quad pipeline creation.
       - Sampler creation.
       - RTV creation.
       - Swap chain management or presentation.
       - Device/context lifetime management.

   NOTES:
       - Class name is deterministic: Core_Draw.
       - Assumes all pipeline resources have been initialized elsewhere.
       - No resource creation or teardown logic lives here.
==================================================================================================== */

using System;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.Mathematics;

namespace SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore
{
    public sealed class DDC_Draw
    {
        private readonly ID3D11DeviceContext _context;
        private readonly ID3D11RenderTargetView _rtv;
        private readonly ID3D11InputLayout _inputLayout;
        private readonly ID3D11Buffer _vertexBuffer;
        private readonly ID3D11VertexShader _vs;
        private readonly ID3D11PixelShader _ps;
        private readonly ID3D11SamplerState _sampler;
        private readonly ID3D11BlendState _blend;
        private readonly int _width;
        private readonly int _height;
        private readonly Action _present;

        public DDC_Draw(
            ID3D11DeviceContext context,
            ID3D11RenderTargetView rtv,
            ID3D11InputLayout inputLayout,
            ID3D11Buffer vertexBuffer,
            ID3D11VertexShader vs,
            ID3D11PixelShader ps,
            ID3D11SamplerState sampler,
            ID3D11BlendState blend,
            int width,
            int height,
            Action presentCallback)
        {
            _context = context;
            _rtv = rtv;
            _inputLayout = inputLayout;
            _vertexBuffer = vertexBuffer;
            _vs = vs;
            _ps = ps;
            _sampler = sampler;
            _blend = blend;
            _width = width;
            _height = height;
            _present = presentCallback;
        }

        /// <summary>
        /// Renders a fullscreen quad sampling the provided framebuffer SRV.
        /// </summary>
        public void RenderFramebufferDrawing(ID3D11ShaderResourceView framebufferSrv)
        {
            if (framebufferSrv == null)
                return;

            // Bind the backbuffer RTV.
            _context.OMSetRenderTargets(_rtv);

            // Configure viewport to match backbuffer size.
            var viewport = new Viewport(
                0,
                0,
                _width,
                _height,
                0.0f,
                1.0f);

            _context.RSSetViewport(viewport);

            // Bind framebuffer SRV.
            _context.PSSetShaderResource(0, framebufferSrv);

            // Bind fullscreen quad pipeline.
            _context.IASetPrimitiveTopology(PrimitiveTopology.TriangleStrip);
            _context.IASetInputLayout(_inputLayout);

            uint stride = (uint)(sizeof(float) * 8);
            uint offset = 0;

            // FIXED CS1503: Wrapped fields into explicit inline arrays to satisfy Vortice's context bindings
            _context.IASetVertexBuffers(0, new[] { _vertexBuffer }, new[] { stride }, new[] { offset });

            _context.VSSetShader(_vs);
            _context.PSSetShader(_ps);
            _context.PSSetSampler(0, _sampler);
            _context.OMSetBlendState(_blend);

            // Draw fullscreen quad.
            _context.Draw(4, 0);
        }

        // FIXED CS0102 / CS1656: Completely deleted the conflicting stub method that duplicated our field name.

        /// <summary>
        /// Renders the framebuffer and presents the swap chain.
        /// </summary>
        public void RenderFramebufferDrawingAndPresent(ID3D11ShaderResourceView framebufferSrv)
        {
            RenderFramebufferDrawing(framebufferSrv);
            _present?.Invoke();
        }
    }
}
