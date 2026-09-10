// =====================================================================================================
//  FILE: TC_EffectsValidator.cs
//  PATH: Engine/Towers/TowerControl/Upgrades/Validators/TC_EffectsValidator.cs
//  SUBSYSTEM: Towers/TowerControl/Upgrades/Validators
//
//  ROLE:
//      Provides deterministic validation of upgrade effect definitions and built effect objects.
//      Ensures that effect definitions are structurally valid and that constructed effects meet
//      minimal deterministic requirements before being consumed by downstream systems.
//      Supports initialization, execution entry, and shutdown to align with engine lifecycle flow.
//
//  RESPONSIBILITIES:
//      - Initialize validator state if required.
//      - Validate effect definitions for structural correctness.
//      - Validate built effect objects for minimal deterministic completeness.
//      - Maintain lifecycle structure: Initialize → Execute → Shutdown.
//      - Emit diagnostic trace statements for lifecycle and validation operations.
//
//  NON-RESPONSIBILITIES:
//      - Storing or constructing effect definitions (handled by TC_EffectDefinitions / TC_EffectBuilder).
//      - Applying effects, triggering animations, or performing rendering operations.
//      - Managing persistence, loading, or saving operations.
//      - Mutating external engine state or performing runtime effect logic.
//
//  ARCHITECTURAL NOTES:
//      - Operates strictly as a validator; does not mutate external engine state.
//      - Follows the engine-hosted lifecycle pattern for consistency across TowerControl subsystems.
//      - Shutdown clears internal references to maintain deterministic teardown behavior.
// =====================================================================================================

using System.Diagnostics;
using SASZombieAssaultTD.Engine.Towers.TowerControl.Upgrades.Definitions;
using SASZombieAssaultTD.Engine.Towers.TowerControl.Upgrades.Effects;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Upgrades.Validators
{
    internal sealed class TC_EffectsValidator
    {
        public void Initialize()
        {
            // Trace lifecycle initialization
            Trace.WriteLine("EffectsValidator: Initialize - Validator ready for effect validation.");
        }

        public void Execute()
        {
            // Trace execution entry (no runtime operations required)
            Trace.WriteLine("EffectsValidator: Execute - No active runtime operations required.");
        }

        public void Shutdown()
        {
            // Trace teardown
            Trace.WriteLine("EffectsValidator: Shutdown - Validator teardown complete.");
        }

        public bool ValidateDefinition(EffectDefinition definition)
        {
            // Trace validation request
            Trace.WriteLine($"TC_EffectsValidator: ValidateDefinition - " +
                $"Validating effect definition '{definition?.Key}'.");

            // Guard against null definition
            if (definition == null)
            {
                Trace.WriteLine("TC_EffectsValidator: ValidateDefinition - " +
                    "Definition is null; validation failed.");
                return false;
            }

            // Inline comment: ensure key and description are non-empty
            if (string.IsNullOrWhiteSpace(definition.Key) ||
                string.IsNullOrWhiteSpace(definition.Description))
            {
                Trace.WriteLine("TC_EffectsValidator: ValidateDefinition - " +
                    "Definition missing required fields; validation failed.");
                return false;
            }

            // Trace successful validation
            Trace.WriteLine($"TC_EffectsValidator: ValidateDefinition - " +
                $"Definition '{definition.Key}' validated successfully.");
            return true;
        }

        public bool ValidateBuiltEffect(BuiltEffect effect)
        {
            // Trace validation request
            Trace.WriteLine($"TC_EffectsValidator: ValidateBuiltEffect - Validating built effect '{effect?.Key}'.");

            // Guard against null effect
            if (effect == null)
            {
                Trace.WriteLine("TC_EffectsValidator: ValidateBuiltEffect - Built effect is null; validation failed.");
                return false;
            }

            // Inline comment: ensure built effect contains required fields
            if (string.IsNullOrWhiteSpace(effect.Key) ||
                string.IsNullOrWhiteSpace(effect.Description))
            {
                Trace.WriteLine("TC_EffectsValidator: ValidateBuiltEffect - " +
                    "Built effect missing required fields; validation failed.");
                return false;
            }

            // Trace successful validation
            Trace.WriteLine($"TC_EffectsValidator: ValidateBuiltEffect -" +
                $" Built effect '{effect.Key}' validated successfully.");
            return true;
        }
    }
}
