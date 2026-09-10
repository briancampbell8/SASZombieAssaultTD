// ====================================================================================================
//  FILE: EventTrackCloner.cs.
//  PATH: Engine/Animation/EventTrack/EventTrackCloner.cs
//  SUBSYSTEM: Amination EventTrack
//
//  ROLE:
//      Cloning module for AnimationEventTrack.
//
//  RESPONSIBILITIES:
//      - Provide Clone() behavior for the Core subsystem.
//      - Deep-clone AnimationEventTrack instances.
//      - Clone all contained AnimationEvent instances.
//      - Preserve enabled and looping state.
//
//  NON-RESPONSIBILITIES:
//      - Event triggering or evaluation (handled by AnimationEventTrackEvaluator).
//      - Validation (handled by AnimationEventTrackValidator).
//      - Diagnostics and statistics (handled by AnimationEventTrackDiagnostics).
//
//  NOTES:
//      Produces a structurally identical track with independent event instances.
// ====================================================================================================

using SASZombieAssaultTD.Engine.Animation.Core;

namespace SASZombieAssaultTD.Engine.Animation.EventTrack
{
    public static class EventTrackCloner
    {
        public static AnimationEventTrack Clone(AnimationEventTrack source)
        {
            var clonedTrack = new AnimationEventTrack(
                source.TrackId,
                source.TrackName,
                source.ClipId,
                source.ClipDuration)
            {
                IsEnabled = source.IsEnabled,
                IsLooping = source.IsLooping
            };

            foreach (var animationEvent in source.InternalEvents)
            {
                var clonedEvent = new AnimationEvent(
                    animationEvent.EventId,
                    animationEvent.EventName,
                    animationEvent.Timestamp,
                    animationEvent.Payload)
                {
                    IsEnabled = animationEvent.IsEnabled,
                    IsTriggered = animationEvent.IsTriggered
                };

                clonedTrack.InternalEvents.Add(clonedEvent);
            }

            return clonedTrack;
        }
    }
}
