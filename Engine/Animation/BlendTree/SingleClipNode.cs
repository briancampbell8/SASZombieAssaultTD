// ====================================================================================================
//  FILE: SingleClipNode.cs
//  PATH: ./Engine/Animation/BlendTree/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the SingleClipNode module.
//
//  RESPONSIBILITIES:
//      - Provide Evaluate() behavior for the Core subsystem.
//      - Provide GetDebugInfo() behavior for the Core subsystem.
//      - Provide Validate() behavior for the Core subsystem.
//      - Provide CreateAuto() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Animation.BlendTree
{
    ///<summary>
    ///Leaf node that returns a single animation clip with deterministic parameter reporting.
    ///</summary>
    public class SingleClipNode : IBlendNode
    {
        ///<summary>
        ///Unique identifier for this node.
        ///</summary>
        public string NodeId { get; }

        ///<summary>
        ///Human-readable name for debugging.
        ///</summary>
        public string DisplayName { get; }

        ///<summary>
        ///The animation clip identifier this node returns.
        ///</summary>
        public string ClipId { get; }

        ///<summary>
        ///The node's contribution weight (single clip is full weight).
        ///</summary>
        public float Weight => 1.0f;

        ///<summary>
        ///List of parameters required by this node (empty for single clip nodes).
        ///</summary>
        public IReadOnlyList<string> RequiredParameters => Array.Empty<string>();

        ///<summary>
        ///Initializes a new single clip node.
        ///</summary>
        ///<param name="nodeId">Unique node identifier.</param>
        ///<param name="displayName">Human-readable name.</param>
        ///<param name="clipId">Animation clip identifier.</param>
        public SingleClipNode(string nodeId, string displayName, string clipId)
        {
            NodeId = string.IsNullOrWhiteSpace(nodeId) ? throw new ArgumentException("Node ID cannot be null or whitespace.", nameof(nodeId)) : nodeId;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? throw new ArgumentException("Display name cannot be null or whitespace.", nameof(displayName)) : displayName;
            ClipId = string.IsNullOrWhiteSpace(clipId) ? throw new ArgumentException("Clip ID cannot be null or whitespace.", nameof(clipId)) : clipId;
        }

        ///<summary>
        ///Evaluates the node and returns the single animation clip.
        ///</summary>
        ///<param name="parameters">Parameter set (not used by this node).</param>
        ///<param name="context">Evaluation context.</param>
        ///<returns>Deterministic animation clip result.</returns>
        public BlendNodeResult Evaluate(BlendParameters parameters, BlendContext context)
        {
            return new BlendNodeResult(ClipId, Weight);
        }

        ///<summary>
        ///Gets debug information about this node's state.
        ///</summary>
        ///<param name="parameters">Current parameter set.</param>
        ///<returns>Debug information string.</returns>
        public string GetDebugInfo(BlendParameters parameters)
        {
            return $"SingleClipNode: {DisplayName} (ID: {NodeId}) -> Clip: {ClipId}";
        }

        ///<summary>
        ///Validates that this node is properly configured.
        ///</summary>
        ///<returns>Validation result with any issues.</returns>
        public BlendNodeValidationResult Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(NodeId))
                errors.Add("Node ID cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(DisplayName))
                errors.Add("Display name cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(ClipId))
                errors.Add("Clip ID cannot be null or empty.");

            return new BlendNodeValidationResult(errors.Count == 0, errors, Array.Empty<string>());
        }

        ///<summary>
        ///Creates a single clip node with auto-generated ID.
        ///</summary>
        ///<param name="displayName">Human-readable name.</param>
        ///<param name="clipId">Animation clip identifier.</param>
        ///<returns>New single clip node.</returns>
        public static SingleClipNode CreateAuto(string displayName, string clipId)
        {
            string nodeId = $"single_clip_{Guid.NewGuid():N}";
            return new SingleClipNode(nodeId, displayName, clipId);
        }
    }
}





