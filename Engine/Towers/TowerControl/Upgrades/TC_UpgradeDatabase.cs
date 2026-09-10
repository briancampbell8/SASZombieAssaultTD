// =====================================================================================================
//  FILE: TC_UpgradeDatabase.cs
//  PATH: Engine/Towers/TowerControl/Upgrades/TC_UpgradeDatabase.cs
//  SUBSYSTEM: Towers TowerControl Upgrades Subsystem
//
//  ROLE:
//      Provides deterministic loading, storage, and retrieval of tower upgrade data.
//      Supports lifecycle operations for initialization, execution entry, and shutdown.
//      Acts as the authoritative in-memory upgrade database for the TowerControl subsystem.
//
//  RESPONSIBILITIES:
//      - Load upgrade data from an external source or provided collection.
//      - Store upgrades in deterministic in-memory structures.
//      - Provide lookup operations for upgrades by level or type.
//      - Maintain lifecycle structure: Initialize → Execute → Shutdown.
//
//  NON-RESPONSIBILITIES:
//      - Performing cost calculations or upgrade application logic.
//      - Managing persistence formats, file I/O, or serialization rules.
//      - Validating upgrade correctness or enforcing upgrade progression rules.
//
//  ARCHITECTURAL NOTES:
//      - Operates strictly on provided upgrade data; does not mutate external systems.
//      - Shutdown clears internal references to ensure deterministic teardown behavior.
//      - Serves as a foundational data provider for other TowerControl upgrade programs.
// =====================================================================================================

using System.Collections.Generic;
namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Database
{
    public sealed class TC_UpgradeDatabase
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

        public TowerUpgrade GetUpgradeByLevel(int level)
        {
            if (_upgrades == null)
                return null;

            for (int i = 0; i < _upgrades.Count; i++)
            {
                if (_upgrades[i].Level == level)
                    return _upgrades[i];
            }

            return null;
        }

        public IReadOnlyList<TowerUpgrade> GetAllUpgrades()
        {
            return _upgrades;
        }
    }
}
