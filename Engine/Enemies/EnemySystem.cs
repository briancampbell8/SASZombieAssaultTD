/*
File:    EnemySystem.cs
Purpose: Reference to EntityManager; AddEnemy, RemoveEnemy; enemy update loop in Update.
Features: Event publishing for enemy spawn and death events.
*/
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Events;
using SASZombieAssaultTD.Engine.Navigation;
using SASZombieAssaultTD.Engine.Waves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Enemies
{
    ///<summary>
    ///Service locator pattern for dependency injection.
    ///</summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static void RegisterService<T>(T service)
        {
            _services[typeof(T)] = service;
        }

        public static T GetService<T>()
        {
            return _services.TryGetValue(typeof(T), out var service) ? (T)service : default(T);
        }
    }
    ///<summary>
    ///Manages enemies, including their lifecycle, updates, and event publishing.
    ///</summary>
    public class EnemySystem
    {
        private readonly List<Enemy> _enemies = new();
        private readonly EntityManager _entityManager;
        private readonly EventRouter _eventBus;

        public EnemySystem(EntityManager entityManager, EventRouter eventBus)
        {
            _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        ///<summary>
        ///Gets the list of all enemies.
        ///</summary>
        public IReadOnlyList<Enemy> Enemies => _enemies;

        ///<summary>
        ///Gets or sets the maximum number of enemies allowed.
        ///</summary>
        public int MaxEnemies { get; set; } = 100;

        ///<summary>
        ///Updates all enemies in the system.
        ///</summary>
        ///<param name="deltaTime">Time elapsed since last frame.</param>
        public void UpdateEnemies(float deltaTime)
        {
            foreach (var enemy in _enemies)
            {
                //Update logic for each enemy
                enemy.Update(deltaTime);
            }
        }

        ///<summary>
        ///Removes an enemy by ID.
        ///</summary>
        ///<param name="enemyId">ID of enemy to remove.</param>
        public void RemoveEnemy(int enemyId)
        {
            var enemy = _enemies.FirstOrDefault(e => e.Entity.Id == enemyId);
            if (enemy != null)
            {
                RemoveEnemy(enemy);
            }
        }

        ///<summary>
        ///Clears all enemies from the system.
        ///</summary>
        public void ClearAllEnemies()
        {
            var enemiesToRemove = _enemies.ToList();
            foreach (var enemy in enemiesToRemove)
            {
                RemoveEnemy(enemy);
            }
        }

        ///<summary>
        ///Adds an enemy to the system and publishes an event.
        ///</summary>
        public void AddEnemy(Enemy enemy)
        {
            if (enemy == null) throw new ArgumentNullException(nameof(enemy));
            if (_enemies.Contains(enemy)) return;

            _enemies.Add(enemy);
            //TODO: Fix EntityManager integration when methods are available
            //_entityManager.AddEntity(enemy.Entity);

            _eventBus.Publish(new EnemySpawnedEvent(enemy));
        }

        ///<summary>
        ///Removes an enemy from the system and publishes an event.
        ///</summary>
        ///        public EnemyDeathEvent(int entityId, ZombieType Type, Vector2 Position)
        public EnemySystem( int ntityId, ZombieType type, Vector3 position)
        {
            int entityId = 0;
            int EntityId = entityId;
            ZombieType Type = type;
            Vector3 Position = position;
        }

        public EnemySystem()
        {
        }

        public void RemoveEnemy(Enemy enemy)
        {
            if (enemy == null) throw new ArgumentNullException(nameof(enemy));
            if (!_enemies.Remove(enemy)) return;

            //TODO: Fix EntityManager integration when methods are available
            //_entityManager.RemoveEntity(enemy.Entity);

        //   _eventBus.Publish(
          //     new EnemyDeathEvent(
            //       enemy.Entity.Id,
              //     enemy.Type,
                //   enemy.Position
            _eventBus.Publish(new EnemyDeathEvent(enemy.Entity.Id, enemy.Type, enemy.Position
            ));
        }

        ///<summary>
        ///Updates all enemies and processes deaths.
        ///</summary>
        public void Update(float deltaTime)
        {
            foreach (var enemy in _enemies)
            {
                //Update logic for each enemy (if needed)
            }

            ProcessEnemyDeaths();
        }

        ///<summary>
        ///Processes all dead enemies and removes them from the system.
        ///</summary>
        private void ProcessEnemyDeaths()
        {
            var deadEnemies = _enemies.FindAll(e => e.IsDead);
            foreach (var deadEnemy in deadEnemies)
            {
                KillEnemy(deadEnemy);
            }
        }

        ///<summary>
        ///Handles the death of an enemy.
        ///</summary>
        private void KillEnemy(Enemy enemy)
        {
            if (enemy == null) throw new ArgumentNullException(nameof(enemy));
            RemoveEnemy(enemy);
        }

        ///<summary>
        ///Gets the current wave number from the WaveController.
        ///</summary>
        private int GetCurrentWaveNumber()
        {
            try
            {
                var waveSystem = ServiceLocator.GetService<WaveDirector>();
                return waveSystem?.CurrentWave ?? 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}




