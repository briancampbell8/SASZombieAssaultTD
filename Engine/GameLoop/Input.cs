// ====================================================================================================
//  FILE: Input.cs
//  PATH: Engine/GameLoop/Input.cs
//  PURPOSE: P11-09-01 — Contains all input-related operations for GameLoop.
//           Handles input polling and routing within the game loop.
//
//  ROLE: Game loop input specialist.
//      - Input polling coordination
//      - UIInputRouter integration
//      - InputDiagnostics class
//      - Input state management
//      - Input timing coordination
//
//  NOTES:
//      - Contains all input logic extracted from GameLoop.
//      - Delegates to UIInputRouter but coordinates timing and diagnostics.
//      - Provides clean separation of input concerns.
// ====================================================================================================


using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.UI.Input;
namespace SASZombieAssaultTD.Engine.Systems
//
{
    ///<summary>
    ///Partial class containing input logic for GameLoop.
    ///</summary>
    public partial class GameLoop
    {
        //Input tracking
        private float _lastInputTime = 0f;
        private int _inputEventsProcessed = 0;
        private readonly object _inputLock = new();

        ///<summary>
        ///Gets the input router for input operations.
        ///</summary>
        public UIInputRouter InputRouter => _input;

        ///<summary>
        ///Gets whether input is currently active.
        ///</summary>
        public bool IsInputActive => _input != null && _isRunning;

        ///<summary>
        ///Gets the number of input events processed.
        ///</summary>
        public int InputEventsProcessed => _inputEventsProcessed;

        ///<summary>
        ///Gets the last input time.
        ///</summary>
        public float LastInputTime => _lastInputTime;

        ///<summary>
        ///Processes input for the current frame.
        ///</summary>
        ///<param name="deltaTime">Time since last frame in seconds.</param>
        private void ProcessInputFrame(float deltaTime)
        {
            if (_input == null || !_isRunning)
                return;

            try
            {
                lock (_inputLock)
                {
                    var inputStartTime = DateTime.Now;

                    //Process input through the input router
                    _input.ProcessInput(deltaTime);

                    //Update input statistics
                    UpdateInputStatistics(inputStartTime);

                    DLogger.Log($"Input processed in {deltaTime:F4}s");
                }
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.GameLoop, LogLevel.Info, "ERROR", $"Input processing failed: {ex.Message}");
                DLogger.Log(
                    LogSubsystems.GameLoop,
                    LogLevel.Info, ex.ToString(),
                     "Input processing");
                _diagnostics.RecordFrameError(ex);
            }
        }

        ///<summary>
        ///Updates input statistics.
        ///</summary>
        ///<param name="inputStartTime">The time when input processing started.</param>
        private void UpdateInputStatistics(DateTime inputStartTime)
        {
            var inputProcessingTime = (float)(DateTime.Now - inputStartTime).TotalSeconds;
            _lastInputTime = inputProcessingTime;
            _inputEventsProcessed++;
        }

        ///<summary>
        ///Gets detailed input diagnostics.
        ///</summary>
        ///<returns>Input diagnostics information.</returns>
        public InputDiagnostics GetDetailedInputDiagnostics()
        {
            lock (_inputLock)
            {
                return new InputDiagnostics
                {
                    IsInputActive = IsInputActive,
                    InputEventsProcessed = _inputEventsProcessed,
                    LastInputTime = _lastInputTime,
                    MousePosition = GetMousePosition(),
                    KeyStates = GetKeyStates()
                };
            }
        }

        ///<summary>
        ///Gets the current mouse position.
        ///</summary>
        ///<returns>The current mouse position.</returns>
        private System.Drawing.Point GetMousePosition()
        {
            try
            {
                var mousePos = _input?.GetMousePosition();
                return mousePos.HasValue ? new System.Drawing.Point((int)mousePos.Value.X, (int)mousePos.Value.Y) : System.Drawing.Point.Empty;
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.GameLoop, LogLevel.Info, "ERROR", $"Failed to get mouse position: {ex.Message}");
                DLogger.Log(
                    LogSubsystems.GameLoop,
                    LogLevel.Info,
                    ex.ToString(), "Mouse position");
                return System.Drawing.Point.Empty;
            }
        }

        ///<summary>
        ///Gets the current keyboard state.
        ///</summary>
        ///<returns>Array of key states.</returns>
        private bool[] GetKeyStates()
        {
            try
            {
                //This would be implemented based on the actual input system
                return _input?.GetKeyStates() ?? new bool[256];
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.GameLoop, LogLevel.Info, "ERROR", $"Failed to get key states: {ex.Message}");
                DLogger.Log(
                    LogSubsystems.GameLoop, LogLevel.Info, ex.ToString(), "Key states");
                return new bool[256];
            }
        }

        ///<summary>
        ///Resets input statistics.
        ///</summary>
        public void ResetInputStatistics()
        {
            lock (_inputLock)
            {
                _inputEventsProcessed = 0;
                _lastInputTime = 0f;
            }
        }

        ///<summary>
        ///Enables or disables input processing.
        ///</summary>
        ///<param name="enabled">Whether input should be enabled.</param>
        public void SetInputEnabled(bool enabled)
        {
            if (_input != null)
            {
                _input.SetEnabled(enabled);
                DLogger.Log($"Input processing {(enabled ? "enabled" : "disabled")}");
            }
        }

        ///<summary>
        ///Gets input performance metrics.
        ///</summary>
        ///<returns>Input performance metrics.</returns>
        public InputMetrics GetInputMetrics()
        {
            lock (_inputLock)
            {
                return new InputMetrics
                {
                    EventsProcessed = _inputEventsProcessed,
                    AverageProcessingTime = _lastInputTime,
                    IsActive = IsInputActive,
                    ProcessingRate = _frameCount > 0 ? (float)_inputEventsProcessed / _frameCount : 0f
                };
            }
        }
    }

    ///<summary>
    ///Input performance metrics.
    ///</summary>
    public class InputMetrics
    {
        public int EventsProcessed { get; set; }
        public float AverageProcessingTime { get; set; }
        public bool IsActive { get; set; }
        public float ProcessingRate { get; set; }
    }
}
