/*
File:    PauseScene.cs
Author:  BDC
Created: 2026-02-07
Purpose: Pause overlay scene. Handles pause menu initialization, input updates, and rendering.
Notes:   Inherits from Scene. Provides debug breakpoints and placeholder UI behavior.
*/

using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.UI.Input;
using SASZombieAssaultTD.Engine.Systems;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Input;
using System;
using System.Diagnostics;
using ModernLoggingSystem = SASZombieAssaultTD.Engine.Core.ModernLoggingSystem;
using Vector3 = SASZombieAssaultTD.Engine.VectorMath.Vector3;
using System.Reflection;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public sealed class PauseScene : BaseScene
    {
        private bool _resumeRequested;
        private Scene? previousGameScene;

        public PauseScene()
        {
            ModernLoggingSystem.Log("BREAKPOINT", "Execution reached PauseScene constructor");
            ModernLoggingSystem.Log("BREAKPOINT", "Reached execution checkpoint");
            ModernLoggingSystem.Log(
            "BREAKPOINT",
            $"Method={nameof(MethodBase.GetCurrentMethod)}, " +
            $"Line={new StackTrace(true).GetFrame(0)?.GetFileLineNumber()}"
            );
        }

        /// <summary>
        /// P11-11-02: Called when the pause scene becomes active.
        /// </summary>
        public override void OnEnter()
        {
            _resumeRequested = false;
            ModernLoggingSystem.Log("BREAKPOINT", "PauseScene.OnEnter() completed");
        }

        /// <summary>
        /// P11-11-02: Called when the pause scene becomes inactive.
        /// </summary>
        public override void OnExit()
        {
            // Cleanup pause resources
            _resumeRequested = false;
            ModernLoggingSystem.Log("Info", "[PauseScene] OnExit: Pause scene shutting down.");
        }

        /// <summary>
        /// P11-11-02: Called every frame to update pause menu logic.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame.</param>
        public override void OnUpdate(float deltaTime)
        {
            // Handle pause menu input - check if the user has requested to resume.
            // P11-11-06: Input is now accessible through the Input property
            if (Input != null)
            {
                // Check for ESC key press to resume game
                if (SASZombieAssaultTD.Engine.Input.InputSystem.IsKeyPressed(KeyCode.Escape))
                {
                    _resumeRequested = true;
                }
            }

            if (_resumeRequested)
            {
                ModernLoggingSystem.Log("Info", "[PauseScene] Resume requested.");
                // Queue transition back to previous scene via SceneManager
                SceneManager?.QueueScene(previousGameScene.Name);
            }
        }

        /// <summary>
        /// P11-11-02: Called every frame to render the pause overlay UI.
        /// </summary>
        /// <param name="context">The render context.</param>
        public override void OnRender(SASZombieAssaultTD.Engine.Rendering.IRenderContext context)
        {
            // Render pause overlay UI - draw semi-transparent overlay text.
            // ClearScreen is intentionally NOT called so the game scene
            // remains visible beneath the pause overlay.
            context.DrawText("PAUSED", new Vector3(340, 270, 0), Color.White, 24.0f);
            context.DrawText("Press ESC to Resume", new Vector3(240, 320, 0), Color.White, 18.0f);
        }

        // Legacy methods for backward compatibility
        public override void Initialize()
        {
            OnEnter();
        }

        public override void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        public override void Render(SASZombieAssaultTD.Engine.Rendering.IRenderContext context)
        {
            OnRender(context);
        }

        /// <summary>
        /// Signals the pause scene to resume gameplay.
        /// Called by the input subsystem when ESC is pressed.
        /// </summary>
        public void RequestResume()
        {
            _resumeRequested = true;
        }
    }
}




