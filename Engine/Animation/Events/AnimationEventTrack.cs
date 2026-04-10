#nullable enable

using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Animation.Events
{
    /// <summary>
    /// P11-19-02: Event track that stores ordered animation events and provides deterministic lookup and evaluation.
    /// Manages a collection of animation events with temporal ordering and efficient evaluation.
    /// </summary>
    public class AnimationEventTrack
    {
        public string TrackId { get; }
        public string TrackName { get; }
        public string ClipId { get; }
        public float ClipDuration { get; }
        public bool IsEnabled { get; set; } = true;
        public bool IsLooping { get; set; } = false;

        private readonly List<AnimationEvent> _events = new();
        public IReadOnlyList<AnimationEvent> Events => _events.AsReadOnly();

        public float CurrentTime { get; private set; }
        private float _previousTime;
        public int LoopCount { get; private set; }

        public AnimationEventTrack(string trackId, string trackName, string clipId, float clipDuration)
        {
            if (string.IsNullOrEmpty(trackId))
                throw new ArgumentException("Track ID cannot be null or empty", nameof(trackId));
            if (string.IsNullOrEmpty(trackName))
                throw new ArgumentException("Track name cannot be null or empty", nameof(trackName));
            if (string.IsNullOrEmpty(clipId))
                throw new ArgumentException("Clip ID cannot be null or empty", nameof(clipId));
            if (clipDuration <= 0f)
                throw new ArgumentException("Clip duration must be positive", nameof(clipDuration));

            TrackId = trackId;
            TrackName = trackName;
            ClipId = clipId;
            ClipDuration = clipDuration;

            ModernLoggingSystem.Log("DEBUG", $"AnimationEventTrack: Created track '{TrackName}' ({TrackId}) for clip '{ClipId}' ({ClipDuration:F3}s)");
        }

        public bool AddEvent(AnimationEvent animationEvent)
        {
            if (animationEvent == null)
            {
                ModernLoggingSystem.Log("ERROR", "AnimationEventTrack: Cannot add null animation event");
                return false;
            }

            if (animationEvent.Timestamp > ClipDuration)
            {
                ModernLoggingSystem.Log("WARNING", $"AnimationEventTrack: Event '{animationEvent.EventName}' timestamp ({animationEvent.Timestamp:F3}s) exceeds clip duration ({ClipDuration:F3}s)");
                return false;
            }

            if (_events.Any(e => e.EventId == animationEvent.EventId))
            {
                ModernLoggingSystem.Log("ERROR", $"AnimationEventTrack: Event ID '{animationEvent.EventId}' already exists in track '{TrackId}'");
                return false;
            }

            _events.Add(animationEvent);
            _events.Sort((a, b) => a.Timestamp.CompareTo(b.Timestamp));

            ModernLoggingSystem.Log("DEBUG", $"AnimationEventTrack: Added event '{animationEvent.EventName}' ({animationEvent.EventId}) at {animationEvent.Timestamp:F3}s to track '{TrackId}'");
            return true;
        }

        public bool RemoveEvent(string eventId)
        {
            if (string.IsNullOrEmpty(eventId))
            {
                ModernLoggingSystem.Log("ERROR", "AnimationEventTrack: Cannot remove event with null or empty ID");
                return false;
            }

            var eventToRemove = _events.FirstOrDefault(e => e.EventId == eventId);
            if (eventToRemove == null)
            {
                ModernLoggingSystem.Log("WARNING", $"AnimationEventTrack: Event ID '{eventId}' not found in track '{TrackId}'");
                return false;
            }

            _events.Remove(eventToRemove);
            ModernLoggingSystem.Log("DEBUG", $"AnimationEventTrack: Removed event '{eventToRemove.EventName}' ({eventId}) from track '{TrackId}'");
            return true;
        }

        public AnimationEvent GetEvent(string eventId) =>
            string.IsNullOrEmpty(eventId) ? null : _events.FirstOrDefault(e => e.EventId == eventId);

        public IReadOnlyList<AnimationEvent> GetEventsAtTime(float time)
        {
            if (!IsEnabled)
                return Array.Empty<AnimationEvent>();

            var adjustedTime = IsLooping ? time % ClipDuration : time;
            return _events.Where(e => e.IsEnabled && !e.IsTriggered && System.MathF.Abs(adjustedTime - e.Timestamp) < 0.001f).ToList();
        }

        public IReadOnlyList<AnimationEvent> GetEventsInRange(float startTime, float endTime)
        {
            if (!IsEnabled || startTime >= endTime)
                return Array.Empty<AnimationEvent>();

            return _events.Where(e => e.IsEnabled && e.Timestamp >= startTime && e.Timestamp <= endTime).ToList();
        }

        public IReadOnlyList<AnimationEvent> Update(float deltaTime, float absoluteTime)
        {
            if (!IsEnabled)
                return Array.Empty<AnimationEvent>();

            _previousTime = CurrentTime;
            CurrentTime = absoluteTime;

            if (IsLooping && ClipDuration > 0f)
            {
                var newLoopCount = (int)(absoluteTime / ClipDuration);
                if (newLoopCount != LoopCount)
                {
                    ResetEvents();
                    LoopCount = newLoopCount;
                    ModernLoggingSystem.Log("DEBUG", $"AnimationEventTrack: Loop #{LoopCount} for track '{TrackId}'");
                }
            }

            var eventsToTrigger = GetEventsAtTime(absoluteTime);
            foreach (var animationEvent in eventsToTrigger)
            {
                animationEvent.Trigger();
            }

            if (eventsToTrigger.Count > 0)
            {
                ModernLoggingSystem.Log("DEBUG", $"AnimationEventTrack: Triggered {eventsToTrigger.Count} event(s) at {absoluteTime:F3}s in track '{TrackId}'");
            }

            return eventsToTrigger;
        }

        public void ResetEvents()
        {
            foreach (var animationEvent in _events)
            {
                animationEvent.Reset();
            }

            ModernLoggingSystem.Log("DEBUG", $"AnimationEventTrack: Reset all {Events.Count} events in track '{TrackId}'");
        }

        public void Reset()
        {
            ResetEvents();
            CurrentTime = 0f;
            _previousTime = 0f;
            LoopCount = 0;

            ModernLoggingSystem.Log("DEBUG", $"AnimationEventTrack: Reset track '{TrackId}' to initial state");
        }

        public AnimationEventTrackValidationResult Validate()
        {
            var errors = new List<string>();
            var warnings = new List<string>();

            if (string.IsNullOrEmpty(TrackId))
                errors.Add("Track ID cannot be null or empty");
            if (string.IsNullOrEmpty(TrackName))
                errors.Add("Track name cannot be null or empty");
            if (string.IsNullOrEmpty(ClipId))
                errors.Add("Clip ID cannot be null or empty");
            if (ClipDuration <= 0f)
                errors.Add("Clip duration must be positive");

            var eventIds = new HashSet<string>();
            var lastTimestamp = 0f;

            foreach (var animationEvent in _events)
            {
                if (!eventIds.Add(animationEvent.EventId))
                    errors.Add($"Duplicate event ID '{animationEvent.EventId}' found");

                if (animationEvent.Timestamp < lastTimestamp)
                    errors.Add($"Event '{animationEvent.EventName}' timestamp ({animationEvent.Timestamp:F3}s) is out of order");

                if (animationEvent.Timestamp < 0f)
                    errors.Add($"Event '{animationEvent.EventName}' timestamp ({animationEvent.Timestamp:F3}s) is negative");

                if (animationEvent.Timestamp > ClipDuration)
                    warnings.Add($"Event '{animationEvent.EventName}' timestamp ({animationEvent.Timestamp:F3}s) exceeds clip duration ({ClipDuration:F3}s)");

                var eventValidation = animationEvent.Validate();
                errors.AddRange(eventValidation.Errors);
                warnings.AddRange(eventValidation.Warnings);

                lastTimestamp = animationEvent.Timestamp;
            }

            return new AnimationEventTrackValidationResult(errors.Count == 0, errors.ToArray(), warnings.ToArray());
        }

        public string GetDebugInfo()
        {
            var info = $"AnimationEventTrack: {TrackName} (ID: {TrackId})";
            info += $"\n  Clip ID: {ClipId}";
            info += $"\n  Clip Duration: {ClipDuration:F3}s";
            info += $"\n  Enabled: {IsEnabled}";
            info += $"\n  Looping: {IsLooping}";
            info += $"\n  Current Time: {CurrentTime:F3}s";
            info += $"\n  Loop Count: {LoopCount}";
            info += $"\n  Event Count: {_events.Count}";

            if (_events.Count > 0)
            {
                info += "\n  Events:";
                foreach (var animationEvent in _events)
                {
                    info += $"\n    {animationEvent.EventName} ({animationEvent.EventId}) @ {animationEvent.Timestamp:F3}s - {(animationEvent.IsTriggered ? "Triggered" : "Pending")}";
                }
            }

            return info;
        }

        public AnimationEventTrackStatistics GetStatistics() =>
            new()
            {
                TrackId = TrackId,
                TrackName = TrackName,
                ClipId = ClipId,
                ClipDuration = ClipDuration,
                EventCount = _events.Count,
                TriggeredEventCount = _events.Count(e => e.IsTriggered),
                EnabledEventCount = _events.Count(e => e.IsEnabled),
                IsEnabled = IsEnabled,
                IsLooping = IsLooping,
                CurrentTime = CurrentTime,
                LoopCount = LoopCount
            };

        public AnimationEventTrack Clone()
        {
            var clonedTrack = new AnimationEventTrack(TrackId, TrackName, ClipId, ClipDuration)
            {
                IsEnabled = IsEnabled,
                IsLooping = IsLooping
            };

            foreach (var animationEvent in _events)
            {
                clonedTrack.AddEvent(animationEvent.Clone());
            }

            ModernLoggingSystem.Log("DEBUG", $"AnimationEventTrack: Cloned track '{TrackName}' ({TrackId})");
            return clonedTrack;
        }
    }

    public class AnimationEventTrackValidationResult
    {
        public bool IsValid { get; }
        public IReadOnlyList<string> Errors { get; }
        public IReadOnlyList<string> Warnings { get; }

        public AnimationEventTrackValidationResult(bool isValid, string[]? errors = null, string[]? warnings = null)
        {
            IsValid = isValid;
            Errors = errors ?? Array.Empty<string>();
            Warnings = warnings ?? Array.Empty<string>();
        }
    }

    public class AnimationEventTrackStatistics
    {
        public string TrackId { get; set; }
        public string TrackName { get; set; }
        public string ClipId { get; set; }
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




