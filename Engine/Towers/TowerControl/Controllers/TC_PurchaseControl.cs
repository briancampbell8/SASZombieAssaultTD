// =====================================================================================================
//  FILE: TC_PurchaseControl.cs
//  PATH: Engine/Towers/TowerControl/Controllers/TC_PurchaseControl.cs
//  SUBSYSTEM: Towers > TowerControl > Controllers
//
//  ROLE:
//      Contains all upgrade‑purchase and upgrade‑removal logic extracted directly from
//      NeuralManager.cs. This program centralizes deterministic purchase operations,
//      upgrade application, removal, and event triggering.
//
//  RESPONSIBILITIES:
//      - Purchase upgrades for towers.
//      - Apply purchased upgrades to tower instances.
//      - Remove upgrades from towers.
//      - Track purchased upgrades per tower.
//      - Trigger upgrade‑related events.
//
//  NON-RESPONSIBILITIES:
//      - Availability checks (TC_AvailabilityControl).
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

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Diagnostics;
using SASZombieAssaultTD.Engine.Managers;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Controllers
{
    public class TC_PurchaseControl
    {
        private readonly Dictionary<string, TowerUpgrade> _purchasedUpgrades;
        private readonly Dictionary<Tower, List<TowerUpgrade>> _towerUpgrades;

        public event Action<TowerUpgrade> OnUpgradePurchased;
        public event Action<TowerUpgrade> OnUpgradeApplied;
        public event Action<TowerUpgrade> OnUpgradeRemoved;
        public event Action<Tower> OnTowerUpgraded;

        public TC_PurchaseControl(
            Dictionary<string, TowerUpgrade> purchasedUpgrades,
            Dictionary<Tower, List<TowerUpgrade>> towerUpgrades)
        {
            _purchasedUpgrades = purchasedUpgrades;
            _towerUpgrades = towerUpgrades;
        }

        // Extracted from NeuralManager.PurchaseUpgrade()
        public bool PurchaseUpgrade(Tower tower, TowerUpgrade upgrade)
        {
            if (tower == null || upgrade == null)
                return false;

            var upgradeKey = $"{tower.Type}_{upgrade.Level}_{upgrade.Name}";
            if (_purchasedUpgrades.ContainsKey(upgradeKey))
                return false;

            var economyManager = EconomyManager.Instance;
            var playerCash = economyManager.CurrentCash;

            if (!upgrade.CanPurchase(playerCash, tower.Level))
            {
                PlayErrorSound("message");
                return false;
            }

            if (!upgrade.Purchase(playerCash))
                return false;

            upgrade.ApplyToTower(tower);

            _purchasedUpgrades[upgradeKey] = upgrade;

            if (!_towerUpgrades.ContainsKey(tower))
                _towerUpgrades[tower] = new List<TowerUpgrade>();

            _towerUpgrades[tower].Add(upgrade);

            OnUpgradePurchased?.Invoke(upgrade);
            OnUpgradeApplied?.Invoke(upgrade);
            OnTowerUpgraded?.Invoke(tower);

            Debug.WriteLine($"Purchased upgrade: {upgrade.Name} for {tower.Type}");
            return true;
        }

        // Extracted from NeuralManager.RemoveUpgrade()
        public bool RemoveUpgrade(Tower tower, TowerUpgrade upgrade)
        {
            if (tower == null || upgrade == null)
                return false;

            upgrade.RemoveFromTower(tower);

            var upgradeKey = $"{tower.Type}_{upgrade.Level}_{upgrade.Name}";
            _purchasedUpgrades.Remove(upgradeKey);

            if (_towerUpgrades.ContainsKey(tower))
                _towerUpgrades[tower].Remove(upgrade);

            OnUpgradeRemoved?.Invoke(upgrade);

            Debug.WriteLine($"Removed upgrade: {upgrade.Name} from {tower.Type}");
            return true;
        }

        // Extracted from NeuralManager.IsUpgradePurchased()
        public bool IsUpgradePurchased(Tower tower, TowerUpgrade upgrade)
        {
            if (tower == null || upgrade == null)
                return false;

            var upgradeKey = $"{tower.Type}_{upgrade.Level}_{upgrade.Name}";
            return _purchasedUpgrades.ContainsKey(upgradeKey);
        }

        // Extracted from NeuralManager.PlaySuccessSound()
        private void PlaySuccessSound(string soundName)
        {
            // ModernAudioSubsystem.PlaySound(soundName);
        }

        // Extracted from NeuralManager.PlayErrorSound()
        private void PlayErrorSound(string soundName)
        {
            // ModernAudioSubsystem.PlaySound(soundName);
        }
    }
}
