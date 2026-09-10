// ====================================================================================================
//  FILE: TowerRegistry.cs
//  PATH: ./Engine/Towers/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the TowerRegistry module.
//
//  RESPONSIBILITIES:
//      - Provide GetTower() behavior for the Core subsystem.
//      - Provide GetTower() behavior for the Core subsystem.
//      - Provide AddTower() behavior for the Core subsystem.
//      - Provide RemoveTower() behavior for the Core subsystem.
//      - Provide ClearAllTowers() behavior for the Core subsystem.
//      - Provide GetAllTowers() behavior for the Core subsystem.
//      - Provide GetTowerCount() behavior for the Core subsystem.
//      - Provide GetTotalTowerValue() behavior for the Core subsystem.
//      - Provide GetTowersByType() behavior for the Core subsystem.
//      - Provide GetTowerType() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    TowerRegistry.cs
Purpose: Registry for managing all tower instances in the game.
Features: Tower registration, retrieval, and management.
*/

using System.Collections.Generic;
using System.Linq;
using static SASZombieAssaultTD.Engine.Towers.TowerEnums;

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

        public TowerRegistry() => Instance = this;

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

