// ====================================================================================================
//  FILE: NavigationCell.cs
//  PATH: //Engine/Navigation/
//  SUBSYSTEM: Navigation
//
//  ROLE:
//      Represents a deterministic grid cell used by the Navigation subsystem for pathfinding,
//      walkability evaluation, and spatial cost analysis. Each cell stores navigation metadata,
//      A* pathfinding state, and flag-based terrain classification.
//
//  RESPONSIBILITIES:
//      - Store grid coordinates, walkability, terrain cost, and height metadata.
//      - Maintain deterministic A* pathfinding state (GCost, HCost, FCost, Parent).
//      - Provide adjacency, distance, and squared-distance calculations.
//      - Provide flag-based terrain classification through NavigationFlags.
//      - Serve as the foundational data structure for NavigationGrid and pathfinding algorithms.
//
//  NON-RESPONSIBILITIES:
//      - Executing navigation logic or performing pathfinding algorithms directly.
//      - Managing ECSEntityCore lifecycle, subsystem orchestration, or runtime sequencing.
//      - Performing migration, validation, or analysis operations.
//
//  NOTES:
//      This class was extracted from legacy NavigationMigrationHelper.cs and now resides in its
//      proper subsystem-aligned module. Contains no logic beyond deterministic navigation cell
//      storage and helper calculations.
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Navigation.NavigationEnums;

namespace SASZombieAssaultTD.Engine.Navigation
{
    /// <summary>
    /// Navigation cell for pathfinding and grid-based movement. Optimized for performance with bit-based flags.
    /// </summary>
    public class NavigationCell
    {
        public int X { get; }
        public int Y { get; }
        public bool Walkable { get; set; }
        public float Cost { get; set; } = 1.0f;
        public float Height { get; set; } = 0.0f;
        public NavigationFlags Flags { get; set; } = NavigationFlags.None;

        // A* pathfinding properties
        public float GCost { get; set; } = float.MaxValue;

        public float HCost { get; set; } = float.MaxValue;
        public float FCost => GCost + HCost;
        public NavigationCell Parent { get; set; }

        // Required for heap operations
        public int HeapIndex { get; set; } = -1;

        public bool IsInOpenSet { get; set; } = false;
        public bool IsInClosedSet { get; set; } = false;

        public NavigationCell(int x, int y, bool walkable = true)
        {
            X = x;
            Y = y;
            Walkable = walkable;
        }

        /// <summary>
        /// Grid position as Vector3Int.
        /// </summary>
        public Vector3Int Position => new(X, Y);

        /// <summary>
        /// Gets the center position in world coordinates.
        /// </summary>
        public Vector3 Center => new(X + 0.5f, 0f, Y + 0.5f);

        /// <summary>
        /// Checks if this cell is adjacent to another cell.
        /// </summary>
        public bool IsAdjacentTo(NavigationCell other)
        {
            var dx = System.Math.Abs(X - other.X);
            var dy = System.Math.Abs(Y - other.Y);
            return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
        }

        /// <summary>
        /// Gets the distance to another cell.
        /// </summary>
        public float DistanceTo(NavigationCell other)
        {
            var dx = X - other.X;
            var dy = Y - other.Y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Gets the squared distance to another cell (faster than DistanceTo).
        /// </summary>
        public float DistanceSquaredTo(NavigationCell other)
        {
            var dx = X - other.X;
            var dy = Y - other.Y;
            return dx * dx + dy * dy;
        }

        /// <summary>
        /// Checks if this cell has a specific flag.
        /// </summary>
        public bool HasFlag(NavigationFlags flag) => (Flags & flag) == flag;

        /// <summary>
        /// Sets a specific flag.
        /// </summary>
        public void SetFlag(NavigationFlags flag) => Flags |= flag;

        /// <summary>
        /// Clears a specific flag.
        /// </summary>
        public void ClearFlag(NavigationFlags flag) => Flags &= ~flag;

        /// <summary>
        /// Gets a hash code for the cell.
        /// </summary>
        public override int GetHashCode() => HashCode.Combine(X, Y);

        /// <summary>
        /// Checks if this cell equals another cell.
        /// </summary>
        public override bool Equals(object obj) =>
            obj is NavigationCell other && X == other.X && Y == other.Y;

        /// <summary>
        /// Gets a string representation of the cell.
        /// </summary>
        public override string ToString() => $"Cell({X}, {Y}, Walkable: {Walkable})";

        /// <summary>
        /// Resets pathfinding data for A* algorithm.
        /// </summary>
        public void ResetPathfindingData()
        {
            GCost = float.MaxValue;
            HCost = float.MaxValue;
            Parent = null;
            HeapIndex = -1;
            IsInOpenSet = false;
            IsInClosedSet = false;
        }
    }
}
