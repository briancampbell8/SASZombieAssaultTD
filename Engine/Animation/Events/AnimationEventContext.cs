// ====================================================================================================
//  FILE: AnimationEventContext.cs
//  PATH: ./Engine/Animation/Events/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the AnimationEventContext module.
//
//  RESPONSIBILITIES:
//      - Provide GetDebugInfo() behavior for the Core subsystem.
//      - Provide Validate() behavior for the Core subsystem.
//      - Provide Clone() behavior for the Core subsystem.
//      - Provide AddError() behavior for the Core subsystem.
//      - Provide AddWarning() behavior for the Core subsystem.
//      - Provide GetSummary() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    AnimationEventContext.cs
Purpose: P11-19-08 - Context information for animation event dispatch.
Provides deterministic context for event evaluation and dispatch.
*/

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Animation.Core
{
    /// <summary>
    /// P11-19-08: Context information for animation event evaluation. Provides deterministic context for event
    /// dispatching and diagnostics.
    /// </summary>
    public class AnimationEventContext
    {
        /// <summary>
        /// P11-19-08: ECSEntityCore identifier for the event. Deterministic ECSEntityCore tracking for event context.
        /// </summary>
        public uint EntityId { get; set; }

        /// <summary>
        /// P11-19-08: Animation clip identifier being evaluated. Deterministic clip tracking for event context.
        /// </summary>
        public string ClipId { get; set; } = string.Empty;

        /// <summary>
        /// P11-19-08: Event track identifier being evaluated. Deterministic track tracking for event context.
        /// </summary>
        public string TrackId { get; set; } = string.Empty;

        /// <summary>
        /// P11-19-08: Current playback time of the animation. Deterministic timing for event evaluation.
        /// </summary>
        public float PlaybackTime { get; set; }

        /// <summary>
        /// P11-19-08: Delta time since last update. Deterministic timing for event evaluation.
        /// </summary>
        public float DeltaTime { get; set; }

        /// <summary>
        /// P11-19-08: Whether the animation is currently looping. Deterministic loop state for event context.
        /// </summary>
        public bool IsLooping { get; set; }

        /// <summary>
        /// P11-19-08: Number of times the animation has looped. Deterministic loop counting for event context.
        /// </summary>
        public int LoopCount { get; set; }

        /// <summary>
        /// P11-19-08: Additional context data for event evaluation. Deterministic additional data storage for
        /// extensibility.
        /// </summary>
        public Dictionary<string, object> AdditionalData { get; set; } = new();

        /// <summary>
        /// P11-19-08: Constructor for animation event context. Initializes context with required fields.
        /// </summary>
        /// <param name="ECSEntityCoreId">Entity identifier</param>
        /// <param name="clipId">Animation clip identifier</param>
        /// <param name="trackId">Event track identifier</param>
        /// <param name="playbackTime">Current playback time</param>
        /// <param name="deltaTime">Delta time since last update</param>
        /// <param name="isLooping">Whether animation is looping</param>
        /// <param name="loopCount">Number of loops completed</param>
        public AnimationEventContext(uint ECSEntityCoreId, string clipId, string trackId,
            float playbackTime, float deltaTime, bool isLooping, int loopCount)
        {
            EntityId = ECSEntityCoreId;
            ClipId = clipId ?? string.Empty;
            TrackId = trackId ?? string.Empty;
            PlaybackTime = playbackTime;
            DeltaTime = deltaTime;
            IsLooping = isLooping;
            LoopCount = loopCount;
        }

        /// <summary>
        /// P11-19-08: Gets debug information for the context. Provides deterministic debug output for diagnostics.
        /// </summary>
        /// <returns>Debug information string</returns>
        public string GetDebugInfo()
        {
            var info = $"AnimationEventContext: Entity={EntityId}, Clip={ClipId}, Track={TrackId}, ";
            info += $"Time={PlaybackTime:F3}, Delta={DeltaTime:F3}, Looping={IsLooping}, Loops={LoopCount}";

            if (AdditionalData.Count > 0)
            {
                info += ", Data=[" + string.Join(", ", AdditionalData.Select(kvp => $"{kvp.Key}={kvp.Value}")) + "]";
            }

            return info;
        }

        /// <summary>
        /// P11-19-08: Validates the context for consistency. Provides deterministic validation for event context.
        /// </summary>
        /// <returns>Validation result with warnings and errors</returns>
        public AnimationEventContextValidationResult Validate()
        {
            var result = new AnimationEventContextValidationResult();

            //Validate ECSEntityCore ID
            if (EntityId == 0)
                result.AddWarning("Entity ID is 0, may indicate uninitialized context");

            //Validate clip ID
            if (string.IsNullOrEmpty(ClipId))
                result.AddError("Clip ID is null or empty");

            //Validate track ID
            if (string.IsNullOrEmpty(TrackId))
                result.AddError("Track ID is null or empty");

            //Validate timing
            if (PlaybackTime < 0f)
                result.AddError($"Playback time is negative: {PlaybackTime}");

            if (DeltaTime < 0f)
                result.AddError($"Delta time is negative: {DeltaTime}");

            //Validate loop count
            if (LoopCount < 0)
                result.AddError($"Loop count is negative: {LoopCount}");

            //Validate additional data
            foreach (var kvp in AdditionalData)
            {
                if (string.IsNullOrEmpty(kvp.Key))
                    result.AddWarning("Additional data contains null or empty key");

                if (kvp.Value == null)
                    result.AddWarning($"Additional data value for key '{kvp.Key}' is null");
            }

            return result;
        }

        /// <summary>
        /// P11-19-08: Creates a copy of the context. Provides deterministic cloning for event context.
        /// </summary>
        /// <returns>Cloned context</returns>
        public AnimationEventContext Clone()
        {
            var clone = new AnimationEventContext(EntityId, ClipId, TrackId,
                PlaybackTime, DeltaTime, IsLooping, LoopCount)
            {
                AdditionalData = new Dictionary<string, object>(AdditionalData)
            };

            return clone;
        }
    }

    /// <summary>
    /// P11-19-08: Validation result for animation event context. Provides deterministic validation output for event
    /// context.
    /// </summary>
    public class AnimationEventContextValidationResult
    {
        /// <summary>
        /// P11-19-08: List of validation errors. Deterministic error storage for validation results.
        /// </summary>
        public List<string> Errors { get; } = new();

        /// <summary>
        /// P11-19-08: List of validation warnings. Deterministic warning storage for validation results.
        /// </summary>
        public List<string> Warnings { get; } = new();

        /// <summary>
        /// P11-19-08: Whether the validation passed without errors. Deterministic validation status.
        /// </summary>
        public bool IsValid => Errors.Count == 0;

        /// <summary>
        /// P11-19-08: Adds an error to the validation result. Deterministic error addition for validation.
        /// </summary>
        /// <param name="error">Error message to add</param>
        public void AddError(string error)
        {
            if (!string.IsNullOrEmpty(error))
                Errors.Add(error);
        }

        /// <summary>
        /// P11-19-08: Adds a warning to the validation result. Deterministic warning addition for validation.
        /// </summary>
        /// <param name="warning">Warning message to add</param>
        public void AddWarning(string warning)
        {
            if (!string.IsNullOrEmpty(warning))
                Warnings.Add(warning);
        }

        /// <summary>
        /// P11-19-08: Gets formatted validation summary. Provides deterministic output for validation results.
        /// </summary>
        /// <returns>Formatted validation summary</returns>
        public string GetSummary()
        {
            var summary = $"AnimationEventContext Validation: {(IsValid ? "PASSED" : "FAILED")}";

            if (Errors.Count > 0)
            {
                summary += $"\n  Errors ({Errors.Count}):";
                summary += string.Join("\n    - ", Errors);
            }

            if (Warnings.Count > 0)
            {
                summary += $"\n  Warnings ({Warnings.Count}):";
                summary += string.Join("\n    - ", Warnings);
            }

            return summary;
        }
    }
}
