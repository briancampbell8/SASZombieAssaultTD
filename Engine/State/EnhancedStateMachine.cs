using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.State
{
    /// <summary>
    /// Enhanced state machine with profiling, debugging, and advanced features.
    /// P20-02-Enhancement: Production-ready state machine with comprehensive monitoring.
    /// </summary>
    public class EnhancedStateMachine : AdvancedStateMachine
    {
        private readonly StateMachineProfiler _profiler;
        private readonly StateFactory _stateFactory;
        private readonly Dictionary<GameStateType, DateTime> _lastStateEnterTimes;
        
        /// <summary>
        /// Gets the profiler for performance monitoring.
        /// </summary>
        public StateMachineProfiler Profiler => _profiler;
        
        /// <summary>
        /// Gets the state factory for state creation.
        /// </summary>
        public StateFactory StateFactory => _stateFactory;
        
        /// <summary>
        /// Event fired when a state is entered.
        /// </summary>
        public event Action<GameStateType> OnStateEntered;
        
        /// <summary>
        /// Event fired when a state is exited.
        /// </summary>
        public event Action<GameStateType> OnStateExited;
        
        /// <summary>
        /// Initializes a new enhanced state machine.
        /// </summary>
        /// <param name="maxHistorySize">Maximum number of transitions to keep in history.</param>
        public EnhancedStateMachine(int maxHistorySize = 100) : base(maxHistorySize)
        {
            _profiler = new StateMachineProfiler();
            _stateFactory = new StateFactory();
            _lastStateEnterTimes = new Dictionary<GameStateType, DateTime>();
            
            // Set up event handlers
            OnTransitionStarted += HandleTransitionStarted;
            OnTransitionCompleted += HandleTransitionCompleted;
            OnTransitionFailed += HandleTransitionFailed;
            
            ModernLoggingSystem.Log("INFO", "EnhancedStateMachine: Initialized with profiler and factory");
        }
        
        /// <summary>
        /// Registers a state with profiling and debugging.
        /// </summary>
        /// <param name="type">The state type.</param>
        /// <param name="state">The state instance.</param>
        public override void RegisterState(GameStateType type, IGameState state)
        {
            using var session = _profiler.StartProfiling(type, "RegisterState");
            
            base.RegisterState(type, state);
            
            StateDebugger.LogStateEvent(StateDebugEventType.StateRegistered, type, $"State registered with instance type {state.GetType().Name}");
        }
        
        /// <summary>
        /// Changes state with enhanced monitoring and validation.
        /// </summary>
        /// <param name="type">The target state type.</param>
        /// <param name="triggerEvent">The event that triggered the transition.</param>
        public override void ChangeState(GameStateType type, GameEvent? triggerEvent = null)
        {
            using var session = _profiler.StartProfiling(type, "ChangeState");
            
            var previousState = CurrentStateType;
            
            try
            {
                base.ChangeState(type, triggerEvent);
                
                // Record state enter time
                _lastStateEnterTimes[type] = DateTime.UtcNow;
                
                // Fire state-specific events
                OnStateEntered?.Invoke(type);
                
                StateDebugger.LogStateEvent(StateDebugEventType.StateEntered, type,
                $"State entered from {previousState}", triggerEvent);
            }
            catch (Exception ex)
            {
                StateDebugger.LogStateEvent(StateDebugEventType.TransitionFailed, type,
                $"State transition failed: {ex.Message}", ex);
                throw;
            }
        }
        
        /// <summary>
        /// Updates the current state with profiling.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last update.</param>
        public override void Update(float deltaTime)
        {
            if (CurrentState == null)
            return;
            
            using var session = _profiler.StartProfiling(CurrentStateType, "Update");
            
            try
            {
                base.Update(deltaTime);
                
                StateDebugger.LogStateEvent(StateDebugEventType.StateUpdate, CurrentStateType,
                $"State updated with deltaTime: {deltaTime:F4}s");
            }
            catch (Exception ex)
            {
                StateDebugger.LogStateEvent(StateDebugEventType.Custom, CurrentStateType,
                $"State update failed: {ex.Message}", ex);
                throw;
            }
        }
        
        /// <summary>
        /// Handles an event with profiling and debugging.
        /// </summary>
        /// <param name="gameEvent">The game event to handle.</param>
        public override void HandleEvent(GameEvent gameEvent)
        {
            if (CurrentState == null || gameEvent == null)
            return;
            
            using var session = _profiler.StartProfiling(CurrentStateType, "HandleEvent");
            
            try
            {
                base.HandleEvent(gameEvent);
                
                StateDebugger.LogStateEvent(StateDebugEventType.EventHandled, CurrentStateType,
                $"Event handled: {gameEvent.GetType().Name}", gameEvent);
            }
            catch (Exception ex)
            {
                StateDebugger.LogStateEvent(StateDebugEventType.Custom, CurrentStateType,
                $"Event handling failed: {ex.Message}", ex);
                throw;
            }
        }
        
        /// <summary>
        /// Resets the state machine with enhanced cleanup.
        /// </summary>
        public override void Reset()
        {
            using var session = _profiler.StartProfiling(CurrentStateType, "Reset");
            
            var previousState = CurrentStateType;
            
            base.Reset();
            
            _lastStateEnterTimes.Clear();
            _profiler.ClearMetrics();
            
            StateDebugger.LogStateEvent(StateDebugEventType.StateMachineReset, previousState,
            "State machine reset");
        }
        
        /// <summary>
        /// Configures the state machine using the internal state factory.
        /// </summary>
        public new void ConfigureWithFactory()
        {
            using var session = _profiler.StartProfiling(GameStateType.Boot, "ConfigureWithFactory");
            
            base.ConfigureWithFactory();
        }
        
        /// <summary>
        /// Gets comprehensive statistics including performance data.
        /// </summary>
        /// <returns>Enhanced state machine statistics.</returns>
        public new EnhancedStateMachineStatistics GetStatistics()
        {
            var baseStats = base.GetAdvancedStatistics();
            var stateMetrics = _profiler.GetAllStateMetrics();
            var operationMetrics = _profiler.GetAllOperationMetrics();
            var performanceIssues = _profiler.IdentifyPerformanceIssues();
            
            return new EnhancedStateMachineStatistics
            {
                CurrentState = baseStats.CurrentState,
                RegisteredStates = baseStats.RegisteredStates,
                ValidTransitions = baseStats.ValidTransitions,
                HasCurrentState = baseStats.HasCurrentState,
                CurrentStateDuration = baseStats.CurrentStateDuration,
                TotalTransitions = baseStats.TotalTransitions,
                AverageStateDuration = baseStats.AverageStateDuration,
                MostFrequentTransition = baseStats.MostFrequentTransition,
                StateEnterTimes = baseStats.StateEnterTimes,
                StateMetrics = stateMetrics,
                OperationMetrics = operationMetrics,
                PerformanceIssues = performanceIssues,
                ProfilingEnabled = _profiler.ProfilingEnabled,
                DebugEnabled = StateDebugger.DebugEnabled
            };
        }
        
        /// <summary>
        /// Generates a comprehensive report including performance and debugging data.
        /// </summary>
        /// <returns>Comprehensive state machine report.</returns>
        public string GenerateComprehensiveReport()
        {
            var report = new List<string>
            {
                "Enhanced State Machine Comprehensive Report",
                $"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}",
                ""
            };
            
            // Basic statistics
            var stats = GetStatistics();
            report.Add("=== Basic Statistics ===");
            report.Add($"Current State: {stats.CurrentState}");
            report.Add($"Current State Duration: {stats.CurrentStateDuration.TotalSeconds:F2}s");
            report.Add($"Total Transitions: {stats.TotalTransitions}");
            report.Add($"Registered States: {stats.RegisteredStates}");
            report.Add("");
            
            // Performance summary
            report.Add("=== Performance Summary ===");
            report.Add(_profiler.GeneratePerformanceReport());
            report.Add("");
            
            // Debug summary
            report.Add("=== Debug Summary ===");
            report.Add(StateDebugger.GeneratePerformanceReport(this));
            report.Add("");
            
            // Performance issues
            if (stats.PerformanceIssues.Count > 0)
            {
                report.Add("=== Performance Issues ===");
                foreach (var issue in stats.PerformanceIssues)
                {
                    report.Add($"⚠ {issue}");
                }
                report.Add("");
            }
            
            // Validation results
            var validationIssues = StateDebugger.ValidateStateMachine(this);
            if (validationIssues.Count > 0)
            {
                report.Add("=== Validation Issues ===");
                foreach (var issue in validationIssues)
                {
                    report.Add($"❌ {issue}");
                }
            }
            else
            {
                report.Add("=== Validation ===");
                report.Add("✅ No validation issues found");
            }
            
            return string.Join(Environment.NewLine, report);
        }
        
        /// <summary>
        /// Enables or disables profiling for a specific state.
        /// </summary>
        /// <param name="stateType">The state type.</param>
        /// <param name="enabled">Whether to enable profiling.</param>
        public void SetStateProfiling(GameStateType stateType, bool enabled)
        {
            // This would require extending the profiler to support per-state profiling
            // For now, we'll just log the request
            StateDebugger.LogStateEvent(StateDebugEventType.Custom, stateType,
            $"Profiling {(enabled ? "enabled" : "disabled")}");
        }
        
        /// <summary>
        /// Handles transition started events.
        /// </summary>
        /// <param name="transition">The transition that started.</param>
        private void HandleTransitionStarted(StateTransition transition)
        {
            StateDebugger.LogStateEvent(StateDebugEventType.TransitionStarted, transition.ToState,
            $"Transition started from {transition.FromState}", transition);
        }
        
        /// <summary>
        /// Handles transition completed events.
        /// </summary>
        /// <param name="transition">The transition that completed.</param>
        private void HandleTransitionCompleted(StateTransition transition)
        {
            // Fire state exit event for previous state
            OnStateExited?.Invoke(transition.FromState);
            
            StateDebugger.LogStateEvent(StateDebugEventType.TransitionCompleted, transition.ToState,
            $"Transition completed from {transition.FromState} in {transition.DurationMs}ms", transition);
        }
        
        /// <summary>
        /// Handles transition failed events.
        /// </summary>
        /// <param name="transition">The transition that failed.</param>
        /// <param name="exception">The exception that caused the failure.</param>
        private void HandleTransitionFailed(StateTransition transition, Exception exception)
        {
            StateDebugger.LogStateEvent(StateDebugEventType.TransitionFailed, transition.ToState,
            $"Transition failed from {transition.FromState}: {exception.Message}", exception);
        }
    }
    
    /// <summary>
    /// Enhanced statistics for the EnhancedStateMachine.
    /// </summary>
    public class EnhancedStateMachineStatistics : AdvancedStateMachineStatistics
    {
        /// <summary>
        /// State-specific performance metrics.
        /// </summary>
        public Dictionary<GameStateType, StatePerformanceMetrics> StateMetrics { get; set; }
        
        /// <summary>
        /// Operation-specific performance metrics.
        /// </summary>
        public Dictionary<string, OperationMetrics> OperationMetrics { get; set; }
        
        /// <summary>
        /// List of identified performance issues.
        /// </summary>
        public List<string> PerformanceIssues { get; set; }
        
        /// <summary>
        /// Whether profiling is enabled.
        /// </summary>
        public bool ProfilingEnabled { get; set; }
        
        /// <summary>
        /// Whether debugging is enabled.
        /// </summary>
        public bool DebugEnabled { get; set; }
        
        /// <summary>
        /// Returns a string representation of the enhanced statistics.
        /// </summary>
        public override string ToString()
        {
            return $"Enhanced StateMachine Stats: Current={CurrentState}, Transitions={TotalTransitions}, " +
            $"PerformanceIssues={PerformanceIssues?.Count ?? 0}, Profiling={ProfilingEnabled}, Debug={DebugEnabled}";
        }
    }
}




