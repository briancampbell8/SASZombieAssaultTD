// =====================================================================================================
//  FILE: StateMachine.cs
//  PATH: Engine/State/StateMachine.cs
//  SUBSYSTEM: State
//
//  ROLE:
//      Provides deterministic, minimal, Option‑B‑architecture state management for the engine lifecycle.
//      Stores simple state flags (Running, Paused, GameOver) and coordinates a single active scene
//      without introducing polymorphic state classes or hierarchies.
//
//  RESPONSIBILITIES:
//      - Maintain core engine state flags.
//      - Provide deterministic transitions: Start → Pause → Resume → GameOver → Reset.
//      - Provide state queries for GameRootMain, GameRootStateController, and GameRootUpdateLoop.
//      - Coordinate a single active BaseScene instance for update and render.
//      - Provide simple state and scene reporting for diagnostics and audit logging.
//
//  NON-RESPONSIBILITIES:
//      - Managing complex scene graphs or nested scene hierarchies.
//      - Managing gameplay logic.
//      - Managing rendering backend logic.
//      - Managing ECS systems.
//      - Managing assets or subsystems.
//
//  ARCHITECTURAL NOTES:
//      - Fully aligned with Option‑B architecture: no state polymorphism, no state classes, no hierarchy.
//      - StateMachine is intentionally simple and flag‑driven, with a single active scene reference.
//      - GameRootStateController orchestrates transitions; StateMachine stores state and active scene.
//      - Deterministic and audit‑friendly; no randomness or external dependencies.
//      - Designed for compatibility with GameRootMain.cs, GameRootUpdateLoop.cs, RenderManager, and scenes.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.Scenes;
using SASZombieAssaultTD.Engine.Systems;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.State
{
    public sealed class StateMachine : IGameStateMachine
    {
        // --------------------------------------------------------------------------------------------
        //  STATE FLAGS
        // --------------------------------------------------------------------------------------------

        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }
        public bool IsGameOver { get; private set; }

        private string _currentStateName = "Uninitialized";
        private readonly SystemRegistry _systemRegistry;

        // --------------------------------------------------------------------------------------------
        //  ACTIVE SCENE
        // --------------------------------------------------------------------------------------------

        private BaseScene _activeScene;

        public StateMachine(SystemRegistry systemRegistry)
        {
            _systemRegistry = systemRegistry ?? throw new ArgumentNullException(nameof(systemRegistry));
        }

        public StateMachine()
        {
        }

        // --------------------------------------------------------------------------------------------
        //  LIFECYCLE TRANSITIONS
        // --------------------------------------------------------------------------------------------

        public void StartGame()
        {
            IsRunning = true;
            IsPaused = false;
            IsGameOver = false;
            _currentStateName = "Running";

            DLogger.Log(LogSubsystems.State, LogLevel.Info,
                "[StateMachine] StartGame: Engine entering Running state.");
        }

        public void PauseGame()
        {
            if (!IsRunning || IsGameOver)
                return;

            IsPaused = true;
            _currentStateName = "Paused";

            DLogger.Log(LogSubsystems.State, LogLevel.Info,
                "[StateMachine] PauseGame: Engine entering Paused state.");
        }

        public void ResumeGame()
        {
            if (!IsRunning || IsGameOver)
                return;

            IsPaused = false;
            _currentStateName = "Running";

            DLogger.Log(LogSubsystems.State, LogLevel.Info,
                "[StateMachine] ResumeGame: Engine resuming Running state.");
        }

        public void ResetGame()
        {
            IsRunning = false;
            IsPaused = false;
            IsGameOver = false;
            _currentStateName = "Reset";

            DLogger.Log(LogSubsystems.State, LogLevel.Info,
                "[StateMachine] ResetGame: Engine reset to initial state.");
        }

        public void GameOver()
        {
            IsRunning = false;
            IsPaused = false;
            IsGameOver = true;
            _currentStateName = "GameOver";

            DLogger.Log(LogSubsystems.State, LogLevel.Info,
                "[StateMachine] GameOver: Engine entering GameOver state.");
        }

        public void Shutdown()
        {
            IsRunning = false;
            IsPaused = false;
            IsGameOver = true;
            _currentStateName = "Shutdown";

            DLogger.Log(LogSubsystems.State, LogLevel.Info,
                "[StateMachine] Shutdown: Engine entering Shutdown state.");

            if (_activeScene != null)
            {
                _activeScene.Cleanup();
                _activeScene = null;
            }
        }

        // --------------------------------------------------------------------------------------------
        //  SCENE MANAGEMENT
        // --------------------------------------------------------------------------------------------

        public void SetScene(BaseScene scene)
        {
            if (scene == null)
                throw new ArgumentNullException(nameof(scene));

            if (_activeScene != null)
            {
                DLogger.Log(LogSubsystems.State, LogLevel.Info,
                    $"[StateMachine] SetScene: Cleaning up previous scene '{_activeScene.GetType().Name}'.");
                _activeScene.Cleanup();
            }

            _activeScene = scene;

            DLogger.Log(LogSubsystems.State, LogLevel.Info,
                $"[StateMachine] SetScene: Activating scene '{_activeScene.GetType().Name}'.");

            _activeScene.Initialize();
            _activeScene.OnLoad();
            _activeScene.OnStart();
        }

        // --------------------------------------------------------------------------------------------
        //  UPDATE / RENDER
        // --------------------------------------------------------------------------------------------

        public void Update(float deltaTime)
        {
            if (!IsRunning || IsPaused || IsGameOver)
                return;

            _activeScene?.Update(deltaTime);
        }

        public void Render(D3D11Adapter_Core adapter)
        {
            if (!IsRunning || IsGameOver)
                return;

            if (_activeScene == null || adapter == null)
                return;

            _activeScene.Render(adapter);
        }

        public void GetRender(D3D11Adapter_Core uiContext)
        {
            if (uiContext == null)
                throw new ArgumentNullException(nameof(uiContext));

            var scene = _activeScene;
            if (scene == null)
                return;

            try
            {
                scene.Render(uiContext);
            }
            catch
            {
                // swallow to preserve render loop stability
            }
        }

        // --------------------------------------------------------------------------------------------
        //  NEXT LEVEL
        // --------------------------------------------------------------------------------------------

        public void NextLevel()
        {
            if (IsGameOver)
                return;

            IsPaused = false;
            IsRunning = true;
            _currentStateName = "Running";

            DLogger.Log(LogSubsystems.State, LogLevel.Info,
                "[StateMachine] NextLevel: Continuing in Running state.");
        }

        // --------------------------------------------------------------------------------------------
        //  STATE QUERIES
        // --------------------------------------------------------------------------------------------

        public string GetCurrentState() => _currentStateName;

        bool IGameStateMachine.IsPaused() => IsPaused;
        bool IGameStateMachine.IsRunning() => IsRunning;
        bool IGameStateMachine.IsGameOver() => IsGameOver;

        internal T GetService<T>()
        {
            throw new NotImplementedException();
        }
    }
}
