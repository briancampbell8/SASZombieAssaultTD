// =====================================================================================================
//  FILE: TowerRuntimeState.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/RuntimeStates/TowerRuntimeState.cs
//  SUBSYSTEM: GameRoot GameSave Subsystems RuntimeStates
//
//  ROLE:
//      Provides a deterministic, immutable snapshot of tower-related runtime values as produced
//      by TowerSubsystem during active gameplay. This class contains no lifecycle logic and is
//      not executed directly by the engine. It serves strictly as a data container for shutdown-
//      time extraction, debugging, analytics, or post-run inspection.
//
//  RESPONSIBILITIES:
//      - Represent the final per-tower runtime values in a deterministic structure.
//      - Maintain strict immutability rules except for controlled subsystem mutation.
//      - Serve as a clean, engine-facing state object for post-run analysis or reporting.
//      - Provide a stable, single-class runtime state model for future subsystem expansions.
//
//  NON-RESPONSIBILITIES:
//      - Implementing lifecycle sequencing (Initialize → Tick → Shutdown).
//      - Executing frame-level update logic, combat behavior, or upgrade logic.
//      - Managing subsystem orchestration or engine-hosted program responsibilities.
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
    public sealed class TowerRuntimeState
    {
        // Immutable idECSEntityCore/configuration fields
        public int TowerId { get; }
        public string TowerType { get; }
        public int Level { get; }
        public int Damage { get; }
        public float Range { get; }

        // Controlled mutable runtime fields
        public int ShotsFired { get; set; }
        public int CurrentFrame { get; set; }

        public TowerRuntimeState(
            int towerId,
            string towerType,
            int level,
            int damage,
            float range,
            int shotsFired,
            int currentFrame)
        {
            TowerId = towerId;
            TowerType = towerType;
            Level = level;
            Damage = damage;
            Range = range;
            ShotsFired = shotsFired;
            CurrentFrame = currentFrame;
        }
    }
}
