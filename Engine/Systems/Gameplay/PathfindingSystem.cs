//File: Engine/Systems/Gameplay/PathfindingSystem.cs
//Purpose: Implements a pathfinding system for navigating a grid-based map.
//Features: Supports walkability checks, cell management, and A* pathfinding algorithm.

using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Collections.Generic;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Systems.Gameplay
{
    ///<summary>
    ///Pathfinding system for navigating a grid-based map.
    ///</summary>
    public class PathfindingSystem
    {
        private readonly NavigationGrid _navigationGrid;

        ///<summary>
        ///Initializes a new instance of the PathfindingSystem class.
        ///</summary>
        ///<param name="width">Width of the navigation grid.</param>
        ///<param name="height">Height of the navigation grid.</param>
        public PathfindingSystem(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Grid dimensions must be positive.");

            _navigationGrid = new NavigationGrid(width, height);
        }

        ///<summary>
        ///Checks if a cell is walkable.
        ///</summary>
        public bool IsWalkable(int x, int y) => _navigationGrid.IsWalkable(x, y);

        ///<summary>
        ///Sets a cell as walkable or blocked.
        ///</summary>
        public void SetWalkable(int x, int y, bool walkable) =>
            _navigationGrid.SetWalkable(x, y, walkable);

        ///<summary>
        ///Finds the shortest path between two points using the A* algorithm.
        ///</summary>
        ///<param name="start">Start position.</param>
        ///<param name="end">End position.</param>
        ///<returns>A list of positions representing the path, or an empty list if no path is found.</returns>
        public List<Vector3> FindPath(Vector3 start, Vector3 end)
        {
            if (!IsValidPosition(start))
                throw new ArgumentException("Start position is invalid or not walkable.", nameof(start));

            if (!IsValidPosition(end))
                throw new ArgumentException("End position is invalid or not walkable.", nameof(end));

            var openList = new PriorityQueue<Vector3, float>();
            var cameFrom = new Dictionary<Vector3, Vector3>();
            var gScore = new Dictionary<Vector3, float> { [start] = 0 };
            var fScore = new Dictionary<Vector3, float> { [start] = Heuristic(start, end) };
            var closedSet = new HashSet<Vector3>();

            var openSet = new HashSet<Vector3>();
            openSet.Add(start);
            openList.Enqueue(start, fScore[start]);

            while (openList.Count > 0)
            {
                var current = openList.Dequeue();
                openSet.Remove(current);

                if (current.Equals(end))
                    return ReconstructPath(cameFrom, current);

                closedSet.Add(current);

                foreach (var neighbor in _navigationGrid.GetNeighbors(new Vector3((int)current.X, (int)current.Y, 0), true))
                {
                    var neighborPosition = new Vector3(neighbor.X, neighbor.Y, 0);

                    if (gScore.ContainsKey(neighborPosition))
                        continue;

                    var tentativeGScore = gScore[current] + Distance(current, neighborPosition);

                    if (!gScore.ContainsKey(neighborPosition) || tentativeGScore < gScore[neighborPosition])
                    {
                        cameFrom[neighborPosition] = current;
                        gScore[neighborPosition] = tentativeGScore;
                        fScore[neighborPosition] = tentativeGScore + Heuristic(neighborPosition, end);

                        if (!openSet.Contains(neighborPosition))
                            openList.Enqueue(neighborPosition, fScore[neighborPosition]);
                    }
                }
            }

            return new List<Vector3>();
        }

        ///<summary>
        ///Validates if a position is within bounds and walkable.
        ///</summary>
        private bool IsValidPosition(Vector3 position)
        {
            var gridPos = new Vector3Int((int)position.X, (int)position.Y);
            return _navigationGrid.IsWalkable(gridPos.X, gridPos.Y);
        }

        ///<summary>
        ///Calculates the Manhattan distance heuristic.
        ///</summary>
        private static float Heuristic(Vector3 a, Vector3 b) =>
            System.Math.Abs(a.X - b.X) + System.Math.Abs(a.Y - b.Y);

        ///<summary>
        ///Calculates the Euclidean distance between two points.
        ///</summary>
        private static float Distance(Vector3 a, Vector3 b) =>
            (float)System.Math.Sqrt(System.Math.Pow(a.X - b.X, 2) + System.Math.Pow(a.Y - b.Y, 2));

        ///<summary>
        ///Reconstructs the path from the end node to the start node.
        ///</summary>
        private static List<Vector3> ReconstructPath(Dictionary<Vector3, Vector3> cameFrom, Vector3 current)
        {
            var path = new List<Vector3> { current };
            while (cameFrom.TryGetValue(current, out var previous))
            {
                current = previous;
                path.Add(current);
            }
            path.Reverse();
            return path;
        }
    }
}
