// ====================================================================================================
// FILE: GameRootInitialization.cs
// PATH: Engine/GameRoot/GameRootInitialization.cs
// SUBSYSTEM: Engine Bring-Up / Platform Abstraction Layer
//
// ROLE:
//     Coordinates deterministic engine startup, including system registration, render context binding,
//     UI renderer initialization, and state machine activation.
//
// RESPONSIBILITIES:
//     • Register and initialize core engine systems (SystemManager, UpdateManager, RenderManager).
//     • Bind D3D11Adapter_Core as the active render context.
//     • Bind D3D11DrawingContext as the active GPU-backed drawing surface for RenderSystem.
//     • Initialize ModernUIRenderer and integrate it into the render pipeline.
//     • Activate StateMachine in a deterministic, audit-friendly sequence.
//     • Pre-load critical UI textures before the first Present().
//
// NON-RESPONSIBILITIES:
//     • Frame rendering, update tick execution, or gameplay rule evaluation.
//     • Asset streaming, disk I/O, or runtime resource management.
//     • Shutdown sequencing or destruction routines.
//
// ARCHITECTURE NOTES:
//     • Strict Option-B constructor chaining.
//     • Executes immediately after EngineBootstrap establishes GPU context.
//     • All operations are deterministic and logged for replay/debugging.
//
// AUTHOR: BDC
// LAST UPDATED: 2026-09-10 (D3D11DrawingContext Integration)
// ====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Input;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.Render.D3D11;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using SASZombieAssaultTD.Engine.Resources.AssetPipeline;
using SASZombieAssaultTD.Engine.Scenes;
using SASZombieAssaultTD.Engine.State;
using SASZombieAssaultTD.Engine.Systems;
using SASZombieAssaultTD.Engine.TextureRendering;
using SASZombieAssaultTD.Engine.UI.Rendering.Modern;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.GameRoot
{
    internal sealed class GameRootInitialization
    {
        private readonly ISystemRegistry _systemRegistry;

        private readonly SystemManager _systemManager;
        private readonly UpdateManager _updateManager;
        private readonly RenderManager _renderManager;
        private readonly UIInputRouter _inputRouter;
        private readonly StateMachine _stateMachine;
        private SceneManager _sceneManager;
        private readonly ModernUIRenderer _uiRenderer;
        private readonly D3D11Adapter_Core d3D11Adapter;

        private bool _initialized;
        private readonly object _stateLock = new();

        public bool IsInitialized => _initialized;

        public GameRootInitialization(
            ISystemRegistry systemRegistry,
            SystemManager systemManager,
            UpdateManager updateManager,
            RenderManager renderManager,
            UIInputRouter inputRouter,
            StateMachine stateMachine,
            ModernUIRenderer uiRenderer,
            D3D11Adapter_Core adapter_Core)
        {
            _systemRegistry = systemRegistry;
            _systemManager = systemManager;
            _updateManager = updateManager;
            _renderManager = renderManager;
            _inputRouter = inputRouter;
            _stateMachine = stateMachine;
            _uiRenderer = uiRenderer;
            d3D11Adapter = adapter_Core;
        }

        internal void Execute()
        {
            if (_initialized)
                return;

            try
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, ">>> GAME ROOT INITIALIZATION START <<<");

                // 1. Register core systems
                _systemRegistry.RegisterSystem(_systemManager);
                _systemRegistry.RegisterSystem(_updateManager);
                _systemRegistry.RegisterSystem(_renderManager);
                _systemRegistry.RegisterSystem(_inputRouter);
                _systemRegistry.RegisterSystem(_uiRenderer);

                DLogger.Log(LogSubsystems.ResourcesPipeline, "GameRootInitialization: Core systems registered");

                // 2. Initialize system manager
                _systemManager.InitializeAll();
                DLogger.Log(LogSubsystems.ResourcesPipeline, "GameRootInitialization: SystemManager initialized");

                var sceneManager = _systemRegistry.GetService<SceneManager>()
                    ?? throw new InvalidOperationException("SceneManager not registered.");

                _sceneManager = sceneManager;

                // 3. Assign D3D11 render context
                _renderManager.SetRenderContext(d3D11Adapter);
                DLogger.Log(LogSubsystems.ResourcesPipeline, "GameRootInitialization: RenderContext assigned");

                // 3.5 Bind GPU-backed drawing context
                var deviceCore = _systemRegistry.GetService<D3D11DeviceCore>()
                    ?? throw new InvalidOperationException(
                        "D3D11DeviceCore not registered or unavailable during GameRootInitialization.");

                var drawingContext = new D3D11DrawingContext(d3D11Adapter, deviceCore);
                _renderManager.SetDrawingContext(drawingContext);

                DLogger.Log(LogSubsystems.ResourcesPipeline,
                    "GameRootInitialization: D3D11DrawingContext bound to RenderSystem");

                // 4. Register ModernUIRenderer
                _renderManager.RegisterSystem(_uiRenderer);
                DLogger.Log(LogSubsystems.ResourcesPipeline, "GameRootInitialization: ModernUIRenderer registered");

                // 5. Initialize UI renderer
                _uiRenderer.Initialize();
                DLogger.Log(LogSubsystems.ResourcesPipeline, "GameRootInitialization: UI Renderer initialized");

                // 6. Activate state machine
                _stateMachine.ResetGame();
                _stateMachine.StartGame();
                DLogger.Log(LogSubsystems.ResourcesPipeline, "GameRootInitialization: StateMachine activated");

                // 6.5 Pre-load level assets (deterministic, fully guarded)
                var textureManager = _systemRegistry.GetService<TextureManager>()
                    ?? throw new InvalidOperationException("TextureManager not registered.");

                DLogger.Log(LogSubsystems.ResourcesPipeline,
                    "GameRootInitialization: Pre-loading level map layout from maps/MeanStreets.png...");

                byte[] pngBytes = AssetPipeline_Runtime.LoadBytes("maps/MeanStreets.png")
                    ?? throw new InvalidOperationException("AssetPipeline_Runtime.LoadBytes returned null for maps/MeanStreets.png.");

                if (pngBytes.Length == 0)
                    throw new InvalidOperationException("MeanStreets.png data stream is missing or empty.");

                var gpuSurface = TextureLoaderGPU.CreateTextureFromPng(deviceCore, pngBytes)
                    ?? throw new InvalidOperationException(
                        "TextureLoaderGPU failed to create GPU texture from MeanStreets.png.");

                textureManager.AddRaw("MainMenu.png", gpuSurface);

                DLogger.Log(LogSubsystems.ResourcesPipeline,
                    "GameRootInitialization: MeanStreets.png successfully baked to VRAM under MainMenu layout handle");

                lock (_stateLock)
                {
                    _initialized = true;
                }

                DLogger.Log(LogSubsystems.ResourcesPipeline, ">>> GAME ROOT INITIALIZATION SUCCESS <<<");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline,
                    "[FATAL ENGINE STARTUP EXCEPTION] " + ex.ToString());
                throw;
            }
        }
    }
}
