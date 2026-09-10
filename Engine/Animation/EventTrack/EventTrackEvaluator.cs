// ====================================================================================================
//  FILE: EventTrackEvaluator.cs
//  PATH: Engine/Animation/EventTrack/EventTrackEvaluator.cs
//  SUBSYSTEM: Animation EventTrack
//  ROLE:
//      Deterministic evaluator for AnimationEventTrack.
//      Advances time, handles looping, and triggers events.
//
//  RESPONSIBILITIES:
//      - Provide Update() behavior for the Core subsystem.
//      - Provide GetEventsAtTime() behavior for the Core subsystem.
//      - Provide GetEventsInRange() behavior for the Core subsystem.
//      - Handle loop detection and loop-count updates.
//      - Reset events on loop boundaries.
//
//  NON-RESPONSIBILITIES:
//      - Owning event data (handled by AnimationEventTrack).
//      - Validation of event configuration (handled by AnimationEventTrackValidator).
//      - Diagnostics and statistics (handled by AnimationEventTrackDiagnostics).
//      - Cloning behavior (handled by AnimationEventTrackCloner).
//
//  NOTES:
//      Operates purely on AnimationEventTrack data and AnimationEvent instances.
//      No ECS or rendering responsibilities.
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Linq;
using SASZombieAssaultTD.Engine.Animation.Core;

namespace SASZombieAssaultTD.Engine.Animation.EventTrack
{
    public static class EventTrackEvaluator
    {
        public static IReadOnlyList<AnimationEvent> Update(AnimationEventTrack track, float deltaTime, float absoluteTime)
        {
            if (!track.IsEnabled)
                return Array.Empty<AnimationEvent>();

            track.CurrentTime = absoluteTime;

            if (track.IsLooping && track.ClipDuration > 0f)
            {
                int newLoopCount = (int)(absoluteTime / track.ClipDuration);
                if (newLoopCount != track.LoopCount)
                {
                    ResetEvents(track);
                    track.LoopCount = newLoopCount;
                }
            }

            var eventsToTrigger = GetEventsAtTime(track, absoluteTime);

            foreach (var animationEvent in eventsToTrigger)
            {
                animationEvent.Trigger();
            }

            return eventsToTrigger;
        }

        public static IReadOnlyList<AnimationEvent> GetEventsAtTime(AnimationEventTrack track, float time)
        {
            if (!track.IsEnabled)
                return Array.Empty<AnimationEvent>();

            float adjustedTime = track.IsLooping && track.ClipDuration > 0f
                ? time % track.ClipDuration
                : time;

            return track.InternalEvents
                .Where(e => e.IsEnabled && !e.IsTriggered && MathF.Abs(adjustedTime - e.Timestamp) < 0.001f)
                .ToList();
        }

        public static IReadOnlyList<AnimationEvent> GetEventsInRange(AnimationEventTrack track, float startTime, float endTime)
        {
            if (!track.IsEnabled || startTime >= endTime)
                return Array.Empty<AnimationEvent>();

            return track.InternalEvents
                .Where(e => e.IsEnabled && e.Timestamp >= startTime && e.Timestamp <= endTime)
                .ToList();
        }

        public static void ResetEvents(AnimationEventTrack track)
        {
            foreach (var animationEvent in track.InternalEvents)
            {
                animationEvent.Reset();
            }
        }
    }
}
