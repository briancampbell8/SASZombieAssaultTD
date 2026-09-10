// =====================================================================================================
//  FILE: SceneManager.cs
//  PATH: Engine/Scenes/SceneManager.cs
//  SUBSYSTEM: Scene System / Deterministic Scene Pipeline Manager
//
//  ROLE:
//      Serves as the authoritative coordinator for the entire scene processing pipeline.
//      Ensures all scene subsystems (BaseScene, SceneFactory, SceneTransitions, and all concrete scenes)
//      remain synchronized deterministically. SceneManager does NOT perform rendering, creation,
//      caching, unloading, or transition timing itself.
//
//  RESPONSIBILITIES:
//      - Maintain deterministic synchronization between all scene subsystems.
//      - Coordinate scene lifecycle sequencing across the pipeline.
//      - Delegate scene creation, caching, and unloading to SceneFactory.
//      - Delegate transition timing and progress to SceneTransitions.
//      - Delegate rendering to RenderManager via BaseScene / StateMachine.
//      - Emit processing flags confirming subsystem completion states.
//      - Serve as the single point of orchestration for scene state consistency.
//
//  NON-RESPONSIBILITIES:
//      - Creating or destroying scenes directly.
//      - Performing transition timing or visual effects.
//      - Managing GameRootMain wiring or asset registration.
//      - Executing render commands or frame updates.
//      - Handling diagnostics or logging beyond synchronization events.
//
//  ARCHITECTURAL NOTES:
//      - SceneManager is the synchronization nucleus of the scene system.
//      - SceneFactory, SceneTransitions, BaseScene, and StateMachine operate as independent subsystems.
//      - All scene state changes flow through SceneManager for deterministic sequencing.
//      - StateMachine owns the active scene and its Update/Render; SceneManager coordinates transitions.
//
//  CHANGE HISTORY:
//      2026-07-30 — Removed invalid GameRootMain-only constructor; enforced SceneFactory-based construction.
//      2026-07-31 — Wired SceneManager.SetScene(...) to StateMachine.SetScene(BaseScene) for active scene.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.State;
using static SASZombieAssaultTD.Engine.Scenes.SceneTransitions;

namespace SASZombieAssaultTD.Engine.Scenes
{
    // -------------------------------------------------------------------------------------------------
    //  ENUM: SceneProcessingFlags
    // -------------------------------------------------------------------------------------------------
    public enum SceneProcessingFlags
    {
        None = 0,

        // Scene lifecycle confirmations
        SceneLoaded = 1,
        SceneStarted = 2,
        SceneUpdated = 4,
        SceneRendered = 8,
        SceneUnloaded = 16,

        // Transition confirmations
        TransitionStarted = 32,
        TransitionCompleted = 64,

        // Factory confirmations
        FactoryCreated = 128,
        FactoryCached = 256,
        FactoryUnloaded = 512,

        // Error or diagnostic conditions
        ErrorDetected = 1024,
        RecoveryAttempted = 2048,
        RecoverySucceeded = 4096,

        // Pipeline synchronization
        PipelineSynchronized = 8192,
        PipelineDesynchronized = 16384
    }

    // -------------------------------------------------------------------------------------------------
    //  CLASS: SceneManager
    // -------------------------------------------------------------------------------------------------
    public sealed class SceneManager
    {
        private readonly SceneFactory _factory;
        private readonly StateMachine _stateMachine;

        private BaseScene? _currentScene;
        private SceneTransitions? _activeTransition;
        private string? _nextSceneName;

        private SceneProcessingFlags _processingFlags = SceneProcessingFlags.None;
        private SceneFactory sceneFactory;

        public BaseScene? CurrentScene => _currentScene;
        public SceneProcessingFlags ProcessingState => _processingFlags;

        public event Action<string, string>? OnSceneTransitionStarted;
        public event Action<string, string>? OnSceneTransitionCompleted;

        public SceneManager(SceneFactory factory, StateMachine stateMachine)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        }

        //public SceneManager(SceneFactory sceneFactory) => this.sceneFactory = sceneFactory;

        //public SceneManager() invalid constructor removed in 2026-07-30 refactor
        //{

        //}

        // -------------------------------------------------------------------------------------------------
        //  FLAG MANAGEMENT
        // -------------------------------------------------------------------------------------------------
        private void SetFlag(SceneProcessingFlags flag)
        {
            _processingFlags |= flag;
        }

        private void ClearFlags()
        {
            _processingFlags = SceneProcessingFlags.None;
        }

        // -------------------------------------------------------------------------------------------------
        //  IMMEDIATE SCENE SWITCHING
        // -------------------------------------------------------------------------------------------------
        public bool SetScene(string sceneName)
        {
            ClearFlags();

            var newScene = _factory.Create(sceneName);
            if (newScene == null)
            {
                SetFlag(SceneProcessingFlags.ErrorDetected);
                return false;
            }

            SetFlag(SceneProcessingFlags.FactoryCreated);
            SetFlag(SceneProcessingFlags.FactoryCached);
            SetFlag(SceneProcessingFlags.SceneLoaded);

            string fromName = _currentScene?.GetType().Name.Replace("Scene", "") ?? "None";
            string toName = newScene.GetType().Name.Replace("Scene", "");

            OnSceneTransitionStarted?.Invoke(fromName, toName);
            SetFlag(SceneProcessingFlags.TransitionStarted);

            // Previous scene cleanup is handled by StateMachine.SetScene(...)
            _currentScene = newScene;

            // Hand off active scene ownership to StateMachine
            _stateMachine.SetScene(_currentScene);
            SetFlag(SceneProcessingFlags.SceneStarted);

            OnSceneTransitionCompleted?.Invoke(fromName, toName);
            SetFlag(SceneProcessingFlags.TransitionCompleted);

            SetFlag(SceneProcessingFlags.PipelineSynchronized);
            return true;
        }

        // -------------------------------------------------------------------------------------------------
        //  TIMED SCENE SWITCHING (via SceneTransitions)
        // -------------------------------------------------------------------------------------------------
        public bool SwitchSceneWithTransition(string sceneName, SceneTransitionType type, float duration)
        {
            ClearFlags();

            if (_activeTransition?.IsPlaying == true)
            {
                SetFlag(SceneProcessingFlags.ErrorDetected);
                return false;
            }

            _nextSceneName = sceneName;
            _activeTransition = new SceneTransitions(type, duration);

            _activeTransition.OnTransitionCompleted += () =>
            {
                SetFlag(SceneProcessingFlags.TransitionCompleted);

                if (_nextSceneName != null)
                    SetScene(_nextSceneName);

                _nextSceneName = null;
            };

            _activeTransition.Execute(() =>
            {
                SetFlag(SceneProcessingFlags.TransitionCompleted);
            });

            SetFlag(SceneProcessingFlags.TransitionStarted);
            return true;
        }

        // -------------------------------------------------------------------------------------------------
        //  UPDATE & RENDER DELEGATION (TRANSITION ONLY)
        // -------------------------------------------------------------------------------------------------
        public void Update(float deltaTime)
        {
            if (_activeTransition?.IsPlaying == true)
            {
                _activeTransition.Update(deltaTime);
                SetFlag(SceneProcessingFlags.SceneUpdated);
            }
        }

        public void Render(D3D11Adapter_Core adapter_Core)
        {
            if (_activeTransition?.IsPlaying == true)
            {
                _activeTransition.Render(adapter_Core);
                SetFlag(SceneProcessingFlags.SceneRendered);
            }
        }

        // -------------------------------------------------------------------------------------------------
        //  TRANSITION HELPERS (for UI/HUD renderers)
        // -------------------------------------------------------------------------------------------------
        public float GetFadeAlpha() =>
            _activeTransition?.GetFadeAlpha() ?? 0f;

        public float GetSlideOffset() =>
            _activeTransition?.GetSlideOffset() ?? 0f;
    }
}
