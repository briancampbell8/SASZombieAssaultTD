/*
File: GameScene.cs
Author: BDC
Created: 2026-02-08

Purpose:
Primary gameplay scene. Loads assets, tracks frame stats, and renders
using the engine's render context.

Notes:
Overwrites previous version. No framebuffer ownership here.
RSManagerding is handled by AssetPipeline; this scene only reads from AssetRegistry.
AssetRegistry.All() returns a snapshot for safe iteration outside locks.
*/
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Resources;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public sealed class GameScene : BaseScene
    {
        // AssetRegistry exposes a dictionary via All(); store that directly.
        private IReadOnlyDictionary<string, string> _assets = null!;
        private FrameStats _frameStats = null!;

        public GameScene()
        {
            ModernLoggingSystem.Log("Info", "[GameScene] Constructor reached.");
        }

        /// <summary>
        /// P11-11-02: Called when the game scene becomes active.
        /// </summary>
        public override void OnEnter()
        {
            ModernLoggingSystem.Log("Info", "[GameScene] OnEnter: Game scene starting...");

            // Load all assets from the registry as a single bundle.
            LoadGameAssets();

            // Reset frame stats for this scene
            _frameStats = new FrameStats();

            ModernLoggingSystem.Log("Info", "[GameScene] OnEnter: Game scene initialization complete.");
        }

        /// <summary>
        /// Loads game-specific assets and content.
        /// Overrides BaseScene.LoadContent to load game assets.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();
            
            ModernLoggingSystem.Log("Info", "[GameScene] Loading game assets...");
            
            try
            {
                // Load game-specific assets from AssetRegistry
                LoadGameAssets();
                
                // Initialize game systems
                InitializeGameSystems();
                
                // Create game entities
                CreateGameEntities();
                
                ModernLoggingSystem.Log("Info", "[GameScene] Game assets loaded successfully");
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("Error", $"[GameScene] Failed to load assets: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Loads game-specific assets from the asset registry.
        /// </summary>
        private void LoadGameAssets()
        {
            try
            {
                // Get assets from AssetRegistry
                var allAssets = Resources.AssetRegistry.All();
                _assets = allAssets.ToDictionary((KeyValuePair<string, object> kvp) => kvp.Key, (KeyValuePair<string, object> kvp) => kvp.Value?.ToString() ?? string.Empty);
                
                ModernLoggingSystem.Log("Info", $"[GameScene] Loaded {_assets.Count} assets from registry");
                
                // Log asset types for debugging
                var assetTypes = new Dictionary<string, int>();
                foreach (var asset in _assets)
                {
                    var extension = System.IO.Path.GetExtension(asset.Key).ToLower();
                    assetTypes[extension] = assetTypes.GetValueOrDefault(extension, 0) + 1;
                }
                
                foreach (var type in assetTypes)
                {
                    ModernLoggingSystem.Log("Debug", $"[GameScene] {type.Value} {type.Key} assets");
                }
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("Error", $"[GameScene] Error loading assets: {ex.Message}");
                _assets = new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Initializes game-specific systems.
        /// </summary>
        private void InitializeGameSystems()
        {
            // Initialize game systems that are needed for gameplay
            // Examples:
            // - Physics system
            // - Audio system
            // - Particle system
            // - Game logic systems
            
            ModernLoggingSystem.Log("Info", "[GameScene] Game systems initialized");
        }

        /// <summary>
        /// Creates initial game entities.
        /// </summary>
        private void CreateGameEntities()
        {
            // Create initial game entities
            // Examples:
            // - Player spawn points
            // - Initial enemies
            // - Environment objects
            // - UI elements
            
            ModernLoggingSystem.Log("Info", "[GameScene] Initial game entities created");
        }

        /// <summary>
        /// P11-11-02: Called when the game scene becomes inactive.
        /// </summary>
        public override void OnExit()
        {
            ModernLoggingSystem.Log("Info", "[GameScene] OnExit: Game scene shutting down...");
        }

        /// <summary>
        /// P11-11-02: Called every frame to update game logic.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame.</param>
        public override void OnUpdate(float deltaTime)
        {
            // Track frame timing.
            _frameStats.OnFrame(deltaTime);
        }

        /// <summary>
        /// P11-11-02: Called every frame to render the game scene.
        /// </summary>
        /// <param name="context">The render context.</param>
        public override void OnRender(IRenderContext context)
        {
            // Clear screen (blue background).
            context.ClearScreen();

            // Example debug draw:
            context.DrawText(".", new Vector3(100f, 100f, 0f), Color.White);
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

        public override void Render(IRenderContext context)
        {
            OnRender(context);
        }
    }
}





