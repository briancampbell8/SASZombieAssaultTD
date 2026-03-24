using System;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.State
{
    /// <summary>
    /// Extension methods for StateMachineBuilder.
    /// </summary>
    public static class StateMachineBuilderExtensions
    {
        /// <summary>
        /// Adds UI system integration to state machine.
        /// </summary>
        public static StateMachineBuilder WithUISystem(this StateMachineBuilder builder)
        {
            // Implementation would add UI system
            return builder;
        }

        /// <summary>
        /// Adds audio system integration to state machine.
        /// </summary>
        public static StateMachineBuilder WithAudioSystem(this StateMachineBuilder builder)
        {
            // Implementation would add audio system
            return builder;
        }

        /// <summary>
        /// Creates a development environment state machine builder.
        /// </summary>
        public static StateMachineBuilder CreateDevelopment()
        {
            return StateMachineBuilder.Create()
                .WithDefaultStates()
                .WithInitialState(GameStateType.Boot)
                .WithProfiling()
                .WithDebugging();
        }

        /// <summary>
        /// Creates a production environment state machine builder.
        /// </summary>
        public static StateMachineBuilder CreateProduction()
        {
            return StateMachineBuilder.Create()
                .WithDefaultStates()
                .WithInitialState(GameStateType.Boot);
        }

        /// <summary>
        /// Creates a default environment state machine builder.
        /// </summary>
        public static StateMachineBuilder CreateDefault()
        {
            return StateMachineBuilder.Create()
                .WithDefaultStates()
                .WithInitialState(GameStateType.Boot);
        }
    }
}
