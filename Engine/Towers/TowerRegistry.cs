/*
File:    TowerRegistry.cs
Purpose: Registry for managing all tower instances in the game.
Features: Tower registration, retrieval, and management.
*/

using System;
using System.Collections.Generic;
using System.Linq;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Towers
{
    ///<summary>
    ///Registry for managing all tower instances in the game.
    ///Provides centralized access to tower data and statistics.
    ///</summary>
    public class TowerRegistry
    {
        ///<summary>
        ///Global instance of the tower registry.
        ///Assigned when the registry is constructed.
        ///</summary>
        public static TowerRegistry Instance { get; private set; }

        private readonly Dictionary<uint, Tower> _towers = new Dictionary<uint, Tower>();
        private readonly Dictionary<string, TowerType> _towerTypes = new Dictionary<string, TowerType>();

        public TowerRegistry()
        {
            Instance = this;
        }

        public Tower GetTower(uint id)
        {
            _towers.TryGetValue(id, out var tower);
            return tower;
        }

        public Tower GetTower(string name)
        {
            return _towers.Values.FirstOrDefault(t => t.Name == name);
        }

        public void AddTower(Tower tower)
        {
            if (tower != null)
            {
                _towers[tower.Id] = tower;
            }
        }

        public void RemoveTower(uint id)
        {
            _towers.Remove(id);
        }

        public void ClearAllTowers()
        {
            _towers.Clear();
        }

        public IEnumerable<Tower> GetAllTowers()
        {
            return _towers.Values;
        }

        public int GetTowerCount()
        {
            return _towers.Count;
        }

        public int GetTotalTowerValue()
        {
            return _towers.Values.Sum(t => t.Cost);
        }

        public IEnumerable<Tower> GetTowersByType(TowerType type)
        {
            return _towers.Values.Where(t => t.Type == type);
        }

        public TowerType GetTowerType(string typeName)
        {
            _towerTypes.TryGetValue(typeName, out var type);
            return type;
        }
    }
}
