// ====================================================================================================
//  FILE: TowerRegistry.cs
//  PATH: ./Engine/Towers/Registry/
//  MODULE: Core – Tower Registry Subsystem
//
//  ROLE:
//      Centralized registry for all active towers.
//
//  RESPONSIBILITIES:
//      - Maintain authoritative list of active towers.
//      - Provide lookup for placement validation and path blocking checks.
//      - Support cleanup operations.
//
//  NOTES:
//      Singleton instance used by TowerManager.
// ====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Towers.Registry
{
    public sealed class TowerRegistry : ITowerRegistry
    {
        public static TowerRegistry Instance { get; } = new TowerRegistry();

        private readonly List<Tower> _towers = new();

        public void Register(Tower tower) => _towers.Add(tower);
        public void Unregister(Tower tower) => _towers.Remove(tower);
        public IEnumerable<Tower> GetAllTowers() => _towers;
        public void Clear() => _towers.Clear();
    }
}
