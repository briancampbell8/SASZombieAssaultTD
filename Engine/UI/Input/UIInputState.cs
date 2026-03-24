using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.UI.Input
{
    /// <summary>
    /// Input state data used by UIInputRouter
    /// P80-05-02: UIInputState defining input state data used by UIInputRouter
    /// </summary>
    public class UIInputState
    {
        private System.Drawing.PointF _mousePosition;
        private bool[] _mouseButtons;
        private bool[] _previousMouseButtons;
        private bool[] _keys;
        private bool[] _previousKeys;
        private System.Drawing.PointF _mouseDelta;
        private float _mouseWheelDelta;

        /// <summary>
        /// Gets the current mouse position
        /// </summary>
        public System.Drawing.PointF MousePosition => _mousePosition;

        /// <summary>
        /// Gets the current mouse delta
        /// </summary>
        public System.Drawing.PointF MouseDelta => _mouseDelta;

        /// <summary>
        /// Gets the current mouse wheel delta
        /// </summary>
        public float MouseWheelDelta => _mouseWheelDelta;

        /// <summary>
        /// Gets the current mouse button states
        /// </summary>
        public IReadOnlyList<bool> MouseButtons => Array.AsReadOnly(_mouseButtons);

        /// <summary>
        /// Gets the current key states
        /// </summary>
        public IReadOnlyList<bool> Keys => Array.AsReadOnly(_keys);

        /// <summary>
        /// Gets the number of mouse buttons supported
        /// </summary>
        public int MouseButtonCount => _mouseButtons?.Length ?? 0;

        /// <summary>
        /// Gets the number of keys supported
        /// </summary>
        public int KeyCount => _keys?.Length ?? 0;

        /// <summary>
        /// Initializes a new UIInputState
        /// </summary>
        /// <param name="mouseButtonCount">Number of mouse buttons to track</param>
        /// <param name="keyCount">Number of keys to track</param>
        public UIInputState(int mouseButtonCount = 5, int keyCount = 256)
        {
            _mousePosition = new System.Drawing.PointF(0, 0);
            _mouseButtons = new bool[mouseButtonCount];
            _previousMouseButtons = new bool[mouseButtonCount];
            _keys = new bool[keyCount];
            _previousKeys = new bool[keyCount];
            _mouseDelta = new System.Drawing.PointF(0, 0);
            _mouseWheelDelta = 0f;

            Console.WriteLine($"UIInputState: Initialized with {mouseButtonCount} mouse buttons and {keyCount} keys");
        }

        /// <summary>
        /// Updates the input state
        /// </summary>
        /// <param name="mousePosition">New mouse position</param>
        /// <param name="mouseButtons">New mouse button states</param>
        /// <param name="keys">New key states</param>
        public void Update(System.Drawing.PointF mousePosition, bool[] mouseButtons, bool[] keys)
        {
            try
            {
                // Calculate deltas
                _mouseDelta = new System.Drawing.PointF(
                mousePosition.X - _mousePosition.X,
                mousePosition.Y - _mousePosition.Y
                );

                _mousePosition = mousePosition;

                // Store previous states
                Array.Copy(_mouseButtons, _previousMouseButtons, System.Math.Min(_mouseButtons.Length, _previousMouseButtons.Length));
                Array.Copy(_keys, _previousKeys, System.Math.Min(_keys.Length, _previousKeys.Length));

                // Update current states
                Array.Copy(mouseButtons, _mouseButtons, System.Math.Min(mouseButtons.Length, _mouseButtons.Length));
                Array.Copy(keys, _keys, System.Math.Min(keys.Length, _keys.Length));

                Console.WriteLine($"UIInputState: Updated input state");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIInputState: Error updating input state - {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a mouse button is currently pressed
        /// </summary>
        /// <param name="buttonIndex">Mouse button index</param>
        /// <returns>True if button is pressed</returns>
        public bool IsMouseButtonPressed(int buttonIndex)
        {
            try
            {
                if (buttonIndex >= 0 && buttonIndex < _mouseButtons.Length)
                {
                    return _mouseButtons[buttonIndex];
                }

                Console.WriteLine($"UIInputState: Invalid mouse button index {buttonIndex}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIInputState: Error checking mouse button - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if a mouse button was just pressed
        /// </summary>
        /// <param name="buttonIndex">Mouse button index</param>
        /// <returns>True if button was just pressed</returns>
        public bool IsMouseButtonJustPressed(int buttonIndex)
        {
            try
            {
                if (buttonIndex >= 0 && buttonIndex < _mouseButtons.Length &&
                buttonIndex < _previousMouseButtons.Length)
                {
                    return _mouseButtons[buttonIndex] && !_previousMouseButtons[buttonIndex];
                }

                Console.WriteLine($"UIInputState: Invalid mouse button index {buttonIndex}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIInputState: Error checking just pressed mouse button - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if a mouse button was just released
        /// </summary>
        /// <param name="buttonIndex">Mouse button index</param>
        /// <returns>True if button was just released</returns>
        public bool IsMouseButtonJustReleased(int buttonIndex)
        {
            try
            {
                if (buttonIndex >= 0 && buttonIndex < _mouseButtons.Length &&
                buttonIndex < _previousMouseButtons.Length)
                {
                    return !_mouseButtons[buttonIndex] && _previousMouseButtons[buttonIndex];
                }

                Console.WriteLine($"UIInputState: Invalid mouse button index {buttonIndex}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIInputState: Error checking just released mouse button - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if a key is currently pressed
        /// </summary>
        /// <param name="keyCode">Key code</param>
        /// <returns>True if key is pressed</returns>
        public bool IsKeyPressed(int keyCode)
        {
            try
            {
                if (keyCode >= 0 && keyCode < _keys.Length)
                {
                    return _keys[keyCode];
                }

                Console.WriteLine($"UIInputState: Invalid key code {keyCode}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIInputState: Error checking key - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if a key was just pressed
        /// </summary>
        /// <param name="keyCode">Key code</param>
        /// <returns>True if key was just pressed</returns>
        public bool IsKeyJustPressed(int keyCode)
        {
            try
            {
                if (keyCode >= 0 && keyCode < _keys.Length &&
                keyCode < _previousKeys.Length)
                {
                    return _keys[keyCode] && !_previousKeys[keyCode];
                }

                Console.WriteLine($"UIInputState: Invalid key code {keyCode}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIInputState: Error checking just pressed key - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if a key was just released
        /// </summary>
        /// <param name="keyCode">Key code</param>
        /// <returns>True if key was just released</returns>
        public bool IsKeyJustReleased(int keyCode)
        {
            try
            {
                if (keyCode >= 0 && keyCode < _keys.Length &&
                keyCode < _previousKeys.Length)
                {
                    return !_keys[keyCode] && _previousKeys[keyCode];
                }

                Console.WriteLine($"UIInputState: Invalid key code {keyCode}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIInputState: Error checking just released key - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Clears all input state
        /// </summary>
        public void Clear()
        {
            try
            {
                Array.Clear(_mouseButtons);
                Array.Clear(_previousMouseButtons);
                Array.Clear(_keys);
                Array.Clear(_previousKeys);

                _mousePosition = new System.Drawing.PointF(0, 0);
                _mouseDelta = new System.Drawing.PointF(0, 0);
                _mouseWheelDelta = 0f;

                Console.WriteLine("UIInputState: Cleared all input state");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIInputState: Error clearing input state - {ex.Message}");
            }
        }

        /// <summary>
        /// Gets a string representation of the current input state
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            try
            {
                var pressedButtons = new List<string>();
                var pressedKeys = new List<string>();

                for (int i = 0; i < _mouseButtons.Length; i++)
                {
                    if (_mouseButtons[i])
                        pressedButtons.Add($"Mouse{i}");
                }

                for (int i = 0; i < _keys.Length; i++)
                {
                    if (_keys[i])
                        pressedKeys.Add($"Key{i}");
                }

                return $"Mouse: {_mousePosition}, Buttons: [{string.Join(", ", pressedButtons)}], Keys: [{string.Join(", ", pressedKeys)}]";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIInputState: Error creating string representation - {ex.Message}");
                return "UIInputState: Error";
            }
        }
    }
}




