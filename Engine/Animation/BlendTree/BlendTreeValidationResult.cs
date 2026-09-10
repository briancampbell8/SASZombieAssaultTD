// ====================================================================================================
//  FILE: BlendTreeValidationResult.cs
//  PATH: ./Engine/Animation/BlendTree/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the BlendTreeValidationResult module.
//
//  RESPONSIBILITIES:
//      - Provide AddError() behavior for the Core subsystem.
//      - Provide AddWarning() behavior for the Core subsystem.
//      - Provide Combine() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//File: Engine/Animation/BlendTrees/Nodes/BlendTreeValidationResult.cs
//Purpose: Represents the result of validating a blend tree.
//Required by BlendTreeSerializer and BlendTreeValidator.

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.Animation.AnimationEnums;

namespace SASZombieAssaultTD.Engine.Animation.BlendTree
{
    /// <summary>
    /// Validation severity levels.
    /// </summary>

    /// <summary>
    /// Represents the result of validating a blend tree structure. Used by serializers and validators to report issues
    /// or confirm correctness.
    /// </summary>
    public class BlendTreeValidationResult
    {
        /// <summary>
        /// Indicates whether the blend tree is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Validation severity level.
        /// </summary>
        public ValidationSeverity Severity { get; set; }

        /// <summary>
        /// List of validation errors.
        /// </summary>
        public List<string> Errors { get; } = new();

        /// <summary>
        /// List of validation warnings.
        /// </summary>
        public List<string> Warnings { get; } = new();

        /// <summary>
        /// Error message for quick access.
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Creates a default validation result.
        /// </summary>
        public BlendTreeValidationResult()
        { }

        /// <summary>
        /// Creates a validation result with explicit values.
        /// </summary>
        /// <param name="isValid">Indicates whether the blend tree is valid.</param>
        /// <param name="errors">List of validation errors.</param>
        /// <param name="warnings">List of validation warnings.</param>
        public BlendTreeValidationResult(bool isValid, IEnumerable<string> errors, IEnumerable<string> warnings)
        {
            IsValid = isValid;
            Errors.AddRange(errors);
            Warnings.AddRange(warnings);
        }

        /// <summary>
        /// Adds an error to the validation result.
        /// </summary>
        /// <param name="error">The error message to add.</param>
        public void AddError(string error)
        {
            if (!string.IsNullOrWhiteSpace(error))
            {
                Errors.Add(error);
                IsValid = false;
            }
        }

        /// <summary>
        /// Adds a warning to the validation result.
        /// </summary>
        /// <param name="warning">The warning message to add.</param>
        public void AddWarning(string warning)
        {
            if (!string.IsNullOrWhiteSpace(warning))
            {
                Warnings.Add(warning);
            }
        }

        /// <summary>
        /// Combines another validation result into this one.
        /// </summary>
        /// <param name="other">The other validation result to combine.</param>
        public void Combine(BlendTreeValidationResult other)
        {
            if (other == null) return;

            Errors.AddRange(other.Errors);
            Warnings.AddRange(other.Warnings);
            if (!other.IsValid)
            {
                IsValid = false;
            }
        }
    }
}
