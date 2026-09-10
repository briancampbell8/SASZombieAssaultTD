// ====================================================================================================
//  FILE: NavigationGridCore.cs
//  PATH: Engine/Navigation/NavigationGridCore.cs
//  SUBSYSTEM: Navigation
//
//  ROLE:
//      Provides the deterministic 2D navigation grid used by all pathfinding operations.
//      Stores walkability, movement cost, and cell metadata, and exposes safe accessors
//      for NavigationCellGridCore instances.
//
//  RESPONSIBILITIES:
//      - Maintain a deterministic 2D grid of NavigationCellGridCore objects.
//      - Provide safe cell retrieval via GetCell().
//      - Provide deterministic bounds checking via IsInBounds().
//      - Initialize all cells with walkability and movement cost defaults.
//      - Emit DLogger.Log trace statements for all critical grid operations.
//
//  NON-RESPONSIBILITIES:
//      - Performing A* pathfinding (handled by AStarPathfinder).
//      - Converting world-space coordinates (handled by NavigationGridUtility).
//      - Managing neighbor logic (handled by NavigationGridUtility).
//      - Storing or mutating A* state (handled by NavigationCellGridCore).
//
//  ARCHITECTURAL NOTES:
//      - Must remain deterministic across all platforms.
//      - Must allocate all cells up front to avoid runtime allocation spikes.
//      - Must expose only safe, bounds-checked accessors.
// ====================================================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Navigation
{
    /// <summary>
    /// Deterministic 2D navigation grid storing NavigationCellGridCore instances.
    /// </summary>
    public sealed class NavigationGridCore
    {
        private readonly NavigationCellGridCore[,] _cells;

        public int Width { get; }
        public int Height { get; }
        public float CellSize { get; }

        /// <summary>
        /// Constructs a deterministic navigation grid.
        /// </summary>
        public NavigationGridCore(int width, int height, float cellSize = 1.0f)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;

            DLogger.Log(
                $"Navigation.NavigationGridCore.Constructor: Width={width} Height={height} CellSize={cellSize}");

            _cells = new NavigationCellGridCore[Width, Height];

            InitializeGrid();
        }

        /// <summary>
        /// Initializes all grid cells with default walkability and movement cost.
        /// </summary>
        private void InitializeGrid()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var pos = new Vector3Int(x, y, 0);

                    _cells[x, y] = new NavigationCellGridCore(
                        pos,
                        isWalkable: true,
                        movementCost: 1.0f);

                    DLogger.Log(
                        $"Navigation.NavigationGridCore.InitializeGrid: CreatedCell=({x},{y},0) Walkable=true MovementCost=1.0");
                }
            }
        }

        /// <summary>
        /// Returns true if the given coordinates are within grid bounds.
        /// </summary>
        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Width &&
                   y >= 0 && y < Height;
        }

        /// <summary>
        /// Returns true if the given Vector3Int is within grid bounds.
        /// </summary>
        public bool IsInBounds(Vector3Int pos)
        {
            return IsTileOccupied(pos.X, pos.Y);
        }
        public bool IsTileOccupied(int x, int y)
        {
            return x >= 0 && x < Width &&
                   y >= 0 && y < Height;
        }

        /// <summary>
        /// Returns true if the given Vector3Int is within grid bounds.
        /// </summary>
        public bool IsTileOccupied(Vector3Int pos)
        {
            return IsTileOccupied(pos.X, pos.Y);
        }
        /// <summary>
        /// Retrieves a cell at the given coordinates, or null if out of bounds.
        /// </summary>
        public NavigationCellGridCore GetCell(int x, int y)
        {
            if (!IsInBounds(x, y))
            {
                DLogger.Log(
                    $"Navigation.NavigationGridCore.GetCell: OutOfBounds=({x},{y})");
                return null;
            }

            return _cells[x, y];
        }

        /// <summary>
        /// Retrieves a cell at the given grid position, or null if out of bounds.
        /// </summary>
        public NavigationCellGridCore GetCell(Vector3Int pos)
        {
            return GetCell(pos.X, pos.Y);
        }
    }
}
