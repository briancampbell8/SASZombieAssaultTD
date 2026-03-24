using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.State
{
    /// <summary>
    /// Extension methods for enhanced StateMachine functionality.
    /// P20-02-Enhancement: Additional StateMachine utilities and helpers.
    /// </summary>
    public static class StateMachineExtensions
    {
        /// <summary>
        /// Attempts to change state with validation and returns success status.
        /// </summary>
        /// <param name="stateMachine">The state machine.</param>
        /// <param name="targetState">The target state.</param>
        /// <param name="triggerEvent">The event triggering the transition.</param>
        /// <returns>True if the transition succeeded, false otherwise.</returns>
        public static bool TryChangeState(this StateMachine stateMachine, GameStateType targetState, GameEvent? triggerEvent = null)
        {
            try
            {
                if (!StateTransitionRules.IsTransitionAllowed(stateMachine.CurrentStateType, targetState))
                {
                    ModernLoggingSystem.Log("WARNING", $"StateMachine: Transition disallowed - {StateTransitionRules.GetValidationMessage(stateMachine.CurrentStateType, targetState)}");
                    return false;
                }
                
                stateMachine.ChangeState(targetState);
                return true;
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"StateMachine: Transition failed - {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Gets the state transition history (if implemented).
        /// </summary>
        /// <param name="stateMachine">The state machine.</param>
        /// <returns>List of recent state transitions.</returns>
        public static List<StateTransition> GetTransitionHistory(this StateMachine stateMachine)
        {
            // This would require extending StateMachine to track history
            // For now, return empty list as placeholder
            return new List<StateTransition>();
        }
        
        /// <summary>
        /// Checks if the state machine can transition to the specified state.
        /// </summary>
        /// <param name="stateMachine">The state machine.</param>
        /// <param name="targetState">The target state.</param>
        /// <returns>True if the transition is allowed, false otherwise.</returns>
        public static bool CanTransitionTo(this StateMachine stateMachine, GameStateType targetState)
        {
            return StateTransitionRules.IsTransitionAllowed(stateMachine.CurrentStateType, targetState) &&
            stateMachine.HasState(targetState);
        }
        
        /// <summary>
        /// Gets all valid transition targets from the current state.
        /// </summary>
        /// <param name="stateMachine">The state machine.</param>
        /// <returns>Array of valid target states.</returns>
        public static GameStateType[] GetValidTransitions(this StateMachine stateMachine)
        {
            var allStates = stateMachine.GetRegisteredStates();
            var validTransitions = allStates.Where(state =>
            StateTransitionRules.IsTransitionAllowed(stateMachine.CurrentStateType, state) &&
            state != stateMachine.CurrentStateType
            ).ToArray();
            
            return validTransitions;
        }
        
        /// <summary>
        /// Creates a state transition event for the specified action.
        /// </summary>
        /// <param name="stateMachine">The state machine.</param>
        /// <param name="action">The menu action.</param>
        /// <returns>A menu input event.</returns>
        public static GameEvent CreateMenuEvent(this StateMachine stateMachine, MenuAction action)
        {
            return new MenuInputEvent(action);
        }
        
        /// <summary>
        /// Creates a gameplay input event for the specified action.
        /// </summary>
        /// <param name="stateMachine">The state machine.</param>
        /// <param name="action">The gameplay action.</param>
        /// <returns>A gameplay input event.</returns>
        public static GameEvent CreateGameplayEvent(this StateMachine stateMachine, GameplayAction action)
        {
            return new GameplayInputEvent(action);
        }
        
        /// <summary>
        /// Creates a pause input event for the specified action.
        /// </summary>
        /// <param name="stateMachine">The state machine.</param>
        /// <param name="action">The pause action.</param>
        /// <returns>A pause input event.</returns>
        public static GameEvent CreatePauseEvent(this StateMachine stateMachine, PauseAction action)
        {
            return new PauseInputEvent(action);
        }
        
        /// <summary>
        /// Gets state machine statistics for monitoring.
        /// </summary>
        /// <param name="stateMachine">The state machine.</param>
        /// <returns>State machine statistics.</returns>
        public static StateMachineStatistics GetStatistics(this StateMachine stateMachine)
        {
            return new StateMachineStatistics
            {
                CurrentState = stateMachine.CurrentStateType,
                RegisteredStates = stateMachine.GetRegisteredStates().Length,
                ValidTransitions = stateMachine.GetValidTransitions().Length,
                HasCurrentState = stateMachine.CurrentState != null
            };
        }
    }
    
    /// <summary>
    /// Statistics about the StateMachine state.
    /// </summary>
    public class StateMachineStatistics
    {
        /// <summary>
        /// The current active state.
        /// </summary>
        public GameStateType CurrentState { get; set; }
        
        /// <summary>
        /// Number of registered states.
        /// </summary>
        public int RegisteredStates { get; set; }
        
        /// <summary>
        /// Number of valid transitions from current state.
        /// </summary>
        public int ValidTransitions { get; set; }
        
        /// <summary>
        /// Whether the state machine has a current state.
        /// </summary>
        public bool HasCurrentState { get; set; }
        
        /// <summary>
        /// Returns a string representation of the statistics.
        /// </summary>
        public override string ToString()
        {
            return $"StateMachine Stats: Current={CurrentState}, Registered={RegisteredStates}, ValidTransitions={ValidTransitions}, HasState={HasCurrentState}";
        }
    }
}




