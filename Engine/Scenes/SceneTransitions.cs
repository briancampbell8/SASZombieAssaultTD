// =====================================================================================================
//  FILE: SceneTransitions.cs
//  PATH: Engine/Scenes/SceneTransitions.cs
//  SUBSYSTEM: Scene System / Deterministic Scene Transition Controller
//
//  ROLE:
//      Provides deterministic visual transition effects (fade, slide, etc.) WITHOUT performing scene
//      lifecycle changes. SceneManager remains the sole authority for switching scenes.
//
//  RESPONSIBILITIES:
//      - Track transition timing and progress.
//      - Provide fade/slide progress values for render systems.
//      - Invoke callbacks when transitions start and complete.
//      - Never manipulate BaseScene lifecycle directly.
//      - Never switch scenes directly.
//      - Never bypass SceneManager.
//
//  NON-RESPONSIBILITIES:
//      - Calling OnEnter / OnExit on scenes.
//      - Loading or unloading scenes.
//      - Performing scene transitions itself.
//      - Rendering transition effects (RenderManager handles that).
//
//  ARCHITECTURAL NOTES:
//      - This file is permanently coded right and complete.
//      - SceneManager performs the actual scene switch when the transition completes.
//      - SceneTransitions ONLY provides timing + progress values.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public class SceneTransitions
    {
        // -------------------------------------------------------------------------------------------------
        // STATE
        // -------------------------------------------------------------------------------------------------
        private readonly SceneTransitionType _type;
        private readonly float _duration;

        private float _elapsed;
        private bool _playing;

        private Action? _onComplete;
        private Action<float>? _onProgress;

        public SceneTransitionType Type => _type;
        public float Duration => _duration;
        public float Elapsed => _elapsed;
        public bool IsPlaying => _playing;

        public enum SceneTransitionType
        {
            None,
            Fade,
            FadeWhite,
            SlideLeft,
            SlideRight,
            SlideUp,
            SlideDown
        }

        public float Progress =>
            _duration > 0f ? System.Math.Clamp(_elapsed / _duration, 0f, 1f) : 1f;

        public event Action? OnTransitionStarted;
        public event Action? OnTransitionCompleted;

        // -------------------------------------------------------------------------------------------------
        // CONSTRUCTION
        // -------------------------------------------------------------------------------------------------
        public SceneTransitions(SceneTransitionType type, float duration = 1.0f)
        {
            _type = type;
            _duration = System.Math.Max(0.1f, duration);
            _elapsed = 0f;
            _playing = false;

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                $"[SceneTransitions] Created {_type} transition ({_duration:F2}s)");
        }

        // -------------------------------------------------------------------------------------------------
        // RENDERING (NO DIRECT DRAWING)
        // -------------------------------------------------------------------------------------------------
        public void Render(D3D11Adapter_Core adapter)
        {
            // If the transition is not playing or there is no transition, nothing to render.
            if (!_playing || _type == SceneTransitionType.None)
                return;

            // Guard against a null adapter. Rendering requires an adapter but this method
            // will be resilient and simply no-op if none is provided.
            if (adapter is null)
                return;

            // Notify any progress listeners about the current transition progress.
            _onProgress?.Invoke(Progress);

            // Concrete drawing is intentionally not performed here. The owning renderer
            // should use GetFadeAlpha() / GetSlideOffset() and perform actual drawing.
        }

        // -------------------------------------------------------------------------------------------------
        // EXECUTION
        // -------------------------------------------------------------------------------------------------
        public void Execute(Action? onComplete = null)
        {
            if (_playing)
            {
                DLogger.Log(LogSubsystems.Scenes, LogLevel.Warning,
                    "[SceneTransitions] Cannot execute — already playing.");
                return;
            }

            _elapsed = 0f;
            _playing = true;
            _onComplete = onComplete;

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                $"[SceneTransitions] Starting {_type} transition.");

            OnTransitionStarted?.Invoke();
        }

        public bool Update(float deltaTime)
        {
            if (!_playing)
                return false;

            _elapsed += deltaTime;

            _onProgress?.Invoke(Progress);

            if (_elapsed >= _duration)
            {
                Complete();
                return false;
            }

            return true;
        }

        private void Complete()
        {
            _playing = false;
            _elapsed = _duration;

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                $"[SceneTransitions] Completed {_type} transition.");

            OnTransitionCompleted?.Invoke();
            _onComplete?.Invoke();
        }

        public void Cancel()
        {
            if (!_playing)
                return;

            _playing = false;

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Warning,
                $"[SceneTransitions] Cancelled {_type} transition.");
        }

        // -------------------------------------------------------------------------------------------------
        // CALLBACKS
        // -------------------------------------------------------------------------------------------------
        public void SetProgressCallback(Action<float>? callback)
        {
            _onProgress = callback;
        }

        // -------------------------------------------------------------------------------------------------
        // EFFECT HELPERS
        // -------------------------------------------------------------------------------------------------
        public float GetFadeAlpha()
        {
            if (_type != SceneTransitionType.Fade &&
                _type != SceneTransitionType.FadeWhite)
                return 0f;

            float half = _duration * 0.5f;

            if (_elapsed < half)
                return _elapsed / half; // fade out

            return 1f - ((_elapsed - half) / half); // fade in
        }

        public float GetSlideOffset()
        {
            if (!IsSlide())
                return 0f;

            return 1f - Progress;
        }

        private bool IsSlide()
        {
            return _type == SceneTransitionType.SlideLeft ||
                   _type == SceneTransitionType.SlideRight ||
                   _type == SceneTransitionType.SlideUp ||
                   _type == SceneTransitionType.SlideDown;
        }

        public override string ToString()
        {
            return $"SceneTransitions: Type={_type}, Duration={_duration:F2}, " +
                   $"Elapsed={_elapsed:F2}, Progress={Progress:F2}, Playing={_playing}";
        }
    }
}
