// ====================================================================================================
//  FILE: EventTrackValidator.cs
//  PATH: Engine/Animation/EventTrack/EventTrackValidator.cs
//  MODULE: Animation EventTrack
//
//  ROLE:
//      Deterministic validation module for AnimationEventTrack.
//
//  RESPONSIBILITIES:
//      - Provide Validate() behavior for the Core subsystem.
//      - Verify track idECSEntityCore fields (TrackId, TrackName, ClipId).
//      - Validate clip duration constraints.
//      - Detect duplicate event IDs.
//      - Detect out-of-order timestamps.
//      - Detect negative timestamps.
//      - Emit warnings for timestamps beyond clip duration.
//      - Aggregate validation results into AnimationEventTrackValidationResult.
//
//  NON-RESPONSIBILITIES:
//      - Event triggering or evaluation (handled by EventTrackEvaluator).
//      - Diagnostics and statistics (handled by EventTrackDiagnostics).
//      - Cloning behavior (handled by EventTrackCloner).
//
//  NOTES:
//      Produces a pure, deterministic AnimationEventTrackValidationResult summary.
// ====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Animation.Core;

namespace SASZombieAssaultTD.Engine.Animation.EventTrack
{
    public static class EventTrackValidator
    {
        public static EventTrackValidationResult Validate(AnimationEventTrack track)
        {
            var errors = new List<string>();
            var warnings = new List<string>();

            if (string.IsNullOrEmpty(track.TrackId))
                errors.Add("Track ID cannot be null or empty");
            if (string.IsNullOrEmpty(track.TrackName))
                errors.Add("Track name cannot be null or empty");
            if (string.IsNullOrEmpty(track.ClipId))
                errors.Add("Clip ID cannot be null or empty");
            if (track.ClipDuration <= 0f)
                errors.Add("Clip duration must be positive");

            var eventIds = new HashSet<string>();
            float lastTimestamp = 0f;

            foreach (var animationEvent in track.InternalEvents)
            {
                if (!eventIds.Add(animationEvent.EventId))
                    errors.Add($"Duplicate event ID '{animationEvent.EventId}' found");

                if (animationEvent.Timestamp < lastTimestamp)
                    errors.Add($"Event '{animationEvent.EventName}' timestamp ({animationEvent.Timestamp:F3}s) is out of order");

                if (animationEvent.Timestamp < 0f)
                    errors.Add($"Event '{animationEvent.EventName}' timestamp ({animationEvent.Timestamp:F3}s) is negative");

                if (animationEvent.Timestamp > track.ClipDuration)
                    warnings.Add($"Event '{animationEvent.EventName}' timestamp ({animationEvent.Timestamp:F3}s) exceeds clip duration ({track.ClipDuration:F3}s)");

                lastTimestamp = animationEvent.Timestamp;
            }

            return new EventTrackValidationResult(errors.Count == 0, errors.ToArray(), warnings.ToArray());
        }
    }
}
