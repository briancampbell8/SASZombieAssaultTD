/*
File:    AnimationStateMachine.cs
Purpose: P11-17-01 - Create initial state machine class with explicit fields for CurrentState, PreviousState, and deterministic UpdateState method.
Provides deterministic state management for animation system with explicit state tracking.
*/
using SASZombieAssaultTD.Engine.Animation.Core;
using SASZombieAssaultTD.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Animation.Components
{
    /// <summary>
    /// P11-17-01: Deterministic animation state machine with explicit state tracking.
    /// Provides deterministic state management with explicit CurrentState, PreviousState, and UpdateState method.
    /// P11-17-06: Enhanced with deterministic transition registration logic and state dictionary.
    /// </summary>
    public class AnimationStateMachine
    {
        private readonly Dictionary<string, IAnimationState> _states = new();
        private readonly Dictionary<string, HashSet<string>> _allowedTransitions = new();
        private readonly Dictionary<string, object> _globalParameters = new();

        public IAnimationState? CurrentState { get; private set; }
        public IAnimationState? PreviousState { get; private set; }
        public string CurrentStateId => CurrentState?.Name ?? string.Empty;
        public string PreviousStateId => PreviousState?.Name ?? string.Empty;
        public bool HasValidState => CurrentState != null;
        public float TimeInCurrentState { get; private set; }
        public AnimationTransition? LastTransition { get; private set; }

        public void UpdateState(float deltaTime)
        {
            if (CurrentState == null) return;

            TimeInCurrentState += deltaTime;
            CurrentState.Update(deltaTime, TimeInCurrentState);

            var nextState = CurrentState.CheckTransitions();
            if (nextState != null)
            {
                TransitionToState(nextState);
            }
        }

        private void TransitionToState(IAnimationState newState)
        {
            if (newState == null)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", "AnimationStateMachine: Cannot transition to null state");
                return;
            }

            if (CurrentState != null && !IsTransitionAllowed(CurrentState.Name, newState.Name))
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"AnimationStateMachine: Transition '{CurrentState.Name}' -> '{newState.Name}' is not registered");
                return;
            }

            CurrentState?.Exit();
            PreviousState = CurrentState;
            CurrentState = newState;
            TimeInCurrentState = 0f;
            CurrentState.Enter();

            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"AnimationStateMachine: Transitioned to state '{CurrentState.Name}'");
        }

        public void SetInitialState(IAnimationState initialState)
        {
            if (initialState == null || !initialState.IsValid())
            {
                Engine.Diagnostics.DebugLogger.LogDebug("WARNING", "AnimationStateMachine: Invalid initial state, applying fallback");
                SetFallbackState();
                return;
            }

            CurrentState = initialState;
            PreviousState = null;
            TimeInCurrentState = 0f;
            CurrentState.Enter();

            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"AnimationStateMachine: Set initial state '{CurrentState.Name}'");
        }

        private void SetFallbackState()
        {
            var fallbackState = new InlineState("Fallback");
            if (!_states.ContainsKey("Fallback"))
            {
                RegisterState(fallbackState);
            }

            TransitionToState(fallbackState);
            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", "AnimationStateMachine: Transitioned to fallback state");
        }

        public void RegisterState(IAnimationState state)
        {
            if (state == null || string.IsNullOrEmpty(state.Name))
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", "AnimationStateMachine: Cannot register null or unnamed state");
                return;
            }

            _states[state.Name] = state;
            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"AnimationStateMachine: Registered state '{state.Name}'");
        }

        public void RegisterTransition(string fromState, string toState)
        {
            if (string.IsNullOrEmpty(fromState) || string.IsNullOrEmpty(toState) || fromState == toState)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("WARNING", "AnimationStateMachine: Invalid transition registration");
                return;
            }

            if (!_allowedTransitions.ContainsKey(fromState))
            {
                _allowedTransitions[fromState] = new HashSet<string>();
            }

            _allowedTransitions[fromState].Add(toState);
            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"AnimationStateMachine: Registered transition '{fromState}' -> '{toState}'");
        }

        public bool IsTransitionAllowed(string fromState, string toState) =>
            _allowedTransitions.TryGetValue(fromState, out var transitions) && transitions.Contains(toState);

        public string[] GetRegisteredStateNames() => _states.Keys.ToArray();

        public string[] GetAllowedTransitions(string stateName) =>
            _allowedTransitions.TryGetValue(stateName, out var transitions) ? transitions.ToArray() : Array.Empty<string>();

        /// <summary>
        /// Forces a transition to a specific state.
        /// </summary>
        /// <param name="stateName">Name of the state to transition to.</param>
        public void ForceTransitionTo(string stateName)
        {
            if (!_states.TryGetValue(stateName, out var newState))
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"AnimationStateMachine: Cannot transition to unknown state '{stateName}'");
                return;
            }

            var oldState = CurrentState;
            PreviousState = CurrentState;
            CurrentState = newState;
            TimeInCurrentState = 0f;

            oldState?.Exit();
            newState.Enter();

            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"AnimationStateMachine: Forced transition '{oldState?.Name}' -> '{stateName}'");
        }

        /// <summary>
        /// Forces a transition to a specific state (alias for ForceTransitionTo).
        /// </summary>
        /// <param name="stateName">Name of the state to transition to.</param>
        public void ForceTransition(string stateName) => ForceTransitionTo(stateName);

        public void SetParameter(string name, object value)
        {
            if (string.IsNullOrEmpty(name)) return;

            _globalParameters[name] = value;
            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"AnimationStateMachine: Set parameter '{name}' = {value}");
        }

        public T GetGlobalParameter<T>(string name, T defaultValue = default!) =>
            _globalParameters.TryGetValue(name, out var value) && value is T typedValue ? typedValue : defaultValue;

        private sealed class InlineState : IAnimationState
        {
            public string Name { get; }
            public AnimationClip? Clip { get; set; }

            public InlineState(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));
            public void Enter() { }
            public void Exit() { }
            public void Update(float deltaTime, float timeInState) { }
            public IAnimationState? CheckTransitions() => null;
            public Dictionary<string, object> GetParameters() => new();
            public bool IsValid() => true;
            public string GetDebugInfo() => $"InlineState: {Name}";
        }
    }
}

































































































































































































