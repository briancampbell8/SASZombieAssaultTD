// =====================================================================================================
//  FILE: TC_DatabaseControl.cs
//  PATH: Engine/Towers/TowerControl/Controllers/TC_DatabaseControl.cs
//  SUBSYSTEM: Towers > TowerControl > Controllers
//
//  ROLE:
//      Centralizes upgrade database creation and retrieval for the TowerControl subsystem.
//      This program contains all database logic extracted directly from NeuralManager.cs.
//
//  RESPONSIBILITIES:
//      - Create and initialize upgrade databases for each TowerType.
//      - Maintain the mapping of TowerType → TowerUpgradeDatabase.
//      - Provide database retrieval services to TC_Manager and other TowerControl modules.
//
//  NON-RESPONSIBILITIES:
//      - Managing upgrade paths (TC_PathControl).
//      - Performing availability checks (TC_AvailabilityControl).
//      - Purchasing or applying upgrades (TC_PurchaseControl).
//      - Saving or loading upgrade data (TC_SaveControl / TC_LoadControl).
//      - Dispatching upgrade events (TC_EventDispatching).
//
//  ARCHITECTURAL NOTES:
//      - This NEW PROGRAM shell is now fully populated using logic extracted from NeuralManager.cs.
//      - Declared public because TC_Manager exposes it through public properties.
//      - Contains no lifecycle contract.
// =====================================================================================================

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Towers.TowerControl.Databases;
using static SASZombieAssaultTD.Engine.Towers.TowerEnums;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Controllers
{
    public class TC_DatabaseControl
    {
        private readonly Dictionary<TowerType, TowerUpgradeDatabase> _upgradeDatabases;

        public TC_DatabaseControl() => _upgradeDatabases = new Dictionary<TowerType, TowerUpgradeDatabase>();

        // Extracted from NeuralManager.InitializeUpgradeDatabases()
        public void InitializeUpgradeDatabases()
        {
            foreach (TowerType towerType in Enum.GetValues<TowerType>())
            {
                var db = new TowerUpgradeDatabase();
                _upgradeDatabases[towerType] = db;
            }

            System.Diagnostics.Debug.WriteLine(
                $"Initialized {_upgradeDatabases.Count} upgrade databases");
        }

        // Extracted from NeuralManager.GetUpgradeDatabase()
        public TowerUpgradeDatabase GetUpgradeDatabase(TowerType towerType)
        {
            return _upgradeDatabases.TryGetValue(towerType, out var database)
                ? database
                : null;
        }

        // Exposes internal dictionary for TC_Manager wiring
        public IReadOnlyDictionary<TowerType, TowerUpgradeDatabase> Databases
            => _upgradeDatabases;
    }
}
