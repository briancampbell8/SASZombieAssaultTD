// =====================================================================================================
//  FILE: HUDOverlayDiagnostics.cs
//  PATH: Engine/UI/HUDOverlay/HUDOverlayDiagnostics.cs
//  SUBSYSTEM: HUDOverlay (Debug Visualization Subsystem)
//  LAYER: UI → HUDOverlay
//
//  ROLE:
//      Provides structured diagnostic data for HUDOverlay pages. This module collects, formats,
//      and exposes Finalizer metrics, crosshair geometry, render‑order hierarchy, viewport bounds,
//      and engine telemetry. HUDOverlayDiagnostics does not render or manage window state.
//
//  RESPONSIBILITIES:
//      - Query subsystem state through SystemRegistry.
//      - Provide formatted diagnostic strings for HUDOverlayPages.
//      - Emit full tracing statements for all diagnostic retrieval operations.
//      - Maintain strict separation from HUDManager, HUDRenderer, and HUDPanelFinalizer internals.
//      - Operate deterministically without side effects.
//
//  NON-RESPONSIBILITIES:
//      - Does NOT modify HUDManager state or textures.
//      - Does NOT perform rendering or input handling.
//      - Does NOT access private Finalizer internals or legacy HUDManager fields.
//      - Does NOT persist diagnostic data to disk.
//
//  ARCHITECTURE NOTES:
//      - HUDOverlayManager is the authoritative controller; diagnostics are subordinate.
//      - All subsystem dependencies must be resolved through SystemRegistry.
//      - All tracing uses DLogger.Log with subsystem tag: LogSubsystems.HUDOverlay.
//      - Diagnostic data is formatted but never cached; always retrieved fresh.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Input;
using SASZombieAssaultTD.Engine.Systems;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI.HUDOverlay
{
    /// <summary>
    /// Provides formatted diagnostic data for HUDOverlayPages.
    /// </summary>
    internal sealed class HUDOverlayDiagnostics
    {
        private readonly SystemRegistry _registry;

        public HUDOverlayDiagnostics(SystemRegistry registry)
        {
            _registry = registry;

            DLogger.Log(LogSubsystems.HUDOverlay, LogLevel.Info,
                "HUDOverlayDiagnostics: Initialized and ready for diagnostic retrieval.");
        }

        // -------------------------------------------------------------------------------------------------
        //  FINALIZER PAGE
        // -------------------------------------------------------------------------------------------------

        public string[] GetFinalizerDiagnostics()
        {
            DLogger.Log(LogSubsystems.HUDOverlay, LogLevel.Trace,
                "HUDOverlayDiagnostics: Retrieving Finalizer diagnostics.");

            var finalizer = _registry.Get<HUDPanelFinalizer_Manager>();

            if (finalizer == null)
            {
                return new[]
                {
                    "Finalizer subsystem unavailable.",
                    "No diagnostic data available."
                };
            }

            var element = finalizer.GetActiveElement();
            var manualColors = finalizer.UseManualPanelColors;

            return new[]
            {
                "--- FINALIZER PIPELINE METRICS ---",
                $"Manual Color Override: {manualColors}",
                $"Active Element ID: {element?.Id ?? "Null"}",
                $"Anchor Panel: {element?.AnchorPanelId ?? "Null"}"
            };
        }

        // -------------------------------------------------------------------------------------------------
        //  CROSSHAIR PAGE
        // -------------------------------------------------------------------------------------------------

        public string[] GetCrosshairDiagnostics()
        {
            DLogger.Log(LogSubsystems.HUDOverlay, LogLevel.Trace,
                "HUDOverlayDiagnostics: Retrieving Crosshair diagnostics.");

            var finalizer = _registry.Get<HUDPanelFinalizer_Manager>();
            var state = finalizer?.GetResolvedState();

            if (state == null)
            {
                return new[]
                {
                    "--- CROSSHAIR ANCHOR GEOMETRY ---",
                    "No resolved crosshair state available."
                };
            }

            return new[]
            {
                "--- CROSSHAIR ANCHOR GEOMETRY ---",
                $"Center: X={state.CrosshairCenterX}, Y={state.CrosshairCenterY}",
                $"Arm Length: {state.CrosshairArmLength}px",
                $"Thickness: {state.CrosshairThickness}px"
            };
        }

        // -------------------------------------------------------------------------------------------------
        //  RENDER ORDER PAGE
        // -------------------------------------------------------------------------------------------------

        public string[] GetRenderOrderDiagnostics()
        {
            DLogger.Log(LogSubsystems.HUDOverlay, LogLevel.Trace,
                "HUDOverlayDiagnostics: Retrieving RenderOrder diagnostics.");

            var finalizer = _registry.Get<HUDPanelFinalizer_Manager>();

            return new[]
            {
                "--- ACTIVE HIERARCHICAL LAYERS ---",
                $"Finalizer Layer: {finalizer?.Layer ?? 0}",
                "0: [CASH_PANEL] Layer: 999 (Authoritative)"
            };
        }

        // -------------------------------------------------------------------------------------------------
        //  HUD BOUNDS PAGE
        // -------------------------------------------------------------------------------------------------

        public string[] GetHUDBoundsDiagnostics()
        {
            DLogger.Log(LogSubsystems.HUDOverlay, LogLevel.Trace,
                "HUDOverlayDiagnostics: Retrieving HUDBounds diagnostics.");

            var finalizer = _registry.Get<HUDPanelFinalizer_Manager>();
            var panel = finalizer?.GetCashPanel();

            if (panel == null)
            {
                return new[]
                {
                    "--- SPATIAL VIEWPORT BOUNDARIES ---",
                    "CashPanel unavailable."
                };
            }

            return new[]
            {
                "--- SPATIAL VIEWPORT BOUNDARIES ---",
                $"Position: X={panel.X}, Y={panel.Y}",
                $"Size: W={panel.Width}, H={panel.Height}"
            };
        }

        // -------------------------------------------------------------------------------------------------
        //  ENGINE STATS PAGE
        // -------------------------------------------------------------------------------------------------

        public string[] GetEngineStatsDiagnostics()
        {
            DLogger.Log(LogSubsystems.HUDOverlay, LogLevel.Trace,
                "HUDOverlayDiagnostics: Retrieving EngineStats diagnostics.");

            var router = _registry.Get<UIInputRouter>();
            InputRouterStats stats = (InputRouterStats)(router?.GetStats());

            if (stats == null)
            {
                return new[]
                {
                    "--- SIMULATION HOST TELEMETRY ---",
                    "InputRouter unavailable."
                };
            }

            return new[]
            {
                "--- SIMULATION HOST TELEMETRY ---",
                $"Keys Buffered: {stats.KeysTracked}",
                $"Mouse Events: {stats.MouseEventsProcessed}",
                $"Router Enabled: {stats.IsEnabled}"
            };
        }
    }
}
