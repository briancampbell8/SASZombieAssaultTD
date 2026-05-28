// ============================================================================
// File:    D3D11DeviceCoreFullscreenQuad.cs
// Path:    Engine/Rendering/D3D11/D3D11DeviceCore.FullscreenQuad.cs
// Author:  BDC
// Purpose: Adds fullscreen quad pipeline scaffolding to D3D11DeviceCore.
//          This is the GPU draw path that will eventually render the CPU
//          framebuffer texture to the backbuffer.
//
//          CURRENT STATE:
//          - Safe, compiling scaffold.
//          - No real GPU calls yet.
//          - Defines the API RenderContextD3D11 will call.
//          - Prepares the engine for shader + vertex buffer creation.
//
//          FUTURE STATE:
//          - Create vertex buffer for fullscreen quad.
//          - Create pass-through vertex shader.
//          - Create texture-sampling pixel shader.
//          - Bind SRV + sampler.
//          - Issue Draw() call.
// ============================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering.D3D11
{
    public sealed partial class D3D11DeviceCore
    {
        // --------------------------------------------------------------------
        // Fields (future GPU resources)
        // --------------------------------------------------------------------

        private object _fullscreenQuadVB = null;     // Vertex buffer placeholder
        private object _fullscreenVS = null;         // Vertex shader placeholder
        private object _fullscreenPS = null;         // Pixel shader placeholder
        private object _samplerState = null;         // Sampler placeholder

        private bool _quadPipelineInitialized = false;

        // --------------------------------------------------------------------
        // Public API
        // --------------------------------------------------------------------

        /// <summary>
        /// Initializes the fullscreen quad pipeline.
        /// This must be called once before DrawFullscreenTexturedQuad().
        /// </summary>
        public void InitializeFullscreenQuadPipeline()
        {
            DebugLogger.LogInfo(
                "D3D11DeviceCore.InitializeFullscreenQuadPipeline: (placeholder) creating quad pipeline.");

            // TODO (future surgical pass):
            // 1. Create vertex buffer for fullscreen quad:
            //    [-1,-1], [1,-1], [-1,1], [1,1]
            //
            // 2. Compile/load vertex shader:
            //    float4 VS(float2 pos : POSITION) : SV_POSITION { return float4(pos, 0, 1); }
            //
            // 3. Compile/load pixel shader:
            //    float4 PS(float2 uv : TEXCOORD) : SV_TARGET { return texture.Sample(sampler, uv); }
            //
            // 4. Create sampler state.

            _quadPipelineInitialized = true;
        }

        /// <summary>
        /// Draws a fullscreen quad sampling the provided texture view.
        /// </summary>
        /// <param name="textureView">ShaderResourceView for the framebuffer texture.</param>
        public void DrawFullscreenTexturedQuad(object textureView)
        {
            if (!_quadPipelineInitialized)
            {
                DebugLogger.LogWarning(
                    "D3D11DeviceCore.DrawFullscreenTexturedQuad: pipeline not initialized. Draw skipped.");
                return;
            }

            if (textureView == null)
            {
                DebugLogger.LogWarning(
                    "D3D11DeviceCore.DrawFullscreenTexturedQuad: textureView is null. Draw skipped.");
                return;
            }

            DebugLogger.LogDebug(
                "D3D11DeviceCore.DrawFullscreenTexturedQuad: (placeholder) drawing fullscreen quad.");

            // TODO (future surgical pass):
            // 1. Bind RTV (already bound by ClearRenderTarget).
            // 2. Bind vertex buffer:
            //    _context.IASetVertexBuffers(...)
            //
            // 3. Bind shaders:
            //    _context.VSSetShader(_fullscreenVS)
            //    _context.PSSetShader(_fullscreenPS)
            //
            // 4. Bind texture + sampler:
            //    _context.PSSetShaderResources(0, textureView)
            //    _context.PSSetSamplers(0, _samplerState)
            //
            // 5. Issue draw call:
            //    _context.Draw(4, 0);
        }
    }
}
