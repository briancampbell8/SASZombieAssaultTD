using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Core; // Add this namespace for ModernLoggingSystem

namespace SASZombieAssaultTD.Engine.Scenes
{
    /// <summary>
    /// Scene manager for handling scene transitions and lifecycle.
    /// P20-02-05: Manages scene loading, unloading, and transitions.
    /// </summary>
    public class SceneManager
    {
        private BaseScene? _currentScene;
        private BaseScene? _nextScene;
        private Dictionary<string, BaseScene> _loadedScenes;
        private bool _isTransitioning;
        private float _transitionTimer;
        private float _transitionDuration;
        internal string GameStateType;

        /// <summary>
        /// Gets currently active scene.
        /// </summary>
        public BaseScene? CurrentScene => _currentScene;

        /// <summary>
        /// Gets the active scene (alias for CurrentScene).
        /// </summary>
        public BaseScene? ActiveScene => _currentScene;

        /// <summary>
        /// Gets the current game state.
        /// </summary>
        public string GameState => _currentScene?.GetType().Name.Replace("Scene", "") ?? "Unknown";

        /// <summary>
        /// Gets whether a scene transition is in progress.
        /// </summary>
        public bool IsTransitioning => _isTransitioning;

        /// <summary>
        /// Gets the transition duration in seconds.
        /// </summary>
        public float TransitionDuration => _transitionDuration;

        /// <summary>
        /// Event fired when a scene transition begins.
        /// </summary>
        public event Action<string, string>? OnSceneTransitionStarted;

        /// <summary>
        /// Event fired when a scene transition completes.
        /// </summary>
        public event Action<string, string>? OnSceneTransitionCompleted;

        public SceneManager()
        {
            _loadedScenes = new Dictionary<string, BaseScene>();
            _isTransitioning = false;
            _transitionTimer = 0f;
            _transitionDuration = 1.0f;
        }

        /// <summary>
        /// Sets the transition duration.
        /// </summary>
        /// <param name="duration">Duration in seconds.</param>
        public void SetTransitionDuration(float duration)
        {
            _transitionDuration = System.Math.Max(0.1f, duration);
            ModernLoggingSystem.Log("INFO", $"SceneManager: Transition duration set to {_transitionDuration:F2}s");
        }

        /// <summary>
        /// Loads a scene by name.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <returns>The loaded scene, or null if loading failed.</returns>
        public BaseScene? LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                ModernLoggingSystem.Log("ERROR", "SceneManager: Cannot load scene with null or empty name");
                return null;
            }

            if (_loadedScenes.ContainsKey(sceneName))
            {
                ModernLoggingSystem.Log("WARNING", $"SceneManager: Scene '{sceneName}' is already loaded");
                return _loadedScenes[sceneName];
            }

            BaseScene? scene = CreateScene(sceneName);
            if (scene == null)
            {
                ModernLoggingSystem.Log("ERROR", $"SceneManager: Failed to create scene '{sceneName}'");
                return null;
            }

            try
            {
                scene.Initialize();
                scene.LoadContent();

                _loadedScenes[sceneName] = scene;

                ModernLoggingSystem.Log("INFO", $"SceneManager: Successfully loaded scene '{sceneName}'");
                return scene;
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"SceneManager: Exception loading scene '{sceneName}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Unloads a scene by name.
        /// </summary>
        /// <param name="sceneName">The name of the scene to unload.</param>
        /// <returns>True if the scene was unloaded successfully.</returns>
        public bool UnloadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                ModernLoggingSystem.Log("ERROR", "SceneManager: Cannot unload scene with null or empty name");
                return false;
            }

            if (!_loadedScenes.ContainsKey(sceneName))
            {
                ModernLoggingSystem.Log("WARNING", $"SceneManager: Scene '{sceneName}' is not loaded");
                return false;
            }

            if (_currentScene != null && GetSceneName(_currentScene) == sceneName)
            {
                ModernLoggingSystem.Log("ERROR", $"SceneManager: Cannot unload active scene '{sceneName}'");
                return false;
            }

            try
            {
                var scene = _loadedScenes[sceneName];
                scene.Cleanup();

                _loadedScenes.Remove(sceneName);

                ModernLoggingSystem.Log("INFO", $"SceneManager: Successfully unloaded scene '{sceneName}'");
                return true;
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"SceneManager: Exception unloading scene '{sceneName}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Switches to a new scene.
        /// </summary>
        /// <param name="sceneName">The name of the scene to switch to.</param>
        /// <returns>True if the scene switch was initiated successfully.</returns>
        public bool SwitchToScene(string sceneName)
        {
            if (_isTransitioning)
            {
                ModernLoggingSystem.Log("WARNING", "SceneManager: Cannot switch scenes during transition");
                return false;
            }

            var nextScene = LoadScene(sceneName);
            if (nextScene == null)
            {
                ModernLoggingSystem.Log("ERROR", $"SceneManager: Failed to load scene '{sceneName}' for transition");
                return false;
            }

            return StartTransition(_currentScene, nextScene);
        }

        /// <summary>
        /// Sets a scene immediately (no transition).
        /// </summary>
        /// <param name="sceneName">The name of the scene to set.</param>
        /// <returns>True if scene was set successfully.</returns>
        public bool SetScene(string sceneName)
        {
            var scene = LoadScene(sceneName);
            if (scene == null)
            {
                ModernLoggingSystem.Log("ERROR", $"SceneManager: Failed to load scene '{sceneName}'");
                return false;
            }

            // Clean up current scene
            if (_currentScene != null)
            {
                _currentScene.Cleanup();
            }

            // Set new current scene
            _currentScene = scene;
            ModernLoggingSystem.Log("INFO", $"SceneManager: Set scene to '{sceneName}'");
            return true;
        }

        /// <summary>
        /// Queues a scene for loading and switching.
        /// </summary>
        /// <param name="sceneName">The name of the scene to queue.</param>
        /// <returns>True if scene was queued successfully.</returns>
        public bool QueueScene(string sceneName)
        {
            return SwitchToScene(sceneName);
        }

        /// <summary>
        /// Starts a transition between scenes.
        /// </summary>
        /// <param name="fromScene">The scene to transition from.</param>
        /// <param name="toScene">The scene to transition to.</param>
        /// <returns>True if the transition was started successfully.</returns>
        public bool StartTransition(BaseScene? fromScene, BaseScene toScene)
        {
            if (_isTransitioning)
            {
                ModernLoggingSystem.Log("WARNING", "SceneManager: Cannot start transition - already in progress");
                return false;
            }

            _nextScene = toScene;
            _isTransitioning = true;
            _transitionTimer = 0f;

            var fromSceneName = fromScene != null ? GetSceneName(fromScene) : "None";
            var toSceneName = GetSceneName(toScene);

            ModernLoggingSystem.Log("INFO", $"SceneManager: Starting transition from '{fromSceneName}' to '{toSceneName}'");

            // Fire transition started event
            OnSceneTransitionStarted?.Invoke(fromSceneName, toSceneName);

            return true;
        }

        /// <summary>
        /// Updates the scene manager and current scene.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last update in seconds.</param>
        public void Update(float deltaTime)
        {
            // Update current scene
            _currentScene?.Update(deltaTime);

            // Handle scene transitions
            if (_isTransitioning && _nextScene != null)
            {
                _transitionTimer += deltaTime;

                if (_transitionTimer >= _transitionDuration)
                {
                    CompleteTransition();
                }
            }
        }

        /// <summary>
        /// Renders the current scene.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        public void Render(IRenderContext renderContext)
        {
            _currentScene?.Render(renderContext);
        }

        /// <summary>
        /// Completes the current scene transition.
        /// </summary>
        private void CompleteTransition()
        {
            if (!_isTransitioning || _nextScene == null)
                return;

            var fromSceneName = _currentScene != null ? GetSceneName(_currentScene) : "None";
            var toSceneName = GetSceneName(_nextScene);

            ModernLoggingSystem.Log("INFO", $"SceneManager: Completing transition from '{fromSceneName}' to '{toSceneName}'");

            // Clean up current scene
            if (_currentScene != null)
            {
                _currentScene.Cleanup();
            }

            // Set new current scene
            _currentScene = _nextScene;
            _nextScene = null;
            _isTransitioning = false;
            _transitionTimer = 0f;

            // Fire transition completed event
            OnSceneTransitionCompleted?.Invoke(fromSceneName, toSceneName);
        }

        /// <summary>
        /// Creates a scene instance by name.
        /// </summary>
        /// <param name="sceneName">The name of the scene to create.</param>
        /// <returns>The created scene instance, or null if creation failed.</returns>
        private BaseScene? CreateScene(string sceneName)
        {
            switch (sceneName)
            {
                case "MainMenu":
                    return new MainMenuScene();

                case "Gameplay":
                    return new GameScene();

                case "Loading":
                    return new LoadingScene();

                case "Pause":
                    return new PauseScene();

                default:
                    ModernLoggingSystem.Log("ERROR", $"SceneManager: Unknown scene type '{sceneName}'");
                    return null;
            }
        }

        /// <summary>
        /// Gets the name of a scene.
        /// </summary>
        /// <param name="scene">The scene to get the name for.</param>
        /// <returns>The scene name, or "Unknown" if the scene type is not recognized.</returns>
        private string GetSceneName(BaseScene scene)
        {
            if (scene == null)
                return "None";

            return scene.GetType().Name.Replace("Scene", "");
        }

        /// <summary>
        /// Gets all loaded scene names.
        /// </summary>
        /// <returns>Array of loaded scene names.</returns>
        public string[] GetLoadedSceneNames()
        {
            return _loadedScenes.Keys.ToArray();
        }

        /// <summary>
        /// Gets the number of loaded scenes.
        /// </summary>
        /// <returns>The count of loaded scenes.</returns>
        public int GetLoadedSceneCount()
        {
            return _loadedScenes.Count;
        }

        /// <summary>
        /// Checks if a scene is loaded.
        /// </summary>
        /// <param name="sceneName">The name of the scene to check.</param>
        /// <returns>True if the scene is loaded.</returns>
        public bool IsSceneLoaded(string sceneName)
        {
            return _loadedScenes.ContainsKey(sceneName);
        }

        /// <summary>
        /// Cleans up all loaded scenes.
        /// </summary>
        public void Cleanup()
        {
            ModernLoggingSystem.Log("INFO", "SceneManager: Cleaning up all scenes");

            // Cleanup all loaded scenes
            foreach (var kvp in _loadedScenes)
            {
                kvp.Value.Cleanup();
            }

            _loadedScenes.Clear();
            _currentScene = null;
            _nextScene = null;
            _isTransitioning = false;
            _transitionTimer = 0f;
        }

        /// <summary>
        /// Queues a scene for loading and switching using object parameter.
        /// Converts object to string and delegates to string-based method.
        /// </summary>
        /// <param name="name">The scene name as object (for compatibility).</param>
        /// <returns>True if scene was queued successfully.</returns>
        internal void QueueScene(object name)
        {
            if (name == null)
            {
                ModernLoggingSystem.Log("ERROR", "SceneManager: Cannot queue scene with null object");
                return;
            }

            string sceneName = name.ToString();
            if (string.IsNullOrEmpty(sceneName))
            {
                ModernLoggingSystem.Log("ERROR", "SceneManager: Object converted to null or empty string");
                return;
            }

            // Delegate to the existing QueueScene(string) method
            bool success = QueueScene(sceneName);
            if (!success)
            {
                ModernLoggingSystem.Log("ERROR", $"SceneManager: Failed to queue scene '{sceneName}' from object parameter");
            }
        }
    }
}
