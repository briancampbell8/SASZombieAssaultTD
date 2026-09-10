// =====================================================================================================
//  FILE: GameRootSystemRegistration.cs
//  PATH: Engine/GameRoot/GameRootSystemRegistration.cs
//  SUBSYSTEM: Platform Abstraction Layer / System Registration
//
//  ROLE:
//      Provides deterministic, structured, and debuggable system/service registration for the engine
//      host (GameRootMain). This subsystem centralizes all registration operations and ensures that
//      core engine services are available before initialization and update/render pipelines begin.
//
//  RESPONSIBILITIES:
//      - Register core engine systems (SceneManager, TextureManager, future ECS systems, controllers).
//      - Register engine services (input router, state machine, render context, UI renderer).
//      - Provide safe accessors for retrieving registered services.
//      - Maintain strict separation between GameRootMain and registration logic.
//      - Emit consistent diagnostic logs for all registration operations.
//
//  NON-RESPONSIBILITIES:
//      - Executing system logic, update ticks, or render passes.
//      - Managing active update loops or frame presenter structures.
//      - Handling state transitions or modifying gameplay flags.
//      - Owning or altering the engine boot/shutdown lifecycle.
//
//  ARCHITECTURAL NOTES:
//      - This subsystem replaces the legacy partial GameRoot SystemRegistration.cs file.
//      - GameRootMain composes this controller and invokes RegisterCoreSystems() during initialization.
//      - All system/service registration is funneled through this subsystem for clarity and debugging.
//      - ISystemRegistry is the authoritative backing store for all registered systems/services.
//
//  CHANGE LOG:
//      [2026-09-08 | BDC] Added correct TextureManager construction using GPU device.
//      [2026-09-08 | Copilot] Added renderContext injection and deterministic Resolve<T>() alignment.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Input;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using SASZombieAssaultTD.Engine.Scenes;
using SASZombieAssaultTD.Engine.State;
using SASZombieAssaultTD.Engine.Systems;
using SASZombieAssaultTD.Engine.TextureRendering;
using SASZombieAssaultTD.Engine.UI.Rendering.Modern;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.GameRoot
{
    internal sealed class GameRootSystemRegistration
    {
        // Authoritative backing registry and render context
        private readonly ISystemRegistry _registry;
        private readonly D3D11Adapter_Core _renderContext;
        private ISystemRegistry systemRegistry;

        // SINGLE AUTHORITATIVE CONSTRUCTOR
        public GameRootSystemRegistration(
            ISystemRegistry registry,
            SystemManager systemManager,
            UpdateManager updateManager,
            RenderManager renderManager,
            UIInputRouter inputRouter,
            StateMachine stateMachine,
            ModernUIRenderer uiRenderer,
            D3D11Adapter_Core renderContext)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _renderContext = renderContext ?? throw new ArgumentNullException(nameof(renderContext));

            // NOTE:
            // systemManager, updateManager, renderManager, inputRouter, stateMachine, uiRenderer
            // are currently not stored here because registration logic only needs the registry
            // and GPU context. If future registration steps require them, they can be added
            // as readonly fields and used accordingly.
        }

        public GameRootSystemRegistration(ISystemRegistry systemRegistry, D3D11Adapter_Core renderContext)
        {
            this.systemRegistry = systemRegistry;
            _renderContext = renderContext;
        }

        // ------------------------------------------------------------------------------------------------
        // CORE REGISTRATION PIPELINE
        // ------------------------------------------------------------------------------------------------

        public void RegisterCoreSystems()
        {
            try
            {
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Info,
                    "[SystemRegistration] Beginning core system registration...");

                RegisterSceneManager();
                RegisterTextureManager();
                RegisterAdditionalSystems();

                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Info,
                    "[SystemRegistration] Core system registration completed.");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Error,
                    $"[SystemRegistration] Core registration failed: {ex.Message}");
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Error,
                    ex.ToString(), "Core system registration");
                throw;
            }
        }

        // ------------------------------------------------------------------------------------------------
        // SYSTEM REGISTRATION
        // ------------------------------------------------------------------------------------------------

        private void RegisterSceneManager()
        {
            // 1. Resolve the active state machine from the composition root
            var stateMachine = _registry.Resolve<StateMachine>();
            if (stateMachine == null)
                throw new InvalidOperationException("StateMachine is not registered in SystemRegistry; cannot construct SceneManager.");

            // 2. Build the SceneFactory matching the 2026-07 architectural changes
            var sceneFactory = new SceneFactory(); // If SceneFactory needs dependencies, use _registry.Resolve<T>()

            // 3. Invoke the AUTHORITATIVE constructor with fully satisfied components
            var sceneManager = new SceneManager(sceneFactory, stateMachine);
            _registry.Register<SceneManager>(sceneManager);

            DLogger.Log(LogSubsystems.GameRoot, LogLevel.Info,
                "[SystemRegistration] SceneManager registered.");
        }

        private void RegisterTextureManager()
        {
            // Dynamically resolve the fully initialized context from the active registry container
            var activeAdapter = _registry.Resolve<D3D11Adapter_Core>();
            var activeDevice = _registry.Resolve<D3D11DeviceCore>();

            // Find whichever valid hardware device reference is available at this stage of initialization
            var deviceCoreInstance = activeDevice ?? activeAdapter?.DeviceCore;

            if (deviceCoreInstance == null)
                throw new InvalidOperationException("Cannot register TextureManager: " +
                    "No valid D3D11DeviceCore or D3D11Adapter_Core found in the SystemRegistry.");

            var textureManager = new TextureManager(deviceCoreInstance);
            _registry.Register<TextureManager>(textureManager);

            DLogger.Log(LogSubsystems.GameRoot, LogLevel.Info,
                "[SystemRegistration] TextureManager registered.");
        }

        private void RegisterAdditionalSystems()
        {
            DLogger.Log(LogSubsystems.GameRoot, LogLevel.Info,
                "[SystemRegistration] Additional systems registration completed.");
        }

        // ------------------------------------------------------------------------------------------------
        // SERVICE ACCESSORS
        // ------------------------------------------------------------------------------------------------

        public T GetService<T>() where T : class
        {
            return _registry.GetService<T>()
                ?? throw new InvalidOperationException(
                    $"Service of type {typeof(T).Name} is not registered.");
        }

        public T? GetServiceOrNull<T>() where T : class
        {
            return _registry.GetService<T>();
        }

        public void RegisterService<T>(T service) where T : class
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _registry.RegisterService(service);

            DLogger.Log(LogSubsystems.GameRoot, LogLevel.Info,
                $"[SystemRegistration] Service registered: {typeof(T).Name}");
        }
    }
}
