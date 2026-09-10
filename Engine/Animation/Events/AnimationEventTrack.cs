// ====================================================================================================
//  FILE: AnimationEventTrack.cs
//  PATH: ./Engine/Animation/Events/
//  MODULE: Core
//
//  ROLE:
//      Pure deterministic data container for animation event tracks.
//      Stores ordered animation events and track metadata without performing any logic.
//
//  RESPONSIBILITIES:
//      - Hold ordered AnimationEvent instances.
//      - Expose read-only event list.
//      - Maintain track idECSEntityCore (TrackId, TrackName, ClipId).
//      - Maintain clip timing metadata (ClipDuration).
//      - Maintain enabled/looping state.
//      - Maintain current playback time and loop count.
//      - Provide internal access to the mutable event list for subsystem modules.
//
//  NON-RESPONSIBILITIES:
//      - Event triggering or evaluation (handled by AnimationEventTrackEvaluator).
//      - Event validation (handled by AnimationEventTrackValidator).
//      - Event diagnostics or statistics (handled by AnimationEventTrackDiagnostics).
//      - Event cloning (handled by AnimationEventTrackCloner).
//      - Event range queries or update logic.
//
//  NOTES:
//      This file replaces the legacy monolithic AnimationEventTrack class.
//      All operational logic has been moved into dedicated subsystem modules
//      to comply with Option‑B deterministic architecture.
// ====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Animation.Core.Controller;

namespace SASZombieAssaultTD.Engine.Animation.Core
{
    public sealed class AnimationEventTrack
    {
        // IdECSEntityCore
        public string TrackId { get; }
        public string TrackName { get; }
        public string ClipId { get; }

        // Timing metadata
        public float ClipDuration { get; }

        // State flags
        public bool IsEnabled { get; set; } = true;
        public bool IsLooping { get; set; } = false;

        // Event storage
        private readonly List<AnimationEvent> _events = new();
        public IReadOnlyList<AnimationEvent> Events => _events.AsReadOnly();

        // Playback state (updated externally by evaluator)
        public float CurrentTime { get; internal set; }
        public int LoopCount { get; internal set; }

        // Internal mutable access for subsystem modules
        internal List<AnimationEvent> InternalEvents => _events;

        public AnimationEventTrack(string trackId, string trackName, string clipId, float clipDuration)
        {
            TrackId = trackId;
            TrackName = trackName;
            ClipId = clipId;
            ClipDuration = clipDuration;
        }
    }
}
