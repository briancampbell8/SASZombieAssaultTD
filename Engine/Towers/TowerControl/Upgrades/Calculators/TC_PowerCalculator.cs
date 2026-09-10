// =====================================================================================================
//  FILE: TC_PowerCalculator.cs
//  PATH: Engine/Towers/TowerControl/Upgrades/Calculators/TC_PowerCalculator.cs
//  SUBSYSTEM: Towers TowerControl Upgrades Calculators Subsystem
//
//  ROLE:
//      Provides deterministic power metric evaluation for tower upgrades.
//      Computes direct and aggregated power values (e.g., raw damage contribution,
//      multiplier-based power scaling, or composite power scores) using provided upgrade data.
//      Supports initialization, execution entry, and shutdown to align with engine lifecycle flow.
//
//  RESPONSIBILITIES:
//      - Initialize with a read-only list of tower upgrades.
//      - Compute deterministic power metrics for individual upgrades.
//      - Compute aggregated power metrics across all upgrades.
//      - Maintain lifecycle structure: Initialize → Execute → Shutdown.
//      - Emit diagnostic trace statements for lifecycle and calculation operations.
//
//  NON-RESPONSIBILITIES:
//      - Performing cost calculations or performance metric analysis.
//      - Validating upgrade definitions or enforcing upgrade rules.
//      - Managing persistence, loading, or saving operations.
//      - Applying gameplay effects or modifying external engine state.
//
//  ARCHITECTURAL NOTES:
//      - Operates strictly on provided upgrade data; does not mutate external systems.
//      - Follows the engine-hosted lifecycle pattern for consistency across TowerControl subsystems.
//      - Shutdown clears internal references to maintain deterministic teardown behavior.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Diagnostics;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Upgrades.Calculators
{
    internal sealed class TC_PowerCalculator
    {
        private IReadOnlyList<TowerUpgrade> _upgrades;

        public void Initialize(IReadOnlyList<TowerUpgrade> upgrades)
        {
            // Trace lifecycle initialization
            Trace.WriteLine("TC_PowerCalculator: Initialize - Binding upgrade list for power evaluation.");

            // Store provided upgrade list
            _upgrades = upgrades;
        }

        public void Execute()
        {
            // Trace execution entry (no runtime operations required)
            Trace.WriteLine("TC_PowerCalculator: Execute - No active runtime operations required.");
        }

        public void Shutdown()
        {
            // Trace teardown
            Trace.WriteLine("TC_PowerCalculator: Shutdown - Clearing upgrade list reference.");

            // Clear internal state
            _upgrades = null;
        }

        public float GetUpgradePower(int level)
        {
            // Trace power lookup request
            Trace.WriteLine($"TC_PowerCalculator: GetUpgradePower - Requesting power for level {level}.");

            // Guard against uninitialized state
            if (_upgrades == null)
            {
                Trace.WriteLine("TC_PowerCalculator: GetUpgradePower - Upgrade list is null; returning 0.");
                return 0f;
            }

            // Iterate through upgrades to find matching level
            for (int i = 0; i < _upgrades.Count; i++)
            {
                var upgrade = _upgrades[i];

                // Check for matching upgrade level
                if (upgrade.Level == level)
                {
                    // Inline comment: compute power using upgrade's defined metrics
                    float power = ComputePower(upgrade);

                    Trace.WriteLine($"TC_PowerCalculator: GetUpgradePower - Computed power {power} for level {level}.");
                    return power;
                }
            }

            // No matching level found
            Trace.WriteLine($"TC_PowerCalculator: GetUpgradePower - No upgrade found for level {level}; returning 0.");
            return 0f;
        }

        public float GetTotalPower()
        {
            // Trace total power request
            Trace.WriteLine("TC_PowerCalculator: GetTotalPower - Calculating total aggregated power.");

            // Guard against uninitialized state
            if (_upgrades == null)
            {
                Trace.WriteLine("TC_PowerCalculator: GetTotalPower - Upgrade list is null; returning 0.");
                return 0f;
            }

            float total = 0f;

            // Inline comment: accumulate power across all upgrades
            for (int i = 0; i < _upgrades.Count; i++)
            {
                total += ComputePower(_upgrades[i]);
            }

            // Trace final total
            Trace.WriteLine($"TC_PowerCalculator: GetTotalPower - Total power computed: {total}.");

            return total;
        }

        private float ComputePower(TowerUpgrade upgrade)
        {
            // Inline comment: placeholder deterministic power formula
            // This will be replaced once NeuralNet-derived power metrics are integrated.
            float baseValue = upgrade.Cost * 0.5f; // Example placeholder metric

            // Trace internal computation
            Trace.WriteLine($"TC_PowerCalculator: ComputePower - Using placeholder power metric based on cost {upgrade.Cost}.");

            return baseValue;
        }
    }
}
