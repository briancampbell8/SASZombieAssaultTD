// ROLE: Container for blend tree validation results.
// RESPONSIBILITY: Store validation errors, warnings, and severity levels from tree inspection.
// TRIGGERS: Returned by BlendTreeValidator and IBlendNode.Validate during validation operations.
// INPUTS: Receives error and warning messages during validation checks.
// OUTPUTS: Provides IsValid flag and aggregated validation messages.
// DEPENDENCIES: Uses ValidationSeverity enum for categorization.
// CONTENTS: BlendTreeValidationResult class with IsValid, Errors, Warnings, Infos properties 
//           and AddError, AddWarning, AddInfo, Combine methods.

using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Animation.BlendTree
{
    /// <summary>
    /// Validation severity levels.
    /// </summary>
    public enum ValidationSeverity
    {
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// Represents the result of validating a blend tree structure.
    /// Used by serializers and validators to report issues or confirm correctness.
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
        public BlendTreeValidationResult() { }

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
