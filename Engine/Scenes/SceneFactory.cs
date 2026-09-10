// =====================================================================================================
//  FILE: SceneFactory.cs
//  PATH: Engine/Scenes/SceneFactory.cs
//  SUBSYSTEM: Scene Construction / Scene Registry / Lifecycle Wiring
//
//  ROLE:
//      Fully responsible for constructing, registering, caching, unloading, and wiring scenes.
//      This class removes all non-essential responsibilities from SceneManager.
//
//  RESPONSIBILITIES:
//      - Construct scenes deterministically (Option B architecture).
//      - Cache constructed scenes.
//      - Unload scenes safely.
//      - Wire GameRootMain and SceneManager into scenes.
//      - Provide scene name resolution.
//      - Provide diagnostics for scene creation and unloading.
//
//  NON-RESPONSIBILITIES:
//      - Scene transitions (SceneManager).
//      - Scene update/render delegation (SceneManager).
//      - Transition timing (SceneTransitionController).
//
//  ARCHITECTURAL NOTES:
//      - SceneManager now delegates ALL construction, caching, unloading, and wiring to SceneFactory.
//      - SceneFactory is the authoritative source of scene instances.
//
//  DETERMINISTIC EXECUTION FLOW:
//      1. GameRootMain wires SceneFactory with GameRootMain and SceneManager.
//      2. SceneManager requests scenes via SceneFactory.Create(sceneName).
//      3. SceneFactory constructs or returns cached BaseScene instances.
//      4. SceneFactory wires GameRootMain and SceneManager into scenes.
//      5. SceneFactory calls OnLoad() before caching and returning scenes.
//      6. SceneManager activates scenes and drives update/render.
//
//  CHANGE HISTORY:
//      2026-07-30 — Introduced SceneFactory as authoritative scene construction/caching subsystem.
//      2026-07-30 — Moved scene wiring responsibilities from SceneManager to SceneFactory.
// =====================================================================================================


using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public sealed class SceneFactory
    {
        private readonly Dictionary<string, BaseScene> _cache = new();

        private GameRootMain? _gameRoot;
        private SceneManager? _sceneManager;

        // ---------------------------------------------------------------------------------------------
        // Wiring
        // ---------------------------------------------------------------------------------------------

        public void SetGameRoot(GameRootMain root)
        {
            _gameRoot = root;
            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                "SceneFactory: GameRootMain wired.");
        }

        public void SetSceneManager(SceneManager manager)
        {
            _sceneManager = manager;
            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                "SceneFactory: SceneManager wired.");
        }

        // ---------------------------------------------------------------------------------------------
        // Scene Creation
        // ---------------------------------------------------------------------------------------------

        public BaseScene? Create(string sceneName)
        {
            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                $"SceneFactory: Create('{sceneName}') ENTRY");

            if (string.IsNullOrWhiteSpace(sceneName))
            {
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Error,
                    "SceneFactory: Cannot create scene with null or empty name.");
                return null;
            }

            if (_cache.TryGetValue(sceneName, out var existing))
            {
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                    $"SceneFactory: Returning cached scene '{sceneName}'.");
                return existing;
            }

            BaseScene? scene = sceneName switch
            {
                "MainMenu" => new MainMenuScene(),
                "Gameplay" => new GameScene(),
                "Loading" => new LoadingScene(),
                "Pause" => new PauseScene(),
                "MapSelection" => new MapSelectionScene(),

                _ => null
            };

            if (scene == null)
            {
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Error,
                    $"SceneFactory: Unknown scene '{sceneName}'.");
                return null;
            }

            if (_gameRoot != null)
                scene.SetGameRoot(_gameRoot);

            if (_sceneManager != null)
                scene.SetSceneManager(_sceneManager);

            try
            {
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                    $"SceneFactory: Calling OnLoad('{sceneName}')");

                scene.OnLoad();

                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                    $"SceneFactory: OnLoad('{sceneName}') COMPLETED");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Error,
                    $"SceneFactory: Exception during OnLoad('{sceneName}'): {ex.Message}");
                return null;
            }

            _cache[sceneName] = scene;

            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                $"SceneFactory: Scene '{sceneName}' cached.");

            return scene;
        }

        // ---------------------------------------------------------------------------------------------
        // Scene Unloading
        // ---------------------------------------------------------------------------------------------

        public bool Unload(string sceneName)
        {
            if (!_cache.ContainsKey(sceneName))
                return false;

            var scene = _cache[sceneName];

            try
            {
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                    $"SceneFactory: Calling OnUnload('{sceneName}')");

                scene.OnUnload();

                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                    $"SceneFactory: OnUnload('{sceneName}') COMPLETED");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Error,
                    $"SceneFactory: Exception during OnUnload('{sceneName}'): {ex.Message}");
            }

            _cache.Remove(sceneName);

            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                $"SceneFactory: Scene '{sceneName}' removed from cache.");

            return true;
        }

        // ---------------------------------------------------------------------------------------------
        // Diagnostics
        // ---------------------------------------------------------------------------------------------

        public bool IsSceneLoaded(string name) => _cache.ContainsKey(name);

        public string[] GetLoadedSceneNames() => new List<string>(_cache.Keys).ToArray();

        public int GetLoadedSceneCount() => _cache.Count;

        public void Cleanup()
        {
            foreach (var kvp in _cache)
            {
                try
                {
                    kvp.Value.OnUnload();
                }
                catch { }
            }

            _cache.Clear();

            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                "SceneFactory: Cleanup completed.");
        }

        // ---------------------------------------------------------------------------------------------
        // Name Resolution
        // ---------------------------------------------------------------------------------------------

        public string ResolveName(BaseScene scene)
        {
            return scene.GetType().Name.Replace("Scene", "");
        }
    }
}
