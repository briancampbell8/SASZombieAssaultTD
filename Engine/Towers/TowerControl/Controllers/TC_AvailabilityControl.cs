// =====================================================================================================
//  FILE: TC_AvailabilityControl.cs
//  PATH: Engine/Towers/TowerControl/Controllers/TC_AvailabilityControl.cs
//  SUBSYSTEM: Towers > TowerControl > Controllers
//
//  ROLE:
//      Provides deterministic upgrade availability evaluation for towers.
//      This controller extracts and centralizes all availability logic previously
//      contained inside NeuralManager, ensuring clean subsystem separation.
//
//  RESPONSIBILITIES:
//      - Determine which upgrades are available for a given tower.
//      - Enforce availability rules such as IsAvailable flags and purchased-state checks.
//      - Provide availability results to TC_Manager and other TowerControl modules.
//      - Maintain deterministic rule enforcement for upgrade gating.
//
//  NON-RESPONSIBILITIES:
//      - Performing cost calculations (handled by TC_CostQuery / TC_UpgradeCostCalculator).
//      - Performing level queries (handled by TC_LevelQuery).
//      - Purchasing or applying upgrades (handled by TC_PurchaseControl).
//      - Saving or loading upgrade data (handled by TC_SaveControl / TC_LoadControl).
//      - Dispatching upgrade events (handled by TC_EventDispatching).
//
//  ARCHITECTURAL NOTES:
//      - This NEW PROGRAM shell has now been fully populated using logic extracted
//        directly from NeuralManager.cs.
//      - Declared public because TC_Manager exposes it through public properties.
//      - Contains no lifecycle contract; TowerControl controllers do not implement
//        Initialize → Run → Shutdown patterns.
// =====================================================================================================

using System.Collections.Generic;
using System.Linq;
using static SASZombieAssaultTD.Engine.Towers.TowerEnums;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Controllers
{
    public class TC_AvailabilityControl
    {
        private readonly Dictionary<TowerType, List<TowerUpgrade>> _upgradePaths;
        private readonly Dictionary<Tower, List<TowerUpgrade>> _towerUpgrades;

        public TC_AvailabilityControl(
            Dictionary<TowerType, List<TowerUpgrade>> upgradePaths,
            Dictionary<Tower, List<TowerUpgrade>> towerUpgrades)
        {
            _upgradePaths = upgradePaths;
            _towerUpgrades = towerUpgrades;
        }

        /// <summary>
        /// Extracted from NeuralManager.GetAvailableUpgrades()
        /// </summary>
        public List<TowerUpgrade> GetAvailableUpgrades(Tower tower)
        {
            if (tower == null)
                return new List<TowerUpgrade>();

            var availableUpgrades = new List<TowerUpgrade>();
            var towerType = tower.Type;

            if (_upgradePaths.TryGetValue(towerType, out var upgradePath))
            {
                foreach (var upgrade in upgradePath)
                {
                    if (upgrade.IsAvailable && !IsUpgradePurchased(tower, upgrade))
                    {
                        availableUpgrades.Add(upgrade);
                    }
                }
            }

            return availableUpgrades;
        }

        /// <summary>
        /// Extracted from NeuralManager.GetPurchasedUpgrades()
        /// </summary>
        public List<TowerUpgrade> GetPurchasedUpgrades(Tower tower)
        {
            if (tower == null)
                return new List<TowerUpgrade>();

            return _towerUpgrades.TryGetValue(tower, out var upgrades)
                ? upgrades
                : new List<TowerUpgrade>();
        }

        /// <summary>
        /// Extracted from NeuralManager.IsUpgradePurchased()
        /// </summary>
        private bool IsUpgradePurchased(Tower tower, TowerUpgrade upgrade)
        {
            if (tower == null || upgrade == null)
                return false;

            var upgradeKey = $"{tower.Type}_{upgrade.Level}_{upgrade.Name}";

            // Purchased upgrades are stored per tower
            if (_towerUpgrades.TryGetValue(tower, out var upgrades))
            {
                return upgrades.Any(u =>
                    $"{tower.Type}_{u.Level}_{u.Name}" == upgradeKey);
            }

            return false;
        }
    }
}
