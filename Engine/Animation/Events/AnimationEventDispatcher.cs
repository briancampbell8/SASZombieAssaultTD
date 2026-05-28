using SASZombieAssaultTD.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Animation.Events
{
    /// <summary>
    /// Statistics for AnimationEventDispatcher.
    /// </summary>
    public class AnimationEventDispatcherStatistics
    {
        public int TotalUpdates { get; set; }
        public int EventsDispatched { get; set; }
        public int ErrorCount { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public float AverageUpdateTime { get; set; }
    }

    /// <summary>
    /// Validation result for AnimationEventDispatcher.
    /// </summary>
    public class AnimationEventDispatcherValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public int ReceiverCount { get; set; }
        public int TrackCount { get; set; }
    }
    /// <summary>
    /// P11-19-04: Dispatcher that evaluates event tracks, determines which events fire, and sends them to registered receivers.
    /// Provides deterministic event dispatching with ordered processing and comprehensive error handling.
    /// </summary>
    public class AnimationEventDispatcher
    {
        private readonly SortedDictionary<int, List<IAnimationEventReceiver>> _receiversByPriority = new();
        private readonly Dictionary<string, AnimationEventTrack> _eventTracksByClip = new();
        private readonly List<AnimationEventTrack> _allEventTracks = new();

        public bool IsEnabled { get; set; } = true;
        public int ReceiverCount => _receiversByPriority.Values.Sum(list => list.Count);
        public int TrackCount => _eventTracksByClip.Count;
        public AnimationEventDispatcherStatistics Statistics { get; private set; } = new();

        public AnimationEventDispatcher()
        {
            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", "AnimationEventDispatcher: Initialized dispatcher");
        }

        public bool RegisterReceiver(IAnimationEventReceiver receiver)
        {
            if (receiver == null)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", "AnimationEventDispatcher: Cannot register null receiver");
                return false;
            }

            var validation = receiver.Validate();
            if (!validation.IsValid)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"AnimationEventDispatcher: Receiver '{receiver.ReceiverName}' validation failed: {string.Join(", ", validation.Errors)}");
                return false;
            }

            if (_receiversByPriority.Values.SelectMany(list => list).Any(r => r.ReceiverId == receiver.ReceiverId))
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"AnimationEventDispatcher: Receiver ID '{receiver.ReceiverId}' already registered");
                return false;
            }

            if (!_receiversByPriority.ContainsKey(receiver.Priority))
            {
                _receiversByPriority[receiver.Priority] = new List<IAnimationEventReceiver>();
            }
            _receiversByPriority[receiver.Priority].Add(receiver);

            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"AnimationEventDispatcher: Registered receiver '{receiver.ReceiverName}' ({receiver.ReceiverId}) with priority {receiver.Priority}");
            return true;
        }

        public bool DeregisterReceiver(string receiverId)
        {
            if (string.IsNullOrEmpty(receiverId))
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", "AnimationEventDispatcher: Cannot deregister receiver with null or empty ID");
                return false;
            }

            foreach (var priorityList in _receiversByPriority.Values)
            {
                var receiverToRemove = priorityList.FirstOrDefault(r => r.ReceiverId == receiverId);
                if (receiverToRemove != null)
                {
                    priorityList.Remove(receiverToRemove);
                    Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"AnimationEventDispatcher: Deregistered receiver '{receiverToRemove.ReceiverName}' ({receiverId})");
                    return true;
                }
            }

            Engine.Diagnostics.DebugLogger.LogDebug("WARNING", $"AnimationEventDispatcher: Receiver ID '{receiverId}' not found for deregistration");
            return false;
        }

        public bool RegisterEventTrack(AnimationEventTrack eventTrack)
        {
            if (eventTrack == null)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", "AnimationEventDispatcher: Cannot register null event track");
                return false;
            }

            var validation = eventTrack.Validate();
            if (!validation.IsValid)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"AnimationEventDispatcher: Event track '{eventTrack.TrackName}' validation failed: {string.Join(", ", validation.Errors)}");
                return false;
            }

            if (_eventTracksByClip.ContainsKey(eventTrack.ClipId))
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"AnimationEventDispatcher: Event track for clip '{eventTrack.ClipId}' already registered");
                return false;
            }

            _eventTracksByClip[eventTrack.ClipId] = eventTrack;
            _allEventTracks.Add(eventTrack);

            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"AnimationEventDispatcher: Registered event track '{eventTrack.TrackName}' ({eventTrack.TrackId}) for clip '{eventTrack.ClipId}'");
            return true;
        }

        public bool DeregisterEventTrack(string clipId)
        {
            if (string.IsNullOrEmpty(clipId))
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", "AnimationEventDispatcher: Cannot deregister event track with null or empty clip ID");
                return false;
            }

            if (_eventTracksByClip.Remove(clipId, out var eventTrack))
            {
                _allEventTracks.Remove(eventTrack);
                Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"AnimationEventDispatcher: Deregistered event track '{eventTrack.TrackName}' ({eventTrack.TrackId}) for clip '{clipId}'");
                return true;
            }

            Engine.Diagnostics.DebugLogger.LogDebug("WARNING", $"AnimationEventDispatcher: Event track for clip '{clipId}' not found for deregistration");
            return false;
        }

        public IReadOnlyList<AnimationEvent> Update(float deltaTime, string currentClipId, float playbackTime, uint entityId = 0)
        {
            if (!IsEnabled)
                return Array.Empty<AnimationEvent>();

            var dispatchedEvents = new List<AnimationEvent>();
            var updateStartTime = DateTime.Now;

            try
            {
                foreach (var eventTrack in _allEventTracks.Where(track => track.IsEnabled && track.ClipId == currentClipId))
                {
                    var triggeredEvents = eventTrack.Update(deltaTime, playbackTime);

                    foreach (var animationEvent in triggeredEvents)
                    {
                        if (DispatchEvent(animationEvent, currentClipId, playbackTime, entityId, deltaTime))
                        {
                            dispatchedEvents.Add(animationEvent);
                        }
                    }
                }

                Statistics.TotalUpdates++;
                Statistics.EventsDispatched += dispatchedEvents.Count;
                Statistics.LastUpdateTime = DateTime.Now;
                Statistics.AverageUpdateTime = (Statistics.AverageUpdateTime * (Statistics.TotalUpdates - 1) +
                    (float)(DateTime.Now - updateStartTime).TotalMilliseconds) / Statistics.TotalUpdates;

                if (dispatchedEvents.Count > 0)
                {
                    Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"AnimationEventDispatcher: Dispatched {dispatchedEvents.Count} events for clip '{currentClipId}' at {playbackTime:F3}s");
                }
            }
            catch (Exception ex)
            {
                Statistics.ErrorCount++;
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"AnimationEventDispatcher: Error during update: {ex.Message}");
            }

            return dispatchedEvents;
        }

        /// <summary>
        /// Process events for the given clip and playback time (alias for Update).
        /// </summary>
        public IReadOnlyList<AnimationEvent> ProcessEvents(float deltaTime, string currentClipId, float playbackTime, uint entityId = 0)
        {
            return Update(deltaTime, currentClipId, playbackTime, entityId);
        }

        public bool DispatchEvent(AnimationEvent animationEvent, string clipId, float playbackTime, uint entityId = 0, float deltaTime = 0f)
        {
            if (!IsEnabled || animationEvent == null)
                return false;

            var dispatchedToAnyReceiver = false;
            var eventTrack = _eventTracksByClip.GetValueOrDefault(clipId);

            var context = new AnimationEventContext(
                entityId,
                clipId,
                eventTrack?.TrackId ?? string.Empty,
                playbackTime,
                deltaTime,
                eventTrack?.IsLooping ?? false,
                eventTrack?.LoopCount ?? 0
            );

            foreach (var receivers in _receiversByPriority.OrderBy(p => p.Key).Select(kvp => kvp.Value))
            {
                foreach (var receiver in receivers.Where(r => r.IsEnabled))
                {
                    try
                    {
                        receiver.OnAnimationEvent(animationEvent, context);
                        dispatchedToAnyReceiver = true;
                    }
                    catch (Exception ex)
                    {
                        Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"AnimationEventDispatcher: Receiver '{receiver.ReceiverId}' failed to handle event '{animationEvent.EventName}': {ex.Message}");
                    }
                }
            }

            if (dispatchedToAnyReceiver)
            {
                Statistics.EventsDispatched++;
            }

            return dispatchedToAnyReceiver;
        }

        public void Reset()
        {
            foreach (var eventTrack in _allEventTracks)
            {
                eventTrack.Reset();
            }

            Statistics = new AnimationEventDispatcherStatistics();
            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", "AnimationEventDispatcher: Reset all event tracks and statistics");
        }

        public IReadOnlyList<IAnimationEventReceiver> GetRegisteredReceivers() =>
            _receiversByPriority.Values.SelectMany(list => list).ToList().AsReadOnly();

        public IReadOnlyList<AnimationEventTrack> GetRegisteredEventTracks() => _allEventTracks.AsReadOnly();

        public AnimationEventTrack GetEventTrack(string clipId) => _eventTracksByClip.GetValueOrDefault(clipId);

        public AnimationEventDispatcherValidationResult Validate()
        {
            var errors = new List<string>();
            var warnings = new List<string>();

            var receiverIds = new HashSet<string>();
            foreach (var receiver in _receiversByPriority.Values.SelectMany(list => list))
            {
                if (!receiverIds.Add(receiver.ReceiverId))
                {
                    errors.Add($"Duplicate receiver ID '{receiver.ReceiverId}' found");
                }
            }

            var clipIds = new HashSet<string>();
            foreach (var track in _allEventTracks)
            {
                if (!clipIds.Add(track.ClipId))
                {
                    errors.Add($"Duplicate event track for clip '{track.ClipId}' found");
                }

                var trackValidation = track.Validate();
                if (!trackValidation.IsValid)
                {
                    errors.AddRange(trackValidation.Errors);
                }
                warnings.AddRange(trackValidation.Warnings);
            }

            return new AnimationEventDispatcherValidationResult
            {
                IsValid = !errors.Any(),
                Errors = errors,
                Warnings = warnings,
                ReceiverCount = ReceiverCount,
                TrackCount = TrackCount
            };
        }
    }
}
