/*
File:    TowerManager.cs
Purpose: Central tower management system for SAS Zombie Assault TD.
Features: Tower lifecycle management, placement, upgrades, and integration with other managers.

P11-04-07-B: Centralized tower management following established Manager pattern.
Consolidates TowerRegistry, PlacementValidator, and tower upgrade functionality.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.ECS;
using ECSWorld = SASZombieAssaultTD.Engine.ECS.ECSWorld;
using SASZombieAssaultTD.Engine.Navigation;
using SASZombieAssaultTD.Engine.Economy;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Rendering;

namespace SASZombieAssaultTD.Engine.Towers
{
    /// <summary>
    /// Central tower management system for SAS Zombie Assault TD.
    /// Handles tower placement, upgrades, lifecycle, and integration with other systems.
    /// </summary>
    public class TowerManager
    {
        #region Private Fields

        private readonly List<Tower> _towers = new List<Tower>();
        private readonly ECSWorld _ecsWorld;
        private readonly NavigationGrid _navigationGrid;
        private uint _nextTowerId = 1;
        private bool _isInitialized = false;

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
        private void Initialize()
        {
            if (_isInitialized) return;

            ModernLoggingSystem.Log("INFO", "TowerManager: Initializing tower management system");
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
            if (!ValidatePlacement(position, towerType))
            {
                ModernLoggingSystem.Log("WARNING", "TowerManager: Failed to place tower at {position}: Invalid placement");
                return null;
            }

            if (!CanAffordTower(towerType))
            {
                ModernLoggingSystem.Log("WARNING", "TowerManager: Failed to place tower at {position}: Cannot afford");
                return null;
            }

            try
            {
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
                
                // Deduct cost
                var cost = GetTowerCost(towerType);
                EconomyManager.RemoveCash(cost);
                
                ModernLoggingSystem.Log("INFO", "TowerManager: Placed {towerType} tower at {position} (ID: {towerId})");
                
                // Fire event
                OnTowerPlaced?.Invoke(tower);
                
                return tower;
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", "TowerManager: Error placing tower: {ex.Message}");
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
                ModernLoggingSystem.Log("WARNING", "TowerManager: Cannot upgrade tower {towerId}: Tower not found");
                return false;
            }

            if (!CanAffordUpgrade(upgrade))
            {
                ModernLoggingSystem.Log("WARNING", "TowerManager: Cannot upgrade tower {towerId}: Cannot afford upgrade");
                return false;
            }

            try
            {
                // Apply upgrade
                tower.ApplyUpgrade(upgrade);
                
                // Deduct cost
                EconomyManager.RemoveCash(upgrade.Cost);
                
                ModernLoggingSystem.Log("INFO", "TowerManager: Upgraded tower {towerId} with {upgrade.Type}");
                
                // Fire event
                OnTowerUpgraded?.Invoke(tower, upgrade);
                
                return true;
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", "TowerManager: Error upgrading tower {towerId}: {ex.Message}");
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
                // TODO: Fix GetEntityWithComponent call - ECSWorld doesn't have this method signature
                // var entity = _ecsWorld.GetEntityWithComponent<TowerComponent>(c => c.Tower.Id == towerId);
                // if (entity != null)
                // {
                //     _ecsWorld.DestroyEntity(entity.Id);
                // }
                
                // Placeholder: Find tower by ID and destroy it
                var towerToDestroy = GetTower(towerId);
                if (towerToDestroy != null)
                {
                    // TODO: Destroy tower entity
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
        public Tower GetTower(uint towerId)
        {
            return _towers.FirstOrDefault(t => t.Id == towerId);
        }

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
        private bool IsTooCloseToOtherTowers(Vector3Int position, TowerType towerType)
        {
            var minDistance = GetMinDistanceFromTowers(towerType);
            var worldPosition = _navigationGrid.GridToWorld(position);

            foreach (var tower in _towers)
            {
                var distance = Vector3.Distance(worldPosition, tower.Position);
                if (distance < minDistance)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if tower placement blocks enemy path.
        /// </summary>
        /// <param name="position">Position to check.</param>
        /// <param name="towerType">Type of tower being placed.</param>
        /// <returns>True if blocks enemy path.</returns>
        private bool BlocksEnemyPath(Vector3Int position, TowerType towerType)
        {
            // TODO: Implement path blocking check
            // This would require integration with the pathfinding system
            return false;
        }

        /// <summary>
        /// Gets the minimum distance required from other towers.
        /// </summary>
        /// <param name="towerType">Type of tower.</param>
        /// <returns>Minimum distance in world units.</returns>
        private float GetMinDistanceFromTowers(TowerType towerType)
        {
            // TODO: Implement tower-specific distance requirements
            return 2.0f; // Default minimum distance
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
            var cost = GetTowerCost(towerType);
            return EconomyManager.CurrentCash >= cost;
        }

        /// <summary>
        /// Checks if the player can afford an upgrade.
        /// </summary>
        /// <param name="upgrade">Upgrade to check.</param>
        /// <returns>True if affordable, false otherwise.</returns>
        public bool CanAffordUpgrade(TowerUpgrade upgrade)
        {
            return EconomyManager.CurrentCash >= upgrade.Cost;
        }

        /// <summary>
        /// Gets the cost of a tower.
        /// </summary>
        /// <param name="towerType">Type of tower.</param>
        /// <returns>Tower cost.</returns>
        public int GetTowerCost(TowerType towerType)
        {
            // TODO: Implement tower cost database
            return towerType switch
            {
                TowerType.Basic => 100,
                TowerType.Sniper => 200,
                TowerType.Splash => 150,
                TowerType.Freeze => 175,
                _ => 100
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
