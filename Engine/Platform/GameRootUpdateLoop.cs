// =====================================================================================================
//  FILE: GameRootUpdateLoop.cs
//  PATH: Engine/Platform/GameRootUpdateLoop.cs
//  SUBSYSTEM: Platform Abstraction Layer / Frame Loop Coordination
//
//  ROLE:
//      Deterministic per-frame coordinator for update and render sequencing. This subsystem drives the
//      engine’s heartbeat by invoking UpdateManager and RenderManager using the authoritative GPU
//      render context (D3D11Adapter_Core).
//
//  RESPONSIBILITIES:
//      - Invoke state machine update and render paths.
//      - Dispatch UpdateManager.Tick() deterministically.
//      - Dispatch RenderManager.RenderAll() deterministically.
//      - Maintain strict frame sequencing (Update → Render).
//      - Manage BeginFrame / EndFrame / Present on the GPU adapter.
//      - Emit diagnostic logs for update/render heartbeat.
//
//  NON-RESPONSIBILITIES:
//      - CPU framebuffer upload logic.
//      - Legacy IDrawingContext or adapter manager paths.
//      - GPU device or swap-chain creation/management.
//      - High-level game logic or scene transitions.
//      - UI composition (handled by ModernUIRenderer).
//
//  ARCHITECTURAL NOTES:
//      - Enforces GPU-only pipeline via D3D11Adapter_Core.
//      - Replaces legacy partial-method update/render implementations.
//      - GameRootMain composes and invokes this subsystem directly.
//      - Strict Option B architecture: concrete subsystem types, no interface indirection.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.Scenes;
using SASZombieAssaultTD.Engine.State;
using SASZombieAssaultTD.Engine.Systems;
using SASZombieAssaultTD.Engine.UI.Rendering.Modern;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.GameRoot
{
    internal sealed class GameRootUpdateLoop
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly UpdateManager _updateManager;
        private readonly RenderManager _renderManager;
        private readonly ISystemRegistry _registry;
        private readonly D3D11Adapter_Core _adapter;

        private ulong _updateCounter;
        private ulong _renderCounter;

        // ----------------------------------------------------------------------------------------------
        // Authoritative Constructor — GPU-only pipeline
        // ----------------------------------------------------------------------------------------------
        public GameRootUpdateLoop(
            ISystemRegistry registry,
            SystemManager systemManager,
            UpdateManager updateManager,
            RenderManager renderManager,
            StateMachine stateMachine,
            D3D11Adapter_Core renderContext,
            ModernUIRenderer uiRenderer)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _updateManager = updateManager ?? throw new ArgumentNullException(nameof(updateManager));
            _renderManager = renderManager ?? throw new ArgumentNullException(nameof(renderManager));
            _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));

            _adapter = renderContext ?? throw new ArgumentNullException(nameof(renderContext));
        }

        public GameRootUpdateLoop(StateMachine stateMachine, UpdateManager updateManager, RenderManager renderManager, D3D11Adapter_Core renderContext)
        {
            _stateMachine = stateMachine;
            _updateManager = updateManager;
            _renderManager = renderManager;
            _adapter = renderContext;
        }

        // ----------------------------------------------------------------------------------------------
        // Update Sequence
        // ----------------------------------------------------------------------------------------------
        public void UpdateFrame(float deltaTime)
        {
            try
            {
                _stateMachine.Update(deltaTime);
                _updateManager.Tick(deltaTime);

                _updateCounter++;

                if (_updateCounter % 3600 == 0)
                {
                    DLogger.Log(LogSubsystems.GameRoot, LogLevel.Debug,
                        $"[UpdateLoop] Update heartbeat OK. Total updates: {_updateCounter}");
                }
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Error,
                    $"[UpdateLoop] Update failed: {ex.Message}");
            }
        }
        // ----------------------------------------------------------------------------------------------
        // Render Sequence — Correct, Deterministic, Single-Pass GPU Frame
        // ----------------------------------------------------------------------------------------------
        public void RenderFrame()
        {
            if (_adapter == null)
                return;

            // Begin GPU frame
            _adapter.BeginFrame();
            _adapter.ClearScreen();

            // Resolve the SceneManager directly out of our active dependencies
            var activeRegistry = _registry ?? (_stateMachine as StateMachine)?.GetService<ISystemRegistry>();
            var sceneManager = activeRegistry?.Resolve<SceneManager>() ?? _adapter.GetService<SceneManager>();

            if (sceneManager?.CurrentScene != null)
            {
                // Explicitly check if the current active scene is the MainMenuScene
                if (sceneManager.CurrentScene is MainMenuScene mainMenu)
                {
                    // Force-route directly to your specialized multi-parameter render call
                    mainMenu.Render(_adapter, 1f / 60f);
                }
                else
                {
                    // Fallback render route for generic scenes
                    sceneManager.CurrentScene.Render(_adapter);
                }
            }

            // Render state machine (gameplay, world modules)
            _stateMachine?.Render(_adapter);

            // Render all registered render systems
            _renderManager?.RenderAll(_adapter);

            // End + Present
            _adapter.EndFrame();
            _adapter.Present();

            _renderCounter++;

            if (_renderCounter % 3600 == 0)
            {
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Debug,
                    $"[UpdateLoop] Render heartbeat OK. Total frames: {_renderCounter}");
            }
        }
    }
}
