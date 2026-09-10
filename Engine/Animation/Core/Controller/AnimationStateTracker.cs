// =====================================================================================================
//  FILE: AnimationStateTracker.cs
//  PATH: Engine/Animation/Core/Controller/AnimationStateTracker.cs
//  SUBSYSTEM: Animation Core Controller
//
//  ROLE:
//      Tracks deterministic animation state transitions for an ECSEntityCore. Provides the Animation
//      subsystem with a stable, queryable surface for determining when clips change, when transitions
//      occur, and when state‑dependent events should be fired.
//
//  RESPONSIBILITIES:
//      - Track the currently active animation clip.
//      - Track the previously active clip for transition detection.
//      - Expose deterministic surfaces for state‑change checks.
//      - Provide AnimationController and AnimationSystem with state‑transition information.
//      - Maintain strict Option‑B determinism: no hidden state, no side effects.
//
//  NON-RESPONSIBILITIES:
//      - Performing animation playback (handled by AnimationController).
//      - Dispatching events (handled by AnimationEventDispatcher / AnimationSystem).
//      - Managing ECS entities or components.
//      - Owning clip libraries or transition rules.
//
//  ARCHITECTURAL NOTES:
//      - Lives in Animation Core Controller alongside AnimationController, AnimationPlaybackState,
//        AnimationCrossFadeEngine, and AnimationEventDispatcher.
//      - Designed for deterministic animation pipelines: predictable, reproducible state changes.
//      - Can be extended later to support layered states, blend‑tree states, or multi‑track states.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Animation.Core.Controller
{
    internal sealed class AnimationStateTracker
    {
        /// <summary>
        /// The name of the currently active animation clip.
        /// </summary>
        public string? CurrentClip { get; private set; }

        /// <summary>
        /// The name of the previously active animation clip.
        /// </summary>
        public string? PreviousClip { get; private set; }

        /// <summary>
        /// True if the active clip changed this frame.
        /// </summary>
        public bool StateChangedThisFrame { get; private set; }

        /// <summary>
        /// Called when AnimationController switches to a new clip.
        /// </summary>
        public void SetActiveClip(string clipName)
        {
            StateChangedThisFrame = clipName != CurrentClip;

            if (StateChangedThisFrame)
                PreviousClip = CurrentClip;

            CurrentClip = clipName;
        }

        /// <summary>
        /// Clears per‑frame state flags. Called once per update by AnimationSystem.
        /// </summary>
        public void ResetFrameFlags()
        {
            StateChangedThisFrame = false;
        }
    }
}
