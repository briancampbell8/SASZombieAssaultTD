using System;

namespace SASZombieAssaultTD.Engine.State
{
    /// <summary>
    /// Represents a state transition with metadata.
    /// P20-02-Enhancement: Enhanced state transition tracking and validation.
    /// </summary>
    public class StateTransition
    {
        /// <summary>
        /// The state being transitioned from.
        /// </summary>
        public GameStateType FromState { get; }
        
        /// <summary>
        /// The state being transitioned to.
        /// </summary>
        public GameStateType ToState { get; }
        
        /// <summary>
        /// The timestamp when the transition occurred.
        /// </summary>
        public DateTime Timestamp { get; }
        
        /// <summary>
        /// The event that triggered the transition (if any).
        /// </summary>
        public GameEvent TriggerEvent { get; }
        
        /// <summary>
        /// The duration of the transition in milliseconds.
        /// </summary>
        public long DurationMs { get; private set; }
        
        private readonly DateTime _startTime;
        
        /// <summary>
        /// Initializes a new state transition.
        /// </summary>
        /// <param name="fromState">The state being transitioned from.</param>
        /// <param name="toState">The state being transitioned to.</param>
        /// <param name="triggerEvent">The event that triggered the transition.</param>
        public StateTransition(GameStateType fromState, GameStateType toState, GameEvent? triggerEvent = null)
        {
            FromState = fromState;
            ToState = toState;
            TriggerEvent = triggerEvent;
            Timestamp = DateTime.UtcNow;
            _startTime = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Marks the transition as complete and records the duration.
        /// </summary>
        public void Complete()
        {
            DurationMs = (long)(DateTime.UtcNow - _startTime).TotalMilliseconds;
        }
        
        /// <summary>
        /// Returns a string representation of the transition.
        /// </summary>
        public override string ToString()
        {
            return $"StateTransition: {FromState} → {ToState} ({DurationMs}ms, Trigger: {TriggerEvent?.GetType().Name ?? "None"})";
        }
    }
    
    /// <summary>
    /// Validation rules for state transitions.
    /// </summary>
    public static class StateTransitionRules
    {
        /// <summary>
        /// Validates if a transition is allowed.
        /// </summary>
        /// <param name="fromState">The current state.</param>
        /// <param name="toState">The target state.</param>
        /// <returns>True if the transition is allowed, false otherwise.</returns>
        public static bool IsTransitionAllowed(GameStateType fromState, GameStateType toState)
        {
            // Allow all transitions for now, but this can be extended with specific rules
            // Examples of rules that could be added:
            // - Cannot transition from Paused to Boot
            // - Cannot transition to the same state
            // - Specific sequences required for certain states
            
            if (fromState == toState)
            return false; // Prevent self-transitions
            
            return true;
        }
        
        /// <summary>
        /// Gets a validation message for a disallowed transition.
        /// </summary>
        /// <param name="fromState">The current state.</param>
        /// <param name="toState">The target state.</param>
        /// <returns>A validation message explaining why the transition is disallowed.</returns>
        public static string GetValidationMessage(GameStateType fromState, GameStateType toState)
        {
            if (fromState == toState)
            return $"Cannot transition from {fromState} to the same state";
            
            return $"Transition from {fromState} to {toState} is not allowed";
        }
    }
}




