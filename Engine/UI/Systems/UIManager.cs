// ====================================================================================================
//  FILE: UIManager.cs
//  PATH: ./Engine/UI/Systems/
//  SUBSYSTEM: UI Subsystem / Core Systems
//
//  ROLE:
//      Central coordinator for UI element lifecycle, input routing, focus management,
//      hover detection, sound mapping, and viewport configuration.
//
//  RESPONSIBILITIES:
//      - Initialize UI subsystem state.
//      - Maintain deterministic UIElement registration and lookup tables.
//      - Route InputState snapshots to UIElements for interaction handling.
//      - Manage focus and hover state transitions.
//      - Provide AddElement(), RemoveElement(), GetElement(), ClearElements().
//      - Provide SetFocus(), ClearFocus(), and hover tracking.
//      - Provide UI sound mapping and dispatch.
//      - Provide viewport configuration.
//
//  NON-RESPONSIBILITIES:
//      - Rendering GPU commands or low-level draw calls.
//      - Asset loading, file I/O, or serialization.
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Input;
using SASZombieAssaultTD.Engine.Render;
using SASZombieAssaultTD.Engine.UI.Elements;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI.Systems
{
    public class UIManager
    {
        private readonly Dictionary<string, string> _soundMappings = new(StringComparer.OrdinalIgnoreCase)
        {
            ["hover"] = "ui_hover",
            ["click"] = "ui_click",
            ["focus"] = "ui_focus",
            ["back"] = "ui_back",
            ["confirm"] = "ui_confirm",
            ["cancel"] = "ui_cancel",
            ["error"] = "ui_error",
            ["success"] = "ui_success"
        };

        private readonly List<UIElement> _uiElements = new();
        private readonly Dictionary<string, UIElement> _uiElementsById = new();
        private UIElement _focusedElement;
        private UIElement _hoveredElement;
        private bool _inputEnabled = true;
        private bool _isInitialized;
        private bool _soundEnabled = true;
        private float _soundVolume = 0.5f;
        private Vector3 _viewportSize = new(800, 600, 0);
        public event Action<UIElement> OnElementAdded;
        public event Action<UIElement> OnElementRemoved;
        public event Action<UIElement, UIElement> OnFocusChanged;
        public event Action<UIElement, UIElement> OnHoverChanged;
        public event Action<string> OnPlayUISound;

        public int ElementCount => _uiElements.Count;
        public UIElement FocusedElement => _focusedElement;
        public UIElement HoveredElement => _hoveredElement;
        public bool InputEnabled
        {
            get => _inputEnabled;
            set => _inputEnabled = value;
        }

        public bool IsInitialized => _isInitialized;
        public bool SoundEnabled
        {
            get => _soundEnabled;
            set
            {
                if (_soundEnabled != value)
                {
                    _soundEnabled = value;
                    DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Debug,
                        $"UIManager: UI sounds {(value ? "enabled" : "disabled")}");
                }
            }
        }

        public float SoundVolume
        {
            get => _soundVolume;
            set
            {
                _soundVolume = System.Math.Clamp(value, 0f, 1f);
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Debug,
                    $"UIManager: UI sound volume set to {_soundVolume:F2}");
            }
        }

        public Vector3 ViewportSize => _viewportSize;
        // ====================================================================================================
        //  ELEMENT MANAGEMENT
        // ====================================================================================================
        public bool AddElement(UIElement element)
        {
            if (element == null || string.IsNullOrEmpty(element.Id))
                return false;

            if (_uiElementsById.ContainsKey(element.Id))
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Warning,
                    $"UIManager: Element with ID '{element.Id}' already exists");
                return false;
            }

            _uiElements.Add(element);
            _uiElementsById[element.Id] = element;

            OnElementAdded?.Invoke(element);

            return true;
        }

        // ====================================================================================================
        //  CLEAR ELEMENTS
        // ====================================================================================================
        public void ClearElements()
        {
            // Iterate over a copy to avoid potential modification during event callbacks.
            foreach (var element in _uiElements.ToArray())
                OnElementRemoved?.Invoke(element);

            _uiElements.Clear();
            _uiElementsById.Clear();

            _focusedElement = null;
            _hoveredElement = null;

            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Debug,
                "UIManager: Cleared all elements");
        }

        public void ClearFocus()
        {
            if (_focusedElement == null)
                return;

            var previous = _focusedElement;
            previous.ReleaseFocus();

            _focusedElement = null;

            OnFocusChanged?.Invoke(previous, null);
        }

        public UIElement GetElement(string id) =>
                    _uiElementsById.TryGetValue(id, out var element) ? element : null;

        public List<T> GetElements<T>() where T : UIElement
        {
            // Manual iteration avoids LINQ allocations and multiple enumerators.
            var result = new List<T>(_uiElements.Count);
            foreach (var e in _uiElements)
            {
                if (e is T t) result.Add(t);
            }
            return result;
        }

        public string GetSoundMapping(string eventType) =>
                    !string.IsNullOrEmpty(eventType) && _soundMappings.TryGetValue(eventType, out var soundName)
                        ? soundName
                        : null;

        // ====================================================================================================
        //  INITIALIZATION
        // ====================================================================================================
        public void Initialize(int viewportWidth = 800, int viewportHeight = 600)
        {
            if (_isInitialized)
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Debug, "UIManager: Already initialized");
                return;
            }

            _viewportSize = new Vector3(viewportWidth, viewportHeight, 0);
            _isInitialized = true;

            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Debug,
                $"UIManager: Initialized with viewport {_viewportSize.X}x{_viewportSize.Y}");
        }

        // ====================================================================================================
        //  SOUND MAPPING
        // ====================================================================================================
        public void PlayUISound(string soundType)
        {
            if (!_soundEnabled || string.IsNullOrEmpty(soundType))
                return;

            if (_soundMappings.TryGetValue(soundType.ToLower(), out var soundName))
            {
                OnPlayUISound?.Invoke(soundName);
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Debug,
                    $"UIManager: Playing UI sound '{soundName}' for type '{soundType}'");
            }
            else
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Warning,
                    $"UIManager: Unknown UI sound type '{soundType}'");
            }
        }

        public bool RemoveElement(UIElement element)
        {
            if (element == null || string.IsNullOrEmpty(element.Id))
                return false;

            if (!_uiElements.Remove(element))
                return false;

            _uiElementsById.Remove(element.Id);

            if (_focusedElement == element)
                ClearFocus();

            if (_hoveredElement == element)
                _hoveredElement = null;

            OnElementRemoved?.Invoke(element);

            return true;
        }

        public bool RemoveElement(string id) =>
                    !string.IsNullOrEmpty(id) &&
                    _uiElementsById.TryGetValue(id, out var element) &&
                    RemoveElement(element);

        public void RemoveSoundMapping(string eventType)
        {
            if (_soundMappings.Remove(eventType?.ToLower()))
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Debug,
                    $"UIManager: Removed sound mapping for '{eventType}'");
            }
        }

        // ====================================================================================================
        //  RENDER
        // ====================================================================================================
        public void Render(Renderer renderer)
        {
            if (!_isInitialized || renderer == null) return;

            try
            {
                foreach (var element in _uiElements)
                    element.Render();
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Error,
                    $"UIManager: Failed to render - {ex.Message}");
            }
        }

        // ====================================================================================================
        //  FOCUS MANAGEMENT
        // ====================================================================================================
        public void SetFocus(UIElement element)
        {
            if (element == null || !element.IsEnabled || !element.IsFocusable)
                return;

            var previous = _focusedElement;

            if (previous == element)
                return;

            previous?.ReleaseFocus();

            _focusedElement = element;
            _focusedElement.RequestFocus();

            OnFocusChanged?.Invoke(previous, _focusedElement);
        }

        public void SetSoundMapping(string eventType, string soundName)
        {
            if (!string.IsNullOrEmpty(eventType) && !string.IsNullOrEmpty(soundName))
            {
                _soundMappings[eventType.ToLower()] = soundName;
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Debug,
                    $"UIManager: Set sound mapping '{eventType}' -> '{soundName}'");
            }
        }

        // ====================================================================================================
        //  VIEWPORT
        // ====================================================================================================
        public void SetViewport(int width, int height)
        {
            _viewportSize = new Vector3(width, height, 0);
            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Debug,
                $"UIManager: Set viewport to {width}x{height}");
        }

        // ====================================================================================================
        //  UPDATE
        // ====================================================================================================
        public void Update(float deltaTime, InputState input)
        {
            if (!_isInitialized) return;

            try
            {
                foreach (var element in _uiElements)
                    element.Update(deltaTime);

                if (_inputEnabled)
                    HandleInput(input);
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Error,
                    $"UIManager: Failed to update - {ex.Message}");
            }
        }
        // ====================================================================================================
        //  INPUT ROUTING (MODE A: Topmost element under mouse)
        // ====================================================================================================
        private void HandleInput(InputState input)
        {
            var mousePoint = input.MousePoint;

            // Find topmost element under mouse
            UIElement hovered = null;

            for (int i = _uiElements.Count - 1; i >= 0; i--)
            {
                var element = _uiElements[i];

                if (!element.IsEnabled || !element.IsVisible)
                    continue;

                if (element.HitTest(mousePoint))
                {
                    hovered = element;
                    break;
                }
            }

            // Hover change detection
            if (_hoveredElement != hovered)
            {
                var previous = _hoveredElement;
                _hoveredElement = hovered;

                OnHoverChanged?.Invoke(previous, hovered);

                if (hovered != null)
                    PlayUISound("hover");
            }

            // Click routing
            if (hovered != null && input.LeftMouseClicked)
            {
                SetFocus(hovered);
                PlayUISound("click");
            }

            // Route input to focused element
            _focusedElement?.HandleInput(input);
        }
    }
}
