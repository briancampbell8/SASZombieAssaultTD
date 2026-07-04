// =========================================================
//  FILE: SceneManager.cs
//  PATH: Engine/Platform/BaseScene.cs
//  SUBSYSTEM: Platform Abstraction Layer
//  ROLE: Defines the deterministic lifecycle contract
//  =========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Rendering;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public class SceneManager
    {
        private BaseScene? _currentScene;
        private BaseScene? _nextScene;
        private readonly Dictionary<string, BaseScene> _loadedScenes;

        private bool _isTransitioning;
        private float _transitionTimer;
        private float _transitionDuration = 1.0f;

        private GameRoot? _gameRoot;
        internal string GameStateType;

        public BaseScene? CurrentScene => _currentScene;
        public BaseScene? ActiveScene => _currentScene;
        public bool IsTransitioning => _isTransitioning;
        public float TransitionDuration => _transitionDuration;

        public string GameState =>
            _currentScene?.GetType().Name.Replace("Scene", "") ?? "Unknown";

        public event Action<string, string>? OnSceneTransitionStarted;
        public event Action<string, string>? OnSceneTransitionCompleted;

        public SceneManager()
        {
            _loadedScenes = new Dictionary<string, BaseScene>();
        }

        // ---------------------------------------------------------------------
        // GameRoot Wiring
        // ---------------------------------------------------------------------
        public void SetGameRoot(GameRoot root)
        {
            _gameRoot = root;
        }

        // ---------------------------------------------------------------------
        // Transition Configuration
        // ---------------------------------------------------------------------
        public void SetTransitionDuration(float duration)
        {
            _transitionDuration = System.Math.Max(0.1f, duration);
            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                $"SceneManager: Transition duration set to {_transitionDuration:F2}s");
        }

        // ---------------------------------------------------------------------
        // Scene Loading (Deterministic Lifecycle: OnLoad)
        // ---------------------------------------------------------------------
        public BaseScene? LoadScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                DLogger.Log(LogSubsystems.Scenes, LogLevel.Error,
                    "SceneManager: Cannot load scene with null or empty name");
                return null;
            }

            if (_loadedScenes.TryGetValue(sceneName, out var existing))
                return existing;

            BaseScene? scene = CreateScene(sceneName);
            if (scene == null)
            {
                DLogger.Log(LogSubsystems.Scenes, LogLevel.Error,
                    $"SceneManager: Unknown scene '{sceneName}'");
                return null;
            }

            try
            {
                if (_gameRoot != null)
                    scene.SetGameRoot(_gameRoot);

                scene.SetSceneManager(this);

                // Deterministic preload lifecycle
                scene.OnLoad();

                _loadedScenes[sceneName] = scene;

                DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                    $"SceneManager: Scene '{sceneName}' loaded (OnLoad completed)");

                return scene;
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Scenes, LogLevel.Error,
                    $"SceneManager: Exception loading scene '{sceneName}': {ex.Message}");
                return null;
            }
        }

        // ---------------------------------------------------------------------
        // Scene Unloading (Deterministic Lifecycle: OnUnload)
        // ---------------------------------------------------------------------
        public bool UnloadScene(string sceneName)
        {
            if (!_loadedScenes.ContainsKey(sceneName))
                return false;

            if (_currentScene != null &&
                GetSceneName(_currentScene) == sceneName)
            {
                DLogger.Log(LogSubsystems.Scenes, LogLevel.Error,
                    $"SceneManager: Cannot unload active scene '{sceneName}'");
                return false;
            }

            var scene = _loadedScenes[sceneName];
            scene.OnUnload();
            _loadedScenes.Remove(sceneName);

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                $"SceneManager: Scene '{sceneName}' unloaded (OnUnload completed)");

            return true;
        }

        // ---------------------------------------------------------------------
        // Scene Switching (Immediate)
        // ---------------------------------------------------------------------
        public bool SetScene(string sceneName)
        {
            var scene = LoadScene(sceneName);
            if (scene == null)
                return false;

            string fromName = _currentScene != null ? GetSceneName(_currentScene) : "None";
            string toName = GetSceneName(scene);

            _currentScene?.OnUnload();

            _currentScene = scene;
            _currentScene.OnStart();

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                $"SceneManager: Immediate switch from '{fromName}' to '{toName}'");

            OnSceneTransitionStarted?.Invoke(fromName, toName);
            OnSceneTransitionCompleted?.Invoke(fromName, toName);

            return true;
        }

        // ---------------------------------------------------------------------
        // Scene Switching (Timed Transition)
        // ---------------------------------------------------------------------
        public bool SwitchToScene(string sceneName)
        {
            if (_isTransitioning)
                return false;

            var next = LoadScene(sceneName);
            if (next == null)
                return false;

            return StartTransition(_currentScene, next);
        }

        public bool StartTransition(BaseScene? fromScene, BaseScene toScene)
        {
            if (_isTransitioning)
                return false;

            _nextScene = toScene;
            _isTransitioning = true;
            _transitionTimer = 0f;

            string fromName = fromScene != null ? GetSceneName(fromScene) : "None";
            string toName = GetSceneName(toScene);

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                $"SceneManager: Transition started from '{fromName}' to '{toName}' " +
                $"(duration {_transitionDuration:F2}s)");

            OnSceneTransitionStarted?.Invoke(fromName, toName);

            return true;
        }

        private void CompleteTransition()
        {
            if (!_isTransitioning || _nextScene == null)
                return;

            string fromName = _currentScene != null ? GetSceneName(_currentScene) : "None";
            string toName = GetSceneName(_nextScene);

            _currentScene?.OnUnload();

            _currentScene = _nextScene;
            _nextScene = null;

            _currentScene.OnStart();

            _isTransitioning = false;
            _transitionTimer = 0f;

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                $"SceneManager: Transition completed from '{fromName}' to '{toName}'");

            OnSceneTransitionCompleted?.Invoke(fromName, toName);
        }

        // ---------------------------------------------------------------------
        // Update & Render (Deterministic Lifecycle)
        // ---------------------------------------------------------------------
        public void Update(float deltaTime)
        {
            if (_isTransitioning && _nextScene != null)
            {
                _transitionTimer += deltaTime;
                if (_transitionTimer >= _transitionDuration)
                    CompleteTransition();
            }
            else
            {
                _currentScene?.OnUpdate(deltaTime);
            }
        }

        public void Render(IRenderContext context)
        {
            _currentScene?.OnRender(context);
        }

        // ---------------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------------
        private BaseScene? CreateScene(string sceneName)
        {
            return sceneName switch
            {
                "MainMenu" => new MainMenuScene(),
                "Gameplay" => new GameScene(),
                "Loading" => new LoadingScene(),
                "Pause" => new PauseScene(),
                _ => null
            };
        }

        private string GetSceneName(BaseScene scene)
        {
            return scene.GetType().Name.Replace("Scene", "");
        }

        public string[] GetLoadedSceneNames() => _loadedScenes.Keys.ToArray();
        public int GetLoadedSceneCount() => _loadedScenes.Count;
        public bool IsSceneLoaded(string name) => _loadedScenes.ContainsKey(name);

        public void Cleanup()
        {
            foreach (var scene in _loadedScenes.Values)
                scene.OnUnload();

            _loadedScenes.Clear();
            _currentScene = null;
            _nextScene = null;
            _isTransitioning = false;
            _transitionTimer = 0f;

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "SceneManager: Cleanup completed, all scenes unloaded");
        }

        internal void QueueScene(string name)
        {
            throw new NotImplementedException();
        }
    }
}
