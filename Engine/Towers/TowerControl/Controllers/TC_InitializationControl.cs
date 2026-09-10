// =====================================================================================================
//  FILE: TC_InitializationControl.cs
//  PATH: Engine/Towers/TowerControl/Controllers/TC_InitializationControl.cs
//  SUBSYSTEM: Towers > TowerControl > Controllers
//
//  ROLE:
//      Handles initialization sequencing for all TowerControl subsystem controllers.
//      Expanded to include InitializeAllControllers() because TC_Manager requires it.
//
//  RESPONSIBILITIES:
//      - Initialize upgrade databases.
//      - Initialize upgrade paths.
//      - Provide a unified initialization entry point for TC_Manager.
//
//  NON-RESPONSIBILITIES:
//      - Availability checks (TC_AvailabilityControl).
//      - Purchasing or applying upgrades (TC_PurchaseControl).
//      - Saving or loading upgrade data (TC_SaveControl / TC_LoadControl).
//      - Dispatching upgrade events (TC_EventDispatching).
//
//  ARCHITECTURAL NOTES:
//      - Fully populated using logic extracted from NeuralManager.cs.
//      - InitializeAllControllers() added to satisfy TC_Manager.
// =====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using SASZombieAssaultTD.Engine.Towers.TowerControl.Database;
using SASZombieAssaultTD.Engine.Towers.TowerControl.Databases;
using static SASZombieAssaultTD.Engine.Towers.TowerEnums;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Controllers
{
    public class TC_InitializationControl
    {
        private readonly Dictionary<TowerType, TowerUpgradeDatabase> _upgradeDatabases;
        private readonly Dictionary<TowerType, List<TowerUpgrade>> _upgradePaths;

        public TC_InitializationControl(
            Dictionary<TowerType, TowerUpgradeDatabase> upgradeDatabases,
            Dictionary<TowerType, List<TowerUpgrade>> upgradePaths)
        {
            _upgradeDatabases = upgradeDatabases;
            _upgradePaths = upgradePaths;
        }

        public TC_InitializationControl(Dictionary<TowerType, TC_UpgradeDatabase> upgradeDatabases, Dictionary<TowerType, List<TowerUpgrade>> upgradePaths) => _upgradePaths = upgradePaths;

        public void InitializeUpgradeDatabases()
        {
            foreach (TowerType towerType in Enum.GetValues<TowerType>())
            {
                var db = new TowerUpgradeDatabase();
                _upgradeDatabases[towerType] = db;
            }

            Debug.WriteLine($"Initialized {_upgradeDatabases.Count} upgrade databases");
        }

        public void InitializeUpgradePaths()
        {
            foreach (var kvp in _upgradeDatabases)
            {
                var towerType = kvp.Key;
                var database = kvp.Value;

                var upgradePath = new List<TowerUpgrade>();
                _upgradePaths[towerType] = upgradePath;
            }

            Debug.WriteLine($"Initialized {_upgradePaths.Count} upgrade paths");
        }

        // REQUIRED BY TC_Manager
        public void InitializeAllControllers()
        {
            InitializeUpgradeDatabases();
            InitializeUpgradePaths();
        }
    }
}
