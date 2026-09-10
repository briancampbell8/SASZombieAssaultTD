// =====================================================================================================
//  FILE: TC_PathControl.cs
//  PATH: Engine/Towers/TowerControl/Controllers/TC_PathControl.cs
//  SUBSYSTEM: Towers > TowerControl > Controllers
//
//  ROLE:
//      Manages upgrade path storage and retrieval for the TowerControl subsystem.
//      All upgrade-path logic extracted directly from NeuralManager.cs is centralized here.
//
//  RESPONSIBILITIES:
//      - Maintain the mapping of TowerType → List<TowerUpgrade> upgrade paths.
//      - Provide upgrade-path retrieval services to TC_Manager and other controllers.
//      - Support initialization sequencing via TC_InitializationControl.
//
//  NON-RESPONSIBILITIES:
//      - Creating upgrade databases (TC_DatabaseControl).
//      - Performing availability checks (TC_AvailabilityControl).
//      - Purchasing or applying upgrades (TC_PurchaseControl).
//      - Saving or loading upgrade data (TC_SaveControl / TC_LoadControl).
//      - Dispatching upgrade events (TC_EventDispatching).
//
//  ARCHITECTURAL NOTES:
//      - Fully populated using logic extracted from NeuralManager.cs.
//      - Declared public because TC_Manager exposes it through public properties.
//      - Contains no lifecycle contract.
// =====================================================================================================

using System.Collections.Generic;
using static SASZombieAssaultTD.Engine.Towers.TowerEnums;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Controllers
{
    public class TC_PathControl
    {
        private readonly Dictionary<TowerType, List<TowerUpgrade>> _upgradePaths;

        public TC_PathControl(Dictionary<TowerType, List<TowerUpgrade>> upgradePaths) => _upgradePaths = upgradePaths;



        // Extracted from NeuralManager.GetUpgradePath()
        public List<TowerUpgrade> GetUpgradePath(TowerType towerType)
        {
            return _upgradePaths.TryGetValue(towerType, out var path)
                ? path
                : new List<TowerUpgrade>();
        }

        // Exposes internal dictionary for TC_Manager wiring
        public IReadOnlyDictionary<TowerType, List<TowerUpgrade>> Paths
            => _upgradePaths;
    }
}
