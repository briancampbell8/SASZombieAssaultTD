//
// * File:    RendererSelector.cs
// * Path:    Engine/Rendering/RendererSelector.cs
// * Purpose: Renderer backend selector - switches between D3D11 and BGFX.
// *
// * Role:    - Factory for creating the appropriate graphics device
// *          - Allows side-by-side testing of renderers
//

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Rendering.D3D11;
using SASZombieAssaultTD.Engine.UI.Rendering;
using System;
using System.Collections.Generic;
using Vortice.Direct3D11;

namespace SASZombieAssaultTD.Engine.Rendering
{
    /// <summary>
    /// Renderer backend selector factory.
    /// Creates the appropriate graphics device based on backend selection.
    /// </summary>
    public static class RendererSelector
    {
        public static IGraphicsDevice Create(RendererBackend backend)
        {
            switch (backend)
            {
                case RendererBackend.D3D11:
                    return new D3D11GraphicsDevice(IntPtr.Zero, 800, 600);

                default:
                    NI.Hit();
                    return NI.Hit<IGraphicsDevice>();
            }
        }
    }

    internal class D3D11GraphicsDevice : IGraphicsDevice
    {
        public D3D11GraphicsDevice(nint zero, int v1, int v2)
        {
        }

        public bool IsInitialized => NI.Hit<bool>();

        public int Width => NI.Hit<int>();

        public int Height => NI.Hit<int>();

        public void ApplyClear(BackendCommand op)
        {
            NI.Hit();
        }

        public void ApplyDrawQuad(ID3D11DeviceContext context, BackendCommand op)
        {
            NI.Hit();
        }

        public void ApplyDrawText(BackendCommand op)
        {
            NI.Hit();
        }

        public void ApplySetRenderTarget(BackendCommand op)
        {
            NI.Hit();
        }

        public void ApplySetScissor(BackendCommand op)
        {
            NI.Hit();
        }

        public IRenderTarget CreateRenderTarget(string name, int width, int height, PixelFormat format)
        {
            return NI.Hit<IRenderTarget>();
        }

        public ID3D11RenderTargetView CreateRenderTarget(object x, object y, object color)
        {
            return NI.Hit<ID3D11RenderTargetView>();
        }

        public void Dispose()
        {
            NI.Hit();
        }

        public void ExecuteCommands(IEnumerable<RenderCommand> commands)
        {
            NI.Hit();
        }

        public ID3D11BlendState GetOrCreateBlendState()
        {
            return NI.Hit<ID3D11BlendState>();
        }

        public ID3D11InputLayout GetOrCreateInputLayout(ID3D11VertexShader vs)
        {
            return NI.Hit<ID3D11InputLayout>();
        }

        public ID3D11PixelShader GetOrCreatePixelShader()
        {
            return NI.Hit<ID3D11PixelShader>();
        }

        public ID3D11SamplerState GetOrCreateSampler()
        {
            return NI.Hit<ID3D11SamplerState>();
        }

        public ID3D11ShaderResourceView GetOrCreateShaderResourceView(ITexture2D texture)
        {
            return NI.Hit<ID3D11ShaderResourceView>();
        }

        public ID3D11VertexShader GetOrCreateVertexShader()
        {
            return NI.Hit<ID3D11VertexShader>();
        }

        public ID3D11DeviceContext Get_context1()
        {
            return NI.Hit<ID3D11DeviceContext>();
        }

        public void Initialize(nint windowHandle, int width, int height)
        {
            NI.Hit();
        }

        public void Present()
        {
            NI.Hit();
        }

        public void PresentFramebuffer(Framebuffer fb, ID3D11DeviceContext context)
        {
            NI.Hit();
        }

        public void Resize(int width, int height)
        {
            NI.Hit();
        }

        public void UploadFramebuffer(Framebuffer fb)
        {
            NI.Hit();
        }
    }
}
