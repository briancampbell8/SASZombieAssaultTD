// =====================================================================================================
//  FILE: ValidationResult.cs
//  PATH: Engine/Render/Validation/ValidationResult.cs
//  SUBSYSTEM: Render/Validation Subsystem
//
//  ROLE:
//      Unified validation result object for the Render subsystem and any dependent validators.
//      Tracks validity, errors, warnings, messages, and severity classification.
//
//  RESPONSIBILITIES:
//      - Provide a consistent API for all validation operations.
//      - Store aggregated error, warning, and message information.
//      - Expose severity levels for diagnostic and reporting systems.
//
//  NON-RESPONSIBILITIES:
//      - Performing validation logic itself.
//      - Writing logs or displaying UI diagnostics.
//
//  ARCHITECTURAL NOTES:
//      - This is the authoritative ValidationResult for the engine.
//      - All subsystem-specific duplicates have been removed.
// =====================================================================================================

using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Towers;

namespace SASZombieAssaultTD.Engine.Render.Validation
{
    public class ValidationResult
    {
        // ---------------------------------------------------------------------------------------------
        //  CORE STATE
        // ---------------------------------------------------------------------------------------------
        public bool IsValid { get; set; }


        public string ComponentName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public ValidationEnums.ValidationSeverity Severity { get; set; } =
            ValidationEnums.ValidationSeverity.Info;

        // ---------------------------------------------------------------------------------------------
        //  COLLECTIONS
        // ---------------------------------------------------------------------------------------------
        public List<string> Errors { get; } = new List<string>();
        public List<string> Warnings { get; } = new List<string>();
        public List<string> Messages { get; } = new List<string>();

        // ---------------------------------------------------------------------------------------------
        //  AGGREGATED TEXT (optional convenience)
        // ---------------------------------------------------------------------------------------------
        public string ErrorText => Errors.Count > 0 ? string.Join("; ", Errors) : string.Empty;
        public string WarningText => Warnings.Count > 0 ? string.Join("; ", Warnings) : string.Empty;
        public string MessagesText => Messages.Count > 0 ? string.Join("; ", Messages) : string.Empty;

        public object ErrorMessage { get; internal set; }

        // ---------------------------------------------------------------------------------------------
        //  API METHODS
        // ---------------------------------------------------------------------------------------------
        public void AddError(string error)
        {
            Errors.Add(error);
            Message = error;
            IsValid = false;
            Severity = ValidationEnums.ValidationSeverity.Error;
        }

        public void AddWarning(string warning)
        {
            Warnings.Add(warning);
            Severity = ValidationEnums.ValidationSeverity.Warning;
        }

        public void AddMessage(string message)
        {
            Messages.Add(message);
        }

        // ---------------------------------------------------------------------------------------------
        //  TOWER UPGRADE VALIDATION (kept exactly as in your original file)
        // ---------------------------------------------------------------------------------------------
        private ValidationResult ValidateUpgrade(TowerUpgrade upgrade, object currentTowerType = null)
        {
            var result = new ValidationResult { IsValid = true };

            if (upgrade.Level <= 0)
                result.AddError("Upgrade level must be positive");

            if (upgrade.Cost < 0)
                result.AddError("Upgrade cost cannot be negative");

            if (string.IsNullOrEmpty(upgrade.Name))
                result.AddError("Upgrade name cannot be empty");

            if (currentTowerType != null && !upgrade.TowerType.Equals(currentTowerType))
                result.AddError("Upgrade tower type mismatch");

            return result;
        }
    }
}
