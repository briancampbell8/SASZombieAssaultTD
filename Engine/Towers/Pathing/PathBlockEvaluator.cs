// ====================================================================================================
//  FILE: PathBlockEvaluator.cs
//  PATH: ./Engine/Towers/Pathing/
//  MODULE: Core – Path Blocking Evaluation Subsystem
//
//  ROLE:
//      Provides path blocking evaluation for tower placement.
//
//  RESPONSIBILITIES:
//      - Evaluate whether a tower blocks enemy movement paths.
//      - Provide deterministic results for placement validation.
//
//  NOTES:
//      Current implementation is a non-blocking stub.
//      Future integration with NavigationGrid and pathfinding system.
// ====================================================================================================

using SASZombieAssaultTD.Engine.Dictionary;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Towers.Pathing
{
    public sealed class PathBlockEvaluator : IPathBlockEvaluator
    {
        public static PathBlockEvaluator Instance { get; } = new PathBlockEvaluator();

        public bool WouldBlockPath(Vector3 worldPosition, TowerType towerType)
        {
            return false; // Stub
        }

        public bool WouldBlockPath(Vector3 worldPosition, TowerEnums.TowerType towerType)
        {
            throw new System.NotImplementedException();
        }
    }
}
