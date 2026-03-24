using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SASZombieAssaultTD.Engine.Utility;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Animation.Events
{
    /// <summary>
    /// P11-19-01: Core animation event class with explicit fields for event name, timestamp, parameters, and deterministic validation.
    /// Represents a single animation event that can be triggered at a specific time during animation playback.
    /// </summary>
    public class AnimationEvent
    {
        /// <summary>
        /// Unique identifier for this animation event.
        /// </summary>
        public string EventId { get; }
        
        /// <summary>
        /// Human-readable name for this animation event.
        /// </summary>
        public string EventName { get; }
        
        /// <summary>
        /// Timestamp in seconds when this event should trigger.
        /// Deterministic timing relative to animation start.
        /// </summary>
        public float Timestamp { get; }
        
        /// <summary>
        /// Parameters associated with this animation event.
        /// Deterministic parameter storage for event data.
        /// </summary>
        public IReadOnlyDictionary<string, object> Parameters { get; }
        
        /// <summary>
        /// Whether this event has been triggered.
        /// Deterministic state tracking for event execution.
        /// </summary>
        public bool IsTriggered { get; private set; }
        
        /// <summary>
        /// Whether this event is enabled for triggering.
        /// Deterministic control for event activation.
        /// </summary>
        public bool IsEnabled { get; set; }
        
        /// <summary>
        /// Number of times this event has been triggered.
        /// Deterministic counter for event execution tracking.
        /// </summary>
        public int TriggerCount { get; private set; }
        
        /// <summary>
        /// Initializes a new animation event with explicit validation.
        /// </summary>
        /// <param name="eventId">Unique event identifier</param>
        /// <param name="eventName">Human-readable event name</param>
        /// <param name="timestamp">Timestamp in seconds</param>
        /// <param name="parameters">Event parameters (optional)</param>
        public AnimationEvent(string eventId, string eventName, float timestamp, Dictionary<string, object>? parameters = null)
        {
            // Validate inputs
            if (string.IsNullOrEmpty(eventId))
            throw new ArgumentException("Event ID cannot be null or empty", nameof(eventId));
            
            if (string.IsNullOrEmpty(eventName))
            throw new ArgumentException("Event name cannot be null or empty", nameof(eventName));
            
            if (timestamp < 0f)
            throw new ArgumentException("Timestamp cannot be negative", nameof(timestamp));

            // Initialize fields
            Parameters = parameters ?? new Dictionary<string, object>();
            EventId = eventId;
            EventName = eventName;
            Timestamp = timestamp;
            IsTriggered = false;
            IsEnabled = true;
            TriggerCount = 0;
            
            // Validate parameters
            ValidateParameters();
        }
        
        /// <summary>
        /// Triggers the animation event with deterministic behavior.
        /// Marks the event as triggered and increments the trigger count.
        /// </summary>
        /// <returns>True if event was successfully triggered, false if already triggered or disabled</returns>
        public bool Trigger()
        {
            if (!IsEnabled)
            {
                ModernLoggingSystem.Log("DEBUG", $"AnimationEvent: Event '{EventName}' ({EventId}) is disabled, not triggering");
                return false;
            }
            
            if (IsTriggered)
            {
                ModernLoggingSystem.Log("DEBUG", $"AnimationEvent: Event '{EventName}' ({EventId}) already triggered at {Timestamp:F3}s");
                return false;
            }
            
            IsTriggered = true;
            TriggerCount++;
            
            ModernLoggingSystem.Log("DEBUG", $"AnimationEvent: Triggered event '{EventName}' ({EventId}) at {Timestamp:F3}s (trigger #{TriggerCount})");
            return true;
        }
        
        /// <summary>
        /// Resets the animation event to its initial state.
        /// Allows the event to be triggered again.
        /// </summary>
        public void Reset()
        {
            IsTriggered = false;
            ModernLoggingSystem.Log("DEBUG", $"AnimationEvent: Reset event '{EventName}' ({EventId})");
        }
        
        /// <summary>
        /// Gets a parameter value by key with type safety.
        /// </summary>
        /// <typeparam name="T">Parameter type</typeparam>
        /// <param name="key">Parameter key</param>
        /// <param name="defaultValue">Default value if parameter not found</param>
        /// <returns>Parameter value or default</returns>
        public T GetParameter<T>(string key, T defaultValue = default(T))
        {
            if (string.IsNullOrEmpty(key))
            return defaultValue;
            
            if (Parameters.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;
            
            return defaultValue;
        }
        
        /// <summary>
        /// Checks if a parameter exists.
        /// </summary>
        /// <param name="key">Parameter key to check</param>
        /// <returns>True if parameter exists, false otherwise</returns>
        public bool HasParameter(string key)
        {
            return !string.IsNullOrEmpty(key) && Parameters.ContainsKey(key);
        }
        
        /// <summary>
        /// Gets all parameter keys.
        /// </summary>
        /// <returns>Array of parameter keys</returns>
        public string[] GetParameterKeys()
        {
            return Parameters.Keys.ToArray();
        }
        
        /// <summary>
        /// Validates the animation event configuration.
        /// Performs deterministic validation of event properties.
        /// </summary>
        /// <returns>Validation result</returns>
        public AnimationEventValidationResult Validate()
        {
            var errors = new List<string>();
            var warnings = new List<string>();
            
            // Validate event ID
            if (string.IsNullOrEmpty(EventId))
            {
                errors.Add("Event ID cannot be null or empty");
            }
            
            // Validate event name
            if (string.IsNullOrEmpty(EventName))
            {
                errors.Add("Event name cannot be null or empty");
            }
            
            // Validate timestamp
            if (Timestamp < 0f)
            {
                errors.Add("Timestamp cannot be negative");
            }
            
            // Validate parameters
            ValidateParameters(errors, warnings);
            
            return new AnimationEventValidationResult(errors.Count == 0, errors.ToArray(), warnings.ToArray());
        }
        
        /// <summary>
        /// Gets debug information about this animation event.
        /// Provides deterministic debugging information.
        /// </summary>
        /// <returns>Debug information string</returns>
        public string GetDebugInfo()
        {
            var info = $"AnimationEvent: {EventName} (ID: {EventId})";
            info += $"\n  Timestamp: {Timestamp:F3}s";
            info += $"\n  Enabled: {IsEnabled}";
            info += $"\n  Triggered: {IsTriggered}";
            info += $"\n  Trigger Count: {TriggerCount}";
            info += $"\n  Parameters: {Parameters.Count}";
            
            if (Parameters.Count > 0)
            {
                info += "\n  Parameter Details:";
                foreach (var kvp in Parameters)
                {
                    info += $"\n    {kvp.Key}: {kvp.Value} ({kvp.Value?.GetType().Name ?? "null"})";
                }
            }
            
            return info;
        }
        
        /// <summary>
        /// Validates event parameters with deterministic checks.
        /// </summary>
        private void ValidateParameters()
        {
            var errors = new List<string>();
            var warnings = new List<string>();
            ValidateParameters(errors, warnings);
            
            if (errors.Count > 0)
            {
                ModernLoggingSystem.Log("ERROR", $"AnimationEvent: Parameter validation failed for event '{EventName}': {string.Join(", ", errors)}");
            }
            
            if (warnings.Count > 0)
            {
                ModernLoggingSystem.Log("WARNING", $"AnimationEvent: Parameter validation warnings for event '{EventName}': {string.Join(", ", warnings)}");
            }
        }
        
        /// <summary>
        /// Internal parameter validation method.
        /// </summary>
        private void ValidateParameters(List<string> errors, List<string> warnings)
        {
            foreach (var kvp in Parameters)
            {
                // Check for null keys
                if (string.IsNullOrEmpty(kvp.Key))
                {
                    errors.Add("Parameter key cannot be null or empty");
                    continue;
                }
                
                // Check for null values
                if (kvp.Value == null)
                {
                    warnings.Add($"Parameter '{kvp.Key}' has null value");
                }
                
                // Check for supported parameter types
                var valueType = kvp.Value?.GetType();
                if (valueType != null && !IsSupportedParameterType(valueType))
                {
                    warnings.Add($"Parameter '{kvp.Key}' has unsupported type '{valueType.Name}'. Supported types: string, int, float, bool");
                }
            }
        }
        
        /// <summary>
        /// Checks if a parameter type is supported.
        /// </summary>
        private static bool IsSupportedParameterType(Type type)
        {
            return type == typeof(string) ||
            type == typeof(int) ||
            type == typeof(float) ||
            type == typeof(bool) ||
            type == typeof(double) ||
            type == typeof(long) ||
            type == typeof(short) ||
            type == typeof(byte) ||
            type == typeof(decimal);
        }
        
        /// <summary>
        /// Creates a copy of this animation event.
        /// Provides deterministic event cloning.
        /// </summary>
        /// <returns>New animation event instance with same properties</returns>
        public AnimationEvent Clone()
        {
            var clonedParameters = new Dictionary<string, object>(Parameters);
            var clonedEvent = new AnimationEvent(EventId, EventName, Timestamp, clonedParameters);
            clonedEvent.IsEnabled = IsEnabled;
            
            ModernLoggingSystem.Log("DEBUG", $"AnimationEvent: Cloned event '{EventName}' ({EventId})");
            return clonedEvent;
        }
        
        /// <summary>
        /// Determines if this animation event equals another.
        /// Deterministic equality comparison based on event ID.
        /// </summary>
        /// <param name="other">Other animation event</param>
        /// <returns>True if events are equal, false otherwise</returns>
        public override bool Equals(object obj)
        {
            return obj is AnimationEvent other && EventId == other.EventId;
        }
        
        /// <summary>
        /// Gets hash code for this animation event.
        /// Deterministic hash code based on event ID.
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return EventId?.GetHashCode() ?? 0;
        }
        
        /// <summary>
        /// Gets string representation of this animation event.
        /// Deterministic string formatting.
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            return $"AnimationEvent[{EventId}]: {EventName} @ {Timestamp:F3}s";
        }
    }
    
    /// <summary>
    /// P11-19-01: Validation result for animation events.
    /// Deterministic validation result structure.
    /// </summary>
    public class AnimationEventValidationResult
    {
        /// <summary>
        /// Whether the validation passed.
        /// </summary>
        public bool IsValid { get; }
        
        /// <summary>
        /// List of validation errors.
        /// </summary>
        public IReadOnlyList<string> Errors { get; }
        
        /// <summary>
        /// List of validation warnings.
        /// </summary>
        public IReadOnlyList<string> Warnings { get; }
        
        /// <summary>
        /// Initializes a new validation result.
        /// </summary>
        /// <param name="isValid">Whether validation passed</param>
        /// <param name="errors">List of errors</param>
        /// <param name="warnings">List of warnings</param>
        public AnimationEventValidationResult(bool isValid, string[] errors = null, string[] warnings = null)
        {
            IsValid = isValid;
            Errors = errors ?? Array.Empty<string>();
            Warnings = warnings ?? Array.Empty<string>();
        }
    }
}




