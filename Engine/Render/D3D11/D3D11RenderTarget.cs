// =====================================================================================================
//  FILE: D3D11RenderTarget.cs
//  PATH: Engine/Render/D3D11RenderTarget.cs
//  SUBSYSTEM: Render / Direct3D11
//
//  ROLE:
//      Represents a GPU-backed render target surface allocated from the Direct3D11 device.
//      Provides immutable width/height/format metadata and owns the underlying D3D11 texture resource.
//
//  RESPONSIBILITIES:
//      - Allocate a GPU texture to serve as a render target.
//      - Expose logical render target metadata (Name, Width, Height).
//      - Ensure proper disposal of GPU resources.
//      - Serve as a backend implementation of IRenderTarget for the rendering pipeline.
//
//  NON-RESPONSIBILITIES:
//      - Frame sequencing (GameRootUpdateLoop).
//      - CPU rendering (FramebufferDrawing).
//      - Resource pooling (D3D11ResourcePool).
//      - Shader binding or pipeline configuration.
//
//  ARCHITECTURAL NOTES:
//      - Strict Option‑B architecture: no simulation, no pipeline logic.
//      - Immutable dimensions for lifetime.
//      - GPU resource ownership is explicit and deterministic.
// =====================================================================================================

using System.Drawing.Imaging;
using SASZombieAssaultTD.Engine.Interfaces;
using Vortice.Direct3D11;

namespace SASZombieAssaultTD.Engine.Render.D3D11
{
    internal sealed class D3D11RenderTarget : IRenderTarget
    {
        private readonly string _name;
        private readonly int _width;
        private readonly int _height;
        private readonly PixelFormat _format;

        private readonly ID3D11Texture2D _texture;
        private readonly ID3D11RenderTargetView _rtv;


        public D3D11RenderTarget(string name, int width, int height, PixelFormat format, ID3D11Device device)
        {
            _name = name;
            _width = width;
            _height = height;
            _format = format;

            // Create GPU texture
            var texDesc = new Texture2DDescription
            {
                Width = (uint)width,
                Height = (uint)height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Vortice.DXGI.Format.R8G8B8A8_UNorm,
                SampleDescription = new Vortice.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CPUAccessFlags = CpuAccessFlags.None,
                MiscFlags = ResourceOptionFlags.None
            };

            _texture = device.CreateTexture2D(texDesc);
            _rtv = device.CreateRenderTargetView(_texture);
        }

        // -------------------------------------------------------------------------------------------------
        //  IRenderTarget IMPLEMENTATION
        // -------------------------------------------------------------------------------------------------

        public string Name => _name;

        public int Width => _width;

        public int Height => _height;

        // Optional metadata field (not part of IRenderTarget)
        public string Usage => "GPU RenderTarget";

        // -------------------------------------------------------------------------------------------------
        //  DISPOSAL
        // -------------------------------------------------------------------------------------------------

        public void Dispose()
        {
            _rtv?.Dispose();
            _texture?.Dispose();
        }

        // -------------------------------------------------------------------------------------------------
        //  INTERNAL ACCESSORS (OPTION‑B SAFE)
        // -------------------------------------------------------------------------------------------------

        internal ID3D11Texture2D Texture => _texture;

        internal ID3D11RenderTargetView RTV => _rtv;
    }

    public class GetOptionFlags
    {
    }
}
