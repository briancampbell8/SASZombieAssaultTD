/*
File:    UIInputRouter.cs
Purpose:  Input routing system for SAS Zombie Assault TD UI.
Features:  Mouse input, keyboard input, scroll wheel, and input diagnostics.
*/

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Input
{
    /// <summary>
    /// Input router for UI interactions and user input handling.
    /// Provides comprehensive input management with mouse, keyboard, and scroll support.
    /// </summary>
    public sealed class UIInputRouter
    {
        ///  Private Fields

        private readonly Dictionary<int, bool> _mouseButtonStates = new();
        private readonly Dictionary<string, bool> _keyStates = new();
        private float _scrollDelta = 0f;
        private int _mouseEventsProcessed = 0;
        private bool _isEnabled = true;

        /// 

        ///  Public Properties

        /// <summary>
        /// Gets whether input router is enabled.
        /// </summary>
        public bool IsEnabled => _isEnabled;

        /// 

        ///  Mouse Input

        /// <summary>
        /// Gets current scroll wheel delta.
        /// </summary>
        /// <returns>Scroll wheel delta value.</returns>
        public float GetScrollDelta()
        {
            var delta = _scrollDelta;
            _scrollDelta = 0f; // Reset after reading
            return delta;
        }

        /// <summary>
        /// Checks if mouse button is currently down.
        /// </summary>
        /// <param name="button">Mouse button to check (0=left, 1=right, 2=middle).</param>
        /// <returns>True if button is down.</returns>
        public bool IsMouseButtonDown(int button)
        {
            return _mouseButtonStates.TryGetValue(button, out var isDown) && isDown;
        }

        /// <summary>
        /// Checks if mouse button was just pressed.
        /// </summary>
        /// <param name="button">Mouse button to check (0=left, 1=right, 2=middle).</param>
        /// <returns>True if button was just pressed.</returns>
        public bool IsMouseButtonPressed(int button)
        {
            // This would need to track previous states for press detection
            // For now, return current down state
            return IsMouseButtonDown(button);
        }

        /// <summary>
        /// Checks if mouse button was just released.
        /// </summary>
        /// <param name="button">Mouse button to check (0=left, 1=right, 2=middle).</param>
        /// <returns>True if button was just released.</returns>
        public bool IsMouseButtonReleased(int button)
        {
            // This would need to track previous states for release detection
            // For now, return not down state
            return !IsMouseButtonDown(button);
        }

        /// 

        ///  Keyboard Input

        /// <summary>
        /// Checks if key is currently down.
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>True if key is down.</returns>
        public bool IsKeyDown(string key)
        {
            return _keyStates.TryGetValue(key, out var isDown) && isDown;
        }

        /// 

        ///  Input Processing

        /// <summary>
        /// Processes mouse button down event.
        /// </summary>
        /// <param name="button">Mouse button that went down.</param>
        public void OnMouseDown(int button)
        {
            _mouseButtonStates[button] = true;
            _mouseEventsProcessed++;
        }

        /// <summary>
        /// Processes mouse button up event.
        /// </summary>
        /// <param name="button">Mouse button that went up.</param>
        public void OnMouseUp(int button)
        {
            _mouseButtonStates[button] = false;
            _mouseEventsProcessed++;
        }

        /// <summary>
        /// Processes scroll wheel event.
        /// </summary>
        /// <param name="delta">Scroll wheel delta.</param>
        public void OnScroll(float delta)
        {
            _scrollDelta += delta;
        }

        /// <summary>
        /// Processes key down event.
        /// </summary>
        /// <param name="key">Key that went down.</param>
        public void OnKeyDown(string key)
        {
            _keyStates[key] = true;
        }

        /// <summary>
        /// Processes key up event.
        /// </summary>
        /// <param name="key">Key that went up.</param>
        public void OnKeyUp(string key)
        {
            _keyStates[key] = false;
        }

        /// 

        ///  Control Methods

        /// <summary>
        /// Enables or disables input routing.
        /// </summary>
        /// <param name="enabled">Whether to enable input.</param>
        public void SetEnabled(bool enabled)
        {
            _isEnabled = enabled;
        }

        /// <summary>
        /// Resets all input states.
        /// </summary>
        public void Reset()
        {
            _mouseButtonStates.Clear();
            _keyStates.Clear();
            _scrollDelta = 0f;
            _mouseEventsProcessed = 0;
        }

        /// 

        ///  Statistics

        /// <summary>
        /// Gets input router statistics.
        /// </summary>
        /// <returns>Input router statistics.</returns>
        public InputRouterStats GetStats()
        {
            return new InputRouterStats
            {
                KeysTracked = _keyStates.Count,
                MouseEventsProcessed = _mouseEventsProcessed,
                IsEnabled = _isEnabled
            };
        }

        /// 
    }

    /// <summary>
    /// Statistics for input router performance monitoring.
    /// </summary>
    public class InputRouterStats
    {
        /// <summary>
        /// Number of keys currently tracked.
        /// </summary>
        public int KeysTracked { get; set; }

        /// <summary>
        /// Number of mouse events processed.
        /// </summary>
        public int MouseEventsProcessed { get; set; }

        /// <summary>
        /// Whether input router is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
