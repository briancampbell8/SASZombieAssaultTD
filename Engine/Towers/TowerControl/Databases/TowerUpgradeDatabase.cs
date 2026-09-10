// =====================================================================================================
//  FILE: TowerUpgradeDatabase.cs
//  PATH: Engine/Towers/TowerControl/Database/TowerUpgradeDatabase.cs
//  SUBSYSTEM: Towers > TowerControl > Database
//
//  ROLE:
//      Immutable per‑tower upgrade data container used by TC_DatabaseControl.
//      Stores deterministic upgrade entries for each upgrade level.
//
//  RESPONSIBILITIES:
//      - Hold upgrade entries for a single tower type.
//      - Provide deterministic lookup by upgrade level.
//      - Serve as a stable data container for TowerControl subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Upgrade path logic (TC_PathControl).
//      - Availability checks (TC_AvailabilityControl).
//      - Purchasing or applying upgrades (TC_PurchaseControl).
//      - Saving or loading upgrade data (TC_SaveControl / TC_LoadControl).
//      - Dispatching upgrade events (TC_EventDispatching).
//
//  ARCHITECTURAL NOTES:
//      - Pure micro‑class.
//      - Deterministic, no mutation beyond controlled AddEntry.
//      - Created and owned by TC_DatabaseControl.
// =====================================================================================================

using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Databases
{
    /// <summary>
    /// Deterministic per‑tower upgrade database.
    /// </summary>
    public sealed class TowerUpgradeDatabase
    {
        private readonly Dictionary<int, TowerUpgradeEntry> _entries;

        public TowerUpgradeDatabase()
        {
            _entries = new Dictionary<int, TowerUpgradeEntry>();
        }

        /// <summary>
        /// Adds a deterministic upgrade entry for a specific level.
        /// </summary>
        public void AddEntry(int level, TowerUpgradeEntry entry)
        {
            _entries[level] = entry;
        }

        /// <summary>
        /// Retrieves the upgrade entry for a given level.
        /// </summary>
        public TowerUpgradeEntry GetEntry(int level)
        {
            return _entries.TryGetValue(level, out var entry)
                ? entry
                : null;
        }

        /// <summary>
        /// Exposes the internal dictionary for read‑only wiring.
        /// </summary>
        public IReadOnlyDictionary<int, TowerUpgradeEntry> Entries => _entries;
    }

    /// <summary>
    /// Immutable upgrade entry describing a single upgrade level.
    /// </summary>
    public sealed class TowerUpgradeEntry
    {
        public int Level { get; }
        public int Cost { get; }
        public float DamageMultiplier { get; }
        public float RangeMultiplier { get; }
        public float FireRateMultiplier { get; }

        public TowerUpgradeEntry(int level, int cost, float dmg, float range, float fireRate)
        {
            Level = level;
            Cost = cost;
            DamageMultiplier = dmg;
            RangeMultiplier = range;
            FireRateMultiplier = fireRate;
        }
    }
}
