using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Physics
{
    /// <summary>
    /// Uniform grid spatial partitioning structure optimized for 2D tower defense maps.
    /// Provides efficient spatial queries and collision pair generation.
    /// </summary>
    public sealed class SpatialPartitionGrid
    {
        private readonly Dictionary<int, GridCell> _cells = new();
        private readonly Dictionary<uint, GridEntry> _entityEntries = new();
        private readonly float _cellSize;
        private readonly int _gridWidth;
        private readonly int _gridHeight;
        private readonly Vector3 _worldMin;
        private readonly Vector3 _worldMax;

        public float CellSize => _cellSize;
        public int CellCount => _cells.Count;
        public int EntityCount => _entityEntries.Count;

        public SpatialPartitionGrid(Vector3 worldMin, Vector3 worldMax, float cellSize)
        {
            if (cellSize <= 0)
                throw new ArgumentException("Cell size must be positive", nameof(cellSize));

            _worldMin = worldMin;
            _worldMax = worldMax;
            _cellSize = cellSize;

            var worldSize = worldMax - worldMin;
            _gridWidth = System.Math.Max(1, (int)System.Math.Ceiling(worldSize.X / cellSize));
            _gridHeight = System.Math.Max(1, (int)System.Math.Ceiling(worldSize.Y / cellSize));

            LogInfo($"Created {_gridWidth}x{_gridHeight} grid with {_cellSize:F1} cell size");
        }

        public void Insert(Entity entity, BoundingBox bounds)
        {
            ArgumentNullException.ThrowIfNull(entity);

            Remove(entity);

            var cellIndices = GetCellIndices(bounds);
            var entry = new GridEntry(entity, bounds, cellIndices);

            _entityEntries[entity.Id] = entry;

            foreach (var cellIndex in cellIndices)
            {
                if (!_cells.TryGetValue(cellIndex, out var cell))
                {
                    cell = new GridCell(cellIndex);
                    _cells[cellIndex] = cell;
                }

                cell.Entities.Add(entity);
            }

            LogDebug($"Inserted entity {entity.Id} into {cellIndices.Count} cells");
        }

        public bool Remove(Entity entity)
        {
            if (entity == null || !_entityEntries.Remove(entity.Id, out var entry))
                return false;

            foreach (var cellIndex in entry.CellIndices)
            {
                if (_cells.TryGetValue(cellIndex, out var cell))
                {
                    cell.Entities.Remove(entity);
                    if (cell.Entities.Count == 0)
                        _cells.Remove(cellIndex);
                }
            }

            LogDebug($"Removed entity {entity.Id}");
            return true;
        }

        public bool Update(Entity entity, BoundingBox newBounds)
        {
            if (entity == null || !_entityEntries.TryGetValue(entity.Id, out var entry))
                return false;

            if (entry.Bounds.Equals(newBounds))
            {
                entry.LastUpdate = DateTime.UtcNow;
                return true;
            }

            Remove(entity);
            Insert(entity, newBounds);

            LogDebug($"Updated entity {entity.Id} position");
            return true;
        }

        public IEnumerable<Entity> Query(BoundingBox area)
        {
            var cellIndices = GetCellIndices(area);
            var result = new HashSet<Entity>();

            foreach (var cellIndex in cellIndices)
            {
                if (_cells.TryGetValue(cellIndex, out var cell))
                {
                    foreach (var entity in cell.Entities)
                    {
                        if (_entityEntries.TryGetValue(entity.Id, out var entry) && entry.Bounds.Intersects(area))
                        {
                            result.Add(entity);
                        }
                    }
                }
            }

            return result;
        }

        public IEnumerable<(Entity, Entity)> GetPotentialCollisions()
        {
            var processedPairs = new HashSet<(uint, uint)>();
            var result = new List<(Entity, Entity)>();

            foreach (var cell in _cells.Values)
            {
                var entities = cell.Entities.ToList();

                for (int i = 0; i < entities.Count; i++)
                {
                    for (int j = i + 1; j < entities.Count; j++)
                    {
                        var entityA = entities[i];
                        var entityB = entities[j];
                        var pair = CreateOrderedPair(entityA.Id, entityB.Id);

                        if (processedPairs.Add(pair))
                        {
                            result.Add((entityA, entityB));
                        }
                    }
                }
            }

            return result;
        }

        public IEnumerable<Entity> GetEntitiesInRadius(Vector3 center, float radius)
        {
            var radiusBounds = BoundingBox.FromCenterAndSize(center, new Vector3(radius * 2, radius * 2, 0));
            return Query(radiusBounds).Where(entity =>
            {
                if (!_entityEntries.TryGetValue(entity.Id, out var entry))
                    return false;

                return Vector3.Distance(center, entry.Bounds.Center) <= radius;
            });
        }

        public void Clear()
        {
            _cells.Clear();
            _entityEntries.Clear();
            LogInfo("Cleared all entities");
        }

        public SpatialGridStats GetStats()
        {
            var cellOccupancies = _cells.Values.Select(cell => cell.Entities.Count).ToList();

            return new SpatialGridStats
            {
                TotalCells = _gridWidth * _gridHeight,
                OccupiedCells = _cells.Count,
                TotalEntities = _entityEntries.Count,
                AverageEntitiesPerCell = cellOccupancies.Any() ? cellOccupancies.Average() : 0,
                MaxEntitiesPerCell = cellOccupancies.Any() ? cellOccupancies.Max() : 0,
                MinEntitiesPerCell = cellOccupancies.Any() ? cellOccupancies.Min() : 0,
                GridWidth = _gridWidth,
                GridHeight = _gridHeight,
                CellSize = _cellSize
            };
        }

        private HashSet<int> GetCellIndices(BoundingBox bounds)
        {
            var indices = new HashSet<int>();

            var clampedMin = new Vector3(System.Math.Max(bounds.Min.X, _worldMin.X), System.Math.Max(bounds.Min.Y, _worldMin.Y), System.Math.Max(bounds.Min.Z, _worldMin.Z));
            var clampedMax = new Vector3(System.Math.Min(bounds.Max.X, _worldMax.X), System.Math.Min(bounds.Max.Y, _worldMax.Y), System.Math.Min(bounds.Max.Z, _worldMax.Z));

            var minCellX = System.Math.Clamp((int)System.Math.Floor((clampedMin.X - _worldMin.X) / _cellSize), 0, _gridWidth - 1);
            var minCellY = System.Math.Clamp((int)System.Math.Floor((clampedMin.Y - _worldMin.Y) / _cellSize), 0, _gridHeight - 1);
            var maxCellX = System.Math.Clamp((int)System.Math.Floor((clampedMax.X - _worldMin.X) / _cellSize), 0, _gridWidth - 1);
            var maxCellY = System.Math.Clamp((int)System.Math.Floor((clampedMax.Y - _worldMin.Y) / _cellSize), 0, _gridHeight - 1);

            for (int x = minCellX; x <= maxCellX; x++)
            {
                for (int y = minCellY; y <= maxCellY; y++)
                {
                    indices.Add(GetCellIndex(x, y));
                }
            }

            return indices;
        }

        private int GetCellIndex(int x, int y) => y * _gridWidth + x;

        private static (uint, uint) CreateOrderedPair(uint id1, uint id2) => id1 < id2 ? (id1, id2) : (id2, id1);

        private void LogInfo(string message) => Engine.Diagnostics.DebugLogger.LogDebug("INFO", message);
        private void LogDebug(string message) => Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", message);
    }

    internal sealed class GridCell
    {
        public int Index { get; }
        public HashSet<Entity> Entities { get; } = new();

        public GridCell(int index) => Index = index;
    }

    internal sealed class GridEntry
    {
        public Entity Entity { get; }
        public BoundingBox Bounds { get; }
        public HashSet<int> CellIndices { get; }
        public DateTime LastUpdate { get; set; }

        public GridEntry(Entity entity, BoundingBox bounds, HashSet<int> cellIndices)
        {
            Entity = entity;
            Bounds = bounds;
            CellIndices = cellIndices;
            LastUpdate = DateTime.UtcNow;
        }
    }

    public sealed class SpatialGridStats
    {
        private static object TheContainingType;
        private static object TheContainingMember;

        public int TotalCells { get; set; }
        public int OccupiedCells { get; set; }
        public int TotalEntities { get; set; }
        public double AverageEntitiesPerCell { get; set; }
        public int MaxEntitiesPerCell { get; set; }
        public int MinEntitiesPerCell { get; set; }
        public int GridWidth { get; set; }
        public int GridHeight { get; set; }
        public float CellSize { get; set; }

        public override string ToString() =>
            $"Grid: {OccupiedCells}/{TotalCells} occupied, {TotalEntities} entities, avg {AverageEntitiesPerCell:F2}/cell";

        public static explicit operator SpatialGridStats(string v)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }
    }
}
