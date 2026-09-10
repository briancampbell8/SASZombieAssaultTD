// =====================================================================================================
//  FILE: TC_UpgradeDefaults.cs
//  PATH: Engine/Towers/TowerControl/Upgrades/TC_UpgradeDefaults.cs
//  SUBSYSTEM: Towers TowerControl Upgrades Subsystem
//
//  ROLE:
//      Provides deterministic default upgrade definitions for towers.
//      Supplies baseline upgrade values used when no explicit upgrade data is provided.
//      Supports lifecycle operations for initialization, execution entry, and shutdown.
//
//  RESPONSIBILITIES:
//      - Initialize and store a deterministic set of default tower upgrades.
//      - Provide lookup access to default upgrades by level.
//      - Maintain lifecycle structure: Initialize → Execute → Shutdown.
//
//  NON-RESPONSIBILITIES:
//      - Performing cost calculations or dynamic upgrade scaling.
//      - Managing persistence, file I/O, or external data loading.
//      - Validating upgrade correctness or enforcing upgrade progression rules.
//
//  ARCHITECTURAL NOTES:
//      - Operates strictly on internally defined default data.
//      - Shutdown clears internal references to ensure deterministic teardown behavior.
//      - Serves as a fallback upgrade provider for TowerControl systems.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Database
{
    internal sealed class TC_UpgradeDefaults
    {
        private IReadOnlyList<TowerUpgrade> _defaultUpgrades;

        public void Initialize(IReadOnlyList<TowerUpgrade> defaults)
        {
            _defaultUpgrades = defaults;
        }

        public void Execute()
        {
        }

        public void Shutdown()
        {
            _defaultUpgrades = null;
        }

        public TowerUpgrade GetDefaultUpgrade(int level)
        {
            if (_defaultUpgrades == null)
                return null;

            for (int i = 0; i < _defaultUpgrades.Count; i++)
            {
                if (_defaultUpgrades[i].Level == level)
                    return _defaultUpgrades[i];
            }

            return null;
        }

        public IReadOnlyList<TowerUpgrade> GetAllDefaults()
        {
            return _defaultUpgrades;
        }
    }
}
