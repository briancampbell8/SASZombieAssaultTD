// =====================================================================================================
//  FILE: WaveRuntimeSummary.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/RuntimeSummaries/WaveRuntimeSummary.cs
//  SUBSYSTEM: GameRoot GameSave Subsystems RuntimeSummaries
//
//  ROLE:
//      Provides a deterministic, immutable final summary of wave-related runtime values as
//      produced by WaveSubsystem at shutdown. This class contains no lifecycle logic and is not
//      executed directly by the engine. It serves strictly as a final report for analytics,
//      debugging, UI display, or post-run inspection.
//
//  RESPONSIBILITIES:
//      - Represent the final per-wave runtime values in a deterministic summary structure.
//      - Provide a stable, single-class summary model for higher-level systems.
//      - Serve as a clean, engine-facing summary object for post-run analysis or reporting.
//
//  NON-RESPONSIBILITIES:
//      - Implementing lifecycle sequencing (Initialize → Tick → Shutdown).
//      - Executing frame-level update logic, spawning, or wave progression logic.
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
    public sealed class WaveRuntimeSummary
    {
        public int CurrentWave { get; }
        public int EnemiesRemaining { get; }
        public int EnemiesSpawned { get; }
        public int WaveStartFrame { get; }
        public int FinalFrame { get; }

        public WaveRuntimeSummary(
            int currentWave,
            int enemiesRemaining,
            int enemiesSpawned,
            int waveStartFrame,
            int finalFrame)
        {
            CurrentWave = currentWave;
            EnemiesRemaining = enemiesRemaining;
            EnemiesSpawned = enemiesSpawned;
            WaveStartFrame = waveStartFrame;
            FinalFrame = finalFrame;
        }
    }
}
