// ====================================================================================================
//  FILE: TowerManager.cs
//  PATH: ./Engine/Towers/
//  MODULE: Core – Tower Lifecycle & Placement Subsystem
//
//  ROLE:
//      Central authority for tower lifecycle, placement validation, upgrades, destruction,
//      and ECS/world‑grid integration.
//
//  RESPONSIBILITIES:
//      - PlaceTower()
//      - UpgradeTower()
//      - DestroyTower()
//      - GetTower()
//      - GetTowersInArea()
//      - GetTowersByType()
//      - ValidatePlacement()
//      - CanAffordTower()
//      - CanAffordUpgrade()
//      - GetTowerCost()
//      - GetTowerRefund()
//      - Cleanup()
//
//  NOTES:
//      Header upgraded to robust standard.
//      All non‑BGFX TODOs implemented.
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Economy;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Navigation;
using SASZombieAssaultTD.Engine.Towers.Pathing;
using SASZombieAssaultTD.Engine.Towers.Registry;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.Towers.TowerEnums;

namespace SASZombieAssaultTD.Engine.Towers
{
    public class TowerManager
    {
        public uint Id { get; set; }

        public override bool Equals(object obj) =>
            obj is Tower other && Id == other.Id;

        private readonly List<Tower> _towers = new();
        private readonly ECSRuntimeCore _runtime;
        private readonly NavigationGrid _navigationGrid;
        private readonly ITowerRegistry _towerRegistry;
        private readonly IPathBlockEvaluator _pathBlockEvaluator;

        private uint _nextTowerId = 1;
        private bool _isInitialized;

        public IReadOnlyList<Tower> Towers => _towers.AsReadOnly();
        public int TowerCount => _towers.Count;
        public uint NextTowerId => _nextTowerId;

        public TowerManager(ECSRuntimeCore ecsWorld, NavigationGrid navigationGrid)
        {
            _runtime = ecsWorld ?? throw new ArgumentNullException(nameof(ecsWorld));
            _navigationGrid = navigationGrid ?? throw new ArgumentNullException(nameof(navigationGrid));

            _towerRegistry = (ITowerRegistry)TowerRegistry.Instance;
            _pathBlockEvaluator = PathBlockEvaluator.Instance;

            Initialize();
        }

        private void Initialize()
        {
            if (_isInitialized) return;
            DLogger.Log(LogSubsystems.Towers, LogLevel.Info, "TowerManager: Initializing tower management system");
            _isInitialized = true;
        }

        // ====================================================================================================
        //  PLACE TOWER
        // ====================================================================================================

        public Tower PlaceTower(TowerType towerType, Vector3Int position)
        {
            if (!ValidatePlacement(position, towerType))
                return null;

            if (!CanAffordTower(towerType))
                return null;

            try
            {
                uint towerId = _nextTowerId++;
                Vector3 worldPos = _navigationGrid.GridToWorld(position);

                var tower = new Tower(towerId, towerType, worldPos);

                // Create ECS entity
                var entity = _runtime.CreateEntity();

                // Add components via ECSRuntimeCore (modern ECS)
                _runtime.AddComponent(entity, new TowerComponent(tower));
                _runtime.AddComponent(entity, new SASZombieAssaultTD.Engine.Components.TransformComponent(worldPos));

                _towers.Add(tower);
                _towerRegistry.Register(tower);

                _navigationGrid.SetOccupied(position.X, position.Y, position, true);

                EconomyManager.RemoveCash(GetTowerCost(towerType));

                OnTowerPlaced?.Invoke(tower);
                return tower;
            }
            catch
            {
                return null;
            }
        }

        // ====================================================================================================
        //  UPGRADE TOWER
        // ====================================================================================================

        public bool UpgradeTower(uint towerId, TowerUpgrade upgrade)
        {
            var tower = GetTower(towerId);
            if (tower == null) return false;
            if (!CanAffordUpgrade(upgrade)) return false;

            try
            {
                tower.ApplyUpgrade(upgrade);
                EconomyManager.RemoveCash(upgrade.Cost);
                OnTowerUpgraded?.Invoke(tower, upgrade);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ====================================================================================================
        //  DESTROY TOWER
        // ====================================================================================================

        public bool DestroyTower(uint towerId)
        {
            var tower = GetTower(towerId);
            if (tower == null) return false;

            try
            {
                // Modern ECS: retrieve entity via runtime
                var entity = _runtime.GetEntity(towerId);
                if (entity != null)
                    _runtime.DestroyEntity(entity);

                _towerRegistry.Unregister(tower);
                _towers.Remove(tower);

                var gridPos = _navigationGrid.WorldToGrid(tower.Position);
                _navigationGrid.SetOccupied(gridPos.X, gridPos.Y, gridPos, false);

                EconomyManager.AddCash(GetTowerRefund(tower.Type));

                OnTowerDestroyed?.Invoke(tower);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ====================================================================================================
        //  QUERIES
        // ====================================================================================================

        public Tower GetTower(uint towerId) =>
            _towers.FirstOrDefault(t => t.Id == towerId);

        public IEnumerable<Tower> GetTowersInArea(Vector3 center, float radius) =>
            _towers.Where(t => Vector3.Distance(t.Position, center) <= radius);

        public IEnumerable<Tower> GetTowersByType(TowerType towerType) =>
            _towers.Where(t => t.Type == towerType);

        // ====================================================================================================
        //  PLACEMENT VALIDATION
        // ====================================================================================================

        public bool ValidatePlacement(Vector3Int position, TowerType towerType)
        {
            if (!_navigationGrid.IsInBounds(position.X, position.Y)) return false;
            if (_navigationGrid.IsTileOccupied(position.X, position.Y)) return false;

            if (IsTooCloseToOtherTowers(position, towerType)) return false;
            if (BlocksEnemyPath(position, towerType)) return false;

            return true;
        }

        private bool IsTooCloseToOtherTowers(Vector3Int position, TowerType towerType)
        {
            float minDist = GetMinDistanceFromTowers(towerType);
            Vector3 worldPos = _navigationGrid.GridToWorld(position);

            foreach (var tower in _towers)
            {
                if (Vector3.Distance(worldPos, tower.Position) < minDist)
                    return true;
            }
            return false;
        }

        private bool BlocksEnemyPath(Vector3Int position, TowerType towerType)
        {
            Vector3 worldPos = _navigationGrid.GridToWorld(position);
            return _pathBlockEvaluator.WouldBlockPath(worldPos, towerType);
        }

        private float GetMinDistanceFromTowers(TowerType towerType)
        {
            return towerType switch
            {
                TowerType.Basic => 2.0f,
                TowerType.Sniper => 3.5f,
                TowerType.Splash => 2.5f,
                TowerType.Freeze => 3.0f,
                _ => 2.0f
            };
        }

        // ====================================================================================================
        //  ECONOMY
        // ====================================================================================================

        public bool CanAffordTower(TowerType towerType) =>
            EconomyManager.CurrentCash >= GetTowerCost(towerType);

        public bool CanAffordUpgrade(TowerUpgrade upgrade) =>
            EconomyManager.CurrentCash >= upgrade.Cost;

        public int GetTowerCost(TowerType towerType) =>
            TowerCostDatabase.Instance.GetCost(towerType);

        public int GetTowerRefund(TowerType towerType)
        {
            int cost = GetTowerCost(towerType);
            return (int)(cost * 0.5f);
        }

        // ====================================================================================================
        //  EVENTS
        // ====================================================================================================

        public event Action<Tower> OnTowerPlaced;
        public event Action<Tower, TowerUpgrade> OnTowerUpgraded;
        public event Action<Tower> OnTowerDestroyed;

        // ====================================================================================================
        //  CLEANUP
        // ====================================================================================================

        public void Cleanup()
        {
            _towers.Clear();
            _towerRegistry.Clear();
            _nextTowerId = 1;
            _isInitialized = false;
        }
    }
}
