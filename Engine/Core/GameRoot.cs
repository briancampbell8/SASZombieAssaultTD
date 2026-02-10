/*
File:    GameRoot.cs
Author:  BDC
Created: 2026-02-10

Purpose:
Central engine bootstrapper. Wires subsystems, creates the window,
initializes the first scene, and runs the main loop.

Notes:
- Entry point for engine-level lifecycle.
- Default framebuffer/window size controlled by DefaultWidth/DefaultHeight.
- Logs early checkpoints for debugging.
*/
using SASZombieAssaultTD.Engine.Platform;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Scenes;
using SASZombieAssaultTD.Engine.Systems;
using SASZombieAssaultTD.Engine.Systems.Assets;
using SASZombieAssaultTD.Engine.Systems.Diagnostics;
using SASZombieAssaultTD.Engine.Systems.Gameplay;
using System;
using System.Diagnostics;

namespace SASZombieAssaultTD.Engine.Core
{
    /// <summary>
    /// Central engine bootstrapper. Wires subsystems and initializes the first scene.
    /// </summary>
    public sealed class GameRoot
    {
        private const int DefaultWidth = 800;
        private const int DefaultHeight = 600;

        private readonly Framebuffer _fb;

        private readonly AnimationSystem _animationSystem;
        private readonly CameraSystem _cameraSystem;
        private readonly PathfindingSystem _pathfindingSystem;
        private readonly ProjectileSystem _projectileSystem;

        // Engine-wide services (wired; actual usage comes with gameplay integration)
        private readonly EventDispatcher _eventDispatcher;
        private readonly GameStateManager _gameStateManager;
        private readonly InputRouter _inputRouter;

        // Render pipeline frontend (wired; Enqueue/Sort/Flush integration is future work)
        private readonly RenderQueueFrontend _renderQueue;

        private GameScene? _activeScene;
        private Win32Window? _window;

        public GameRoot()
        {
            // Match your window size
            _fb = new Framebuffer(DefaultWidth, DefaultHeight);

            _pathfindingSystem = new PathfindingSystem();
            _animationSystem = new AnimationSystem();
            _cameraSystem = new CameraSystem(DefaultWidth, DefaultHeight);
            _projectileSystem = new ProjectileSystem();

            // Engine-wide services
            _eventDispatcher = new EventDispatcher();
            _gameStateManager = new GameStateManager();
            _inputRouter = new InputRouter();

            // Render pipeline
            _renderQueue = new RenderQueueFrontend();
        }

        public void Initialize()
        {
            // Initialize assets before creating the scene/window
            try
            {
                AssetInitializer.InitializeAllAssets();
            }
            catch (Exception ex)
            {
                DebugLogger.Log(DebugLogger.Phase5,
                    $"[GameRoot] AssetInitializer.InitializeAllAssets failed: {ex.Message}");
                return;
            }

            // Load all registered assets via AssetPipeline
            try
            {
                var loadContext = new AssetLoadContext("Content");
                foreach (var kvp in AssetRegistry.All())
                {
                    string? ext = AssetUtils.GetExtension(kvp.Value);
                    AssetType type = InferAssetType(ext);
                    loadContext.Assets.Add(new AssetMetadata(kvp.Key, kvp.Value, type, format: ext));
                }

                var loadResult = AssetPipeline.LoadAll(loadContext);

                DebugLogger.Log(DebugLogger.Phase5,
                    $"[GameRoot] Asset load complete: {loadResult}");

                if (loadResult.FailureCount > 0)
                {
                    DebugLogger.Log(DebugLogger.Phase5,
                        $"[GameRoot] {loadResult.FailureCount} asset(s) failed to load. Aborting initialization.");
                    return;
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Log(DebugLogger.Phase5,
                    $"[GameRoot] Asset loading failed: {ex.Message}");
                return;
            }

            // Validate runtime readiness before proceeding
            if (!AssetPipeline.ValidateRuntimeReadiness())
            {
                DebugLogger.Log(DebugLogger.Phase5,
                    "[GameRoot] Runtime readiness check failed. Aborting initialization.");
                return;
            }

            // Initialize subsystems in engine order:
            // PathfindingSystem → AnimationSystem → CameraSystem → ProjectileSystem
            try
            {
                _pathfindingSystem.Initialize();
                _animationSystem.Initialize();
                _cameraSystem.Initialize();
                _projectileSystem.Initialize();
            }
            catch (Exception ex)
            {
                DebugLogger.Log(DebugLogger.Phase5,
                    $"[GameRoot] Subsystem initialization failed: {ex.Message}");
                return;
            }

            // Create window and pass framebuffer
            try
            {
                _window = new Win32Window(DefaultWidth, DefaultHeight, "SAS Zombie Assault TD", _fb);
                _window.Create();
            }
            catch (Exception ex)
            {
                DebugLogger.Log(DebugLogger.Phase5,
                    $"[GameRoot] Window creation failed: {ex.Message}");
                return;
            }

            // Create first scene and pass framebuffer
            try
            {
                _activeScene = new GameScene();
                _activeScene.Initialize();
            }
            catch (Exception ex)
            {
                DebugLogger.Log(DebugLogger.Phase5,
                    $"[GameRoot] Scene initialization failed: {ex.Message}");
                return;
            }

            // Start main loop
            RunMainLoop();
        }

        private void RunMainLoop()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (true)
            {
                // Pump OS messages
                if (_window is null || !_window.PumpMessages())
                    break;

                // Compute delta time
                float deltaSeconds = (float)stopwatch.Elapsed.TotalSeconds;
                stopwatch.Restart();

                // Update subsystems in engine order and bridge data between them
                UpdateSubsystems(deltaSeconds);

                // Update scene
                _activeScene?.Update(TimeSpan.FromSeconds(deltaSeconds));
                _activeScene?.Render(_fb); // Framebuffer now implements IRenderContext

                // Present framebuffer to window
                _window.Present();
            }

            // Shutdown active scene first (it may depend on subsystems)
            _activeScene?.Shutdown();

            // Then shutdown subsystems in reverse order
            _projectileSystem.Shutdown();
            _cameraSystem.Shutdown();
            _animationSystem.Shutdown();
            _pathfindingSystem.Shutdown();

            // Flush and close the diagnostic log stream
            DebugLogger.Shutdown();
        }

        /// <summary>
        /// Updates all gameplay subsystems in the correct order and bridges
        /// output data between them each frame.
        /// Order: PathfindingSystem → AnimationSystem → CameraSystem → ProjectileSystem
        /// </summary>
        private void UpdateSubsystems(float deltaSeconds)
        {
            // 1. Pathfinding produces movement vectors
            _pathfindingSystem.Update(deltaSeconds);

            // 2. Bridge: feed PathfindingSystem movement into AnimationSystem
            foreach (var kvp in _pathfindingSystem.MovementOutputs)
            {
                MovementOutput output = kvp.Value;
                _animationSystem.ApplyMovement(kvp.Key, output.DeltaX, output.DeltaY, output.State);
            }

            // 3. Animation advances frames and applies movement
            _animationSystem.Update(deltaSeconds);

            // 4. Bridge: feed AnimationSystem follow-target into CameraSystem
            int followId = _cameraSystem.GetFollowTargetId();
            if (followId >= 0 &&
                _animationSystem.Transforms.TryGetValue(followId, out AnimationTransform? target))
            {
                _cameraSystem.ApplyFollowTarget(target.X, target.Y, deltaSeconds);
            }

            // 5. Camera rebuilds view/projection matrices
            _cameraSystem.Update(deltaSeconds);

            // 6. Projectiles update independently (no data bridge needed)
            _projectileSystem.Update(deltaSeconds);
        }

        private static AssetType InferAssetType(string? ext)
        {
            if (ext is null) return AssetType.Unknown;

            return ext.ToLowerInvariant() switch
            {
                "png" or "jpg" or "jpeg" => AssetType.Texture,
                "json" => AssetType.Json,
                "wav" => AssetType.Sound,
                "ogg" => AssetType.Music,
                _ => AssetType.Binary
            };
        }
    }
}
