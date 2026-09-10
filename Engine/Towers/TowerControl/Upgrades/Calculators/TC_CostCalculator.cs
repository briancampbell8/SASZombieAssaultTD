// =====================================================================================================
//  FILE: CostCalculator.cs
//  PATH: Engine/Towers/TowerControl/Upgrades/Calculators/CostCalculator.cs
//  SUBSYSTEM: Towers TowerControl Upgrades Subsystem
//
//  ROLE:
//      Provides deterministic cost evaluation for tower upgrades using a supplied upgrade list.
//      Supports initialization, execution entry, and shutdown to align with engine lifecycle flow.
//
//  RESPONSIBILITIES:
//      - Initialize with a read-only list of tower upgrades.
//      - Provide deterministic cost lookup for a specific upgrade level.
//      - Provide deterministic total cost calculation across all upgrades.
//      - Maintain lifecycle structure: Initialize → Execute → Shutdown.
//
//  NON-RESPONSIBILITIES:
//      - Performing upgrade validation or enforcing upgrade rules.
//      - Managing upgrade persistence, loading, or saving operations.
//      - Modifying upgrade data or applying dynamic cost modifiers.
//
//  ARCHITECTURAL NOTES:
//      - Follows the engine-hosted lifecycle pattern for consistency.
//      - Operates strictly on provided upgrade data; does not fetch or mutate external state.
//      - Shutdown clears internal references to maintain deterministic teardown behavior.
// =====================================================================================================
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Upgrades
{
    internal sealed class TC_CostCalculator
    {
        private IReadOnlyList<TowerUpgrade> _upgrades;

        public void Initialize(IReadOnlyList<TowerUpgrade> upgrades)
        {
            _upgrades = upgrades;
        }

        public void Execute()
        {
        }

        public void Shutdown()
        {
            _upgrades = null;
        }

        public int GetUpgradeCost(int level)
        {
            if (_upgrades == null)
                return 0;

            for (int i = 0; i < _upgrades.Count; i++)
            {
                if (_upgrades[i].Level == level)
                    return _upgrades[i].Cost;
            }

            return 0;
        }

        public int GetTotalUpgradeCost()
        {
            if (_upgrades == null)
                return 0;

            int total = 0;

            for (int i = 0; i < _upgrades.Count; i++)
            {
                total += _upgrades[i].Cost;
            }

            return total;
        }
    }
}
