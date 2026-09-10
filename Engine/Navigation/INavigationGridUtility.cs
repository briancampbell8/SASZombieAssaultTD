// ====================================================================================================
//  FILE: INavigationGridUtility.cs
//  PATH: Engine/Navigation/INavigationGridUtility.cs
//  SUBSYSTEM: Navigation
//
//  ROLE:
//      Defines the stateless helper contract for NavigationGridUtility.
//      Provides world/grid coordinate conversion and deterministic neighbor retrieval.
//
//  RESPONSIBILITIES:
//      - Convert world positions to grid positions and vice versa.
//      - Retrieve deterministic neighbors from NavigationGridCore.
//      - Expose pure helper operations that do not mutate grid state.
//
//  NON-RESPONSIBILITIES:
//      - Storing grid cells (handled by NavigationGridCore).
//      - Managing navigation state (handled by NavigationGrid).
//      - Computing statistics (handled by NavigationGridStats).
//
//  ARCHITECTURAL NOTES:
//      - Must remain stateless.
//      - Must not expose or require mutable engine state.
//      - Must not expose internal-only types.
// ====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Navigation
{
    public interface INavigationGridUtility
    {
        // ------------------------------------------------------------------------------------------------
        // Coordinate Conversion
        // ------------------------------------------------------------------------------------------------
        Vector3Int WorldToGrid(Vector3 worldPosition, float cellSize);
        Vector3 GridToWorld(Vector3Int gridPosition, float cellSize);

        // ------------------------------------------------------------------------------------------------
        // Neighbor Retrieval
        // ------------------------------------------------------------------------------------------------
        List<NavigationCellGridCore> GetNeighbors(
            NavigationGridCore grid,
            Vector3Int gridPosition,
            bool allowDiagonal);
    }
}
