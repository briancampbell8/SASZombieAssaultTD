// ====================================================================================================
//  FILE: D3D11DeviceCore_Draw.cs
//  PATH: Engine/Rendering/ 
//  PROGRAM: D3D11DeviceCore_Draw.cs
//  MODULE: Resource Management Framework
//  ROLE:
//      Defines the structures, loaders, and integration points responsible for discovering, validating, and providing engine resources in a deterministic manner.
//
//  RESPONSIBILITIES:
//      - Provide a unified API for loading, caching, and resolving engine resources.
//      - Enforce deterministic resource lookup and lifecycle rules.
//      - Abstract file formats, storage locations, and integration layers behind a stable interface.
//      - Ensure resource availability for all engine subsystems (Rendering, Audio, Gameplay, UI).
//
//  NON-RESPONSIBILITIES:
//      - Performing rendering or GPU upload operations.
//      - Managing gameplay logic or scene entities.
//      - Handling diagnostics, logging, or performance metrics.
//      - Encoding or authoring resource files.
//
//  ARCHITECTURAL NOTES:
//      - The Resource Management Framework acts as the central authority for all asset retrieval.
//      - Resource modules must remain pure: no side effects outside resource acquisition and validation.
//      - All resource types (textures, data files, definitions, metadata) must follow deterministic load rules.
//  ====================================================================================================

/* ====================================================================================================
 *  FILE:       D3D11DeviceCore_Draw.cs
 *  PATH:       Engine/Rendering/D3D11/
 *  SUBSYSTEM:  Rendering.D3D11
 *  ROLE:       Fullscreen-quad draw path for D3D11DeviceCore. Binds the framebuffer SRV, sets the
 *              render target and viewport, and issues the draw call against the swap chain backbuffer.
 *
 *  RESPONSIBILITIES:
 *      - Bind the swap chain render target for drawing.
 *      - Bind the framebuffer shader resource view to the pixel shader.
 *      - Configure viewport dimensions to match the current backbuffer size.
 *      - Issue the fullscreen quad draw call.
 *
 *  NON-RESPONSIBILITIES:
 *      - Creating or managing the framebuffer texture/SRV (delegated to D3D11FramebufferPresenter).
 *      - Creating the D3D11 device, context, or swap chain (delegated to D3D11DeviceCore core ctor).
 *      - Configuring shaders, input layout, vertex buffer, or sampler state
 *        (delegated to InitializeFullscreenQuadPipeline / InitializeSampler).
 *
 *  DEPENDENCIES:
 *      - D3D11DeviceCore (core device + swap chain + pipeline initialization).
 *      - D3D11FramebufferPresenter (provides the framebuffer SRV).
 *      - Vortice.Direct3D11 (render target, viewport, draw).
 *      - DebugLogger (diagnostics).
 *
 *  CALLED BY:
 *      - Platform-specific render loop after framebuffer upload.
 *
 *  CALLS INTO:
 *      - ID3D11DeviceContext.OMSetRenderTargets
 *      - ID3D11DeviceContext.RSSetViewports
 *      - ID3D11DeviceContext.PSSetShaderResources
 *      - ID3D11DeviceContext.Draw
 *      - D3D11DeviceCore.Present
 *
 *  ARCHITECTURAL NOTES:
 *      - This partial contains only draw-path logic; creation and teardown live in the core file.
 *      - Assumes InitializeRenderTargets, InitializeSampler, and InitializeFullscreenQuadPipeline
 *        have been called successfully before use.
 *      - This file is complete for draw-path responsibilities and must not be further split.
 * ==================================================================================================== */

//
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Scenes.Battlefields;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.Mathematics;
namespace SASZombieAssaultTD.Engine.Rendering.D3D11
{
    public sealed partial class D3D11DeviceCore
    {
        //These fields are expected to be created and disposed in the core partial:
        //- _renderTargetView (ID3D11RenderTargetView)
        //- _width, _height (backbuffer dimensions)
        //- _disposed (lifecycle flag)

        private ID3D11RenderTargetView? _renderTargetView;

        ///<summary>
        ///Binds the framebuffer shader resource view and renders a fullscreen quad to the swap chain
        ///backbuffer. The caller is responsible for ensuring the SRV is valid and up to date.
        ///</summary>
        ///<param name="framebufferSrv">Shader resource view exposing the framebuffer texture.</param>
        public void RenderFramebuffer(ID3D11ShaderResourceView framebufferSrv)
        {
            if (_disposed)
                return;

            if (framebufferSrv == null)
            {
                Dlogger.Log(
                    LogSubsystems.D3D11,
                    LogLevel.Debug,
                    "D3D11DeviceCore.RenderFramebuffer: framebuffer SRV is null.");
                return;
            }
            if (_renderTargetView == null)
            {
                Dlogger.Log(
                    LogSubsystems.D3D11,
                    LogLevel.Debug,
                    "D3D11DeviceCore.RenderFramebuffer: render target view is not initialized.");
                return;
            }

            //Bind the backbuffer render target.
            ImmediateContext.OMSetRenderTargets(_renderTargetView);

            //Configure viewport to match current backbuffer size.
            var viewport = new Viewport(0, 0, _width, _height, 0.0f, 1.0f);
            var viewports = new Viewport[1];
            viewports[0] = viewport;
            ImmediateContext.RSSetViewports(viewports);

            //Bind the framebuffer texture to the pixel shader.
            ImmediateContext.PSSetShaderResource(0, framebufferSrv);

            //Fullscreen quad pipeline (shaders, input layout, vertex buffer, sampler)
            //is assumed to have been set up by InitializeFullscreenQuadPipeline / InitializeSampler.

            ImmediateContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleStrip);

            //Draw a 2-triangle fullscreen quad (4 vertices in a triangle strip).
            ImmediateContext.Draw(4, 0);
        }

        ///<summary>
        ///Convenience method that renders the framebuffer and presents the swap chain in a single call.
        ///</summary>
        ///<param name="framebufferSrv">Shader resource view exposing the framebuffer texture.</param>
        public void RenderFramebufferAndPresent(ID3D11ShaderResourceView framebufferSrv)
        {
            RenderFramebuffer(framebufferSrv);
            Present();
        }
    }
}
