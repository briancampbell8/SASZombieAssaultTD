using System;
using SASZombieAssaultTD.Engine.VectorMath;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Systems.Gameplay
{
    ///<summary>
    ///Navigation cell for pathfinding and grid-based movement.
    ///Optimized for performance with bit-based flags.
    ///</summary>
    public class NavigationCell
    {
        public int X { get; }
        public int Y { get; }
        public bool Walkable { get; set; }
        public float Cost { get; set; } = 1.0f;
        public float Height { get; set; } = 0.0f;
        public NavigationFlags Flags { get; set; } = NavigationFlags.None;

        //A* pathfinding properties
        public float GCost { get; set; } = float.MaxValue;
        public float HCost { get; set; } = float.MaxValue;
        public float FCost => GCost + HCost;
        public NavigationCell Parent { get; set; }
        public bool IsInOpenSet { get; set; } = false;
        public bool IsInClosedSet { get; set; } = false;

        public NavigationCell(int x, int y, bool walkable = true)
        {
            X = x;
            Y = y;
            Walkable = walkable;
        }

        ///<summary>
        ///</summary>
        public Vector3Int Position => new(X, Y);

        ///<summary>
        ///Gets the center position in world coordinates.
        ///</summary>
        public Vector3 Center => new(X + 0.5f, 0f, Y + 0.5f);

        ///<summary>
        ///Checks if this cell is adjacent to another cell.
        ///</summary>
        public bool IsAdjacentTo(NavigationCell other)
        {
            var dx = System.Math.Abs(X - other.X);
            var dy = System.Math.Abs(Y - other.Y);
            return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
        }

        ///<summary>
        ///Gets the distance to another cell.
        ///</summary>
        public float DistanceTo(NavigationCell other)
        {
            var dx = X - other.X;
            var dy = Y - other.Y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }

        ///<summary>
        ///Gets the squared distance to another cell (faster than DistanceTo).
        ///</summary>
        public float DistanceSquaredTo(NavigationCell other)
        {
            var dx = X - other.X;
            var dy = Y - other.Y;
            return dx * dx + dy * dy;
        }

        ///<summary>
        ///Checks if this cell has a specific flag.
        ///</summary>
        public bool HasFlag(NavigationFlags flag) => (Flags & flag) == flag;

        ///<summary>
        ///Sets a specific flag.
        ///</summary>
        public void SetFlag(NavigationFlags flag) => Flags |= flag;

        ///<summary>
        ///Clears a specific flag.
        ///</summary>
        public void ClearFlag(NavigationFlags flag) => Flags &= ~flag;

        ///<summary>
        ///Gets a hash code for the cell.
        ///</summary>
        public override int GetHashCode() => HashCode.Combine(X, Y);

        ///<summary>
        ///Checks if this cell equals another cell.
        ///</summary>
        public override bool Equals(object obj) =>
            obj is NavigationCell other && X == other.X && Y == other.Y;

        ///<summary>
        ///Gets a string representation of the cell.
        ///</summary>
        public override string ToString() => $"Cell({X}, {Y}, Walkable: {Walkable})";

        ///<summary>
        ///Resets pathfinding data for A* algorithm.
        ///</summary>
        public void ResetPathfindingData()
        {
            GCost = float.MaxValue;
            HCost = float.MaxValue;
            Parent = null;
            IsInOpenSet = false;
            IsInClosedSet = false;
        }
    }


    ///<summary>
    ///Navigation flags for cell properties.
    ///</summary>
    [Flags]
    public enum NavigationFlags
    {
        None = 0,
        Walkable = 1 << 0,
        Blocked = 1 << 1,
        Water = 1 << 2,
        Lava = 1 << 3,
        Spikes = 1 << 4,
        Slow = 1 << 5,
        Fast = 1 << 6,
        Teleport = 1 << 7,
        SpawnPoint = 1 << 8,
        Objective = 1 << 9,
        Hazard = 1 << 10,
        Cover = 1 << 11,
        Elevated = 1 << 12,
        Underground = 1 << 13,
        Indoor = 1 << 14,
        Outdoor = 1 << 15
    }
}
