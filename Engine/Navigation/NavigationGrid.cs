/*
File:    NavigationGrid.cs
Purpose: Grid-based navigation system for pathfinding.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Navigation;
using SASZombieAssaultTD.Engine.Core;
using Vector3 = SASZombieAssaultTD.Engine.VectorMath.Vector3;
using Vector3Int = SASZombieAssaultTD.Engine.VectorMath.Vector3Int;
using SASZombieAssaultTD.Engine.Towers;

namespace SASZombieAssaultTD.Engine.Navigation
{
    /// <summary>
    /// Grid-based navigation system for pathfinding.
    /// Represents a 2D grid where cells can be walkable or blocked.
    /// </summary>
    public sealed class NavigationGrid
    {
        // Singleton instance to satisfy callers that expect NavigationGrid.Instance
        private static NavigationGrid? _instance;
        public static NavigationGrid? Instance
        {
            get => _instance;
            set => _instance = value;
        }

        /// <summary>
        /// Helper to create and assign a global instance.
        /// Call once during initialization (e.g. GameRoot/Bootstrap).
        /// </summary>
        public static void InitializeInstance(int width, int height) => Instance = new NavigationGrid(width, height);

        /// <summary>
        /// Clears the global instance (useful for tests or shutdown).
        /// </summary>
        public static void ClearInstance() => Instance = null;

        private readonly bool[,] _walkable;
        private readonly int _width;
        private readonly int _height;

        /// <summary>
        /// Gets or sets whether the grid has a path.
        /// </summary>
        public bool HasPath { get; set; } = false;

        /// <summary>
        /// Gets the current path through the grid.
        /// </summary>
        public IReadOnlyList<Vector3Int> CurrentPath { get; private set; } = Array.Empty<Vector3Int>();

        /// <summary>
        /// Clears the current path.
        /// </summary>
        public void ClearPath()
        {
            CurrentPath = Array.Empty<Vector3Int>();
        }

        /// <summary>
        /// Gets navigation grid statistics.
        /// </summary>
        /// <returns>Navigation grid statistics.</returns>
        public NavigationGridStats GetStats()
        {
            int totalCells = Width * Height;
            int blockedCells = 0;
            int occupiedCells = 0;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (!_walkable[x, y])
                    {
                        blockedCells++;
                    }
                    else if (IsOccupied(new Vector3Int(x, y)))
                    {
                        occupiedCells++;
                    }
                }
            }

            int freeCells = totalCells - blockedCells - occupiedCells;
            float occupancyRatio = totalCells > 0 ? (float)occupiedCells / totalCells : 0f;

            return new NavigationGridStats
            {
                TotalCells = totalCells,
                BlockedCells = blockedCells,
                OccupiedCells = occupiedCells,
                FreeCells = freeCells,
                OccupancyRatio = occupancyRatio
            };
        }

        /// <summary>
        /// Gets the width of the grid.
        /// </summary>
        public int Width => _width;

        /// <summary>
        /// Gets the height of the grid.
        /// </summary>
        public int Height => _height;

        /// <summary>
        /// Gets the size of each grid cell in world units.
        /// Phase 3: Navigation System Completion - Fix CS1061 errors
        /// </summary>
        public float CellSize { get; } = 1.0f;

        /// <summary>
        /// Initializes a new NavigationGrid.
        /// </summary>
        /// <param name="width">Grid width.</param>
        /// <param name="height">Grid height.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when width or height is less than or equal to zero.</exception>
        public NavigationGrid(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Grid dimensions must be positive.");

            _width = width;
            _height = height;
            _walkable = new bool[width, height];

            InitializeGrid();
        }

        private void InitializeGrid()
        {
            // Initialize all cells as walkable
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _walkable[x, y] = true;
                }
            }
        }

        /// <summary>
        /// Checks if a position is walkable.
        /// </summary>
        /// <param name="position">Position to check.</param>
        /// <returns>True if walkable.</returns>
        public bool IsWalkable(Vector3Int position) =>
            IsInBounds(position) && _walkable[position.X, position.Y];

        /// <summary>
        /// Checks if a world position is walkable.
        /// </summary>
        /// <param name="position">World position to check.</param>
        /// <returns>True if walkable.</returns>
        public bool IsWalkable(Vector3 position) =>
            IsWalkable(new Vector3Int((int)position.X, (int)position.Y));

        /// <summary>
        /// Checks if a grid position is walkable using coordinate components.
        /// Adapts component-based walkability calls to the canonical IsWalkable implementation.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <returns>True if walkable.</returns>
        public bool IsWalkable(int x, int y)
        {
            var position = new Vector3Int(x, y);
            return IsWalkable(position);
        }

        /// <summary>
        /// Sets a cell as walkable or blocked.
        /// </summary>
        /// <param name="position">Cell position.</param>
        /// <param name="walkable">Whether the cell is walkable.</param>
        public void SetWalkable(Vector3Int position, bool walkable)
        {
            if (IsInBounds(position))
            {
                _walkable[position.X, position.Y] = walkable;
            }
        }

        /// <summary>
        /// Updates an area on the grid to be walkable or blocked.
        /// </summary>
        /// <param name="position">Top-left position of the area.</param>
        /// <param name="size">Size of the area.</param>
        /// <param name="isWalkable">Whether the area is walkable.</param>
        public void UpdateEntityArea(Vector3 position, Vector3 size, bool isWalkable)
        {
            var startX = System.Math.Max(0, (int)position.X);
            var startY = System.Math.Max(0, (int)position.Y);
            var endX = System.Math.Min(Width, startX + (int)size.X);
            var endY = System.Math.Min(Height, startY + (int)size.Y);

            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    _walkable[x, y] = isWalkable;
                }
            }
        }

        /// <summary>
        /// Gets the total number of cells in the grid.
        /// </summary>
        public int TotalCells => Width * Height;

        /// <summary>
        /// Converts a world position to a grid position.
        /// Phase 3: Navigation System Completion - Fix CS1061 errors
        /// </summary>
        /// <param name="worldPosition">The world position.</param>
        /// <returns>The corresponding grid position.</returns>
        public Vector3Int WorldToGrid(Vector3 worldPosition)
        {
            int gx = (int)(worldPosition.X / CellSize);
            int gy = (int)(worldPosition.Y / CellSize);
            return new Vector3Int(gx, gy, 0);
        }

        /// <summary>
        /// Converts grid coordinates to world coordinates.
        /// </summary>
        /// <param name="gridPosition">The grid position.</param>
        /// <returns>The corresponding world position.</returns>
        public Vector3 GridToWorld(Vector3Int gridPosition)
        {
            float wx = gridPosition.X * CellSize;
            float wy = gridPosition.Y * CellSize;
            return new Vector3(wx, wy, 0);
        }

        /// <summary>
        /// Gets the cell at the specified grid position.
        /// Phase 3: Navigation System Completion - Fix CS1061 errors
        /// </summary>
        /// <param name="x">Grid X coordinate.</param>
        /// <param name="y">Grid Y coordinate.</param>
        /// <returns>The NavigationCell at the position.</returns>
        public NavigationCell GetCell(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                return new NavigationCell(new Vector3Int(x, y), _walkable[x, y]);
            }
            return null;
        }

        /// <summary>
        /// Gets the cell at the specified grid position.
        /// </summary>
        /// <param name="gridPosition">The grid position.</param>
        /// <returns>The NavigationCell at the position, or null if out of bounds.</returns>
        public NavigationCell GetCell(Vector3Int gridPosition)
        {
            if (IsInBounds(gridPosition))
            {
                return new NavigationCell(gridPosition, _walkable[gridPosition.X, gridPosition.Y]);
            }
            return null;
        }

        /// <summary>
        /// Gets the neighbors of the specified grid position.
        /// </summary>
        /// <param name="gridPosition">The grid position.</param>
        /// <param name="allowDiagonal">Whether to include diagonal neighbors.</param>
        /// <returns>A list of neighboring cells.</returns>
        public List<NavigationCell> GetNeighbors(Vector3Int gridPosition, bool allowDiagonal)
        {
            var neighbors = new List<NavigationCell>();

            int[,] directions = allowDiagonal
                ? new int[,] { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } }
                : new int[,] { { -1, 0 }, { 0, -1 }, { 0, 1 }, { 1, 0 } };

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                var neighborPosition = new Vector3Int(gridPosition.X + directions[i, 0], gridPosition.Y + directions[i, 1]);
                if (IsInBounds(neighborPosition))
                {
                    neighbors.Add(new NavigationCell(neighborPosition, _walkable[neighborPosition.X, neighborPosition.Y]));
                }
            }

            return neighbors;
        }

        public bool IsInBounds(int x, Vector3Int position) =>
            position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height;

        /// <summary>
        /// Checks if a grid position is within bounds.
        /// Adapts single-parameter bounds checking calls to the canonical IsInBounds implementation.
        /// </summary>
        /// <param name="position">Grid position to check.</param>
        /// <returns>True if position is within grid bounds.</returns>
        public bool IsInBounds(Vector3Int position)
        {
            return position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height;
        }

        // Placeholder: detect if a cell is occupied (implementation project-specific)
        private bool IsOccupied(Vector3Int pos) => false;

        /// <summary>
        /// Sets a cell as occupied or unoccupied.
        /// Adapts multi-parameter occupancy calls to the grid occupancy system.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="gridSize">Size of the grid area to mark.</param>
        /// <param name="occupied">Whether the area is occupied.</param>
        public void SetOccupied(int x, int y, Vector3Int gridSize, bool occupied)
        {
            // Mark the area covered by the grid size as occupied/unoccupied
            for (int dx = 0; dx < gridSize.X; dx++)
            {
                for (int dy = 0; dy < gridSize.Y; dy++)
                {
                    var pos = new Vector3Int(x + dx, y + dy);
                    if (IsInBounds(pos))
                    {
                        // This would need to be integrated with an actual occupancy system
                        // For now, this is a placeholder implementation
                    }
                }
            }
        }

        internal bool IsOccupied(int x, int y)
        {
            throw new NotImplementedException();
        }

        internal bool IsInBounds(int x, int y)
        {
            throw new NotImplementedException();
        }

        internal TerrainType GetTerrainType(Vector3Int gridPosition)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Represents a cell in the navigation grid.
    /// </summary>
    public class NavigationCell
    {
        internal int FCost;

        public Vector3Int Position { get; }
        public bool IsWalkable { get; }
        public float MovementCost { get; }

        // Updated to class to avoid cyclic dependency
        public float GCost { get; set; }
        public float HCost { get; set; }
        public NavigationCell Parent { get; set; }
        public int HeapIndex { get; set; }

        /// <summary>
        /// Gets or sets the grid position of the cell.
        /// </summary>
        public Vector3Int GridPosition { get; set; }

        public NavigationCell(Vector3Int position, bool isWalkable, float movementCost = 1.0f)
        {
            Position = position;
            IsWalkable = isWalkable;
            MovementCost = movementCost;
            GridPosition = position;
            GCost = 0;
            HCost = 0;
            Parent = null;
            HeapIndex = 0;
        }

        internal bool IsInOpenSet()
        {
            throw new NotImplementedException();
        }
    }
}

/// <summary>
/// Statistics for navigation grid analysis.
/// </summary>
public class NavigationGridStats
{
    /// <summary>
    /// Total number of cells in the grid.
    /// </summary>
    public int TotalCells { get; set; }

    /// <summary>
    /// Number of blocked cells.
    /// </summary>
    public int BlockedCells { get; set; }

    /// <summary>
    /// Number of occupied cells.
    /// </summary>
    public int OccupiedCells { get; set; }

    /// <summary>
    /// Number of free cells.
    /// </summary>
    public int FreeCells { get; set; }

    /// <summary>
    /// Ratio of occupied cells (0.0 to 1.0).
    /// </summary>
    public float OccupancyRatio { get; set; }
}
