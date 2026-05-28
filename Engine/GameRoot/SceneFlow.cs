/*
File:    SceneFlow.cs
Path:    Engine/GameRoot/SceneFlow.cs
Purpose: P11-09-01 - Controls all scene transitions and scene lifecycle.
         Ensures scenes load, initialize, and unload cleanly.

Role:     Scene lifecycle management specialist.
         - Scene switching functions
         - Scene loading/unloading functions
         - Scene initialization functions
         - Scene cleanup functions
         - Scene transition coordination

Notes:    Contains all scene management logic extracted from GameRoot.
         Works with the unified Engine/Scenes namespace.
         Scene transitions are coordinated through the state machine.
*/

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Scenes;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Systems;
using SASZombieAssaultTD.Engine.UI.Input;

namespace SASZombieAssaultTD.Engine
{
    /// <summary>
    /// Partial class containing scene flow logic for GameRoot.
    /// </summary>
    public partial class GameRoot
    {
        public GameRoot(SystemRegistry systemRegistry, SystemManager systemManager, UpdateManager updateManager, RenderManager renderManager, UIInputRouter inputRouter, IGameStateMachine gameStateMachine, Rendering.RenderContextD3D11Adapter renderContextAdapter)
        {
            _systemRegistry = systemRegistry;
            _systemManager = systemManager;
            _updateManager = updateManager;
            _renderManager = renderManager;
        }

        /// <summary>
        /// Gets the scene manager for scene operations.
        /// </summary>
        public SceneManager? SceneManager => GetSceneManager();

        /// <summary>
        /// Retrieves the scene manager from the system registry.
        /// </summary>
        /// <returns>The scene manager instance, or null if not available.</returns>
        private SceneManager? GetSceneManager()
        {
            try
            {
                return _systemRegistry.GetService<SceneManager>();
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"Failed to retrieve SceneManager: {ex.Message}");
                Engine.Diagnostics.DebugLogger.Exception(ex, "SceneManager retrieval");
                return null;
            }
        }

        /// <summary>
        /// Switches to a new scene by name.
        /// </summary>
        /// <param name="sceneName">The name of the scene to switch to.</param>
        /// <returns>True if the scene switch was initiated successfully.</returns>
        public bool SwitchToScene(string sceneName)
        {
            var sceneManager = GetSceneManager();
            if (sceneManager == null)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", "Cannot switch scene - SceneManager not available");
                return false;
            }

            return sceneManager.SwitchToScene(sceneName);
        }

        /// <summary>
        /// Loads a scene by name without switching to it.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <returns>The loaded scene, or null if loading failed.</returns>
        public BaseScene? LoadScene(string sceneName)
        {
            var sceneManager = GetSceneManager();
            if (sceneManager == null)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", "Cannot load scene - SceneManager not available");
                return null;
            }

            return sceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Unloads a scene by name.
        /// </summary>
        /// <param name="sceneName">The name of the scene to unload.</param>
        /// <returns>True if the scene was unloaded successfully.</returns>
        public bool UnloadScene(string sceneName)
        {
            var sceneManager = GetSceneManager();
            if (sceneManager == null)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", "Cannot unload scene - SceneManager not available");
                return false;
            }

            return sceneManager.UnloadScene(sceneName);
        }

        /// <summary>
        /// Gets the currently active scene.
        /// </summary>
        /// <returns>The currently active scene, or null if no scene is active.</returns>
        public BaseScene? GetCurrentScene()
        {
            var sceneManager = GetSceneManager();
            return sceneManager?.CurrentScene;
        }

        /// <summary>
        /// Gets all loaded scene names.
        /// </summary>
        /// <returns>Array of loaded scene names.</returns>
        public string[] GetLoadedSceneNames()
        {
            var sceneManager = GetSceneManager();
            return sceneManager?.GetLoadedSceneNames() ?? new string[0];
        }

        /// <summary>
        /// Checks if a scene is loaded.
        /// </summary>
        /// <param name="sceneName">The name of the scene to check.</param>
        /// <returns>True if the scene is loaded.</returns>
        public bool IsSceneLoaded(string sceneName)
        {
            var sceneManager = GetSceneManager();
            return sceneManager?.IsSceneLoaded(sceneName) ?? false;
        }
    }
}
