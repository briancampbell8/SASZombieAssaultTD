// =====================================================================================================
//  FILE: NavMigrationAnalyzer.cs
//  PATH: Engine/Navigation/Migration/NavMigrationAnalyzer.cs
//  SUBSYSTEM: Navigation – Migration
//
//  ROLE:
//      Provides migration analysis utilities for the Navigation Migration subsystem.
//      Computes migration progress and produces recommendations based on completion percentage.
//
//  RESPONSIBILITIES:
//      - Count total entities.
//      - Count migrated entities (entities with NavAgentComponent).
//      - Compute migration percentage.
//      - Generate recommendations based on migration completeness.
//      - Produce a MigrationAnalysisResult structure.
//
//  NON-RESPONSIBILITIES:
//      - Performing migration (handled by NavMigrationSingle / NavMigrationBatch).
//      - Validating migration correctness (handled by NavMigrationValidator).
//      - Creating NavAgent entities (handled by NavAgentFactory).
//      - Configuring NavAgents (handled by NavAgentConfigurator).
//
//  ARCHITECTURAL NOTES:
//      - Stateless analysis module extracted directly from NavigationMigrationHelper.cs.
//      - Uses ECSRuntimeCore for ECSEntityCore enumeration.
// =====================================================================================================

using System.Linq;
using SASZombieAssaultTD.Engine.ECS;

namespace SASZombieAssaultTD.Engine.Navigation.Migration
{
    internal static class NavMigrationAnalyzer
    {
        private static readonly ECSComponents _components;
        private static readonly ECSEntityCore _entity;
        private static readonly ECSRuntimeEntities _entities;
        private static readonly ECSRuntimeCore _runtime;
        public static MigrationAnalysisResult AnalyzeMigration(ECSRuntimeCore eCSRuntime)
        {
            var entities = _runtime.Entities.All;

            var total = _components.Count;
            var migrated =
                entities.Count(ECSEntityCore => eCSRuntime.HasComponent<NavAgentComponent>(ECSEntityCore));

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
}
