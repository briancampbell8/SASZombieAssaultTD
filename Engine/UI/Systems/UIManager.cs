using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.UI.Systems
//
{
    ///<summary>
    ///UI manager for handling UI elements and input.
    ///</summary>
    public class UIManager
    {
        private readonly List<UIElement> _uiElements = new();
        private readonly Dictionary<string, UIElement> _uiElementsById = new();
        private readonly Dictionary<string, string> _soundMappings = new()
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

        private Vector3 _viewportSize = new(800, 600, 0);
        private UIElement _focusedElement;
        private UIElement _hoveredElement;
        private bool _isInitialized;
        private bool _inputEnabled = true;
        private bool _soundEnabled = true;
        private float _soundVolume = 0.5f;

        public bool IsInitialized => _isInitialized;
        public Vector3 ViewportSize => _viewportSize;
        public int ElementCount => _uiElements.Count;
        public UIElement FocusedElement => _focusedElement;
        public UIElement HoveredElement => _hoveredElement;

        public bool InputEnabled
        {
            get => _inputEnabled;
            set => _inputEnabled = value;
        }

        public bool SoundEnabled
        {
            get => _soundEnabled;
            set
            {
                if (_soundEnabled != value)
                {
                    _soundEnabled = value;
                    DLogger.Log(LogSubsystems.UI, LogLevel.Debug, $"UIManager: UI sounds {(value ? "enabled" : "disabled")}");
                }
            }
        }

        public float SoundVolume
        {
            get => _soundVolume;
            set
            {
                _soundVolume = System.Math.Clamp(value, 0f, 1f);
                DLogger.Log(LogSubsystems.UI, LogLevel.Debug, $"UIManager: UI sound volume set to {_soundVolume:F2}");
            }
        }

        public event Action<UIElement> OnElementAdded;
        public event Action<UIElement> OnElementRemoved;
        public event Action<UIElement, UIElement> OnFocusChanged;
        public event Action<UIElement, UIElement> OnHoverChanged;
        public event Action<string> OnPlayUISound;

        public void Initialize(int viewportWidth = 800, int viewportHeight = 600)
        {
            if (_isInitialized)
            {
                DLogger.Log(LogSubsystems.UI, LogLevel.Warning, "UIManager: Already initialized");
                return;
            }

            _viewportSize = new Vector3(viewportWidth, viewportHeight, 0);
            _isInitialized = true;

            DLogger.Log(LogSubsystems.UI, LogLevel.Info, $"UIManager: Initialized with viewport {viewportWidth}x{viewportHeight}");
        }

        public void Update(float deltaTime)
        {
            if (!_isInitialized) return;

            try
            {
                _uiElements.ForEach(element => element.Update(deltaTime));
                if (_inputEnabled) HandleInput();
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.UI, LogLevel.Error, $"UIManager: Failed to update - {ex.Message}");
            }
        }

        public void Render(Renderer renderer)
        {
            if (!_isInitialized || renderer == null) return;

            try
            {
                _uiElements.ForEach(element => element.Render());
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.UI, LogLevel.Error, $"UIManager: Failed to render - {ex.Message}");
            }
        }

        //TODO: Implement UIManager with proper UIElement interface
        public bool AddElement(UIElement element)
        {
            //TODO: Implement when UIElement has required properties (Id, IsEnabled, etc.)
            return false;
        }

        //TODO: Implement UIManager with proper UIElement interface
        public bool RemoveElement(UIElement element)
        {
            //TODO: Implement when UIElement has required properties (Id, etc.)
            return false;
        }

        public bool RemoveElement(string id) =>
            !string.IsNullOrEmpty(id) && _uiElementsById.TryGetValue(id, out var element) && RemoveElement(element);

        public UIElement GetElement(string id) =>
            _uiElementsById.TryGetValue(id, out var element) ? element : null;

        public List<T> GetElements<T>() where T : UIElement =>
            _uiElements.OfType<T>().ToList();

        //TODO: Implement UIManager with proper UIElement interface
        public void SetFocus(UIElement element)
        {
            //TODO: Implement when UIElement has required properties (IsEnabled, SetFocus, etc.)
        }

        //TODO: Implement UIManager with proper UIElement interface
        public void ClearFocus()
        {
            //TODO: Implement when UIElement has required properties (RemoveFocus, etc.)
        }

        public void SetViewport(int width, int height)
        {
            _viewportSize = new Vector3(width, height, 0);
            DLogger.Log(LogSubsystems.UI, LogLevel.Debug, $"UIManager: Set viewport to {width}x{height}");
        }

        //TODO: Implement UIManager with proper UIElement interface
        private void HandleInput()
        {
            //TODO: Implement when UIElement has required properties (IsEnabled, Bounds, HandleInput, etc.)
        }

        public void ClearElements()
        {
            _uiElements.ForEach(element => OnElementRemoved?.Invoke(element));
            _uiElements.Clear();
            _uiElementsById.Clear();
            _focusedElement = null;
            _hoveredElement = null;

            DLogger.Log(LogSubsystems.UI, LogLevel.Debug, "UIManager: Cleared all elements");
        }

        public void PlayUISound(string soundType)
        {
            if (!_soundEnabled || string.IsNullOrEmpty(soundType)) return;

            if (_soundMappings.TryGetValue(soundType.ToLower(), out var soundName))
            {
                OnPlayUISound?.Invoke(soundName);
                DLogger.Log(LogSubsystems.UI, LogLevel.Debug, $"UIManager: Playing UI sound '{soundName}' for type '{soundType}'");
            }
            else
            {
                DLogger.Log(LogSubsystems.UI, LogLevel.Warning, $"UIManager: Unknown UI sound type '{soundType}'");
            }
        }

        public void SetSoundMapping(string eventType, string soundName)
        {
            if (!string.IsNullOrEmpty(eventType) && !string.IsNullOrEmpty(soundName))
            {
                _soundMappings[eventType.ToLower()] = soundName;
                DLogger.Log(LogSubsystems.UI, LogLevel.Debug, $"UIManager: Set sound mapping '{eventType}' -> '{soundName}'");
            }
        }

        public string GetSoundMapping(string eventType) =>
            _soundMappings.TryGetValue(eventType?.ToLower(), out var soundName) ? soundName : null;

        public void RemoveSoundMapping(string eventType)
        {
            if (_soundMappings.Remove(eventType?.ToLower()))
            {
                DLogger.Log(LogSubsystems.UI, LogLevel.Debug, $"UIManager: Removed sound mapping for '{eventType}'");
            }
        }
    }
}

























