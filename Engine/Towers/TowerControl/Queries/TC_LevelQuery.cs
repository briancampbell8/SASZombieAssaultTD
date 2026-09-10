// =====================================================================================================
//  FILE: TC_LevelQuery.cs
//  PATH: Engine/Towers/TowerControl/Queries/TC_LevelQuery.cs
//  SUBSYSTEM: Tower > TowerControl > Queries
//
//  ROLE:
//      Provides upgrade‑level lookup logic extracted directly from NeuralManager.cs.
//      This program centralizes deterministic level‑query operations for the TowerControl subsystem.
//
//  RESPONSIBILITIES:
//      - Return the maximum upgrade level for a given TowerType.
//      - Query upgrade paths to determine highest defined upgrade level.
//      - Provide level‑query services to TC_Manager and other TowerControl modules.
//
//  NON-RESPONSIBILITIES:
//      - Performing cost queries (TC_CostQuery).
//      - Availability checks (TC_AvailabilityControl).
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
    public class TC_LevelQuery
    {
        private readonly Dictionary<TowerType, List<TowerUpgrade>> _upgradePaths;

        public TC_LevelQuery(Dictionary<TowerType, List<TowerUpgrade>> upgradePaths) => _upgradePaths = upgradePaths;

        // Extracted from NeuralManager.GetMaxUpgradeLevel()
        public int GetMaxUpgradeLevel(TowerType towerType)
        {
            if (_upgradePaths.TryGetValue(towerType, out var upgradePath))
            {
                return upgradePath.Count > 0
                    ? upgradePath.Max(u => u.Level)
                    : 1;
            }

            return 1;
        }
    }
}
