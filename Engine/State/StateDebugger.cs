using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//
using SASZombieAssaultTD.Engine.Core;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.State
{
    ///<summary>
    ///Debugging and monitoring utilities for the state machine system.
    ///P20-02-Enhancement: Comprehensive debugging and profiling tools.
    ///</summary>
    public static class StateDebugger
    {
        private static readonly List<StateDebugEvent> _debugEvents = new List<StateDebugEvent>();
        private static readonly object _debugLock = new object();
        private static bool _debugEnabled = true;
        private static int _maxDebugEvents = 1000;
        
        ///<summary>
        ///Enables or disables state debugging.
        ///</summary>
        public static bool DebugEnabled
        {
            get => _debugEnabled;
            set => _debugEnabled = value;
        }
        
        ///<summary>
        ///Gets or sets the maximum number of debug events to keep.
        ///</summary>
        public static int MaxDebugEvents
        {
            get => _maxDebugEvents;
            set => _maxDebugEvents = System.Math.Max(0, value);
        }
        
        ///<summary>
        ///Logs a state machine event for debugging.
        ///</summary>
        ///<param name="eventType">The type of debug event.</param>
        ///<param name="stateType">The state type involved.</param>
        ///<param name="message">The debug message.</param>
        ///<param name="data">Additional data associated with the event.</param>
        public static void LogStateEvent(StateDebugEventType eventType, GameStateType stateType, string message, object? data = null)
        {
            if (!_debugEnabled)
            return;
            
            lock (_debugLock)
            {
                var debugEvent = new StateDebugEvent
                {
                    Timestamp = DateTime.UtcNow,
                    EventType = eventType,
                    StateType = stateType,
                    Message = message,
                    Data = data
                };
                
                _debugEvents.Add(debugEvent);
                
                //Maintain maximum event count
                while (_debugEvents.Count > _maxDebugEvents)
                {
                    _debugEvents.RemoveAt(0);
                }
                
                DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "DEBUG", $"StateDebugger: [{eventType}] {stateType} - {message}");
            }
        }
        
        ///<summary>
        ///Gets all debug events.
        ///</summary>
        ///<returns>Read-only list of debug events.</returns>
        public static IReadOnlyList<StateDebugEvent> GetDebugEvents()
        {
            lock (_debugLock)
            {
                return _debugEvents.AsReadOnly();
            }
        }
        
        ///<summary>
        ///Gets debug events filtered by event type.
        ///</summary>
        ///<param name="eventType">The event type to filter by.</param>
        ///<returns>Filtered list of debug events.</returns>
        public static IReadOnlyList<StateDebugEvent> GetDebugEvents(StateDebugEventType eventType)
        {
            lock (_debugLock)
            {
                return _debugEvents.Where(e => e.EventType == eventType).ToList().AsReadOnly();
            }
        }
        
        ///<summary>
        ///Gets debug events filtered by state type.
        ///</summary>
        ///<param name="stateType">The state type to filter by.</param>
        ///<returns>Filtered list of debug events.</returns>
        public static IReadOnlyList<StateDebugEvent> GetDebugEvents(GameStateType stateType)
        {
            lock (_debugLock)
            {
                return _debugEvents.Where(e => e.StateType == stateType).ToList().AsReadOnly();
            }
        }
        
        ///<summary>
        ///Gets debug events within a time range.
        ///</summary>
        ///<param name="startTime">The start time.</param>
        ///<param name="endTime">The end time.</param>
        ///<returns>Filtered list of debug events.</returns>
        public static IReadOnlyList<StateDebugEvent> GetDebugEvents(DateTime startTime, DateTime endTime)
        {
            lock (_debugLock)
            {
                return _debugEvents.Where(e => e.Timestamp >= startTime && e.Timestamp <= endTime).ToList().AsReadOnly();
            }
        }
        
        ///<summary>
        ///Clears all debug events.
        ///</summary>
        public static void ClearDebugEvents()
        {
            lock (_debugLock)
            {
                _debugEvents.Clear();
                DLogger.Log(LogSubsystems.State,LogLevel.Info, "StateDebugger: Debug events cleared");
            }
        }
        
        ///<summary>
        ///Exports debug events to a file.
        ///</summary>
        ///<param name="filePath">The file path to export to.</param>
        public static void ExportDebugEvents(string filePath)
        {
            lock (_debugLock)
            {
                try
                {
                    var lines = new List<string>
                    {
                        "State Machine Debug Log",
                        $"Exported: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}",
                        $"Total Events: {_debugEvents.Count}",
                        ""
                    };
                    
                    foreach (var debugEvent in _debugEvents)
                    {
                        lines.Add($"{debugEvent.Timestamp:HH:mm:ss.fff} | {debugEvent.EventType} | {debugEvent.StateType} | {debugEvent.Message}");
                        if (debugEvent.Data != null)
                        {
                            lines.Add($"    Data: {debugEvent.Data}");
                        }
                    }
                    
                    File.WriteAllLines(filePath, lines);
                    DLogger.Log(LogSubsystems.State,LogLevel.Info, $"StateDebugger: Exported {_debugEvents.Count} debug events to {filePath}");
                }
                catch (Exception ex)
                {
 DLogger.Log(LogSubsystems.State,LogLevel.Info,"ERROR",$"StateDebugger: Failed to export debug events - {ex.Message}");
                    throw;
                }
            }
        }
        
        ///<summary>
        ///Generates a state machine performance report.
        ///</summary>
        ///<param name="stateMachine">The state machine to analyze.</param>
        ///<returns>Performance report as a string.</returns>
        public static string GeneratePerformanceReport(StateMachine stateMachine)
        {
            if (stateMachine == null)
            throw new ArgumentNullException(nameof(stateMachine));
            
            var report = new List<string>
            {
                "State Machine Performance Report",
                $"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}",
                ""
            };
            
            //Basic statistics
            report.Add("=== Basic Statistics ===");
            report.Add($"Current State: {stateMachine.CurrentStateType}");
            report.Add($"Registered States: {stateMachine.GetRegisteredStates().Length}");
            report.Add($"Valid Transitions: {stateMachine.GetValidTransitions().Length}");
            report.Add("");
            
            //Debug event statistics
            lock (_debugLock)
            {
                report.Add("=== Debug Event Statistics ===");
                report.Add($"Total Debug Events: {_debugEvents.Count}");
                
                var eventCounts = _debugEvents.GroupBy(e => e.EventType)
                .ToDictionary(g => g.Key, g => g.Count());
                
                foreach (var kvp in eventCounts.OrderByDescending(x => x.Value))
                {
                    report.Add($"{kvp.Key}: {kvp.Value} events");
                }
                report.Add("");
                
                //State-specific statistics
                report.Add("=== State-Specific Statistics ===");
                var stateCounts = _debugEvents.GroupBy(e => e.StateType)
                .ToDictionary(g => g.Key, g => g.Count());
                
                foreach (var kvp in stateCounts.OrderByDescending(x => x.Value))
                {
                    report.Add($"{kvp.Key}: {kvp.Value} events");
                }
                report.Add("");
                
                //Recent events
                report.Add("=== Recent Events (Last 10) ===");
                var recentEvents = _debugEvents.TakeLast(10);
                foreach (var debugEvent in recentEvents)
                {
                    report.Add($"{debugEvent.Timestamp:HH:mm:ss.fff} | {debugEvent.EventType} | {debugEvent.StateType} | {debugEvent.Message}");
                }
            }
            
            return string.Join(Environment.NewLine, report);
        }
        
        ///<summary>
        ///Validates state machine configuration and reports issues.
        ///</summary>
        ///<param name="stateMachine">The state machine to validate.</param>
        ///<returns>List of validation issues.</returns>
        public static List<string> ValidateStateMachine(StateMachine stateMachine)
        {
            if (stateMachine == null)
            throw new ArgumentNullException(nameof(stateMachine));
            
            var issues = new List<string>();
            
            //Check if current state is registered
            if (stateMachine.CurrentState != null && !stateMachine.HasState(stateMachine.CurrentStateType))
            {
                issues.Add($"Current state {stateMachine.CurrentStateType} is not registered");
            }
            
            //Check for required states
            var requiredStates = new[] { GameStateType.Boot, GameStateType.MainMenu, GameStateType.Gameplay, GameStateType.Paused };
            foreach (var requiredState in requiredStates)
            {
                if (!stateMachine.HasState(requiredState))
                {
                    issues.Add($"Required state {requiredState} is not registered");
                }
            }
            
            //Check for valid transitions
            var registeredStates = stateMachine.GetRegisteredStates();
            foreach (var fromState in registeredStates)
            {
                var validTargets = registeredStates.Where(toState =>
                StateTransitionRules.IsTransitionAllowed(fromState, toState) && fromState != toState);
                
                if (!validTargets.Any())
                {
                    issues.Add($"State {fromState} has no valid transition targets");
                }
            }
            
            //Check debug event consistency
            lock (_debugLock)
            {
                var stateEnterEvents = _debugEvents.Where(e => e.EventType == StateDebugEventType.StateEntered);
                var stateExitEvents = _debugEvents.Where(e => e.EventType == StateDebugEventType.StateExited);
                
                foreach (var enterEvent in stateEnterEvents)
                {
                    var correspondingExit = stateExitEvents.FirstOrDefault(e =>
                    e.StateType == enterEvent.StateType && e.Timestamp > enterEvent.Timestamp);
                    
                    if (correspondingExit == null && enterEvent.StateType != stateMachine.CurrentStateType)
                    {
                        issues.Add($"State {enterEvent.StateType} was entered but never exited");
                    }
                }
            }
            
            return issues;
        }
    }
    
    ///<summary>
    ///Types of debug events for state machine debugging.
    ///</summary>
    public enum StateDebugEventType
    {
        ///<summary>State was entered.</summary>
        StateEntered,
        
        ///<summary>State was exited.</summary>
        StateExited,
        
        ///<summary>State transition started.</summary>
        TransitionStarted,
        
        ///<summary>State transition completed.</summary>
        TransitionCompleted,
        
        ///<summary>State transition failed.</summary>
        TransitionFailed,
        
        ///<summary>State update called.</summary>
        StateUpdate,
        
        ///<summary>Event handled by state.</summary>
        EventHandled,
        
        ///<summary>Event ignored by state.</summary>
        EventIgnored,
        
        ///<summary>State registered.</summary>
        StateRegistered,
        
        ///<summary>State machine reset.</summary>
        StateMachineReset,
        
        ///<summary>Custom debug event.</summary>
        Custom
    }
    
    ///<summary>
    ///Represents a debug event for state machine debugging.
    ///</summary>
    public class StateDebugEvent
    {
        ///<summary>
        ///Timestamp when the event occurred.
        ///</summary>
        public DateTime Timestamp { get; set; }
        
        ///<summary>
        ///Type of debug event.
        ///</summary>
        public StateDebugEventType EventType { get; set; }
        
        ///<summary>
        ///State type involved in the event.
        ///</summary>
        public GameStateType StateType { get; set; }
        
        ///<summary>
        ///Debug message.
        ///</summary>
        public string Message { get; set; }
        
        ///<summary>
        ///Additional data associated with the event.
        ///</summary>
        public object Data { get; set; }
        
        ///<summary>
        ///Returns a string representation of the debug event.
        ///</summary>
        public override string ToString()
        {
            return $"{Timestamp:HH:mm:ss.fff} | {EventType} | {StateType} | {Message}";
        }
    }
}




