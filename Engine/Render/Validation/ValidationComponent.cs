// =====================================================================================================
//  FILE: ValidationComponent.cs
//  PATH: Engine/Render/Validation/ValidationComponent.cs
//  SUBSYSTEM: Render/Validation Subsystem
//
//  ROLE:
//      Defines the minimal deterministic lifecycle contract for any engine-hosted program.
//      This interface is implemented by engine hosts (e.g., GameRootMain) to provide a clean,
//      engine-facing API for startup, execution entry, and deterministic shutdown operations.
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface for program orchestration.
//      - Enforce the structural sequencing contract: Initialize → Run Loop Execution → Shutdown.
//      - Serve as the base contract for any future top-level engine-hosted program modules.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deep frame-level update calculation rules or rendering commands directly.
//      - Managing active systems registration pools, engine assets, or game states.
//      - Handling discrete hardware device allocation boundaries.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces the legacy GameRoot partial lifecycle methods.
//      - GameRootMain implements this interface and delegates to its subsystems:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================

using System;
using System.Collections.Generic;
using System.Reflection;        // CRITICAL FIX: Required for PropertyInfo
using static SASZombieAssaultTD.Engine.Render.Validation.ValidationEnums;

namespace SASZombieAssaultTD.Engine.Render.Validation
{
    public class ValidationComponent
    {
        private readonly List<ValidationResult> _validationResults = new List<ValidationResult>();

        public bool ValidateComponent(object component, string componentName)
        {
            var result = new ValidationResult
            {
                ComponentName = componentName,
                IsValid = true,
                Severity = ValidationSeverity.Info
            };

            try
            {
                //Check for null references
                if (component == null)
                {
                    result.IsValid = false;
                    // NOTE: Swapped 'Critical' to 'Error' to match your previously defined ValidationSeverity enum
                    result.Severity = ValidationSeverity.Error;
                    result.Message = "Component is null";
                    _validationResults.Add(result);
                    return false;
                }

                //Check for required properties using reflection
                var componentType = component.GetType();

                var requiredProperties = GetRequiredProperties(componentType);

                foreach (var property in requiredProperties)
                {
                    var value = property.GetValue(component);
                    if (value == null)
                    {
                        result.IsValid = false;
                        result.Severity = ValidationSeverity.Error;
                        result.Message = $"Required property '{property.Name}' is null";
                        break;
                    }
                }

                result.Message = result.IsValid ?
                "Component validation passed" :
                result.Message;
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.Severity = ValidationSeverity.Error;
                result.Message = $"Validation exception: {ex.Message}";
            }

            _validationResults.Add(result);
            return result.IsValid;
        }

        //===============================================================================================
        // REFLECTION HELPER
        //===============================================================================================
        /// <summary>
        /// Returns all public instance properties on the incoming object type
        /// </summary>
        private IEnumerable<PropertyInfo> GetRequiredProperties(Type type)
        {
            if (type == null)
                return Array.Empty<PropertyInfo>();

            // Natively returns all public instance properties on the incoming object type
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }
    }
}
