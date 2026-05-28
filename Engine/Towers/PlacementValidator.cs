using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Dictionary;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Navigation;
using SASZombieAssaultTD.Engine.Towers;
using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Collections.Generic;
using System.Linq;
using static SASZombieAssaultTD.Engine.Towers.TowerData; // Import TowerData members

namespace SASZombieAssaultTD.Engine.Towers
{
    /// <summary>
    /// Tower placement validation system for SAS Zombie Assault TD.
    /// Validates tower placement rules and constraints.
    /// </summary>
    public class PlacementValidator
    {
        private readonly List<PlacementRule> _rules;
        private NavigationGrid _navigationGrid;
        private object TowerRegistry;
        private bool _isInitialized = false;
        private object TheType;
        private object TheMember;

        /// <summary>
        /// Initialize the placement validator.
        /// </summary>
        public PlacementValidator()
        {
            _rules = new List<PlacementRule>();
            InitializeRules();
        }

        /// <summary>
        /// Check if a tower can be placed at the specified grid position.
        /// </summary>
        /// <param name="gridPosition">Grid position to check.</param>
        /// <param name="towerData">Tower data for validation.</param>
        /// <returns>True if placement is valid.</returns>
        public bool CanPlaceTower(Vector3Int gridPosition, TowerData towerData)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            if (towerData == null)
            {
                System.Diagnostics.Debug.WriteLine("Tower data is null");
                return false;
            }

            try
            {
                // Check all placement rules
                foreach (var rule in _rules)
                {
                    if (!rule.IsValid(gridPosition, towerData))
                    {
                        System.Diagnostics.Debug.WriteLine($"Placement failed: {rule.GetFailureMessage(gridPosition, towerData)}");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error validating tower placement: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get detailed validation result for a placement attempt.
        /// </summary>
        /// <param name="gridPosition">Grid position to check.</param>
        /// <param name="towerData">Tower data for validation.</param>
        /// <returns>Validation result with details.</returns>
        public ValidationResult GetValidationResult(Vector3Int gridPosition, TowerData towerData)
        {
            var result = new ValidationResult { IsValid = true };

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
                        result.AddError(ruleResult.ErrorMessage());
                    }

                    // Add warnings from rule
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

        /// <summary>
        /// Check if a position is within the playable area.
        /// </summary>
        /// <param name="gridPosition">Grid position to check.</param>
        /// <returns>True if within playable area.</returns>
        public bool IsInPlayableArea(Vector3Int gridPosition)
        {
            if (_navigationGrid == null)
                return false;

            return _navigationGrid.IsInBounds(gridPosition.X, gridPosition.Y);
        }

        /// <summary>
        /// Check if a position blocks enemy paths.
        /// </summary>
        /// <param name="gridPosition">Grid position to check.</param>
        /// <param name="towerData">Tower data for size calculation.</param>
        /// <returns>True if placement would block paths.</returns>
        public bool WouldBlockPaths(Vector3Int gridPosition, TowerData towerData)
        {
            if (_navigationGrid == null || towerData == null)
                return true;

            try
            {
                // Temporarily mark area as occupied
                var originalOccupancy = GetAreaOccupancy(gridPosition.X, gridPosition.Y, towerData.GridSize);
                SetAreaOccupancy(gridPosition.X, gridPosition.Y, towerData.GridSize, true);

                // Check if path still exists from start to end
                var hasPath = _navigationGrid.HasPath;

                // Restore original occupancy
                SetAreaOccupancy(gridPosition.X, gridPosition.Y, towerData.GridSize, false);
                RestoreAreaOccupancy(gridPosition.X, gridPosition.Y, towerData.GridSize, originalOccupancy);

                return !hasPath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking path blocking: {ex.Message}");
                return true; // Assume it blocks paths on error
            }
        }

        private bool[,] GetAreaOccupancy(int x, int y, Vector3Int gridSize)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        private void SetAreaOccupancy(int x, int y, Vector3Int gridSize, bool v)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        private void RestoreAreaOccupancy(int x, int y, Vector3Int gridSize, bool[,] originalOccupancy)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the minimum distance required from other towers.
        /// </summary>
        /// <param name="towerType">Type of tower being placed.</param>
        /// <returns>Minimum distance in grid units.</returns>
        public float GetMinDistanceFromTowers(TowerType towerType)
        {
            // Different tower types have different spacing requirements
            return towerType switch
            {
                TowerType.Basic => 1.0f,
                TowerType.Tesla => 1.5f,
                TowerType.Splash => 2.0f,
                TowerType.Rapid => 0.5f,
                TowerType.Sniper => 1.0f,
                _ => 1.0f
            };
        }

        /// <summary>
        /// Check if placement is too close to other towers.
        /// </summary>
        /// <param name="gridPosition">Grid position to check.</param>
        /// <param name="towerData">Tower data for the tower being placed.</param>
        /// <returns>True if too close to other towers.</returns>
        public bool IsTooCloseToOtherTowers(Vector3Int gridPosition, TowerData towerData)
        {
            // TODO: Implement tower registry when available
            // if (_towerRegistry == null)
            //     return false;
            return false;

            var minDistance = GetMinDistanceFromTowers(towerData.Type);
            var worldPosition = _navigationGrid.GridToWorld(gridPosition);

            // TODO: Implement tower registry when available
            // foreach (var tower in _towerRegistry.GetAllTowers())
            foreach (var tower in new List<Tower>()) // Empty placeholder
            {
                var distance = Vector3.Distance(new Vector3(worldPosition.X, worldPosition.Y, 0), tower.Position);
                if (distance < minDistance)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if placement is on valid terrain.
        /// </summary>
        /// <param name="gridPosition">Grid position to check.</param>
        /// <param name="towerData">Tower data for terrain requirements.</param>
        /// <returns>True if terrain is valid.</returns>
        public bool IsValidTerrain(Vector3Int gridPosition, TowerData towerData)
        {
            if (_navigationGrid == null)
                return false;

            // Check if the terrain type supports this tower
            var terrainType = _navigationGrid.GetTerrainType(gridPosition);

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
        /// Add a custom placement rule.
        /// </summary>
        /// <param name="rule">Rule to add.</param>
        public void AddRule(PlacementRule rule)
        {
            _rules.Add(rule);
            System.Diagnostics.Debug.WriteLine($"Added placement rule: {rule.GetType().Name}");
        }

        /// <summary>
        /// Remove a placement rule.
        /// </summary>
        /// <param name="rule">Rule to remove.</param>
        public void RemoveRule(PlacementRule rule)
        {
            if (_rules.Remove(rule))
            {
                System.Diagnostics.Debug.WriteLine($"Removed placement rule: {rule.GetType().Name}");
            }
        }

        /// <summary>
        /// Initialize the validator with required systems.
        /// </summary>
        private void Initialize()
        {
            _navigationGrid = NavigationGrid.Instance;
            TowerRegistry = null; // TODO: Implement TowerRegistry when available
            _isInitialized = true;
        }

        /// <summary>
        /// Initialize default placement rules.
        /// </summary>
        private void InitializeRules()
        {
            // Grid bounds rule
            _rules.Add(new GridBoundsRule());

            // Occupancy rule
            _rules.Add(new OccupancyRule());

            // Path blocking rule
            _rules.Add(new PathBlockingRule());

            // Tower spacing rule
            _rules.Add(new TowerSpacingRule());

            // Terrain rule
            _rules.Add(new TerrainRule());

            // Economy rule
            _rules.Add(new EconomyRule());

            System.Diagnostics.Debug.WriteLine($"Initialized {_rules.Count} placement rules");
        }

        /// <summary>
        /// Get occupancy data for an area.
        /// </summary>
        public bool[,] GetAreaOccupancy(int x,
                                        Vector3Int position,
                                        int size,
                                        int length)
        {
            var occupancy = new bool[size, size];

            ///            for (int x = size - 1; x >= 0; x--)
            for (int i = 0; i < size; i++)
            {

            }
            {
                position = Presence(position, size, occupancy, x);
            }

            return occupancy;
        }

        private Vector3Int Presence(Vector3Int position, int size, bool[,] occupancy, int x)
        {
            {
                for (int y = 0; y < size; y++)
                {
                    var checkPos = new Vector3Int(position.X + x, position.Y + y);
                    occupancy[x, y] = _navigationGrid.IsOccupied(checkPos.X, checkPos.Y);
                }
            }

            return position;
        }

        /// <summary>
        /// Set occupancy for an area.
        /// </summary>
        private void SetAreaOccupancy(int offsetX, int offsetY, Vector3Int position, int size, bool occupied)
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    var checkPos = new Vector3Int(
                        position.x + offsetX + x,
                        position.y + offsetY + y,
                        position.z  // Preserve original Z
                    );
                    _navigationGrid.SetOccupied(checkPos.x, checkPos.y, occupied);
                }
            }
        }

        /// <summary>
        /// Restore occupancy for an area.
        /// </summary>
        private void RestoreAreaOccupancy(Vector3Int areaStart, int width, int height, bool[,] savedOccupancy)
        {
            for (int localX = 0; localX < width; localX++)
            {
                for (int localY = 0; localY < height; localY++)
                {
                    var worldPos = new Vector3Int(
                        areaStart.x + localX,
                        areaStart.y + localY,
                        areaStart.z
                    );

                    _navigationGrid.SetOccupied(
                        worldPos.x,
                        worldPos.y,
                        savedOccupancy[localX, localY]
                    );
                }
            }
        }

        /// <summary>
        /// Cleanup resources.
        /// </summary>
        public void Cleanup()
        {
            _rules.Clear();
            _isInitialized = false;
        }
    }

    /// <summary>
    /// Base class for placement rules.
    /// </summary>
    public abstract class PlacementRule
    {
        /// <summary>
        /// Check if the rule is satisfied.
        /// </summary>
        /// <param name="gridPosition">Grid position.</param>
        /// <param name="towerData">Tower data.</param>
        /// <returns>True if rule is satisfied.</returns>
        public abstract bool IsValid(Vector3Int gridPosition, TowerData towerData);

        /// <summary>
        /// Get detailed validation result.
        /// </summary>
        /// <param name="gridPosition">Grid position.</param>
        /// <param name="towerData">Tower data.</param>
        /// <returns>Validation result.</returns>
        public virtual ValidationResult Validate(Vector3Int gridPosition, TowerData towerData)
        {
            var result = new ValidationResult { IsValid = IsValid(gridPosition, towerData) };

            if (!result.IsValid)
            {
                result.AddError(GetFailureMessage(gridPosition, towerData));
            }

            return result;
        }

        /// <summary>
        /// Get failure message for this rule.
        /// </summary>
        /// <param name="gridPosition">Grid position.</param>
        /// <param name="towerData">Tower data.</param>
        /// <returns>Failure message.</returns>
        public abstract string GetFailureMessage(Vector3Int gridPosition, TowerData towerData);
    }

    /// <summary>
    /// Validation result with errors and warnings.
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; } = new();
        public List<string> Warnings { get; } = new();

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public void AddWarning(string warning)
        {
            Warnings.Add(warning);
        }
    }

    /// <summary>
    /// Rule: Check if position is within grid bounds.
    /// </summary>
    public class GridBoundsRule : PlacementRule
    {
        public override bool IsValid(Vector3Int gridPosition, TowerData towerData)
        {
            var grid = NavigationGrid.Instance;
            if (grid == null) return false;

            return grid.IsInBounds(gridPosition.X, gridPosition.Y) &&
                   grid.IsInBounds(gridPosition.X + towerData.GridSize.X - 1,
                                   gridPosition.Y + towerData.GridSize.Y - 1);
        }

        public override string GetFailureMessage(Vector3Int gridPosition, TowerData towerData)
        {
            return "Position is outside the playable area";
        }
    }

    /// <summary>
    /// Rule: Check if position is already occupied.
    /// </summary>
    public class OccupancyRule : PlacementRule
    {
        public override bool IsValid(Vector3Int gridPosition, TowerData towerData)
        {
            var grid = NavigationGrid.Instance;
            if (grid == null) return false;

            // Check all cells the tower would occupy
            for (int x = 0; x < towerData.GridSize.X; x++)
            {
                for (int y = 0; y < towerData.GridSize.Y; y++)
                {
                    var checkPos = new Vector3Int(gridPosition.X + x, gridPosition.Y + y);
                    if (grid.IsOccupied(checkPos.X, checkPos.Y))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override string GetFailureMessage(Vector3Int gridPosition, TowerData towerData)
        {
            return "Position is already occupied";
        }
    }

    /// <summary>
    /// Rule: Check if placement would block enemy paths.
    /// </summary>
    public class PathBlockingRule : PlacementRule
    {
        public override bool IsValid(Vector3Int gridPosition, TowerData towerData)
        {
            var validator = new PlacementValidator();
            return !validator.WouldBlockPaths(gridPosition, towerData);
        }

        public override string GetFailureMessage(Vector3Int gridPosition, TowerData towerData)
        {
            return "Placement would block enemy paths";
        }
    }

    /// <summary>
    /// Rule: Check minimum distance from other towers.
    /// </summary>
    public class TowerSpacingRule : PlacementRule
    {
        public override bool IsValid(Vector3Int gridPosition, TowerData towerData)
        {
            var validator = new PlacementValidator();
            return !validator.IsTooCloseToOtherTowers(gridPosition, towerData);
        }

        public override string GetFailureMessage(Vector3Int gridPosition, TowerData towerData)
        {
            return "Too close to other towers";
        }
    }

    /// <summary>
    /// Rule: Check if terrain supports the tower.
    /// </summary>
    public class TerrainRule : PlacementRule
    {
        public override bool IsValid(Vector3Int gridPosition, TowerData towerData)
        {
            var validator = new PlacementValidator();
            return validator.IsValidTerrain(gridPosition, towerData);
        }

        public override string GetFailureMessage(Vector3Int gridPosition, TowerData towerData)
        {
            return "Terrain does not support this tower type";
        }
    }

    /// <summary>
    /// Rule: Check if player can afford the tower.
    /// </summary>
    public class EconomyRule : PlacementRule
    {
        public override bool IsValid(Vector3Int gridPosition, TowerData towerData)
        {
            // Economy system not available - always return true
            return true;
        }

        public override string GetFailureMessage(Vector3Int gridPosition, TowerData towerData)
        {
            return "Insufficient funds";
        }
    }
}
