using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Towers;
using SASZombieAssaultTD.Engine.Towers.TowerControl;
using System;
using System.Linq;

public static class TowerUpgradeManagerExtensionsBase
{
    ///<summary>
    ///Get upgrade efficiency for a tower.
    ///</summary>
    public static float GetUpgradeEfficiency(
        this TowerUpgradeManager manager,
        Tower tower)
    {
        ArgumentNullException.ThrowIfNull(manager);
        if (manager.GetPurchasedUpgrades(tower).ToList().Count == 0)
        {
            return 0f;
        }

        var totalCost = manager.GetPurchasedUpgrades(tower).ToList().Sum(static u => ((TowerUpgrade)u).Cost);
        var totalPower = manager.GetPurchasedUpgrades(tower).ToList().Sum(static u => ((TowerUpgrade)u).GetPowerRating());

        return totalPower > 0 ? totalPower / totalCost : 0f;
    }
}