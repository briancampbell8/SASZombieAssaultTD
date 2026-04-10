// ROLE: Animation state machine component for ECS.
// RESPONSIBILITY: Manage animation states, transitions, and state-specific behavior for entities.
// TRIGGERS: Added to entities by EntityManager during entity initialization.
// INPUTS: Receives animation commands and state change requests.
// OUTPUTS: Controls animation playback through state machine.
// DEPENDENCIES: Extends BaseComponent, uses AnimationStateMachine and IAnimationState.
// CONTENTS: AnimationStateMachineComponent class with Entity, StateMachine properties and 
//           PlayAnimation, StopAnimation, Update methods.

using SASZombieAssaultTD.Engine.Animation.Events;
using SASZombieAssaultTD.Engine.Animation.Core;
using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.ECS;
using IEntity = SASZombieAssaultTD.Engine.ECS.Entity;

namespace SASZombieAssaultTD.Engine.Animation.Components
{
    /// <summary>
    /// P11-16-03: Animation state machine component for ECS entities.
    /// Manages animation states, transitions, and state-specific behavior.
    /// </summary>
    public sealed class AnimationStateMachineComponent : BaseComponent
    {
        /// <summary>
        /// The entity this state machine is attached to.
        /// </summary>
        public ECS.Entity Entity
        {
            get => _entity;
            set
            {
                _entity = value;
            }
        }

        /// <summary>
        /// Current active animation state.
        /// </summary>
        public IAnimationState CurrentState => _currentState;

        /// <summary>
        /// Whether the state machine is currently running.
        /// </summary>
        public bool IsRunning => _isRunning;

        private ECS.Entity _entity;
        private IAnimationState _currentState;
        private Dictionary<string, IAnimationState> _states;
        private bool _isRunning;

        /// <summary>
        /// Initializes the animation state machine.
        /// </summary>
        public AnimationStateMachineComponent()
        {
            _states = new Dictionary<string, IAnimationState>();
            _isRunning = false;
        }

        /// <summary>
        /// Adds a state to the state machine.
        /// </summary>
        /// <param name="state">The state to add.</param>
        public void AddState(IAnimationState state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            _states[state.Name] = state;
        }

        /// <summary>
        /// Removes a state from the state machine.
        /// </summary>
        /// <param name="stateName">The name of the state to remove.</param>
        /// <returns>True if the state was removed, false if it didn't exist.</returns>
        public bool RemoveState(string stateName)
        {
            if (string.IsNullOrEmpty(stateName))
                return false;

            return _states.Remove(stateName);
        }

        /// <summary>
        /// Gets a state by name.
        /// </summary>
        /// <param name="stateName">The name of the state to get.</param>
        /// <returns>The state if found, null otherwise.</returns>
        public IAnimationState GetState(string stateName)
        {
            if (string.IsNullOrEmpty(stateName))
                return null;

            _states.TryGetValue(stateName, out var state);
            return state;
        }

        /// <summary>
        /// Transitions to a new state.
        /// </summary>
        /// <param name="stateName">The name of the state to transition to.</param>
        /// <returns>True if the transition was successful, false otherwise.</returns>
        public bool TransitionTo(string stateName)
        {
            if (string.IsNullOrEmpty(stateName))
                return false;

            if (!_states.TryGetValue(stateName, out var newState))
                return false;

            // Exit current state
            if (_currentState != null)
            {
                _currentState.Exit();
            }

            // Enter new state
            _currentState = newState;
            _currentState.Enter();
            _isRunning = true;
            _timeInState = 0f;

            return true;
        }

        private float _timeInState;

        /// <summary>
        /// Updates the current state.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update.</param>
        public void Update(float deltaTime)
        {
            if (!_isRunning || _currentState == null)
                return;

            _timeInState += deltaTime;
            _currentState.Update(deltaTime, _timeInState);
        }

        /// <summary>
        /// Stops the state machine.
        /// </summary>
        public void Stop()
        {
            if (_currentState != null)
            {
                _currentState.Exit();
            }

            _isRunning = false;
            _timeInState = 0f;
        }

        /// <summary>
        /// Starts the state machine with the specified initial state.
        /// </summary>
        /// <param name="initialStateName">The name of the initial state.</param>
        /// <returns>True if the state machine was started successfully, false otherwise.</returns>
        public bool Start(string initialStateName)
        {
            if (string.IsNullOrEmpty(initialStateName))
                return false;

            if (!_states.TryGetValue(initialStateName, out var initialState))
                return false;

            _currentState = initialState;
            _currentState.Enter();
            _isRunning = true;
            _timeInState = 0f;

            return true;
        }

        /// <summary>
        /// Gets all states in the state machine.
        /// </summary>
        /// <returns>ReadOnly collection of all states.</returns>
        public IReadOnlyCollection<IAnimationState> GetAllStates()
        {
            return _states.Values;
        }

        /// <summary>
        /// Clears all states from the state machine.
        /// </summary>
        public void ClearStates()
        {
            Stop();
            _states.Clear();
            _currentState = null;
        }
    }
}
