/*
File:    TowerManager.cs
Purpose: Central tower management system for SAS Zombie Assault TD.
Features: Tower lifecycle management, placement, upgrades, and integration with other managers.

P11-04-07-B: Centralized tower management following established Manager pattern.
Consolidates TowerRegistry, PlacementValidator, and tower upgrade functionality.
*/

using SASZombieAssaultTD.Engine.Economy;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Navigation;
using SASZombieAssaultTD.Engine.Player;
using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Collections.Generic;
using System.Linq;
using ECSWorld = SASZombieAssaultTD.Engine.ECS.ECSWorld;

namespace SASZombieAssaultTD.Engine.Towers
{
    /// <summary>
    /// Central tower management system for SAS Zombie Assault TD.
    /// Handles tower placement, upgrades, lifecycle, and integration with other systems.
    /// </summary>
    public class TowerManager
    {
        #region Private Fields

        readonly List<Tower> _towers = new List<Tower>();
        readonly ECSWorld _ecsWorld;
        readonly NavigationGrid _navigationGrid;
        uint _nextTowerId = 1;
        bool _isInitialized;

        #endregion

        #region Properties

        /// <summary>
        /// Gets all active towers.
        /// </summary>
        public IReadOnlyList<Tower> Towers => _towers.AsReadOnly();

        /// <summary>
        /// Gets the count of active towers.
        /// </summary>
        public int TowerCount => _towers.Count;

        /// <summary>
        /// Gets the next available tower ID.
        /// </summary>
        public uint NextTowerId => _nextTowerId;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the tower manager.
        /// </summary>
        /// <param name="ecsWorld">The ECS world for entity management.</param>
        /// <param name="navigationGrid">The navigation grid for placement validation.</param>
        public TowerManager(ECSWorld ecsWorld, NavigationGrid navigationGrid)
        {
            _ecsWorld = ecsWorld ?? throw new ArgumentNullException(nameof(ecsWorld));
            _navigationGrid = navigationGrid ?? throw new ArgumentNullException(nameof(navigationGrid));

            Initialize();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize the tower manager.
        /// </summary>
        void Initialize()
        {
            if (_isInitialized) return;

            ModernLoggingSystem.Log("INFO", "TowerManager: Initializing tower management system");
            
            // Initialize TowerManager integration
            TowerManagerIntegration.Initialize();
            
            _isInitialized = true;
        }

        #endregion

        #region Tower Lifecycle Management

        /// <summary>
        /// Places a new tower at the specified position.
        /// </summary>
        /// <param name="towerType">Type of tower to place.</param>
        /// <param name="position">Grid position for placement.</param>
        /// <returns>The placed tower, or null if placement failed.</returns>
        public Tower PlaceTower(TowerType towerType, Vector3Int position)
        {
            // Validate placement through TowerManagerIntegration
            var validationResult = TowerManagerIntegration.ValidateTowerPlacement(towerType, position);
            // Changed line 105 to:
            // Use the variable 'validationResult' and remove the parentheses
            if (!validationResult.IsSuccess)


            {
                ModernLoggingSystem.Log("WARNING", $"TowerManager: Failed to place tower at {position}: {validationResult.Message}");
                return null;
            }

            // Additional placement validation (position, path blocking, etc.)
            if (!ValidatePlacement(position, towerType))
            {
                ModernLoggingSystem.Log("WARNING", $"TowerManager: Failed to place tower at {position}: Invalid placement location");
                return null;
            }

            try
            {
                // Process tower placement through TowerManagerIntegration
                var placementResult = TowerManagerIntegration.ProcessTowerPlacement(towerType, _navigationGrid.GridToWorld(position));
                // Use the variable 'validationResult' and remove the parentheses
                if (!validationResult.IsSuccess)

                {
                    ModernLoggingSystem.Log("WARNING", $"TowerManager: Failed to process tower placement: {placementResult.Message}");
                    return null;
                }

                // Create tower entity
                var towerId = _nextTowerId++;
                var worldPosition = _navigationGrid.GridToWorld(position);

                var tower = new Tower(towerId, towerType, worldPosition);

                // Add to ECS world
                var entity = _ecsWorld.CreateEntity();
                entity.AddComponent(new TowerComponent(tower));
                entity.AddComponent(new SASZombieAssaultTD.Engine.Components.TransformComponent(worldPosition));

                // Add to tower list
                _towers.Add(tower);

                // Mark grid as occupied
                _navigationGrid.SetOccupied(position.X, position.Y, true);

                ModernLoggingSystem.Log("INFO", $"TowerManager: Placed {towerType} tower at {position} (ID: {towerId})");

                // Fire event
                OnTowerPlaced?.Invoke(tower);

                return tower;
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"TowerManager: Error placing tower: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Upgrades an existing tower.
        /// </summary>
        /// <param name="towerId">ID of the tower to upgrade.</param>
        /// <param name="upgrade">Upgrade to apply.</param>
        /// <returns>True if upgrade succeeded, false otherwise.</returns>
        public bool UpgradeTower(uint towerId, TowerUpgrade upgrade)
        {
            var tower = GetTower(towerId);

            if (tower == null)
            {
                ModernLoggingSystem.Log("WARNING", $"TowerManager: Cannot upgrade tower {towerId}: Tower not found");
                return false;
            }

            // Validate upgrade through TowerManagerIntegration
            var validationResult = TowerManagerIntegration.ValidateTowerUpgrade(towerId, upgrade);
            // Use the variable 'validationResult' and remove the parentheses
            if (!validationResult.IsSuccess)

            {
                ModernLoggingSystem.Log("WARNING", $"TowerManager: Cannot upgrade tower {towerId}: {validationResult.Message}");
                return false;
            }

            try
            {
                // Process tower upgrade through TowerManagerIntegration
                var upgradeResult = TowerManagerIntegration.ProcessTowerUpgrade(towerId, upgrade);
                // Use the variable 'validationResult' and remove the parentheses
                if (!validationResult.IsSuccess)

                {
                    ModernLoggingSystem.Log("WARNING", $"TowerManager: Failed to process tower upgrade: {upgradeResult.Message}");
                    return false;
                }

                // Apply upgrade
                tower.ApplyUpgrade(upgrade);

                ModernLoggingSystem.Log("INFO", $"TowerManager: Upgraded tower {towerId} with {upgrade.Type}");

                // Fire event
                OnTowerUpgraded?.Invoke(tower, upgrade);

                return true;
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"TowerManager: Error upgrading tower {towerId}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Destroys a tower.
        /// </summary>
        /// <param name="towerId">ID of the tower to destroy.</param>
        /// <returns>True if destruction succeeded, false otherwise.</returns>
        public bool DestroyTower(uint towerId)
        {
            var tower = GetTower(towerId);

            if (tower == null)
            {
                ModernLoggingSystem.Log("WARNING", "TowerManager: Cannot destroy tower {towerId}: Tower not found");
                return false;
            }

            try
            {
                // Find and destroy the tower entity from ECS world
                var towerToDestroy = GetTower(towerId);

                if (towerToDestroy != null && towerToDestroy.Entity != null)
                {
                    // Destroy the ECS entity associated with this tower
                    _ecsWorld.DestroyEntity((uint)towerToDestroy.Entity.Id);
                }

                // Mark grid as unoccupied
                var gridPos = _navigationGrid.WorldToGrid(tower.Position);
                _navigationGrid.SetOccupied(gridPos.X, gridPos.Y, false);

                // Remove from tower list
                _towers.Remove(tower);

                // Refund partial cost
                var refund = GetTowerRefund(tower.Type);
                EconomyManager.AddCash(refund);

                ModernLoggingSystem.Log("INFO", "TowerManager: Destroyed tower {towerId} at {tower.Position}");

                // Fire event
                OnTowerDestroyed?.Invoke(tower);

                return true;
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", "TowerManager: Error destroying tower {towerId}: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Tower Queries

        /// <summary>
        /// Gets a tower by ID.
        /// </summary>
        /// <param name="towerId">ID of the tower to retrieve.</param>
        /// <returns>The tower, or null if not found.</returns>
        public Tower GetTower(uint towerId) => _towers.FirstOrDefault(t => t.Id == towerId);

        /// <summary>
        /// Gets towers within a specified radius of a position.
        /// </summary>
        /// <param name="center">Center position.</param>
        /// <param name="radius">Search radius.</param>
        /// <returns>Towers within the radius.</returns>
        public IEnumerable<Tower> GetTowersInArea(Vector3 center, float radius)
        {
            return _towers.Where(t => Vector3.Distance(t.Position, center) <= radius);
        }

        /// <summary>
        /// Gets towers of a specific type.
        /// </summary>
        /// <param name="towerType">Type of tower to find.</param>
        /// <returns>Towers of the specified type.</returns>
        public IEnumerable<Tower> GetTowersByType(TowerType towerType)
        {
            return _towers.Where(t => t.Type == towerType);
        }

        #endregion

        #region Placement Validation

        /// <summary>
        /// Validates if a tower can be placed at the specified position.
        /// </summary>
        /// <param name="position">Position to validate.</param>
        /// <param name="towerType">Type of tower being placed.</param>
        /// <returns>True if placement is valid, false otherwise.</returns>
        public bool ValidatePlacement(Vector3Int position, TowerType towerType)
        {
            // Check grid bounds
            if (!_navigationGrid.IsInBounds(position.X, position.Y))
            {
                return false;
            }

            // Check if position is occupied
            if (_navigationGrid.IsOccupied(position.X, position.Y))
            {
                return false;
            }

            // Check if too close to other towers
            if (IsTooCloseToOtherTowers(position, towerType))
            {
                return false;
            }

            // Check if blocks enemy path
            if (BlocksEnemyPath(position, towerType))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if placement is too close to other towers.
        /// </summary>
        /// <param name="position">Position to check.</param>
        /// <param name="towerType">Type of tower being placed.</param>
        /// <returns>True if too close to other towers.</returns>
        bool IsTooCloseToOtherTowers(Vector3Int position, TowerType towerType)
        {
            var minDistance = GetMinDistanceFromTowers(towerType);
            var worldPosition = _navigationGrid.GridToWorld(position);

            foreach (var tower in _towers)
            {
                var distance = Vector3.Distance(worldPosition, tower.Position);
                if (distance < minDistance) return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if tower placement blocks enemy path.
        /// Simulates tower placement and checks if path still exists.
        /// </summary>
        /// <param name="position">Position to check.</param>
        /// <param name="towerType">Type of tower being placed.</param>
        /// <returns>True if blocks enemy path.</returns>
        bool BlocksEnemyPath(Vector3Int position, TowerType towerType)
        {
            if (_navigationGrid == null) return false;

            try
            {
                // Get tower data for size calculation
                var towerData = GetTowerData(towerType);
                if (towerData == null) return false;

                // Temporarily mark area as occupied
                var originalOccupancy = GetAreaOccupancy(position.X, position.Y, towerData.GridSize);
                SetAreaOccupancy(position.X, position.Y, towerData.GridSize, true);

                // Check if path still exists from start to end
                var hasPath = _navigationGrid.HasPath;

                // Restore original occupancy
                RestoreAreaOccupancy(position.X, position.Y, towerData.GridSize, originalOccupancy);

                return !hasPath;
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"Error checking path blocking: {ex.Message}");
                return true; // Assume it blocks paths on error
            }
        }

        /// <summary>
        /// Gets the current occupancy state of a grid area.
        /// Helper method for path blocking checking.
        /// </summary>
        bool[,] GetAreaOccupancy(int x, int y, Vector3Int gridSize)
        {
            var occupancy = new bool[gridSize.X, gridSize.Y];

            for (int i = 0; i < gridSize.X; i++)
            {
                for (int j = 0; j < gridSize.Y; j++)
                {
                    var checkX = x + i;
                    var checkY = y + j;
                    occupancy[i, j] = _navigationGrid.IsOccupied(checkX, checkY);
                }
            }

            return occupancy;
        }

        /// <summary>
        /// Sets the occupancy state of a grid area.
        /// Helper method for path blocking checking.
        /// </summary>
        void SetAreaOccupancy(int x, int y, Vector3Int gridSize, bool occupied)
        {
            for (int i = 0; i < gridSize.X; i++)
            {
                for (int j = 0; j < gridSize.Y; j++)
                {
                    var checkX = x + i;
                    var checkY = y + j;
                    _navigationGrid.SetOccupied(checkX, checkY, occupied);
                }
            }
        }

        /// <summary>
        /// Restores the occupancy state of a grid area.
        /// Helper method for path blocking checking.
        /// </summary>
        void RestoreAreaOccupancy(int x, int y, Vector3Int gridSize, bool[,] originalOccupancy)
        {
            for (int i = 0; i < gridSize.X; i++)
            {
                for (int j = 0; j < gridSize.Y; j++)
                {
                    var checkX = x + i;
                    var checkY = y + j;
                    _navigationGrid.SetOccupied(checkX, checkY, originalOccupancy[i, j]);
                }
            }
        }

        /// <summary>
        /// Gets tower data for the specified tower type.
        /// Helper method for accessing tower data.
        /// </summary>
        TowerData GetTowerData(TowerType towerType)
        {
            // This would typically get data from a registry or database
            // For now, return basic data
            return new TowerData(towerType.ToString())
            {
                Type = towerType,
                GridSize = new Vector3Int(1, 1, 0)
            };
        }

        /// <summary>
        /// Gets the minimum distance required from other towers.
        /// Different tower types have different spacing requirements.
        /// </summary>
        /// <param name="towerType">Type of tower.</param>
        /// <returns>Minimum distance in world units.</returns>
        float GetMinDistanceFromTowers(TowerType towerType)
        {
            // Different tower types have different spacing requirements
            return towerType switch
            {
                TowerType.Basic => 2.0f,
                TowerType.Sniper => 3.0f,      // Snipers need more space
                TowerType.Splash => (float)System.Math.Round(2.5f, 2),      // Splash towers need moderate space
                TowerType.Freeze => (float)System.Math.Round(2.0f, 2),
                TowerType.Rapid => (float)System.Math.Round(1.5f, 2),       // Rapid towers can be closer
                TowerType.Poison => (float)System.Math.Round(2.0f, 2),      // Poison towers need moderate space
                TowerType.Laser => (float)System.Math.Round(2.5f, 2),
                TowerType.Tesla => (float)System.Math.Round(3.0f, 2),       // Tesla towers need space for arcs
                TowerType.Mortar => (float)System.Math.Round(4.0f, 2),      // Mortars need most space
                TowerType.Flame => (float)System.Math.Round(2.0f, 2),
                TowerType.Ice => (float)System.Math.Round(2.0f, 2),
                TowerType.Electric => (float)System.Math.Round(2.5f, 2),
                _ => (float)System.Math.Round(2.0f, 2)  // Default minimum distance
            };
        }

        #endregion

        #region Economy Integration

        /// <summary>
        /// Checks if the player can afford a tower.
        /// </summary>
        /// <param name="towerType">Type of tower to check.</param>
        /// <returns>True if affordable, false otherwise.</returns>
        public bool CanAffordTower(TowerType towerType)
        {
            // Use TowerManagerIntegration for affordability checking
            var validationResult = TowerManagerIntegration.ValidateTowerPlacement(towerType, new Vector3Int(0, 0, 0));
            // Changed line 498 and 510 to:
            // Changed line 509 to:
            return validationResult.IsSuccess && !validationResult.Message.Contains("funds");


        }

        /// <summary>
        /// Checks if the player can afford an upgrade.
        /// </summary>
        /// <param name="upgrade">Upgrade to check.</param>
        /// <returns>True if affordable, false otherwise.</returns>
        public bool CanAffordUpgrade(TowerUpgrade upgrade)
        {
            // Use TowerManagerIntegration for affordability checking
            var validationResult = TowerManagerIntegration.ValidateTowerUpgrade(1, upgrade); // Use dummy tower ID
            return validationResult.IsSuccess && !validationResult.Message.Contains("funds");
        }

        /// <summary>
        /// Gets the cost of a tower.
        /// Returns the base cost for the specified tower type.
        /// </summary>
        /// <param name="towerType">Type of tower.</param>
        /// <returns>Tower cost.</returns>
        public int GetTowerCost(TowerType towerType)
        {
            // Complete tower cost database with all tower types
            return towerType switch
            {
                TowerType.Basic => 100,
                TowerType.Sniper => 200,
                TowerType.Splash => 150,
                TowerType.Freeze => 175,
                TowerType.Rapid => 125,
                TowerType.Poison => 160,
                TowerType.Laser => 250,
                TowerType.Tesla => 225,
                TowerType.Mortar => 300,
                TowerType.Flame => 180,
                TowerType.Ice => 190,
                TowerType.Electric => 210,
                _ => 100  // Default cost for unknown types
            };
        }

        /// <summary>
        /// Gets the refund amount for a tower.
        /// </summary>
        /// <param name="towerType">Type of tower.</param>
        /// <returns>Refund amount.</returns>
        public int GetTowerRefund(TowerType towerType)
        {
            var cost = GetTowerCost(towerType);
            return (int)(cost * 0.5f); // 50% refund
        }

        #endregion

        #region Events

        /// <summary>
        /// Event fired when a tower is placed.
        /// </summary>
        public event Action<Tower> OnTowerPlaced;

        /// <summary>
        /// Event fired when a tower is upgraded.
        /// </summary>
        public event Action<Tower, TowerUpgrade> OnTowerUpgraded;

        /// <summary>
        /// Event fired when a tower is destroyed.
        /// </summary>
        public event Action<Tower> OnTowerDestroyed;

        #endregion

        #region Cleanup

        /// <summary>
        /// Cleans up the tower manager.
        /// </summary>
        public void Cleanup()
        {
            ModernLoggingSystem.Log("INFO", "TowerManager: Cleaning up tower management system");

            _towers.Clear();
            _nextTowerId = 1;
            _isInitialized = false;
        }

        #endregion
    }
}