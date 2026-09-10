// =====================================================================================================
//  FILE: TC_CostQuery.cs
//  PATH: Engine/Towers/TowerControl/Queries/TC_CostQuery.cs
//  SUBSYSTEM: Tower > TowerControl > Queries
//
//  ROLE:
//      Provides upgrade‑cost lookup logic extracted directly from NeuralManager.cs.
//      This program centralizes deterministic cost‑query operations for the TowerControl subsystem.
//
//  RESPONSIBILITIES:
//      - Return the cost of an upgrade at a specific level for a given TowerType.
//      - Query upgrade paths to locate matching upgrade entries.
//      - Provide cost‑query services to TC_Manager and other TowerControl modules.
//
//  NON-RESPONSIBILITIES:
//      - Performing availability checks (TC_AvailabilityControl).
//      - Purchasing or applying upgrades (TC_PurchaseControl).
//      - Saving or loading upgrade data (TC_SaveControl / TC_LoadControl).
//      - Database initialization (TC_DatabaseControl).
//      - Upgrade path management (TC_PathControl).
//
//  ARCHITECTURAL NOTES:
//      - Fully populated using logic extracted from NeuralManager.cs.
//      - Declared public because TC_Manager exposes it through public properties.
//      - Contains no lifecycle contract.
// =====================================================================================================

using System.Collections.Generic;
using System.Linq;
using static SASZombieAssaultTD.Engine.Towers.TowerEnums;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Queries
{
    public class TC_CostQuery
    {
        private readonly Dictionary<TowerType, List<TowerUpgrade>> _upgradePaths;

        public TC_CostQuery(Dictionary<TowerType, List<TowerUpgrade>> upgradePaths) => _upgradePaths = upgradePaths;

        // Extracted from NeuralManager.GetUpgradeCost()
        public int GetUpgradeCost(TowerType towerType, int level)
        {
            if (_upgradePaths.TryGetValue(towerType, out var upgradePath))
            {
                var upgrade = upgradePath.FirstOrDefault(u => u.Level == level);
                return upgrade?.Cost ?? 0;
            }

            return 0;
        }
    }
}
