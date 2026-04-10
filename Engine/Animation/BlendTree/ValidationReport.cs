// ROLE: Simplified validation report container.
// RESPONSIBILITY: Provide lightweight alternative to BlendTreeValidationResult for basic reporting.
// TRIGGERS: Returned by BlendTreeValidator for simplified validation reporting.
// INPUTS: Receives validation error and warning messages.
// OUTPUTS: Provides IsValid flag and three-tier reporting lists.
// DEPENDENCIES: Self-contained with List-based storage.
// CONTENTS: ValidationReport class with Errors, Warnings, Recommendations lists and IsValid property.

using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Animation.BlendTree
{
    /// <summary>
    /// Validation report for blend tree validation.
    /// Provides simplified error/warning/recommendation tracking.
    /// </summary>
    public class ValidationReport
    {
        /// <summary>Validation errors that prevent proper functioning.</summary>
        public List<string> Errors { get; set; } = new List<string>();
        
        /// <summary>Warnings that indicate potential issues.</summary>
        public List<string> Warnings { get; set; } = new List<string>();
        
        /// <summary>Recommendations for improvements.</summary>
        public List<string> Recommendations { get; set; } = new List<string>();
        
        /// <summary>Returns true if no validation errors exist.</summary>
        public bool IsValid => Errors.Count == 0;
    }
}
