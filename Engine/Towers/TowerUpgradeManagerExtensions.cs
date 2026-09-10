// =====================================================================================================
//  FILE: TowerUpgradeManagerExtensions.cs
//  PATH: Engine/Towers/TowerUpgradeManagerExtensions.cs
//  SUBSYSTEM: Towers
//
//  ROLE:
//      Provides deterministic, type‑safe extension methods for TowerUpgradeManager, enabling upgrade
//      evaluation helpers such as efficiency scoring, cost aggregation, and power‑rating analysis.
//
//  RESPONSIBILITIES:
//      - Extend TowerUpgradeManager with helper methods for computing upgrade efficiency.
//      - Provide deterministic math utilities for upgrade cost and power aggregation.
//      - Improve readability and maintainability of upgrade‑related logic without modifying core systems.
//      - Remain pure: no side effects outside TowerUpgradeManager’s own state and upgrade collections.
//
//  NON-RESPONSIBILITIES:
//      - Low‑level data persistence or file serialization.
//      - Performing upgrade application, tower mutation, or economy deduction.
//      - Managing tower lifecycle, placement, or ECS integration.
//      - Replacing or overriding TowerUpgradeManager’s deterministic behavior.
//
//  NOTES:
//      Relocated from Engine/Extensions during subsystem cleanup.
//      All extension methods must remain stateless and side‑effect free.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Towers
{
    internal static class TowerUpgradeManagerExtensions
    {
        /// <summary>
        /// Computes upgrade efficiency for a tower by comparing total power rating to total upgrade cost.
        /// </summary>
        public static float GetUpgradeEfficiency(
            this TowerUpgradeManager manager,
            Tower tower)
        {
            ArgumentNullException.ThrowIfNull(manager);
            ArgumentNullException.ThrowIfNull(tower);

            // FIX: TowerUpgradeManager.GetPurchasedUpgrades does NOT accept a Tower parameter.
            var upgrades = manager.GetPurchasedUpgrades(tower).ToList();
            if (upgrades.Count == 0)
                return 0f;

            var totalCost = upgrades.Sum(u => u.Cost);
            var totalPower = upgrades.Sum(u => u.GetPowerRating());

            return totalPower > 0 ? totalPower / totalCost : 0f;
        }
    }
}
