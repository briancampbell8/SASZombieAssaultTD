// ====================================================================================================
//  FILE: EnemyManager.cs
//  PATH: ./Engine/Enemies/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the EnemyManager module.
//
//  RESPONSIBILITIES:
//      - Provide SpawnEnemy() behavior for the Core subsystem.
//      - Provide GetAllEnemies() behavior for the Core subsystem.
//      - Provide GetAllEnemiesReadOnly() behavior for the Core subsystem.
//      - Provide GetAllEnemiesIncludingInactive() behavior for the Core subsystem.
//      - Provide GetEnemiesByType() behavior for the Core subsystem.
//      - Provide GetEnemiesInRadius() behavior for the Core subsystem.
//      - Provide RemoveEnemy() behavior for the Core subsystem.
//      - Provide Update() behavior for the Core subsystem.
//      - Provide ClearAllEnemies() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    EnemyManager.cs
Folder:  Engine/Enemies/
Purpose:  Enemy management system for SAS Zombie Assault TD.
Features: Enemy spawning, enumeration, and lifecycle management.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Enemies.EnemiesEnums;

namespace SASZombieAssaultTD.Engine.Enemies
{
    /// <summary>
    /// Enemy management system for SAS Zombie Assault TD. Handles enemy spawning, tracking, and lifecycle management.
    /// </summary>
    public class EnemyManager : IEnemyManager

    {
        // ---------------------------------------------------------------------------------------------
        // Private Fields
        // ---------------------------------------------------------------------------------------------

        private readonly List<Enemy> _enemies = new();
        private readonly ECSRuntimeCore _ecsWorld;
        private uint _nextEnemyId = 1;

        private int _totalSpawned;
        private int _totalKilled;
        private int _totalEscaped;

        // ---------------------------------------------------------------------------------------------
        // Singleton Instance
        // ---------------------------------------------------------------------------------------------

        public static EnemyManager Instance { get; private set; }

        public EnemyManager(ECSRuntimeCore ecsWorld)
        {
            _ecsWorld = ecsWorld;
            Instance = this;
        }

        // ---------------------------------------------------------------------------------------------
        // Public API
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Spawns a new enemy of the specified type at the given position.
        /// </summary>
        /// <param name="enemyType">The type of enemy to spawn.</param>
        /// <param name="position">The spawn position.</param>
        /// <returns>The spawned enemy, or null if spawn failed.</returns>
        public Enemy SpawnEnemy(EnemyType enemyType, Vector3 position)
        {
            try
            {
                var ECSEntityCore = _ecsWorld.CreateEntity();
                var enemy = new Enemy(ECSEntityCore)
                {
                    Type = enemyType.ToString(),
                    Position = position,
                    IsActive = true
                };

                _enemies.Add(enemy);
                _totalSpawned++;

                return enemy;
            }
            catch (Exception ex)
            {
                DLogger.Log($"Failed to spawn enemy {enemyType}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets all currently active enemies.
        /// </summary>
        public IEnumerable<Enemy> GetAllEnemies()
        {
            return _enemies.Where(e => e.IsActive);
        }

        /// <summary>
        /// Gets all enemies as IReadOnlyList for compatibility.
        /// </summary>
        public IReadOnlyList<Enemy> GetAllEnemiesReadOnly()
        {
            return _enemies.AsReadOnly();
        }

        /// <summary>
        /// Gets all enemies (including inactive ones).
        /// </summary>
        public IEnumerable<Enemy> GetAllEnemiesIncludingInactive()
        {
            return _enemies;
        }

        /// <summary>
        /// Gets the next available enemy ID.
        /// </summary>
        public uint GetNextEnemyId() => _nextEnemyId++;

        /// <summary>
        /// Gets enemies of a specific type.
        /// </summary>
        public IEnumerable<Enemy> GetEnemiesByType(EnemyType enemyType)
        {
            var typeName = enemyType.ToString();
            return _enemies.Where(e => e.Type == typeName && e.IsActive);
        }

        /// <summary>
        /// Gets enemies within a specified radius.
        /// </summary>
        public IEnumerable<Enemy> GetEnemiesInRadius(Vector3 center, float radius)
        {
            return _enemies.Where(e => e.IsActive &&
                Vector3.Distance(e.Position, center) <= radius);
        }

        /// <summary>
        /// Removes an enemy from the game.
        /// </summary>
        public void RemoveEnemy(Enemy enemy)
        {
            if (enemy == null) return;

            if (enemy.IsActive)
            {
                _totalKilled++;
            }

            enemy.IsActive = false;

            if (enemy.ECSEntityCore != null)
            {
                _ecsWorld.DestroyEntity(enemy.ECSEntityCore);
            }

            _enemies.Remove(enemy);
        }

        /// <summary>
        /// Marks an enemy as escaped.
        /// </summary>
        public void MarkEnemyEscaped(Enemy enemy)
        {
            if (enemy == null) return;

            if (enemy.IsActive)
            {
                _totalEscaped++;
            }

            enemy.IsActive = false;

            if (enemy.ECSEntityCore != null)
            {
                _ecsWorld.DestroyEntity(enemy.ECSEntityCore);
            }

            _enemies.Remove(enemy);
        }

        /// <summary>
        /// Updates all enemies.
        /// </summary>
        public void Update(float deltaTime)
        {
            foreach (var enemy in _enemies.Where(e => e.IsActive))
            {
                enemy.Update(deltaTime);
            }

            var deadEnemies = _enemies.Where(e => !e.IsActive).ToList();
            foreach (var deadEnemy in deadEnemies)
            {
                RemoveEnemy(deadEnemy);
            }
        }

        /// <summary>
        /// Gets the count of active enemies.
        /// </summary>
        public int ActiveEnemyCount => _enemies.Count(e => e.IsActive);

        /// <summary>
        /// Gets the total count of enemies (including inactive).
        /// </summary>
        public int TotalEnemyCount => _enemies.Count;

        public IEnumerable<Enemy> ActiveEnemies => _enemies.Where(e => e.IsActive);

        // ---------------------------------------------------------------------------------------------
        // Save Pipeline API (used by GSCapture)
        // ---------------------------------------------------------------------------------------------

        public int GetTotalSpawned()
        {
            return _totalSpawned;
        }

        public int GetTotalKilled()
        {
            return _totalKilled;
        }

        public int GetTotalEscaped()
        {
            return _totalEscaped;
        }

        public int GetActiveEnemyCount()
        {
            return ActiveEnemyCount;
        }

        /// <summary>
        /// Allows external systems to set total spawned (if needed).
        /// </summary>
        public void SetTotalSpawned(int totalSpawned)
        {
            _totalSpawned = totalSpawned;
        }

        /// <summary>
        /// Clears all enemies.
        /// </summary>
        public void ClearAllEnemies()
        {
            foreach (var enemy in _enemies)
            {
                if (enemy.ECSEntityCore != null)
                {
                    _ecsWorld.DestroyEntity(enemy.ECSEntityCore);
                }
            }

            _enemies.Clear();
            _totalSpawned = 0;
            _totalKilled = 0;
            _totalEscaped = 0;
        }

        // ---------------------------------------------------------------------------------------------
        // Private Methods
        // ---------------------------------------------------------------------------------------------

        private void UpdateEnemyStats(Enemy enemy)
        {
            // Integrate with difficulty/wave systems when available.
        }
    }
}