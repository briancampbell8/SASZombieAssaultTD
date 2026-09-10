// ====================================================================================================
//  FILE: IPathBlockEvaluator.cs
//  PATH: ./Engine/Towers/Pathing/
//  MODULE: Core – Path Blocking Evaluation Subsystem
//
//  ROLE:
//      Defines the contract for determining whether a tower placement blocks enemy navigation paths.
//
//  RESPONSIBILITIES:
//      - WouldBlockPath(Vector3 worldPosition, TowerType towerType)
//
//  NOTES:
//      TowerManager depends on this interface for placement validation.
// ====================================================================================================

using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Towers.TowerEnums;

namespace SASZombieAssaultTD.Engine.Towers.Pathing
{
    public interface IPathBlockEvaluator
    {
        bool WouldBlockPath(Vector3 worldPosition, TowerType towerType);
    }
}
