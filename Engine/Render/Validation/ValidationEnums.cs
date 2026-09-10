// =====================================================================================================
//  FILE: ValidationEnums.cs
//  PATH: Engine/Render/Validation/ValidationEnums.cs
//  SUBSYSTEM: Render/Validation Subsystem
//
//  ROLE:
//      Defines enumerations used by the ValidateSystemDependencies interface.
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

namespace SASZombieAssaultTD.Engine.Render.Validation
{
    public class ValidationEnums
    {
        /// <summary>
        /// Severity levels for validation results.
        /// </summary>
        public enum ValidationSeverity
        {
            Info = 0,
            Warning = 1,
            Error = 2,
            Critical = 3
        }

        public enum ValidationResultType
        {
            Info,
            Warning,
            Error,
            Critical
        }

        public enum ValidationResultStatus
        {
            Valid,
            Invalid,
            Unknown
        }

        public enum ValidationResultStatusSeverity
        {
            Info,
            Warning,
            Error,
            Critical
        }

        public enum ValidationResultStatusType
        {
            Info,
            Warning,
            Error,
            Critical
        }

    }
}
