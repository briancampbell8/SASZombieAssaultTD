// =====================================================================================================
//  FILE: HUDOverlayManager.cs
//  PATH: Engine/UI/HUDOverlay/HUDOverlayManager.cs
//  SUBSYSTEM: HUDOverlay (Debug Visualization Subsystem)
//  LAYER: UI → HUDOverlay
//
//  ROLE:
//      Authoritative controller for the HUDOverlay subsystem. Coordinates initialization, input routing,
//      diagnostic page management, window state, and rendering delegation. All HUDOverlay components
//      operate under this manager following the Management‑Only Policy.
//
//  RESPONSIBILITIES:
//      - Initialize and maintain subordinate HUDOverlay components (Window, Pages, Input, Renderer).
//      - Provide deterministic update and render sequencing.
//      - Expose public API for page switching, collapse toggling, and overlay visibility.
//      - Emit full tracing statements for all subsystem interactions, state transitions,
//        initialization paths, update cycles, and rendering passes.
//      - Resolve subsystem dependencies exclusively through SystemRegistry.
//
//  NON-RESPONSIBILITIES:
//      - Does NOT modify HUDManager state or textures.
//      - Does NOT participate in gameplay logic or rendering pipelines outside HUDOverlayRenderer.
//      - Does NOT access private Finalizer internals or legacy HUDManager fields.
//      - Does NOT persist debug data to disk.
//
//  ARCHITECTURE NOTES:
//      - HUDOverlayManager is the root coordinator for all HUDOverlay components.
//      - Subordinate classes: HUDOverlayWindow, HUDOverlayPages, HUDOverlayInput, HUDOverlayRenderer.
//      - No HUDOverlay class may directly reference HUDManager.Instance.
//      - All subsystem dependencies must be resolved through SystemRegistry.
//      - All tracing uses DLogger.Log with subsystem tag: LogSubsystems.HUDOverlay.
//
//  VERSION:
//      Created: July 2026 — Foundational subsystem shell established.
//      Change Log:
//          - Implemented full HUDOverlayManager according to Management‑Only Policy.
//          - Added deterministic initialization, update, and render sequencing.
//          - Added full tracing for all subsystem interactions.
//          - Removed all legacy HUDDebugOverlay dependencies and Gemini artifacts.
//          - Established clean Option‑B architecture boundaries.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;

// using SASZombieAssaultTD.Engine.Extensions; // Extensions Removed
using SASZombieAssaultTD.Engine.Systems;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI.HUDOverlay
{
    /// <summary>
    /// Root controller for the HUDOverlay subsystem. All subordinate HUDOverlay components
    /// are initialized, updated, and rendered exclusively through this manager.
    /// </summary>
    internal sealed class HUDOverlayManager
    {
        // -------------------------------------------------------------------------------------------------
        //  SUBSYSTEM COMPONENTS
        // -------------------------------------------------------------------------------------------------

        private HUDOverlayWindow _window;
        private HUDOverlayPages _pages;
        private HUDOverlayInput _input;
        private HUDOverlayRenderer _renderer;

        private readonly SystemRegistry _registry;

        // Overlay visibility state
        private bool _isVisible = true;

        // -------------------------------------------------------------------------------------------------
        //  CONSTRUCTOR
        // -------------------------------------------------------------------------------------------------

        public HUDOverlayManager(SystemRegistry registry)
        {
            _registry = registry;

            DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Info,
                "HUDOverlayManager: Initializing subsystem components.");

            // Initialize subordinate components
            _window = new HUDOverlayWindow();
            _pages = new HUDOverlayPages(registry);
            _input = new HUDOverlayInput(registry);
            _renderer = new HUDOverlayRenderer(_window, _pages);

            DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Info,
                "HUDOverlayManager: Subsystem components initialized successfully.");
        }

        // -------------------------------------------------------------------------------------------------
        //  PUBLIC API — MANAGEMENT ONLY
        // -------------------------------------------------------------------------------------------------

        public void SetVisible(bool visible)
        {
            _isVisible = visible;

            DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                $"HUDOverlayManager: Visibility changed → {visible}");
        }

        public void ToggleCollapse()
        {
            _window.ToggleCollapse();

            DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                "HUDOverlayManager: Collapse toggled.");
        }

        public void NextPage()
        {
            _pages.NextPage();

            DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                $"HUDOverlayManager: Page switched → {_pages.CurrentPage}");
        }

        // -------------------------------------------------------------------------------------------------
        //  UPDATE SEQUENCE
        // -------------------------------------------------------------------------------------------------

        public void Update(float deltaTime)
        {
            if (!_isVisible)
                return;

            DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                "HUDOverlayManager: Update cycle started.");

            _input.Update(deltaTime);
            _window.Update(deltaTime);
            _pages.Update(deltaTime);

            DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                "HUDOverlayManager: Update cycle completed.");
        }

        // -------------------------------------------------------------------------------------------------
        //  RENDER SEQUENCE
        // -------------------------------------------------------------------------------------------------

        public void Draw(D3D11Adapter_Core context)
        {
            if (!_isVisible)
                return;

            DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                "HUDOverlayManager: Render pass started.");

            _renderer.Draw(context);

            DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                "HUDOverlayManager: Render pass completed.");
        }
    }
}
