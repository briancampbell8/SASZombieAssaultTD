/*
File:    State.cs
Path:    Engine/GameRoot/State.cs
Purpose: P11-09-01 - Controls the overall game state.
         Manages transitions between gameplay states and maintains flags.

Role:     Game state management specialist.
         - Game start/pause/resume/reset functions
         - Game over/level transition functions
         - Internal state machine logic
         - State transition coordination
         - State persistence management

Notes:    Contains all state management logic extracted from GameRoot.
         Works with the IGameStateMachine for state transitions.
         State changes are centralized for better control and debugging.
*/

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using IRenderContext = SASZombieAssaultTD.Engine.Interfaces.IRenderContext;
namespace SASZombieAssaultTD.Engine
//
{
    ///<summary>
    ///Interface for game state machine operations.
    ///</summary>
    public interface IGameStateMachine
    {
        ///<summary>
        ///Initializes the state machine.
        ///</summary>
        void Initialize();

        ///<summary>
        ///Shuts down the state machine.
        ///</summary>
        void Shutdown();

        ///<summary>
        ///Updates the state machine.
        ///</summary>
        ///<param name="deltaTime">Time since last update.</param>
        void Update(float deltaTime);

        ///<summary>
        ///Renders the current state.
        ///</summary>
        ///<param name="renderContext">The render context.</param>
        void Render(IRenderContext renderContext);

        ///<summary>
        ///Starts the game.
        ///</summary>
        void StartGame();

        ///<summary>
        ///Pauses the game.
        ///</summary>
        void PauseGame();

        ///<summary>
        ///Resumes the game.
        ///</summary>
        void ResumeGame();

        ///<summary>
        ///Resets the game.
        ///</summary>
        void ResetGame();

        ///<summary>
        ///Triggers game over.
        ///</summary>
        void GameOver();

        ///<summary>
        ///Transitions to next level.
        ///</summary>
        void NextLevel();

        ///<summary>
        ///Gets the current state.
        ///</summary>
        ///<returns>The current state object.</returns>
        object? GetCurrentState();

        ///<summary>
        ///Checks if the game is paused.
        ///</summary>
        ///<returns>True if paused.</returns>
        bool IsPaused();

        ///<summary>
        ///Checks if the game is running.
        ///</summary>
        ///<returns>True if running.</returns>
        bool IsRunning();

        ///<summary>
        ///Checks if the game is over.
        ///</summary>
        ///<returns>True if game over.</returns>
        bool IsGameOver();
        object InitializeAsync(object value);
    }

    ///<summary>
    ///Partial class containing state management logic for GameRoot.
    ///</summary>
    public partial class GameRoot
    {
        ///<summary>
        ///Gets the current game state machine.
        ///</summary>
        public IGameStateMachine GameStateMachine => _stateMachine;

        ///<summary>
        ///Starts the game.
        ///</summary>
        public void StartGame()
        {
            if (!_isInitialized)
            {
                DLogger.Log("Cannot start game - engine not initialized");
                return;
            }

            try
            {
                _stateMachine.StartGame();
                DLogger.Log("Game started successfully");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Error, $"Failed to start game: {ex.Message}");
                DLogger.Log(
                    LogSubsystems.GameRoot,
                    LogLevel.Error,
                    ex.ToString(), "Game start");
            }
        }

        ///<summary>
        ///Pauses the game.
        ///</summary>
        public void PauseGame()
        {
            if (!_isInitialized)
            {
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Error, "Cannot pause game - engine not initialized");
                return;
            }

            try
            {
                _stateMachine.PauseGame();
                DLogger.Log("Game paused successfully");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Error, $"Failed to pause game: {ex.Message}");
                DLogger.Log(ex.ToString(), "Game pause");
            }
        }

        ///<summary>
        ///Resumes the game.
        ///</summary>
        public void ResumeGame()
        {
            if (!_isInitialized)
            {
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Error, "Cannot resume game - engine not initialized");
                return;
            }

            try
            {
                _stateMachine.ResumeGame();
                DLogger.Log("Game resumed successfully");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Error, $"Failed to resume game: {ex.Message}");
                DLogger.Log(ex.ToString(), "Game resume");
            }
        }

        ///<summary>
        ///Resets the game.
        ///</summary>
        public void ResetGame()
        {
            if (!_isInitialized)
            {
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Error, "Cannot reset game - engine not initialized");
                return;
            }

            try
            {
                _stateMachine.ResetGame();
                DLogger.Log("Game reset successfully");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Error, $"Failed to reset game: {ex.Message}");
                DLogger.Log(ex.ToString(), "Game reset");
            }
        }

        ///<summary>
        ///Triggers game over.
        ///</summary>
        public void GameOver()
        {
            if (!_isInitialized)
            {
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Error, "Cannot trigger game over - engine not initialized");
                return;
            }

            try
            {
                _stateMachine.GameOver();
                DLogger.Log("Game over triggered successfully");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Error, $"Failed to trigger game over: {ex.Message}");
                DLogger.Log(ex.ToString(), "Game over");
            }
        }

        ///<summary>
        ///Transitions to the next level.
        ///</summary>
        public void NextLevel()
        {
            if (!_isInitialized)
            {
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Error, "Cannot transition to next level - engine not initialized");
                return;
            }

            try
            {
                _stateMachine.NextLevel();
                DLogger.Log("Transitioned to next level successfully");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Error, $"Failed to transition to next level: {ex.Message}");
                DLogger.Log(ex.ToString(), "Next level transition");
            }
        }

        ///<summary>
        ///Gets the current game state.
        ///</summary>
        ///<returns>The current game state.</returns>
        public string GetCurrentGameState()
        {
            return _stateMachine.GetCurrentState()?.ToString() ?? "Unknown";
        }

        ///<summary>
        ///Checks if the game is currently paused.
        ///</summary>
        ///<returns>True if the game is paused.</returns>
        public bool IsGamePaused()
        {
            return _stateMachine.IsPaused();
        }

        ///<summary>
        ///Checks if the game is currently running.
        ///</summary>
        ///<returns>True if the game is running.</returns>
        public bool IsGameRunning()
        {
            return _stateMachine.IsRunning();
        }

        ///<summary>
        ///Checks if the game is over.
        ///</summary>
        ///<returns>True if the game is over.</returns>
        public bool IsGameOver()
        {
            return _stateMachine.IsGameOver();
        }
    }
}
