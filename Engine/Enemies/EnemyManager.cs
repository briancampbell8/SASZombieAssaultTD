/*
File:    EnemyManager.cs
Folder:  Engine/Enemies/
Purpose:  Enemy management system for SAS Zombie Assault TD.
Features: Enemy spawning, enumeration, and lifecycle management.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.ECS;

namespace SASZombieAssaultTD.Engine.Enemies
{
    /// <summary>
    /// Enemy management system for SAS Zombie Assault TD.
    /// Handles enemy spawning, tracking, and lifecycle management.
    /// </summary>
    public class EnemyManager
    {
        ///  Private Fields

        private readonly List<Enemy> _enemies = new();
        private readonly ECSWorld _ecsWorld;
        private uint _nextEnemyId = 1;

        /// 

        ///  Constructor

        /// <summary>
        /// Creates a new enemy manager.
        /// </summary>
        /// <param name="ecsWorld">The ECS world for entity management.</param>
        public EnemyManager(ECSWorld ecsWorld)
        {
            _ecsWorld = ecsWorld ?? throw new ArgumentNullException(nameof(ecsWorld));
        }

        /// 

        ///  Public API

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
                // Create ECS entity for the enemy
                var entity = _ecsWorld.CreateEntity();
                var enemy = new Enemy(entity);
                enemy.Type = enemyType.ToString();
                enemy.Position = position;
                
                // Add enemy to tracking list
                _enemies.Add(enemy);
                
                return enemy;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to spawn enemy {enemyType}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets all currently active enemies.
        /// </summary>
        /// <returns>Collection of all active enemies.</returns>
        public IEnumerable<Enemy> GetAllEnemies()
        {
            return _enemies.Where(e => e.IsActive);
        }

        /// <summary>
        /// Gets all enemies as IReadOnlyList for compatibility.
        /// </summary>
        /// <returns>ReadOnly list of all enemies.</returns>
        public IReadOnlyList<Enemy> GetAllEnemiesReadOnly()
        {
            return _enemies.AsReadOnly();
        }

        /// <summary>
        /// Gets all enemies (including inactive ones).
        /// </summary>
        /// <returns>Collection of all enemies.</returns>
        public IEnumerable<Enemy> GetAllEnemiesIncludingInactive()
        {
            return _enemies;
        }

        /// <summary>
        /// Gets enemies of a specific type.
        /// </summary>
        /// <param name="enemyType">The enemy type to filter by.</param>
        /// <returns>Collection of enemies of the specified type.</returns>
        public IEnumerable<Enemy> GetEnemiesByType(EnemyType enemyType)
        {
            return _enemies.Where(e => e.Type.ToString() == enemyType.ToString() && e.IsActive);
        }

        /// <summary>
        /// Gets enemies within a specified radius.
        /// </summary>
        /// <param name="center">The center position.</param>
        /// <param name="radius">The search radius.</param>
        /// <returns>Collection of enemies within the radius.</returns>
        public IEnumerable<Enemy> GetEnemiesInRadius(Vector3 center, float radius)
        {
            return _enemies.Where(e => e.IsActive && 
                Vector3.Distance(e.Position, center) <= radius);
        }

        /// <summary>
        /// Removes an enemy from the game.
        /// </summary>
        /// <param name="enemy">The enemy to remove.</param>
        public void RemoveEnemy(Enemy enemy)
        {
            if (enemy == null) return;

            enemy.IsActive = false;
            
            // Remove from ECS world if entity exists
            if (enemy.Entity != null)
            {
                _ecsWorld.DestroyEntity(enemy.Entity);
            }
            
            _enemies.Remove(enemy);
        }

        /// <summary>
        /// Updates all enemies.
        /// </summary>
        /// <param name="deltaTime">Time since last update.</param>
        public void Update(float deltaTime)
        {
            // Update all active enemies
            foreach (var enemy in _enemies.Where(e => e.IsActive))
            {
                enemy.Update(deltaTime);
            }

            // Remove dead enemies
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

        /// <summary>
        /// Clears all enemies.
        /// </summary>
        public void ClearAllEnemies()
        {
            foreach (var enemy in _enemies)
            {
                if (enemy.Entity != null)
                {
                    _ecsWorld.DestroyEntity(enemy.Entity);
                }
            }
            
            _enemies.Clear();
        }

        /// 

        ///  Private Methods

        /// <summary>
        /// Updates enemy statistics.
        /// </summary>
        /// <param name="enemy">The enemy to update.</param>
        private void UpdateEnemyStats(Enemy enemy)
        {
            // Update enemy stats based on difficulty, wave, etc.
            // This would integrate with the difficulty system
        }

        /// 
    }
}
