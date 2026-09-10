// =====================================================================================================
//  FILE: TC_PrerequisitesValidator.cs
//  PATH: Engine/Towers/TowerControl/Upgrades/Validators/TC_PrerequisitesValidator.cs
//  SUBSYSTEM: Towers/TowerControl/Upgrades/Validators
//
//  ROLE:
//      Provides deterministic validation of upgrade prerequisite definitions and built
//      prerequisite objects. Ensures that prerequisite definitions are structurally valid
//      and that constructed TC_Prerequisites meet minimal deterministic requirements before
//      being consumed by downstream systems.
//      Supports initialization, execution entry, and shutdown to align with engine lifecycle flow.
//
//  RESPONSIBILITIES:
//      - Initialize validator state if required.
//      - Validate prerequisite definitions for structural correctness.
//      - Validate built prerequisite objects for deterministic completeness.
//      - Maintain lifecycle structure: Initialize → Execute → Shutdown.
//      - Emit diagnostic trace statements for lifecycle and validation operations.
//
//  NON-RESPONSIBILITIES:
//      - Storing or constructing prerequisite definitions (handled by TC_RequirementDefinitions / TC_RequirementBuilder).
//      - Evaluating prerequisite logic or determining whether TC_Prerequisites are met.
//      - Managing persistence, loading, or saving operations.
//      - Mutating external engine state or performing runtime prerequisite logic.
//
//  ARCHITECTURAL NOTES:
//      - Operates strictly as a validator; does not mutate external engine state.
//      - Follows the engine-hosted lifecycle pattern for consistency across TowerControl subsystems.
//      - Shutdown clears internal references to maintain deterministic teardown behavior.
// =====================================================================================================

using System.Diagnostics;
using SASZombieAssaultTD.Engine.Towers.TowerControl.Upgrades.Definitions;
using SASZombieAssaultTD.Engine.Towers.TowerControl.Upgrades.Requirements;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Upgrades.Validators
{
    internal sealed class TC_PrerequisitesValidator
    {
        public void Initialize()
        {
            // Trace lifecycle initialization
            Trace.WriteLine("TC_PrerequisitesValidator: Initialize - Validator ready for prerequisite validation.");
        }

        public void Execute()
        {
            // Trace execution entry (no runtime operations required)
            Trace.WriteLine("TC_PrerequisitesValidator: Execute - No active runtime operations required.");
        }

        public void Shutdown()
        {
            // Trace teardown
            Trace.WriteLine("TC_PrerequisitesValidator: Shutdown - Validator teardown complete.");
        }

        public bool ValidateDefinition(RequirementDefinition definition)
        {
            // Trace validation request
            Trace.WriteLine($"TC_PrerequisitesValidator: ValidateDefinition - Validating prerequisite definition '{definition?.Key}'.");

            // Guard against null definition
            if (definition == null)
            {
                Trace.WriteLine("TC_PrerequisitesValidator: ValidateDefinition - Definition is null; validation failed.");
                return false;
            }

            // Inline comment: ensure key and description are non-empty
            if (string.IsNullOrWhiteSpace(definition.Key) ||
                string.IsNullOrWhiteSpace(definition.Description))
            {
                Trace.WriteLine("TC_PrerequisitesValidator: ValidateDefinition - Definition missing required fields; validation failed.");
                return false;
            }

            // Trace successful validation
            Trace.WriteLine($"TC_PrerequisitesValidator: ValidateDefinition - Definition '{definition.Key}' validated successfully.");
            return true;
        }

        public bool ValidateBuiltRequirement(BuiltRequirement requirement)
        {
            // Trace validation request
            Trace.WriteLine($"TC_PrerequisitesValidator: ValidateBuiltRequirement - Validating built requirement '{requirement?.Key}'.");

            // Guard against null requirement
            if (requirement == null)
            {
                Trace.WriteLine("TC_PrerequisitesValidator: ValidateBuiltRequirement - Built requirement is null; validation failed.");
                return false;
            }

            // Inline comment: ensure built requirement contains required fields
            if (string.IsNullOrWhiteSpace(requirement.Key) ||
                string.IsNullOrWhiteSpace(requirement.Description))
            {
                Trace.WriteLine("TC_PrerequisitesValidator: ValidateBuiltRequirement - Built requirement missing required fields; validation failed.");
                return false;
            }

            // Trace successful validation
            Trace.WriteLine($"TC_PrerequisitesValidator: ValidateBuiltRequirement - Built requirement '{requirement.Key}' validated successfully.");
            return true;
        }
    }
}
