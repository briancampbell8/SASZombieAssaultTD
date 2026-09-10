// =====================================================================================================
//  FILE: UnlockRuntimeState.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/RuntimeStates/UnlockRuntimeState.cs
//  SUBSYSTEM: GameRoot GameSave Subsystems RuntimeStates
//
//  ROLE:
//      Provides a deterministic, immutable snapshot of unlockable game content as produced by
//      UnlockSubsystem during active gameplay. This class contains no lifecycle logic and is
//      not executed directly by the engine. It serves strictly as a data container for
//      shutdown-time extraction, debugging, analytics, or post-run inspection.
//
//  RESPONSIBILITIES:
//      - Represent the final unlock-state values in a deterministic structure.
//      - Maintain strict immutability rules except for controlled subsystem mutation.
//      - Serve as a clean, engine-facing state object for post-run analysis or reporting.
//      - Provide a stable, single-class runtime state model for future subsystem expansions.
//
//  NON-RESPONSIBILITIES:
//      - Implementing lifecycle sequencing (Initialize → Tick → Shutdown).
//      - Executing frame-level update logic or unlock triggers.
//      - Managing subsystem orchestration or engine-hosted program responsibilities.
//      - Handling save/load serialization or persistent state responsibilities.
//
//  ARCHITECTURAL NOTES:
//      - RuntimeStates are produced by subsystems and moved into this folder manually.
//      - They are not subsystems and do not participate in GameRootMain lifecycle execution.
//      - They exist solely as deterministic runtime snapshots for shutdown-time extraction.
//      - All RuntimeState classes MUST remain single-class files without nested types.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems.RuntimeStates
{
    public sealed class UnlockRuntimeState
    {
        // Immutable list of unlocked items
        public IReadOnlyList<string> UnlockedItems { get; }

        // Controlled mutable runtime field
        public int CurrentFrame { get; set; }

        public UnlockRuntimeState(
            List<string> unlockedItems,
            int currentFrame)
        {
            UnlockedItems = unlockedItems.AsReadOnly();
            CurrentFrame = currentFrame;
        }
    }
}
