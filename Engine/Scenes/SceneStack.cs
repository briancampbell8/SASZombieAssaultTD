// =====================================================================================================
//  FILE: SceneStack.cs
//  PATH: Engine/Scenes/SceneStack.cs
//  SUBSYSTEM: Scene System / Overlay Scene Stack
//
//  ROLE:
//      Deterministic overlay scene stack for managing temporary scenes such as PauseScene, LoadingScene,
//      and other non-primary overlays. Provides push/pop/replace semantics for overlay navigation while
//      remaining fully in sync with the modern BaseScene + SceneManager architecture.
//
//  RESPONSIBILITIES:
//      - Maintain a stack of BaseScene overlay instances.
//      - Provide Push / Pop / Replace / Peek operations for overlay scenes.
//      - Forward Update / Render calls to the top overlay scene.
//      - Expose events for scene push/pop/replace operations.
//
//  NON-RESPONSIBILITIES:
//      - Managing primary scenes (MainMenuScene, GameScene, MapSelectionScene).
//      - Loading scenes by name or performing asset management.
//      - Owning global scene transitions (handled by SceneManager).
//
//  ARCHITECTURAL NOTES:
//      - Permanently coded right and complete as the canonical overlay SceneStack.
//      - Uses BaseScene’s public Update / Render surface.
//      - Does not call legacy lifecycle methods.
//      - Does not perform name-based loading; callers construct and wire scenes explicitly.
// =====================================================================================================

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public sealed class SceneStack
    {
        private readonly Stack<BaseScene> _sceneStack = new();
        private SceneManager? _sceneManager;

        // -------------------------------------------------------------------------------------------------
        // PROPERTIES
        // -------------------------------------------------------------------------------------------------

        public int Count => _sceneStack.Count;
        public bool IsEmpty => _sceneStack.Count == 0;

        public event Action<BaseScene>? OnScenePushed;
        public event Action<BaseScene>? OnScenePopped;
        public event Action<BaseScene, BaseScene>? OnSceneReplaced;

        // -------------------------------------------------------------------------------------------------
        // WIRING
        // -------------------------------------------------------------------------------------------------

        public void SetSceneManager(SceneManager manager)
        {
            _sceneManager = manager;

            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                "[SceneStack] SceneManager wired.");
        }

        // -------------------------------------------------------------------------------------------------
        // STACK OPERATIONS
        // -------------------------------------------------------------------------------------------------

        public bool Push(BaseScene scene)
        {
            if (scene == null)
            {
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Error,
                    "[SceneStack] Cannot push null scene.");
                return false;
            }

            try
            {
                _sceneStack.Push(scene);

                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                    $"[SceneStack] Pushed scene '{scene.GetType().Name}' (stack size: {_sceneStack.Count}).");

                OnScenePushed?.Invoke(scene);
                return true;
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Error,
                    $"[SceneStack] Failed to push scene: {ex.Message}");
                return false;
            }
        }

        public BaseScene? Pop()
        {
            if (_sceneStack.Count == 0)
            {
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Warning,
                    "[SceneStack] Cannot pop from empty stack.");
                return null;
            }

            try
            {
                var popped = _sceneStack.Pop();
                popped.Cleanup();

                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                    $"[SceneStack] Popped scene '{popped.GetType().Name}' (stack size: {_sceneStack.Count}).");

                OnScenePopped?.Invoke(popped);
                return popped;
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Error,
                    $"[SceneStack] Failed to pop scene: {ex.Message}");
                return null;
            }
        }

        public BaseScene? Replace(BaseScene scene)
        {
            if (scene == null)
            {
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Error,
                    "[SceneStack] Cannot replace with null scene.");
                return null;
            }

            if (_sceneStack.Count == 0)
            {
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Warning,
                    "[SceneStack] Replace on empty stack; performing push instead.");
                Push(scene);
                return null;
            }

            try
            {
                var replaced = _sceneStack.Pop();
                replaced.Cleanup();

                _sceneStack.Push(scene);

                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                    $"[SceneStack] Replaced '{replaced.GetType().Name}' with '{scene.GetType().Name}'.");

                OnSceneReplaced?.Invoke(replaced, scene);
                return replaced;
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Error,
                    $"[SceneStack] Failed to replace scene: {ex.Message}");
                return null;
            }
        }

        public BaseScene? Peek()
        {
            return _sceneStack.Count == 0 ? null : _sceneStack.Peek();
        }

        public void Clear()
        {
            if (_sceneStack.Count == 0)
            {
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Warning,
                    "[SceneStack] Stack already empty.");
                return;
            }

            try
            {
                var count = _sceneStack.Count;

                while (_sceneStack.Count > 0)
                {
                    var scene = _sceneStack.Pop();
                    scene.Cleanup();
                }

                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                    $"[SceneStack] Cleared {count} scenes from stack.");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Error,
                    $"[SceneStack] Failed to clear stack: {ex.Message}");
            }
        }

        public BaseScene[] GetAllScenes()
        {
            return _sceneStack.ToArray();
        }

        // -------------------------------------------------------------------------------------------------
        // UPDATE / RENDER FOR TOP OVERLAY
        // -------------------------------------------------------------------------------------------------

        public void Update(float deltaTime)
        {
            var top = Peek();
            top?.Update(deltaTime);
        }

        public void Render(D3D11Adapter_Core adapter_Core)
        {
            var top = Peek();
            top?.Render(adapter_Core);
        }

        // -------------------------------------------------------------------------------------------------
        // DEBUG
        // -------------------------------------------------------------------------------------------------

        public override string ToString()
        {
            var names = new List<string>();

            foreach (var scene in _sceneStack)
                names.Add(scene.GetType().Name);

            names.Reverse();

            return $"SceneStack: Count={_sceneStack.Count}, Scenes=[{string.Join(" -> ", names)}]";
        }
    }
}
