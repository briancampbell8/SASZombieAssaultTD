// =====================================================================================================
//  FILE: HUDManagerAttachment.cs
//  PATH: Engine/GameRoot/HUDManagerAttachment.cs
//  SUBSYSTEM: GameRoot Initialization
//
//  ROLE:
//      Wires HUD subsystems into the deterministic GPU render pipeline. HUDManager provides HUD state
//      and texture data, while HUDRenderer is the subsystem that participates in the render loop.
//      ModernUIRenderer consumes HUDManager state for UI composition.
//
//  RESPONSIBILITIES:
//      - Resolve HUDManager, HUDRenderer, ModernUIRenderer, and RenderManager from SystemRegistry.
//      - Register HUDRenderer with RenderManager (HUDManager is NOT a render system).
//      - Bind HUDManager to ModernUIRenderer for HUD state access.
//      - Maintain deterministic bootstrap ordering for HUD subsystem integration.
//      - Emit diagnostic logs confirming HUD pipeline attachment.
//
//  NON-RESPONSIBILITIES:
//      - HUD rendering (performed by HUDRenderer).
//      - HUD panel/finalizer logic (HUDPanelFinalizer_Manager).
//      - HUD configuration (HUDConfigManager).
//      - UI rendering (ModernUIRenderer handles this).
//
//  ARCHITECTURAL NOTES:
//      - Fully aligned with Option‑B deterministic architecture.
//      - HUDManager is a pure state/texture provider and never registered as a render system.
//      - HUDRenderer is the authoritative HUD render subsystem.
//      - ModernUIRenderer consumes HUDManager state but does not register HUDManager itself.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Systems;
using SASZombieAssaultTD.Engine.TextureRendering.HUD;
using SASZombieAssaultTD.Engine.UI.HUD;
using SASZombieAssaultTD.Engine.UI.Rendering.Modern;

namespace SASZombieAssaultTD.Engine.GameRoot
{
    internal static class HUDManagerAttachment
    {
        public static void Attach(SystemRegistry registry)
        {
            // ---------------------------------------------------------------------------------------------
            // Resolve core subsystems
            // ---------------------------------------------------------------------------------------------
            var renderManager = registry.Get<RenderManager>();
            var modernUiRenderer = registry.Get<ModernUIRenderer>();
            var hudManager = registry.Get<HUDManager>();
            var hudRenderer = registry.Get<HUDRenderer>();

            if (renderManager == null)
                throw new InvalidOperationException("HUDManagerAttachment: RenderManager not registered.");

            if (modernUiRenderer == null)
                throw new InvalidOperationException("HUDManagerAttachment: ModernUIRenderer not registered.");

            if (hudManager == null)
                throw new InvalidOperationException("HUDManagerAttachment: HUDManager not registered.");

            if (hudRenderer == null)
                throw new InvalidOperationException("HUDManagerAttachment: HUDRenderer not registered.");

            // ---------------------------------------------------------------------------------------------
            // Register HUDRenderer into the deterministic render pipeline
            // (HUDManager is NOT a render system — HUDRenderer consumes HUDManager state)
            // ---------------------------------------------------------------------------------------------
            renderManager.RegisterSystem(hudRenderer);

            // ---------------------------------------------------------------------------------------------
            // Bind HUDManager to ModernUIRenderer (state only)
            // ---------------------------------------------------------------------------------------------
            modernUiRenderer.AttachHUDManager(hudManager);

            // ---------------------------------------------------------------------------------------------
            // Diagnostics
            // ---------------------------------------------------------------------------------------------
            DLogger.Log(
                "HUDManagerAttachment: HUDRenderer attached to RenderManager; HUDManager bound to ModernUIRenderer.");
        }
    }
}
