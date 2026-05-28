using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.UI
{
    /// <summary>
    /// Root UI system class providing initialization, update, and render entry points
    /// P80-01-01: UIRoot containing root UI system with initialization, update, and render entry points
    /// </summary>
    public class UIRoot : UIElement
    {
        private readonly List<UIElement> _elements;
        private bool _isInitialized = false;

        /// <summary>
        /// Gets whether the UI system is initialized
        /// </summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// Gets the collection of UI elements
        /// </summary>
        public IReadOnlyList<UIElement> Elements => _elements.AsReadOnly();

        private bool needsLayoutUpdate = true;

        /// <summary>
        /// Gets whether the UI system needs a layout update
        /// </summary>
        public bool GetNeedsLayoutUpdate()
        {
            return needsLayoutUpdate;
        }

        private void SetNeedsLayoutUpdate(bool value)
        {
            needsLayoutUpdate = value;
        }

        /// <summary>
        /// Initializes the UI system
        /// </summary>
        public void Initialize()
        {
            try
            {
                if (_isInitialized)
                {
                    System.Diagnostics.Debug.WriteLine("UIRoot: Already initialized");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("UIRoot: Initializing UI system");

                _elements.Clear();
                SetNeedsLayoutUpdate(true);
                _isInitialized = true;

                System.Diagnostics.Debug.WriteLine("UIRoot: UI system initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIRoot: Error during initialization - {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Updates the UI system
        /// </summary>
        /// <param name="deltaTime">Time since last update in seconds</param>
        public override void Update(float deltaTime)
        {
            try
            {
                if (!_isInitialized)
                {
                    System.Diagnostics.Debug.WriteLine("UIRoot: Cannot update - not initialized");
                    return;
                }

                // Update all UI elements
                for (int i = 0; i < _elements.Count; i++)
                {
                    _elements[i].Update(deltaTime);
                }

                // Mark layout as needing update if any element needs it
                foreach (var element in _elements)
                {
                    if (element.NeedsLayoutUpdate)
                    {
                        SetNeedsLayoutUpdate(true);
                        break;
                    }
                }

                System.Diagnostics.Debug.WriteLine($"UIRoot: Updated {_elements.Count} UI elements");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIRoot: Error during update - {ex.Message}");
            }
        }

        /// <summary>
        /// Renders the UI system
        /// </summary>
        public override void Render()
        {
            try
            {
                if (!_isInitialized)
                {
                    System.Diagnostics.Debug.WriteLine("UIRoot: Cannot render - not initialized");
                    return;
                }

                // Update layouts if needed
                if (GetNeedsLayoutUpdate())
                {
                    UpdateLayouts();
                }

                // Render all UI elements
                for (int i = 0; i < _elements.Count; i++)
                {
                    _elements[i].Render();
                }

                System.Diagnostics.Debug.WriteLine($"UIRoot: Rendered {_elements.Count} UI elements");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIRoot: Error during render - {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a UI element to the root
        /// </summary>
        /// <param name="element">UI element to add</param>
        public void AddElement(UIElement element)
        {
            try
            {
                if (element == null)
                {
                    System.Diagnostics.Debug.WriteLine("UIRoot: Cannot add null element");
                    return;
                }

                if (_elements.Contains(element))
                {
                    System.Diagnostics.Debug.WriteLine("UIRoot: Element already exists in UI root");
                    return;
                }

                _elements.Add(element);
                element.Parent = this;
                SetNeedsLayoutUpdate(true);

                System.Diagnostics.Debug.WriteLine($"UIRoot: Added UI element, total: {_elements.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIRoot: Error adding element - {ex.Message}");
            }
        }

        /// <summary>
        /// Removes a UI element from the root
        /// </summary>
        /// <param name="element">UI element to remove</param>
        public void RemoveElement(UIElement element)
        {
            try
            {
                if (element == null)
                {
                    System.Diagnostics.Debug.WriteLine("UIRoot: Cannot remove null element");
                    return;
                }

                if (_elements.Remove(element))
                {
                    element.Parent = null;
                    SetNeedsLayoutUpdate(true);
                    System.Diagnostics.Debug.WriteLine($"UIRoot: Removed UI element, remaining: {_elements.Count}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("UIRoot: Element not found in UI root");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIRoot: Error removing element - {ex.Message}");
            }
        }

        /// <summary>
        /// Updates layouts for all UI elements
        /// </summary>
        private void UpdateLayouts()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("UIRoot: Updating UI layouts");

                foreach (var element in _elements)
                {
                    element.UpdateLayout();
                }

                SetNeedsLayoutUpdate(false);
                System.Diagnostics.Debug.WriteLine("UIRoot: Layout update completed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIRoot: Error updating layouts - {ex.Message}");
            }
        }

        /// <summary>
        /// Shuts down the UI system
        /// </summary>
        public void Shutdown()
        {
            try
            {
                if (!_isInitialized)
                {
                    System.Diagnostics.Debug.WriteLine("UIRoot: Already shut down");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("UIRoot: Shutting down UI system");

                _elements.Clear();
                SetNeedsLayoutUpdate(false);
                _isInitialized = false;

                System.Diagnostics.Debug.WriteLine("UIRoot: UI system shut down successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIRoot: Error during shutdown - {ex.Message}");
            }
        }
    }
}










