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

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Diagnostics;
using System;
using System.Threading.Tasks;

namespace SASZombieAssaultTD.Engine
{
    public partial class GameRoot
    {
        private void PerformInitialization()
        {
            Diagnostics_Entry("PerformInitialization");
            DebugLogger.LogInfo("=== ENGINE INITIALIZATION START ===");

            try
            {
                // ------------------------------------------------------------
                // PHASE 1: SYSTEM MANAGER
                // ------------------------------------------------------------
                Diagnostics_Write("Initializing SystemManager...");

                // *** HIGH-SIGNAL CUSTOM DIAGNOSTIC BREADCRUMB ***
                DebugLogger.LogError(
                    "ENGINE INIT DIAGNOSTIC — entering SystemManager.Initialize()\n" +
                    "Subsystem: SystemManager\n" +
                    "File: SystemManager.cs\n" +
                    "Caller: GameRoot.PerformInitialization\n" +
                    $"Timestamp: {DateTime.Now:O}\n" +
                    "Details: Beginning initialization of SystemManager. If a NotImplementedException " +
                    "occurs immediately after this message, the failure originates inside " +
                    "SystemManager.Initialize()."
                );

                _systemManager.Initialize();
                Diagnostics_Write("SystemManager initialized");
                DebugLogger.LogInfo("SystemManager initialized");

                // ------------------------------------------------------------
                // PHASE 2: UPDATE MANAGER
                // ------------------------------------------------------------
                Diagnostics_Write("Initializing UpdateManager...");
                _updateManager.Initialize();
                Diagnostics_Write("UpdateManager initialized");
                DebugLogger.LogInfo("UpdateManager initialized");

                // ------------------------------------------------------------
                // PHASE 3: RENDER MANAGER
                // ------------------------------------------------------------
                Diagnostics_Write("Initializing RenderManager...");
                _renderManager.Initialize();
                Diagnostics_Write("RenderManager initialized");
                DebugLogger.LogInfo("RenderManager initialized");

                // ------------------------------------------------------------
                // PHASE 4: INPUT ROUTER
                // ------------------------------------------------------------
                Diagnostics_Write("InputManager ready");
                DebugLogger.LogInfo("InputManager ready");

                // ------------------------------------------------------------
                // PHASE 5: GAME STATE MACHINE (ASYNC)
                // ------------------------------------------------------------
                Diagnostics_Write("Initializing GameStateMachine (async)...");
                var asyncResult = _stateMachine.InitializeAsync(null);

                if (asyncResult is Task task)
                    task.GetAwaiter().GetResult();
                else if (asyncResult is ValueTask valueTask)
                    valueTask.GetAwaiter().GetResult();
                else
                    Diagnostics_Write("InitializeAsync returned non-awaitable type — treating as synchronous");

                Diagnostics_Write("GameStateMachine initialized");
                DebugLogger.LogInfo("GameStateMachine initialized");

                // ------------------------------------------------------------
                // PHASE 6: RENDER CONTEXT
                // ------------------------------------------------------------
                Diagnostics_Write("Initializing RenderContext...");
                _renderContext.Initialize();
                Diagnostics_Write("RenderContext initialized");
                DebugLogger.LogInfo("RenderContext initialized");

                DebugLogger.LogInfo("=== ENGINE INITIALIZATION COMPLETE ===");
            }
            catch (Exception ex)
            {
                Diagnostics_Exception(ex, "PerformInitialization");
                DebugLogger.Exception(ex, "Engine initialization failure");
                throw;
            }
            finally
            {
                Diagnostics_Exit("PerformInitialization");
            }
        }

        private void PerformShutdown()
        {
            Diagnostics_Entry("PerformShutdown");
            DebugLogger.LogInfo("=== ENGINE SHUTDOWN START ===");

            try
            {
                Diagnostics_Write("Shutting down GameStateMachine...");
                _stateMachine.Shutdown();
                Diagnostics_Write("GameStateMachine shutdown");
                DebugLogger.LogInfo("GameStateMachine shutdown");

                Diagnostics_Write("Shutting down InputManager (no-op)");
                DebugLogger.LogInfo("InputManager shutdown");

                Diagnostics_Write("Shutting down RenderManager...");
                _renderManager.Shutdown();
                Diagnostics_Write("RenderManager shutdown");
                DebugLogger.LogInfo("RenderManager shutdown");

                Diagnostics_Write("Shutting down UpdateManager...");
                _updateManager.Shutdown();
                Diagnostics_Write("UpdateManager shutdown");
                DebugLogger.LogInfo("UpdateManager shutdown");

                Diagnostics_Write("Shutting down SystemManager...");
                _systemManager.Shutdown();
                Diagnostics_Write("SystemManager shutdown");
                DebugLogger.LogInfo("SystemManager shutdown");

                Diagnostics_Write("Shutting down RenderContext...");
                _renderContext.Shutdown();
                Diagnostics_Write("RenderContext shutdown");
                DebugLogger.LogInfo("RenderContext shutdown");

                DebugLogger.LogInfo("=== ENGINE SHUTDOWN COMPLETE ===");
            }
            catch (Exception ex)
            {
                Diagnostics_Exception(ex, "PerformShutdown");
                DebugLogger.Exception(ex, "Engine shutdown failure");
                throw;
            }
            finally
            {
                Diagnostics_Exit("PerformShutdown");
            }
        }

        private void Diagnostics_Entry(string scope) =>
            DebugLogger.LogInfo($"[ENTER] {scope}");

        private void Diagnostics_Exit(string scope) =>
            DebugLogger.LogInfo($"[EXIT] {scope}");

        private void Diagnostics_Write(string message) =>
            DebugLogger.LogInfo(message);

        private void Diagnostics_Exception(Exception ex, string scope) =>
            DebugLogger.Exception(ex, scope);
    }
}
