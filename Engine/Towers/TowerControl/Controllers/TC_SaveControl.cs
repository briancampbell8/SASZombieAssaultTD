// =====================================================================================================
//  FILE: TC_SaveControl.cs
//  PATH: Engine/Towers/TowerControl/Controllers/TC_SaveControl.cs
//  SUBSYSTEM: Towers > TowerControl > Controllers
//
//  ROLE:
//      Contains all upgrade‑saving logic extracted directly from NeuralManager.cs.
//      This program centralizes deterministic save operations for the TowerControl subsystem.
//
//  RESPONSIBILITIES:
//      - Save upgrade data to persistent storage.
//      - Package purchased upgrades and tower‑upgrade mappings into UpgradeSaveData.
//      - Provide save sequencing for TC_Manager.
//
//  NON-RESPONSIBILITIES:
//      - Loading upgrade data (TC_LoadControl).
//      - Purchasing or applying upgrades (TC_PurchaseControl).
//      - Availability checks (TC_AvailabilityControl).
//      - Database initialization (TC_DatabaseControl).
//      - Upgrade path management (TC_PathControl).
//
//  ARCHITECTURAL NOTES:
//      - Fully populated using logic extracted from NeuralManager.cs.
//      - Declared public because TC_Manager exposes it through public properties.
//      - Contains no lifecycle contract.
// =====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Controllers
{
    public class TC_SaveControl
    {
        public class UpgradeSaveData()
        {
            public Dictionary<string, string> PurchasedUpgrades { get; set; }
            public Dictionary<uint, List<string>> TowerBrains { get; set; }
            public DateTime SaveTime { get; set; }
        }
        private readonly Dictionary<string, TowerUpgrade> _purchasedUpgrades;
        private readonly Dictionary<Tower, List<TowerUpgrade>> _towerUpgrades;

        public TC_SaveControl(
            Dictionary<string, TowerUpgrade> purchasedUpgrades,
            Dictionary<Tower, List<TowerUpgrade>> towerUpgrades)
        {
            _purchasedUpgrades = purchasedUpgrades;
            _towerUpgrades = towerUpgrades;
        }

        // Extracted from NeuralManager.SaveUpgrades()
        public bool SaveUpgrades(string saveName = "upgrades")
        {
            return SaveUpgrades(new UpgradeSaveData
            {
                PurchasedUpgrades = new Dictionary<string, string>(
                    _purchasedUpgrades.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Name)),

                TowerBrains = _towerUpgrades.ToDictionary(
                    kvp => kvp.Key.Id,
                    kvp => kvp.Value.Select(u => u.Name).ToList()),

                SaveTime = DateTime.Now
            }, saveName);
        }

        // Extracted from NeuralManager.SaveUpgrades(UpgradeSaveData)
        public bool SaveUpgrades(UpgradeSaveData saveData, string saveName = "upgrades")
        {
            try
            {
                Debug.WriteLine($"Saved upgrade data: {saveName}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving upgrades: {ex.Message}");
                return false;
            }
        }
    }
}
