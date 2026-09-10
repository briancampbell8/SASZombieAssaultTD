// ====================================================================================================
//  FILE: UIDebugOverlay.cs
//  PATH: ./Engine/UI/Debug/
//  MODULE: UI
//
//  ROLE:
//      Provide UI layout, interaction logic, or HUD rendering.
//
//  RESPONSIBILITIES:
//      - Provide AddElement() behavior for the UI subsystem.
//      - Provide RemoveElement() behavior for the UI subsystem.
//      - Provide Update() behavior for the UI subsystem.
//      - Provide Render() behavior for the UI subsystem.
//      - Provide Clear() behavior for the UI subsystem.
//      - Provide ToString() behavior for the UI subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.UI.Elements;
using SASZombieAssaultTD.Engine.UI.Input;

namespace SASZombieAssaultTD.Engine.UI.Debug
{
    /// <summary>
    /// A basic debug overlay showing UI hierarchy and layout bounds P80-07-01: UIDebugOverlay providing a basic debug
    /// overlay showing UI hierarchy and layout bounds
    /// </summary>
    public class UIDebugOverlay
    {
        private readonly List<UIElement> _elements;
        private bool _isVisible = false;
        private bool _showBounds = true;
        private bool _showHierarchy = true;
        private UIElement _selectedElement;
        private readonly System.Drawing.Color _backgroundColor = System.Drawing.Color.FromArgb(128, 0, 0, 0);
        private readonly System.Drawing.Color _textColor = System.Drawing.Color.White;
        private readonly System.Drawing.Color _borderColor = System.Drawing.Color.Yellow;
        private readonly System.Drawing.Color _selectedColor = System.Drawing.Color.Cyan;
        private UIInputState _inputState;

        /// <summary>
        /// Gets or sets whether the overlay is visible
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    DLogger.Log($"UIDebugOverlay: Visibility set to {value}");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show element bounds
        /// </summary>
        public bool ShowBounds
        {
            get => _showBounds;
            set
            {
                if (_showBounds != value)
                {
                    _showBounds = value;
                    DLogger.Log($"UIDebugOverlay: Show bounds set to {value}");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show UI hierarchy
        /// </summary>
        public bool ShowHierarchy
        {
            get => _showHierarchy;
            set
            {
                if (_showHierarchy != value)
                {
                    _showHierarchy = value;
                    DLogger.Log($"UIDebugOverlay: Show hierarchy set to {value}");
                }
            }
        }

        /// <summary>
        /// Gets the currently selected element
        /// </summary>
        public UIElement SelectedElement => _selectedElement;

        /// <summary>
        /// Initializes a new UIDebugOverlay
        /// </summary>
        public UIDebugOverlay()
        {
            _elements = new List<UIElement>();
            _inputState = new UIInputState();
            DLogger.Log(LogSubsystems.ResourcesPipeline, "UIDebugOverlay: Initialized");
        }

        /// <summary>
        /// Adds a UI element to the overlay
        /// </summary>
        /// <param name="element">Element to add</param>
        public void AddElement(UIElement element)
        {
            try
            {
                if (element == null)
                {
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "UIDebugOverlay: Cannot add null element");
                    return;
                }

                if (_elements.Contains(element))
                {
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "UIDebugOverlay: Element already exists in overlay");
                    return;
                }

                _elements.Add(element);
                DLogger.Log($"UIDebugOverlay: Added element, total: {_elements.Count}");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIDebugOverlay: Error adding element - {ex.Message}");
            }
        }

        /// <summary>
        /// Removes a UI element from the overlay
        /// </summary>
        /// <param name="element">Element to remove</param>
        public void RemoveElement(UIElement element)
        {
            try
            {
                if (element == null)
                {
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "UIDebugOverlay: Cannot remove null element");
                    return;
                }

                if (_elements.Remove(element))
                {
                    DLogger.Log($"UIDebugOverlay: Removed element, remaining: {_elements.Count}");

                    //Clear selection if this element was selected
                    if (_selectedElement == element)
                    {
                        _selectedElement = null;
                    }
                }
                else
                {
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "UIDebugOverlay: Element not found in overlay");
                }
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIDebugOverlay: Error removing element - {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the debug overlay
        /// </summary>
        /// <param name="deltaTime">Time since last update in seconds</param>
        public void Update(float deltaTime)
        {
            try
            {
                //Update element hover states
                UpdateHoverStates();

                //Update selection with keyboard
                UpdateSelection();

                DLogger.Log(LogSubsystems.ResourcesPipeline, "UIDebugOverlay: Updated overlay");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIDebugOverlay: Error during update - {ex.Message}");
            }
        }

        /// <summary>
        /// Renders the debug overlay
        /// </summary>
        public void Render()
        {
            try
            {
                if (!_isVisible)
                    return;

                //This would use the actual rendering system
                //For now, just log the overlay state
                DLogger.Log($"UIDebugOverlay: Rendering overlay with {_elements.Count} elements");

                //Render each element's debug info
                foreach (var element in _elements)
                {
                    RenderElementDebugInfo(element);
                }
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIDebugOverlay: Error during render - {ex.Message}");
            }
        }

        /// <summary>
        /// Renders debug information for a single element
        /// </summary>
        /// <param name="element">Element to render debug info for</param>
        private void RenderElementDebugInfo(UIElement element)
        {
            try
            {
                var position = element.AbsolutePosition;
                var size = element.Size;
                var isSelected = element == _selectedElement;
                var isHovered = element is UIWidgetBase widgetBase && widgetBase.IsHovered;

                //This would render actual debug information
                //For now, just log the debug info
                DLogger.Log($"UIDebugOverlay: Element '{element.GetType().Name}' at {position}, Size: {size}, Selected: {isSelected}, Hovered: {isHovered}");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIDebugOverlay: Error rendering element debug info - {ex.Message}");
            }
        }

        /// <summary>
        /// Updates hover states for all elements
        /// </summary>
        private void UpdateHoverStates()
        {
            try
            {
                //Check hover state for each element
                foreach (var element in _elements)
                {
                    // Fixed: Convert System.Drawing.PointF to your engine's custom Components.PointF
                    var isHovered = element.ContainsPoint(
                        new Components.PointF(
                        _inputState.MousePosition.X,
                        _inputState.MousePosition.Y
                    ));

                    UIWidgetBase? widgetBase = element as UIWidgetBase;
                    var wasHovered = widgetBase?.IsHovered ?? false;

                    if (wasHovered && !isHovered)
                    {
                        widgetBase?.OnMouseExit();
                    }
                    else if (!wasHovered && isHovered)
                    {
                        widgetBase?.OnMouseEnter();
                    }
                }

            }
            catch (Exception ex)
            {
                DLogger.Log($"UIDebugOverlay: Error updating hover states - {ex.Message}");
            }
        }

        /// <summary>
        /// Updates selection with keyboard input
        /// </summary>
        private void UpdateSelection()
        {
            try
            {
                //Check for keyboard selection
                if (_inputState.IsKeyJustPressed(13)) //Enter key
                {
                    SelectNextElement();
                }
                else if (_inputState.IsKeyJustPressed(38)) //Up arrow key
                {
                    SelectPreviousElement();
                }
                else if (_inputState.IsKeyJustPressed(40)) //Down arrow key
                {
                    SelectNextElement();
                }
                else if (_inputState.IsMouseButtonJustPressed(0)) //Left mouse button
                {
                    ///SelectElementAtPosition(_inputState.MousePosition);
                    SelectElementAtPosition(new System.Drawing.PointF(_inputState.MousePosition.X, _inputState.MousePosition.Y));
                }

                DLogger.Log($"UIDebugOverlay: Updated selection with keyboard input");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIDebugOverlay: Error updating selection - {ex.Message}");
            }
        }

        /// <summary>
        /// Selects the next element in the overlay
        /// </summary>
        private void SelectNextElement()
        {
            try
            {
                if (_elements.Count == 0)
                    return;

                var currentIndex = _selectedElement != null ? _elements.IndexOf(_selectedElement) : -1;

                if (currentIndex < _elements.Count - 1)
                {
                    _selectedElement = _elements[currentIndex + 1];
                }
                else
                {
                    _selectedElement = _elements[0];
                }

                DLogger.Log($"UIDebugOverlay: Selected next element: {_selectedElement?.GetType().Name}");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIDebugOverlay: Error selecting next element - {ex.Message}");
            }
        }

        /// <summary>
        /// Selects the previous element in the overlay
        /// </summary>
        private void SelectPreviousElement()
        {
            try
            {
                if (_elements.Count == 0)
                    return;

                var currentIndex = _selectedElement != null ? _elements.IndexOf(_selectedElement) : -1;

                if (currentIndex > 0)
                {
                    _selectedElement = _elements[currentIndex - 1];
                }
                else
                {
                    _selectedElement = _elements[_elements.Count - 1];
                }

                DLogger.Log($"UIDebugOverlay: Selected previous element: {_selectedElement?.GetType().Name}");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIDebugOverlay: Error selecting previous element - {ex.Message}");
            }
        }

        /// <summary>
        /// Selects the element at the specified position
        /// </summary>
        private void SelectElementAtPosition(System.Drawing.PointF position)
        {
            try
            {
                UIElement closestElement = null;
                float closestDistance = float.MaxValue;

                foreach (var element in _elements)
                {
                    if (!element.IsVisible)
                        continue;

                    var distance = CalculateDistance(position, element);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestElement = element;
                    }
                }

                if (closestElement != null)
                {
                    _selectedElement = closestElement;
                    DLogger.Log($"UIDebugOverlay: Selected element at position" +
                        $" {position}: {closestElement?.GetType().Name}");
                }
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIDebugOverlay: Error selecting element at position - {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates distance between a point and an element
        /// </summary>
        /// <param name="point">Point to calculate distance from</param>
        /// <param name="element">Element to calculate distance to</param>
        /// <returns>Distance between point and element</returns>
        private float CalculateDistance(System.Drawing.PointF point, UIElement element)
        {
            try
            {
                var elementCenter = new System.Drawing.PointF(
                element.AbsolutePosition.X + element.Size.Width / 2,
                element.AbsolutePosition.Y + element.Size.Height / 2
                );

                var dx = point.X - elementCenter.X;
                var dy = point.Y - elementCenter.Y;

                return (float)System.Math.Sqrt(dx * dx + dy * dy);
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIDebugOverlay: Error calculating distance - {ex.Message}");
                return float.MaxValue;
            }
        }

        /// <summary>
        /// Clears all elements from the overlay
        /// </summary>
        public void Clear()
        {
            try
            {
                _elements.Clear();
                _selectedElement = null;

                DLogger.Log(LogSubsystems.ResourcesPipeline, "UIDebugOverlay: Cleared all elements");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIDebugOverlay: Error clearing elements - {ex.Message}");
            }
        }

        /// <summary>
        /// Gets a string representation of the debug overlay state
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            try
            {
                return $"UIDebugOverlay: {_elements.Count} elements, Visible: {_isVisible}, Selected: {_selectedElement?.GetType().Name}";
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIDebugOverlay: Error creating string representation - {ex.Message}");
                return "UIDebugOverlay: Error";
            }
        }
    }
}
