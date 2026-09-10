// ====================================================================================================
//  FILE: TowerCostDatabase.cs
//  PATH: ./Engine/Towers/
//  MODULE: Core – Tower Economy Subsystem
//
//  ROLE:
//      Provides authoritative cost values for all tower types.
//
//  RESPONSIBILITIES:
//      - GetCost(TowerType)
//      - Maintain cost table
//
//  NOTES:
//      Used by TowerManager for placement and refund calculations.
// ====================================================================================================

using System.Collections.Generic;
using static SASZombieAssaultTD.Engine.Towers.TowerEnums;

namespace SASZombieAssaultTD.Engine.Towers
{
    public sealed class TowerCostDatabase
    {
        public static TowerCostDatabase Instance { get; } = new TowerCostDatabase();

        private readonly Dictionary<TowerType, int> _costs = new()
        {
            { TowerType.Basic, 100 },
            { TowerType.Sniper, 200 },
            { TowerType.Splash, 150 },
            { TowerType.Freeze, 175 }
        };

        public int GetCost(TowerType type)
        {
            return _costs.TryGetValue(type, out var cost) ? cost : 100;
        }
    }
}
