using System;
using System.Collections.Generic;
using System.Text;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Animation.BlendTree
{
    ///<summary>
    ///Linear blend node that blends two child nodes based on a single float parameter.
    ///</summary>
    public class LinearBlendNode : IBlendNode
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
        ///Parameter name used for blending (0.0 to 1.0 range).
        ///</summary>
        public string BlendParameter { get; }

        ///<summary>
        ///Child node for the minimum value (blend parameter = 0.0).
        ///</summary>
        public IBlendNode ChildA { get; }

        ///<summary>
        ///Child node for the maximum value (blend parameter = 1.0).
        ///</summary>
        public IBlendNode ChildB { get; }

        ///<summary>
        ///Node weight approximated from children (safe default).
        ///</summary>
        public float Weight => System.MathF.Max(ChildA?.Weight ?? 0f, ChildB?.Weight ?? 0f);

        ///<summary>
        ///List of parameters required by this node.
        ///</summary>
        public IReadOnlyList<string> RequiredParameters
        {
            get
            {
                var parameters = new HashSet<string> { BlendParameter };
                if (ChildA != null) parameters.UnionWith(ChildA.RequiredParameters);
                if (ChildB != null) parameters.UnionWith(ChildB.RequiredParameters);
                return new List<string>(parameters);
            }
        }

        ///<summary>
        ///Initializes a new linear blend node.
        ///</summary>
        ///<param name="nodeId">Unique node identifier.</param>
        ///<param name="displayName">Human-readable name.</param>
        ///<param name="blendParameter">Parameter name for blending.</param>
        ///<param name="childA">Child node for minimum value.</param>
        ///<param name="childB">Child node for maximum value.</param>
        public LinearBlendNode(string nodeId, string displayName, string blendParameter, IBlendNode childA, IBlendNode childB)
        {
            NodeId = nodeId ?? throw new ArgumentNullException(nameof(nodeId));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            BlendParameter = blendParameter ?? throw new ArgumentNullException(nameof(blendParameter));
            ChildA = childA ?? throw new ArgumentNullException(nameof(childA));
            ChildB = childB ?? throw new ArgumentNullException(nameof(childB));
        }

        ///<summary>
        ///Evaluates the node by blending two child nodes based on the blend parameter.
        ///</summary>
        ///<param name="parameters">Parameter set for evaluation.</param>
        ///<param name="context">Evaluation context.</param>
        ///<returns>Deterministic animation clip result.</returns>
        public BlendNodeResult Evaluate(BlendParameters parameters, BlendContext context)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            float blendValue = parameters.TryGetFloat(BlendParameter, out float value) ? System.Math.Clamp(value, 0.0f, 1.0f) : 0.0f;

            BlendNodeResult resultA = ChildA.Evaluate(parameters, context);
            BlendNodeResult resultB = ChildB.Evaluate(parameters, context);

            if (!resultA.IsValid && !resultB.IsValid) return BlendNodeResult.Invalid;
            if (!resultA.IsValid) return new BlendNodeResult(resultB.ClipId, resultB.Weight * blendValue);
            if (!resultB.IsValid) return new BlendNodeResult(resultA.ClipId, resultA.Weight * (1.0f - blendValue));

            return blendValue switch
            {
                <= 0.0f => new BlendNodeResult(resultA.ClipId, resultA.Weight),
                >= 1.0f => new BlendNodeResult(resultB.ClipId, resultB.Weight),
                _ => new BlendNodeResult(
                    blendValue < 0.5f ? resultA.ClipId : resultB.ClipId,
                    System.MathF.Max(resultA.Weight, resultB.Weight))
            };
        }

        ///<summary>
        ///Gets debug information about this node's state.
        ///</summary>
        ///<param name="parameters">Current parameter set.</param>
        ///<returns>Debug information string.</returns>
        public string GetDebugInfo(BlendParameters parameters)
        {
            float blendValue = parameters?.TryGetFloat(BlendParameter, out float value) == true ? value : 0.0f;

            var info = new StringBuilder();
            info.AppendLine($"LinearBlendNode: {DisplayName} (ID: {NodeId})");
            info.AppendLine($"  Blend Parameter: {BlendParameter} = {blendValue:F3}");
            info.AppendLine($"  Child A: {ChildA?.DisplayName ?? "None"}");
            info.AppendLine($"  Child B: {ChildB?.DisplayName ?? "None"}");

            if (ChildA != null) info.AppendLine($"    A Debug: {ChildA.GetDebugInfo(parameters)}");
            if (ChildB != null) info.AppendLine($"    B Debug: {ChildB.GetDebugInfo(parameters)}");

            return info.ToString();
        }

        ///<summary>
        ///Validates that this node is properly configured.
        ///</summary>
        ///<returns>Validation result with any issues.</returns>
        public BlendNodeValidationResult Validate()
        {
            var errors = new List<string>();
            var warnings = new List<string>();

            if (string.IsNullOrEmpty(NodeId)) errors.Add("Node ID cannot be null or empty.");
            if (string.IsNullOrEmpty(DisplayName)) errors.Add("Display name cannot be null or empty.");
            if (string.IsNullOrEmpty(BlendParameter)) errors.Add("Blend parameter cannot be null or empty.");

            if (ChildA == null) errors.Add("Child A cannot be null.");
            else
            {
                var childAValidation = ChildA.Validate();
                errors.AddRange(childAValidation.Errors);
                warnings.AddRange(childAValidation.Warnings);
            }

            if (ChildB == null) errors.Add("Child B cannot be null.");
            else
            {
                var childBValidation = ChildB.Validate();
                errors.AddRange(childBValidation.Errors);
                warnings.AddRange(childBValidation.Warnings);
            }

            return new BlendNodeValidationResult(errors.Count == 0, errors, warnings);
        }

        ///<summary>
        ///Creates a linear blend node with auto-generated ID.
        ///</summary>
        ///<param name="displayName">Human-readable name.</param>
        ///<param name="blendParameter">Parameter name for blending.</param>
        ///<param name="childA">Child node for minimum value.</param>
        ///<param name="childB">Child node for maximum value.</param>
        ///<returns>New linear blend node.</returns>
        public static LinearBlendNode CreateAuto(string displayName, string blendParameter, IBlendNode childA, IBlendNode childB)
        {
            string nodeId = $"linear_blend_{Guid.NewGuid():N}";
            return new LinearBlendNode(nodeId, displayName, blendParameter, childA, childB);
        }
    }
}




