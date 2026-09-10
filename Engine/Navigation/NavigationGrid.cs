// ====================================================================================================
//  FILE: NavigationGrid.cs
//  PATH: Engine/Navigation/NavigationGrid.cs
//  SUBSYSTEM: Navigation
//
//  ROLE:
//      NavigationGrid Pipeline Manager.
//      Provides the gameplay-facing navigation API and orchestrates the pipeline between
//      NavigationGridCore (grid storage), NavigationGridUtility (stateless helpers),
//      and NavigationGridStats (analytics).
//
//  RESPONSIBILITIES:
//      - Maintain a singleton instance for gameplay systems.
//      - Manage high-level navigation state (CurrentPath, HasPath, occupancy flags, spawn points).
//      - Delegate cell access to NavigationGridCore.
//      - Delegate coordinate conversion and neighbor retrieval to NavigationGridUtility.
//      - Delegate analytics to NavigationGridStats.
//      - Provide gameplay-level operations such as IsWalkable, SetWalkable, UpdateEntityArea, SetOccupied.
//      - Emit DLogger.Log trace statements for pipeline-level operations.
//
//  NON-RESPONSIBILITIES:
//      - Storing grid cells (handled by NavigationGridCore).
//      - Performing low-level math (handled by NavigationGridUtility).
//      - Computing statistics (handled by NavigationGridStats).
//      - Performing A* pathfinding (handled by AStarPathfinder).
//
//  ARCHITECTURAL NOTES:
//      - Must remain a thin pipeline manager.
//      - Must not duplicate low-level logic from Core or Utility.
//      - Must not mutate NavigationGridCore cell arrays directly.
// ====================================================================================================

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Navigation
{
    /// <summary>
    /// NavigationGrid Pipeline Manager. Orchestrates NavigationGridCore, NavigationGridUtility, and
    /// NavigationGridStats.
    /// </summary>
    public sealed class NavigationGrid
    {
        // ----------------------------------------------------------------------------------------------------
        // Singleton Pipeline Manager
        // ----------------------------------------------------------------------------------------------------
        private static NavigationGrid? _instance;

        public static NavigationGrid Instance => _instance ??= new NavigationGrid();

        private NavigationGridCore _core;
        private int v1;
        private int v2;
        private object cellSize;
        private object seed;

        // ----------------------------------------------------------------------------------------------------
        // Gameplay-Level State
        // ----------------------------------------------------------------------------------------------------
        public IReadOnlyList<Vector3Int> CurrentPath { get; private set; } = Array.Empty<Vector3Int>();

        public bool HasPath { get; private set; }
        public bool IsInitialized { get; private set; }

        //public bool IsTileOccupied { get; private set; }
        // Change this:
        // public bool IsTileOccupied { get; private set; }

        // To this:



        public bool IsOccupied { get; private set; }
        private readonly List<Vector3> _spawnPoints = new();
        public IReadOnlyList<Vector3> SpawnPoints => _spawnPoints;

        public int Width { get; internal set; }
        public int Height { get; internal set; }
        public int CellSize { get; internal set; }
        public Vector3 WorldOrigin { get; set; }
        public int[,] Terrain { get; internal set; }



        // ----------------------------------------------------------------------------------------------------
        // Initialization
        // ----------------------------------------------------------------------------------------------------
        private NavigationGrid(int x = 0, int y = 0)
        {
            Width = x;
            Height = y;
        }

        public NavigationGrid()
        {
            int width = 0;
            int height = 0;
            Width = width;
            Height = height;
        }

        public NavigationGrid(int v1, int v2, object cellSize, object seed)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.cellSize = cellSize;
            this.seed = seed;
        }

        public class UseNavigationGrid()
        {

            public object GetRandomNavigablePoint { get; private set; }
        }

        public void InitializeInstance(int width, int height, float cellSize = 1.0f)
        {
            _core = new NavigationGridCore(width, height, cellSize);
            IsInitialized = true;

            DLogger.Log(
                $"Navigation.NavigationGrid.InitializeInstance: Width={width} Height={height} CellSize={cellSize}");
        }

        public void ClearInstance()
        {
            _core = null;
            IsInitialized = false;
            CurrentPath = Array.Empty<Vector3Int>();
            HasPath = false;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Navigation.NavigationGrid.ClearInstance: Core cleared.");
        }

        // ----------------------------------------------------------------------------------------------------
        // Pipeline Delegation: Core Access
        // ----------------------------------------------------------------------------------------------------
        public NavigationCellGridCore GetCell(int x, int y)
        {
            return _core?.GetCell(x, y);
        }

        public NavigationCellGridCore GetCell(Vector3Int pos)
        {
            return _core?.GetCell(pos);
        }

        public bool IsInBounds(int x, int y)
        {
            return _core?.IsInBounds(x, y) ?? false;
        }

        public bool IsInBounds(Vector3Int pos)
        {
            return _core?.IsInBounds(pos) ?? false;
        }
        public bool IsTileOccupied(int x, int y)
        {
            return _core?.IsTileOccupied(x, y) ?? false;
        }

        public bool IsTileOccupied(Vector3Int pos)
        {
            return _core?.IsTileOccupied(pos) ?? false;
        }
        // ----------------------------------------------------------------------------------------------------
        // Pipeline Delegation: Utility Operations
        // ----------------------------------------------------------------------------------------------------
        public Vector3Int WorldToGrid(Vector3 worldPosition)
        {
            return NavigationGridUtility.WorldToGrid(worldPosition, _core.CellSize);
        }

        public Vector3 GridToWorld(Vector3Int gridPosition)
        {
            return NavigationGridUtility.GridToWorld(gridPosition, _core.CellSize);
        }

        public List<NavigationCellGridCore> GetNeighbors(Vector3Int pos, bool allowDiagonal)
        {
            return NavigationGridUtility.GetNeighbors(_core, pos, allowDiagonal);
        }

        // ----------------------------------------------------------------------------------------------------
        // Gameplay-Level Operations
        // ----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Determines whether the specified cell is walkable.
        /// </summary>
        public bool IsWalkable(int x, int y)
        {
            var pos = new Vector3Int(x, y, 0);
            var cell = _core?.GetCell(pos);
            return cell != null && cell.IsWalkable;
        }

        /// <summary>
        /// Determines whether the specified cell is walkable.
        /// </summary>
        public bool IsWalkable(Vector3Int pos)
        {
            var cell = _core?.GetCell(pos);
            return cell != null && cell.IsWalkable;
        }

        /// <summary>
        /// Determines whether the specified cell is walkable.
        /// </summary>
        public bool IsWalkable(int x, int y, Vector3Int pos)
        {
            var cell = _core?.GetCell(pos);
            return !(cell == null || !cell.IsWalkable);
        }

        /// <summary>
        /// Determines whether the specified cell is walkable.
        /// </summary>
        public bool IsWalkable(int x, int y, Vector3 worldPos)
        {
            return IsWalkable(
                x,
                y,
                WorldToGrid(worldPos));
        }

        public void SetWalkable(Vector3Int pos, bool walkable)
        {
            var cell = _core?.GetCell(pos);
            if (cell != null)
            {
                cell.IsWalkable = walkable;

                DLogger.Log(
                    $"Navigation.NavigationGrid.SetWalkable: Pos=({pos.X},{pos.Y}) Walkable={walkable}");
            }
        }

        public void UpdateEntityArea(Vector3 worldPos, Vector3 size, bool walkable)
        {
            var start = WorldToGrid(worldPos);

            for (int x = 0; x < (int)size.X; x++)
            {
                for (int y = 0; y < (int)size.Y; y++)
                {
                    var pos = new Vector3Int(start.X + x, start.Y + y, 0);

                    if (IsInBounds(pos))
                        SetWalkable(pos, walkable);
                }
            }

            DLogger.Log(
                $"Navigation.NavigationGrid.UpdateEntityArea: Start=({start.X},{start.Y}) Size=({size.X},{size.Y}) Walkable={walkable}");
        }

        public void SetOccupied(int startX, int startY, Vector3Int size, bool occupied)
        {
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    var pos = new Vector3Int(startX + x, startY + y, 0);

                    if (IsInBounds(pos))
                        SetWalkable(pos, !occupied);
                }
            }

            DLogger.Log(
                $"Navigation.NavigationGrid.SetOccupied: Start=({startX},{startY}) Size=({size.X},{size.Y}) Occupied={occupied}");
        }

        // ----------------------------------------------------------------------------------------------------
        // Path State
        // ----------------------------------------------------------------------------------------------------
        public void ClearPath()
        {
            CurrentPath = Array.Empty<Vector3Int>();
            HasPath = false;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Navigation.NavigationGrid.ClearPath: Path cleared.");
        }

        // ----------------------------------------------------------------------------------------------------
        // Pipeline Delegation: Stats
        // ----------------------------------------------------------------------------------------------------
        public NavigationGridStats GetStats()
        {
            return new NavigationGridStats(_core, this);
        }

        internal Vector3 GetRandomNavigablePoint()
        {
            // Prefer configured spawn points if any exist
            if (_spawnPoints != null && _spawnPoints.Count > 0)
            {
                var rnd = new Random();
                int idx = rnd.Next(0, _spawnPoints.Count);
                return _spawnPoints[idx];
            }

            // Otherwise, attempt to find a random walkable tile within the grid bounds
            if (_core != null && Width > 0 && Height > 0)
            {
                var rnd = new Random();
                const int maxAttempts = 50;
                for (int i = 0; i < maxAttempts; i++)
                {
                    int x = rnd.Next(0, Width);
                    int y = rnd.Next(0, Height);
                    if (IsWalkable(x, y))
                    {
                        var gridPos = new Vector3Int(x, y, 0);
                        return GridToWorld(gridPos);
                    }
                }
            }

            // If nothing found, return the world origin as a safe fallback
            return WorldOrigin;
        }
    }
}
