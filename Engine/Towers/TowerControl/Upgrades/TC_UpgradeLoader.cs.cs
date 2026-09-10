// =====================================================================================================
//  FILE: TC_UpgradeLoader.cs
//  PATH: Engine/Towers/TowerControl/Upgrades/TC_UpgradeLoader.cs
//  SUBSYSTEM: Towers TowerControl Upgrades Subsystem
//
//  ROLE:
//      Loads tower upgrade data from an external or provided source into deterministic in-memory form.
//      Acts as the controlled entry point for upgrade data ingestion within the TowerControl subsystem.
//      Supports lifecycle operations for initialization, execution entry, and shutdown.
//
//  RESPONSIBILITIES:
//      - Load upgrade data from a supplied external provider or collection.
//      - Store loaded upgrades in deterministic internal structures.
//      - Provide access to the loaded upgrade set for downstream systems.
//      - Maintain lifecycle structure: Initialize → Execute → Shutdown.
//
//  NON-RESPONSIBILITIES:
//      - Performing cost calculations or upgrade progression logic.
//      - Managing persistence formats, file I/O, or serialization rules.
//      - Validating upgrade correctness or enforcing upgrade rules.
//
//  ARCHITECTURAL NOTES:
//      - Operates strictly on provided upgrade data; does not mutate external systems.
//      - Shutdown clears internal references to ensure deterministic teardown behavior.
//      - Serves as the controlled loading mechanism for TowerControl upgrade subsystems.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Database
{
    internal sealed class TC_UpgradeLoader
    {
        private IReadOnlyList<TowerUpgrade> _loadedUpgrades;

        public void Initialize(IReadOnlyList<TowerUpgrade> upgrades)
        {
            _loadedUpgrades = upgrades;
        }

        public void Execute()
        {
        }

        public void Shutdown()
        {
            _loadedUpgrades = null;
        }

        public IReadOnlyList<TowerUpgrade> GetLoadedUpgrades()
        {
            return _loadedUpgrades;
        }
    }
}
