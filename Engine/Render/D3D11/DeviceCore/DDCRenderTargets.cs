// =====================================================================================================
//  FILE: DDC_RenderTargets.cs
//  PATH: Engine/Rendering/D3D11
//  SUBSYSTEM: D3D11 Backend – Render Target Program
//
//  ROLE:
//      Provides deterministic render-target binding and clearing logic.
//      Serves as the standalone render-target subsystem used by Core_Draw, Core_Presentation,
//      and D3D11Manager.
//
//  RESPONSIBILITIES:
//      Bind the render target view (RTV) to the output merger.
//      Configure viewport dimensions.
//      Clear the render target to a specified color.
//
//  NON-RESPONSIBILITIES:
//      Creating RTV or backbuffer resources.
//      Creating swap chain or resizing buffers.
//      Presenting the swap chain.
//      Managing device or context lifetime.
//      Creating shaders, samplers, or pipelines.
//
//  ARCHITECTURAL NOTES:
//      Standalone program; no partials.
//      All GPU objects are injected explicitly.
//      No hidden fields or cross-file state.
//      Pure render-target logic only.
// =====================================================================================================

using Vortice.Direct3D11;
using Vortice.Mathematics;

namespace SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore
{
    public sealed class CoreRenderTargets
    {
        private readonly ID3D11DeviceContext _context;
        private readonly ID3D11RenderTargetView _rtv;
        private readonly int _width;
        private readonly int _height;

        public CoreRenderTargets(
            ID3D11DeviceContext context,
            ID3D11RenderTargetView rtv,
            int width,
            int height)
        {
            _context = context;
            _rtv = rtv;
            _width = width;
            _height = height;
        }

        public void Bind()
        {
            _context.OMSetRenderTargets(_rtv);

            var viewport = new Viewport(
                0,
                0,
                _width,
                _height,
                0.0f,
                1.0f);

            _context.RSSetViewport(viewport);
        }

        public void Clear(float r, float g, float b, float a)
        {
            _context.ClearRenderTargetView(_rtv, new Color4(r, g, b, a));
        }
    }
}
