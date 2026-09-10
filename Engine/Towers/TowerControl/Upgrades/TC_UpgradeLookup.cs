// =====================================================================================================
//  FILE: TC_UpgradeLookup.cs
//  PATH: Engine/Towers/TowerControl/Upgrades/TC_UpgradeLookup.cs
//  SUBSYSTEM: Towers TowerControl Upgrades Subsystem
//
//  ROLE:
//      Provides deterministic lookup operations for tower upgrades.
//      Acts as a read-only query surface over a supplied upgrade collection.
//      Supports lifecycle operations for initialization, execution entry, and shutdown.
//
//  RESPONSIBILITIES:
//      - Initialize with a read-only list of tower upgrades.
//      - Provide deterministic lookup by level.
//      - Provide deterministic lookup by index.
//      - Maintain lifecycle structure: Initialize → Execute → Shutdown.
//
//  NON-RESPONSIBILITIES:
//      - Performing cost calculations or upgrade progression logic.
//      - Managing persistence, file I/O, or external data loading.
//      - Validating upgrade correctness or enforcing upgrade rules.
//
//  ARCHITECTURAL NOTES:
//      - Operates strictly on provided upgrade data; does not mutate external systems.
//      - Shutdown clears internal references to ensure deterministic teardown behavior.
//      - Serves as a lightweight query layer for TowerControl upgrade subsystems.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Database
{
    internal sealed class TC_UpgradeLookup
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

        public TowerUpgrade GetByLevel(int level)
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

        public TowerUpgrade GetByIndex(int index)
        {
            if (_upgrades == null)
                return null;

            if (index < 0 || index >= _upgrades.Count)
                return null;

            return _upgrades[index];
        }

        public IReadOnlyList<TowerUpgrade> GetAll()
        {
            return _upgrades;
        }
    }
}
