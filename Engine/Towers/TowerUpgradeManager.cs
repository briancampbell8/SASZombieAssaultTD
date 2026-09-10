// =====================================================================================================
//  FILE: TowerUpgradeManager.cs
//  PATH: Engine/Towers/TowerUpgradeManager.cs
//  SUBSYSTEM: Towers Subsystem
//
//  ROLE:
//      Owns and manages all runtime tower upgrade state for the engine.
//      Provides deterministic access to purchased upgrades, upgrade counts, upgrade queries,
//      and upgrade application/removal operations.
//      Serves as the authoritative upgrade state holder for all TowerControl subsystems.
//
//  RESPONSIBILITIES:
//      - Store purchased upgrades deterministically.
//      - Provide lookup, count, and query operations for upgrades.
//      - Apply and remove upgrades in a deterministic manner.
//      - Emit trace statements for all state mutations and queries.
//
//  NON-RESPONSIBILITIES:
//      - Building upgrades (handled by TC_UpgradeBuilder).
//      - Resolving upgrades (handled by TC_UpgradeResolver).
//      - Validating upgrades (handled by TC_RequirementsValidator / TC_RewardsValidator).
//      - Computing statistics (handled by TC_CostStats / TC_PerformanceStats / TC_PowerStats).
//
//  ARCHITECTURAL NOTES:
//      - This subsystem replaces all legacy NeuralNet.cs upgrade ownership logic.
//      - All upgrade state must flow through this manager; no other subsystem may store upgrades.
//      - Deterministic behavior is enforced through strict trace logging and controlled mutation.
// =====================================================================================================

using System.Collections.Generic;
using System.Diagnostics;
using static SASZombieAssaultTD.Engine.Towers.TowerEnums;

namespace SASZombieAssaultTD.Engine.Towers
{
    public sealed class TowerUpgradeManager
    {
        public readonly List<TowerUpgrade> _purchasedUpgrades;

        public TowerUpgradeManager()
        {
            // Inline comment: initialize deterministic upgrade storage
            _purchasedUpgrades = new List<TowerUpgrade>();

            Trace.WriteLine("TowerUpgradeManager: Constructed - Upgrade storage initialized.");
        }

        // -------------------------------------------------------------------------------------------------
        //  APPLY UPGRADE
        // -------------------------------------------------------------------------------------------------

        public void ApplyUpgrade(TowerUpgrade upgrade)
        {
            // Trace application request
            Trace.WriteLine($"TowerUpgradeManager: ApplyUpgrade - Applying upgrade '{upgrade?.Key}'.");

            if (upgrade == null)
            {
                Trace.WriteLine("TowerUpgradeManager: ApplyUpgrade - Null upgrade ignored.");
                return;
            }

            _purchasedUpgrades.Add(upgrade);

            Trace.WriteLine($"TowerUpgradeManager: ApplyUpgrade - Upgrade '{upgrade.Key}' applied.");
        }

        // -------------------------------------------------------------------------------------------------
        //  REMOVE UPGRADE
        // -------------------------------------------------------------------------------------------------

        public void RemoveUpgrade(string key)
        {
            // Trace removal request
            Trace.WriteLine($"TowerUpgradeManager: RemoveUpgrade - Removing upgrade '{key}'.");

            if (string.IsNullOrWhiteSpace(key))
                return;

            for (int i = _purchasedUpgrades.Count - 1; i >= 0; i--)
            {
                if (_purchasedUpgrades[i].Key == key)
                {
                    _purchasedUpgrades.RemoveAt(i);
                    Trace.WriteLine($"TowerUpgradeManager: RemoveUpgrade - Upgrade '{key}' removed.");
                }
            }
        }

        // -------------------------------------------------------------------------------------------------
        //  HAS UPGRADE
        // -------------------------------------------------------------------------------------------------

        public bool HasUpgrade(string key)
        {
            // Trace lookup
            Trace.WriteLine($"TowerUpgradeManager: HasUpgrade - Checking upgrade '{key}'.");

            if (string.IsNullOrWhiteSpace(key))
                return false;

            for (int i = 0; i < _purchasedUpgrades.Count; i++)
            {
                if (_purchasedUpgrades[i].Key == key)
                    return true;
            }

            return false;
        }

        // -------------------------------------------------------------------------------------------------
        //  GET UPGRADE COUNT (BY KEY)
        // -------------------------------------------------------------------------------------------------

        public int GetUpgrades(string key)
        {
            // Trace lookup
            Trace.WriteLine($"TowerUpgradeManager: GetUpgrades - Counting upgrades for key '{key}'.");

            if (string.IsNullOrWhiteSpace(key))
                return 0;

            int count = 0;

            for (int i = 0; i < _purchasedUpgrades.Count; i++)
            {
                if (_purchasedUpgrades[i].Key == key)
                    count++;
            }

            return count;
        }

        // -------------------------------------------------------------------------------------------------
        //  GET UPGRADE COUNT (BY TYPE)
        // -------------------------------------------------------------------------------------------------

        public int GetUpgrades(UpgradeType type)
        {
            // Trace lookup
            Trace.WriteLine($"TowerUpgradeManager: GetUpgrades - Counting upgrades for type '{type}'.");

            int count = 0;

            for (int i = 0; i < _purchasedUpgrades.Count; i++)
            {
                if (_purchasedUpgrades[i].Type == type)
                    count++;
            }

            return count;
        }

        // -------------------------------------------------------------------------------------------------
        //  GET ALL PURCHASED UPGRADES
        // -------------------------------------------------------------------------------------------------

        public IReadOnlyList<TowerUpgrade> GetPurchasedUpgrades(Tower tower)
        {
            // Trace retrieval
            Trace.WriteLine("TowerUpgradeManager: GetPurchasedUpgrades - Returning upgrade list.");

            return _purchasedUpgrades;
        }
    }
}
