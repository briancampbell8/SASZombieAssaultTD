// =====================================================================================================
//  FILE: WorldDebug.cs
//  PATH: Engine/ECS/ECSDebug/WorldDebug.cs
//  SUBSYSTEM: ECS ECSDebug
//
//  ROLE:
//      Provides deterministic world‑level debug, inspection, and performance reporting utilities
//      for the ECS subsystem. Supplies safe, predictable introspection of world state, ECSEntityCore
//      distribution, component statistics, and memory‑usage estimation.
//
//  RESPONSIBILITIES:
//      - Provide GetWorldDebugInfo() behavior for the ECSDebug subsystem.
//      - Provide LogWorldState() behavior for the ECSDebug subsystem.
//      - Provide GetPerformanceReport() behavior for the ECSDebug subsystem.
//      - Provide EstimateMemoryUsage() behavior for the ECSDebug subsystem.
//      - Integrate deterministic diagnostic tracing using DLogger.Log.
//      - Maintain strict read‑only inspection semantics.
//
//  NON-RESPONSIBILITIES:
//      - Performing ECSEntityCore lifecycle operations.
//      - Mutating component collections or world state.
//      - Executing system update logic or rendering.
//      - Performing spatial or collision‑related queries.
//
//  ARCHITECTURAL NOTES:
//      - WorldDebug is a dedicated subsystem file extracted from the original ECSDebugInspector.
//      - Suffix naming rule enforced: “Debug” is a suffix, not a prefix.
//      - All diagnostic output uses DLogger.Log for deterministic engine tracing.
// =====================================================================================================

using System.Linq;
using System.Text;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.ECS.ECSDebug
{
    internal static class WorldDebug
    {
        public static string GetWorldDebugInfo(ECSRuntimeCore ecsWorld)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECS.WorldDebug", 1, "WorldInfo",
                "Generating ECS world debug information.");

            if (ecsWorld == null)
                return "ECSRuntimeCore is null";

            var info = new StringBuilder();
            info.AppendLine("=== ECS World Debug Information ===");
            info.AppendLine($"Total Entities: {ecsWorld.EntityCount}");
            info.AppendLine();

            // Component statistics (correct modern ECS API)
            var componentStats = ecsWorld.Entities.All
                .SelectMany(e => ecsWorld.Components.GetAllComponents<object>(e.Id))
                .GroupBy(c => c.GetType().Name)
                .ToDictionary(g => g.Key, g => g.Count());

            info.AppendLine("Component Statistics:");
            foreach (var stat in componentStats.OrderByDescending(x => x.Value))
                info.AppendLine($"  {stat.Key}: {stat.Value} entities");

            info.AppendLine();

            // Entity details (first 10)
            info.AppendLine("Entity Details (first 10):");
            var entities = ecsWorld.Entities.All.Take(10).ToList();

            for (int i = 0; i < entities.Count; i++)
                info.AppendLine($"  [{i + 1}] Entity.{entities[i].Id}");

            if (ecsWorld.EntityCount > 10)
                info.AppendLine($"  ... and {ecsWorld.EntityCount - 10} more entities");

            return info.ToString();
        }

        public static void LogWorldState(ECSRuntimeCore ecsWorld, string logLevel = "INFO")
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECS.WorldDebug", 2, "LogWorld",
                "Logging ECS world state.");

            var debugInfo = GetWorldDebugInfo(ecsWorld);

            // Fix ambiguous overload
            DLogger.Log(LogSubsystems.ECSECSDebug,
                $"{logLevel}", debugInfo);
        }

        public static string GetPerformanceReport(ECSRuntimeCore ecsWorld)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECS.WorldDebug", 3, "PerfReport",
                "Generating ECS performance report.");

            if (ecsWorld == null)
                return "ECSRuntimeCore is null";

            var report = new StringBuilder();
            report.AppendLine("=== ECS Performance Report ===");

            int entityCount = ecsWorld.EntityCount;
            report.AppendLine($"Entity Count: {entityCount}");

            var componentStats = ecsWorld.Entities.All
                .SelectMany(e => ecsWorld.Components.GetAllComponents<object>(e.Id))
                .GroupBy(c => c.GetType().Name)
                .ToDictionary(g => g.Key, g => g.Count());

            report.AppendLine($"Component Types: {componentStats.Count}");
            report.AppendLine($"Total Components: {componentStats.Values.Sum()}");

            float avgComponents = entityCount > 0
                ? (float)componentStats.Values.Sum() / entityCount
                : 0;

            report.AppendLine($"Avg Components/Entity: {avgComponents:F2}");

            long estimatedMemory = EstimateMemoryUsage(ecsWorld);
            report.AppendLine($"Estimated Memory Usage: {estimatedMemory / 1024.0:F1} KB");

            report.AppendLine();
            report.AppendLine("Component Distribution:");

            foreach (var stat in componentStats.OrderByDescending(x => x.Value))
            {
                double pct = entityCount > 0
                    ? (stat.Value * 100.0 / entityCount)
                    : 0;

                report.AppendLine($"  {stat.Key}: {stat.Value} ({pct:F1}%)");
            }

            return report.ToString();
        }

        private static long EstimateMemoryUsage(ECSRuntimeCore ecsWorld)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECS.WorldDebug", 4, "MemoryEstimate",
                "Estimating ECS memory usage.");

            const int entityOverhead = 64;
            const int componentOverhead = 32;

            long total = ecsWorld.EntityCount * entityOverhead;

            foreach (var entity in ecsWorld.Entities.All)
            {
                int count = ecsWorld.Components.GetAllComponents<object>(entity.Id).Count();
                total += count * componentOverhead;
            }

            return total;
        }
    }
}
