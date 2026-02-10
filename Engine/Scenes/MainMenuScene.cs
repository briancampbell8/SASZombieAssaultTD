/*
    File:    MainMenuScene.cs
    Author:  BDC
    Created: 2026-02-07
    Purpose: Main menu scene. Handles menu initialization, input updates, and UI rendering.
    Notes:   Inherits from Scene. Provides debug breakpoints and placeholder UI behavior.
*/

using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Systems.Diagnostics;
using System;
using System.Diagnostics;
using System.Reflection;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public sealed class MainMenuScene : Scene
    {
        private bool _startRequested;

        public MainMenuScene()
            : base()
        {
            DebugLogger.Log("BREAKPOINT", "Execution reached MainMenuScene constructor");
            DebugLogger.Log("BREAKPOINT", "Reached execution checkpoint");
            DebugLogger.Log(
                "BREAKPOINT",
                $"Method={nameof(MethodBase.GetCurrentMethod)}, " +
                $"Line={new StackTrace(true).GetFrame(0)?.GetFileLineNumber()}"
            );
        }

        public override void Initialize()
        {
            _startRequested = false;
            DebugLogger.Log("BREAKPOINT", "MainMenuScene.Initialize() completed");
        }

        public override void Update(TimeSpan deltaTime)
        {
            // Handle menu input — check if the user has requested to start the game.
            // When input subsystem is wired, this will read from InputRouter.
            // For now, _startRequested can be set externally or by a future input binding.
            if (_startRequested)
            {
                DebugLogger.Log("Info", "[MainMenuScene] Start requested.");
            }
        }

        public override void Render(IRenderContext context)
        {
            // Render menu UI — clear screen and draw title text.
            context.ClearScreen();
            context.DrawText("SAS Zombie Assault TD", 200, 250);
            context.DrawText("Press ENTER to Start", 220, 300);
        }

        public override void Shutdown()
        {
            // Cleanup menu resources
            _startRequested = false;

            DebugLogger.Log("Info", "[MainMenuScene] Shutdown.");
        }

        /// <summary>
        /// Signals the menu to transition to the game scene.
        /// Called by the input subsystem when ENTER is pressed.
        /// </summary>
        public void RequestStart()
        {
            _startRequested = true;
        }
    }
}