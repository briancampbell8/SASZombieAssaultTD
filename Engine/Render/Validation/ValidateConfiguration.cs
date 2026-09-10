// =====================================================================================================
//  FILE: ValidateConfiguration.cs
//  PATH: Engine/Render/Validation/ValidateConfiguration.cs
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

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.Render.Validation.ValidationEnums;

namespace SASZombieAssaultTD.Engine.Render.Validation
{
    public class ValidateConfiguration
    {
        private class _validationResults
        {
            public string ComponentName { get; set; }
            public object Severity { get; set; }
            public bool IsValid { get; internal set; }
            public bool AddError { get; internal set; }
            public string Message { get; internal set; }
            public static void Add(_validationResults result)
            {
                System.ArgumentNullException.ThrowIfNull(result);
                DLogger.Log(
                    result.ComponentName,
                    result.Severity,
                    result.IsValid,
                    result.AddError,
                    result.Message);
            }


        }
        private void Configuration(RenderPipelineConfig config)
        {
            var result = new _validationResults
            {
                ComponentName = "RenderPipelineConfig",
                Severity = ValidationSeverity.Error
            };

            if (config == null)
            {
                result.IsValid = false;
                result.AddError = true;
                result.Message = "RenderPipelineConfig is null";
            }
            else if (!config.ValidateSettings(out var errorMessage))
            {
                result.IsValid = false;
                result.AddError = true;
                result.Message = $"Configuration validation failed: {errorMessage}";
            }
            else
            {
                result.IsValid = true;
                result.AddError = false;
                result.Message = "Configuration is valid";
                result.Severity = ValidationSeverity.Info;
            }
            _validationResults.Add(result);


        }


    }
}
