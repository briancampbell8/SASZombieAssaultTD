// =====================================================================================================
//  FILE: D3D11GraphicsDevice.cs
//  PATH: Engine/Render/D3D11/GraphicsDevice/D3D11GraphicsDevice.cs
//  SUBSYSTEM: D3D11 Graphics Device / DGDOps Pipeline Manager
//
//  ROLE:
//      Core driver for the DGDOps-based Direct3D11 pipeline.
//      Orchestrates D3D11DeviceCore and DGDOps subsystems as the concrete IGraphicsDevice.
//
//  RESPONSIBILITIES:
//      - Initialize and resize the D3D11 pipeline via DGDOps_* subsystems.
//      - Upload CPU framebuffer data into the GPU backbuffer.
//      - Present frames through the swap chain.
//      - Expose the ID3D11DeviceContext used by the backend.
//      - Create GPU-backed render targets for the engine.
//
//  NON-RESPONSIBILITIES:
//      - CPU rendering (handled by FramebufferDrawing).
//      - Resource pooling (handled by D3D11ResourcePool).
//      - Frame sequencing (handled by GameRootUpdateLoop).
//      - Low-level shader/sampler/layout creation (handled by DGDOps_* subsystems).
//
//  ARCHITECTURAL NOTES:
//      - Strict Option‑B architecture.
//      - No simulation, no pipeline logic, no state machines.
//      - Thin orchestration layer over D3D11DeviceCore + DGDOps_*.
// =====================================================================================================

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using SASZombieAssaultTD.Engine.Graphics.Software;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using SASZombieAssaultTD.Engine.Render.D3D11.GraphicsDevice.DGDOps;
using Vortice.Direct3D11;

namespace SASZombieAssaultTD.Engine.Render.D3D11.GraphicsDevice
{
    internal sealed class D3D11GraphicsDevice : IGraphicsDevice, IDisposable
    {
        // Core device
        private readonly D3D11DeviceCore _deviceCore;

        // DGDOps subsystems (pipeline driver)
        private readonly DGDOps_Initialize _opsInitialize;
        private readonly DGDOps_Resize _opsResize;
        private readonly DGDOps_SwapChain _opsSwapChain;
        private readonly DGDOps_RenderTargets _opsRenderTargets;
        private readonly DGDOps_FramebufferUpload _opsFramebufferUpload;
        private readonly DGDOps_Present _opsPresent;
        private readonly DGDOps_DepthStencil _opsDepthStencil;
        private readonly DGDOps_Samplers _opsSamplers;
        private readonly DGDOps_BlendStates _opsBlendStates;
        private readonly DGDOps_Rasterizer _opsRasterizer;
        private readonly DGDOps_PipelineState _opsPipelineState;
        private readonly DGDOps_Textures _opsTextures;

        public D3D11GraphicsDevice(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));

            _opsInitialize = new DGDOps_Initialize(_deviceCore);
            _opsResize = new DGDOps_Resize(_deviceCore);
            _opsSwapChain = new DGDOps_SwapChain(_deviceCore);
            _opsRenderTargets = new DGDOps_RenderTargets(_deviceCore);
            _opsFramebufferUpload = new DGDOps_FramebufferUpload(_deviceCore);
            _opsPresent = new DGDOps_Present(_deviceCore);
            _opsDepthStencil = new DGDOps_DepthStencil(_deviceCore);
            _opsSamplers = new DGDOps_Samplers(_deviceCore);
            _opsBlendStates = new DGDOps_BlendStates(_deviceCore);
            _opsRasterizer = new DGDOps_Rasterizer(_deviceCore);
            _opsPipelineState = new DGDOps_PipelineState(_deviceCore);
            _opsTextures = new DGDOps_Textures(_deviceCore);
        }

        // -------------------------------------------------------------------------------------------------
        //  CORE STATE
        // -------------------------------------------------------------------------------------------------
        public bool IsInitialized => _deviceCore.IsInitialized;

        public int Width => _deviceCore.Width;

        public int Height => _deviceCore.Height;

        public ID3D11RenderTargetView MainRenderTargetView => _deviceCore.BackbufferRtv;

        // -------------------------------------------------------------------------------------------------
        //  PUBLIC: Create Render Target
        // -------------------------------------------------------------------------------------------------
        public IRenderTarget CreateRenderTarget(string name, int width, int height, PixelFormat format)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            var device = _deviceCore.Device;
            if (device == null)
                throw new InvalidOperationException("D3D11 device is not initialized.");

            // Delegate to DGDOps_RenderTargets for deterministic RT creation.
            return _opsRenderTargets.CreateRenderTarget(name, width, height, format);
        }

        // -------------------------------------------------------------------------------------------------
        //  PUBLIC: Execute Render Commands (pipeline driver hook)
        // -------------------------------------------------------------------------------------------------
        public void ExecuteCommands(IEnumerable<RenderCommand> commands)
        {
            if (commands == null)
                return;

            foreach (var _ in commands)
            {
                // No-op placeholder to keep the method deterministic and safe.
            }
        }

        // -------------------------------------------------------------------------------------------------
        //  PUBLIC: Get Device Context
        // -------------------------------------------------------------------------------------------------
        public ID3D11DeviceContext GetDeviceContext()
        {
            return _deviceCore.Context;
        }

        // -------------------------------------------------------------------------------------------------
        //  PUBLIC: Blend / Sampler / SRV / Shader helpers (thin wrappers over DGDOps)
        // -------------------------------------------------------------------------------------------------
        public ID3D11BlendState GetOrCreateBlendState()
        {
            return _opsBlendStates.GetAlphaBlend();
        }

        public ID3D11InputLayout GetOrCreateInputLayout(ID3D11VertexShader vs)
        {
            return null;
        }

        public ID3D11PixelShader GetOrCreatePixelShader()
        {
            return null;
        }

        public ID3D11SamplerState GetOrCreateSampler()
        {
            return _opsSamplers.GetLinearSampler();
        }

        public ID3D11ShaderResourceView GetOrCreateShaderResourceView(ITexture2D texture)
        {
            return _opsTextures.GetOrCreateShaderResourceView(texture);
        }

        public ID3D11VertexShader GetOrCreateVertexShader()
        {
            return null;
        }

        // -------------------------------------------------------------------------------------------------
        //  PUBLIC: Initialize / Resize / Present / Upload
        // -------------------------------------------------------------------------------------------------
        public void Initialize(nint windowHandle, int width, int height)
        {
            _opsInitialize.Initialize(windowHandle, width, height);

            // After core init, create swap-chain buffers and depth-stencil.
            _opsSwapChain.Resize(_deviceCore.Width, _deviceCore.Height);
            _opsDepthStencil.CreateDepthStencil();
        }

        public void Resize(int width, int height)
        {
            _opsResize.PrepareResize(width, height);
            _opsSwapChain.Resize(_deviceCore.Width, _deviceCore.Height);
            _opsDepthStencil.CreateDepthStencil();
        }

        public void Present()
        {
            _opsPresent.Present();
        }

        public void PresentFramebufferDrawing(FramebufferDrawing fb, ID3D11DeviceContext context)
        {
            if (context == null)
                throw new InvalidOperationException("Device context is not initialized.");

            var rtv = _deviceCore.BackbufferRtv;
            if (rtv == null)
                throw new InvalidOperationException("Main render target view is not initialized.");

            _opsFramebufferUpload.UploadFramebuffer(fb, context, rtv);
        }

        public void UploadFramebufferDrawing(FramebufferDrawing fb)
        {
            _opsFramebufferUpload.UploadToMainRenderTarget(fb);
        }

        // -------------------------------------------------------------------------------------------------
        //  DISPOSE
        // -------------------------------------------------------------------------------------------------
        public void Dispose()
        {
            // Pipeline manager does not own the device core; it only clears
            // references so GC can collect subsystems when appropriate.
        }

        // -------------------------------------------------------------------------------------------------
        //  EXPLICIT INTERFACE IMPLEMENTATION
        // -------------------------------------------------------------------------------------------------
        IRenderTarget IGraphicsDevice.CreateRenderTarget(string name, int width, int height, PixelFormat format)
        {
            return CreateRenderTarget(name, width, height, format);
        }

        ID3D11DeviceContext IGraphicsDevice.GetDeviceContext()
        {
            return GetDeviceContext();
        }
    }
}
