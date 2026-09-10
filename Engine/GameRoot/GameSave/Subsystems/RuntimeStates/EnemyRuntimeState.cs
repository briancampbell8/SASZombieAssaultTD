// =====================================================================================================
//  FILE: EnemyRuntimeState.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/RuntimeStates/EnemyRuntimeState.cs
//  SUBSYSTEM: GameRoot GameSave Subsystems RuntimeStates
//
//  ROLE:
//      Provides a deterministic, immutable snapshot of an enemy’s runtime state as produced by
//      EnemySubsystem during active gameplay. This class contains no lifecycle logic and is not
//      executed directly by the engine. It serves strictly as a data container for shutdown-time
//      state extraction, debugging, analytics, or post-run inspection.
//
//  RESPONSIBILITIES:
//      - Represent the final per-enemy runtime values in a deterministic structure.
//      - Maintain strict immutability rules except for controlled subsystem mutation.
//      - Serve as a clean, engine-facing state object for post-run analysis or reporting.
//      - Provide a stable, single-class runtime state model for future subsystem expansions.
//
//  NON-RESPONSIBILITIES:
//      - Implementing lifecycle sequencing (Initialize → Tick → Shutdown).
//      - Executing frame-level update logic, AI, movement, or damage calculations.
//      - Managing subsystem orchestration or engine-hosted program behavior.
//      - Handling save/load serialization or persistent state responsibilities.
//
//  ARCHITECTURAL NOTES:
//      - RuntimeStates are produced by subsystems and moved into this folder manually.
//      - They are not subsystems and do not participate in GameRootMain lifecycle execution.
//      - They exist solely as deterministic runtime snapshots for shutdown-time extraction.
//      - All RuntimeState classes MUST remain single-class files without nested types.
// =====================================================================================================


namespace SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems.RuntimeSummeries
{
    internal sealed class EnemyRuntimeState
    {
        public int Id { get; }
        public string Type { get; }
        public int MaxHealth { get; }
        public int Health { get; set; }
        public bool IsDead { get; set; }
        public int CurrentFrame { get; set; }

        public EnemyRuntimeState(int id, string type, int maxHealth)
        {
            Id = id;
            Type = type;
            MaxHealth = maxHealth;
            Health = maxHealth;
            IsDead = false;
        }
    }
}
