// ====================================================================================================
//  FILE: AnimationMachine.cs
//  PATH: ./Engine/Animation/Core/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the AnimationStateMachine module.
//
//  RESPONSIBILITIES:
//      - Provide UpdateState() behavior for the Core subsystem.
//      - Provide SetInitialState() behavior for the Core subsystem.
//      - Provide RegisterState() behavior for the Core subsystem.
//      - Provide RegisterTransition() behavior for the Core subsystem.
//      - Provide IsTransitionAllowed() behavior for the Core subsystem.
//      - Provide GetRegisteredStateNames() behavior for the Core subsystem.
//      - Provide GetAllowedTransitions() behavior for the Core subsystem.
//      - Provide ForceTransitionTo() behavior for the Core subsystem.
//      - Provide ForceTransition() behavior for the Core subsystem.
//      - Provide SetParameter() behavior for the Core subsystem.
//      - Provide Enter() behavior for the Core subsystem.
//      - Provide Exit() behavior for the Core subsystem.
//      - Provide Update() behavior for the Core subsystem.
//      - Provide CheckTransitions() behavior for the Core subsystem.
//      - Provide IsValid() behavior for the Core subsystem.
//      - Provide GetDebugInfo() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    AnimationStateMachine.cs
Purpose: P11-17-01 - Create initial state machine class with explicit fields for CurrentState, PreviousState, and deterministic UpdateState method.
Provides deterministic state management for animation system with explicit state tracking.
*/
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Linq;
using SASZombieAssaultTD.Engine.Animation.Core.Clips;
using SASZombieAssaultTD.Engine.Animation.Core.State;
using SASZombieAssaultTD.Engine.Animation.Core.Transitions;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
namespace SASZombieAssaultTD.Engine.Animation.Core
//
{
    ///<summary>
    ///P11-17-01: Deterministic animation state machine with explicit state tracking.
    ///Provides deterministic state management with explicit CurrentState, PreviousState, and UpdateState method.
    ///P11-17-06: Enhanced with deterministic transition registration logic and state dictionary.
    ///</summary>
    public class AnimationMachine
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
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Error, "AnimationStateMachine: Cannot transition to null state");
                return;
            }

            if (CurrentState != null && !IsTransitionAllowed(CurrentState.Name, newState.Name))
            {
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Error, $"AnimationStateMachine: Transition '{CurrentState.Name}' -> '{newState.Name}' is not registered");
                return;
            }

            CurrentState?.Exit();
            PreviousState = CurrentState;
            CurrentState = newState;
            TimeInCurrentState = 0f;
            CurrentState.Enter();

            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, $"AnimationStateMachine: Transitioned to state '{CurrentState.Name}'");
        }

        public void SetInitialState(IAnimationState initialState)
        {
            if (initialState == null || !initialState.IsValid())
            {
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Warning, "AnimationStateMachine: Invalid initial state, applying fallback");
                SetFallbackState();
                return;
            }

            CurrentState = initialState;
            PreviousState = null;
            TimeInCurrentState = 0f;
            CurrentState.Enter();

            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, $"AnimationStateMachine: Set initial state '{CurrentState.Name}'");
        }

        private void SetFallbackState()
        {
            var fallbackState = new InlineState("Fallback");
            if (!_states.ContainsKey("Fallback"))
            {
                RegisterState(fallbackState);
            }

            TransitionToState(fallbackState);
            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, "AnimationStateMachine: Transitioned to fallback state");
        }

        public void RegisterState(IAnimationState state)
        {
            if (state == null || string.IsNullOrEmpty(state.Name))
            {
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Error, "AnimationStateMachine: Cannot register null or unnamed state");
                return;
            }

            _states[state.Name] = state;
            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, $"AnimationStateMachine: Registered state '{state.Name}'");
        }

        public void RegisterTransition(string fromState, string toState)
        {
            if (string.IsNullOrEmpty(fromState) || string.IsNullOrEmpty(toState) || fromState == toState)
            {
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Warning, "AnimationStateMachine: Invalid transition registration");
                return;
            }

            if (!_allowedTransitions.ContainsKey(fromState))
            {
                _allowedTransitions[fromState] = new HashSet<string>();
            }

            _allowedTransitions[fromState].Add(toState);
            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, $"AnimationStateMachine: Registered transition '{fromState}' -> '{toState}'");
        }

        public bool IsTransitionAllowed(string fromState, string toState) =>
            _allowedTransitions.TryGetValue(fromState, out var transitions) && transitions.Contains(toState);

        public string[] GetRegisteredStateNames() => _states.Keys.ToArray();

        public string[] GetAllowedTransitions(string stateName) =>
            _allowedTransitions.TryGetValue(stateName, out var transitions) ? transitions.ToArray() : Array.Empty<string>();

        ///<summary>
        ///Forces a transition to a specific state.
        ///</summary>
        ///<param name="stateName">Name of the state to transition to.</param>
        public void ForceTransitionTo(string stateName)
        {
            if (!_states.TryGetValue(stateName, out var newState))
            {
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Error, $"AnimationStateMachine: Cannot transition to unknown state '{stateName}'");
                return;
            }

            var oldState = CurrentState;
            PreviousState = CurrentState;
            CurrentState = newState;
            TimeInCurrentState = 0f;

            oldState?.Exit();
            newState.Enter();

            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, $"AnimationStateMachine: Forced transition '{oldState?.Name}' -> '{stateName}'");
        }

        ///<summary>
        ///Forces a transition to a specific state (alias for ForceTransitionTo).
        ///</summary>
        ///<param name="stateName">Name of the state to transition to.</param>
        public void ForceTransition(string stateName) => ForceTransitionTo(stateName);

        public void SetParameter(string name, object value)
        {
            if (string.IsNullOrEmpty(name)) return;

            _globalParameters[name] = value;
            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, $"AnimationStateMachine: Set parameter '{name}' = {value}");
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
