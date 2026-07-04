using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Core;
//

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.State
{
    ///<summary>
    ///State machine controller for managing game states.
    ///P20-02-03: Handles state registration, transitions, and event forwarding.
    ///</summary>
    public class StateMachine
    {
        private IGameState _currentState;
        private readonly Dictionary<GameStateType, IGameState> _states;
        private GameStateType _currentStateType;
        
        ///<summary>
        ///Gets the current active state type.
        ///</summary>
        public GameStateType CurrentStateType => _currentStateType;
        
        ///<summary>
        ///Gets the current active state instance.
        ///</summary>
        public IGameState CurrentState => _currentState;
        
        ///<summary>
        ///Initializes a new state machine instance.
        ///</summary>
        public StateMachine()
        {
            _states = new Dictionary<GameStateType, IGameState>();
            _currentState = null;
            _currentStateType = GameStateType.Boot; //Default to boot state
        }
        
        ///<summary>
        ///Registers a state instance with the state machine.
        ///</summary>
        ///<param name="type">The state type identifier.</param>
        ///<param name="state">The state instance to register.</param>
        ///<exception cref="ArgumentNullException">Thrown when state is null.</exception>
        ///<exception cref="ArgumentException">Thrown when state type is already registered.</exception>
        public virtual void RegisterState(GameStateType type, IGameState state)
        {
            if (state == null)
            throw new ArgumentNullException(nameof(state));
            
            if (_states.ContainsKey(type))
            {
                DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "WARNING", $"StateMachine: State type {type} is already registered. Overwriting.");
                _states[type] = state;
            }
            else
            {
                _states.Add(type, state);
                DLogger.Log(LogSubsystems.State,LogLevel.Info, $"StateMachine: Registered state {type}");
            }
        }
        
        ///<summary>
        ///Changes to a new state, exiting the current state and entering the new one.
        ///</summary>
        ///<param name="type">The state type to transition to.</param>
        ///<exception cref="ArgumentException">Thrown when state type is not registered.</exception>
        public virtual void ChangeState(GameStateType type, GameEvent? triggerEvent = null)
        {
            if (!_states.TryGetValue(type, out var newState))
            {
DLogger.Log(LogSubsystems.State,LogLevel.Info,"ERROR",$"StateMachine: Cannot change to unregistered state {type}");
                throw new ArgumentException($"State type {type} is not registered", nameof(type));
            }
            
            //Exit current state if it exists
            if (_currentState != null)
            {
                DLogger.Log(LogSubsystems.State,LogLevel.Info, $"StateMachine: Exited state {_currentStateType}");
                _currentState.Exit();
            }
            
            //Enter new state
            _currentState = newState;
            _currentStateType = type;
            DLogger.Log(LogSubsystems.State,LogLevel.Info, $"StateMachine: Entered state {type}");
            DLogger.Log(LogSubsystems.State,LogLevel.Info, $"StateMachine: Entering state {type}");
            _currentState.Enter();
        }
        
        ///<summary>
        ///Updates the current state with delta time.
        ///</summary>
        ///<param name="deltaTime">Time elapsed since last update in seconds.</param>
        public virtual void Update(float deltaTime)
        {
            if (_currentState != null)
            {
                _currentState.Update(deltaTime);
            }
            else
            {
                DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "WARNING", "StateMachine: No current state to update");
            }
        }
        
        ///<summary>
        ///Forwards an event to the current state for handling.
        ///</summary>
        ///<param name="gameEvent">The game event to handle.</param>
        ///<exception cref="ArgumentNullException">Thrown when gameEvent is null.</exception>
        public virtual void HandleEvent(GameEvent gameEvent)
        {
            if (gameEvent == null)
            throw new ArgumentNullException(nameof(gameEvent));
            
            if (_currentState != null)
            {
                _currentState.HandleEvent(gameEvent);
            }
            else
            {
                DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "WARNING", $"StateMachine: No current state to handle event {gameEvent.GetType().Name}");
            }
        }
        
        ///<summary>
        ///Checks if a state type is registered with the state machine.
        ///</summary>
        ///<param name="type">The state type to check.</param>
        ///<returns>True if the state type is registered, false otherwise.</returns>
        public bool HasState(GameStateType type)
        {
            return _states.ContainsKey(type);
        }
        
        ///<summary>
        ///Gets a registered state instance without changing the current state.
        ///</summary>
        ///<param name="type">The state type to retrieve.</param>
        ///<returns>The state instance if registered, null otherwise.</returns>
        public IGameState GetState(GameStateType type)
        {
            _states.TryGetValue(type, out var state);
            return state;
        }
        
        ///<summary>
        ///Gets all registered state types.
        ///</summary>
        ///<returns>Array of registered state types.</returns>
        public GameStateType[] GetRegisteredStates()
        {
            var stateTypes = new GameStateType[_states.Count];
            _states.Keys.CopyTo(stateTypes, 0);
            return stateTypes;
        }
        
        ///<summary>
        ///Gets statistics about the state machine.
        ///</summary>
        ///<returns>State machine statistics.</returns>
        public StateMachineStatistics GetStatistics()
        {
            return new StateMachineStatistics
            {
                CurrentState = _currentStateType,
                RegisteredStates = GetRegisteredStates().Length,
                ValidTransitions = GetRegisteredStates().Length,
                HasCurrentState = _currentState != null
            };
        }
        
        ///<summary>
        ///Resets the state machine by exiting the current state and clearing all registrations.
        ///</summary>
        public virtual void Reset()
        {
            if (_currentState != null)
            {
                DLogger.Log(LogSubsystems.State,LogLevel.Info, $"StateMachine: Exiting current state {_currentStateType} during reset");
                _currentState.Exit();
                _currentState = null;
            }
            
            _states.Clear();
            _currentStateType = GameStateType.Boot;
            DLogger.Log(LogSubsystems.State,LogLevel.Info, "StateMachine: Reset complete - all states cleared");
        }
    }
}




