// -----------------------------------------------------------------------------
// Two-dimensional blend node for animation blend trees
// Namespace: SASZombieAssaultTD.Engine.Animation.BlendTree
// Implements: IBlendNode
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Animation.BlendTree
{
    /// <summary>
    /// A 2D blend node that blends between child nodes based on two parameters
    /// (for example X/Y movement, speed/direction, etc.).
    /// </summary>
    public class TwoDBlendNode : IBlendNode
    {
        /// <summary>
        /// Unique identifier for this node.
        /// </summary>
        public string NodeId { get; }

        /// <summary>
        /// Human-readable name for debugging.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Parameter name for the X-axis blending.
        /// </summary>
        public string BlendParameterX { get; }

        /// <summary>
        /// Parameter name for the Y-axis blending.
        /// </summary>
        public string BlendParameterY { get; }

        /// <summary>
        /// Child node for the bottom-left corner (X=0, Y=0).
        /// </summary>
        public IBlendNode? ChildBottomLeft { get; }

        /// <summary>
        /// Child node for the bottom-right corner (X=1, Y=0).
        /// </summary>
        public IBlendNode? ChildBottomRight { get; }

        /// <summary>
        /// Child node for the top-left corner (X=0, Y=1).
        /// </summary>
        public IBlendNode? ChildTopLeft { get; }

        /// <summary>
        /// Child node for the top-right corner (X=1, Y=1).
        /// </summary>
        public IBlendNode? ChildTopRight { get; }

        /// <summary>
        /// The names of the parameters this node requires (X and Y).
        /// </summary>
        public IReadOnlyList<string> RequiredParameters => new[] { BlendParameterX, BlendParameterY };

        /// <summary>
        /// Child nodes arranged in 2D space.
        /// </summary>
        public IReadOnlyList<IBlendNode> Children { get; }

        /// <summary>
        /// Optional metadata describing how the 2D space is interpreted.
        /// </summary>
        public float MinX { get; set; } = 0.0f;
        public float MaxX { get; set; } = 1.0f;
        public float MinY { get; set; } = 0.0f;
        public float MaxY { get; set; } = 1.0f;

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoDBlendNode"/> class.
        /// </summary>
        /// <param name="displayName">Human-readable name.</param>
        /// <param name="blendParameterX">Parameter name for X-axis blending.</param>
        /// <param name="blendParameterY">Parameter name for Y-axis blending.</param>
        /// <param name="childBottomLeft">Child node for the bottom-left corner.</param>
        /// <param name="childBottomRight">Child node for the bottom-right corner.</param>
        /// <param name="childTopLeft">Child node for the top-left corner.</param>
        /// <param name="childTopRight">Child node for the top-right corner.</param>
        public TwoDBlendNode(
            string displayName,
            string blendParameterX,
            string blendParameterY,
            IBlendNode? childBottomLeft = null,
            IBlendNode? childBottomRight = null,
            IBlendNode? childTopLeft = null,
            IBlendNode? childTopRight = null)
        {
            NodeId = Guid.NewGuid().ToString();
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? throw new ArgumentException("Display name cannot be null or whitespace.", nameof(displayName)) : displayName;
            BlendParameterX = string.IsNullOrWhiteSpace(blendParameterX) ? throw new ArgumentException("Blend parameter X cannot be null or whitespace.", nameof(blendParameterX)) : blendParameterX;
            BlendParameterY = string.IsNullOrWhiteSpace(blendParameterY) ? throw new ArgumentException("Blend parameter Y cannot be null or whitespace.", nameof(blendParameterY)) : blendParameterY;

            ChildBottomLeft = childBottomLeft;
            ChildBottomRight = childBottomRight;
            ChildTopLeft = childTopLeft;
            ChildTopRight = childTopRight;

            Children = new List<IBlendNode?> { childBottomLeft, childBottomRight, childTopLeft, childTopRight }
                .Where(child => child != null)
                .Cast<IBlendNode>()
                .ToList();
        }

        public TwoDBlendNode(string nodeId, string displayName, string xParameter, string yParameter)
        {
            NodeId = nodeId;
            DisplayName = displayName;
            XParameter = xParameter;
            YParameter = yParameter;
        }

        /// <summary>
        /// Evaluates the 2D blend node and returns a blended result from its children.
        /// </summary>
        /// <param name="parameters">Parameter set for evaluation.</param>
        /// <param name="context">Evaluation context.</param>
        /// <returns>Deterministic animation clip result.</returns>
        public BlendNodeResult Evaluate(BlendParameters parameters, BlendContext context)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            float x = parameters.TryGetFloat(BlendParameterX, out float xValue) ? System.Math.Clamp(xValue, MinX, MaxX) : MinX;
            float y = parameters.TryGetFloat(BlendParameterY, out float yValue) ? System.Math.Clamp(yValue, MinY, MaxY) : MinY;

            // Placeholder: delegate to the first valid child for now.
            var validChild = Children.FirstOrDefault();
            return validChild?.Evaluate(parameters, context) ?? BlendNodeResult.Invalid;
        }

        /// <summary>
        /// Returns debug information about this node and its children.
        /// </summary>
        /// <param name="parameters">Current parameter set.</param>
        /// <returns>Debug information string.</returns>
        public string GetDebugInfo(BlendParameters parameters)
        {
            var info = $"TwoDBlendNode: {DisplayName} (Id: {NodeId}, Children: {Children.Count})";
            foreach (var child in Children)
            {
                info += $"\n  Child Debug: {child.GetDebugInfo(parameters)}";
            }
            return info;
        }

        /// <summary>
        /// Validates that the node is structurally sound (parameters, children, ranges).
        /// </summary>
        /// <returns>Validation result with any issues.</returns>
        public BlendNodeValidationResult Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(BlendParameterX))
                errors.Add("Blend parameter X cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(BlendParameterY))
                errors.Add("Blend parameter Y cannot be null or empty.");

            if (!Children.Any())
                errors.Add($"TwoDBlendNode '{DisplayName}' has no children to blend.");

            foreach (var child in Children)
            {
                var childValidation = child.Validate();
                if (!childValidation.IsValid)
                {
                    errors.AddRange(childValidation.Errors);
                }
            }

            return new BlendNodeValidationResult(!errors.Any(), errors, Array.Empty<string>());
        }

        /// <summary>
        /// The node's weight calculated from children (safe default).
        /// </summary>
        public float Weight
        {
            get
            {
                float max = 0f;
                foreach (var c in Children)
                    max = System.MathF.Max(max, c?.Weight ?? 0f);
                return max;
            }
        }

        public string XParameter { get; }
        public string YParameter { get; }

        /// <summary>
        /// Creates a TwoDBlendNode with automatic configuration.
        /// </summary>
        public static TwoDBlendNode CreateAuto(string nodeId, string displayName, string xParameter, string yParameter)
        {
            return new TwoDBlendNode(nodeId, displayName, xParameter, yParameter);
        }
    }
}
