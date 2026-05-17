using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Input
{
    /// <summary>
    /// Input system for SAS Zombie Assault TD.
    /// Provides keyboard and mouse input state tracking.
    /// </summary>
    public static class InputSystem
    {
        private static readonly Dictionary<string, bool> _keyStates = new Dictionary<string, bool>();
        private static readonly Dictionary<string, bool> _previousKeyStates = new Dictionary<string, bool>();
        private static Vector3 _mousePosition = new Vector3();
        private static Vector3 _previousMousePosition = new Vector3();
        private static bool[] _mouseButtons = new bool[3];
        private static bool[] _previousMouseButtons = new bool[3];

        /// <summary>
        /// Checks if a specific key is currently pressed.
        /// </summary>
        /// <param name="keyName">Name of the key (e.g., "F5", "Space", "Escape").</param>
        /// <returns>True if the key is pressed.</returns>
        public static bool IsKeyPressed(string keyName)
        {
            return _keyStates.TryGetValue(keyName.ToUpper(), out bool pressed) && pressed;
        }

        /// <summary>
        /// Checks if a specific key was just pressed this frame.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns>True if the key was just pressed.</returns>
        public static bool IsKeyJustPressed(string keyName)
        {
            string key = keyName.ToUpper();
            bool currentlyPressed = _keyStates.TryGetValue(key, out bool current) && current;
            bool previouslyPressed = _previousKeyStates.TryGetValue(key, out bool previous) && previous;
            return currentlyPressed && !previouslyPressed;
        }

        /// <summary>
        /// Checks if a specific key was just released this frame.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns>True if the key was just released.</returns>
        public static bool IsKeyJustReleased(string keyName)
        {
            string key = keyName.ToUpper();
            bool currentlyPressed = _keyStates.TryGetValue(key, out bool current) && current;
            bool previouslyPressed = _previousKeyStates.TryGetValue(key, out bool previous) && previous;
            return !currentlyPressed && previouslyPressed;
        }

        /// <summary>
        /// Gets the current mouse position.
        /// </summary>
        /// <returns>Mouse position as Vector3.</returns>
        public static Vector3 GetMousePosition()
        {
            return _mousePosition;
        }

        /// <summary>
        /// Gets the mouse movement delta since last frame.
        /// </summary>
        /// <returns>Mouse movement as Vector3.</returns>
        public static Vector3 GetMouseDelta()
        {
            return new Vector3(_mousePosition.X - _previousMousePosition.X, 
                               _mousePosition.Y - _previousMousePosition.Y, 
                               0f);
        }

        /// <summary>
        /// Checks if a mouse button is currently pressed.
        /// </summary>
        /// <param name="button">Mouse button index (0=Left, 1=Right, 2=Middle).</param>
        /// <returns>True if the button is pressed.</returns>
        public static bool IsMouseButtonPressed(int button)
        {
            return button >= 0 && button < _mouseButtons.Length && _mouseButtons[button];
        }

        /// <summary>
        /// Checks if a mouse button was just pressed this frame.
        /// </summary>
        /// <param name="button">Mouse button index.</param>
        /// <returns>True if the button was just pressed.</returns>
        public static bool IsMouseButtonJustPressed(int button)
        {
            return button >= 0 && button < _mouseButtons.Length && 
                   _mouseButtons[button] && !_previousMouseButtons[button];
        }

        /// <summary>
        /// Checks if a mouse button was just released this frame.
        /// </summary>
        /// <param name="button">Mouse button index.</param>
        /// <returns>True if the button was just released.</returns>
        public static bool IsMouseButtonJustReleased(int button)
        {
            return button >= 0 && button < _mouseButtons.Length && 
                   !_mouseButtons[button] && _previousMouseButtons[button];
        }

        /// <summary>
        /// Updates the input state (call this once per frame).
        /// </summary>
        public static void Update()
        {
            // Store previous states
            _previousMousePosition = _mousePosition;
            Array.Copy(_mouseButtons, _previousMouseButtons, _mouseButtons.Length);

            // Update key states dictionary
            foreach (var kvp in _keyStates)
            {
                _previousKeyStates[kvp.Key] = kvp.Value;
            }
        }

        /// <summary>
        /// Sets a key state (used by input handling system).
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="pressed">Whether the key is pressed.</param>
        public static void SetKeyState(string keyName, bool pressed)
        {
            _keyStates[keyName.ToUpper()] = pressed;
        }

        /// <summary>
        /// Sets the mouse position (used by input handling system).
        /// </summary>
        /// <param name="x">Mouse X coordinate.</param>
        /// <param name="y">Mouse Y coordinate.</param>
        public static void SetMousePosition(float x, float y)
        {
            _mousePosition = new Vector3(x, y, 0f);
        }

        /// <summary>
        /// Sets a mouse button state (used by input handling system).
        /// </summary>
        /// <param name="button">Mouse button index.</param>
        /// <param name="pressed">Whether the button is pressed.</param>
        public static void SetMouseButtonState(int button, bool pressed)
        {
            if (button >= 0 && button < _mouseButtons.Length)
            {
                _mouseButtons[button] = pressed;
            }
        }

        /// <summary>
        /// Clears all input states.
        /// </summary>
        public static void Clear()
        {
            _keyStates.Clear();
            _previousKeyStates.Clear();
            _mousePosition = new Vector3();
            _previousMousePosition = new Vector3();
            Array.Clear(_mouseButtons, 0, _mouseButtons.Length);
            Array.Clear(_previousMouseButtons, 0, _previousMouseButtons.Length);
        }

        /// <summary>
        /// Gets all currently pressed keys.
        /// </summary>
        /// <returns>Array of pressed key names.</returns>
        public static string[] GetPressedKeys()
        {
            var pressedKeys = new List<string>();
            foreach (var kvp in _keyStates)
            {
                if (kvp.Value)
                {
                    pressedKeys.Add(kvp.Key);
                }
            }
            return pressedKeys.ToArray();
        }
    }
}
