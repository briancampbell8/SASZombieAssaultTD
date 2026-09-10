// =============================================================================================================
//  FILE: GameEngineLoop.cs
//  PATH: Engine/Core/GameEngineLoop.cs
//  SUBSYSTEM: Engine Runtime Core
//
//  ROLE:
//      Orchestrator driving updates, raw input sampling updates, and explicit frame presenter execution.
//      This class coordinates the top-level execution sequence of the core engine managers once per frame.
//
//  RESPONSIBILITIES:
//      - Coordinate the canonical tick sequence: update managers, tick state machines, and wrap render blocks.
//      - Provide a clean runtime driver execution context for the initialized subsystem graphs.
//      - Act as a protective lifecycle host for active rendering boundaries.
//
//  NON-RESPONSIBILITIES:
//      - Low-level Win32 platform message pumping or OS window loop draining.
//      - Allocating native Direct3D11 graphics context resources or swap chains directly.
//      - Defining gameplay state rules, level data serialization, or ECSEntityCore updates.
//
//  ARCHITECTURAL NOTES:
//      - This class acts as the runtime host designed to execute blocks synchronized via EngineBootstrap.
//      - Subsystem dependencies (UpdateManager, StateMachine, RenderManager) are managed internally
//        by GameRootMain to respect encapsulation modifiers and Option-B architectural boundaries.
// =============================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Platform;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;

namespace SASZombieAssaultTD.Engine.CoreSize
{
    public sealed class GameEngineLoop : IDisposable
    {
        private readonly D3D11Window _window;
        private readonly D3D11DeviceCore _deviceCore;
        private readonly GameRootMain _gameRoot;

        /// <summary>
        /// Instantiates the game loop orchestrator using the engine's real subcomponent signatures.
        /// </summary>
        public GameEngineLoop(D3D11Window window, D3D11DeviceCore deviceCore)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));

            DLogger.Log(LogSubsystems.ResourcesPipeline, "[GameEngineLoop] Initializing runtime loop boundaries.");

            // Construct and bind systems deterministically via composition root
            _gameRoot = EngineBootstrap.CreateGameRootMain(_window, _deviceCore);
        }

        /// <summary>
        /// Active execution frame pump driving gameplay state tracking routines.
        /// </summary>
        public void Run()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "[GameEngineLoop] Initializing GameRootMain subsystem infrastructure.");
            _gameRoot.Initialize();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "[GameEngineLoop] Entering core simulation frame pump via GameRootMain.");

            // Relinquish control to GameRootMain's authoritative fixed-timestep loop execution
            _gameRoot.Run();
        }

        public void Dispose()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "[GameEngineLoop] Tearing down loop allocation handles.");
            _gameRoot?.Dispose();
        }
    }
}
