using System;

namespace SASZombieAssaultTD.Engine.State
{
    /// <summary>
    /// Interface for game states in the state machine system.
    /// P20-02-02: Defines the contract that all game states must implement.
    /// </summary>
    public interface IGameState
    {
        /// <summary>
        /// Called when the state is entered.
        /// Initialize state-specific resources and logic here.
        /// </summary>
        void Enter();
        
        /// <summary>
        /// Called when the state is exited.
        /// Clean up state-specific resources and logic here.
        /// </summary>
        void Exit();
        
        /// <summary>
        /// Called each frame when the state is active.
        /// Update state-specific logic and systems here.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last update in seconds.</param>
        void Update(float deltaTime);
        
        /// <summary>
        /// Called when events are forwarded to the state.
        /// Handle state-specific input and system events here.
        /// </summary>
        /// <param name="gameEvent">The game event to handle.</param>
        void HandleEvent(GameEvent gameEvent);
    }
}




