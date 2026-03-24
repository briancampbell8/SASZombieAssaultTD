/*
File:    FlowField.cs
Purpose: P11-15-07 - Basic flow field generation for many agents.
*/
using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SASZombieAssaultTD.Engine.Navigation
{
    /// <summary>
    /// Optimized flow field generation for many agents sharing a common goal.
    /// Provides efficient direction-based navigation for large groups of agents.
    /// </summary>
    public sealed class FlowField
    {
        private readonly NavigationGrid _grid;
        private readonly Vector3Int[,] _directions;
        private readonly float[,] _costs;
        private readonly bool[,] _valid;
        private Vector3Int _targetPosition;
        private bool _isGenerated;
        private readonly int _width;
        private readonly int _height;

        /// <summary>
        /// Gets the target position of this flow field.
        /// </summary>
        public Vector3Int TargetPosition => _targetPosition;

        /// <summary>
        /// Gets whether the flow field has been generated.
        /// </summary>
        public bool IsGenerated => _isGenerated;

        /// <summary>
        /// Gets the width of the flow field.
        /// </summary>
        public int Width => _width;

        /// <summary>
        /// Gets the height of the flow field.
        /// </summary>
        public int Height => _height;

        /// <summary>
        /// Initializes a new FlowField.
        /// </summary>
        /// <param name="grid">The navigation grid to use.</param>
        public FlowField(NavigationGrid grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
            _width = grid.Width;
            _height = grid.Height;
            _directions = new Vector3Int[_height, _width];
            _costs = new float[_height, _width];
            _valid = new bool[_height, _width];
            _targetPosition = Vector3Int.Zero;
            _isGenerated = false;

            ModernLoggingSystem.Log("INFO", "FlowField: Initialized");
        }

        /// <summary>
        /// P11-15-07: Generates a flow field toward a target position.
        /// </summary>
        /// <param name="targetPosition">Target position in grid coordinates.</param>
        public async Task GenerateAsync(Vector3Int targetPosition, CancellationToken cancellationToken = default)
        {
            if (!_grid.IsInBounds(targetPosition))
            {
                ModernLoggingSystem.Log("WARNING", $"FlowField: Target position {targetPosition} is out of bounds");
                return;
            }

            _targetPosition = targetPosition;

            try
            {
                // Reset flow field
                ResetFlowField();
                await Task.Run(() => GenerateFlowFieldDijkstra(targetPosition, cancellationToken), cancellationToken);

                _isGenerated = true;
                ModernLoggingSystem.Log("DEBUG", $"FlowField: Generated flow field to target {targetPosition}");
            }
            catch (OperationCanceledException)
            {
                ModernLoggingSystem.Log("INFO", "FlowField: Flow field generation was canceled");
                _isGenerated = false;
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"FlowField: Error generating flow field: {ex.Message}");
                _isGenerated = false;
            }
        }

        /// <summary>
        /// Gets the flow direction at a specific grid position.
        /// </summary>
        /// <param name="gridPosition">Grid position to query.</param>
        /// <returns>Flow direction vector, or Vector3Int.Zero if no valid direction.</returns>
        public Vector3Int GetDirection(Vector3Int gridPosition)
        {
            if (!_isGenerated || !_grid.IsInBounds(gridPosition))
                return Vector3Int.Zero;

            return _directions[gridPosition.Y, gridPosition.X];
        }

        /// <summary>
        /// Gets the flow cost at a specific grid position.
        /// </summary>
        /// <param name="gridPosition">Grid position to query.</param>
        /// <returns>Flow cost, or float.MaxValue if invalid.</returns>
        public float GetCost(Vector3Int gridPosition)
        {
            if (!_isGenerated || !_grid.IsInBounds(gridPosition))
                return float.MaxValue;

            return _costs[gridPosition.Y, gridPosition.X];
        }

        /// <summary>
        /// Checks if a position has a valid flow direction.
        /// </summary>
        /// <param name="gridPosition">Grid position to check.</param>
        /// <returns>True if position has valid flow direction.</returns>
        public bool IsValid(Vector3Int gridPosition)
        {
            if (!_isGenerated || !_grid.IsInBounds(gridPosition))
                return false;

            return _valid[gridPosition.Y, gridPosition.X];
        }

        /// <summary>
        /// Gets all valid flow directions in the field.
        /// </summary>
        /// <returns>Collection of valid flow directions.</returns>
        public IEnumerable<(Vector3Int position, Vector3Int direction)> GetValidDirections()
        {
            if (!_isGenerated)
                yield break;

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_valid[y, x])
                    {
                        yield return (new Vector3Int(x, y), _directions[y, x]);
                    }
                }
            }
        }

        /// <summary>
        /// Clears the flow field.
        /// </summary>
        public void Clear()
        {
            ResetFlowField();
            _isGenerated = false;
            ModernLoggingSystem.Log("DEBUG", "FlowField: Cleared flow field");
        }

        /// <summary>
        /// Resets all flow field data.
        /// </summary>
        private void ResetFlowField()
        {
            Parallel.For(0, _height, y =>
            {
                for (int x = 0; x < _width; x++)
                {
                    _directions[y, x] = Vector3Int.Zero;
                    _costs[y, x] = float.MaxValue;
                    _valid[y, x] = false;
                }
            });
        }

        /// <summary>
        /// Generates flow field using Dijkstra's algorithm.
        /// </summary>
        /// <param name="targetPosition">Target position in grid coordinates.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        private void GenerateFlowFieldDijkstra(Vector3Int targetPosition, CancellationToken cancellationToken)
        {
            var targetCell = _grid.GetCell(targetPosition);
            if (!targetCell.IsWalkable)
                return;

            var queue = new ConcurrentQueue<FlowFieldNode>();
            var visited = new bool[_height, _width];

            // Initialize target cell
            targetCell.GCost = 0;
            queue.Enqueue(new FlowFieldNode(targetPosition, 0));
            visited[targetPosition.Y, targetPosition.X] = true;

            // Dijkstra's algorithm
            while (queue.TryDequeue(out var current))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var currentPos = current.Position;

                // Set flow direction for this cell
                if (currentPos != targetPosition)
                {
                    var direction = CalculateDirection(currentPos, current.Parent);
                    _directions[currentPos.Y, currentPos.X] = direction;
                    _costs[currentPos.Y, currentPos.X] = current.Cost;
                    _valid[currentPos.Y, currentPos.X] = true;
                }

                // Explore neighbors
                foreach (var neighbor in GetNeighborsForFlowField(currentPos))
                {
                    var neighborPos = neighbor.GridPosition;
                    var neighborCell = _grid.GetCell(neighborPos);

                    if (!visited[neighborPos.Y, neighborPos.X] && neighborCell.IsWalkable)
                    {
                        var newCost = current.Cost + neighborCell.MovementCost;
                        var existingCost = _costs[neighborPos.Y, neighborPos.X];

                        if (newCost < existingCost)
                        {
                            neighborCell.GCost = newCost;
                            queue.Enqueue(new FlowFieldNode(neighborPos, newCost));
                            visited[neighborPos.Y, neighborPos.X] = true;
                        }
                    }
                }
            }

            // Set target cell direction to zero
            _directions[targetPosition.Y, targetPosition.X] = Vector3Int.Zero;
            _costs[targetPosition.Y, targetPosition.X] = 0;
            _valid[targetPosition.Y, targetPosition.X] = true;
        }

        /// <summary>
        /// Gets neighbors suitable for flow field generation.
        /// </summary>
        /// <param name="position">Position to get neighbors for.</param>
        /// <returns>Collection of neighbor cells.</returns>
        private IEnumerable<NavigationCell> GetNeighborsForFlowField(Vector3Int position)
        {
            // Use 4-way connectivity for flow fields (more predictable)
            var directions = new[]
            {
                new Vector3Int(0, 1),   // Up
                new Vector3Int(1, 0),   // Right
                new Vector3Int(0, -1),  // Down
                new Vector3Int(-1, 0)   // Left
            };

            foreach (var direction in directions)
            {
                var neighborPos = position + direction;
                if (_grid.IsInBounds(neighborPos))
                {
                    var cell = _grid.GetCell(neighborPos);
                    if (cell != null)
                        yield return new NavigationCell(neighborPos, true, 0f);
                }
            }
        }

        /// <summary>
        /// Calculates direction from one position to another.
        /// </summary>
        /// <param name="from">Starting position.</param>
        /// <param name="to">Target position.</param>
        /// <returns>Direction vector.</returns>
        private Vector3Int CalculateDirection(Vector3Int from, Vector3Int to)
        {
            var diff = to - from;

            // Normalize to cardinal directions
            return System.Math.Abs(diff.X) > System.Math.Abs(diff.Y)
                ? new Vector3Int(System.Math.Sign(diff.X), 0)
                : new Vector3Int(0, System.Math.Sign(diff.Y));
        }

        /// <summary>
        /// Gets debug information about the flow field.
        /// </summary>
        /// <returns>Debug information string.</returns>
        public string GetDebugInfo()
        {
            return $"FlowField Debug Info:\n" +
                   $"  Dimensions: {_width}x{_height}\n" +
                   $"  Target Position: {_targetPosition}\n" +
                   $"  Is Generated: {_isGenerated}\n" +
                   (_isGenerated ? GetGeneratedFieldStats() : string.Empty);
        }

        /// <summary>
        /// Gets generated field statistics for debug info.
        /// </summary>
        /// <returns>Statistics string.</returns>
        private string GetGeneratedFieldStats()
        {
            var validCount = 0;
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_valid[y, x])
                        validCount++;
                }
            }

            return $"  Valid Cells: {validCount}/{_width * _height}\n" +
                   $"  Valid Percentage: {(validCount * 100.0 / (_width * _height)):F1}%\n";
        }

        /// <summary>
        /// Node for flow field generation.
        /// </summary>
        private sealed class FlowFieldNode
        {
            public Vector3Int Position { get; }
            public float Cost { get; }
            public Vector3Int Parent { get; }

            public FlowFieldNode(Vector3Int position, float cost)
            {
                Position = position;
                Cost = cost;
                Parent = Vector3Int.Zero;
            }
        }
    }
}




