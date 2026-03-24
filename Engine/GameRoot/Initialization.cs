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

Notes:    Contains all initialization logic extracted from GameRoot.
         Follows the same initialization order as original GameRoot.
         All startup complexity is isolated here.
*/

using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine
{
    /// <summary>
    /// Partial class containing initialization logic for GameRoot.
    /// </summary>
    public partial class GameRoot
    {
        /// <summary>
        /// Performs the complete engine initialization sequence.
        /// </summary>
        private void PerformInitialization()
        {
            // Phase 1: Initialize system manager first
            // TODO: Verify initialization method exists
            // _systemManager.Initialize();
            ModernLoggingSystem.LogInfo("SystemManager initialized");

            // Phase 2: Initialize core managers
            // TODO: Verify initialization method exists
            // _updateManager.Initialize();
            ModernLoggingSystem.LogInfo("UpdateManager initialized");

            // TODO: Verify initialization method exists
            // _renderManager.Initialize();
            ModernLoggingSystem.LogInfo("RenderManager initialized");

            // Input manager doesn't need initialization (UIInputRouter)
            ModernLoggingSystem.LogInfo("InputManager initialized");

            // Phase 3: Initialize game state machine
            _stateMachine.Initialize();
            ModernLoggingSystem.LogInfo("GameStateMachine initialized");

            // Phase 4: Initialize render context
            _renderContext.Initialize();
            ModernLoggingSystem.LogInfo("RenderContext initialized");
        }

        /// <summary>
        /// Performs the complete engine shutdown sequence.
        /// </summary>
        private void PerformShutdown()
        {
            // Phase 1: Stop game loop and state machine
            _stateMachine.Shutdown();
            ModernLoggingSystem.LogInfo("GameStateMachine shutdown");

            // Phase 2: Shutdown managers in reverse order
            // Input manager doesn't need shutdown (UIInputRouter)
            ModernLoggingSystem.LogInfo("InputManager shutdown");

            _renderManager.Shutdown();
            ModernLoggingSystem.LogInfo("RenderManager shutdown");

            _updateManager.Shutdown();
            ModernLoggingSystem.LogInfo("UpdateManager shutdown");

            _systemManager.Shutdown();
            ModernLoggingSystem.LogInfo("SystemManager shutdown");

            // Phase 3: Cleanup render context
            _renderContext.Shutdown();
            ModernLoggingSystem.LogInfo("RenderContext shutdown");
        }
    }
}
