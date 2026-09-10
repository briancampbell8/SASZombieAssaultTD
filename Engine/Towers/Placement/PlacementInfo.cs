// ====================================================================================================
//  FILE: PlacementInfo.cs
//  PATH: ./Engine/Towers/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the PlacementInfo module.
//
//  RESPONSIBILITIES:
//      - Provide AddWarning() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    PlacementInfo.cs
Folder:  Engine/Towers/
Purpose:  Tower placement information structure.
Features: Position validation, tower data reference, and placement status.
*/

using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Towers.TowerEnums;

namespace SASZombieAssaultTD.Engine.Towers.Placement
{
    /// <summary>
    /// Information about a tower placement attempt. Used by placement validation and UI systems.
    /// </summary>
    public class PlacementInfo
    {
        /// Properties

        /// <summary>
        /// Grid position where tower is being placed.
        /// </summary>
        public Vector3Int GridPosition { get; set; }

        /// <summary>
        /// World position where tower is being placed.
        /// </summary>
        public Vector3 WorldPosition { get; set; }

        /// <summary>
        /// Tower data for the tower being placed.
        /// </summary>
        public TowerData TowerData { get; set; }

        /// <summary>
        /// Whether the placement is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Error message if placement is invalid.
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Warning messages for placement.
        /// </summary>
        public System.Collections.Generic.List<string> Warnings { get; set; } = new();

        /// <summary>
        /// Whether the placement would block enemy paths.
        /// </summary>
        public bool WouldBlockPaths { get; set; }

        /// <summary>
        /// Distance to nearest tower.
        /// </summary>
        public float DistanceToNearestTower { get; set; }

        /// <summary>
        /// Whether the placement is valid.
        /// </summary>
        public bool CanPlace { get; set; }

        /// <summary>
        /// Whether the player can afford the tower.
        /// </summary>
        public bool CanAfford { get; set; }

        /// <summary>
        /// Terrain type at placement position.
        /// </summary>
        public TerrainType TerrainType { get; set; }

        /// <summary>
        /// Whether the position is occupied.
        /// </summary>
        public bool IsOccupied { get; set; }

        //Missing properties
        public TowerType TowerType { get; set; }

        public string TowerName { get; set; }
        public int Cost { get; set; }
        public Vector3 CurrentPosition { get; set; }
        public float Range { get; set; }
        public float Damage { get; set; }
        public float FireRate { get; set; }

        ///

        /// Constructor

        /// <summary>
        /// Creates a new placement info instance.
        /// </summary>
        /// <param name="gridPosition">Grid position.</param>
        /// <param name="towerData">Tower data.</param>
        public PlacementInfo(Vector3Int gridPosition, TowerData towerData)
        {
            GridPosition = gridPosition;
            WorldPosition = new Vector3(gridPosition.X, gridPosition.Y, 0);
            TowerData = towerData ?? throw new System.ArgumentNullException(nameof(towerData));
        }

        ///

        /// Methods

        /// <summary>
        /// Adds a warning message.
        /// </summary>
        /// <param name="message">Warning message.</param>
        public void AddWarning(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Warnings.Add(message);
            }
        }

        /// <summary>
        /// Gets a summary of the placement info.
        /// </summary>
        /// <returns>Summary string.</returns>
        public override string ToString()
        {
            var status = IsValid ? "Valid" : "Invalid";
            var details = $"Position: {GridPosition}, Tower: {TowerData?.Name}, Status: {status}";

            if (!IsValid && !string.IsNullOrEmpty(ErrorMessage))
            {
                details += $", Error: {ErrorMessage}";
            }

            if (Warnings.Count > 0)
            {
                details += $", Warnings: {Warnings.Count}";
            }

            return details;
        }

        ///
    }
}