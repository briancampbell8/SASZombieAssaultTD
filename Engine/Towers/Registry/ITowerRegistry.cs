// ====================================================================================================
//  FILE: ITowerRegistry.cs
//  PATH: ./Engine/Towers/Registry/
//  MODULE: Core – Tower Registry Subsystem
//
//  ROLE:
//      Defines the contract for tower registration, lookup, and lifecycle tracking.
//
//  RESPONSIBILITIES:
//      - Register(Tower)
//      - Unregister(Tower)
//      - GetAllTowers()
//      - Clear()
//
//  NOTES:
//      Required by TowerManager for placement validation and destruction logic.
// ====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Towers.Registry
{
    public interface ITowerRegistry
    {
        void Register(Tower tower);
        void Unregister(Tower tower);
        IEnumerable<Tower> GetAllTowers();
        void Clear();
    }
}
