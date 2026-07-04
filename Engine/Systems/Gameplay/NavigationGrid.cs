//File: Engine/Systems/Gameplay/NavigationGrid.cs
//Purpose: Represents a grid-based navigation system for pathfinding.
//Features: Provides grid dimensions, cell management, and directional navigation.

using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Navigation;
using System;
using System.Collections.Generic;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Systems.Gameplay
{
    ///<summary>
    ///Represents a grid-based navigation system for pathfinding.
    ///</summary>
    public class NavigationGrid
    {
        ///<summary>
        ///Gets the width of the grid.
        ///</summary>
        public int Width { get; private set; }

        ///<summary>
        ///Gets the height of the grid.
        ///</summary>
        public int Height { get; private set; }

        ///<summary>
        ///Gets the total number of cells in the grid.
        ///</summary>
        public int TotalCells => Width * Height;

        private NavigationCell[,] _grid; //Backing data structure
        private static readonly int[,] DiagonalDirections = { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };
        private static readonly int[,] OrthogonalDirections = { { -1, 0 }, { 0, -1 }, { 0, 1 }, { 1, 0 } };

        ///<summary>
        ///Initializes a new instance of the <see cref="NavigationGrid"/> class with the specified width and height.
        ///</summary>
        ///<param name="width">The width of the grid.</param>
        ///<param name="height">The height of the grid.</param>
        public NavigationGrid(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than zero.");

            Width = width;
            Height = height;
            _grid = new NavigationCell[width, height];

            ResetGrid();
        }

        ///<summary>
        ///Resets the grid to its default state with all cells marked as walkable.
        ///</summary>
        public void ResetGrid()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    _grid[x, y] = new NavigationCell(x, y, true);
                }
            }
        }

        ///<summary>
        ///Resizes the grid to the specified dimensions, preserving existing cells where possible.
        ///</summary>
        ///<param name="newWidth">The new width of the grid.</param>
        ///<param name="newHeight">The new height of the grid.</param>
        public void ResizeGrid(int newWidth, int newHeight)
        {
            if (newWidth <= 0)
                throw new ArgumentOutOfRangeException(nameof(newWidth), "New width must be greater than zero.");

            if (newHeight <= 0)
                throw new ArgumentOutOfRangeException(nameof(newHeight), "New height must be greater than zero.");

            var newGrid = new NavigationCell[newWidth, newHeight];

            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    newGrid[x, y] = (x < Width && y < Height) ? _grid[x, y] : new NavigationCell(x, y, true);
                }
            }

            Width = newWidth;
            Height = newHeight;
            _grid = newGrid;
        }

        ///<summary>
        ///Determines whether the specified cell is walkable.
        ///</summary>
        ///<param name="x">The x-coordinate of the cell.</param>
        ///<param name="y">The y-coordinate of the cell.</param>
        ///<returns><c>true</c> if the cell is walkable; otherwise, <c>false</c>.</returns>
        public bool IsWalkable(int x, int y) =>
            IsWithinBounds(x, y) && _grid[x, y].Walkable;

        ///<summary>
        ///Sets the walkability of the specified cell.
        ///</summary>
        ///<param name="x">The x-coordinate of the cell.</param>
        ///<param name="y">The y-coordinate of the cell.</param>
        ///<param name="walkable">A value indicating whether the cell is walkable.</param>
        public void SetWalkable(int x, int y, bool walkable)
        {
            if (IsWithinBounds(x, y))
            {
                _grid[x, y].Walkable = walkable;
            }
        }

        ///<summary>
        ///Converts a world position to a grid position.
        ///</summary>
        ///<param name="worldPosition">The world position.</param>
        ///<returns>The corresponding grid position.</returns>
        public Vector3 WorldToGrid(Vector3 worldPosition) =>
            new((int)(worldPosition.X / Width), (int)(worldPosition.Y / Height), 0);

        ///<summary>
        ///Converts a grid position to a world position.
        ///</summary>
        ///<param name="gridPosition">The grid position.</param>
        ///<returns>The corresponding world position.</returns>
        public Vector3 GridToWorld(Vector3 gridPosition) =>
            new(gridPosition.X * Width, gridPosition.Y * Height, 0);

        ///<summary>
        ///Gets the <see cref="NavigationCell"/> at the specified grid position.
        ///</summary>
        ///<param name="gridPosition">The grid position.</param>
        ///<returns>The <see cref="NavigationCell"/> at the specified position.</returns>
        public NavigationCell GetCell(Vector3 gridPosition)
        {
            int x = (int)gridPosition.X;
            int y = (int)gridPosition.Y;

            if (!IsWithinBounds(x, y))
                throw new ArgumentOutOfRangeException(nameof(gridPosition), "Grid position is out of bounds.");

            return _grid[x, y];
        }

        ///<summary>
        ///Gets the neighbors of the specified grid position.
        ///</summary>
        ///<param name="gridPosition">The grid position.</param>
        ///<param name="allowDiagonal">A value indicating whether diagonal neighbors are allowed.</param>
        ///<returns>An array of neighboring <see cref="NavigationCell"/> objects.</returns>
        public NavigationCell[] GetNeighbors(Vector3 gridPosition, bool allowDiagonal)
        {
            int x = (int)gridPosition.X;
            int y = (int)gridPosition.Y;

            if (!IsWithinBounds(x, y))
                return Array.Empty<NavigationCell>();

            var neighbors = new List<NavigationCell>();
            int[,] directions = allowDiagonal ? DiagonalDirections : OrthogonalDirections;

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int neighborX = x + directions[i, 0];
                int neighborY = y + directions[i, 1];

                if (IsWithinBounds(neighborX, neighborY) && _grid[neighborX, neighborY].Walkable)
                {
                    neighbors.Add(_grid[neighborX, neighborY]);
                }
            }

            return neighbors.ToArray();
        }

        ///<summary>
        ///Checks if a grid position is within bounds.
        ///</summary>
        ///<param name="gridPosition">The grid position to check.</param>
        ///<returns>True if the position is within bounds, false otherwise.</returns>
        public bool IsInBounds(Vector3Int gridPosition)
        {
            return gridPosition.X >= 0 && gridPosition.X < Width &&
                   gridPosition.Y >= 0 && gridPosition.Y < Height;
        }

        private bool IsWithinBounds(int x, int y) =>
            x >= 0 && x < Width && y >= 0 && y < Height;
    }
}
