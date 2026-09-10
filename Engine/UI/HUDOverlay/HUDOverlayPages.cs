// =====================================================================================================
//  FILE: HUDOverlayPages.cs
//  PATH: Engine/UI/HUDOverlay/HUDOverlayPages.cs
//  SUBSYSTEM: HUDOverlay (Debug Visualization Subsystem)
//  LAYER: UI → HUDOverlay
//
//  ROLE:
//      Provides deterministic page routing for the HUDOverlay subsystem. HUDOverlayPages selects,
//      maintains, and retrieves diagnostic content for the active debug page. All diagnostic data is
//      sourced exclusively through HUDOverlayDiagnostics.
//
//  RESPONSIBILITIES:
//      - Maintain the active HUDOverlay debug page.
//      - Provide formatted diagnostic content for the renderer.
//      - Emit full tracing statements for all page transitions and diagnostic retrieval operations.
//      - Maintain strict separation from HUDManager, HUDRenderer, and HUDPanelFinalizer internals.
//      - Resolve diagnostic data exclusively through HUDOverlayDiagnostics.
//
//  NON-RESPONSIBILITIES:
//      - Does NOT perform rendering or window layout logic.
//      - Does NOT handle input or mouse interactions.
//      - Does NOT modify HUDManager or Finalizer state.
//      - Does NOT persist diagnostic data to disk.
//
//  ARCHITECTURE NOTES:
//      - HUDOverlayManager is the authoritative controller; HUDOverlayPages is subordinate.
//      - HUDOverlayDiagnostics provides all diagnostic content.
//      - All tracing uses DLogger.Log with subsystem tag: LogSubsystems.HUDOverlay.
//      - Page routing is deterministic and cyclic.
//
//  VERSION:
//      Created: July 2026 — Foundational subsystem shell established.
//      Change Log:
//          - Implemented HUDOverlayPages according to Management‑Only Policy.
//          - Added deterministic page routing and diagnostic retrieval.
//          - Added full tracing for all page transitions.
//          - Removed all legacy HUDDebugOverlay dependencies and Gemini artifacts.
//          - Established clean Option‑B architecture boundaries.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Systems;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.UI.UIEnums;

namespace SASZombieAssaultTD.Engine.UI.HUDOverlay
{
    /// <summary>
    /// Provides deterministic page routing and diagnostic retrieval for HUDOverlay.
    /// </summary>
    internal sealed class HUDOverlayPages
    {


        private OverlayPage _currentPage = OverlayPage.Finalizer;

        private readonly HUDOverlayDiagnostics _diagnostics;

        // -------------------------------------------------------------------------------------------------
        //  CONSTRUCTOR
        // -------------------------------------------------------------------------------------------------

        public HUDOverlayPages(SystemRegistry registry)
        {
            _diagnostics = new HUDOverlayDiagnostics(registry);

            DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Info,
                "HUDOverlayPages: Initialized with default page Finalizer.");
        }

        // -------------------------------------------------------------------------------------------------
        //  PUBLIC API — PAGE MANAGEMENT
        // -------------------------------------------------------------------------------------------------

        public OverlayPage CurrentPage => _currentPage;

        public void NextPage()
        {
            var oldPage = _currentPage;

            _currentPage = _currentPage switch
            {
                OverlayPage.Finalizer => OverlayPage.Crosshair,
                OverlayPage.Crosshair => OverlayPage.RenderOrder,
                OverlayPage.RenderOrder => OverlayPage.HUDBounds,
                OverlayPage.HUDBounds => OverlayPage.EngineStats,
                OverlayPage.EngineStats => OverlayPage.Finalizer,
                _ => OverlayPage.Finalizer
            };

            DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                $"HUDOverlayPages: Page changed → {oldPage} → {_currentPage}");
        }

        // -------------------------------------------------------------------------------------------------
        //  DIAGNOSTIC RETRIEVAL
        // -------------------------------------------------------------------------------------------------

        public string[] GetPageDiagnostics()
        {
            DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                $"HUDOverlayPages: Retrieving diagnostics for page {_currentPage}");

            return _currentPage switch
            {
                OverlayPage.Finalizer => _diagnostics.GetFinalizerDiagnostics(),
                OverlayPage.Crosshair => _diagnostics.GetCrosshairDiagnostics(),
                OverlayPage.RenderOrder => _diagnostics.GetRenderOrderDiagnostics(),
                OverlayPage.HUDBounds => _diagnostics.GetHUDBoundsDiagnostics(),
                OverlayPage.EngineStats => _diagnostics.GetEngineStatsDiagnostics(),
                _ => new[] { "Unknown page." }
            };
        }

        // -------------------------------------------------------------------------------------------------
        //  UPDATE HOOK (OPTIONAL)
        // -------------------------------------------------------------------------------------------------

        public void Update(float deltaTime)
        {
            // Reserved for future page‑specific update logic.

            DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                $"HUDOverlayPages: Update cycle executed (dt={deltaTime}).");
        }
    }
}
