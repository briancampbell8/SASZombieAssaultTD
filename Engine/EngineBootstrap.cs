// =====================================================================================================
//  FILE: EngineBootstrap.cs
//  PATH: Engine/EngineBootstrap.cs
//  SUBSYSTEM: Core Composition Root
//
//  ROLE:
//      Deterministic engine composition root responsible for constructing all top‑level engine managers,
//      wiring the Win32/D3D11 platform stack into the rendering pipeline, registering core services into
//      the SystemRegistry, and instantiating GameRootMain using the authoritative GPU render context.
//
//  ARCHITECTURE (2026‑09):
//      - CPU framebuffer (FramebufferDrawing) is NOT part of the render pipeline.
//      - GPU render context (D3D11Adapter_Core) is the authoritative D3D11Adapter_Core.
//      - ModernUIRenderer uses the GPU context directly (Option A1).
//      - RenderContextForwarder is NOT used here.
//      - Composition root is clean, deterministic, and GPU‑centric.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Input;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using SASZombieAssaultTD.Engine.State;
using SASZombieAssaultTD.Engine.Systems;
using SASZombieAssaultTD.Engine.UI.Rendering.Modern;

namespace SASZombieAssaultTD.Engine.Platform
{
    public static class EngineBootstrap
    {
        public static GameRootMain Start(ISystemRegistry registry)
        {
            if (registry == null)
                throw new ArgumentNullException(nameof(registry));

            var systemManager = registry.Resolve<SystemManager>();
            var updateManager = registry.Resolve<UpdateManager>();
            var renderManager = registry.Resolve<RenderManager>();
            var inputRouter = registry.Resolve<UIInputRouter>();
            var stateMachine = registry.Resolve<StateMachine>();
            var adapterCore = registry.Resolve<D3D11Adapter_Core>();
            var uiRenderer = registry.Resolve<ModernUIRenderer>();

            var root = new GameRootMain(
                registry,
                systemManager,
                updateManager,
                renderManager,
                inputRouter,
                stateMachine,
                adapterCore,
                uiRenderer);

            root.Initialize();
            root.Run();

            return root;
        }

        internal static GameRootMain CreateGameRootMain(D3D11Window window, D3D11DeviceCore deviceCore)
        {
            // Validate arguments to ensure deterministic composition root behavior.
            if (window == null)
                throw new ArgumentNullException(nameof(window));
            if (deviceCore == null)
                throw new ArgumentNullException(nameof(deviceCore));

            // 1. Initialize the central DI registry container
            ISystemRegistry registry = new SystemRegistry();

            // 2. Explicitly register hardware components FIRST so dependencies can resolve them instantly
            registry.Register<D3D11Window>(window);
            registry.Register<D3D11DeviceCore>(deviceCore);

            // 3. Construct and register the authoritative GPU-only adapter context
            var adapterCore = new D3D11Adapter_Core(window, deviceCore);
            registry.Register<D3D11Adapter_Core>(adapterCore);

            // 4. Spin up required managers/subsystems
            var systemManager = new SystemManager((SystemRegistry)registry);
            var updateManager = new UpdateManager();
            var renderManager = new RenderManager();
            var inputRouter = new UIInputRouter();
            var stateMachine = new StateMachine();


            var textureAtlasManager = new UITextureAtlasManager(); // Make sure to use your exact class name if it differs
            registry.Register<UITextureAtlasManager>(textureAtlasManager);

            // 5. Build the UI Renderer and BIND its core hardware dependencies immediately
            var uiRenderer = new ModernUIRenderer(adapterCore);
            uiRenderer.P2_BindCore(deviceCore, adapterCore, textureAtlasManager); // <--- THIS KILLS THE CRASH!

            registry.Register<SystemManager>(systemManager);
            registry.Register<UpdateManager>(updateManager);
            registry.Register<RenderManager>(renderManager);
            registry.Register<UIInputRouter>(inputRouter);
            registry.Register<StateMachine>(stateMachine);
            registry.Register<ModernUIRenderer>(uiRenderer);

            // 6. Invoke the AUTHORITATIVE primary constructor with fully satisfied dependencies
            return new GameRootMain(
                registry,
                systemManager,
                updateManager,
                renderManager,
                inputRouter,
                stateMachine,
                adapterCore,
                uiRenderer);
        }

    }
}
