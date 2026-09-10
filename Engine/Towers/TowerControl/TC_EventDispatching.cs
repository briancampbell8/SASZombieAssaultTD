// =====================================================================================================
//  FILE: TC_EventDispatching.cs
//  PATH: Engine/Towers/TowerControl/TC_EventDispatching.cs
//  SUBSYSTEM: Platform Abstraction Layer
//
//  ROLE:
//      Centralized event hub for all TowerControl upgrade‑related events.
//      Extracted directly from NeuralManager.cs and expanded to include the
//      initialization entry point required by TC_Manager.
//
//  RESPONSIBILITIES:
//      - Expose upgrade‑related events (purchase, apply, remove, tower upgraded).
//      - Provide deterministic dispatching methods for all upgrade events.
//      - Provide Initialize() so TC_Manager can perform controller‑level startup.
//
//  NON-RESPONSIBILITIES:
//      - Performing upgrade purchases (TC_PurchaseControl).
//      - Performing availability checks (TC_AvailabilityControl).
//      - Managing upgrade paths (TC_PathControl).
//      - Saving or loading upgrade data (TC_SaveControl / TC_LoadControl).
//      - Database initialization (TC_DatabaseControl).
//
//  ARCHITECTURAL NOTES:
//      - Fully populated using event definitions extracted from NeuralManager.cs.
//      - Initialize() added because TC_Manager requires it.
//      - Declared public because TC_Manager exposes it through public properties.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl
{
    public class TC_EventDispatching
    {
        public event Action<TowerUpgrade> OnUpgradePurchased;
        public event Action<TowerUpgrade> OnUpgradeApplied;
        public event Action<TowerUpgrade> OnUpgradeRemoved;
        public event Action<Tower> OnTowerUpgraded;

        // Required by TC_Manager
        public void Initialize()
        {
            // No initialization logic required; method exists only to satisfy TC_Manager.
        }

        public void DispatchUpgradePurchased(TowerUpgrade upgrade)
        {
            OnUpgradePurchased?.Invoke(upgrade);
        }

        public void DispatchUpgradeApplied(TowerUpgrade upgrade)
        {
            OnUpgradeApplied?.Invoke(upgrade);
        }

        public void DispatchUpgradeRemoved(TowerUpgrade upgrade)
        {
            OnUpgradeRemoved?.Invoke(upgrade);
        }

        public void DispatchTowerUpgraded(Tower tower)
        {
            OnTowerUpgraded?.Invoke(tower);
        }
    }
}
