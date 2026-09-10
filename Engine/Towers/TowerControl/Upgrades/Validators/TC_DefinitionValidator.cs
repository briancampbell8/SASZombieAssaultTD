// =====================================================================================================
//  FILE: TC_DefinitionValidator.cs
//  PATH: Engine/Towers/TowerControl/Upgrades/Validators/TC_DefinitionValidator.cs
//  SUBSYSTEM: Towers TowerControl Upgrades Validators Subsystem
//
//  ROLE:
//      Performs deterministic validation checks on tower upgrade definitions.
//      Ensures upgrade data meets required structural and logical constraints.
//      Supports lifecycle operations for initialization, execution entry, and shutdown.
//
//  RESPONSIBILITIES:
//      - Initialize with a read-only list of tower upgrades.
//      - Validate upgrade level ordering, cost validity, and definition consistency.
//      - Provide deterministic validation results for downstream systems.
//      - Maintain lifecycle structure: Initialize → Execute → Shutdown.
//
//  NON-RESPONSIBILITIES:
//      - Performing cost calculations or statistical analysis.
//      - Managing persistence, file I/O, or external data loading.
//      - Enforcing gameplay rules or applying upgrade effects.
//
//  ARCHITECTURAL NOTES:
//      - Operates strictly on provided upgrade definitions; does not mutate external systems.
//      - Shutdown clears internal references and validation results for deterministic teardown.
//      - Serves as a lightweight definition validation layer for TowerControl upgrade subsystems.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Upgrades.Validators
{
    internal sealed class TC_DefinitionValidator
    {
        private IReadOnlyList<TowerUpgrade> _upgrades;

        private bool _isValid;
        private string _lastError;

        public void Initialize(IReadOnlyList<TowerUpgrade> upgrades)
        {
            _upgrades = upgrades;
            Validate();
        }

        public void Execute()
        {
        }

        public void Shutdown()
        {
            _upgrades = null;
            _isValid = false;
            _lastError = null;
        }

        private void Validate()
        {
            if (_upgrades == null || _upgrades.Count == 0)
            {
                _isValid = false;
                _lastError = "No upgrades provided.";
                return;
            }

            int previousLevel = -1;

            for (int i = 0; i < _upgrades.Count; i++)
            {
                var upgrade = _upgrades[i];

                if (upgrade.Level < 0)
                {
                    _isValid = false;
                    _lastError = "Upgrade level cannot be negative.";
                    return;
                }

                if (upgrade.Cost < 0)
                {
                    _isValid = false;
                    _lastError = "Upgrade cost cannot be negative.";
                    return;
                }

                if (upgrade.Level <= previousLevel)
                {
                    _isValid = false;
                    _lastError = "Upgrade levels must be strictly increasing.";
                    return;
                }

                previousLevel = upgrade.Level;
            }

            _isValid = true;
            _lastError = null;
        }

        public bool IsValid() => _isValid;

        public string GetLastError() => _lastError;
    }
}
