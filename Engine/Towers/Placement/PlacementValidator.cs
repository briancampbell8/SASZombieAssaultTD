// ====================================================================================================
//  FILE: PlacementValidator.cs
//  PATH: Engine/Towers/Placement/PlacementValidator.cs
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the PlacementValidator module.
//
//  RESPONSIBILITIES:
//      - Provide CanPlaceTower() behavior for the Core subsystem.
//      - Provide GetValidationResult() behavior for the Core subsystem.
//      - Provide WouldBlockPaths() behavior for the Core subsystem.
//      - Provide IsTooCloseToOtherTowers() behavior for the Core subsystem.
//      - Provide IsValidTerrain() behavior for the Core subsystem.
//      - Provide Cleanup() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//
using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Navigation;
using SASZombieAssaultTD.Engine.Towers.Placement;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Towers.TowerEnums;

namespace SASZombieAssaultTD.Engine.Towers
{
    public class PlacementValidator
    {
        private readonly List<PlacementRule> _rules = new List<PlacementRule>();
        private bool _isInitialized = false;

        private void Initialize()
        {
            _rules.Add(new GridBoundsRule());
            _rules.Add(new OccupancyRule());
            _rules.Add(new PathBlockingRule());
            _rules.Add(new TowerSpacingRule());
            _rules.Add(new TerrainRule());
            _rules.Add(new EconomyRule());
            _isInitialized = true;
        }

        /// <summary>
        /// Check if a tower can be placed at the specified grid position.
        /// </summary>
        public bool CanPlaceTower(Vector3Int gridPosition, TowerData towerData)
        {
            return GetValidationResult(gridPosition, towerData).IsValid;
        }

        /// <summary>
        /// Get detailed validation result for a placement attempt.
        /// </summary>
        public PlacementValidResult GetValidationResult(Vector3Int gridPosition, TowerData towerData)
        {
            var result = new PlacementValidResult { IsValid = true };

            if (!_isInitialized)
            {
                Initialize();
            }

            if (towerData == null)
            {
                result.IsValid = false;
                result.AddError("Tower data is null");
                return result;
            }

            try
            {
                // Check all placement rules and collect errors/warnings
                foreach (var rule in _rules)
                {
                    var ruleResult = rule.Validate(gridPosition, towerData);
                    if (!ruleResult.IsValid)
                    {
                        result.IsValid = false;

                        // Explicitly unpack string errors to avoid list casting mismatches
                        foreach (var err in ruleResult.Errors)
                        {
                            result.AddError(err);
                        }
                    }

                    // Unpack and add warnings from rule
                    foreach (var warning in ruleResult.Warnings)
                    {
                        result.AddWarning(warning);
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.AddError($"Validation error: {ex.Message}");
            }

            return result;
        }

        public bool WouldBlockPaths(Vector3Int gridPosition, TowerData towerData)
        {
            var grid = NavigationGrid.Instance;
            if (grid == null || towerData == null) return true;
            return !grid.HasPath;
        }

        public bool IsTooCloseToOtherTowers(Vector3Int gridPosition, TowerData towerData)
        {
            return false;
        }

        public bool IsValidTerrain(Vector3Int gridPosition, TowerData towerData)
        {
            var grid = NavigationGrid.Instance;
            if (grid == null) return false;

            // Explicitly casts the un-typed object field to a 2D integer array matrix
            var terrainGrid = (int[,])grid.Terrain;
            var terrainType = terrainGrid[gridPosition.X, gridPosition.Y];

            return towerData.RequiredTerrain switch
            {
                TerrainType.Any => true,
                TerrainType.Ground => (TerrainType)terrainType == TerrainType.Ground,
                TerrainType.Rooftop => (TerrainType)terrainType == TerrainType.Rooftop,
                TerrainType.Water => (TerrainType)terrainType == TerrainType.Water,
                _ => false
            };
        }



        /// <summary>
        /// Cleanup resources and clear active rules.
        /// </summary>
        public void Cleanup()
        {
            _rules.Clear();
            _isInitialized = false;
        }
    }
}
