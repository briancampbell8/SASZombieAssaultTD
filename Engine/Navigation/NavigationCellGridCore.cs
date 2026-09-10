// ====================================================================================================
//  FILE: NavigationCellGridCore.cs
//  PATH: Engine/Navigation/
//  SUBSYSTEM: Navigation
//
//  ROLE:
//      Provides a deterministic, lightweight navigation cell used by grid-based pathfinding.
//      Stores immutable grid coordinates, walkability state, movement cost, and A* pathfinding metadata.
//
//  RESPONSIBILITIES:
//      - Maintain immutable grid position and walkability state.
//      - Store terrain movement cost for pathfinding heuristics.
//      - Maintain deterministic A* pathfinding fields (GCost, HCost, FCost, Parent, HeapIndex).
//      - Provide ResetPathfindingData() to clear A* state without reallocating the cell.
//      - Serve as the atomic node type consumed by NavigationGridCore and AStarPathfinder.
//
//  NON-RESPONSIBILITIES:
//      - Managing grid-wide walkability or terrain metadata (handled by NavigationGridCore).
//      - Performing pathfinding algorithms (handled by AStarPathfinder).
//      - Converting world-space coordinates (handled by NavigationGridUtility).
//      - Storing neighbor relationships or grid containers.
//
//  ARCHITECTURAL NOTES:
//      - Must remain allocation-safe and lightweight for high-performance pathfinding.
//      - Coordinates must remain immutable after construction.
//      - All A* fields must be resettable deterministically without creating new cell instances.
// ====================================================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Navigation
{
    /// <summary>
    /// Represents a single navigation cell used by grid-based pathfinding. Stores walkability, movement cost, and A*
    /// pathfinding state.
    /// </summary>
    public sealed class NavigationCellGridCore
    {
        /// <summary>
        /// Immutable grid position of this cell.
        /// </summary>
        public bool IsWalkable { get; set; }

        /// <summary>
        /// Immutable grid position of this cell.
        /// </summary>
        public Vector3Int Position { get; }

        /// <summary>
        /// Movement cost multiplier for this cell (terrain cost).
        /// </summary>
        public float MovementCost { get; }

        /// <summary>
        /// A* cost from start node to this node.
        /// </summary>
        public float GCost { get; set; }

        /// <summary>
        /// A* heuristic cost from this node to the target.
        /// </summary>
        public float HCost { get; set; }

        /// <summary>
        /// Total A* cost (G + H).
        /// </summary>
        public float FCost => GCost + HCost;

        /// <summary>
        /// Parent cell in the A* path.
        /// </summary>
        public NavigationCellGridCore Parent { get; set; }

        /// <summary>
        /// Heap index used by priority queue / binary heap implementations.
        /// </summary>
        public int HeapIndex { get; set; }

        /// <summary>
        /// Constructs a new navigation cell with immutable position and walkability.
        /// </summary>
        public NavigationCellGridCore(Vector3Int position, bool isWalkable, float movementCost = 1.0f)
        {
            Position = position;
            IsWalkable = isWalkable;
            MovementCost = movementCost;

            DLogger.Log(
                $"Navigation.NavigationCellGridCore.Constructor: Position=({position.X},{position.Y},{position.Z}) " +
                $"Walkable={isWalkable} MovementCost={movementCost}");

            ResetPathfindingData();
        }

        /// <summary>
        /// Resets A* pathfinding data to default values without reallocating the cell.
        /// </summary>
        public void ResetPathfindingData()
        {
            GCost = float.MaxValue;
            HCost = float.MaxValue;
            Parent = null;
            HeapIndex = -1;

            DLogger.Log(
                $"Navigation.NavigationCellGridCore.ResetPathfindingData: Position=({Position.X},{Position.Y},{Position.Z}) " +
                $"GCost={GCost} HCost={HCost} HeapIndex={HeapIndex} Parent=null");
        }
    }
}
