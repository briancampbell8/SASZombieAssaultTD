// =====================================================================================================
//  FILE: InputSystem.cs
//  PATH: ./Engine/Input/
//  SUBSYSTEM: Core Input System
//
//  ROLE:
//      Concrete engine subsystem that provides deterministic, high-level input queries for gameplay,
//      simulation, and core engine logic. InputSystem does not track raw input itself; instead, it
//      consumes transitional and state data produced by UIInputRouter and exposes a stable façade
//      for the rest of the engine.
//
//  RESPONSIBILITIES:
//      - Expose IsKeyPressed / IsKeyJustPressed / IsKeyJustReleased queries.
//      - Expose IsMouseButtonPressed / IsMouseButtonJustPressed / IsMouseButtonJustReleased queries.
//      - Provide GetMousePosition / GetMouseDelta for gameplay and UI logic.
//      - Provide GetScrollDelta for scroll-based interactions.
//      - Maintain per-frame mouse delta tracking.
//      - Integrate cleanly with SystemRegistry via IEngineSubsystem.
//      - Act as a compatibility layer for older engine code that previously relied on static input.
//
//  NON-RESPONSIBILITIES:
//      - Polling OS-level input events (delegated to platform input loops).
//      - Tracking raw input states (delegated to UIInputRouter).
//      - Performing UI-specific routing (delegated to UIManager/UIElement).
//      - Managing input mappings or gesture recognition.
//
//  NOTES:
//      This subsystem is intentionally thin. All deterministic input tracking lives inside
//      UIInputRouter. InputSystem simply exposes a stable façade for gameplay and engine systems.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Input
{
    /// <summary>
    /// Deterministic input subsystem providing high-level input queries backed by UIInputRouter.
    /// </summary>
    public sealed class InputSystem : IEngineSubsystem
    {
        private readonly UIInputRouter _router;

        private Vector3 _mousePositionPrev = Vector3.Zero;
        private Vector3 _mousePositionCurr = Vector3.Zero;

        /// <summary>
        /// Constructs the InputSystem and binds it to the engine's UIInputRouter.
        /// </summary>
        public InputSystem(UIInputRouter router)
        {
            _router = router ?? throw new ArgumentNullException(nameof(router));
        }

        // =====================================================================================================
        //  KEYBOARD QUERIES
        // =====================================================================================================

        public bool IsKeyPressed(string keyName)
        {
            if (string.IsNullOrEmpty(keyName))
                return false;

            return _router.IsKeyDown(keyName);
        }

        public bool IsKeyJustPressed(string keyName)
        {
            if (string.IsNullOrEmpty(keyName))
                return false;

            return _router.IsKeyPressed(keyName);
        }

        public bool IsKeyJustReleased(string keyName)
        {
            if (string.IsNullOrEmpty(keyName))
                return false;

            return _router.IsKeyReleased(keyName);
        }

        // =====================================================================================================
        //  MOUSE QUERIES
        // =====================================================================================================

        public bool IsMouseButtonPressed(int button)
        {
            return _router.IsMouseButtonDown(button);
        }

        public bool IsMouseButtonJustPressed(int button)
        {
            return _router.IsMouseButtonPressed(button);
        }

        public bool IsMouseButtonJustReleased(int button)
        {
            return _router.IsMouseButtonReleased(button);
        }

        public Vector3 GetMousePosition()
        {
            return _mousePositionCurr;
        }

        public Vector3 GetMouseDelta()
        {
            return new Vector3(
                _mousePositionCurr.X - _mousePositionPrev.X,
                _mousePositionCurr.Y - _mousePositionPrev.Y,
                0f
            );
        }

        public float GetScrollDelta()
        {
            return _router.GetScrollDelta();
        }

        // =====================================================================================================
        //  UPDATE / CLEAR
        // =====================================================================================================

        public void Update(float deltaTime)
        {
            _mousePositionPrev = _mousePositionCurr;

            var pos = _router.GetMousePosition();
            if (pos is Tuple<float, float> tuple)
            {
                _mousePositionCurr = new Vector3(tuple.Item1, tuple.Item2, 0f);
            }
        }

        public void Clear()
        {
            _mousePositionPrev = Vector3.Zero;
            _mousePositionCurr = Vector3.Zero;
        }

        // =====================================================================================================
        //  UTILITY
        // =====================================================================================================

        public string[] GetPressedKeys()
        {
            // Router only exposes bool[] for now; names require router enhancement.
            return Array.Empty<string>();
        }

        public InputRouterStats GetStats()
        {
            return (InputRouterStats)_router.GetStats();
        }

        internal static bool IsKeyPressed(KeyCode escape)
        {
            throw new NotImplementedException();
        }
    }
}
