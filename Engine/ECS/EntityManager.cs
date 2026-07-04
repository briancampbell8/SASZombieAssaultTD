/*
File:    EntityManager.cs
Purpose: P11-13-09 - Entity management system replacing legacy managers.
*/
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Physics.Components;
using SASZombieAssaultTD.Engine.Components;
using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Dictionary;
using System.Linq;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.ECS
//
{
    ///<summary>
    ///P11-13-09: Entity management system that replaces EnemyManager and ProjectileManager.
    ///Provides query methods and entity lifecycle management using ECS queries.
    ///</summary>
    public sealed class EntityManager
    {
        private readonly ECSWorld _ecsWorld;
        internal IEnumerable<object> Entities;

        ///<summary>
        ///Initializes a new EntityManager.
        ///</summary>
        ///<param name="ecsWorld">The ECS world to manage entities in.</param>
        public EntityManager(ECSWorld ecsWorld)
        {
            _ecsWorld = ecsWorld ?? throw new ArgumentNullException(nameof(ecsWorld));
            DLogger.Log(LogSubsystems.ECS, LogLevel.Info, "EntityManager: Initialized");
        }

        public EntityManager()
        {
        }

        /// Query Methods

        public IEnumerable<Entity> GetEnemies() => _ecsWorld.GetEntitiesWith<SASZombieAssaultTD.Engine.Components.EnemyTypeComponent>();

        public IEnumerable<Entity> GetActiveEnemies() =>
            _ecsWorld.GetEntitiesWith<SASZombieAssaultTD.Engine.Components.EnemyTypeComponent, SASZombieAssaultTD.Engine.Components.ActiveComponent>()
                     .Where(entity => entity.TryGetComponent<SASZombieAssaultTD.Engine.Components.ActiveComponent>(out var comp) ? comp.IsActive == true : false);

        public IEnumerable<Entity> GetEnemiesByType(EnemyType enemyType) =>
            _ecsWorld.GetEntitiesWith<SASZombieAssaultTD.Engine.Components.EnemyTypeComponent>()
                     .Where(entity => entity.TryGetComponent<SASZombieAssaultTD.Engine.Components.EnemyTypeComponent>(out var comp) ? comp.Type == enemyType : false);

        public IEnumerable<Entity> GetProjectiles() =>
            _ecsWorld.GetEntitiesWith<SASZombieAssaultTD.Engine.Components.DamageComponent, SASZombieAssaultTD.Engine.Components.ActiveComponent>()
                     .Where(entity => !entity.HasComponent<SASZombieAssaultTD.Engine.Components.EnemyTypeComponent>());

        public IEnumerable<Entity> GetActiveProjectiles() =>
            _ecsWorld.GetEntitiesWith<SASZombieAssaultTD.Engine.Components.DamageComponent, SASZombieAssaultTD.Engine.Components.ActiveComponent>()
                     .Where(entity => entity.TryGetComponent<SASZombieAssaultTD.Engine.Components.ActiveComponent>(out var comp) ? comp.IsActive == true : false &&
                                      !entity.HasComponent<SASZombieAssaultTD.Engine.Components.EnemyTypeComponent>());

        public IEnumerable<Entity> GetDamageDealers() => _ecsWorld.GetEntitiesWith<SASZombieAssaultTD.Engine.Components.DamageComponent>();

        public IEnumerable<Entity> GetDamageableEntities() =>
            _ecsWorld.GetEntitiesWith<SASZombieAssaultTD.Engine.Components.HealthComponent>()
                     .Where(entity => !entity.TryGetComponent<SASZombieAssaultTD.Engine.Components.HealthComponent>(out var healthComp) || !healthComp.IsDead);

        public IEnumerable<Entity> GetDeadEntities() =>
            _ecsWorld.GetEntitiesWith<SASZombieAssaultTD.Engine.Components.HealthComponent>()
                     .Where(entity => entity.TryGetComponent<SASZombieAssaultTD.Engine.Components.HealthComponent>(out var healthComp) && !healthComp.IsDead);

        public IEnumerable<Entity> GetInactiveEntities() =>
            _ecsWorld.GetEntitiesWith<SASZombieAssaultTD.Engine.Components.ActiveComponent>()
                     .Where(entity => !entity.TryGetComponent<SASZombieAssaultTD.Engine.Components.ActiveComponent>(out var activeComp) || !activeComp.IsActive);

        public IEnumerable<Entity> GetEntitiesInRadius(Vector3 position, float radius) =>
            _ecsWorld.Entities
                     .Where(entity => entity.IsEnabled && !entity.IsDestroyed)
                     .Where(entity =>
                     {
                         if (entity.TryGetComponent<SASZombieAssaultTD.Engine.Components.TransformComponent>(out var transform))
                             return Vector3.Distance(transform.Position, position) <= radius;
                         return false;
                     });

        public IEnumerable<Entity> GetEnemiesInRadius(Vector3 position, float radius) =>
            GetEntitiesInRadius(position, radius).Where(entity => entity.HasComponent<EnemyTypeComponent>());

        public IEnumerable<Entity> GetProjectilesInRadius(Vector3 position, float radius) =>
            GetEntitiesInRadius(position, radius)
                .Where(entity => entity.HasComponent<DamageComponent>() && !entity.HasComponent<EnemyTypeComponent>());

        public Entity? GetNearestEnemy(Vector3 position, float maxRange = float.MaxValue) =>
            GetEnemiesInRadius(position, maxRange)
                .OrderBy(entity => Vector3.Distance(entity.TryGetComponent<SASZombieAssaultTD.Engine.Components.TransformComponent>(out var transform) ? transform.Position : Vector3.Zero, position))
                .FirstOrDefault();

        public Entity? GetNearestEnemyOfType(Vector3 position, EnemyType enemyType, float maxRange = float.MaxValue) =>
            GetEnemiesByType(enemyType)
                .Where(entity => entity.TryGetComponent<SASZombieAssaultTD.Engine.Components.TransformComponent>(out var transform) ? Vector3.Distance(transform.Position, position) <= maxRange : false)
                .OrderBy(entity => Vector3.Distance(entity.TryGetComponent<SASZombieAssaultTD.Engine.Components.TransformComponent>(out var transform) ? transform.Position : Vector3.Zero, position))
                .FirstOrDefault();

        ///<summary>
        ///Gets all entities that have both PhysicsComponent and TransformComponent.
        ///</summary>
        ///<returns>Entities with physics and transform components.</returns>
        public IEnumerable<Entity> GetEntitiesWithPhysicsAndTransform() =>
            _ecsWorld.GetEntitiesWith<SASZombieAssaultTD.Engine.ECS.BaseComponent, SASZombieAssaultTD.Engine.Components.TransformComponent>();

        ///<summary>
        ///Gets all entities that have collision components and TransformComponent.
        ///</summary>
        ///<returns>Entities with collision and transform components.</returns>
        public IEnumerable<Entity> GetEntitiesWithCollisionAndTransform() =>
            _ecsWorld.Entities
                .Where(entity => entity.IsEnabled && !entity.IsDestroyed)
                .Where(entity => entity.HasComponent<TransformComponent>() && 
                                (entity.HasComponent<CollisionComponent>() || entity.HasComponent<PhysicsComponent>()));

        ///

        /// Entity Lifecycle Management

        ///<summary>
        ///Adds an entity to the manager.
        ///</summary>
        ///<param name="entity">The entity to add.</param>
        public void AddEntity(Entity entity)
        {
            //Entity is already managed by ECSWorld, no additional tracking needed
            DLogger.Log(LogSubsystems.ECS, LogLevel.Debug, $"EntityManager: Entity {entity.Id} added");
        }

        ///<summary>
        ///Removes an entity from the manager.
        ///</summary>
        ///<param name="entity">The entity to remove.</param>
        public void RemoveEntity(Entity entity)
        {
            _ecsWorld.DestroyEntity(entity);
            DLogger.Log(LogSubsystems.ECS, LogLevel.Debug, $"EntityManager: Entity {entity.Id} removed");
        }

        public int DestroyDeadEntities() => DestroyEntities(GetDeadEntities(), "dead");

        public int DestroyInactiveEntities() => DestroyEntities(GetInactiveEntities(), "inactive");

        public int DestroyAllProjectiles() => DestroyEntities(GetProjectiles(), "projectiles");

        public int RespawnDeadEnemies()
        {
            var deadEnemies = GetDeadEntities().Where(entity => entity.HasComponent<EnemyTypeComponent>()).ToList();
            int respawnedCount = 0;

            foreach (var enemy in deadEnemies)
            {
                var health = enemy.GetComponent<SASZombieAssaultTD.Engine.Components.HealthComponent>();
                var active = enemy.GetComponent<SASZombieAssaultTD.Engine.Components.ActiveComponent>();

                if (health != null && active != null)
                {
                    health.FullRestore();
                    active.Activate(new Vector3(0f, 0f, 0f));
                    respawnedCount++;
                }
            }

            if (respawnedCount > 0)
            {
                DLogger.Log(LogSubsystems.ECS, LogLevel.Info, $"EntityManager: Respawned {respawnedCount} dead enemies");
            }

            return respawnedCount;
        }

        private int DestroyEntities(IEnumerable<Entity> entities, string entityType)
        {
            var entityList = entities.ToList();
            int destroyedCount = 0;

            foreach (var entity in entityList)
            {
                _ecsWorld.DestroyEntity(entity);
                destroyedCount++;
            }

            if (destroyedCount > 0)
            {
                DLogger.Log(LogSubsystems.ECS, LogLevel.Info, $"EntityManager: Destroyed {destroyedCount} {entityType} entities");
            }

            return destroyedCount;
        }

        ///

        /// Statistics and Debugging

        public EntityStats GetEntityStats()
        {
            var stats = new EntityStats
            {
                TotalEntities = _ecsWorld.EntityCount,
                ActiveEnemies = GetActiveEnemies().Count(),
                DeadEntities = GetDeadEntities().Count(),
                ActiveProjectiles = GetActiveProjectiles().Count(),
                InactiveEntities = GetInactiveEntities().Count()
            };

            foreach (var enemy in GetEnemies())
            {
                var enemyType = enemy.TryGetComponent<SASZombieAssaultTD.Engine.Components.EnemyTypeComponent>(out var comp) ? comp.Type : EnemyType.Zombie;
                stats.EnemyTypeCounts[enemyType] = stats.EnemyTypeCounts.GetValueOrDefault(enemyType, 0) + 1;
            }

            return stats;
        }

        public string GetDebugInfo()
        {
            var stats = GetEntityStats();
            var info = $"EntityManager Debug Info:\n";
            info += $"  Total Entities: {stats.TotalEntities}\n";
            info += $"  Active Enemies: {stats.ActiveEnemies}\n";
            info += $"  Dead Entities: {stats.DeadEntities}\n";
            info += $"  Active Projectiles: {stats.ActiveProjectiles}\n";
            info += $"  Inactive Entities: {stats.InactiveEntities}\n\n";

            info += "Enemy Type Counts:\n";
            foreach (var kvp in stats.EnemyTypeCounts)
            {
                info += $"  {kvp.Key}: {kvp.Value}\n";
            }

            return info;
        }

        ///

        /// Entity and Component Access

        public Entity GetEntity(uint entityId) => _ecsWorld.GetEntity(entityId) ?? new Entity();

        //Compatibility query helpers used by other subsystems
        public IEnumerable<Entity> GetEntitiesWith<T>() where T : BaseComponent
        {
            return _ecsWorld.GetEntitiesWith<T>();
        }

        public IEnumerable<Entity> GetEntitiesWith<T1, T2>()
            where T1 : BaseComponent
            where T2 : BaseComponent
        {
            return _ecsWorld.GetEntitiesWith<T1, T2>();
        }

        public T? GetComponent<T>(uint entityId) where T : class
        {
            var entity = GetEntity(entityId);
            return entity.IsValid ? entity.GetComponent<T>() : null;
        }

        public bool HasComponent<T>(uint entityId)
        {
            var entity = GetEntity(entityId);
            return entity.IsValid && entity.HasComponent<T>();
        }

        public void AddComponent<T>(uint entityId, T component)
        {
            var entity = GetEntity(entityId);
            if (entity.IsValid) entity.AddComponent(component);
        }

        ///
    }

    ///<summary>
    ///Statistics about entities in the world.
    ///</summary>
    public sealed class EntityStats
    {
        public int TotalEntities { get; set; }
        public int ActiveEnemies { get; set; }
        public int DeadEntities { get; set; }
        public int ActiveProjectiles { get; set; }
        public int InactiveEntities { get; set; }
        public Dictionary<EnemyType, int> EnemyTypeCounts { get; set; } = new();

        public override string ToString() =>
            $"Total: {TotalEntities}, Enemies: {ActiveEnemies}, Dead: {DeadEntities}, Projectiles: {ActiveProjectiles}, Inactive: {InactiveEntities}";
    }
}




