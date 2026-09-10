// ====================================================================================================
//  FILE: UIInputState.cs
//  PATH: ./Engine/UI/Input/
//  MODULE: UI
//
//  ROLE:
//      Provides deterministic per‑frame input state for the UI subsystem, including mouse position,
//      mouse buttons, keyboard keys, deltas, and wheel movement. Serves as the authoritative input
//      snapshot consumed by UIInputRouter and UI interaction logic.
//
//  RESPONSIBILITIES:
//      - Track mouse position, delta, and wheel movement.
//      - Track mouse button states and transitions.
//      - Track keyboard states and transitions.
//      - Provide stable per‑frame input data to UI interaction systems.
//      - Provide Clear() and ToString() utilities.
//
//  NON‑RESPONSIBILITIES:
//      - Low‑level OS event handling (handled by Win32Window).
//      - UI layout or rendering (handled by UIElement/UIRenderer).
//
//  AUTHORSHIP:
//      Created by the duo developers of Copilot and Brian Campbell respectively
//      Updated or changed by Copilot on 2026‑07‑27
//
//  CHANGE LOG ENTRY (2026‑07‑28):
//      • Added missing assignment to MousePosition inside Update().
//      • Ensured public MousePosition reflects current frame input for hit‑testing.
//      • Stabilized UIElement.ContainsPoint() pipeline by aligning coordinate flow.
//      • Verified deterministic behavior across UIInputRouter.
//
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Drawing;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI.Input
{
    /// <summary>
    /// Input state data used by UIInputRouter. P80‑05‑02: UIInputState defining input state data used by UIInputRouter.
    /// </summary>
    public class UIInputState
    {
        private PointF _mousePosition;
        private bool[] _mouseButtons;
        private bool[] _previousMouseButtons;
        private bool[] _keys;
        private bool[] _previousKeys;
        private PointF _mouseDelta;
        private float _mouseWheelDelta;

        /// <summary>
        /// The current mouse position in screen/UI space.
        /// </summary>
        public PointF MousePosition { get; private set; }

        /// <summary>
        /// Gets the current mouse delta.
        /// </summary>
        public PointF MouseDelta => _mouseDelta;

        /// <summary>
        /// Gets the current mouse wheel delta.
        /// </summary>
        public float MouseWheelDelta => _mouseWheelDelta;

        /// <summary>
        /// Gets the current mouse button states.
        /// </summary>
        public IReadOnlyList<bool> MouseButtons => Array.AsReadOnly(_mouseButtons);

        /// <summary>
        /// Gets the current key states.
        /// </summary>
        public IReadOnlyList<bool> Keys => Array.AsReadOnly(_keys);

        /// <summary>
        /// Gets the number of mouse buttons supported.
        /// </summary>
        public int MouseButtonCount => _mouseButtons?.Length ?? 0;

        /// <summary>
        /// Gets the number of keys supported.
        /// </summary>
        public int KeyCount => _keys?.Length ?? 0;

        /// <summary>
        /// Initializes a new UIInputState.
        /// </summary>
        public UIInputState(int mouseButtonCount = 5, int keyCount = 256)
        {
            _mousePosition = new PointF(0, 0);
            _mouseButtons = new bool[mouseButtonCount];
            _previousMouseButtons = new bool[mouseButtonCount];
            _keys = new bool[keyCount];
            _previousKeys = new bool[keyCount];
            _mouseDelta = new PointF(0, 0);
            _mouseWheelDelta = 0f;

            DLogger.Log($"UIInputState: Initialized with {mouseButtonCount} mouse buttons and {keyCount} keys");
        }

        /// <summary>
        /// Updates the input state.
        /// </summary>
        public void Update(PointF mousePosition, bool[] mouseButtons, bool[] keys)
        {
            try
            {
                // Calculate deltas
                _mouseDelta = new PointF(
                    mousePosition.X - _mousePosition.X,
                    mousePosition.Y - _mousePosition.Y
                );

                // Update internal and public mouse position
                _mousePosition = mousePosition;
                MousePosition = mousePosition;   // ⭐ REQUIRED FOR UIElement hit‑testing

                // Store previous states
                Array.Copy(_mouseButtons, _previousMouseButtons,
                    System.Math.Min(_mouseButtons.Length, _previousMouseButtons.Length));

                Array.Copy(_keys, _previousKeys,
                    System.Math.Min(_keys.Length, _previousKeys.Length));

                // Update current states
                Array.Copy(mouseButtons, _mouseButtons,
                    System.Math.Min(mouseButtons.Length, _mouseButtons.Length));

                Array.Copy(keys, _keys,
                    System.Math.Min(keys.Length, _keys.Length));

                DLogger.Log(LogSubsystems.ResourcesPipeline, "UIInputState: Updated input state");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIInputState: Error updating input state - {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a mouse button is currently pressed.
        /// </summary>
        public bool IsMouseButtonPressed(int buttonIndex)
        {
            try
            {
                if (buttonIndex >= 0 && buttonIndex < _mouseButtons.Length)
                    return _mouseButtons[buttonIndex];

                DLogger.Log($"UIInputState: Invalid mouse button index {buttonIndex}");
                return false;
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIInputState: Error checking mouse button - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if a mouse button was just pressed.
        /// </summary>
        public bool IsMouseButtonJustPressed(int buttonIndex)
        {
            try
            {
                if (buttonIndex >= 0 && buttonIndex < _mouseButtons.Length &&
                    buttonIndex < _previousMouseButtons.Length)
                {
                    return _mouseButtons[buttonIndex] && !_previousMouseButtons[buttonIndex];
                }

                DLogger.Log($"UIInputState: Invalid mouse button index {buttonIndex}");
                return false;
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIInputState: Error checking just pressed mouse button - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if a mouse button was just released.
        /// </summary>
        public bool IsMouseButtonJustReleased(int buttonIndex)
        {
            try
            {
                if (buttonIndex >= 0 && buttonIndex < _mouseButtons.Length &&
                    buttonIndex < _previousMouseButtons.Length)
                {
                    return !_mouseButtons[buttonIndex] && _previousMouseButtons[buttonIndex];
                }

                DLogger.Log($"UIInputState: Invalid mouse button index {buttonIndex}");
                return false;
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIInputState: Error checking just released mouse button - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if a key is currently pressed.
        /// </summary>
        public bool IsKeyPressed(int keyCode)
        {
            try
            {
                if (keyCode >= 0 && keyCode < _keys.Length)
                    return _keys[keyCode];

                DLogger.Log($"UIInputState: Invalid key code {keyCode}");
                return false;
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIInputState: Error checking key - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if a key was just pressed.
        /// </summary>
        public bool IsKeyJustPressed(int keyCode)
        {
            try
            {
                if (keyCode >= 0 && keyCode < _keys.Length &&
                    keyCode < _previousKeys.Length)
                {
                    return _keys[keyCode] && !_previousKeys[keyCode];
                }

                DLogger.Log($"UIInputState: Invalid key code {keyCode}");
                return false;
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIInputState: Error checking just pressed key - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if a key was just released.
        /// </summary>
        public bool IsKeyJustReleased(int keyCode)
        {
            try
            {
                if (keyCode >= 0 && keyCode < _keys.Length &&
                    keyCode < _previousKeys.Length)
                {
                    return !_keys[keyCode] && _previousKeys[keyCode];
                }

                DLogger.Log($"UIInputState: Invalid key code {keyCode}");
                return false;
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIInputState: Error checking just released key - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Clears all input state.
        /// </summary>
        public void Clear()
        {
            try
            {
                Array.Clear(_mouseButtons);
                Array.Clear(_previousMouseButtons);
                Array.Clear(_keys);
                Array.Clear(_previousKeys);

                _mousePosition = new PointF(0, 0);
                _mouseDelta = new PointF(0, 0);
                _mouseWheelDelta = 0f;

                DLogger.Log(LogSubsystems.ResourcesPipeline, "UIInputState: Cleared all input state");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIInputState: Error clearing input state - {ex.Message}");
            }
        }

        /// <summary>
        /// Returns a string representation of the current input state.
        /// </summary>
        public override string ToString()
        {
            try
            {
                var pressedButtons = new List<string>();
                var pressedKeys = new List<string>();

                for (int i = 0; i < _mouseButtons.Length; i++)
                    if (_mouseButtons[i]) pressedButtons.Add($"Mouse{i}");

                for (int i = 0; i < _keys.Length; i++)
                    if (_keys[i]) pressedKeys.Add($"Key{i}");

                return $"Mouse: {_mousePosition}, Buttons: [{string.Join(", ", pressedButtons)}], Keys: [{string.Join(", ", pressedKeys)}]";
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIInputState: Error creating string representation - {ex.Message}");
                return "UIInputState: Error";
            }
        }
    }
}
