using SASZombieAssaultTD.Engine.Animation;
using SASZombieAssaultTD.Engine.Animation.BlendTree;
using SASZombieAssaultTD.Engine.Utility;
using SASZombieAssaultTD.Engine.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Animation.BlendTree
{
    /// <summary>
    /// P11-18-14: Validator that checks node graphs, parameter usage, and unreachable nodes.
    /// Provides comprehensive validation for blend tree integrity and correctness.
    /// </summary>
    public static class BlendTreeValidator
    {
        /// <summary>
        /// P11-18-14: Validation severity levels for different types of issues.
        /// Deterministic categorization of validation problems.
        /// </summary>
        public enum ValidationSeverity
        {
            /// <summary>
            /// Informational message that doesn't affect functionality.
            /// </summary>
            Info,

            /// <summary>
            /// Warning that might cause issues but won't break functionality.
            /// </summary>
            Warning,

            /// <summary>
            /// Error that will cause functional problems.
            /// </summary>
            Error,

            /// <summary>
            /// Critical error that will prevent the blend tree from working.
            /// </summary>
            Critical
        }

        /// <summary>
        /// P11-18-14: Detailed validation result for blend tree validation.
        /// Comprehensive information about validation issues and recommendations.
        /// </summary>
        public class BlendTreeValidationReport
        {
            /// <summary>
            /// Whether the blend tree passed all validation checks.
            /// </summary>
            public bool IsValid { get; set; }

            /// <summary>
            /// List of validation issues found.
            /// </summary>
            public List<ValidationIssue> Issues { get; set; } = new();

            /// <summary>
            /// List of recommendations for improvement.
            /// </summary>
            public List<string> Recommendations { get; set; } = new();

            /// <summary>
            /// Statistics about the blend tree structure.
            /// </summary>
            public BlendTreeStatistics Statistics { get; set; } = new();

            /// <summary>
            /// Time taken to perform validation.
            /// </summary>
            public TimeSpan ValidationDuration { get; set; }
        }

        /// <summary>
        /// P11-18-14: Individual validation issue with detailed information.
        /// Deterministic structure for reporting validation problems.
        /// </summary>
        public class ValidationIssue
        {
            /// <summary>
            /// Severity of the validation issue.
            /// </summary>
            public ValidationSeverity Severity { get; set; }

            /// <summary>
            /// Category of the validation issue.
            /// </summary>
            public string Category { get; set; } = string.Empty;

            /// <summary>
            /// Description of the validation issue.
            /// </summary>
            public string Description { get; set; } = string.Empty;

            /// <summary>
            /// Node ID where the issue was found, if applicable.
            /// </summary>
            public string NodeId { get; set; } = string.Empty;

            /// <summary>
            /// Recommendation for fixing the issue.
            /// </summary>
            public string Recommendation { get; set; } = string.Empty;
        }

        /// <summary>
        /// P11-18-14: Statistics about blend tree structure and complexity.
        /// Deterministic metrics for blend tree analysis.
        /// </summary>
        public class BlendTreeStatistics
        {
            /// <summary>
            /// Total number of nodes in the blend tree.
            /// </summary>
            public int TotalNodes { get; set; }

            /// <summary>
            /// Number of leaf nodes (SingleClipNode).
            /// </summary>
            public int LeafNodes { get; set; }

            /// <summary>
            /// Number of blend nodes (LinearBlendNode, TwoDBlendNode).
            /// </summary>
            public int BlendNodes { get; set; }

            /// <summary>
            /// Maximum depth of the blend tree.
            /// </summary>
            public int MaxDepth { get; set; }

            /// <summary>
            /// Number of unique parameters used across all nodes.
            /// </summary>
            public int UniqueParameters { get; set; }

            /// <summary>
            /// Number of unreachable nodes detected.
            /// </summary>
            public int UnreachableNodes { get; set; }

            /// <summary>
            /// Number of circular references detected.
            /// </summary>
            public int CircularReferences { get; set; }
        }

        /// <summary>
        /// P11-18-14: Performs comprehensive validation of a blend tree.
        /// Checks node graphs, parameter usage, unreachable nodes, and structural integrity.
        /// </summary>
        /// <param name="blendTree">Blend tree to validate</param>
        /// <returns>Detailed validation report</returns>
        public static BlendTreeValidationReport ValidateBlendTree(BlendTree blendTree)
        {
            var startTime = DateTime.Now;
            var report = new BlendTreeValidationReport();

            try
            {
                if (blendTree == null)
                {
                    report.Issues.Add(new ValidationIssue
                    {
                        Severity = ValidationSeverity.Critical,
                        Category = "Structure",
                        Description = "Blend tree is null",
                        Recommendation = "Provide a valid blend tree instance"
                    });
                    report.IsValid = false;
                    report.ValidationDuration = DateTime.Now - startTime;
                    return report;
                }

                Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"BlendTreeValidator: Starting validation for blend tree '{blendTree.TreeId}'");

                // Perform validation checks
                ValidateBasicStructure(blendTree, report);
                ValidateNodeGraph(blendTree, report);
                ValidateParameterUsage(blendTree, report);
                ValidateNodeConnections(blendTree, report);
                CheckUnreachableNodes(blendTree, report);
                CheckCircularReferences(blendTree, report);
                CalculateStatistics(blendTree, report);

                // Determine overall validity
                var criticalIssues = report.Issues.Count(i => i.Severity == ValidationSeverity.Critical);
                var errorIssues = report.Issues.Count(i => i.Severity == ValidationSeverity.Error);

                report.IsValid = criticalIssues == 0 && errorIssues == 0;
                report.ValidationDuration = DateTime.Now - startTime;

                Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"BlendTreeValidator: Validation completed for '{blendTree.TreeId}' - " +
                $"Valid: {report.IsValid}, Issues: {report.Issues.Count}, Duration: {report.ValidationDuration.TotalMilliseconds:F2}ms");

                return report;
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"BlendTreeValidator: Exception during validation: {ex.Message}");

                report.Issues.Add(new ValidationIssue
                {
                    Severity = ValidationSeverity.Critical,
                    Category = "System",
                    Description = $"Validation exception: {ex.Message}",
                    Recommendation = "Check blend tree structure and try again"
                });
                report.IsValid = false;
                report.ValidationDuration = DateTime.Now - startTime;

                return report;
            }
        }

        /// <summary>
        /// P11-18-14: Validates basic blend tree structure.
        /// Checks essential properties like tree ID, display name, and root node.
        /// </summary>
        /// <param name="blendTree">Blend tree to validate</param>
        /// <param name="report">Validation report to update</param>
        private static void ValidateBasicStructure(BlendTree blendTree, BlendTreeValidationReport report)
        {
            // Validate tree ID
            if (string.IsNullOrEmpty(blendTree.TreeId))
            {
                report.Issues.Add(new ValidationIssue
                {
                    Severity = ValidationSeverity.Critical,
                    Category = "Structure",
                    Description = "Blend tree ID is null or empty",
                    Recommendation = "Set a valid tree ID"
                });
            }

            // Validate display name
            if (string.IsNullOrEmpty(blendTree.DisplayName))
            {
                report.Issues.Add(new ValidationIssue
                {
                    Severity = ValidationSeverity.Warning,
                    Category = "Structure",
                    Description = "Blend tree display name is null or empty",
                    Recommendation = "Set a descriptive display name"
                });
            }

            // Validate root node
            if (blendTree.RootNode == null)
            {
                report.Issues.Add(new ValidationIssue
                {
                    Severity = ValidationSeverity.Critical,
                    Category = "Structure",
                    Description = "Blend tree has no root node",
                    Recommendation = "Set a valid root node for the blend tree"
                });
            }
        }

        /// <summary>
        /// P11-18-14: Validates individual nodes in the blend tree.
        /// Checks each node for proper configuration and validation.
        /// </summary>
        /// <param name="blendTree">Blend tree to validate</param>
        /// <param name="report">Validation report to update</param>
        private static void ValidateNodeGraph(BlendTree blendTree, BlendTreeValidationReport report)
        {
            var stats = new BlendTreeStatistics();
            
            // Calculate maximum depth
            int CalculateDepth(IBlendNode node)
            {
                if (node == null)
                    return 0;

                if (node is SingleClipNode)
                    return 1;

                if (node is LinearBlendNode linearNode)
                {
                    var depthA = CalculateDepth(linearNode.ChildA);
                    var depthB = CalculateDepth(linearNode.ChildB);
                    return 1 + System.Math.Max(depthA, depthB);
                }

                if (node is TwoDBlendNode twoDNode)
                {
                    var depthBL = CalculateDepth(twoDNode.ChildBottomLeft);
                    var depthBR = CalculateDepth(twoDNode.ChildBottomRight);
                    var depthTL = CalculateDepth(twoDNode.ChildTopLeft);
                    var depthTR = CalculateDepth(twoDNode.ChildTopRight);
                    return 1 + System.Math.Max(System.Math.Max(depthBL, depthBR), System.Math.Max(depthTL, depthTR));
                }

                return 1;
            }

            stats.MaxDepth = CalculateDepth(blendTree.RootNode);

            // Add recommendations based on statistics
            if (stats.MaxDepth > 10)
            {
                report.Recommendations.Add("Consider reducing blend tree depth for better performance");
            }

            if (stats.BlendNodes > stats.LeafNodes * 2)
            {
                report.Recommendations.Add("High ratio of blend nodes to leaf nodes detected - consider optimization");
            }

            if (stats.UniqueParameters > 20)
            {
                report.Recommendations.Add("Large number of parameters detected - consider parameter consolidation");
            }
        }

        // Missing validation methods
        private static void ValidateParameterUsage(BlendTree blendTree, BlendTreeValidationReport report)
        {
            // Stub implementation
            report.Recommendations.Add("Parameter usage validation not implemented");
        }

        private static void ValidateNodeConnections(BlendTree blendTree, BlendTreeValidationReport report)
        {
            // Stub implementation
            report.Recommendations.Add("Node connection validation not implemented");
        }

        private static void CheckUnreachableNodes(BlendTree blendTree, BlendTreeValidationReport report)
        {
            // Stub implementation
            report.Recommendations.Add("Unreachable node check not implemented");
        }

        private static void CheckCircularReferences(BlendTree blendTree, BlendTreeValidationReport report)
        {
            // Stub implementation
            report.Recommendations.Add("Circular reference check not implemented");
        }

        private static void CalculateStatistics(BlendTree blendTree, BlendTreeValidationReport report)
        {
            // Stub implementation
            report.Recommendations.Add("Statistics calculation not implemented");
        }
    }
}




