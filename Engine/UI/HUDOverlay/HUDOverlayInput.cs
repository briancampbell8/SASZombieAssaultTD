// =====================================================================================================
//  FILE: HUDOverlayInput.cs
//  PATH: Engine/UI/HUDOverlay/HUDOverlayInput.cs
//  SUBSYSTEM: HUDOverlay (Debug Visualization Subsystem)
//  LAYER: UI → HUDOverlay
//
//  ROLE:
//      Handles all input interactions for the HUDOverlay subsystem. This includes hotkey sampling,
//      mouse interaction routing, and forwarding input events to HUDOverlayWindow and HUDOverlayManager.
//      HUDOverlayInput is the ONLY class permitted to read input under the Management‑Only Policy.
//
//  RESPONSIBILITIES:
//      - Sample F4/F5 hotkeys for collapse toggling and page switching.
//      - Provide mouse button and position queries for HUDOverlayWindow dragging logic.
//      - Emit full tracing statements for all input sampling operations.
//      - Maintain strict separation from HUDManager, HUDRenderer, and HUDPanelFinalizer subsystems.
//      - Resolve UIInputRouter exclusively through SystemRegistry.
//
//  NON-RESPONSIBILITIES:
//      - Does NOT modify HUDManager state or textures.
//      - Does NOT perform rendering or window layout logic.
//      - Does NOT access private Finalizer internals or legacy HUDManager fields.
//      - Does NOT persist input data to disk.
//
//  ARCHITECTURE NOTES:
//      - HUDOverlayManager is the authoritative controller; HUDOverlayInput is subordinate.
//      - All subsystem dependencies must be resolved through SystemRegistry.
//      - All tracing uses DLogger.Log with subsystem tag: LogSubsystems.HUDOverlay.
//      - HUDOverlayInput provides input data but does not act on it directly.
//
//  VERSION:
//      Created: July 2026 — Foundational subsystem shell established.
//      Change Log:
//          - Implemented HUDOverlayInput according to Management‑Only Policy.
//          - Added deterministic hotkey sampling and mouse routing.
//          - Added full tracing for all input operations.
//          - Removed all legacy HUDDebugOverlay dependencies and Gemini artifacts.
//          - Established clean Option‑B architecture boundaries.
//          - Updated mouse position typing to use a concrete struct instead of object.
//          - Aligned hotkey sampling with InputKey enum on UIInputRouter.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Input;
using SASZombieAssaultTD.Engine.Systems;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.UI.UIEnums;

namespace SASZombieAssaultTD.Engine.UI.HUDOverlay
{
    internal sealed class HUDOverlayInput
    {
        private readonly SystemRegistry _registry;
        private readonly UIInputRouter? _router;
        /// <summary>
        /// Enumerated list of supported input keys.
        /// </summary>

        public HUDOverlayInput(SystemRegistry registry)
        {
            _registry = registry;
            _router = registry.Get<UIInputRouter>();

            DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Info,
                "HUDOverlayInput: Initialized and bound to UIInputRouter.");
        }

        // -------------------------------------------------------------------------------------------------
        //  HOTKEY SAMPLING
        // -------------------------------------------------------------------------------------------------

        public bool IsCollapseTogglePressed()
        {
            if (_router == null)
                return false;

            bool pressed = _router.IsKeyPressed(InputKey.F4);

            if (pressed)
            {
                DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                    "HUDOverlayInput: F4 pressed → collapse toggle requested.");
            }

            return pressed;
        }

        public bool IsNextPagePressed()
        {
            if (_router == null)
                return false;

            bool pressed = _router.IsKeyPressed(InputKey.F5);

            if (pressed)
            {
                DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                    "HUDOverlayInput: F5 pressed → next page requested.");
            }

            return pressed;
        }

        // -------------------------------------------------------------------------------------------------
        //  MOUSE INPUT
        // -------------------------------------------------------------------------------------------------

        public bool IsMouseDown()
        {
            if (_router == null)
                return false;

            bool down = _router.IsMouseButtonDown(0);

            if (down)
            {
                DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                    "HUDOverlayInput: Mouse button 0 is down.");
            }

            return down;
        }

        public bool IsMouseJustPressed()
        {
            if (_router == null)
                return false;

            bool pressed = _router.IsMouseButtonPressed(0);

            if (pressed)
            {
                DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                    "HUDOverlayInput: Mouse button 0 just pressed.");
            }

            return pressed;
        }

        public (float X, float Y) GetMousePosition()
        {
            if (_router == null)
                return (0f, 0f);

            // UIInputRouter.GetMousePosition() returns Tuple<float,float>, so read Item1/Item2 instead of casting.
            var posTuple = _router.GetMousePosition();
            float x = posTuple.Item1;
            float y = posTuple.Item2;

            DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                $"HUDOverlayInput: Mouse position sampled → X={x}, Y={y}");

            return (x, y);
        }
        // -------------------------------------------------------------------------------------------------
        //  UPDATE (OPTIONAL HOOK)
        // -------------------------------------------------------------------------------------------------

        public void Update(float deltaTime)
        {
            DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                $"HUDOverlayInput: Update cycle executed (dt={deltaTime}).");
        }
    }
}
