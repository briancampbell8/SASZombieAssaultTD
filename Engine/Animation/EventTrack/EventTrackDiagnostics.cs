// ====================================================================================================
//  FILE: EventTrackDiagnostics.cs
//  PATH: Engine/Animation/EventTrack/EventTrackDiagnostics.cs
//  SUBSYSTEM: Animation EventTrack
//
//  ROLE:
//      Diagnostics and statistics provider for AnimationEventTrack.
//
//  RESPONSIBILITIES:
//      - Provide GetDebugInfo() behavior for the Core subsystem.
//      - Provide GetStatistics() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Event triggering or evaluation (handled by AnimationEventTrackEvaluator).
//      - Validation (handled by AnimationEventTrackValidator).
//      - Cloning behavior (handled by AnimationEventTrackCloner).
//
//  NOTES:
//      Produces human-readable debug strings and structured statistics snapshots.
// ====================================================================================================

using System.Linq;
using System.Text;
using SASZombieAssaultTD.Engine.Animation.Core;

namespace SASZombieAssaultTD.Engine.Animation.EventTrack
{
    public static class EventTrackDiagnostics
    {
        public static string GetDebugInfo(AnimationEventTrack track)
        {
            var info = new StringBuilder();

            info.AppendLine($"AnimationEventTrack: {track.TrackName} (ID: {track.TrackId})");
            info.AppendLine($"  Clip ID: {track.ClipId}");
            info.AppendLine($"  Clip Duration: {track.ClipDuration:F3}s");
            info.AppendLine($"  Enabled: {track.IsEnabled}");
            info.AppendLine($"  Looping: {track.IsLooping}");
            info.AppendLine($"  Current Time: {track.CurrentTime:F3}s");
            info.AppendLine($"  Loop Count: {track.LoopCount}");
            info.AppendLine($"  Event Count: {track.InternalEvents.Count}");

            if (track.InternalEvents.Count > 0)
            {
                info.AppendLine("  Events:");
                foreach (var animationEvent in track.InternalEvents)
                {
                    info.AppendLine(
                        $"    {animationEvent.EventName} ({animationEvent.EventId}) @ {animationEvent.Timestamp:F3}s - {(animationEvent.IsTriggered ? "Triggered" : "Pending")}");
                }
            }

            return info.ToString();
        }

        public static EventTrackStatistics GetStatistics(AnimationEventTrack track)
        {
            return new EventTrackStatistics
            {
                TrackId = track.TrackId,
                TrackName = track.TrackName,
                ClipId = track.ClipId,
                ClipDuration = track.ClipDuration,
                EventCount = track.InternalEvents.Count,
                TriggeredEventCount = track.InternalEvents.Count(e => e.IsTriggered),
                EnabledEventCount = track.InternalEvents.Count(e => e.IsEnabled),
                IsEnabled = track.IsEnabled,
                IsLooping = track.IsLooping,
                CurrentTime = track.CurrentTime,
                LoopCount = track.LoopCount
            };
        }
    }
}
