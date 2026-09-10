// =====================================================================================================
//  FILE: AnimationTrack.cs
//  PATH: Engine/Animation/Core/Clips/AnimationTrack.cs
//  SUBSYSTEM: Animation Core Clips
//
//  ROLE:
//      Defines the minimal deterministic lifecycle contract for any engine-hosted program.
//      This interface is implemented by engine hosts (e.g., GameRootMain) to provide a clean,
//      engine-facing API for startup, execution entry, and deterministic shutdown operations.
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface for program orchestration.
//      - Enforce the structural sequencing contract: Initialize → Run Loop Execution → Shutdown.
//      - Serve as the base contract for any future top-level engine-hosted program modules.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deep frame-level update calculation rules or rendering commands directly.
//      - Managing active systems registration pools, engine assets, or game states.
//      - Handling discrete hardware device allocation boundaries.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces the legacy GameRoot partial lifecycle methods.
//      - GameRootMain implements this interface and delegates to its subsystems:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Animation.Core.Clips
{
    public class AnimationTrack
    {
        public AnimationEnums.AnimationTrackType Type { get; internal set; }
        public object Name { get; internal set; }
        public object Value { get; internal set; }
        public int LoopCount { get; internal set; }
        public bool IsRelative { get; internal set; }
        public bool IsLooping { get; internal set; }
        public bool IsPingPong { get; internal set; }

        public float Position { get; internal set; }

        public float Speed { get; internal set; }

        public float Delay { get; internal set; }

        public float Duration { get; internal set; }

        public float Time { get; internal set; }
        /// <summary>
        /// Tracks
        /// </summary>


        public AnimationTrack(AnimationEnums.AnimationTrackType type, object name, object value)
        {
            Type = type;
            Name = name;
            Value = value;
        }
        public void AddEvent(AnimationEvent animationEvent) { }
        public void RemoveEvent(AnimationEvent animationEvent) { }
        public void ClearEvents() { }
        public void AddTrack(AnimationTrack track) { }
        public void RemoveTrack(AnimationTrack track) { }
        public void GetTrack(AnimationTrack track) { }
        public void ClearTracks() { }
        public void AddClip(AnimationClip animationClip) { }
        public void RemoveClip(AnimationClip animationClip) { }
        public void ClearClips() { }

        public void AddTag(string tag) { }
        public void RemoveTag(string tag) { }
        public void ClearTags() { }

        public struct AnimationEvent
        {
            public float Time { get; internal set; }
            public string Name { get; internal set; }
        }
    }
}
