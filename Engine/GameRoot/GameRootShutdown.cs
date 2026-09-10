// =====================================================================================================
//  FILE: GameRootShutdown.cs
//  PATH: Engine/Platform/GameRootShutdown.cs
//  SUBSYSTEM: Platform Abstraction Layer
//
//  ROLE:
//      Encapsulates deterministic shutdown-phase logic for the engine host. This class is responsible
//      for coordinating the orderly teardown of all engine subsystems, ensuring that resources are
//      released, states are finalized, and diagnostics are logged. GameRootMain delegates its shutdown
//      responsibilities to this standalone class.
//
//  RESPONSIBILITIES:
//      - Shutdown the active state machine.
//      - Shutdown input routing subsystems.
//      - Shutdown system-level update and render managers.
//      - Shutdown the system registry.
//      - Provide deterministic teardown sequencing.
//      - Log shutdown diagnostics and propagate critical failures.
//
//  NON-RESPONSIBILITIES:
//      - Game logic teardown (handled by game-specific systems).
//      - Asset unloading (handled by subsystem managers).
//      - Window or device destruction (handled by platform layer).
//
//  ARCHITECTURAL NOTES:
//      - This class replaces the former partial-method shutdown implementation.
//      - GameRootMain composes and invokes this class directly.
//      - Strict Option B architecture: concrete subsystem types, no interface indirection.
//      - All shutdown-phase exceptions are logged and rethrown for engine-level crash handling.
//
//  AUTHOR: BDC
//  CREATED: 2026-07-17
//  LAST UPDATED: 2026-07-17
// =====================================================================================================

using System;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Input;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.State;
using SASZombieAssaultTD.Engine.Systems;

namespace SASZombieAssaultTD.Engine.GameRoot
{
    /// <summary>
    /// Encapsulates deterministic shutdown-phase logic for the engine host.
    /// </summary>
    internal sealed class GameRootShutdown
    {
        private readonly StateMachine _stateMachine;
        private readonly UIInputRouter _inputRouter;
        private readonly RenderManager _renderManager;
        private readonly UpdateManager _updateManager;
        private readonly SystemManager _systemManager;
        private readonly ISystemRegistry _systemRegistry;

        public GameRootShutdown(
            StateMachine stateMachine,
            UIInputRouter inputRouter,
            RenderManager renderManager,
            UpdateManager updateManager,
            SystemManager systemManager,
            ISystemRegistry systemRegistry)
        {
            _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
            _inputRouter = inputRouter ?? throw new ArgumentNullException(nameof(inputRouter));
            _renderManager = renderManager ?? throw new ArgumentNullException(nameof(renderManager));
            _updateManager = updateManager ?? throw new ArgumentNullException(nameof(updateManager));
            _systemManager = systemManager ?? throw new ArgumentNullException(nameof(systemManager));
            _systemRegistry = systemRegistry ?? throw new ArgumentNullException(nameof(systemRegistry));
        }

        /// <summary>
        /// Executes a deterministic shutdown sequence for all engine subsystems.
        /// </summary>
        public void Execute()
        {
            DLogger.Log(LogSubsystems.GameRoot, LogEnums.LogLevel.Info, "GameRootShutdown.Execute: ENTER");

            try
            {
                // 1. Shutdown state machine
                _stateMachine.Shutdown();

                // 2. Shutdown input routing
                _inputRouter.Shutdown();

                // 3. Shutdown render manager
                _renderManager.Shutdown();

                // 4. Shutdown update manager
                _updateManager.Shutdown();

                // 5. Shutdown system manager
                _systemManager.Shutdown();

                // 6. Shutdown system registry
                _systemRegistry.Shutdown();

                DLogger.Log(LogSubsystems.GameRoot, LogEnums.LogLevel.Info, "GameRootShutdown.Execute: EXIT");
            }
            catch (Exception ex)
            {
                DLogger.Log(
                    LogSubsystems.GameRoot,
                    LogEnums.LogLevel.Error,
                    $"Shutdown failure: {ex.Message}",
                    "GameRootShutdown.Execute");

                throw;
            }
        }
    }
}
