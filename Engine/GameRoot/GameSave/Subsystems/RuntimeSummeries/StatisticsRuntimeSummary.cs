// =====================================================================================================
//  FILE: StatisticsRuntimeSummary.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/RuntimeSummaries/StatisticsRuntimeSummary.cs
//  SUBSYSTEM: GameRoot GameSave Subsystems RuntimeSummaries
//
//  ROLE:
//      Provides a deterministic, immutable final summary of aggregated gameplay statistics as
//      produced by StatisticsSubsystem at shutdown. This class contains no lifecycle logic and
//      is not executed directly by the engine. It serves strictly as a final report for
//      analytics, debugging, UI display, or post-run inspection.
//
//  RESPONSIBILITIES:
//      - Represent the final aggregated gameplay statistics in a deterministic summary structure.
//      - Provide a stable, single-class summary model for higher-level systems.
//      - Serve as a clean, engine-facing summary object for post-run analysis or reporting.
//
//  NON-RESPONSIBILITIES:
//      - Implementing lifecycle sequencing (Initialize → Tick → Shutdown).
//      - Executing frame-level update logic or cross-system orchestration.
//      - Managing subsystem behavior or engine-hosted program responsibilities.
//      - Handling save/load serialization or persistent state responsibilities.
//
//  ARCHITECTURAL NOTES:
//      - RuntimeSummaries are produced by subsystems and moved into this folder manually.
//      - They are not subsystems and do not participate in GameRootMain lifecycle execution.
//      - They exist solely as deterministic shutdown-time summaries.
//      - All RuntimeSummary classes MUST remain single-class files without nested types.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems.RuntimeSummaries
{
    public sealed class StatisticsRuntimeSummary
    {
        public int TotalEnemiesKilled { get; }
        public int TotalDamageDealt { get; }
        public int TotalDamageTaken { get; }
        public int TotalShotsFired { get; }
        public int FinalFrame { get; }

        public StatisticsRuntimeSummary(
            int totalEnemiesKilled,
            int totalDamageDealt,
            int totalDamageTaken,
            int totalShotsFired,
            int finalFrame)
        {
            TotalEnemiesKilled = totalEnemiesKilled;
            TotalDamageDealt = totalDamageDealt;
            TotalDamageTaken = totalDamageTaken;
            TotalShotsFired = totalShotsFired;
            FinalFrame = finalFrame;
        }
    }
}
