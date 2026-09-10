// =====================================================================================================
//  FILE: RenderManager.cs
//  PATH: Engine/Systems/RenderManager.cs
//  SUBSYSTEM: Rendering / Frame Scheduling & Dispatch
//
//  ROLE:
//      Central rendering scheduler responsible for executing all registered IRenderSystem
//      participants in deterministic order. RenderManager does not perform rendering itself;
//      it orchestrates the pipeline.
//
//  RESPONSIBILITIES:
//      - Maintain an ordered list of IRenderSystem subsystems.
//      - Invoke each subsystem’s Render(D3D11Adapter_Core) method per frame.
//      - Guarantee deterministic render ordering (Gameplay → HUD → UI Overlays).
//      - Manage frame lifecycle boundaries (BeginFrame / EndFrame / Present).
//      - Bind and expose the active IDrawingContext for RenderSystem and UI/HUD layers.
//
//  NON-RESPONSIBILITIES:
//      - Actual drawing (handled by individual render subsystems).
//      - Texture loading (handled by TextureManager).
//      - HUD state management (handled by HUDManager).
//      - Simulation updates (handled by UpdateManager).
//
//  ARCHITECTURAL NOTES:
//      - GameplayRenderSystem runs first.
//      - HUDRenderer runs second.
//      - ModernUIRenderer runs last.
//      - DrawingContext binding added for GPU-backed D3D11DrawingContext integration.
//
//  CHANGE LOG:
//      [2026-09-08 | BDC] Verified deterministic ordering and stabilized RenderAll() dispatch.
//      [2026-09-08 | Copilot] Cleaned null‑system removal logic and unified RenderAll() paths.
//      [2026-09-10 | BDC] Added SetDrawingContext() for D3D11DrawingContext integration.
// =====================================================================================================

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using SASZombieAssaultTD.Engine.TextureRendering.HUD;
using SASZombieAssaultTD.Engine.UI.Rendering;
using SASZombieAssaultTD.Engine.UI.Rendering.Modern;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Systems
{
    public sealed class RenderManager
    {
        public bool IsActive { get; private set; } = true;
        public D3D11Adapter_Core? Context { get; private set; }

        private IDrawingContext? _drawingContext;

        private readonly List<IRenderSystem> _systems = new();
        private readonly D3D11DeviceCore deviceCore;
        private readonly SystemRegistry systemRegistry;

        public RenderManager(D3D11DeviceCore deviceCore, SystemRegistry systemRegistry)
        {
            this.deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
            this.systemRegistry = systemRegistry ?? throw new ArgumentNullException(nameof(systemRegistry));

            DLogger.Log(LogSubsystems.ResourcesPipeline, "RenderManager constructed");
        }

        public RenderManager()
        {
        }

        // -------------------------------------------------------------------------------------------------
        //  CONTEXT BINDING
        // -------------------------------------------------------------------------------------------------

        public void SetRenderContext(D3D11Adapter_Core adapter_Core)
        {
            Context = adapter_Core ?? throw new ArgumentNullException(nameof(adapter_Core));
            IsActive = true;

            DLogger.Log($"RenderManager.SetRenderContext: Assigned '{adapter_Core.GetType().Name}'");
        }

        public void SetDrawingContext(IDrawingContext drawingContext)
        {
            _drawingContext = drawingContext ?? throw new ArgumentNullException(nameof(drawingContext));
            DLogger.Log("RenderManager.SetDrawingContext: GPU-backed drawing context assigned");
        }

        // -------------------------------------------------------------------------------------------------
        //  SYSTEM REGISTRATION
        // -------------------------------------------------------------------------------------------------

        public void RegisterSystem(IRenderSystem system)
        {
            if (system == null)
                throw new ArgumentNullException(nameof(system));

            if (!_systems.Contains(system))
            {
                _systems.Add(system);
                DLogger.Log($"RenderManager.RegisterSystem: Registered '{system.GetType().FullName}'");
            }
        }

        internal void RegisterSystem(ModernUIRenderer uiRenderer)
        {
            if (uiRenderer == null)
                throw new ArgumentNullException(nameof(uiRenderer));

            RegisterSystem(new FunctionalCallbackBridge(adapter => uiRenderer.Render(adapter)));
        }

        internal void RegisterSystem(HUDRenderer hudRenderer)
        {
            if (hudRenderer == null)
                throw new ArgumentNullException(nameof(hudRenderer));

            RegisterSystem(new FunctionalCallbackBridge(adapter => hudRenderer.Render(adapter)));
        }

        // -------------------------------------------------------------------------------------------------
        //  FRAME EXECUTION
        // -------------------------------------------------------------------------------------------------

        public void RenderAll()
        {
            if (!IsActive || Context == null)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "RenderManager.RenderAll: No render context assigned");
                return;
            }

            BeginFrame();

            for (int i = 0; i < _systems.Count; i++)
            {
                var system = _systems[i];
                if (system == null)
                    continue;

                try
                {
                    system.Render(Context);
                }
                catch (Exception ex)
                {
                    DLogger.Log(
                        LogSubsystems.ResourcesPipeline,
                        $"RenderManager.RenderAll: Exception in '{system.GetType().FullName}': {ex.Message}");
                }
            }

            EndFrame();
        }

        internal void RenderAll(D3D11Adapter_Core renderContext)
        {
            if (renderContext == null)
                throw new ArgumentNullException(nameof(renderContext));

            if (!IsActive)
                return;

            Context = renderContext;

            BeginFrame();

            for (int i = 0; i < _systems.Count; i++)
            {
                var system = _systems[i];
                if (system == null)
                    continue;

                try
                {
                    system.Render(renderContext);
                }
                catch (Exception ex)
                {
                    DLogger.Log(
                        LogSubsystems.ResourcesPipeline,
                        $"RenderManager.RenderAll(D3D11Adapter_Core): Exception in '{system.GetType().FullName}': {ex.Message}");
                }
            }

            EndFrame();
        }

        // -------------------------------------------------------------------------------------------------
        //  FRAME LIFECYCLE
        // -------------------------------------------------------------------------------------------------

        internal void BeginFrame()
        {
            if (!IsActive || Context == null)
                return;

            Context.BeginFrame();
        }

        internal void EndFrame()
        {
            if (!IsActive || Context == null)
                return;

            Context.EndFrame();
            Context.Present();
        }

        // -------------------------------------------------------------------------------------------------
        //  DIAGNOSTICS / SHUTDOWN
        // -------------------------------------------------------------------------------------------------

        public string GetDiagnostics()
        {
            return $"RenderManager: {_systems.Count} systems registered, Active: {IsActive}";
        }

        public void Shutdown()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "RenderManager.Shutdown: ENTER");

            IsActive = false;
            _systems.Clear();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "RenderManager.Shutdown: EXIT");
        }

        // -------------------------------------------------------------------------------------------------
        //  INTERNAL BRIDGE
        // -------------------------------------------------------------------------------------------------

        private class FunctionalCallbackBridge : IRenderSystem
        {
            private readonly Action<D3D11Adapter_Core> _renderAction;

            public FunctionalCallbackBridge(Action<D3D11Adapter_Core> renderAction)
                => _renderAction = renderAction ?? throw new ArgumentNullException(nameof(renderAction));

            public void Render(D3D11Adapter_Core adapter)
            {
                _renderAction(adapter);
            }
        }
    }

    public interface IRenderSystem
    {
        void Render(D3D11Adapter_Core adapter);
    }
}
