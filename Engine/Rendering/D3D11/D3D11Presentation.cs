// ====================================================================================================
//  FILE: D3D11Presentation.cs
//  PATH: Engine/Rendering/ 
//  PROGRAM: D3D11Presentation.cs
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

/*
Program Name: SASZombieAssaultTD
File Path: Engine\Rendering\D3D11\D3D11Presentation.cs
Purpose: Rendering system for D3D11 graphics, sprites, text, and visual effects.
Features: Render command queuing, sprite batching, D3D11 presentation, and zombie rendering.
*/

//============================================================================
//File:        D3D11Presentation.cs
//Author:      BDC
//Created:     2026-05-14
//Purpose:     Handles DXGI swap-chain presentation and frame submission.
//============================================================================

//
using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SharpGen.Runtime;
using Vortice.DXGI;
namespace SASZombieAssaultTD.Engine.Rendering.D3D11
{
    ///<summary>
    ///Handles swap-chain presentation and DXGI error management.
    ///</summary>
    public sealed class D3D11Presentation : IDisposable
    {
        private readonly D3D11DeviceCore _deviceCore;

        private int _framesPresented;
        private int _vsyncMode;
        private bool _disposed;

        public int FramesPresented => _framesPresented;
        public bool VSyncEnabled => _deviceCore.VSyncEnabled;

        public D3D11Presentation(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
            _vsyncMode = deviceCore.VSyncEnabled ? 1 : 0;
        }

        ///<summary>
        ///Presents the backbuffer to the display.
        ///</summary>
        public void Present()
        {
            if (_disposed || _deviceCore.SwapChain == null)
                return;

            //Vortice expects uint for sync interval
            Result result = _deviceCore.SwapChain.Present((uint)_vsyncMode, PresentFlags.None);

            if (result.Failure)
            {
                HandleDxgiError(result);
                return;
            }

            _framesPresented++;
        }

        ///<summary>
        ///Handles DXGI device removal, reset, or other presentation failures.
        ///</summary>
        private static void HandleDxgiError(Result result)
        {
            int hr = result.Code;   //raw HRESULT

            string message = hr switch
            {
                unchecked((int)0x887A0005) => "D3D11Presentation: Device removed.", //DXGI_ERROR_DEVICE_REMOVED
                unchecked((int)0x887A0007) => "D3D11Presentation: Device reset.",   //DXGI_ERROR_DEVICE_RESET
                unchecked((int)0x887A0006) => "D3D11Presentation: Device hung.",    //DXGI_ERROR_DEVICE_HUNG

                _ => $"D3D11Presentation: Present failed (HRESULT=0x{hr:X8})."
            };

            DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "ERROR",
                message);
        }





        ///<summary>
        ///Updates vsync mode dynamically.
        ///</summary>
        public void SetVSync(bool enabled)
        {
            _vsyncMode = enabled ? 1 : 0;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
        }
    }
}
