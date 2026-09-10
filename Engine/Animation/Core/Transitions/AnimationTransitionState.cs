// =====================================================================================================
//  FILE: AnimationTransitionState.cs
//  PATH: Engine/Animation/Core/Transitions/AnimationTransitionState.cs
//  SUBSYSTEM: Animation Core Transitions
//
//  ROLE:
//      Represents the active state of an animation transition, including timing, progress,
//      and blend weights. Used by the transition engine and controller to drive deterministic
//      cross‑fade behavior.
//
//  RESPONSIBILITIES:
//      - Track transition progress over time.
//      - Maintain source and target clip names.
//      - Store and update blend weights via AnimationBlendState.
//      - Provide deterministic update and completion checks.
//
//  NON-RESPONSIBILITIES:
//      - Evaluating transition rules.
//      - Managing animation playback or events.
//      - Handling parameter logic.
//      - Executing blending operations.
//
//  ARCHITECTURAL NOTES:
//      - Lives under Animation/Core/Transitions as a foundational primitive.
//      - Consumed by AnimationController and AnimationCrossFadeEngine.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Animation.Core.State;

namespace SASZombieAssaultTD.Engine.Animation.Core.Transitions
{
    internal sealed class AnimationTransitionState
    {
        public string SourceClip { get; }
        public string TargetClip { get; }

        public float Duration { get; }
        public float Elapsed { get; private set; }

        public AnimationBlendState BlendState { get; } = new();

        public bool IsComplete => Elapsed >= Duration;

        public AnimationTransitionState(string sourceClip, string targetClip, float duration)
        {
            SourceClip = sourceClip;
            TargetClip = targetClip;
            Duration = duration <= 0f ? 0.0001f : duration;

            BlendState.SetWeight(SourceClip, 1f);
            BlendState.SetWeight(TargetClip, 0f);
        }

        /// <summary>
        /// Advances the transition by deltaTime and updates blend weights.
        /// </summary>
        public void Update(float deltaTime)
        {
            if (IsComplete)
                return;

            Elapsed += deltaTime;
            float t = System.Math.Clamp(Elapsed / Duration, 0f, 1f);

            BlendState.SetWeight(SourceClip, 1f - t);
            BlendState.SetWeight(TargetClip, t);
            BlendState.Normalize();
        }

        /// <summary>
        /// Resets the transition to its initial state.
        /// </summary>
        public void Reset()
        {
            Elapsed = 0f;

            BlendState.Clear();
            BlendState.SetWeight(SourceClip, 1f);
            BlendState.SetWeight(TargetClip, 0f);
        }
    }
}
