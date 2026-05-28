/*
File:    ECSDebugInspector.cs
Purpose: P11-12-09 - Debug and inspection hooks for the ECS system.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Animation.Components;

namespace SASZombieAssaultTD.Engine.ECS
{
    /// <summary>
    /// P11-12-09: Debug and inspection utilities for the ECS system.
    /// </summary>
    public static class ECSDebugInspector
    {
        /// <summary>
        /// Gets comprehensive debug information about an ECS world.
        /// </summary>
        /// <param name="ecsWorld">The ECS world to inspect.</param>
        /// <returns>Formatted debug information.</returns>
        public static string GetWorldDebugInfo(ECSWorld ecsWorld)
        {
            if (ecsWorld == null)
                return "ECSWorld is null";

            var info = new StringBuilder();
            info.AppendLine("=== ECS World Debug Information ===");
            info.AppendLine($"Total Entities: {ecsWorld.EntityCount}");
            info.AppendLine();

            // Component statistics
            var componentStats = GetComponentStatistics(ecsWorld);
            info.AppendLine("Component Statistics:");
            foreach (var stat in componentStats.OrderByDescending(x => x.Value))
            {
                info.AppendLine($"  {stat.Key}: {stat.Value} entities");
            }
            info.AppendLine();

            // Entity details (limited to first 10 for readability)
            info.AppendLine("Entity Details (first 10):");
            var entities = ecsWorld.Entities.Take(10).ToList();
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                info.AppendLine($"  [{i + 1}] {GetEntityDebugInfo(entity)}");
            }

            if (ecsWorld.EntityCount > 10)
            {
                info.AppendLine($"  ... and {ecsWorld.EntityCount - 10} more entities");
            }

            return info.ToString();
        }

        /// <summary>
        /// Gets debug information about a specific entity.
        /// </summary>
        /// <param name="entity">The entity to inspect.</param>
        /// <returns>Formatted debug information.</returns>
        public static string GetEntityDebugInfo(Entity entity)
        {
            if (entity == null)
                return "Entity is null";

            var info = new StringBuilder();
            info.Append($"Entity.{entity.Id}");
            info.Append($" (Enabled: {entity.IsEnabled}, Destroyed: {entity.IsDestroyed})");

            if (entity.Components.Count > 0)
            {
                info.AppendLine(" Components:");
                foreach (var component in entity.Components)
                {
                    info.AppendLine($"    - {component}");
                }
            }
            else
            {
                info.Append(" (No components)");
            }

            return info.ToString();
        }

        /// <summary>
        /// Gets statistics about component usage across all entities.
        /// </summary>
        /// <param name="ecsWorld">The ECS world to analyze.</param>
        /// <returns>Dictionary mapping component types to entity counts.</returns>
        public static Dictionary<string, int> GetComponentStatistics(ECSWorld ecsWorld)
        {
            var stats = new Dictionary<string, int>();

            foreach (var entity in ecsWorld.Entities)
            {
                foreach (var component in entity.Components)
                {
                    var componentType = component.GetType().Name;
                    if (!stats.ContainsKey(componentType))
                    {
                        stats[componentType] = 0;
                    }
                    stats[componentType]++;
                }
            }

            return stats;
        }

        /// <summary>
        /// Finds entities with specific component combinations.
        /// </summary>
        /// <param name="ecsWorld">The ECS world to search.</param>
        /// <param name="componentTypes">Component types to search for.</param>
        /// <returns>List of entities matching the criteria.</returns>
        public static List<Entity> FindEntitiesWithComponents(ECSWorld ecsWorld, params Type[] componentTypes)
        {
            if (ecsWorld == null || componentTypes == null || componentTypes.Length == 0)
                return new List<Entity>();

            return ecsWorld.Entities.Where(entity =>
            {
                return componentTypes.All(componentType =>
                entity.Components.Any(c => c.GetType() == componentType));
            }).ToList();
        }

        /// <summary>
        /// Logs the current state of the ECS world to the debug log.
        /// </summary>
        /// <param name="ecsWorld">The ECS world to log.</param>
        /// <param name="logLevel">The log level to use.</param>
        public static void LogWorldState(ECSWorld ecsWorld, string logLevel = "INFO")
        {
            var debugInfo = GetWorldDebugInfo(ecsWorld);
            Engine.Diagnostics.DebugLogger.LogDebug(logLevel, debugInfo);
        }

        /// <summary>
        /// Validates the integrity of the ECS world.
        /// </summary>
        /// <param name="ecsWorld">The ECS world to validate.</param>
        /// <returns>Validation result with any issues found.</returns>
        public static ECSValidationResult ValidateWorld(ECSWorld ecsWorld)
        {
            var result = new ECSValidationResult();

            if (ecsWorld == null)
            {
                result.AddError("ECSWorld is null");
                return result;
            }

            // Check for duplicate entity IDs
            var entityIds = ecsWorld.Entities.Select(e => e.Id).ToList();
            var duplicateIds = entityIds.GroupBy(id => id).Where(g => g.Count() > 1).Select(g => g.Key).ToList();

            foreach (var duplicateId in duplicateIds)
            {
                result.AddError($"Duplicate entity ID found: {duplicateId}");
            }

            // Check for entities with invalid component ownership
            foreach (var entity in ecsWorld.Entities)
            {
                foreach (var component in entity.Components)
                {
                    if (component.Value is BaseComponent baseComponent && baseComponent.Entity != entity)
                    {
                        result.AddWarning($"Entity {entity.Id} has component {component.GetType().Name} with incorrect owner reference");
                    }
                }
            }

            // Check for destroyed entities still in the world
            var destroyedEntities = ecsWorld.Entities.Where(e => e.IsDestroyed).ToList();
            foreach (var entity in destroyedEntities)
            {
                result.AddWarning($"Destroyed entity {entity.Id} still exists in world");
            }

            // Performance warnings
            if (ecsWorld.EntityCount > 10000)
            {
                result.AddWarning($"High entity count: {ecsWorld.EntityCount} (may impact performance)");
            }

            var avgComponentsPerEntity = ecsWorld.Entities.Sum(e => e.Components.Count) / (float)ecsWorld.EntityCount;
            if (avgComponentsPerEntity > 10)
            {
                result.AddWarning($"High average components per entity: {avgComponentsPerEntity:F1} (may impact performance)");
            }

            if (result.IsValid)
            {
                result.AddInfo($"ECSWorld validation passed: {ecsWorld.EntityCount} entities, {GetComponentStatistics(ecsWorld).Count} component types");
            }

            return result;
        }

        /// <summary>
        /// Creates a performance report for the ECS world.
        /// </summary>
        /// <param name="ecsWorld">The ECS world to analyze.</param>
        /// <returns>Performance report with metrics.</returns>
        public static string GetPerformanceReport(ECSWorld ecsWorld)
        {
            if (ecsWorld == null)
                return "ECSWorld is null";

            var report = new StringBuilder();
            report.AppendLine("=== ECS Performance Report ===");

            var entityCount = ecsWorld.EntityCount;
            report.AppendLine($"Entity Count: {entityCount}");

            var componentStats = GetComponentStatistics(ecsWorld);
            report.AppendLine($"Component Types: {componentStats.Count}");
            report.AppendLine($"Total Components: {componentStats.Values.Sum()}");

            // Calculate average components per entity
            var avgComponents = entityCount > 0 ? (float)componentStats.Values.Sum() / entityCount : 0;
            report.AppendLine($"Avg Components/Entity: {avgComponents:F2}");

            // Memory estimation (rough)
            var estimatedMemory = EstimateMemoryUsage(ecsWorld);
            report.AppendLine($"Estimated Memory Usage: {estimatedMemory / 1024.0:F1} KB");

            // Component distribution
            report.AppendLine();
            report.AppendLine("Component Distribution:");
            foreach (var stat in componentStats.OrderByDescending(x => x.Value))
            {
                var percentage = entityCount > 0 ? (stat.Value * 100.0 / entityCount) : 0;
                report.AppendLine($"  {stat.Key}: {stat.Value} ({percentage:F1}%)");
            }

            return report.ToString();
        }

        /// <summary>
        /// Estimates memory usage of the ECS world (rough calculation).
        /// </summary>
        /// <param name="ecsWorld">The ECS world to analyze.</param>
        /// <returns>Estimated memory usage in bytes.</returns>
        private static long EstimateMemoryUsage(ECSWorld ecsWorld)
        {
            const int entityOverhead = 64; // Rough estimate per entity
            const int componentOverhead = 32; // Rough estimate per component

            long total = 0;

            // Entity overhead
            total += ecsWorld.EntityCount * entityOverhead;

            // Component overhead
            foreach (var entity in ecsWorld.Entities)
            {
                // Count components for this entity by checking common component types
                int componentCount = 0;
                if (entity.TryGetComponent<SASZombieAssaultTD.Engine.Components.TransformComponent>(out var transformComp)) componentCount++;
                if (entity.TryGetComponent<SASZombieAssaultTD.Engine.Components.HealthComponent>(out var healthComp)) componentCount++;
                if (entity.TryGetComponent<SASZombieAssaultTD.Engine.Components.DamageComponent>(out var damageComp)) componentCount++;
                if (entity.TryGetComponent<SASZombieAssaultTD.Engine.Components.EnemyTypeComponent>(out var enemyTypeComp)) componentCount++;
                if (entity.TryGetComponent<SASZombieAssaultTD.Engine.Components.ActiveComponent>(out var activeComp)) componentCount++;
                if (entity.GetComponent<SpriteComponent>() != null) componentCount++;
                if (entity.GetComponent<AnimationControllerComponent>() != null) componentCount++;

                total += componentCount * componentOverhead;
            }

            return total;
        }
    }

    /// <summary>
    /// Result of ECS world validation.
    /// </summary>
    public class ECSValidationResult
    {
        private readonly List<string> _errors = new List<string>();
        private readonly List<string> _warnings = new List<string>();
        private readonly List<string> _info = new List<string>();

        public bool IsValid => _errors.Count == 0;
        public IReadOnlyList<string> Errors => _errors.AsReadOnly();
        public IReadOnlyList<string> Warnings => _warnings.AsReadOnly();
        public IReadOnlyList<string> Info => _info.AsReadOnly();

        public void AddError(string message)
        {
            _errors.Add(message);
        }

        public void AddWarning(string message)
        {
            _warnings.Add(message);
        }

        public void AddInfo(string message)
        {
            _info.Add(message);
        }

        public string GetFullReport()
        {
            var report = new StringBuilder();
            report.AppendLine("=== ECS Validation Report ===");

            if (IsValid)
            {
                report.AppendLine("Status: PASSED");
            }
            else
            {
                report.AppendLine("Status: FAILED");
            }

            if (_errors.Count > 0)
            {
                report.AppendLine();
                report.AppendLine("Errors:");
                foreach (var error in _errors)
                {
                    report.AppendLine($"  - {error}");
                }
            }

            if (_warnings.Count > 0)
            {
                report.AppendLine();
                report.AppendLine("Warnings:");
                foreach (var warning in _warnings)
                {
                    report.AppendLine($"  - {warning}");
                }
            }

            if (_info.Count > 0)
            {
                report.AppendLine();
                report.AppendLine("Information:");
                foreach (var info in _info)
                {
                    report.AppendLine($"  - {info}");
                }
            }

            return report.ToString();
        }
    }
}




