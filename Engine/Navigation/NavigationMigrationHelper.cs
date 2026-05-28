// ============================================================================
// FILE: Engine/Navigation/NavigationMigrationHelper.cs
// AUTHOR: BDC
// PURPOSE: Helper for migrating between navigation systems.
// ============================================================================
/*
File:    NavigationMigrationHelper.cs
Purpose: Helper for migrating between navigation systems.
*/
using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.VectorMath;
using System.Diagnostics;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Navigation
{
    /// <summary>
    /// Provides utilities for migrating legacy movement systems to the modern
    /// NavAgent-based navigation architecture. Supports single-entity migration,
    /// batch migration, validation, and migration analysis.
    /// </summary>
    public static class NavigationMigrationHelper
    {
        // ---------------------------------------------------------------------
        // SINGLE ENTITY MIGRATION
        // ---------------------------------------------------------------------

        /// <summary>
        /// Migrates a single entity from legacy MovementComponent to NavAgentComponent.
        /// Ensures required components exist, configures NavAgent behavior, and
        /// removes legacy movement to prevent conflicts.
        /// </summary>
        public static bool MigrateToNavAgent(Entity entity, ECSWorld world)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (world == null)
                throw new ArgumentNullException(nameof(world));

            var result = true;

            // Required components
            var transform = entity.GetComponent<TransformComponent>();
            var enemyType = entity.GetComponent<EnemyTypeComponent>();
            var legacyMovement = entity.GetComponent<MovementComponent>();

            if (transform == null)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("MIGRATION", "Entity missing TransformComponent.");
                return false;
            }

            if (enemyType == null)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("MIGRATION", "Entity missing EnemyTypeComponent.");
                return false;
            }

            // Skip if already migrated
            if (entity.HasComponent<NavAgentComponent>())
            {
                Engine.Diagnostics.DebugLogger.LogDebug("MIGRATION", "Entity already has NavAgentComponent. Skipping.");
                return true;
            }

            // Extract legacy movement data
            float speed = legacyMovement?.Speed ?? 1.0f;

            // Create NavAgent
            var navAgent = new NavAgentComponent
            {
                Speed = speed,
                StoppingDistance = 0.5f,
                MaxRepathAttempts = 3,
                RepathOnBlock = true
            };

            // Apply behavior-specific configuration
            ConfigureNavAgentForEnemyType(navAgent, enemyType.Type);

            entity.AddComponent(navAgent);

            // Remove legacy movement
            if (legacyMovement != null)
                entity.RemoveComponent<MovementComponent>();

            Engine.Diagnostics.DebugLogger.LogDebug("MIGRATION", $"Migrated entity {entity.Id} to NavAgent.");

            return result;
        }

        // ---------------------------------------------------------------------
        // BATCH MIGRATION
        // ---------------------------------------------------------------------

        /// <summary>
        /// Migrates a collection of entities to NavAgent-based navigation.
        /// Returns the number of successfully migrated entities.
        /// </summary>
        public static int BatchMigrateToNavAgent(IEnumerable<Entity> entities, ECSWorld world)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            int migrated = 0;

            foreach (var entity in entities)
            {
                if (MigrateToNavAgent(entity, world))
                    migrated++;
            }

            Engine.Diagnostics.DebugLogger.LogDebug("MIGRATION", $"Batch migration complete. Migrated {migrated} entities.");

            return migrated;
        }

        // ---------------------------------------------------------------------
        // DIRECT NAVAGENT CREATION
        // ---------------------------------------------------------------------

        /// <summary>
        /// Creates a new entity with NavAgent navigation already configured.
        /// </summary>
        public static Entity CreateNavAgentEntity(ECSWorld world, float speed, Vector3 position)
        {
            if (world == null)
                throw new ArgumentNullException(nameof(world));

            var entity = world.CreateEntity();

            entity.AddComponent(new TransformComponent { X = position.X, Y = position.Y });
            entity.AddComponent(new NavAgentComponent
            {
                Speed = speed,
                StoppingDistance = 0.5f,
                MaxRepathAttempts = 3,
                RepathOnBlock = true
            });

            Engine.Diagnostics.DebugLogger.LogDebug("MIGRATION", $"Created NavAgent entity at {position}");

            return entity;
        }

        // ---------------------------------------------------------------------
        // BEHAVIOR-BASED CONFIGURATION
        // ---------------------------------------------------------------------

        /// <summary>
        /// Configures NavAgent settings based on enemy behavior flags.
        /// </summary>
        public static void ConfigureNavAgentForEnemyType(NavAgentComponent agent, EnemyType type)
        {
            switch (type)
            {
                case EnemyType.Aggressive:
                    agent.MaxRepathAttempts = 5;
                    agent.RepathOnBlock = true;
                    break;

                case EnemyType.Patrol:
                    agent.Speed *= 0.8f;
                    agent.MaxRepathAttempts = 3;
                    agent.RepathOnBlock = true;
                    break;

                case EnemyType.Objective:
                    agent.StoppingDistance = 0.2f;
                    agent.MaxRepathAttempts = 4;
                    agent.RepathOnBlock = true;
                    break;

                case EnemyType.Flocking:
                    agent.UseFlowField = true;
                    break;
            }
        }

        // ---------------------------------------------------------------------
        // VALIDATION
        // ---------------------------------------------------------------------

        /// <summary>
        /// Validates that entities have been properly migrated to NavAgent.
        /// Ensures required components exist and legacy movement is removed.
        /// </summary>
        public static MigrationValidationResult ValidateMigration(IEnumerable<Entity> entities)
        {
            var result = new MigrationValidationResult();

            foreach (var entity in entities)
            {
                bool valid = true;

                if (!entity.HasComponent<NavAgentComponent>())
                {
                    result.AddIssue(entity.Id, "Missing NavAgentComponent.");
                    valid = false;
                }

                if (entity.HasComponent<MovementComponent>())
                {
                    result.AddIssue(entity.Id, "Legacy MovementComponent still present.");
                    valid = false;
                }

                if (!entity.HasComponent<TransformComponent>())
                {
                    result.AddIssue(entity.Id, "Missing TransformComponent.");
                    valid = false;
                }

                if (!entity.HasComponent<EnemyTypeComponent>())
                {
                    result.AddIssue(entity.Id, "Missing EnemyTypeComponent.");
                    valid = false;
                }

                if (valid)
                    result.MigratedCount++;
            }

            return result;
        }

        // ---------------------------------------------------------------------
        // ANALYSIS
        // ---------------------------------------------------------------------

        /// <summary>
        /// Analyzes migration progress and provides recommendations.
        /// </summary>
        public static MigrationAnalysisResult AnalyzeMigration(ECSWorld world)
        {
            var entities = world.GetAllEntities();
            var total = entities.Count;
            var migrated = entities.Count(e => e.HasComponent<NavAgentComponent>());

            float percentage = total == 0 ? 0 : (float)migrated / total * 100f;

            var result = new MigrationAnalysisResult
            {
                TotalEntities = total,
                MigratedEntities = migrated,
                MigrationPercentage = percentage,
                FullyMigrated = percentage >= 95f
            };

            if (!result.FullyMigrated)
            {
                if (percentage < 50f)
                    result.Recommendations.Add("Migration is below 50%. Consider bulk migration.");
                else
                    result.Recommendations.Add("Migration partially complete. Continue migrating remaining entities.");
            }

            return result;
        }
    }

    // =====================================================================
    // SUPPORTING TYPES
    // =====================================================================

    /// <summary>
    /// Stores validation results for migration checks.
    /// </summary>
    public sealed class MigrationValidationResult
    {
        public int MigratedCount { get; set; }
        public Dictionary<int, List<string>> Issues { get; } = new();

        public void AddIssue(int entityId, string issue)
        {
            if (!Issues.ContainsKey(entityId))
                Issues[entityId] = new List<string>();

            Issues[entityId].Add(issue);
        }

        public bool Success => Issues.Count == 0;
    }

    /// <summary>
    /// Stores migration analysis statistics and recommendations.
    /// </summary>
    public sealed class MigrationAnalysisResult
    {
        public int TotalEntities { get; set; }
        public int MigratedEntities { get; set; }
        public float MigrationPercentage { get; set; }
        public bool FullyMigrated { get; set; }
        public List<string> Recommendations { get; } = new();
    }

    // =====================================================================
    // INFERRED ECS + NAVIGATION STRUCTURES
    // =====================================================================

    public enum EnemyType
    {
        Aggressive,
        Patrol,
        Objective,
        Flocking
    }

    public sealed class NavAgentComponent : BaseComponent
    {
        public float Speed;
        public float StoppingDistance;
        public int MaxRepathAttempts;
        public bool RepathOnBlock;
        public bool UseFlowField;
        
        // Navigation state properties
        public bool PathValid { get; set; }
        public List<Vector3> CurrentPath { get; set; } = new List<Vector3>();
        public Vector3 TargetPosition { get; set; }
        
        public string GetStateSummary()
        {
            return $"Speed: {Speed}, PathValid: {PathValid}, PathLength: {CurrentPath.Count}, Target: {TargetPosition}";
        }
    }

    public sealed class TransformComponent : BaseComponent
    {
        public float X;
        public float Y;
        
        public Vector3 Position => new Vector3(X, Y, 0f);
    }

    public sealed class EnemyTypeComponent : BaseComponent
    {
        public EnemyType Type;
    }

    public sealed class MovementComponent : BaseComponent
    {
        public float Speed;
    }

    public abstract class BaseComponent
    {
        public Entity Entity { get; internal set; }
        public virtual void OnAttach() { }
        public virtual void Update(float dt) { }
        public virtual void OnDetach() { }
    }

    public sealed class Entity
    {
        private readonly Dictionary<Type, BaseComponent> _components = new();

        public int Id { get; }
        private readonly ECSWorld _world;

        internal Entity(int id, ECSWorld world)
        {
            Id = id;
            _world = world;
        }

        public void AddComponent(BaseComponent comp)
        {
            var type = comp.GetType();
            _components[type] = comp;
            comp.Entity = this;
            comp.OnAttach();
        }

        public void RemoveComponent<T>() where T : BaseComponent
        {
            var type = typeof(T);
            if (_components.TryGetValue(type, out var comp))
            {
                comp.OnDetach();
                _components.Remove(type);
            }
        }

        public bool HasComponent<T>() where T : BaseComponent =>
            _components.ContainsKey(typeof(T));

        public T GetComponent<T>() where T : BaseComponent =>
            _components.TryGetValue(typeof(T), out var comp) ? (T)comp : null;
    }

    public sealed class ECSWorld
    {
        private int _nextId = 1;
        private readonly Dictionary<int, Entity> _entities = new();

        public Entity CreateEntity()
        {
            var e = new Entity(_nextId++, this);
            _entities[e.Id] = e;
            return e;
        }

        public List<Entity> GetAllEntities() => new(_entities.Values);
    }

    public static class ModernLoggingSystem
    {
        public static void Log(string category, string message)
        {
            Debug.WriteLine($"[{category}] {message}");
        }
    }
}
