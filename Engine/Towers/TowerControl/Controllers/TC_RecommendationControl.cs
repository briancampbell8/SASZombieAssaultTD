// =====================================================================================================
//  FILE: TC_RecommendationControl.cs
//  PATH: Engine/Towers/TowerControl/Controllers/TC_RecommendationControl.cs
//  SUBSYSTEM: Towers > TowerControl > Controllers
//
//  ROLE:
//      Provides upgrade‑recommendation logic extracted directly from NeuralManager.cs.
//      This program centralizes deterministic upgrade‑ranking and selection operations.
//
//  RESPONSIBILITIES:
//      - Generate upgrade recommendations for a given tower.
//      - Rank available upgrades by efficiency rating.
//      - Return a limited number of top recommended upgrades.
//
//  NON-RESPONSIBILITIES:
//      - Availability checks (TC_AvailabilityControl).
//      - Purchasing or applying upgrades (TC_PurchaseControl).
//      - Cost or level queries (TC_CostQuery / TC_LevelQuery).
//      - Database initialization (TC_DatabaseControl).
//      - Saving or loading upgrade data (TC_SaveControl / TC_LoadControl).
//      - Upgrade path management (TC_PathControl).
//
//  ARCHITECTURAL NOTES:
//      - Fully populated using logic extracted from NeuralManager.cs.
//      - Declared public because TC_Manager exposes it through public properties.
//      - Contains no lifecycle contract.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Controllers
{
    public class TC_RecommendationControl
    {
        private readonly TC_AvailabilityControl _availabilityControl;

        public TC_RecommendationControl(TC_AvailabilityControl availabilityControl) => _availabilityControl = availabilityControl;

        // Extracted from NeuralManager.GetUpgradeRecommendations()
        public List<TowerUpgrade> GetUpgradeRecommendations(Tower tower, int count = 3)
        {
            if (tower == null)
                return new List<TowerUpgrade>();

            var availableUpgrades = _availabilityControl.GetAvailableUpgrades(tower);
            if (availableUpgrades.Count == 0)
                return new List<TowerUpgrade>();

            var sortedUpgrades = availableUpgrades
                .OrderByDescending(u => u.GetEfficiencyRating())
                .ToList();

            return sortedUpgrades.Take(count).ToList();
        }
    }
}
