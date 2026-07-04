/*
File:    Initialization.cs
Path:    Engine/GameRoot/Initialization.cs
Purpose: P11-09-01 - Handles all startup and bootstrapping logic.
         Prepares every subsystem, loads configuration, and sets engine
         into a ready state.

Role:     Engine startup and bootstrapping specialist.
         - Multi-phase system initialization
         - Configuration loading
         - Dependency injection setup
         - Error handling and recovery
         - Full diagnostics instrumentation

Notes:    Contains all initialization logic extracted from GameRoot.
         Follows the modern async initialization order.
         Diagnostics instrumentation added for full trace visibility.
*/

using System;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine
{
    public partial class GameRoot
    {
        // =============================================================================================
        //  ENGINE INITIALIZATION
        // =============================================================================================
        private void PerformInitialization()
        {
            Diagnostics_Entry("PerformInitialization");

            DLogger.Log(
                LogSubsystems.GameRoot,
                LogLevel.Info,
                "Initialization",
                "=== ENGINE INITIALIZATION START ===");

            try
            {
                // -------------------------------------------------------------------------------------
                // PHASE 1: SYSTEM MANAGER
                // -------------------------------------------------------------------------------------
                Diagnostics_Write("Initializing SystemManager...");

                DLogger.Log(
                    LogSubsystems.GameRoot,
                    LogLevel.Info,
                    "SystemManager",
                    "Entering SystemManager.Initialize(); if a NotImplementedException occurs after this, " +
                    "the failure originates inside SystemManager.Initialize().");

                _systemManager.Initialize();

                DLogger.Log(
                    LogSubsystems.GameRoot,
                    LogLevel.Info,
                    "SystemManager",
                    "SystemManager initialized");

                // -------------------------------------------------------------------------------------
                // PHASE 2: UPDATE MANAGER
                // -------------------------------------------------------------------------------------
                Diagnostics_Write("Initializing UpdateManager...");

                _updateManager.Initialize();

                DLogger.Log(
                    LogSubsystems.GameRoot,
                    LogLevel.Info,
                    "UpdateManager",
                    "UpdateManager initialized");

                // -------------------------------------------------------------------------------------
                // PHASE 3: RENDER MANAGER
                // -------------------------------------------------------------------------------------
                Diagnostics_Write("Initializing RenderManager...");

                _renderManager.Initialize();

                DLogger.Log(
                    LogSubsystems.GameRoot,
                    LogLevel.Info,
                    "RenderManager",
                    "RenderManager initialized");

                // -------------------------------------------------------------------------------------
                // PHASE 4: INPUT ROUTER
                // -------------------------------------------------------------------------------------
                Diagnostics_Write("InputManager ready");

                DLogger.Log(
                    LogSubsystems.GameRoot,
                    LogLevel.Info,
                    "InputManager",
                    "InputManager ready");

                // -------------------------------------------------------------------------------------
                // PHASE 5: GAME STATE MACHINE (ASYNC)
                // -------------------------------------------------------------------------------------
                Diagnostics_Write("Initializing GameStateMachine (async)...");

                var asyncResult = _stateMachine.InitializeAsync(null);

                if (asyncResult is Task task)
                    task.GetAwaiter().GetResult();
                else if (asyncResult is ValueTask valueTask)
                    valueTask.GetAwaiter().GetResult();
                else
                    Diagnostics_Write("InitializeAsync returned non-awaitable type — treating as synchronous");

                DLogger.Log(
                    LogSubsystems.GameRoot,
                    LogLevel.Info,
                    "GameStateMachine",
                    "GameStateMachine initialized");

                // -------------------------------------------------------------------------------------
                // PHASE 6: RENDER CONTEXT
                // -------------------------------------------------------------------------------------
                Diagnostics_Write("Initializing RenderContext...");

                _renderContext.Initialize();

                DLogger.Log(
                    LogSubsystems.GameRoot,
                    LogLevel.Info,
                    "RenderContext",
                    "RenderContext initialized");

                // -------------------------------------------------------------------------------------
                // COMPLETE
                // -------------------------------------------------------------------------------------
                DLogger.Log(
                    LogSubsystems.GameRoot,
                    LogLevel.Info,
                    "Initialization",
                    "=== ENGINE INITIALIZATION COMPLETE ===");
            }
            catch (Exception ex)
            {
                Diagnostics_Exception(ex, "PerformInitialization");

                DLogger.Log(
                    LogSubsystems.GameRoot,
                    LogLevel.Error,
                    "Initialization",
                    $"Engine initialization failure: {ex.Message}");

                throw;
            }
            finally
            {
                Diagnostics_Exit("PerformInitialization");
            }
        }

        // =============================================================================================
        //  ENGINE SHUTDOWN
        // =============================================================================================
        private void PerformShutdown()
        {
            Diagnostics_Entry("PerformShutdown");

            DLogger.Log(
                LogSubsystems.GameRoot,
                LogLevel.Info,
                "Shutdown",
                "=== ENGINE SHUTDOWN START ===");

            try
            {
                Diagnostics_Write("Shutting down GameStateMachine...");
                _stateMachine.Shutdown();
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Info, "GameStateMachine", "GameStateMachine shutdown");

                Diagnostics_Write("Shutting down InputManager (no-op)");
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Info, "InputManager", "InputManager shutdown");

                Diagnostics_Write("Shutting down RenderManager...");
                _renderManager.Shutdown();
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Info, "RenderManager", "RenderManager shutdown");

                Diagnostics_Write("Shutting down UpdateManager...");
                _updateManager.Shutdown();
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Info, "UpdateManager", "UpdateManager shutdown");

                Diagnostics_Write("Shutting down SystemManager...");
                _systemManager.Shutdown();
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Info, "SystemManager", "SystemManager shutdown");

                Diagnostics_Write("Shutting down RenderContext...");
                _renderContext.Shutdown();
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Info, "RenderContext", "RenderContext shutdown");

                DLogger.Log(
                    LogSubsystems.GameRoot,
                    LogLevel.Info,
                    "Shutdown",
                    "=== ENGINE SHUTDOWN COMPLETE ===");
            }
            catch (Exception ex)
            {
                Diagnostics_Exception(ex, "PerformShutdown");

                DLogger.Log(
                    LogSubsystems.GameRoot,
                    LogLevel.Error,
                    "Shutdown",
                    $"Engine shutdown failure: {ex.Message}");

                throw;
            }
            finally
            {
                Diagnostics_Exit("PerformShutdown");
            }
        }

        // =============================================================================================
        //  DIAGNOSTIC HELPERS (STRUCTURED)
        // =============================================================================================
        private void Diagnostics_Entry(string scope) =>
            DLogger.Log(LogSubsystems.GameRoot, LogLevel.Debug, "Lifecycle", $"[ENTER] {scope}");

        private void Diagnostics_Exit(string scope) =>
            DLogger.Log(LogSubsystems.GameRoot, LogLevel.Debug, "Lifecycle", $"[EXIT] {scope}");

        private void Diagnostics_Write(string message) =>
            DLogger.Log(LogSubsystems.GameRoot, LogLevel.Debug, "Initialization", message);

        private void Diagnostics_Exception(Exception ex, string scope) =>
            DLogger.Log(LogSubsystems.GameRoot, LogLevel.Error, "Initialization", $"[{scope}] {ex.Message}");
    }
}
