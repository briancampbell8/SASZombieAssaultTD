/*
File:    State.cs
Path:    Engine/GameLoop/State.cs
Purpose: P11-09-01 - Contains all state management for GameLoop.
         Controls game loop state and transitions.

Role:     Game loop state specialist.
         - GameLoopState enum
         - Running/stopped flags
         - Shutdown transitions
         - State management logic
         - Thread-safe state operations

Notes:    Contains all state logic extracted from GameLoop.
         Ensures clean state management and thread-safe operations.
         Provides deterministic state transitions.
*/

using System;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Systems
{
    /// <summary>
    /// Partial class containing state management logic for GameLoop.
    /// </summary>
    public partial class GameLoop
    {
        /// <summary>
        /// Stops the game loop gracefully.
        /// </summary>
        public void PerformStop()
        {
            _isRunning = false;
            Engine.Diagnostics.DebugLogger.LogInfo("Game loop stopped gracefully");
        }

        /// <summary>
        /// Performs graceful shutdown with state cleanup.
        /// </summary>
        private void PerformGracefulShutdown()
        {
            try
            {
                Engine.Diagnostics.DebugLogger.LogInfo("Starting graceful shutdown...");

                // Stop the game loop first
                _isRunning = false;

                // Give systems time to cleanup
                System.Threading.Thread.Sleep(100);

                // Log final statistics
                LogFinalStatistics();

                Engine.Diagnostics.DebugLogger.LogInfo("Graceful shutdown completed");
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"Graceful shutdown failed: {ex.Message}");
                Engine.Diagnostics.DebugLogger.Exception(ex, "Graceful shutdown");
            }
        }

        /// <summary>
        /// Performs emergency shutdown due to critical error.
        /// </summary>
        /// <param name="error">The critical error that triggered shutdown.</param>
        private void PerformEmergencyShutdown(Exception error)
        {
            try
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"Emergency shutdown triggered by critical error: {error.Message}");
                Engine.Diagnostics.DebugLogger.Exception(error, "Emergency shutdown");

                // Force stop immediately
                _isRunning = false;
                _isInitialized = false;

                // Log emergency shutdown
                LogEmergencyShutdown(error);

                Engine.Diagnostics.DebugLogger.LogInfo("Emergency shutdown completed");
            }
            catch (Exception shutdownEx)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"Emergency shutdown failed: {shutdownEx.Message}");
                Engine.Diagnostics.DebugLogger.Exception(shutdownEx, "Emergency shutdown");
            }
        }

        /// <summary>
        /// Validates state transitions.
        /// </summary>
        /// <param name="fromState">The current state.</param>
        /// <param name="toState">The desired state.</param>
        /// <returns>True if the transition is valid.</returns>
        private bool IsValidStateTransition(GameLoopState fromState, GameLoopState toState)
        {
            return fromState switch
            {
                GameLoopState.Uninitialized => toState == GameLoopState.Initializing,
                GameLoopState.Initializing => toState == GameLoopState.Running || toState == GameLoopState.Error,
                GameLoopState.Running => toState == GameLoopState.Stopped || toState == GameLoopState.Error,
                GameLoopState.Stopped => toState == GameLoopState.Running || toState == GameLoopState.ShuttingDown,
                GameLoopState.ShuttingDown => toState == GameLoopState.Uninitialized,
                GameLoopState.Error => toState == GameLoopState.ShuttingDown || toState == GameLoopState.Uninitialized,
                _ => false
            };
        }

        /// <summary>
        /// Logs final statistics before shutdown.
        /// </summary>
        private void LogFinalStatistics()
        {
            Engine.Diagnostics.DebugLogger.LogInfo($"Final statistics - Frames: {_frameCount}, Avg FPS: {FramesPerSecond:F2}, Errors: {_diagnostics.ErrorCount}");
        }

        /// <summary>
        /// Logs emergency shutdown information.
        /// </summary>
        /// <param name="error">The critical error.</param>
        private void LogEmergencyShutdown(Exception error)
        {
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"Emergency shutdown - Error: {error.GetType().Name}, Message: {error.Message}");
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"Context - Frame: {_frameCount}, FPS: {FramesPerSecond:F2}, Memory: {GC.GetTotalMemory(false) / 1024 / 1024}MB");
        }

        /// <summary>
        /// Gets detailed state information.
        /// </summary>
        /// <returns>State information.</returns>
        public StateInfo GetStateInfo()
        {
            return new StateInfo
            {
                CurrentState = State,
                IsInitialized = _isInitialized,
                IsRunning = _isRunning,
                FrameCount = _frameCount,
                Uptime = DateTime.Now - (_lastFrameTime - TimeSpan.FromMilliseconds(_frameCount * _averageFrameTime * 1000)),
                LastStateChange = DateTime.Now // Would be tracked in real implementation
            };
        }

        /// <summary>
        /// Forces a state transition (for testing/recovery).
        /// </summary>
        /// <param name="newState">The new state to force.</param>
        /// <returns>True if the transition was successful.</returns>
        public bool ForceStateTransition(GameLoopState newState)
        {
            lock (_stateLock)
            {
                var oldState = State;

                if (!IsValidStateTransition(oldState, newState))
                {
                    Engine.Diagnostics.DebugLogger.LogWarning($"Invalid state transition: {oldState} -> {newState}");
                    return false;
                }

                Engine.Diagnostics.DebugLogger.LogInfo($"Forcing state transition: {oldState} -> {newState}");

                // Apply state changes
                switch (newState)
                {
                    case GameLoopState.Running:
                        _isRunning = true;
                        break;
                    case GameLoopState.Stopped:
                        _isRunning = false;
                        break;
                    case GameLoopState.ShuttingDown:
                        PerformGracefulShutdown();
                        break;
                    case GameLoopState.Uninitialized:
                        _isInitialized = false;
                        _isRunning = false;
                        break;
                }

                return true;
            }
        }
    }

    /// <summary>
    /// Enumeration of game loop states.
    /// </summary>
    public enum GameLoopState
    {
        /// <summary>The game loop has not been initialized.</summary>
        Uninitialized,

        /// <summary>The game loop is currently initializing.</summary>
        Initializing,

        /// <summary>The game loop is running and processing frames.</summary>
        Running,

        /// <summary>The game loop is stopped but can be resumed.</summary>
        Stopped,

        /// <summary>The game loop is shutting down.</summary>
        ShuttingDown,

        /// <summary>The game loop is in an error state.</summary>
        Error
    }

    /// <summary>
    /// State information for the game loop.
    /// </summary>
    public class StateInfo
    {
        public GameLoopState CurrentState { get; set; }
        public bool IsInitialized { get; set; }
        public bool IsRunning { get; set; }
        public int FrameCount { get; set; }
        public TimeSpan Uptime { get; set; }
        public DateTime LastStateChange { get; set; }
    }
}
