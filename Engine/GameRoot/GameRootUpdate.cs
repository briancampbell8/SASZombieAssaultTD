// =====================================================================================================
//  FILE: GameRootUpdate.cs
//  PATH: Engine/Platform/GameRootUpdate.cs
//  SUBSYSTEM: Platform Abstraction Layer
//
//  ROLE:
//      Encapsulates deterministic update-phase logic for the engine host. This class is responsible for
//      coordinating input routing, advancing the active state machine, and dispatching update ticks to
//      all registered engine systems. GameRootMain delegates its update responsibilities to this class.
//
//  RESPONSIBILITIES:
//      - Process routed UI/game input events through UIInputRouter.
//      - Advance the active StateMachine deterministically.
//      - Dispatch update ticks to all registered engine systems via UpdateManager.
//      - Maintain strict, reproducible update sequencing.
//      - Log update-phase exceptions and propagate failures.
//
//  NON-RESPONSIBILITIES:
//      - Rendering logic (handled by GameRootRender / RenderManager).
//      - Asset management or resource loading.
//      - Window or device management.
//      - GameRoot lifecycle orchestration (owned by GameRootMain).
//
//  ARCHITECTURAL NOTES:
//      - Replaces the legacy partial-method update implementation.
//      - GameRootMain composes and invokes this class directly.
//      - Strict Option B architecture: concrete subsystem types, no interface indirection.
//      - All update-phase exceptions are logged and rethrown for engine-level crash handling.
// =====================================================================================================


using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Input;
using SASZombieAssaultTD.Engine.State;
using SASZombieAssaultTD.Engine.Systems;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.GameRoot
{
    /// <summary>
    /// Encapsulates deterministic update-phase logic for the engine host.
    /// </summary>
    internal sealed class GameRootUpdate
    {
        private readonly UIInputRouter _inputRouter;
        private readonly StateMachine _stateMachine;
        private readonly UpdateManager _updateManager;

        public GameRootUpdate(
            UIInputRouter inputRouter,
            StateMachine stateMachine,
            UpdateManager updateManager)
        {
            _inputRouter = inputRouter ?? throw new ArgumentNullException(nameof(inputRouter));
            _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
            _updateManager = updateManager ?? throw new ArgumentNullException(nameof(updateManager));
        }

        /// <summary>
        /// Executes a single deterministic update tick.
        /// </summary>
        public void Execute(float deltaTime)
        {
            try
            {
                // 1. Input routing
                _inputRouter.Update(deltaTime);

                // 2. State machine advancement
                _stateMachine.Update(deltaTime);

                // 3. System-level update dispatch
                _updateManager.Update(deltaTime);
            }
            catch (Exception ex)
            {
                DLogger.Log(
                    LogSubsystems.GameRoot,
                    LogEnums.LogLevel.Error,
                    $"Update failure: {ex.Message}",
                    "GameRootUpdate.Execute");

                throw;
            }
        }
    }
}
