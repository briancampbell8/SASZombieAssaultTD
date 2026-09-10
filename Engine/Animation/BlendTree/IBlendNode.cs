// ====================================================================================================
//  FILE: IBlendNode.cs
//  PATH: ./Engine/Animation/BlendTree/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the IBlendNode module.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation.BlendTree
{
    /// <summary>
    /// Defines the interface for blend tree nodes with deterministic evaluation.
    /// </summary>
    public interface IBlendNode
    {
        /// <summary>
        /// Unique identifier for this node in the blend tree.
        /// </summary>
        string NodeId { get; }

        /// <summary>
        /// Human-readable name for debugging and diagnostics.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Node weight (read-only). Implementations provide a sensible default or computed weight representing node
        /// contribution to blends.
        /// </summary>
        float Weight { get; }

        /// <summary>
        /// List of parameter names required by this node for evaluation.
        /// </summary>
        IReadOnlyList<string> RequiredParameters { get; }

        /// <summary>
        /// Evaluates the node with the given parameters and returns the animation clip result.
        /// </summary>
        /// <param name="parameters">Parameter set for evaluation.</param>
        /// <param name="context">Evaluation context containing ECSEntityCore and time information.</param>
        /// <returns>Deterministic animation clip result.</returns>
        BlendNodeResult Evaluate(BlendParameters parameters, BlendContext context);

        /// <summary>
        /// Gets debug information about this node's current state.
        /// </summary>
        /// <param name="parameters">Current parameter set.</param>
        /// <returns>Debug information string.</returns>
        string GetDebugInfo(BlendParameters parameters);

        /// <summary>
        /// Validates that this node is properly configured.
        /// </summary>
        /// <returns>Validation result with any issues.</returns>
        BlendNodeValidationResult Validate();
    }

    /// <summary>
    /// Result of blend node evaluation with deterministic output.
    /// </summary>
    public readonly struct BlendNodeResult
    {
        /// <summary>
        /// The animation clip identifier resulting from evaluation.
        /// </summary>
        public string ClipId { get; }

        /// <summary>
        /// Blend weight applied to this result (0.0 to 1.0).
        /// </summary>
        public float Weight { get; }

        /// <summary>
        /// Whether this result represents a valid animation clip.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlendNodeResult"/> struct.
        /// </summary>
        /// <param name="clipId">The animation clip identifier.</param>
        /// <param name="weight">The blend weight (0.0 to 1.0).</param>
        public BlendNodeResult(string clipId, float weight = 1.0f)
        {
            ClipId = clipId ?? string.Empty;
            Weight = System.Math.Clamp(weight, 0.0f, 1.0f);
            IsValid = !string.IsNullOrEmpty(ClipId);
        }

        /// <summary>
        /// Creates an invalid blend node result.
        /// </summary>
        public static BlendNodeResult Invalid => new BlendNodeResult(string.Empty, 0.0f);
    }

    /// <summary>
    /// Context information provided during blend node evaluation.
    /// </summary>
    public readonly struct BlendContext
    {
        /// <summary>
        /// ECSEntityCore identifier for context-specific evaluation.
        /// </summary>
        public uint EntityId { get; }

        /// <summary>
        /// Current time in seconds for time-based calculations.
        /// </summary>
        public float CurrentTime { get; }

        /// <summary>
        /// Delta time since last evaluation frame.
        /// </summary>
        public float DeltaTime { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlendContext"/> struct.
        /// </summary>
        /// <param name="ECSEntityCoreId">The ECSEntityCore identifier.</param>
        /// <param name="currentTime">The current time in seconds.</param>
        /// <param name="deltaTime">The delta time since the last frame.</param>
        public BlendContext(uint ECSEntityCoreId, float currentTime, float deltaTime)
        {
            EntityId = ECSEntityCoreId;
            CurrentTime = currentTime;
            DeltaTime = deltaTime;
        }
    }

    /// <summary>
    /// Validation result for blend node configuration.
    /// </summary>
    public readonly struct BlendNodeValidationResult
    {
        /// <summary>
        /// Whether the node passed validation.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// List of validation errors found.
        /// </summary>
        public IReadOnlyList<string> Errors { get; }

        /// <summary>
        /// List of validation warnings found.
        /// </summary>
        public IReadOnlyList<string> Warnings { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlendNodeValidationResult"/> struct.
        /// </summary>
        /// <param name="isValid">Indicates whether the node passed validation.</param>
        /// <param name="errors">List of validation errors.</param>
        /// <param name="warnings">List of validation warnings.</param>
        public BlendNodeValidationResult(bool isValid, IEnumerable<string>? errors = null, IEnumerable<string>? warnings = null)
        {
            IsValid = isValid;
            Errors = errors != null ? new List<string>(errors) : Array.Empty<string>();
            Warnings = warnings != null ? new List<string>(warnings) : Array.Empty<string>();
        }
    }
}
