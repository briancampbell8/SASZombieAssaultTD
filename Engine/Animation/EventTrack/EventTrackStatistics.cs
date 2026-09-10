// ====================================================================================================
//  FILE: EventTrackStatistics.cs
//  PATH: Engine/Animation/EventTrack/EventTrackStatistics.cs
//  SUBSYSTEM: Amination EventTrack
//
//  ROLE:
//      Statistics snapshot data container for AnimationEventTrack.
//
//  RESPONSIBILITIES:
//      - Hold track idECSEntityCore and clip metadata.
//      - Hold event counts and state flags.
//      - Hold current time and loop count.
//
//  NON-RESPONSIBILITIES:
//      - Computing statistics (handled by AnimationEventTrackDiagnostics).
//      - Event triggering or evaluation.
//      - Validation.
// ====================================================================================================

namespace SASZombieAssaultTD.Engine.Animation.Core
{
    public sealed class EventTrackStatistics
    {
        public string TrackId { get; set; } = string.Empty;
        public string TrackName { get; set; } = string.Empty;
        public string ClipId { get; set; } = string.Empty;
        public float ClipDuration { get; set; }

        public int EventCount { get; set; }
        public int TriggeredEventCount { get; set; }
        public int EnabledEventCount { get; set; }

        public bool IsEnabled { get; set; }
        public bool IsLooping { get; set; }

        public float CurrentTime { get; set; }
        public int LoopCount { get; set; }
    }
}
