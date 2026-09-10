// =====================================================================================================
//  FILE: PlayerRuntimeState.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/RuntimeStates/PlayerRuntimeState.cs
//  SUBSYSTEM: GameRoot GameSave Subsystems RuntimeStates
//
//  ROLE:
//      Provides a deterministic, immutable snapshot of the player’s runtime state as produced by
//      PlayerSubsystem during active gameplay. This class contains no lifecycle logic and is not
//      executed directly by the engine. It serves strictly as a data container for shutdown-time
//      state extraction, debugging, analytics, or post-run inspection.
//
//  RESPONSIBILITIES:
//      - Represent the final per-player runtime values in a deterministic structure.
//      - Maintain strict immutability rules except for controlled subsystem mutation.
//      - Serve as a clean, engine-facing state object for post-run analysis or reporting.
//      - Provide a stable, single-class runtime state model for future subsystem expansions.
//
//  NON-RESPONSIBILITIES:
//      - Implementing lifecycle sequencing (Initialize → Tick → Shutdown).
//      - Executing frame-level update logic, movement, or input processing.
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
    public sealed class PlayerRuntimeState
    {
        // Immutable idECSEntityCore fields
        public int PlayerId { get; }
        public string Name { get; }

        // Immutable configuration fields
        public int MaxHealth { get; }

        // Controlled mutable runtime fields
        public int Health { get; set; }
        public int Score { get; set; }
        public int CurrentFrame { get; set; }

        public PlayerRuntimeState(int playerId, string name, int maxHealth)
        {
            PlayerId = playerId;
            Name = name;
            MaxHealth = maxHealth;

            // Defaults — subsystem will overwrite these
            Health = maxHealth;
            Score = 0;
            CurrentFrame = 0;
        }
    }
}
