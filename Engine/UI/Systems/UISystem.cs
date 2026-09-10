// ====================================================================================================
//  FILE: UISystem.cs
//  PATH: ./Engine/UI/Systems/
//  MODULE: UI
//
//  ROLE:
//      Provide UI layout, interaction logic, or HUD rendering.
//
//  RESPONSIBILITIES:
//      - Provide Initialize() behavior for the UI subsystem.
//      - Provide Update() behavior for the UI subsystem.
//      - Provide Render() behavior for the UI subsystem.
//      - Provide AddElement() behavior for the UI subsystem.
//      - Provide RemoveElement() behavior for the UI subsystem.
//      - Provide GetElement() behavior for the UI subsystem.
//      - Provide AddPanel() behavior for the UI subsystem.
//      - Provide RemovePanel() behavior for the UI subsystem.
//      - Provide GetPanel() behavior for the UI subsystem.
//      - Provide ShowPanel() behavior for the UI subsystem.
//      - Provide HidePanel() behavior for the UI subsystem.
//      - Provide HandleInput() behavior for the UI subsystem.
//      - Provide UpdateScreenSize() behavior for the UI subsystem.
//      - Provide Clear() behavior for the UI subsystem.
//      - Provide Shutdown() behavior for the UI subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.UI.Elements;
using SASZombieAssaultTD.Engine.UI.Input;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI
//
{
    /// <summary>
    /// Complete UI system for SAS Zombie Assault TD. Manages UI elements, input handling, rendering, and state
    /// management.
    /// </summary>
    public class UISystem
    {
        private readonly List<UIElement> _elements = new();
        private readonly Dictionary<string, UIPanel> _panels = new();
        private bool _isVisible = true;
        private bool _inputEnabled = true;
        private Vector3 _screenSize;
        private bool _initialized;

        /// <summary>
        /// Whether the UI system is visible.
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Info, $"UI System visibility set to: {value}");
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
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Info, $"UI input enabled set to: {value}");
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
                DLogger.Log(LogSubsystems.ResourcesPipeline, "UI System already initialized");
                return;
            }

            try
            {
                ScreenSize = new Vector3(screenWidth, screenHeight, 0);
                _initialized = true;
                DLogger.Log($"UI System initialized ({screenWidth}x{screenHeight})");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.UISystems,
                    ex.ToString(), "Failed to initialize UI System");
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
                //Update all elements
                foreach (var element in _elements.Where(e => e.IsActive))
                    element.Update(deltaTime);

                //Update all panels
                foreach (var panel in _panels.Values.Where(p => p.IsActive))
                    panel.Update(deltaTime);
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.UISystems,
                    ex.ToString(), "Failed to update UI System");
            }
        }

        /// <summary>
        /// Render the UI system.
        /// </summary>
        /// <param name="renderContext">Render context.</param>
        public void Render(D3D11Adapter_Core renderContext)
        {
            if (!_initialized || !IsVisible) return;

            try
            {
                //Render all panels first (background)
                foreach (var panel in _panels.Values.Where(p => p.IsVisible))
                    panel.Render(renderContext);

                //Render all elements
                foreach (var element in _elements.Where(e => e.IsVisible))
                    element.Render(renderContext);
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.UISystems,
                    ex.ToString(), "Failed to render UI System");
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
                DLogger.Log(LogSubsystems.ResourcesPipeline, "Attempted to add null UI element");
                return;
            }

            if (_elements.Contains(element))
            {
                DLogger.Log($"Element {element.Name} already exists in UI system");
                return;
            }

            _elements.Add(element);
            element.Initialize?.Invoke();
            DLogger.Log($"Added UI element: {element.Name}");
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
                element.CleanUp();
                DLogger.Log(format: $"Removed UI element: {element.Name}");
            }
        }

        /// <summary>
        /// Get a UI element by name.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <returns>UI element or null if not found.</returns>
        public UIElement GetElement(string name) => _elements.FirstOrDefault(e => e.Name == name);

        /// <summary>
        /// Add a UI panel to the system.
        /// </summary>
        /// <param name="panel">Panel to add.</param>
        public void AddPanel(UIPanel panel)
        {
            if (panel == null)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "Attempted to add null UI panel");
                return;
            }

            if (_panels.ContainsKey(panel.Name))
            {
                DLogger.Log($"Panel {panel.Name} already exists in UI system");
                return;
            }

            _panels.Add(panel.Name, panel);
            panel.Initialize();
            DLogger.Log($"Added UI panel: {panel.Name}");
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
                DLogger.Log($"Removed UI panel: {panelName}");
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
                DLogger.Log($"Showed panel: {panelName}");
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
                DLogger.Log($"Hid panel: {panelName}");
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
                //Process input for visible elementsIsVisible
                foreach (var element in _elements.Where(e => !(!e.IsVisible ||
                // (!e.IsVisible && (!e.IsVisible && (!e.IsVisible()))))
                (!e.IsVisible && (!e.IsVisible && !e.IsVisible)))))
                {
                    if (element != null)
                    //              if (elementX.HandleInput(inputEvent))
                    {
                        //Input was handled, stop propagation
                        break;
                    }
                    continue;
                }
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.UISystems,
                    ex.ToString(), "Failed to handle UI input");
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
            DLogger.Log($"Screen size updated to {width}x{height}");
        }

        /// <summary>
        /// Clear all UI elements and panels.
        /// </summary>
        public void Clear()
        {
            //Cleanup all elements
            foreach (var element in _elements)
                element.Cleanup();

            _elements.Clear();

            //Cleanup all panels
            foreach (var panel in _panels.Values)
            {

                panel.Cleanup();
            }

            _panels.Clear();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "UI System cleared");
        }

        /// <summary>
        /// Shutdown the UI system.
        /// </summary>
        public void Shutdown()
        {
            if (!_initialized) return;

            Clear();
            _initialized = false;
            DLogger.Log(LogSubsystems.ResourcesPipeline, "UI System shutdown");
        }

        /// <summary>
        /// Update positions of all elements when screen size changes.
        /// </summary>
        private void UpdateElementPositions()
        {
            //Update all elements
            foreach (var element in _elements)
                element.UpdateLayout();
        }
    }
}
