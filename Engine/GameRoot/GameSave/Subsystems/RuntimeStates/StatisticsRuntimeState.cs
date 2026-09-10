// =====================================================================================================
//  FILE: StatisticsRuntimeState.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/RuntimeStates/StatisticsRuntimeState.cs
//  SUBSYSTEM: GameRoot GameSave Subsystems RuntimeStates
//
//  ROLE:
//      Provides a deterministic, immutable snapshot of aggregated gameplay statistics as produced
//      by StatisticsSubsystem during active gameplay. This class contains no lifecycle logic and
//      is not executed directly by the engine. It serves strictly as a data container for
//      shutdown-time extraction, debugging, analytics, or post-run inspection.
//
//  RESPONSIBILITIES:
//      - Represent the final aggregated gameplay statistics in a deterministic structure.
//      - Maintain strict immutability rules except for controlled subsystem mutation.
//      - Serve as a clean, engine-facing state object for post-run analysis or reporting.
//      - Provide a stable, single-class runtime state model for future subsystem expansions.
//
//  NON-RESPONSIBILITIES:
//      - Implementing lifecycle sequencing (Initialize → Tick → Shutdown).
//      - Executing frame-level update logic or cross-system orchestration.
//      - Managing subsystem behavior or engine-hosted program responsibilities.
//      - Handling save/load serialization or persistent state responsibilities.
//
//  ARCHITECTURAL NOTES:
//      - RuntimeStates are produced by subsystems and moved into this folder manually.
//      - They are not subsystems and do not participate in GameRootMain lifecycle execution.
//      - They exist solely as deterministic runtime snapshots for shutdown-time extraction.
//      - All RuntimeState classes MUST remain single-class files without nested types.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems.RuntimeStates
{
    public sealed class StatisticsRuntimeState
    {
        // Immutable aggregated statistics
        public int TotalEnemiesKilled { get; }
        public int TotalDamageDealt { get; }
        public int TotalDamageTaken { get; }
        public int TotalShotsFired { get; }

        // Controlled mutable runtime field
        public int CurrentFrame { get; set; }

        public StatisticsRuntimeState(
            int totalEnemiesKilled,
            int totalDamageDealt,
            int totalDamageTaken,
            int totalShotsFired,
            int currentFrame)
        {
            TotalEnemiesKilled = totalEnemiesKilled;
            TotalDamageDealt = totalDamageDealt;
            TotalDamageTaken = totalDamageTaken;
            TotalShotsFired = totalShotsFired;
            CurrentFrame = currentFrame;
        }
    }
}
