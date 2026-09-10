// =====================================================================================================
//  FILE: TC_RequirementsValidator.cs
//  PATH: Engine/Towers/TowerControl/Upgrades/Validators/TC_RequirementsValidator.cs
//  SUBSYSTEM: Towers/TowerControl/Upgrades/Validators
//
//  ROLE:
//      Provides deterministic validation of upgrade requirement definitions and built requirement
//      objects. Ensures that requirement definitions are structurally valid and that constructed
//      requirements meet minimal deterministic standards before being consumed by downstream
//      systems. Supports initialization, execution entry, and shutdown to align with engine
//      lifecycle flow.
//
//  RESPONSIBILITIES:
//      - Initialize validator state if required.
//      - Validate requirement definitions for structural correctness.
//      - Validate built requirement objects for deterministic completeness.
//      - Maintain lifecycle structure: Initialize → Execute → Shutdown.
//      - Emit diagnostic trace statements for lifecycle and validation operations.
//
//  NON-RESPONSIBILITIES:
//      - Storing or constructing requirement definitions (handled by TC_RequirementDefinitions / TC_RequirementBuilder).
//      - Evaluating requirement logic or determining whether requirements are met.
//      - Managing persistence, loading, or saving operations.
//      - Mutating external engine state or performing runtime requirement logic.
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
    internal sealed class TC_RequirementsValidator
    {
        public void Initialize()
        {
            // Trace lifecycle initialization
            Trace.WriteLine("TC_RequirementsValidator: Initialize - Validator ready for requirement validation.");
        }

        public void Execute()
        {
            // Trace execution entry (no runtime operations required)
            Trace.WriteLine("TC_RequirementsValidator: Execute - No active runtime operations required.");
        }

        public void Shutdown()
        {
            // Trace teardown
            Trace.WriteLine("TC_RequirementsValidator: Shutdown - Validator teardown complete.");
        }

        public bool ValidateDefinition(RequirementDefinition definition)
        {
            // Trace validation request
            Trace.WriteLine($"TC_RequirementsValidator: ValidateDefinition - Validating requirement definition '{definition?.Key}'.");

            // Guard against null definition
            if (definition == null)
            {
                Trace.WriteLine("TC_RequirementsValidator: ValidateDefinition - Definition is null; validation failed.");
                return false;
            }

            // Inline comment: ensure key and description are non-empty
            if (string.IsNullOrWhiteSpace(definition.Key) ||
                string.IsNullOrWhiteSpace(definition.Description))
            {
                Trace.WriteLine("TC_RequirementsValidator: ValidateDefinition - Definition missing required fields; validation failed.");
                return false;
            }

            // Trace successful validation
            Trace.WriteLine($"TC_RequirementsValidator: ValidateDefinition - Definition '{definition.Key}' validated successfully.");
            return true;
        }

        public bool ValidateBuiltRequirement(BuiltRequirement requirement)
        {
            // Trace validation request
            Trace.WriteLine($"TC_RequirementsValidator: ValidateBuiltRequirement - Validating built requirement '{requirement?.Key}'.");

            // Guard against null requirement
            if (requirement == null)
            {
                Trace.WriteLine("TC_RequirementsValidator: ValidateBuiltRequirement - Built requirement is null; validation failed.");
                return false;
            }

            // Inline comment: ensure built requirement contains required fields
            if (string.IsNullOrWhiteSpace(requirement.Key) ||
                string.IsNullOrWhiteSpace(requirement.Description))
            {
                Trace.WriteLine("TC_RequirementsValidator: ValidateBuiltRequirement - Built requirement missing required fields; validation failed.");
                return false;
            }

            // Trace successful validation
            Trace.WriteLine($"TC_RequirementsValidator: ValidateBuiltRequirement - Built requirement '{requirement.Key}' validated successfully.");
            return true;
        }
    }
}
