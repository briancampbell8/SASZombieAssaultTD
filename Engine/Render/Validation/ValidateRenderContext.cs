// =====================================================================================================
//  FILE: ValidateRenderContext.cs
//  PATH: Engine/Render/Validation/ValidateRenderContext.cs
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

using System.Collections.Generic;
using SASZombieAssaultTD.Engine.UI.Rendering;

namespace SASZombieAssaultTD.Engine.Render.Validation
{
    public class ValidateRenderContext
    {
        // CRITICAL FIX (CS0103): Declare the missing results collection field at the class scope level.
        private readonly List<ValidationResult> _validationResults = new List<ValidationResult>();

        // CRITICAL FIX (CS0542): Renamed the method from 'ValidateRenderContext' to 'ExecuteContextCheck'
        // so it no longer conflicts with the class name.
        private void ExecuteContextCheck(IDrawingContext renderContext)
        {
            // CRITICAL FIX (CS0246): Instantiated 'ValidationResult' instead of the field name variable tracking token.
            // CRITICAL FIX (CS0103): Fully qualified the enum path to 'ValidationEnums.ValidationSeverity.Error' (using Error as a valid replacement for critical).
            var result = new ValidationResult
            {
                ComponentName = "D3D11Adapter_Core",
                Severity = (ValidationEnums.ValidationSeverity)(ValidationSeverity)ValidationEnums.ValidationSeverity.Error
            };

            if (renderContext == null)
            {
                result.IsValid = false;
                result.AddError("D3D11Adapter_Core is null");
                result.Message = "D3D11Adapter_Core is null";
            }

            else if (renderContext.Width <= 0 || renderContext.Height <= 0)
            {
                result.IsValid = false;
                result.AddError($"Invalid render context dimensions: {renderContext.Width}x{renderContext.Height}");
                result.Message = $"Invalid render context dimensions: {renderContext.Width}x{renderContext.Height}";
            }

            else
            {
                result.IsValid = true;
                result.Message = $"Render context initialized: {renderContext.Width}x{renderContext.Height}";
                result.Severity = (ValidationEnums.ValidationSeverity)(ValidationSeverity)ValidationEnums.ValidationSeverity.Info; // CRITICAL FIX (CS0103): Fully qualified enum path
            }

            _validationResults.Add(result);
        }
    }

    // Helper contract stub to ensure seamless local compilation if not loaded in your active window context view

}
