// =====================================================================================================
//  FILE: AnimationController.cs
//  PATH: Engine/Animation/Core/Controller/AnimationController.cs
//  SUBSYSTEM: Animation Core Controller
//
//  ROLE:
//      Authoritative animation playback controller for the Animation subsystem. Provides deterministic
//      clip execution, parameter management, and transition handling. ECS-facing components delegate
//      all animation behavior to this controller.
//
//  RESPONSIBILITIES:
//      - Play and stop animation clips deterministically.
//      - Maintain the current clip name and playback state.
//      - Expose a parameter store for transitions and rule evaluation.
//      - Provide a clean API for ECS components (AnimationControllerComponent).
//
//  NON-RESPONSIBILITIES:
//      - Rendering or sprite manipulation.
//      - ECS lifecycle management.
//      - Event dispatch or system-level orchestration.
//      - Clip storage, asset loading, or track management.
//
//  ARCHITECTURAL NOTES:
//      - Lives under Animation/Core/Controller as the authoritative subsystem controller.
//      - Consumed by AnimationControllerComponent (ECS façade).
//      - Works with AnimationTransitionState, AnimationBlendState, and AnimationParameterStore.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Animation.Core.Transitions;

namespace SASZombieAssaultTD.Engine.Animation.Core.Controller
{
    internal sealed class AnimationController
    {
        // ------------------------------------------------------------------------------------------------
        // PROPERTIES
        // ------------------------------------------------------------------------------------------------
        // Surfaces required by AnimationControllerComponent
        public int? CurrentSpriteIndex { get; private set; }
        public (float X, float Y) CurrentOffset { get; private set; }
        public (float R, float G, float B, float A) CurrentTint { get; private set; }

        public AnimationController() { }
        public bool IsRunning { get; set; }

        public float CurrentTime { get; private set; }

        public float TotalTime { get; private set; }

        public float Speed { get; set; }
        public string CurrentClip { get; private set; } = string.Empty;

        public AnimationPlaybackState PlaybackState { get; } = new AnimationPlaybackState();

        public AnimationParameterStore Parameters { get; } = new AnimationParameterStore();

        private AnimationTransitionState? _activeTransition;

        //public int CurrentSpriteIndex { get; set; }
        //public (float R, float G, float B, float A) CurrentTint { get; internal set; }

        //public struct CurrentOffset { public float X; public float Y; }
        // ------------------------------------------------------------------------------------------------
        // PLAY
        // ------------------------------------------------------------------------------------------------

        public void Play(string clipName)
        {
            if (string.IsNullOrEmpty(clipName))
                return;

            // If already playing, ignore
            if (clipName == CurrentClip)
                return;

            // Start new clip immediately (no transition)
            CurrentClip = clipName;
            _activeTransition = null;
        }

        // ------------------------------------------------------------------------------------------------
        // STOP
        // ------------------------------------------------------------------------------------------------

        public void Stop()
        {
            CurrentClip = string.Empty;
            _activeTransition = null;
        }

        // ------------------------------------------------------------------------------------------------
        // TRANSITION
        // ------------------------------------------------------------------------------------------------

        public void TransitionTo(string targetClip, float duration)
        {
            if (string.IsNullOrEmpty(targetClip))
                return;

            if (targetClip == CurrentClip)
                return;

            _activeTransition = new AnimationTransitionState(CurrentClip, targetClip, duration);
        }

        // ------------------------------------------------------------------------------------------------
        // UPDATE
        // ------------------------------------------------------------------------------------------------

        public void Update(float deltaTime)
        {
            if (_activeTransition == null)
                return;

            _activeTransition.Update(deltaTime);

            if (_activeTransition.IsComplete)
            {
                CurrentClip = _activeTransition.TargetClip;
                _activeTransition = null;
            }
        }
    }
}
