// ====================================================================================================
//  FILE: D3D11DeviceCoreRTV.cs
//  PATH: Engine/Rendering/ 
//  PROGRAM: D3D11DeviceCoreRTV.cs
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

//
using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using Vortice.Direct3D11;
using Vortice.Mathematics;
namespace SASZombieAssaultTD.Engine.Rendering.D3D11
{
    public sealed partial class D3D11DeviceCore : IDisposable
    {
        //--------------------------------------------------------------------
        //Fields
        //--------------------------------------------------------------------

        private ID3D11RenderTargetView? _backbufferRTV;
        private bool _rtvInitialized;

        //--------------------------------------------------------------------
        //Public API
        //--------------------------------------------------------------------

        ///<summary>
        ///Initializes the backbuffer RTV and viewport.
        ///Call this once after the swap chain is created or resized.
        ///</summary>
        public void InitializeRenderTargets(int width, int height)
        {
            DLogger.Log(
                LogSubsystems.D3D11,
                $"D3D11DeviceCore.InitializeRenderTargets: creating RTV + viewport for {width}x{height}");

            _backbufferRTV?.Dispose();
            _backbufferRTV = null;
            _rtvInitialized = false;

            //1. Get backbuffer texture from swap chain
            using ID3D11Texture2D backbuffer = SwapChain.GetBuffer<ID3D11Texture2D>(0);

            //2. Create RTV
            _backbufferRTV = Device.CreateRenderTargetView(backbuffer);

            //3. Bind RTV
            ImmediateContext.OMSetRenderTargets(_backbufferRTV, null);

            //4. Set viewport
            var viewport = new Viewport(0, 0, width, height, 0.0f, 1.0f);
            ImmediateContext.RSSetViewport(viewport);

            _rtvInitialized = true;

            DLogger.Log(
                LogSubsystems.D3D11,
                "D3D11DeviceCore.InitializeRenderTargets: RTV + viewport initialized");
        }

        ///<summary>
        ///Clears the GPU backbuffer to the specified color.
        ///</summary>
        public void ClearRenderTarget(float r, float g, float b, float a)
        {
            if (!_rtvInitialized || _backbufferRTV is null)
            {
                DLogger.Log(
                    LogSubsystems.D3D11,
                    "D3D11DeviceCore.ClearRenderTarget: RTV not initialized, skipping clear");
                return;
            }

            var color = new Color4(r, g, b, a);

            DLogger.Log(
                LogSubsystems.D3D11,
                $"D3D11DeviceCore.ClearRenderTarget: clearing to ({r:F3}, {g:F3}, {b:F3}, {a:F3})");

            ImmediateContext.ClearRenderTargetView(_backbufferRTV, color);
        }

        //--------------------------------------------------------------------
        //Disposal
        //--------------------------------------------------------------------

        private void DisposeRtvResources()
        {
            _backbufferRTV?.Dispose();
            _backbufferRTV = null;
            _rtvInitialized = false;
        }
    }
}
