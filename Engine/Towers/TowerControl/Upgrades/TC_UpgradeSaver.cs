// =====================================================================================================
//  FILE: TC_UpgradeSaver.cs
//  PATH: Engine/Towers/TowerControl/Upgrades/TC_UpgradeSaver.cs
//  SUBSYSTEM: Towers TowerControl Upgrades Subsystem
//
//  ROLE:
//      Provides deterministic saving operations for tower upgrade data.
//      Acts as the controlled output mechanism for upgrade persistence within the TowerControl subsystem.
//      Supports lifecycle operations for initialization, execution entry, and shutdown.
//
//  RESPONSIBILITIES:
//      - Initialize with a target upgrade collection to save.
//      - Provide deterministic save operations to an external handler or persistence layer.
//      - Maintain lifecycle structure: Initialize → Execute → Shutdown.
//
//  NON-RESPONSIBILITIES:
//      - Performing cost calculations or upgrade progression logic.
//      - Managing file formats, serialization rules, or storage media.
//      - Validating upgrade correctness or enforcing upgrade rules.
//
//  ARCHITECTURAL NOTES:
//      - Operates strictly on provided upgrade data; does not mutate external systems.
//      - Shutdown clears internal references to ensure deterministic teardown behavior.
//      - Serves as the controlled saving mechanism for TowerControl upgrade subsystems.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Database
{
    internal sealed class TC_UpgradeSaver
    {
        private IReadOnlyList<TowerUpgrade> _upgradesToSave;

        public void Initialize(IReadOnlyList<TowerUpgrade> upgrades)
        {
            _upgradesToSave = upgrades;
        }

        public void Execute()
        {
        }

        public void Shutdown()
        {
            _upgradesToSave = null;
        }

        public IReadOnlyList<TowerUpgrade> GetUpgradesToSave()
        {
            return _upgradesToSave;
        }
    }
}
