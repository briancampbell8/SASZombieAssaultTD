using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Collections.Generic;
using System.Linq;
using TransformComponent = SASZombieAssaultTD.Engine.Components.TransformComponent;

namespace SASZombieAssaultTD.Engine.Physics
{
    /// <summary>
    /// Debug visualization system for collision detection.
    /// Renders collision shapes, spatial grid cells, and collision contacts.
    /// </summary>
    public sealed class CollisionDebugRenderer
    {
        readonly ECSWorld _ecsWorld;
        object statsText, showContacts, showBounds, showShapes, showGrid;

        public bool Enabled { get; set; }
        public bool ShowGrid { get; set; } = true;
        public bool ShowShapes { get; set; } = true;
        public bool ShowContacts { get; set; } = true;
        public bool ShowBounds { get; set; }
        public bool ShowStats { get; set; } = true;

        public uint GridColor { get; set; } = 0x40404040;
        public uint ShapeColor { get; set; } = 0x40FF4040;
        public uint BoundsColor { get; set; } = 0x4040FFFF;
        public uint ContactColor { get; set; } = 0xFFFF4040;
        public uint TriggerColor { get; set; } = 0x40FFFF40;

        public CollisionDebugRenderer(ECSWorld ecsWorld)
        {
            _ecsWorld = ecsWorld ?? throw new ArgumentNullException(nameof(ecsWorld));
            ModernLoggingSystem.Log("INFO", "CollisionDebugRenderer: Initialized");
        }

        public void Render(IRenderContext context)
        {
            if (!Enabled || context == null) return;

            try
            {
                if (ShowGrid) RenderSpatialGrid(context);
                if (ShowShapes || ShowBounds) RenderCollisionShapes(context);
                if (ShowStats) RenderDebugStats(context);
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"CollisionDebugRenderer: Error during rendering: {ex.Message}");
            }
        }

        void RenderSpatialGrid(IRenderContext context)
        {
            var grid = _ecsWorld.SpatialGrid;

            var stats = new SpatialGridStats
            {
                TotalCells = 10000,
                OccupiedCells = 0,
                MaxEntitiesPerCell = 0,
                AverageEntitiesPerCell = 0f,
                GridWidth = 100,
                GridHeight = 100,
                CellSize = 100
            };

            var worldMin = new Vector3(-1000, -1000, 0);
            var cellSize = stats.CellSize;

            for (int x = 0; x < stats.GridWidth; x++)
            {
                for (int y = 0; y < stats.GridHeight; y++)
                {
                    var cellMin = worldMin + new Vector3(x * cellSize, y * cellSize, 0);
                    var cellMax = cellMin + new Vector3(cellSize, cellSize, 0);
                    var cellIndex = y * stats.GridWidth + x;

                    var isOccupied = _ecsWorld.GetEntitiesInArea(cellMin, cellMax).Any();
                    var color = isOccupied ? 0x60606060 : GridColor;

                    var rect = Rectangle.FromPositionAndSize(cellMin.X, cellMin.Y, cellSize, cellSize);
                    context.FillRectangle(rect, new Color(color));
                }
            }

            var boundsRect = Rectangle.FromPositionAndSize(worldMin.X, worldMin.Y, 2000, 2000);
            context.DrawRectangle(boundsRect, new Color(0xFFFFFFFF), 2.0f);
        }

        void RenderCollisionShapes(IRenderContext context)
        {
            foreach (var entity in _ecsWorld.GetEntitiesWith<ColliderComponent, TransformComponent>())
            {
                var collider = entity.GetComponent<ColliderComponent>();
                var transform = entity.GetComponent<TransformComponent>();

                if (collider?.Shape == null || transform == null || !collider.Enabled) continue;

                var worldShape = collider.GetWorldShape();
                if (worldShape == null) continue;

                if (ShowBounds)
                {
                    var bounds = worldShape.Bounds;
                    var boundsRect = Rectangle.FromPositionAndSize(bounds.Min.X, bounds.Min.Y, bounds.Width, bounds.Height);
                    context.DrawRectangle(boundsRect, Color.FromUint(BoundsColor));
                }

                if (ShowShapes)
                {
                    RenderShape(context, worldShape, transform.Position, collider.IsTrigger);
                }
            }
        }

        void RenderShape(IRenderContext context, CollisionShape shape, Vector3 position, bool isTrigger)
        {
            var color = isTrigger ? TriggerColor : ShapeColor;

            switch (shape.ShapeType)
            {
                case CollisionShapeType.Circle:
                    RenderCircle(context, (CircleShape)shape, position, color);
                    break;

                case CollisionShapeType.AABB:
                    RenderAABB(context, (AABBShape)shape, position, color);
                    break;

                case CollisionShapeType.Capsule:
                    RenderCapsule(context, (CapsuleShape)shape, position, color);
                    break;
            }
        }

        void RenderCircle(IRenderContext context, CircleShape circle, Vector3 position, uint color)
        {
            var centerPos = new Vector3(position.X + circle.Center.X, position.Y + circle.Center.Y, 0);
            context.DrawCircle(centerPos, circle.Radius, new Color(color));
        }

        void RenderAABB(IRenderContext context, AABBShape aabb, Vector3 position, uint color)
        {
            var min = position + aabb.Min;
            var size = aabb.Size;
            var rect = Rectangle.FromPositionAndSize(min.X, min.Y, size.X, size.Y);
            context.DrawRectangle(rect, new Color(color));
        }

        void RenderCapsule(IRenderContext context, CapsuleShape capsule, Vector3 position, uint color)
        {
            var halfHeight = capsule.HalfHeight;
            var radius = capsule.Radius;
            var rectMin = position + new Vector3(-radius, -halfHeight, 0);
            var rectSize = new Vector3(radius * 2, capsule.Height, 0);

            var capsuleRect = Rectangle.FromPositionAndSize(rectMin.X, rectMin.Y, rectSize.X, rectSize.Y);
            context.DrawRectangle(capsuleRect, new Color(color));

            var topCenter = new Vector3(position.X + capsule.GetTopHemisphereCenter().X, position.Y + capsule.GetTopHemisphereCenter().Y, 0);
            var bottomCenter = new Vector3(position.X + capsule.GetBottomHemisphereCenter().X, position.Y + capsule.GetBottomHemisphereCenter().Y, 0);
            context.DrawCircle(topCenter, radius, new Color(color));
            context.DrawCircle(bottomCenter, radius, new Color(color));
        }

        void RenderDebugStats(IRenderContext context)
        {
            var collisionSystem = _ecsWorld.GetSystem<CollisionSystem>();
            var gridStats = _ecsWorld.SpatialGrid.GetStats();

            // 1. Rename to debugOutput
            // 2. Add () to MaxEntitiesPerCell if it is a method (it has a red line in your image)
            var debugOutput =
             $"  Actual Collisions: {collisionSystem?.ActualCollisions ?? 0}\n" +
             "\nSpatial Grid Stats:\n" +
             // Change line 186 to this:
             $"  Total Cells: {(gridStats != null ? ObjectExtensions.TotalCells(gridStats) : 0)}\n" +
             $"  Occupied Cells: {(gridStats != null ? ObjectExtensions.OccupiedCells(gridStats) : 0)}\n" +
             $"  Total Entities: {(gridStats != null ? ObjectExtensions.TotalEntities(gridStats) : 0)}\n" +
             $" Avg Entities/Cell: {(gridStats != null ? ObjectExtensions.AverageEntitiesPerCell(gridStats).ToString("F2") : "0.00")}\n" +
             $"  Max Entities/Cell: {(gridStats != null ? ObjectExtensions.MaxEntitiesPerCell(gridStats) : 0)}\n" +
             $"\nDebug Options: Grid={ShowGrid} Shapes={ShowShapes} Contacts={ShowContacts} Bounds={ShowBounds}";

            // 3. Update the loop to use the new name
            var y = 10;

            foreach (var line in debugOutput.Split('\n'))
            {
                context.DrawText(line, 10, y);
                y += 15;
            }
        }

        IEnumerable<object> Split(object statsText)
        {
            throw new NotImplementedException();
        }

        public string GetDebugInfo() =>
            $"CollisionDebugRenderer Debug Info:\n" +
            $"  Enabled: {Enabled}\n" +
            $"  Show Grid: {ShowGrid}\n" +
            $"  Show Shapes: {ShowShapes}\n" +
            $"  Show Contacts: {ShowContacts}\n" +
            $"  Show Bounds: {ShowBounds}\n" +
            $"  Show Stats: {ShowStats}\n";

        public void ToggleAll()
        {
            ShowGrid = !ShowGrid;
            ShowShapes = !ShowShapes;
            ShowContacts = !ShowContacts;
            ShowBounds = !ShowBounds;
            ShowStats = !ShowStats;
        }

        public void ResetToDefaults()
        {
            ShowGrid = true;
            ShowShapes = true;
            ShowContacts = true;
            ShowBounds = false;
            ShowStats = true;
        }
    }
}

/// <summary>
/// Statistics for spatial grid analysis.
/// </summary>
public class SpatialGridStats
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
    /// Total entities in the grid.
    /// </summary>
    public int TotalEntities { get; set; }

    /// <summary>
    /// Average entities per cell.
    /// </summary>
    public float AverageEntitiesPerCell { get; set; }

    /// <summary>
    /// Maximum entities per cell.
    /// </summary>
    public int MaxEntitiesPerCell { get; set; }

    /// <summary>
    /// Grid width in cells.
    /// </summary>
    public int GridWidth { get; set; }

    /// <summary>
    /// Grid height in cells.
    /// </summary>
    public int GridHeight { get; set; }

    /// <summary>
    /// Cell size.
    /// </summary>
    public float CellSize { get; set; }
}

/// <summary>
/// Utility class for parsing grid statistics.
/// </summary>
public static class GridStatsParser
{
    /// <summary>
    /// Parses grid statistics from debug text.
    /// </summary>
    /// <param name="debugText">Debug text to parse.</param>
    /// <returns>Parsed grid statistics.</returns>
    public static SpatialGridStats ParseGridStats(string debugText)
    {
        var stats = new SpatialGridStats();

        if (string.IsNullOrEmpty(debugText)) return stats;

        var lines = debugText.Split('\n');

        foreach (var line in lines)
        {
            if (line.StartsWith("  Total Cells: "))
            {
                if (int.TryParse(line.Substring(13), out var totalCells))
                    stats.TotalCells = totalCells;
            }
            else if (line.StartsWith("  Occupied Cells: "))
            {
                if (int.TryParse(line.Substring(18), out var occupiedCells))
                    stats.OccupiedCells = occupiedCells;
            }
            else if (line.StartsWith("  Total Entities: "))
            {
                if (int.TryParse(line.Substring(18), out var totalEntities))
                    stats.TotalEntities = totalEntities;
            }
            else if (line.StartsWith("  Avg Entities/Cell: "))
            {
                if (float.TryParse(line.Substring(20), out var avgEntities))
                    stats.AverageEntitiesPerCell = avgEntities;
            }
            else if (line.StartsWith("  Max Entities/Cell: "))
            {
                if (int.TryParse(line.Substring(20), out var maxEntities))
                    stats.MaxEntitiesPerCell = maxEntities;
            }
        }

        stats.FreeCells = stats.TotalCells - stats.BlockedCells - stats.OccupiedCells;

        return stats;
    }
}