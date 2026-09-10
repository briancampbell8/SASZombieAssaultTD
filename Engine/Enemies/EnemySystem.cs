// ====================================================================================================
//  FILE: EnemySystem.cs
//  PATH: ./Engine/Enemies/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the EnemySystem module.
//
//  RESPONSIBILITIES:
//      - Provide UpdateEnemies() behavior for the Core subsystem.
//      - Provide RemoveEnemy() behavior for the Core subsystem.
//      - Provide ClearAllEnemies() behavior for the Core subsystem.
//      - Provide AddEnemy() behavior for the Core subsystem.
//      - Provide RemoveEnemy() behavior for the Core subsystem.
//      - Provide Update() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    EnemySystem.cs
Purpose: Reference to ECSEntityCore; AddEnemy, RemoveEnemy; enemy update loop in Update.
Features: Event publishing for enemy spawn and death events.
*/
using System;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Linq;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Events;
using SASZombieAssaultTD.Engine.Waves.WaveManagement;
using SASZombieAssaultTD.Engine.Systems;

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
        private readonly ECSEntityCore _ECSEntityCore;
        private readonly ECSRuntimeEvents _eventBus;

        public EnemySystem(ECSEntityCore ECSEntityCore, ECSRuntimeEvents eventBus)
        {
            _ECSEntityCore = ECSEntityCore ?? throw new ArgumentNullException(nameof(ECSEntityCore));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public EnemySystem()
        {
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
                enemy.Update(deltaTime);
            }
        }

        ///<summary>
        ///Removes an enemy by ID.
        ///</summary>
        ///<param name="enemyId">ID of enemy to remove.</param>
        public void RemoveEnemy(int enemyId)
        {
            var enemy = _enemies.FirstOrDefault(e => e.ECSEntityCore.Id == enemyId);
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

            // TODO: Wire to the correct ECSEntityCore API (CreateEntity/RegisterEntity/etc.)
            // Example placeholder:
            // _ECSEntityCore.CreateEntity(enemy.Entity);
            // Replace the above line with your actual canonical ECS method.

            _eventBus.TriggerEvent("EnemySpawned", new EnemySpawnedEvent(enemy));
        }

        ///<summary>
        ///Removes an enemy from the system and publishes an event.
        ///</summary>
        public void RemoveEnemy(Enemy enemy)
        {
            if (enemy == null) throw new ArgumentNullException(nameof(enemy));
            if (!_enemies.Remove(enemy)) return;

            // TODO: Fix ECSEntityCore integration when methods are available.
            // Example placeholder:
            // _ECSEntityCore.RemoveEntity(enemy.Entity);

            _eventBus.TriggerEvent(
                "EnemyDeath",
                new EnemyDeathEvent(
                    enemy.ECSEntityCore.Id,
                    enemy.Type,
                    enemy.Position
                )
            );
        }

        ///<summary>
        ///Updates all enemies and processes deaths.
        ///</summary>
        public void Update(float deltaTime)
        {
            foreach (var enemy in _enemies)
            {
                // Per-frame enemy update logic can be added here if needed.
                enemy.Update(deltaTime);
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
        ///Gets the current wave number from the WaveDirector.
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
