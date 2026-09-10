// ====================================================================================================
//  FILE: AnimationEvent.cs
//  PATH: Engine/Animation/Core/AnimationEvent.cs
//  SUBSYSTEM: Animation Core
//
//  ROLE:
//      Represents a deterministic animation event fired at a specific timestamp.
//
//  RESPONSIBILITIES:
//      - Store event idECSEntityCore and timestamp.
//      - Store enabled/triggered state.
//      - Provide Trigger() and Reset() behavior.
//      - Provide Validate() behavior.
//
//  NON-RESPONSIBILITIES:
//      - Event dispatching (AnimationEventDispatcher).
//      - Track evaluation (EventTrackEvaluator).
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation.Core
{
    public class AnimationEvent
    {
        public string EventId { get; }
        public string EventName { get; }
        public float Timestamp { get; }
        public float Payload { get; }

        public bool IsEnabled { get; set; } = true;
        public bool IsPaused { get; set; }
        public bool IsTriggered { get; set; } = false;
        public float Time { get; internal set; }
        public string Validate { get; set; }

        // 1. Add a backing field for the parameters
        private readonly Dictionary<string, object> _parameters;

        // 2. Assign the dictionary in the constructor
        public AnimationEvent(
            string eventId,
            string eventName,
            float timestamp,
            float payload,
            Dictionary<string, object> parameters)
        {
            EventId = eventId ?? throw new ArgumentNullException(nameof(eventId));
            EventName = eventName ?? throw new ArgumentNullException(nameof(eventName));
            Timestamp = timestamp;
            Payload = payload;
            _parameters = parameters ?? new Dictionary<string, object>();
        }

        public AnimationEvent(string eventId, string eventName, float timestamp, float payload)
        {
            EventId = eventId;
            EventName = eventName;
            Timestamp = timestamp;
            Payload = payload;
            _parameters = new Dictionary<string, object>(); // Initialize empty
        }

        public AnimationEvent(string eventId, string eventName, float timestamp, Dictionary<string, object> parameters)
        {
            EventId = eventId;
            EventName = eventName;
            Timestamp = timestamp;
            _parameters = parameters;
        }

        // 3. Add the generic GetParameter method you were trying to call
        public T GetParameter<T>(string key)
        {
            if (_parameters != null && _parameters.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return default;
        }
        // ----------------------------------------------------------------------------------------------------
        //  EVENT STATE CONTROL
        // ----------------------------------------------------------------------------------------------------
        public void Trigger()
        {
            IsTriggered = true;
        }

        public void Reset()
        {
            IsTriggered = false;
        }

        // ... rest of your existing methods (Trigger, Reset, Clone, Validate)
    }
}
