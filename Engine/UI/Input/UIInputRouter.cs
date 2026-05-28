using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Extensions;

namespace SASZombieAssaultTD.Engine.UI.Input
{
    /// <summary>
    /// Input routing logic for mouse and keyboard events to UI elements
    /// P80-05-01: UIInputRouter providing input routing logic for mouse and keyboard events to UI elements
    /// </summary>
    public class UIInputRouter
    {
        private readonly UIInputState _inputState;
        private readonly UIFocusManager _focusManager;
        private readonly List<UIElement> _elements;
        private UIElement _hoveredElement;
        private UIElement _pressedElement;
        private bool _isInitialized = false;
        private UIInputState _currentInput;

        /// <summary>
        /// Gets the currently hovered element
        /// </summary>
        public UIElement HoveredElement => _hoveredElement;

        /// <summary>
        /// Gets the currently pressed element
        /// </summary>
        public UIElement PressedElement => _pressedElement;

        /// <summary>
        /// Gets the input state
        /// </summary>
        public UIInputState InputState => _inputState;

        /// <summary>
        /// Gets the focus manager
        /// </summary>
        public UIFocusManager FocusManager => _focusManager;

        /// <summary>
        /// Gets whether the router is initialized
        /// </summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// Initializes a new UIInputRouter
        /// </summary>
        /// <param name="inputState">Input state to use</param>
        /// <param name="focusManager">Focus manager to use</param>
        public UIInputRouter(UIInputState inputState, UIFocusManager focusManager)
        {
            _inputState = inputState ?? throw new ArgumentNullException(nameof(inputState));
            _focusManager = focusManager ?? throw new ArgumentNullException(nameof(focusManager));
            _elements = new List<UIElement>();
            _hoveredElement = null;
            _pressedElement = null;
            _isInitialized = false;
            _currentInput = inputState;

            System.Diagnostics.Debug.WriteLine("UIInputRouter: Initialized");
        }

        /// <summary>
        /// Adds a UI element to the router
        /// </summary>
        /// <param name="element">Element to add</param>
        public void RegisterElement(UIElement element)
        {
            try
            {
                if (element == null)
                {
                    System.Diagnostics.Debug.WriteLine("UIInputRouter: Cannot register null element");
                    return;
                }

                if (_elements.Contains(element))
                {
                    System.Diagnostics.Debug.WriteLine("UIInputRouter: Element already registered");
                    return;
                }

                _elements.Add(element);
                _focusManager.RegisterFocusableElement(element);

                System.Diagnostics.Debug.WriteLine($"UIInputRouter: Registered element, total: {_elements.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIInputRouter: Error registering element - {ex.Message}");
            }
        }

        /// <summary>
        /// Removes a UI element from the router
        /// </summary>
        /// <param name="element">Element to remove</param>
        public void UnregisterElement(UIElement element)
        {
            try
            {
                if (element == null)
                {
                    System.Diagnostics.Debug.WriteLine("UIInputRouter: Cannot unregister null element");
                    return;
                }

                if (_elements.Remove(element))
                {
                    _focusManager.UnregisterFocusableElement(element);

                    // Clear hover/pressed state if this element was affected
                    if (_hoveredElement == element)
                    {
                        _hoveredElement = null;
                        element.OnMouseExit();
                    }

                    if (_pressedElement == element)
                    {
                        _pressedElement = null;
                        element.OnMouseRelease();
                    }

                    System.Diagnostics.Debug.WriteLine($"UIInputRouter: Unregistered element, remaining: {_elements.Count}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("UIInputRouter: Element not found in router");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIInputRouter: Error unregistering element - {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the input router
        /// </summary>
        /// <param name="mousePosition">Current mouse position</param>
        /// <param name="mouseButtons">Current mouse button states</param>
        /// <param name="keys">Current key states</param>
        public void Update(System.Drawing.PointF mousePosition, bool[] mouseButtons, bool[] keys)
        {
            try
            {
                if (!_isInitialized)
                {
                    System.Diagnostics.Debug.WriteLine("UIInputRouter: Cannot update - not initialized");
                    return;
                }

                // Update input state
                _inputState.Update(mousePosition, mouseButtons, keys);

                // Process hover state changes
                ProcessHoverChanges();

                // Process press state changes
                ProcessPressChanges();

                // Process focus changes
                ProcessFocusChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIInputRouter: Error during update - {ex.Message}");
            }
        }

        /// <summary>
        /// Processes hover state changes
        /// </summary>
        private void ProcessHoverChanges()
        {
            try
            {
                var newHoveredElement = GetElementAtPosition(_inputState.MousePosition);

                // Clear hover on previous element
                if (_hoveredElement != null && _hoveredElement != newHoveredElement)
                {
                    _hoveredElement.OnMouseExit();
                }

                // Set hover on new element
                if (newHoveredElement != null && newHoveredElement != _hoveredElement)
                {
                    _hoveredElement = newHoveredElement;
                    newHoveredElement.OnMouseEnter();
                }

                _hoveredElement = newHoveredElement;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIInputRouter: Error processing hover changes - {ex.Message}");
            }
        }

        /// <summary>
        /// Processes press state changes
        /// </summary>
        private void ProcessPressChanges()
        {
            try
            {
                // Check for new pressed element
                UIElement newPressedElement = null;

                foreach (var element in _elements)
                {
                    if (element.ContainsPoint(_inputState.MousePosition))
                    {
                        newPressedElement = element;
                        break;
                    }
                }

                // Clear press on previous element
                if (_pressedElement != null && _pressedElement != newPressedElement)
                {
                    _pressedElement.OnMouseRelease();
                }

                // Set press on new element
                if (newPressedElement != null && newPressedElement != _pressedElement)
                {
                    _pressedElement = newPressedElement;
                    newPressedElement.OnMousePress();
                }

                _pressedElement = newPressedElement;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIInputRouter: Error processing press changes - {ex.Message}");
            }
        }

        /// <summary>
        /// Processes focus changes
        /// </summary>
        private void ProcessFocusChanges()
        {
            try
            {
                // Check for focus changes based on input
                bool focusRequested = false;

                // Tab key for focus navigation
                if (_inputState.IsKeyJustPressed(9)) // Tab key
                {
                    focusRequested = true;
                }

                // Mouse click for focus selection
                if (_inputState.IsMouseButtonJustPressed(0)) // Left mouse button
                {
                    focusRequested = true;
                }

                // Apply focus changes
                if (focusRequested)
                {
                    // Focus next element
                    _focusManager.FocusNext();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIInputRouter: Error processing focus changes - {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the element at the specified position
        /// </summary>
        /// <param name="position">Position to check</param>
        /// <returns>Element at position, or null if none</returns>
        private UIElement GetElementAtPosition(System.Drawing.PointF position)
        {
            try
            {
                // Check elements in reverse order (top-to-bottom)
                for (int i = _elements.Count - 1; i >= 0; i--)
                {
                    var element = _elements[i];
                    if (element.IsVisible && element.ContainsPoint(position))
                    {
                        return element;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIInputRouter: Error getting element at position - {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Initializes the router
        /// </summary>
        public void Initialize()
        {
            try
            {
                if (_isInitialized)
                {
                    System.Diagnostics.Debug.WriteLine("UIInputRouter: Already initialized");
                    return;
                }

                _isInitialized = true;
                _hoveredElement = null;
                _pressedElement = null;

                System.Diagnostics.Debug.WriteLine("UIInputRouter: Initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIInputRouter: Error during initialization - {ex.Message}");
            }
        }

        /// <summary>
        /// Shuts down the router
        /// </summary>
        public void Shutdown()
        {
            try
            {
                if (!_isInitialized)
                {
                    System.Diagnostics.Debug.WriteLine("UIInputRouter: Already shut down");
                    return;
                }

                // Clear all hover/press states
                if (_hoveredElement != null)
                {
                    _hoveredElement.OnMouseExit();
                }

                if (_pressedElement != null)
                {
                    _pressedElement.OnMouseRelease();
                }

                _elements.Clear();
                _hoveredElement = null;
                _pressedElement = null;
                _isInitialized = false;

                System.Diagnostics.Debug.WriteLine("UIInputRouter: Shut down successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIInputRouter: Error during shutdown - {ex.Message}");
            }
        }

        /// <summary>
        /// Processes input events and routes them to UI elements.
        /// </summary>
        public void ProcessInput()
        {
            try
            {
                // Process input events for all registered elements
                foreach (var element in _elements)
                {
                    if (element.IsActive())
                    {
                        element.ProcessInput(_currentInput);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIInputRouter: Error processing input - {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the current mouse position.
        /// </summary>
        /// <returns>Current mouse position</returns>
        public Vector3 GetMousePosition()
        {
            return new Vector3(_inputState.MousePosition.X, _inputState.MousePosition.Y, 0f);
        }

        /// <summary>
        /// Gets the current key states.
        /// </summary>
        /// <returns>Current key states</returns>
        public bool[] GetKeyStates()
        {
            return (bool[])_inputState.KeyStates();
        }

        /// <summary>
        /// Gets diagnostic information about the UI input system.
        /// </summary>
        /// <returns>Diagnostic information</returns>
        public string GetDiagnostics()
        {
            return $"UIInputRouter: Elements={_elements.Count}, Hovered={_hoveredElement?.GetType().Name ?? "None"}, Pressed={_pressedElement?.GetType().Name ?? "None"}, Initialized={_isInitialized}";
        }

        /// <summary>
        /// Enables or disables the input router.
        /// </summary>
        /// <param name="enabled">Whether the router should be enabled</param>
        public void SetEnabled(bool enabled)
        {
            try
            {
                // Enable/disable input processing
                if (enabled && !_isInitialized)
                {
                    Initialize();
                }
                else if (!enabled && _isInitialized)
                {
                    Shutdown();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIInputRouter: Error setting enabled state - {ex.Message}");
            }
        }

        /// <summary>
        /// Gets menu-specific input state.
        /// </summary>
        /// <returns>Menu input state</returns>
        public object GetMenuInput()
        {
            return new
            {
                MousePosition = GetMousePosition(),
                KeyStates = GetKeyStates(),
                HoveredElement = _hoveredElement,
                PressedElement = _pressedElement
            };
        }
    }
}




