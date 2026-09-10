// =====================================================================================================
//  FILE: ValidationDebug.cs
//  PATH: Engine/ECS/ECSDebug/ValidationDebug.cs
//  SUBSYSTEM: ECS ECSDebug
//
//  ROLE:
//      Provides deterministic ECS world‑validation utilities, including structural integrity checks,
//      duplicate‑ID detection, component‑ownership verification, destroyed‑ECSEntityCore detection, and
//      structured reporting of errors, warnings, and informational diagnostics.
//
//  RESPONSIBILITIES:
//      - Provide ValidateWorld() behavior for the ECSDebug subsystem.
//      - Provide ECSValidationResult reporting behavior for the ECSDebug subsystem.
//      - Integrate deterministic diagnostic tracing using DLogger.Log.
//      - Maintain strict read‑only validation semantics.
//
//  NON-RESPONSIBILITIES:
//      - Performing ECSEntityCore lifecycle operations.
//      - Mutating component collections or world state.
//      - Executing system update logic or rendering.
//      - Performing spatial or collision‑related queries.
//
//  ARCHITECTURAL NOTES:
//      - ValidationDebug is a dedicated subsystem file extracted from the original ECSDebugInspector.
//      - Suffix naming rule enforced: “Debug” is a suffix, not a prefix.
//      - All diagnostic output uses DLogger.Log for deterministic engine tracing.
// =====================================================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.ECS.ECSDebug
{
    internal static class ValidationDebug
    {
        /// <summary>
        /// Performs deterministic validation of the ECS world and returns a structured result.
        /// </summary>
        public static ECSValidationResult ValidateWorld(ECSRuntimeCore ecsWorld)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECS.ValidationDebug", 1, "Validate",
                "Beginning ECS world validation.");

            var result = new ECSValidationResult();

            if (ecsWorld == null)
            {
                result.AddError("ECSRuntimeCore is null");
                return result;
            }

            // Duplicate ECSEntityCore ID detection
            var ids = ecsWorld.Entities.All.Select(e => e.Id).ToList();
            var duplicates = ids.GroupBy(id => id)
                                .Where(g => g.Count() > 1)
                                .Select(g => g.Key);

            foreach (var id in duplicates)
                result.AddError($"Duplicate ECSEntityCore ID found: {id}");

            // Component ownership validation
            foreach (var entity in ecsWorld.Entities.All)
            {
                foreach (var comp in entity.ComponentsList)
                {
                    if (comp is ECSComponents baseComp &&
                        baseComp.Parent != entity)
                    {
                        result.AddWarning(
                            $"Entity {entity.Id} has component {comp.GetType().Name} with incorrect owner reference");
                    }
                }
            }

            // Destroyed ECSEntityCore detection
            foreach (var entity in ecsWorld.Entities.All.Where(e => e.Destroyed))
                result.AddWarning($"Destroyed ECSEntityCore {entity.Id} still exists in world");

            // Performance heuristics
            if (ecsWorld.EntityCount > 10000)
                result.AddWarning($"High ECSEntityCore count: {ecsWorld.EntityCount}");

            var avgComponents = ecsWorld.Entities.All.Sum(e => e.ComponentCount) /
                                (float)System.Math.Max(ecsWorld.EntityCount, 1);

            if (avgComponents > 10)
                result.AddWarning($"High average components per ECSEntityCore: {avgComponents:F1}");

            // Success info
            if (result.IsValid)
            {
                int componentTypes = ecsWorld.Entities.All
                    .SelectMany(e => e.ComponentsList)
                    .Select(c => c.GetType().Name)
                    .Distinct()
                    .Count();

                result.AddInfo(
                    $"ECSRuntimeCore validation passed: {ecsWorld.EntityCount} entities, {componentTypes} component types");
            }

            return result;
        }
    }

    /// <summary>
    /// Structured validation result containing errors, warnings, and informational messages.
    /// </summary>
    internal sealed class ECSValidationResult
    {
        private readonly List<string> _errors = new();
        private readonly List<string> _warnings = new();
        private readonly List<string> _info = new();

        public bool IsValid => _errors.Count == 0;

        public IReadOnlyList<string> Errors => _errors.AsReadOnly();
        public IReadOnlyList<string> Warnings => _warnings.AsReadOnly();
        public IReadOnlyList<string> Info => _info.AsReadOnly();

        public void AddError(string message)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECS.ValidationDebug", 2, "Error", message);
            _errors.Add(message);
        }

        public void AddWarning(string message)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECS.ValidationDebug", 3, "Warning", message);
            _warnings.Add(message);
        }

        public void AddInfo(string message)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECS.ValidationDebug", 4, "Info", message);
            _info.Add(message);
        }

        /// <summary>
        /// Returns a formatted validation report.
        /// </summary>
        public string GetFullReport()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECS.ValidationDebug", 5, "Report",
                "Generating ECS validation report.");

            var report = new StringBuilder();
            report.AppendLine("=== ECS Validation Report ===");
            report.AppendLine(IsValid ? "Status: PASSED" : "Status: FAILED");

            if (_errors.Count > 0)
            {
                report.AppendLine();
                report.AppendLine("Errors:");
                foreach (var e in _errors)
                    report.AppendLine($"  - {e}");
            }

            if (_warnings.Count > 0)
            {
                report.AppendLine();
                report.AppendLine("Warnings:");
                foreach (var w in _warnings)
                    report.AppendLine($"  - {w}");
            }

            if (_info.Count > 0)
            {
                report.AppendLine();
                report.AppendLine("Information:");
                foreach (var i in _info)
                    report.AppendLine($"  - {i}");
            }

            return report.ToString();
        }
    }
}
