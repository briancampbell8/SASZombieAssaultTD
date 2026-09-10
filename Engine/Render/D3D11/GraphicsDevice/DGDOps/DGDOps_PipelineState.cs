//==========================================================================================
// FILE: DGDOps_PipelineState.cs
// PATH: Engine/Render/D3D11/GraphicsDevice/DGDOps/DGDOps_PipelineState.cs
// SUBSYSTEM: DGD / Unified Pipeline State Binding
//
// ROLE:
//     Provides deterministic binding of pipeline state objects:
//     - Shaders
//     - Input layouts
//     - Blend states
//     - Rasterizer states
//     - Depth-stencil states
//
// RESPONSIBILITIES:
//     - Bind vertex/pixel shaders.
//     - Bind input layouts.
//     - Bind blend/rasterizer/depth-stencil states.
//     - Provide a clean, modular pipeline configuration API.
//
// NON-RESPONSIBILITIES:
//     - Creating shaders or pipeline objects (other DGDOps subsystems handle that).
//     - Performing draw calls.
//     - Managing textures or render targets.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using Vortice.Direct3D11;

namespace SASZombieAssaultTD.Engine.Render.D3D11.GraphicsDevice.DGDOps
{
    /// <summary>
    /// Deterministic unified pipeline-state subsystem.
    /// </summary>
    public sealed class DGDOps_PipelineState
    {
        private readonly D3D11DeviceCore _deviceCore;

        public DGDOps_PipelineState(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
        }

        // -------------------------------------------------------------------------
        //  SHADERS
        // -------------------------------------------------------------------------
        public void BindShaders(ID3D11VertexShader vs, ID3D11PixelShader ps)
        {
            var ctx = _deviceCore.Context;
            if (ctx == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            ctx.VSSetShader(vs);
            ctx.PSSetShader(ps);
        }

        public void UnbindShaders()
        {
            var ctx = _deviceCore.Context;
            if (ctx == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            ctx.VSSetShader(null);
            ctx.PSSetShader(null);
        }

        // -------------------------------------------------------------------------
        //  INPUT LAYOUT
        // -------------------------------------------------------------------------
        public void BindInputLayout(ID3D11InputLayout layout)
        {
            var ctx = _deviceCore.Context;
            if (ctx == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            ctx.IASetInputLayout(layout);
        }

        public void UnbindInputLayout()
        {
            var ctx = _deviceCore.Context;
            if (ctx == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            ctx.IASetInputLayout(null);
        }

        // -------------------------------------------------------------------------
        //  BLEND STATE
        // -------------------------------------------------------------------------
        public void BindBlendState(ID3D11BlendState blendState)
        {
            var ctx = _deviceCore.Context;
            if (ctx == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            ctx.OMSetBlendState(blendState);
        }

        public void UnbindBlendState()
        {
            var ctx = _deviceCore.Context;
            if (ctx == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            ctx.OMSetBlendState(null);
        }

        // -------------------------------------------------------------------------
        //  RASTERIZER STATE
        // -------------------------------------------------------------------------
        public void BindRasterizerState(ID3D11RasterizerState rasterizer)
        {
            var ctx = _deviceCore.Context;
            if (ctx == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            ctx.RSSetState(rasterizer);
        }

        public void UnbindRasterizerState()
        {
            var ctx = _deviceCore.Context;
            if (ctx == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            ctx.RSSetState(null);
        }

        // -------------------------------------------------------------------------
        //  DEPTH-STENCIL STATE
        // -------------------------------------------------------------------------
        public void BindDepthStencilState(ID3D11DepthStencilState dss, int stencilRef = 0)
        {
            var ctx = _deviceCore.Context;
            if (ctx == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            ctx.OMSetDepthStencilState(dss, (uint)stencilRef);
        }

        public void UnbindDepthStencilState()
        {
            var ctx = _deviceCore.Context;
            if (ctx == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            ctx.OMSetDepthStencilState(null, 0);
        }

        // -------------------------------------------------------------------------
        //  FULL PIPELINE CONFIGURATION
        // -------------------------------------------------------------------------
        public void ConfigurePipeline(
            ID3D11VertexShader vs,
            ID3D11PixelShader ps,
            ID3D11InputLayout layout,
            ID3D11BlendState blend,
            ID3D11RasterizerState rasterizer,
            ID3D11DepthStencilState depthStencil)
        {
            BindShaders(vs, ps);
            BindInputLayout(layout);
            BindBlendState(blend);
            BindRasterizerState(rasterizer);
            BindDepthStencilState(depthStencil);
        }

        public void ResetPipeline()
        {
            UnbindShaders();
            UnbindInputLayout();
            UnbindBlendState();
            UnbindRasterizerState();
            UnbindDepthStencilState();
        }
    }
}
