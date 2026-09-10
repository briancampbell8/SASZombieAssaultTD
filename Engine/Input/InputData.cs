// ====================================================================================================
//  FILE: InputData.cs
//  PATH: ./Engine/Input/
//  MODULE: Core Input Snapshot
//
//  ROLE:
//      Provides a per-frame, immutable snapshot of interpreted input states used by gameplay,
//      UI, and simulation subsystems. This structure is populated by the engine’s input routing
//      layer (UIInputRouter, PlatformInputLoop, etc.) and consumed by higher-level systems.
//
//  RESPONSIBILITIES:
//      - Store semantic, device-agnostic input states (movement, selection, confirmation, etc.).
//      - Store raw mouse metrics (position, delta, wheel movement).
//      - Provide a stable, deterministic input snapshot for the current frame.
//      - Serve as the canonical input payload passed into ECS systems or gameplay logic.
//
//  NON-RESPONSIBILITIES:
//      - Reading raw platform events (delegated to input routers).
//      - Performing input mapping, debouncing, or transitional state detection.
//      - Persisting or serializing input data.
//
//  NOTES:
//      This structure is intentionally simple and free of logic. It is a pure data container
//      representing the final interpreted input state for a single engine frame.
// ====================================================================================================

using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine
{
    /// <summary>
    /// Immutable per-frame input snapshot containing mouse, keyboard, and semantic action states.
    /// Populated by the engine’s input routing layer.
    /// </summary>
    public class InputData
    {
        // --------------------------------------------------------------------------------------------
        //  MOUSE METRICS
        // --------------------------------------------------------------------------------------------

        /// <summary>
        /// Mouse position in screen or viewport coordinates.
        /// </summary>
        public Vector3 MousePosition { get; set; }

        /// <summary>
        /// Mouse movement delta since last frame.
        /// </summary>
        public Vector3 MouseDelta { get; set; }

        /// <summary>
        /// Scroll wheel delta accumulated for this frame.
        /// </summary>
        public float MouseWheelDelta { get; set; }

        // --------------------------------------------------------------------------------------------
        //  SEMANTIC ACTIONS
        // --------------------------------------------------------------------------------------------

        /// <summary>
        /// Indicates that the player performed a semantic “select” action this frame.
        /// This is device-agnostic and may originate from mouse, keyboard, or controller.
        /// </summary>
        public bool IsSelectPressed { get; set; }

        // --------------------------------------------------------------------------------------------
        //  RAW BUTTON STATES (MOUSE)
        // --------------------------------------------------------------------------------------------

        /// <summary>
        /// Whether the left mouse button is currently pressed.
        /// </summary>
        public bool LeftMousePressed { get; set; }

        /// <summary>
        /// Whether the right mouse button is currently pressed.
        /// </summary>
        public bool RightMousePressed { get; set; }

        /// <summary>
        /// Whether the middle mouse button is currently pressed.
        /// </summary>
        public bool MiddleMousePressed { get; set; }

        // --------------------------------------------------------------------------------------------
        //  RAW BUTTON STATES (KEYBOARD)
        // --------------------------------------------------------------------------------------------

        /// <summary>
        /// Whether up arrow or W key is pressed.
        /// </summary>
        public bool IsUpPressed { get; set; }

        /// <summary>
        /// Whether down arrow or S key is pressed.
        /// </summary>
        public bool IsDownPressed { get; set; }

        /// <summary>
        /// Whether left arrow or A key is pressed.
        /// </summary>
        public bool IsLeftPressed { get; set; }

        /// <summary>
        /// Whether right arrow or D key is pressed.
        /// </summary>
        public bool IsRightPressed { get; set; }

        /// <summary>
        /// Whether Enter key is pressed.
        /// </summary>
        public bool IsEnterPressed { get; set; }

        /// <summary>
        /// Whether Escape key is pressed.
        /// </summary>
        public bool IsEscapePressed { get; set; }

        /// <summary>
        /// Whether Space key is pressed.
        /// </summary>
        public bool IsSpacePressed { get; set; }

        /// <summary>
        /// Whether F5 key is pressed.
        /// </summary>
        public bool F5Pressed { get; set; }

        /// <summary>
        /// Whether any key is pressed.
        /// </summary>
        public bool AnyKeyPressed { get; set; }

        // --------------------------------------------------------------------------------------------
        //  CONSTRUCTION
        // --------------------------------------------------------------------------------------------

        /// <summary>
        /// Creates a new InputData instance with default (unpressed) states.
        /// </summary>
        public InputData()
        {
            MousePosition = Vector3.Zero;
            MouseDelta = Vector3.Zero;
            MouseWheelDelta = 0f;

            LeftMousePressed = false;
            RightMousePressed = false;
            MiddleMousePressed = false;

            IsUpPressed = false;
            IsDownPressed = false;
            IsLeftPressed = false;
            IsRightPressed = false;

            IsEnterPressed = false;
            IsEscapePressed = false;
            IsSpacePressed = false;
            F5Pressed = false;

            AnyKeyPressed = false;
            IsSelectPressed = false;
        }
    }
}
