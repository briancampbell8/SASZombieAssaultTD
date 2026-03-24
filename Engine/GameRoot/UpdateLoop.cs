/*
File:    UpdateLoop.cs
Path:    Engine/GameRoot/UpdateLoop.cs
Purpose: P11-09-01 - Contains the core heartbeat of the engine.
         Runs every frame and coordinates updates across all subsystems.

Role:     Core engine update and rendering specialist.
         - Frame update coordination
         - System update chain management
         - Scene update calls
         - Timing update calls
         - Event dispatch inside update loop
         - Frame rendering coordination

Notes:    Contains the main update and render loops extracted from GameRoot.
         Maintains the same update order and timing as original.
         All frame-level logic is isolated here.
*/

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine
{
    /// <summary>
    /// Partial class containing update loop logic for GameRoot.
    /// </summary>
    public partial class GameRoot
    {
        /// <summary>
        /// Performs the update sequence for the current frame.
        /// </summary>
        /// <param name="deltaTime">Time since last frame in seconds.</param>
        private void PerformUpdate(float deltaTime)
        {
            // Update game state machine first
            _stateMachine.Update(deltaTime);

            // Update all updatable systems
            _updateManager.UpdateAll(deltaTime);

            // Input is handled by UIInputRouter automatically
            ModernLoggingSystem.LogDebug($"Frame update completed in {deltaTime:F4}s");
        }

        /// <summary>
        /// Performs the render sequence for the current frame.
        /// </summary>
        private void PerformRender()
        {
            // Render game state
            _stateMachine.Render(_renderContext);

            // Render all renderable systems
            _renderManager.RenderAll();

            ModernLoggingSystem.LogDebug("Frame render completed");
        }
    }
}
