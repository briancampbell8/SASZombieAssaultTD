using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Core;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.State
{
    ///<summary>
    ///Builder for creating state machines with fluent API.
    ///</summary>
    public class StateMachineBuilder
    {
        private readonly List<GameStateType> _states = new List<GameStateType>();
        private GameStateType? _initialState;
        private bool _profilingEnabled;
        private bool _debuggingEnabled;

        public static StateMachineBuilder Create()
        {
            return new StateMachineBuilder();
        }

        public StateMachineBuilder WithDefaultStates()
        {
            //Implementation would add default states
            return this;
        }

        public StateMachineBuilder WithInitialState(GameStateType state)
        {
            _initialState = state;
            return this;
        }

        public StateMachineBuilder WithProfiling()
        {
            _profilingEnabled = true;
            return this;
        }

        public StateMachineBuilder WithDebugging()
        {
            _debuggingEnabled = true;
            return this;
        }

        public EnhancedStateMachine Build()
        {
            //Implementation would build the state machine
            return new EnhancedStateMachine();
        }

        public EnhancedStateMachine BuildEnhanced()
        {
            //Implementation would build the enhanced state machine
            return new EnhancedStateMachine();
        }

        public StateMachineBuilder WithMaxHistorySize(int size)
        {
            //Implementation would set max history size
            return this;
        }
    }
}
