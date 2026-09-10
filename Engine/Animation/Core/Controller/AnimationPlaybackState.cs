// =====================================================================================================
//  FILE: AnimationPlaybackState.cs
//  PATH: Engine/Animation/Core/Controller/AnimationPlaybackState.cs
//  SUBSYSTEM: Animation Core Controller
//
//  ROLE:
//      Represents the deterministic playback state of an animation clip at runtime. Provides the
//      Animation subsystem with a stable, queryable surface for tracking playback time, loop status,
//      completion status, and per‑frame state transitions.
//
//  RESPONSIBILITIES:
//      - Maintain current playback time for the active clip.
//      - Track whether the clip is looping, completed, or transitioning.
//      - Provide deterministic state surfaces used by AnimationController and AnimationSystem.
//      - Serve as the authoritative state container for clip playback progression.
//
//  NON-RESPONSIBILITIES:
//      - Performing animation playback logic (handled by AnimationController).
//      - Dispatching events (handled by AnimationEventDispatcher / AnimationSystem).
//      - Managing ECS entities or components.
//      - Owning clip libraries or transition rules.
//
//  ARCHITECTURAL NOTES:
//      - Lives in Animation Core Controller alongside AnimationController, AnimationEventDispatcher,
//        AnimationCrossFadeEngine, and AnimationStateTracker.
//      - Designed for deterministic Option‑B architecture: no side effects, no hidden state.
//      - Can be extended later to support blend‑tree state, layered playback, or multi‑track states.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Animation.Core.Controller
{
    internal sealed class AnimationPlaybackState
    {
        public static bool RequestBind { get; internal set; }
        public static string RequestedAssetKey { get; internal set; }
        public static AnimationEnums.DeathType RequestedVisualType { get; internal set; }

        /// <summary>
        /// Current playback time (seconds) within the active clip.
        /// </summary>
        public float Time { get; private set; }

        /// <summary>
        /// True if the clip is configured to loop.
        /// </summary>
        public bool IsLooping { get; private set; }

        /// <summary>
        /// True if the clip has reached its duration and is considered completed.
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// Duration of the active clip.
        /// </summary>
        public float Duration { get; private set; }
        public (float, float) TransformOffset { get; internal set; }

        /// <summary>
        /// Resets playback state for a new clip.
        /// </summary>
        public void Reset(float duration, bool isLooping)
        {
            Duration = duration;
            IsLooping = isLooping;
            Time = 0f;
            IsCompleted = false;
        }

        /// <summary>
        /// Advances playback deterministically.
        /// </summary>
        public void Update(float deltaTime)
        {
            if (IsCompleted)
                return;

            Time += deltaTime;

            if (Time >= Duration)
            {
                if (IsLooping)
                {
                    Time %= Duration;
                }
                else
                {
                    Time = Duration;
                    IsCompleted = true;
                }
            }
        }
    }
}
