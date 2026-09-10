// =====================================================================================================
//  FILE: TowerRuntimeSummary.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/RuntimeSummaries/TowerRuntimeSummary.cs
//  SUBSYSTEM: GameRoot GameSave Subsystems RuntimeSummaries
//
//  ROLE:
//      Provides a deterministic, immutable final summary of tower-related runtime values as
//      produced by TowerSubsystem at shutdown. This class contains no lifecycle logic and is
//      not executed directly by the engine. It serves strictly as a final report for analytics,
//      debugging, UI display, or post-run inspection.
//
//  RESPONSIBILITIES:
//      - Represent the final per-tower runtime values in a deterministic summary structure.
//      - Provide a stable, single-class summary model for higher-level systems.
//      - Serve as a clean, engine-facing summary object for post-run analysis or reporting.
//
//  NON-RESPONSIBILITIES:
//      - Implementing lifecycle sequencing (Initialize → Tick → Shutdown).
//      - Executing frame-level update logic, combat behavior, or upgrade logic.
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
    public sealed class TowerRuntimeSummary
    {
        public int TowerId { get; }
        public string TowerType { get; }
        public int Level { get; }
        public int Damage { get; }
        public float Range { get; }
        public int ShotsFired { get; }
        public int FinalFrame { get; }

        public TowerRuntimeSummary(
            int towerId,
            string towerType,
            int level,
            int damage,
            float range,
            int shotsFired,
            int finalFrame)
        {
            TowerId = towerId;
            TowerType = towerType;
            Level = level;
            Damage = damage;
            Range = range;
            ShotsFired = shotsFired;
            FinalFrame = finalFrame;
        }
    }
}
