// ====================================================================================================
//  FILE: D3D11UIRenderBackendCore.cs
//  PATH: Engine/Rendering/ 
//  PROGRAM: D3D11UIRenderBackendCore.cs
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
File Path: Engine\Rendering\D3D11\D3D11UIRenderBackendCore.cs
Purpose: Rendering system for D3D11 graphics, sprites, text, and visual effects.
Features: Render command queuing, sprite batching, D3D11 presentation, and zombie rendering.
*/

//File:    D3D11UIRenderBackendCore.cs
//Purpose: Core implementation of the D3D11 UI render backend.
//

using System;
using System.Collections.Generic;
//
using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Rendering.D3D11
{
    ///<summary>
    ///Core D3D11 UI render backend implementation.
    ///</summary>
    public class D3D11UIRenderBackendCore : IDisposable
    {
        private readonly Renderer _renderer;
        private int _drawCalls;
        private int _stateChanges;
        private int _framesRendered;

        public D3D11UIRenderBackendCore(Renderer renderer)
        {
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        }

        public void ExecuteCommands(IEnumerable<IRenderCommand> commands)
        {
            if (commands == null)
                return;

            foreach (var command in commands)
            {
                command?.Execute(_renderer);
                _drawCalls++;
            }
        }

        public void Present()
        {
            _framesRendered++;
        }

        public (int DrawCalls, int StateChanges, int FramesRendered) GetStats()
        {
            return (_drawCalls, _stateChanges, _framesRendered);
        }

        public void ResetStats()
        {
            _drawCalls = 0;
            _stateChanges = 0;
            _framesRendered = 0;
        }

        public void Dispose()
        {
            DLogger.Log(
                LogSubsystems.D3D11,
                LogLevel.Debug,
                "PassThru",
                "D3D11UIRenderBackendCore.Dispose() invoked; no GPU resources owned by this component.");



        }


    }
}
