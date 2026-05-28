using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.UI.Input;
using SASZombieAssaultTD.Engine.UI.Rendering;
using SASZombieAssaultTD.Engine.UI.Widgets;
using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SASZombieAssaultTD.Engine.UI
{
    /// <summary>
    /// Complete UI system for SAS Zombie Assault TD.
    /// Manages UI elements, input handling, rendering, and state management.
    /// </summary>
    public class UISystem
    {
        readonly List<UIElement> _elements = new();
        readonly Dictionary<string, UIPanel> _panels = new();
        bool _isVisible = true;
        bool _inputEnabled = true;
        Vector3 _screenSize;
        bool _initialized;

        /// <summary>
        /// Whether the UI system is visible.
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"UI System visibility set to: {value}");
            }
        }

        /// <summary>
        /// Whether UI input is enabled.
        /// </summary>
        public bool InputEnabled
        {
            get => _inputEnabled;
            set
            {
                _inputEnabled = value;
                Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"UI input enabled set to: {value}");
            }
        }

        /// <summary>
        /// Current screen size.
        /// </summary>
        public Vector3 ScreenSize
        {
            get => _screenSize;
            private set
            {
                _screenSize = value;
                UpdateElementPositions();
            }
        }

        /// <summary>
        /// Number of registered UI elements.
        /// </summary>
        public int ElementCount => _elements.Count;

        /// <summary>
        /// Number of registered panels.
        /// </summary>
        public int PanelCount => _panels.Count;

        /// <summary>
        /// Initialize the UI system.
        /// </summary>
        /// <param name="screenWidth">Screen width.</param>
        /// <param name="screenHeight">Screen height.</param>
        public void Initialize(int screenWidth, int screenHeight)
        {
            if (_initialized)
            {
                Engine.Diagnostics.DebugLogger.LogWarning("UI System already initialized");
                return;
            }

            try
            {
                ScreenSize = new Vector3(screenWidth, screenHeight, 0);
                _initialized = true;
                Engine.Diagnostics.DebugLogger.LogInfo($"UI System initialized ({screenWidth}x{screenHeight})");
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.Exception(ex, "Failed to initialize UI System");
                throw;
            }
        }

        /// <summary>
        /// Update the UI system.
        /// </summary>
        /// <param name="deltaTime">Time since last frame.</param>
        public void Update(float deltaTime)
        {
            if (!_initialized || !IsVisible) return;

            try
            {
                // Update all elements
                foreach (var element in _elements.Where(e => e.IsActive()))
                    element.Update(deltaTime);
                

                // Update all panels
                foreach (var panel in _panels.Values.Where(p => p.IsActive()))
                    panel.Update(deltaTime);
                
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.Exception(ex, "Failed to update UI System");
            }
        }

        /// <summary>
        /// Render the UI system.
        /// </summary>
        /// <param name="renderContext">Render context.</param>
        public void Render(UIRenderContext renderContext)
        {
            if (!_initialized || !IsVisible) return;

            try
            {
                // Render all panels first (background)
                foreach (var panel in _panels.Values.Where(p => p.IsVisible))
                    panel.Render(renderContext);
                

                // Render all elements
                foreach (var element in _elements.Where(e => e.IsVisible))
                    element.Render(renderContext);
                
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.Exception(ex, "Failed to render UI System");
            }
        }

        /// <summary>
        /// Add a UI element to the system.
        /// </summary>
        /// <param name="element">Element to add.</param>
        public void AddElement(UIElement element)
        {
            if (element == null)
            {
                Engine.Diagnostics.DebugLogger.LogWarning("Attempted to add null UI element");
                return;
            }

            if (_elements.Contains(element))
            {
                Engine.Diagnostics.DebugLogger.LogWarning($"Element {element.Name} already exists in UI system");
                return;
            }

            _elements.Add(element);
            element.Initialize();
            Engine.Diagnostics.DebugLogger.LogDebug($"Added UI element: {element.Name}");
        }

        /// <summary>
        /// Remove a UI element from the system.
        /// </summary>
        /// <param name="element">Element to remove.</param>
        public void RemoveElement(UIElement element)
        {
            if (element == null) return;

            if (_elements.Remove(element))
            {
                element.Cleanup();
                Engine.Diagnostics.DebugLogger.LogDebug($"Removed UI element: {element.Name}");
            }
        }

        /// <summary>
        /// Get a UI element by name.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <returns>UI element or null if not found.</returns>
        public UIElement GetElement(string name) => _elements.FirstOrDefault(e => e.Name() == name);

        /// <summary>
        /// Add a UI panel to the system.
        /// </summary>
        /// <param name="panel">Panel to add.</param>
        public void AddPanel(UIPanel panel)
        {
            if (panel == null)
            {
                Engine.Diagnostics.DebugLogger.LogWarning("Attempted to add null UI panel");
                return;
            }

            if (_panels.ContainsKey(panel.Name()))
            {
                Engine.Diagnostics.DebugLogger.LogWarning($"Panel {panel.Name()} already exists in UI system");
                return;
            }

            _panels.Add(panel.Name(), panel);
            panel.Initialize();
            Engine.Diagnostics.DebugLogger.LogDebug($"Added UI panel: {panel.Name()}");
        }

        /// <summary>
        /// Remove a UI panel from the system.
        /// </summary>
        /// <param name="panelName">Panel name to remove.</param>
        public void RemovePanel(string panelName)
        {
            if (_panels.TryGetValue(panelName, out var panel))
            {
                _panels.Remove(panelName);
                panel.Cleanup();
                Engine.Diagnostics.DebugLogger.LogDebug($"Removed UI panel: {panelName}");
            }
        }

        /// <summary>
        /// Get a UI panel by name.
        /// </summary>
        /// <param name="name">Panel name.</param>
        /// <returns>UI panel or null if not found.</returns>
        public UIPanel GetPanel(string name)
        {
            return _panels.TryGetValue(name, out var panel) ? panel : null;
        }

        /// <summary>
        /// Show a specific panel.
        /// </summary>
        /// <param name="panelName">Panel name to show.</param>
        public void ShowPanel(string panelName)
        {
            var panel = GetPanel(panelName);

            if (panel != null)
            {
                panel.IsVisible = true;
                Engine.Diagnostics.DebugLogger.LogDebug($"Showed panel: {panelName}");
            }
        }

        /// <summary>
        /// Hide a specific panel.
        /// </summary>
        /// <param name="panelName">Panel name to hide.</param>
        public void HidePanel(string panelName)
        {
            var panel = GetPanel(panelName);

            if (panel != null)
            {
                panel.IsVisible = false;
                Engine.Diagnostics.DebugLogger.LogDebug($"Hid panel: {panelName}");
            }
        }

        /// <summary>
        /// Handle input events.
        /// </summary>
        /// <param name="inputEvent">Input event data.</param>
        public void HandleInput(UIInputState inputEvent, object elementX)
        {
            if (!_initialized || !IsVisible || !InputEnabled) return;

            try
            {
                // Process input for visible elements
                foreach (var element in _elements.Where(e => e.IsVisible && e.IsActive()))
                {
                    if (element != null) 
     //               if (elementX.HandleInput(inputEvent))
                    {
                        // Input was handled, stop propagation
                        break;
                    }
                    continue;
                }
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.Exception(ex, "Failed to handle UI input");
            }
        }

        /// <summary>
        /// Update screen size.
        /// </summary>
        /// <param name="width">New screen width.</param>
        /// <param name="height">New screen height.</param>
        public void UpdateScreenSize(int width, int height)
        {
            ScreenSize = new Vector3(width, height, 0);
            Engine.Diagnostics.DebugLogger.LogDebug($"Screen size updated to {width}x{height}");
        }

        /// <summary>
        /// Clear all UI elements and panels.
        /// </summary>
        public void Clear()
        {
            // Cleanup all elements
            foreach (var element in _elements)
                element.Cleanup();
            

            _elements.Clear();

            // Cleanup all panels
            foreach (var panel in _panels.Values)
            {
                // TODO: Implement Cleanup method for UIPanel
                // panel.Cleanup();
            }

            _panels.Clear();

            Engine.Diagnostics.DebugLogger.LogInfo("UI System cleared");
        }

        /// <summary>
        /// Shutdown the UI system.
        /// </summary>
        public void Shutdown()
        {
            if (!_initialized) return;

            Clear();
            _initialized = false;
            Engine.Diagnostics.DebugLogger.LogInfo("UI System shutdown");
        }

        /// <summary>
        /// Update positions of all elements when screen size changes.
        /// </summary>
        void UpdateElementPositions()
        {
            // Update all elements
            foreach (var element in _elements)
                element.UpdateLayout();
            
        }
    }
}