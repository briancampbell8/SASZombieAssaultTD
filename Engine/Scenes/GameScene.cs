/*
File: GameScene.cs
Author: BDC
Created: 2026-02-08

Purpose:
    Primary gameplay scene. Loads assets, tracks frame stats, and renders
    using the engine's render context.

Notes:
    Overwrites previous version. No framebuffer ownership here.
*/

using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Systems.Assets;
using SASZombieAssaultTD.Engine.Systems.Diagnostics;
using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public sealed class GameScene : Scene
    {
        // AssetRegistry exposes a dictionary via All(); store that directly.
        private IReadOnlyDictionary<string, string> _assets;
        private FrameStats _frameStats;

        public GameScene() : base()
        {
            DebugLogger.Log("Info", "[GameScene] Constructor reached.");

            // Initialize non-nullable fields with safe defaults
            _assets = new Dictionary<string, string>();
            _frameStats = new FrameStats();
        }

        public override void Initialize()
        {
            DebugLogger.Log("Info", "[GameScene] Initializing...");

            // Load all assets from the registry as a single bundle.
            // AssetRegistry does not contain GetAllAsBundle(); use the available API.
            _assets = AssetRegistry.All();

            // Reset frame stats for this scene
            _frameStats = new FrameStats();

            DebugLogger.Log("Info", "[GameScene] Initialization complete.");
        }

        public override void Update(TimeSpan deltaTime)
        {
            // Track frame timing.
            _frameStats.OnFrame((float)deltaTime.TotalSeconds);
        }

        public override void Render(IRenderContext context)
        {
            // Clear screen (blue background).
            context.ClearScreen();

            // Example debug draw:
            context.DrawText(".", 100, 100);
        }

        public override void Shutdown()
        {
            DebugLogger.Log("Info", "[GameScene] Shutdown.");
        }
    }
}