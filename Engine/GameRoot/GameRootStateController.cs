// =====================================================================================================
// FILE: GameRootShutdown.cs
// PATH: Engine/Platform/GameRootShutdown.cs
// SUBSYSTEM: Platform Abstraction Layer
//
// ROLE: Encapsulates deterministic shutdown-phase logic for the engine host. This class is
// responsible for coordinating the orderly teardown of all engine subsystems, ensuring that
// resources are released, states are finalized, and diagnostics are logged.
//
// RESPONSIBILITIES:
// - Shutdown the active state machine.
// - Shutdown input routing subsystems.
// - Shutdown system-level update and render managers.
// - Shutdown the system manager.
// - Shutdown the system registry.
// - Provide deterministic teardown sequencing.
// - Log shutdown diagnostics and propagate critical failures.
//
// NON-RESPONSIBILITIES:
// - Game logic teardown (handled by game-specific systems).
// - Asset unloading (handled by subsystem managers).
// - Window or device destruction (handled by platform layer).
//
// ARCHITECTURAL NOTES:
// - Strict Option B architecture: concrete subsystem types, no interface indirection.
// - All shutdown-phase exceptions are logged and rethrown for engine-level crash handling.
// - Invocation is controlled by the engine host; this class does not self-register. =====================================================================================================


using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.State;
using SASZombieAssaultTD.Engine.Systems;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.GameRoot
{
    internal sealed class GameRootStateController
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly Func<bool> _isInitialized;
        private ISystemRegistry systemRegistry;
        private SystemManager systemManager;

        public GameRootStateController(
            IGameStateMachine stateMachine,
            Func<bool> isInitialized)
        {
            _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
            _isInitialized = isInitialized ?? throw new ArgumentNullException(nameof(isInitialized));
        }

        // FIXED: Forward concrete StateMachine → interface constructor
        public GameRootStateController(StateMachine stateMachine, Func<bool> value)
            : this((IGameStateMachine)stateMachine, value)
        {
        }

        public GameRootStateController(ISystemRegistry systemRegistry, SystemManager systemManager, StateMachine stateMachine)
        {
            this.systemRegistry = systemRegistry;
            this.systemManager = systemManager;
            _stateMachine = stateMachine;
        }

        // ------------------------------------------------------------------------------------------------
        //  STATE TRANSITION HELPERS
        // ------------------------------------------------------------------------------------------------

        public void StartGame()
        {
            if (!ValidateInitialized("start game")) return;

            try
            {
                _stateMachine.StartGame();
                DLogger.Log(LogSubsystems.GameRoot, LogEnums.LogLevel.Info, "Game started.");
            }
            catch (Exception ex)
            {
                LogError("start game", ex);
            }
        }

        public void PauseGame()
        {
            if (!ValidateInitialized("pause game")) return;

            try
            {
                _stateMachine.PauseGame();
                DLogger.Log(LogSubsystems.GameRoot, LogEnums.LogLevel.Info, "Game paused.");
            }
            catch (Exception ex)
            {
                LogError("pause game", ex);
            }
        }

        public void ResumeGame()
        {
            if (!ValidateInitialized("resume game")) return;

            try
            {
                _stateMachine.ResumeGame();
                DLogger.Log(LogSubsystems.GameRoot, LogEnums.LogLevel.Info, "Game resumed.");
            }
            catch (Exception ex)
            {
                LogError("resume game", ex);
            }
        }

        public void ResetGame()
        {
            if (!ValidateInitialized("reset game")) return;

            try
            {
                _stateMachine.ResetGame();
                DLogger.Log(LogSubsystems.GameRoot, LogEnums.LogLevel.Info, "Game reset.");
            }
            catch (Exception ex)
            {
                LogError("reset game", ex);
            }
        }

        public void GameOver()
        {
            if (!ValidateInitialized("trigger game over")) return;

            try
            {
                _stateMachine.GameOver();
                DLogger.Log(LogSubsystems.GameRoot, LogEnums.LogLevel.Info, "Game over triggered.");
            }
            catch (Exception ex)
            {
                LogError("trigger game over", ex);
            }
        }

        public void NextLevel()
        {
            if (!ValidateInitialized("transition to next level")) return;

            try
            {
                _stateMachine.NextLevel();
                DLogger.Log(LogSubsystems.GameRoot, LogEnums.LogLevel.Info, "Next level loaded.");
            }
            catch (Exception ex)
            {
                LogError("transition to next level", ex);
            }
        }

        // ------------------------------------------------------------------------------------------------
        //  STATE QUERIES
        // ------------------------------------------------------------------------------------------------

        public string GetCurrentGameState()
        {
            return _stateMachine.GetCurrentState()?.ToString() ?? "Unknown";
        }

        public bool IsGamePaused() => _stateMachine.IsPaused();
        public bool IsGameRunning() => _stateMachine.IsRunning();
        public bool IsGameOver() => _stateMachine.IsGameOver();

        // ------------------------------------------------------------------------------------------------
        //  INTERNAL HELPERS
        // ------------------------------------------------------------------------------------------------

        private bool ValidateInitialized(string action)
        {

            if (_isInitialized == null)
                return true;

            if (_isInitialized()) return true;

            DLogger.Log(LogSubsystems.GameRoot, LogEnums.LogLevel.Error,
                $"Cannot {action} — engine not initialized.");
            return false;
        }

        private void LogError(string action, Exception ex)
        {
            DLogger.Log(LogSubsystems.GameRoot, LogEnums.LogLevel.Error,
                $"Failed to {action}: {ex.Message}");
            DLogger.Log(LogSubsystems.GameRoot, LogEnums.LogLevel.Error,
                ex.ToString(), $"State transition: {action}");
        }
    }
}
