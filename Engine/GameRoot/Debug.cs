/*
File:    Debug.cs
Path:    Engine/GameRoot/Debug.cs
Purpose: P11-09-01 - Contains all developer-only tools and debugging utilities.
         Keeps debug logic isolated from core engine behavior.

Role:     Debug and diagnostics specialist.
         - Debug overlay functions
         - Logging functions
         - Cheat command functions
         - Diagnostic counters
         - Performance monitoring
         - Developer utilities

Notes:    Contains all debug logic extracted from GameRoot.
         Debug functionality is isolated to prevent production impact.
         All developer tools are centralized for easy maintenance.
*/

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine
{
    /// <summary>
    /// Partial class containing debug logic for GameRoot.
    /// </summary>
    public partial class GameRoot
    {
        private bool _debugMode = false;
        private readonly DiagnosticCounters _diagnosticCounters = new();

        /// <summary>
        /// Gets or sets whether debug mode is enabled.
        /// </summary>
        public bool DebugMode
        {
            get => _debugMode;
            set
            {
                _debugMode = value;
                ModernLoggingSystem.LogInfo($"Debug mode {(value ? "enabled" : "disabled")}");
            }
        }

        /// <summary>
        /// Gets the diagnostic counters.
        /// </summary>
        public DiagnosticCounters DiagnosticCounters => _diagnosticCounters;

        /// <summary>
        /// Gets engine diagnostic information.
        /// </summary>
        /// <returns>Engine diagnostic information.</returns>
        internal EngineDiagnostics GetEngineDiagnostics()
        {
            return new EngineDiagnostics
            {
                State = State,
                IsInitialized = _isInitialized,
                IsRunning = _isRunning,
                FrameAccumulator = _frameAccumulator,
                TargetFrameTime = TargetFrameTime,
                SystemManagerDiagnostics = new SASZombieAssaultTD.Engine.Interfaces.ManagerDiagnostics { Name = "SystemManager", Status = SASZombieAssaultTD.Engine.Interfaces.ManagerStatus.Active },
                UpdateManagerDiagnostics = new SASZombieAssaultTD.Engine.Interfaces.ManagerDiagnostics { Name = "UpdateManager", Status = SASZombieAssaultTD.Engine.Interfaces.ManagerStatus.Active },
                RenderManagerDiagnostics = new SASZombieAssaultTD.Engine.Interfaces.ManagerDiagnostics { Name = "RenderManager", Status = SASZombieAssaultTD.Engine.Interfaces.ManagerStatus.Active },
                InputManagerDiagnostics = _inputManager != null ? new SASZombieAssaultTD.Engine.Interfaces.ManagerDiagnostics { Name = "InputManager", Status = SASZombieAssaultTD.Engine.Interfaces.ManagerStatus.Active } : new SASZombieAssaultTD.Engine.Interfaces.ManagerDiagnostics { Name = "InputManager", Status = SASZombieAssaultTD.Engine.Interfaces.ManagerStatus.Inactive },
            };
        }

        /// <summary>
        /// Enables debug overlay.
        /// </summary>
        public void EnableDebugOverlay()
        {
            if (!_debugMode)
            {
                ModernLoggingSystem.LogWarning("Cannot enable debug overlay - debug mode is disabled");
                return;
            }

            try
            {
                // Enable debug overlay logic here
                ModernLoggingSystem.LogInfo("Debug overlay enabled");
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"Failed to enable debug overlay: {ex.Message}");
                ModernLoggingSystem.Exception(ex, "Debug overlay enable");
            }
        }

        /// <summary>
        /// Disables debug overlay.
        /// </summary>
        public void DisableDebugOverlay()
        {
            try
            {
                // Disable debug overlay logic here
                ModernLoggingSystem.LogInfo("Debug overlay disabled");
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"Failed to disable debug overlay: {ex.Message}");
                ModernLoggingSystem.Exception(ex, "Debug overlay disable");
            }
        }

        /// <summary>
        /// Executes a debug command.
        /// </summary>
        /// <param name="command">The debug command to execute.</param>
        /// <returns>True if the command was executed successfully.</returns>
        public bool ExecuteDebugCommand(string command)
        {
            if (!_debugMode)
            {
                ModernLoggingSystem.LogWarning("Cannot execute debug command - debug mode is disabled");
                return false;
            }

            if (string.IsNullOrEmpty(command))
            {
                ModernLoggingSystem.LogWarning("Debug command is null or empty");
                return false;
            }

            try
            {
                var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                    return false;

                var cmd = parts[0].ToLower();
                var args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

                return ExecuteDebugCommandInternal(cmd, args);
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"Failed to execute debug command '{command}': {ex.Message}");
                ModernLoggingSystem.Exception(ex, "Debug command");
                return false;
            }
        }

        /// <summary>
        /// Internal debug command execution.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="args">The command arguments.</param>
        /// <returns>True if the command was executed successfully.</returns>
        private bool ExecuteDebugCommandInternal(string command, string[] args)
        {
            switch (command)
            {
                case "help":
                    ModernLoggingSystem.LogInfo("Available debug commands: help, stats, fps, memory, scenes, systems");
                    return true;

                case "stats":
                    LogEngineStats();
                    return true;

                case "fps":
                    LogFPSInfo();
                    return true;

                case "memory":
                    LogMemoryInfo();
                    return true;

                case "scenes":
                    LogSceneInfo();
                    return true;

                case "systems":
                    LogSystemInfo();
                    return true;

                default:
                    ModernLoggingSystem.LogWarning($"Unknown debug command: {command}");
                    return false;
            }
        }

        /// <summary>
        /// Logs engine statistics.
        /// </summary>
        private void LogEngineStats()
        {
            var diagnostics = GetEngineDiagnostics();
            ModernLoggingSystem.LogInfo($"Engine State: {diagnostics.State}, Initialized: {diagnostics.IsInitialized}, Running: {diagnostics.IsRunning}");
        }

        /// <summary>
        /// Logs FPS information.
        /// </summary>
        private void LogFPSInfo()
        {
            ModernLoggingSystem.LogInfo($"Target Frame Time: {TargetFrameTime:F4}s ({(1f / TargetFrameTime):F1} FPS)");
            ModernLoggingSystem.LogInfo($"Frame Accumulator: {_frameAccumulator:F4}s");
        }

        /// <summary>
        /// Logs memory information.
        /// </summary>
        private void LogMemoryInfo()
        {
            var memoryUsage = GC.GetTotalMemory(false);
            ModernLoggingSystem.LogInfo($"Managed Memory Usage: {memoryUsage / 1024 / 1024:F1} MB");
        }

        /// <summary>
        /// Logs scene information.
        /// </summary>
        private void LogSceneInfo()
        {
            var sceneNames = GetLoadedSceneNames();
            var currentScene = GetCurrentScene();

            ModernLoggingSystem.LogInfo($"Loaded Scenes: {sceneNames.Length}");
            ModernLoggingSystem.LogInfo($"Current Scene: {currentScene?.GetType().Name ?? "None"}");

            foreach (var sceneName in sceneNames)
            {
                ModernLoggingSystem.LogInfo($"  - {sceneName}");
            }
        }

        /// <summary>
        /// Logs system information.
        /// </summary>
        private void LogSystemInfo()
        {
            ModernLoggingSystem.LogInfo("Engine Systems Status:");
            ModernLoggingSystem.LogInfo($"  SystemManager: {_systemManager != null}");
            ModernLoggingSystem.LogInfo($"  UpdateManager: {_updateManager != null}");
            ModernLoggingSystem.LogInfo($"  RenderManager: {_renderManager != null}");
            ModernLoggingSystem.LogInfo($"  InputManager: {_inputManager != null}");
            ModernLoggingSystem.LogInfo($"  GameStateMachine: {_stateMachine != null}");
            ModernLoggingSystem.LogInfo($"  RenderContext: {_renderContext != null}");
        }

        /// <summary>
        /// Increments a diagnostic counter.
        /// </summary>
        /// <param name="counterName">The name of the counter.</param>
        /// <param name="value">The value to increment by.</param>
        public void IncrementDiagnosticCounter(string counterName, long value = 1)
        {
            _diagnosticCounters.Increment(counterName, value);
        }

        /// <summary>
        /// Gets the value of a diagnostic counter.
        /// </summary>
        /// <param name="counterName">The name of the counter.</param>
        /// <returns>The counter value.</returns>
        public long GetDiagnosticCounter(string counterName)
        {
            return _diagnosticCounters.Get(counterName);
        }

        /// <summary>
        /// Resets all diagnostic counters.
        /// </summary>
        public void ResetDiagnosticCounters()
        {
            _diagnosticCounters.ResetAll();
            ModernLoggingSystem.LogInfo("All diagnostic counters reset");
        }
    }

    /// <summary>
    /// Simple diagnostic counter manager for debugging.
    /// </summary>
    public class DiagnosticCounters
    {
        private readonly Dictionary<string, long> _counters = new();

        /// <summary>
        /// Increments a counter.
        /// </summary>
        /// <param name="name">The counter name.</param>
        /// <param name="value">The value to increment by.</param>
        public void Increment(string name, long value = 1)
        {
            lock (_counters)
            {
                if (_counters.ContainsKey(name))
                    _counters[name] += value;
                else
                    _counters[name] = value;
            }
        }

        /// <summary>
        /// Gets a counter value.
        /// </summary>
        /// <param name="name">The counter name.</param>
        /// <returns>The counter value.</returns>
        public long Get(string name)
        {
            lock (_counters)
            {
                return _counters.TryGetValue(name, out var value) ? value : 0;
            }
        }

        /// <summary>
        /// Resets all counters.
        /// </summary>
        public void ResetAll()
        {
            lock (_counters)
            {
                _counters.Clear();
            }
        }

        /// <summary>
        /// Gets all counter values.
        /// </summary>
        /// <returns>Dictionary of all counters.</returns>
        public Dictionary<string, long> GetAll()
        {
            lock (_counters)
            {
                return new Dictionary<string, long>(_counters);
            }
        }
    }
}
