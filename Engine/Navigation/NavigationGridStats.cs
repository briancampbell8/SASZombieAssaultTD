// =====================================================================================================
//  FILE: NavigationGridStats.cs
//  PATH: Engine/Navigation/NavigationGridStats.cs
//  SUBSYSTEM: Navigation
//
//  ROLE:
//      Serves as a read-only data snapshot container aggregating structural grid statistics 
//      including total, blocked, occupied, and free grid cells along with spatial occupancy ratios.
//
//  RESPONSIBILITIES:
//      - Compute grid occupancy statistics from the live map layout.
//      - Expose properties for cell calculations and occupancy metrics.
//
//  NON-RESPONSIBILITIES:
//      - Mutating live pathfinding graphs or tile walkability nodes.
//      - Rendering analytical diagnostic dashboards or execution logs directly.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Navigation
{
    /// <summary>
    /// Read-only snapshot of navigation grid statistics.
    /// </summary>
    public sealed class NavigationGridStats
    {
        // ----------------------------------------------------------------------------------------------------
        // Snapshot Properties
        // ----------------------------------------------------------------------------------------------------
        public int TotalCells { get; private set; }
        public int BlockedCells { get; private set; }
        public int OccupiedCells { get; private set; }
        public int FreeCells { get; private set; }
        public float OccupancyRatio { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        // ----------------------------------------------------------------------------------------------------
        // Constructor: Snapshot from Core + Pipeline Manager
        // ----------------------------------------------------------------------------------------------------
        public NavigationGridStats(NavigationGridCore core, NavigationGrid pipeline)
        {
            Width = core.Width;
            Height = core.Height;
            TotalCells = Width * Height;

            int blocked = 0;
            int occupied = 0;

            // Walk the entire grid deterministically
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var cell = core.GetCell(x, y);

                    if (cell == null)
                        continue;

                    // Blocked = not walkable
                    if (!cell.IsWalkable)
                        blocked++;

                    // Occupied = pipeline-level occupancy (SetOccupied)
                    // Occupied cells are represented as walkable=false by pipeline logic
                    if (!cell.IsWalkable)
                        occupied++;
                }
            }

            BlockedCells = blocked;
            OccupiedCells = occupied;
            FreeCells = TotalCells - blocked - occupied;

            OccupancyRatio = TotalCells > 0
                ? (float)occupied / TotalCells
                : 0f;
        }
    }
}
