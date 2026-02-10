@"
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine.Systems
{
    public enum GameStateType
    {
        None = 0,
        Boot,
        MainMenu,
        InGame,
        Paused
    }

    public interface IGameState
    {
        GameStateType StateType { get; }
        void Enter();
        void Exit();
        void Update(float deltaTime);
    }

    public class GameStateManager
    {
        public GameStateType CurrentStateType => _currentState?.StateType ?? GameStateType.None;

        private readonly Dictionary<GameStateType, IGameState> _states;
        private IGameState _currentState;

        public GameStateManager()
        {
            _states = new Dictionary<GameStateType, IGameState>();

#if DEBUG
            Debug.WriteLine("[GameStateManager] Constructed.");
#endif
        }

        public void RegisterState(IGameState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            _states[state.StateType] = state;

#if DEBUG
            Debug.WriteLine($"[GameStateManager] Registered state: {state.StateType}");
#endif
        }

        public void ChangeState(GameStateType newStateType)
        {
            if (_currentState != null && _currentState.StateType == newStateType)
                return;

            if (_currentState != null)
            {
#if DEBUG
                Debug.WriteLine($"[GameStateManager] Exiting state: {_currentState.StateType}");
#endif
                _currentState.Exit();
            }

            if (!_states.TryGetValue(newStateType, out var nextState))
            {
#if DEBUG
                Debug.WriteLine($"[GameStateManager] Requested state not registered: {newStateType}");
#endif
                _currentState = null;
                return;
            }

            _currentState = nextState;

#if DEBUG
            Debug.WriteLine($"[GameStateManager] Entering state: {_currentState.StateType}");
#endif
            _currentState.Enter();
        }

        public void Update(float deltaTime)
        {
            _currentState?.Update(deltaTime);
        }

#if DEBUG
        public void DebugPrint()
        {
            var stateName = _currentState?.StateType.ToString() ?? "None";
            Debug.WriteLine($"[GameStateManager] CurrentState={stateName}");
        }
#endif
    }
}
"@
