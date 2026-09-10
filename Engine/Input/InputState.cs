// =====================================================================================================
//  FILE: InputState.cs
//  PATH: Engine/Input/InputState.cs
//  SUBSYSTEM: Input Subsystem / Core Components
//
//  ROLE:
//      Immutable, deterministic per-frame snapshot of interpreted input states used by UIManager,
//      UIElement, and HUD subsystems. This structure is populated by the engine’s routing layer
//      (UIInputRouter) and consumed by UI logic for hit-testing, hover detection, click routing,
//      focus transitions, and scroll-based interactions.
//
//  RESPONSIBILITIES:
//      - Store mouse position, button states, and click flags for the current frame.
//      - Store keyboard down/pressed states and modifier keys.
//      - Store scroll wheel delta for UI scrolling behaviors.
//      - Provide a stable, device-agnostic input packet for UIElement.HandleInput().
//      - Enable deterministic UI interaction logic across menus, HUD panels, and UI subsystems.
//
//  NON-RESPONSIBILITIES:
//      - Performing transitional state detection (handled by UIInputRouter).
//      - Polling OS-level input events or device drivers.
//      - Managing UI hierarchy, rendering, or layout logic.
//      - Persisting input history or performing gesture recognition.
//
//  NOTES:
//      This structure is intentionally simple and free of logic. It represents the final interpreted
//      input state for a single engine frame and is safe to pass across subsystem boundaries.
// =====================================================================================================

using System.Collections.Generic;
using SASZombieAssaultTD.Engine.UI.Components;

namespace SASZombieAssaultTD.Engine.Input
{
    /// <summary>
    /// Deterministic snapshot of input state for the current frame.
    /// Populated by UIInputRouter and consumed by UI systems.
    /// </summary>
    public class InputState
    {
        // ---------------------------------------------------------------------------------------------
        // Mouse State
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Mouse X coordinate in viewport or screen space.
        /// </summary>
        public float MouseX { get; set; }

        /// <summary>
        /// Mouse Y coordinate in viewport or screen space.
        /// </summary>
        public float MouseY { get; set; }

        /// <summary>
        /// True if the left mouse button is currently down.
        /// </summary>
        public bool LeftMouseDown { get; set; }

        /// <summary>
        /// True if the right mouse button is currently down.
        /// </summary>
        public bool RightMouseDown { get; set; }

        /// <summary>
        /// True if the middle mouse button is currently down.
        /// </summary>
        public bool MiddleMouseDown { get; set; }

        /// <summary>
        /// True if the left mouse button was clicked this frame.
        /// </summary>
        public bool LeftMouseClicked { get; set; }

        /// <summary>
        /// True if the right mouse button was clicked this frame.
        /// </summary>
        public bool RightMouseClicked { get; set; }

        /// <summary>
        /// True if the middle mouse button was clicked this frame.
        /// </summary>
        public bool MiddleMouseClicked { get; set; }

        /// <summary>
        /// Scroll wheel delta accumulated for this frame.
        /// </summary>
        public float ScrollDelta { get; set; }

        // ---------------------------------------------------------------------------------------------
        // Keyboard State
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Keys currently held down during this frame.
        /// </summary>
        public HashSet<string> KeysDown { get; } = new();

        /// <summary>
        /// Keys that were pressed (edge-triggered) during this frame.
        /// </summary>
        public HashSet<string> KeysPressed { get; } = new();

        // ---------------------------------------------------------------------------------------------
        // Modifier Keys
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// True if Shift is currently held.
        /// </summary>
        public bool Shift { get; set; }

        /// <summary>
        /// True if Ctrl is currently held.
        /// </summary>
        public bool Ctrl { get; set; }

        /// <summary>
        /// True if Alt is currently held.
        /// </summary>
        public bool Alt { get; set; }

        // ---------------------------------------------------------------------------------------------
        // Utility
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Returns true if the given key is currently held down.
        /// </summary>
        public bool IsKeyDown(string key) => KeysDown.Contains(key);

        /// <summary>
        /// Returns true if the given key was pressed this frame.
        /// </summary>
        public bool IsKeyPressed(string key) => KeysPressed.Contains(key);

        /// <summary>
        /// Returns the mouse position as a PointF structure.
        /// </summary>
        public PointF MousePoint => new PointF(MouseX, MouseY);
    }
}
