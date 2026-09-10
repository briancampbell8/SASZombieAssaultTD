// =====================================================================================================
//  FILE: UIInputRouter.cs
//  PATH: Engine/Input/UIInputRouter.cs
//  SUBSYSTEM: Core Input System
//
//  ROLE:
//      Deterministic input router for UI and gameplay subsystems. Normalizes and buffers mouse and
//      keyboard events into frame-scoped, queryable states (down, pressed, released) and scroll
//      deltas, providing a clean boundary between platform event loops and engine logic.
//
//  RESPONSIBILITIES:
//      - Track current and previous states for mouse buttons and keyboard keys.
//      - Expose edge-triggered transitions (pressed/released) per clock frame.
//      - Accumulate mouse scroll wheel delta and expose it as a consumable metric.
//      - Provide basic diagnostics about input traffic and tracking coverage.
//      - Offer hooks for UI/HUD input receivers without coupling to platform specifics.
//
//  NON-RESPONSIBILITIES:
//      - Directly reading raw unmanaged Win32 window message queues (delegated to platform loops).
//      - Managing persistence, configuration, or physical file serialization of input mappings.
//      - Implementing high-level gameplay or UI logic; this is a low-level routing and sampling layer.
// =====================================================================================================

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Systems;
using SASZombieAssaultTD.Engine.UI;
using SASZombieAssaultTD.Engine.UI.HUD;

namespace SASZombieAssaultTD.Engine.Input
{
    /// <summary>
    /// Deterministic input router for UI interactions and user input handling.
    /// Provides mouse, keyboard, and scroll support with per-frame transitional states.
    /// </summary>
    public sealed class UIInputRouter
    {
        // --------------------------------------------------------------------------------------------
        //  STATE BUFFERS (MOUSE)
        // --------------------------------------------------------------------------------------------

        private readonly Dictionary<int, bool> _mouseDown = new();
        private readonly Dictionary<int, bool> _mouseDownPrev = new();
        private readonly Dictionary<int, bool> _mousePressedThisFrame = new();
        private readonly Dictionary<int, bool> _mouseReleasedThisFrame = new();

        private float _mouseX = 0f;
        private float _mouseY = 0f;

        // --------------------------------------------------------------------------------------------
        //  STATE BUFFERS (KEYBOARD)
        // --------------------------------------------------------------------------------------------

        private readonly Dictionary<string, bool> _keyDown = new();
        private readonly Dictionary<string, bool> _keyDownPrev = new();
        private readonly Dictionary<string, bool> _keyPressedThisFrame = new();
        private readonly Dictionary<string, bool> _keyReleasedThisFrame = new();

        // --------------------------------------------------------------------------------------------
        //  SCROLL / DIAGNOSTICS / ENGINE INTEGRATION
        // --------------------------------------------------------------------------------------------

        private float _scrollDelta = 0f;
        private int _mouseEventsProcessed = 0;
        private bool _isEnabled = true;

        private readonly SystemRegistry _systemRegistry;
        private HUDManager? _hudManager;
        private bool _isEnabledreturn;

        // --------------------------------------------------------------------------------------------
        //  CONSTRUCTION
        // --------------------------------------------------------------------------------------------

        public UIInputRouter(SystemRegistry systemRegistry)
        {
            _systemRegistry = systemRegistry;
        }

        public UIInputRouter()
        {
            _systemRegistry = (SystemRegistry)SystemRegistry.Instance;
        }

        /// <summary>
        /// Gets whether input routing is currently enabled.
        /// </summary>
        public bool IsEnabled => _isEnabled;

        // --------------------------------------------------------------------------------------------
        //  PUBLIC INPUT QUERIES (SCROLL)
        // --------------------------------------------------------------------------------------------

        public float GetScrollDelta()
        {
            if (!_isEnabled)
                return 0f;

            var delta = _scrollDelta;
            _scrollDelta = 0f;
            return delta;
        }

        // --------------------------------------------------------------------------------------------
        //  PUBLIC INPUT QUERIES (MOUSE)
        // --------------------------------------------------------------------------------------------

        public bool IsMouseButtonDown(int button)
        {
            if (!_isEnabled)
                return false;

            return _mouseDown.TryGetValue(button, out var isDown) && isDown;
        }

        public bool IsMouseButtonPressed(int button)
        {
            if (!_isEnabled)
                return false;

            return _mousePressedThisFrame.TryGetValue(button, out var pressed) && pressed;
        }

        public bool IsMouseButtonReleased(int button)
        {
            if (!_isEnabled)
                return false;

            return _mouseReleasedThisFrame.TryGetValue(button, out var released) && released;
        }

        public Tuple<float, float> GetMousePosition()
        {
            return Tuple.Create(_mouseX, _mouseY);
        }

        // --------------------------------------------------------------------------------------------
        //  PUBLIC INPUT QUERIES (KEYBOARD)
        // --------------------------------------------------------------------------------------------

        public bool IsKeyDown(string key)
        {
            if (!_isEnabled)
                return false;

            return _keyDown.TryGetValue(key, out var isDown) && isDown;
        }

        internal void Shutdown()
        {
            throw new NotImplementedException();
        }

        internal void Update(float deltaTime)
        {
            throw new NotImplementedException();
        }

        internal bool IsKeyPressed(UIEnums.InputKey f4)
        {
            throw new NotImplementedException();
        }

        internal object GetStats()
        {
            throw new NotImplementedException();
        }

        internal bool IsKeyReleased(string keyName)
        {
            throw new NotImplementedException();
        }

        internal bool IsKeyPressed(string keyName)
        {
            throw new NotImplementedException();
        }
    }
}
