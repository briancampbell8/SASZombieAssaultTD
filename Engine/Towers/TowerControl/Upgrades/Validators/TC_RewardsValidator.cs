// =====================================================================================================
//  FILE: TC_RewardsValidator.cs
//  PATH: Engine/Towers/TowerControl/Upgrades/Validators/TC_RewardsValidator.cs
//  SUBSYSTEM: Towers/TowerControl/Upgrades/Validators
//
//  ROLE:
//      Provides deterministic validation of upgrade reward definitions and built reward objects.
//      Ensures that reward definitions are structurally valid and that constructed rewards meet
//      minimal deterministic standards before being consumed by downstream systems.
//      Supports initialization, execution entry, and shutdown to align with engine lifecycle flow.
//
//  RESPONSIBILITIES:
//      - Initialize validator state if required.
//      - Validate reward definitions for structural correctness.
//      - Validate built reward objects for deterministic completeness.
//      - Maintain lifecycle structure: Initialize → Execute → Shutdown.
//      - Emit diagnostic trace statements for lifecycle and validation operations.
//
//  NON-RESPONSIBILITIES:
//      - Storing or constructing reward definitions (handled by TC_RewardDefinitions / TC_RewardBuilder).
//      - Applying rewards or modifying tower/player state.
//      - Managing persistence, loading, or saving operations.
//      - Mutating external engine state or performing runtime reward logic.
//
//  ARCHITECTURAL NOTES:
//      - Operates strictly as a validator; does not mutate external engine state.
//      - Follows the engine-hosted lifecycle pattern for consistency across TowerControl subsystems.
//      - Shutdown clears internal references to maintain deterministic teardown behavior.
// =====================================================================================================

using System.Diagnostics;
using SASZombieAssaultTD.Engine.Towers.TowerControl.Upgrades.Definitions;
using SASZombieAssaultTD.Engine.Towers.TowerControl.Upgrades.Rewards;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Upgrades.Validators
{
    internal sealed class TC_RewardsValidator
    {
        public void Initialize()
        {
            // Trace lifecycle initialization
            Trace.WriteLine("TC_RewardsValidator: Initialize - Validator ready for reward validation.");
        }

        public void Execute()
        {
            // Trace execution entry (no runtime operations required)
            Trace.WriteLine("TC_RewardsValidator: Execute - No active runtime operations required.");
        }

        public void Shutdown()
        {
            // Trace teardown
            Trace.WriteLine("TC_RewardsValidator: Shutdown - Validator teardown complete.");
        }

        public bool ValidateDefinition(RewardDefinition definition)
        {
            // Trace validation request
            Trace.WriteLine($"TC_RewardsValidator: ValidateDefinition - Validating reward definition '{definition?.Key}'.");

            // Guard against null definition
            if (definition == null)
            {
                Trace.WriteLine("TC_RewardsValidator: ValidateDefinition - Definition is null; validation failed.");
                return false;
            }

            // Inline comment: ensure key and description are non-empty
            if (string.IsNullOrWhiteSpace(definition.Key) ||
                string.IsNullOrWhiteSpace(definition.Description))
            {
                Trace.WriteLine("TC_RewardsValidator: ValidateDefinition - Definition missing required fields; validation failed.");
                return false;
            }

            // Trace successful validation
            Trace.WriteLine($"TC_RewardsValidator: ValidateDefinition - Definition '{definition.Key}' validated successfully.");
            return true;
        }

        public bool ValidateBuiltReward(BuiltReward reward)
        {
            // Trace validation request
            Trace.WriteLine($"TC_RewardsValidator: ValidateBuiltReward - Validating built reward '{reward?.Key}'.");

            // Guard against null reward
            if (reward == null)
            {
                Trace.WriteLine("TC_RewardsValidator: ValidateBuiltReward - Built reward is null; validation failed.");
                return false;
            }

            // Inline comment: ensure built reward contains required fields
            if (string.IsNullOrWhiteSpace(reward.Key) ||
                string.IsNullOrWhiteSpace(reward.Description))
            {
                Trace.WriteLine("TC_RewardsValidator: ValidateBuiltReward - Built reward missing required fields; validation failed.");
                return false;
            }

            // Trace successful validation
            Trace.WriteLine($"TC_RewardsValidator: ValidateBuiltReward - Built reward '{reward.Key}' validated successfully.");
            return true;
        }
    }
}
