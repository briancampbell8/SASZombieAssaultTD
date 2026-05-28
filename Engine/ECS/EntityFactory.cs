// ============================================================================
// FILE: Engine/ECS/EntityFactory.cs
// AUTHOR: BDC
// PURPOSE: Centralized factory for creating ECS entities with correct component
//          composition, type-safe construction, and legacy migration support.
// ============================================================================

using SASZombieAssaultTD.Engine.Components;
using SASZombieAssaultTD.Engine.Core;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace SASZombieAssaultTD.Engine.ECS
{
    /// <summary>
    /// Provides a centralized, deterministic factory for creating ECS entities.
    /// Ensures correct component composition, type safety, and consistent
    /// initialization for enemies, projectiles, players, and towers.
    /// Also provides migration utilities for converting legacy entities.
    /// </summary>
    
    public static class EntityFactory
    {
        ///  Public Creation Methods

        /// <summary>
        /// Creates a fully configured enemy entity with all required components.
        /// </summary>
        /// 

        public static Entity CreateEnemy(ECSWorld world, EnemyType type, Vector3 position)
        {
            ValidateWorld(world);

            var entity = world.CreateEntity();

            AddCoreComponents(entity, position, $"enemy_{type.ToString().ToLower()}");

            var stats = EnemyStats.Get(type);
            
            entity.AddComponent(new EnemyTypeComponent { Type = (SASZombieAssaultTD.Engine.ECS.EnemyType)(SASZombieAssaultTD.Engine.Enemies.ZombieType)type });
            entity.AddComponent(component: new HealthComponent { CurrentHealth = stats.Health, MaxHealth = stats.Health });
            entity.AddComponent(new MovementComponent { Speed = (float)stats.Speed });
            entity.AddComponent(new ScoreComponent { ScoreValue = stats.Score });
            entity.AddComponent(new ActiveComponent { IsActive = true });

            Engine.Diagnostics.DebugLogger.LogDebug("FACTORY", $"Created enemy '{type}' at {position}");
            return entity;
        }

        /// <summary>
        /// Creates a projectile entity with damage, movement, and lifetime.
        /// </summary>
        // Numeric standardization: All continuous values use double for precision
        public static Entity CreateProjectile(ECSWorld world, Vector3 position, Vector3 velocity, double damage, double lifetimeSeconds = 5.0)
        {
            ValidateWorld(world);

            var entity = world.CreateEntity();
            AddCoreComponents(entity, position, "projectile");
            entity.AddComponent(new SASZombieAssaultTD.Engine.Components.DamageComponent { Damage = (float)damage });
            entity.AddComponent(new SASZombieAssaultTD.Engine.Components.MovementComponent { Speed = (float)velocity.Length() });
            entity.AddComponent(new SASZombieAssaultTD.Engine.Components.LifetimeComponent { RemainingSeconds = (float)lifetimeSeconds });
            entity.AddComponent(new SASZombieAssaultTD.Engine.Components.ActiveComponent { IsActive = true });
            Engine.Diagnostics.DebugLogger.LogDebug("FACTORY", $"Created projectile at {position} with velocity {velocity}");
            return entity;
        }

        /// <summary>
        /// Creates a player entity with fixed stats and active state.
        /// </summary>
        public static Entity CreatePlayer(ECSWorld world, Vector3 position)
        {
            ValidateWorld(world);

            var entity = world.CreateEntity();

            AddCoreComponents(entity, position, "player");

            entity.AddComponent(new HealthComponent { CurrentHealth = 100, MaxHealth = 100 });
            entity.AddComponent(new MovementComponent { Speed = 2.0f });
            entity.AddComponent(new ActiveComponent { IsActive = true });

            Engine.Diagnostics.DebugLogger.LogDebug("FACTORY", $"Created player at {position}");
            return entity;
        }

        /// <summary>
        /// Creates a tower entity with configurable type and health.
        /// </summary>
        public static Entity CreateTower(ECSWorld world, Vector3 position, string towerType = "Basic")
        {
            ValidateWorld(world);

            var entity = world.CreateEntity();

            AddCoreComponents(entity, position, $"tower_{towerType.ToLower()}");

            entity.AddComponent(new HealthComponent { CurrentHealth = 200, MaxHealth = 200 });
            entity.AddComponent(new ActiveComponent { IsActive = true });

            Engine.Diagnostics.DebugLogger.LogDebug("FACTORY", $"Created tower '{towerType}' at {position}");
            return entity;
        }

        /// 

        ///  Legacy Migration Methods

        /// <summary>
        /// Migrates a legacy Enemy object into a modern ECS entity.
        /// Preserves position, health, speed, type, and active state.
        /// </summary>
        public static Entity MigrateLegacyEnemy(ECSWorld world, LegacyEnemy legacy)
        {
            ValidateWorld(world);
            ValidateLegacyObject(legacy);

            var ecsType = LegacyEnemyTypeMapper.Map(legacy.Type);
            var entity = CreateEnemy(world, ecsType, legacy.Position);

            if (entity.TryGetComponent<HealthComponent>(out var healthComp))
                healthComp.CurrentHealth = legacy.Health;

            if (entity.TryGetComponent<MovementComponent>(out var movementComp))
                movementComp.Speed = legacy.Speed;

            if (entity.TryGetComponent<ScoreComponent>(out var scoreComp))
                scoreComp.ScoreValue = legacy.Score;

            if (entity.TryGetComponent<ActiveComponent>(out var activeComp))
                activeComp.IsActive = legacy.IsActive;

            Engine.Diagnostics.DebugLogger.LogDebug("MIGRATION", $"Migrated legacy enemy '{legacy.Type}'");
            return entity;
        }

        /// <summary>
        /// Migrates a legacy Projectile object into a modern ECS entity.
        /// Preserves position, velocity, damage, and active state.
        /// </summary>
        public static Entity MigrateLegacyProjectile(ECSWorld world, LegacyProjectile legacy)
        {
            ValidateWorld(world);
            ValidateLegacyObject(legacy);

            var entity = CreateProjectile(world, legacy.Position, legacy.Velocity, legacy.Damage);

            if (entity.TryGetComponent<ActiveComponent>(out var activeComp))
                activeComp.IsActive = legacy.IsActive;

            Engine.Diagnostics.DebugLogger.LogDebug("MIGRATION", "Migrated legacy projectile");
            return entity;
        }

        /// 

        ///  Private Helper Methods

        static void AddCoreComponents(Entity entity, Vector3 position, string spriteId)
        {
            entity.AddComponent(new SASZombieAssaultTD.Engine.Components.TransformComponent { X = position.X, Y = position.Y });
            entity.AddComponent(new SASZombieAssaultTD.Engine.Components.RenderableComponent { SpriteId = spriteId });
        }

        static void ValidateWorld(ECSWorld world)
        {
            if (world == null)
                throw new ArgumentNullException(nameof(world));
        }

        static void ValidateLegacyObject(object legacy)
        {
            if (legacy == null)
                throw new ArgumentNullException(nameof(legacy));
        }

        /// 
    }

    ///  Supporting Types

    /// <summary>
    /// Defines enemy types used by the ECS.
    /// </summary>
    public enum EnemyType
    {
        Zombie,
        Runner,
        Tank,
        Swarm,
        Boss
    }

    /// <summary>
    /// Stores default stats for each enemy type.
    /// </summary>
    public static class EnemyStats
    {
        // Numeric standardization: All continuous values use double for precision
        static readonly Dictionary<EnemyType, (double Health, double Speed, int Score)> _stats = new()
            {
                { EnemyType.Zombie, (100.0, 1.0, 10) },
                { EnemyType.Runner, (60.0, 2.5, 15) },
                { EnemyType.Tank, (300.0, 0.5, 25) },
                { EnemyType.Swarm, (30.0, 1.5, 8) },
                { EnemyType.Boss, (1000.0, 1.2, 100) }
            };

        public static (double Health, double Speed, int Score) Get(EnemyType type) => _stats[type];
    }

    /// <summary>
    /// Legacy enemy type enumeration for migration support.
    /// </summary>
    public enum LegacyEnemyType
    {
        Basic,
        Fast,
        Tank,
        Swarm,
        Boss
    }

    /// <summary>
    /// Legacy enemy class for migration support.
    /// </summary>
    public class LegacyEnemy
    {
        public LegacyEnemyType Type { get; set; }
        public Vector3 Position { get; set; }
        public float Health { get; set; }
        public float Speed { get; set; }
        public int Score { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Legacy projectile class for migration support.
    /// </summary>
    public class LegacyProjectile
    {
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public float Damage { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Maps legacy enemy types to ECS enemy types.
    /// </summary>
    public static class LegacyEnemyTypeMapper
    {
        public static EnemyType Map(LegacyEnemyType legacyType) =>
            legacyType switch
            {
                LegacyEnemyType.Basic => EnemyType.Zombie,
                LegacyEnemyType.Fast => EnemyType.Runner,
                LegacyEnemyType.Tank => EnemyType.Tank,
                LegacyEnemyType.Swarm => EnemyType.Swarm,
                LegacyEnemyType.Boss => EnemyType.Boss,
                _ => EnemyType.Zombie
            };
    }

    /// 
}