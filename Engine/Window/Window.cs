// ====================================================================================================
//  FILE: Window.cs
//  PATH: ./Engine/Window/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the Window module.
//
//  RESPONSIBILITIES:
//      - Provide Open() behavior for the Core subsystem.
//      - Provide Close() behavior for the Core subsystem.
//      - Provide SetShouldClose() behavior for the Core subsystem.
//      - Provide PollEvents() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Numerics;
using SASZombieAssaultTD.Engine.Core;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
namespace SASZombieAssaultTD.Engine.Window
//
{
    ///<summary>
    ///Window management system for the game engine.
    ///P20-03-02: Implements window operations including Open, Close, ShouldClose.
    ///</summary>
    public class Window
    {
        private IntPtr _handle;
        private string _title;
        private Vector3 _size;
        private Vector3 _position;
        private bool _isOpened;
        private bool _shouldClose;
        private bool _isFocused;

        ///<summary>
        ///Gets the native window handle.
        ///</summary>
        public IntPtr Handle => _handle;

        ///<summary>
        ///Gets or sets the window title.
        ///</summary>
        public string Title
        {
            get => _title;
            set
            {
                _title = value ?? string.Empty;
                UpdateWindowTitle();
            }
        }

        ///<summary>
        ///Gets or sets the window size.
        ///</summary>
        public Vector3 Size
        {
            get => _size;
            set
            {
                _size = value;
                UpdateWindowSize();
            }
        }

        ///<summary>
        ///Gets or sets the window position.
        ///</summary>
        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                UpdateWindowPosition();
            }
        }

        ///<summary>
        ///Gets whether the window is currently open.
        ///</summary>
        public bool IsOpened => _isOpened;

        ///<summary>
        ///Gets whether the window should close.
        ///</summary>
        public bool ShouldClose => _shouldClose;

        ///<summary>
        ///Gets whether the window has focus.
        ///</summary>
        public bool IsFocused => _isFocused;

        ///<summary>
        ///Gets the window width.
        ///</summary>
        public int Width => (int)_size.X;

        ///<summary>
        ///Gets the window height.
        ///</summary>
        public int Height => (int)_size.Y;

        ///<summary>
        ///Event fired when window is opened.
        ///</summary>
        public event Action OnWindowOpened;

        ///<summary>
        ///Event fired when window is closed.
        ///</summary>
        public event Action OnWindowClosed;

        ///<summary>
        ///Event fired when window should close.
        ///</summary>
        public event Action<bool> OnShouldClose;

        ///<summary>
        ///Event fired when window is resized.
        ///</summary>
        public event Action<Vector3> OnWindowResized;

        ///<summary>
        ///Event fired when window focus changes.
        ///</summary>
        public event Action<bool> OnFocusChanged;

        ///<summary>
        ///Event fired when window is moved.
        ///</summary>
        public event Action<Vector3> OnWindowMoved;

        ///<summary>
        ///Initializes a new window instance.
        ///</summary>
        public Window()
        {
            _title = "SAS Zombie Assault TD";
            _size = new Vector3(800f, 600f, 0f);
            _position = Vector3.Zero;
            _isOpened = false;
            _shouldClose = false;
            _isFocused = false;
            _handle = IntPtr.Zero;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Window: Initialized with default settings");
        }

        ///<summary>
        ///Opens the window with specified parameters.
        ///</summary>
        ///<param name="title">Window title.</param>
        ///<param name="width">Window width.</param>
        ///<param name="height">Window height.</param>
        ///<param name="position">Window position.</param>
        ///<returns>True if window opened successfully.</returns>
        public bool Open(string? title = null, int width = 800, int height = 600, Vector3? position = null)
        {
            if (_isOpened)
            {
                DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, "WARNING", "Window: Window is already open");
                return false;
            }

            try
            {
                _title = title ?? _title;
                _size = new Vector3((float)width, (float)height, 0f);
                _position = position ?? _position;

                //Platform-specific window creation would go here
                //For now, we'll simulate successful creation
                _handle = new IntPtr(1); //Simulate window handle
                _isOpened = true;

                DLogger.Log(LogSubsystems.Window, LogEnums.LogLevel.Info, $"Window: Opened '{_title}' ({width}x{height}) at ({_position.X},{_position.Y})");
                OnWindowOpened?.Invoke();

                return true;
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Window, LogEnums.LogLevel.Info,"ERROR",$"Window: Failed to open window - {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Closes the window.
        ///</summary>
        ///<returns>True if window closed successfully.</returns>
        public bool Close()
        {
            if (!_isOpened)
            {
                DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, "WARNING", "Window: Window is not open");
                return false;
            }

            try
            {
                //Platform-specific window closing would go here
                //For now, we'll simulate successful closing
                _isOpened = false;
                _handle = IntPtr.Zero;

                DLogger.Log(LogSubsystems.Window, LogEnums.LogLevel.Info, $"Window: Closed '{_title}'");
                OnWindowClosed?.Invoke();

                return true;
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Window, LogEnums.LogLevel.Info,"ERROR",$"Window: Failed to close window - {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Sets whether the window should close.
        ///</summary>
        ///<param name="shouldClose">Whether the window should close.</param>
        public void SetShouldClose(bool shouldClose)
        {
            _shouldClose = shouldClose;
            DLogger.Log(LogSubsystems.Window, LogEnums.LogLevel.Info, $"Window: ShouldClose set to {shouldClose}");
            OnShouldClose?.Invoke(shouldClose);
        }

        ///<summary>
        ///Polls for window events and processes them.
        ///P20-03-03: Implements window message handling.
        ///</summary>
        public void PollEvents()
        {
            if (!_isOpened)
                return;

            try
            {
                //Platform-specific event polling would go here
                //For now, we'll simulate basic event processing

                //Simulate focus change events
                var newFocusState = SimulateFocusCheck();
                if (newFocusState != _isFocused)
                {
                    _isFocused = newFocusState;
                    DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, "DEBUG", $"Window: Focus changed to {newFocusState}");
                    OnFocusChanged?.Invoke(newFocusState);
                }

                //Simulate resize events
                var newSize = SimulateResizeCheck();
                if (newSize != _size)
                {
                    _size = newSize;
                    DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, "DEBUG", $"Window: Resized to {newSize.X}x{newSize.Y}");
                    OnWindowResized?.Invoke(newSize);
                }

                //Check if window should close
                if (_shouldClose)
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Window, LogEnums.LogLevel.Info,"ERROR",$"Window: Failed to poll events - {ex.Message}");
            }
        }

        ///<summary>
        ///Updates the window title.
        ///</summary>
        private void UpdateWindowTitle()
        {
            if (!_isOpened)
                return;

            //Platform-specific title update would go here
            DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, "DEBUG", $"Window: Updated title to '{_title}'");
        }

        ///<summary>
        ///Updates the window size.
        ///</summary>
        private void UpdateWindowSize()
        {
            if (!_isOpened)
                return;

            //Platform-specific size update would go here
            DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, "DEBUG", $"Window: Updated size to {_size.X}x{_size.Y}");
        }

        ///<summary>
        ///Updates the window position.
        ///</summary>
        private void UpdateWindowPosition()
        {
            if (!_isOpened)
                return;

            //Platform-specific position update would go here
            DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, "DEBUG", $"Window: Updated position to ({_position.X},{_position.Y})");
        }

        ///<summary>
        ///Simulates focus state checking.
        ///</summary>
        ///<returns>Current focus state.</returns>
        private bool SimulateFocusCheck()
        {
            //In a real implementation, this would check platform-specific focus state
            //For now, we'll return true if window is open
            return _isOpened;
        }

        ///<summary>
        ///Simulates resize checking.
        ///</summary>
        ///<returns>Current window size.</returns>
        private Vector3 SimulateResizeCheck()
        {
            //In a real implementation, this would check platform-specific resize events
            //For now, we'll return the current size
            return _size;
        }

        ///<summary>
        ///Gets window configuration information.
        ///</summary>
        ///<returns>Window configuration as a string.</returns>
        public override string ToString()
        {
            return $"Window: Title='{_title}', Size={_size.X}x{_size.Y}, " +
            $"Position=({_position.X},{_position.Y}), Open={_isOpened}, " +
            $"Focused={_isFocused}, ShouldClose={_shouldClose}";
        }
    }
}





