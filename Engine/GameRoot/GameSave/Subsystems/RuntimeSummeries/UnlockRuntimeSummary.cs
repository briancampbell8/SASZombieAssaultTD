// =====================================================================================================
//  FILE: UnlockRuntimeSummary.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/RuntimeSummaries/UnlockRuntimeSummary.cs
//  SUBSYSTEM: GameRoot GameSave Subsystems RuntimeSummaries
//
//  ROLE:
//      Provides a deterministic, immutable final summary of unlockable game content as produced
//      by UnlockSubsystem at shutdown. This class contains no lifecycle logic and is not executed
//      directly by the engine. It serves strictly as a final report for analytics, debugging,
//      UI display, or post-run inspection.
//
//  RESPONSIBILITIES:
//      - Represent the final unlock-state values in a deterministic summary structure.
//      - Provide a stable, single-class summary model for higher-level systems.
//      - Serve as a clean, engine-facing summary object for post-run analysis or reporting.
//
//  NON-RESPONSIBILITIES:
//      - Implementing lifecycle sequencing (Initialize → Tick → Shutdown).
//      - Executing frame-level update logic or unlock triggers.
//      - Managing subsystem behavior or engine-hosted program responsibilities.
//      - Handling save/load serialization or persistent state responsibilities.
//
//  ARCHITECTURAL NOTES:
//      - RuntimeSummaries are produced by subsystems and moved into this folder manually.
//      - They are not subsystems and do not participate in GameRootMain lifecycle execution.
//      - They exist solely as deterministic shutdown-time summaries.
//      - All RuntimeSummary classes MUST remain single-class files without nested types.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems.RuntimeSummaries
{
    public sealed class UnlockRuntimeSummary
    {
        public IReadOnlyList<string> UnlockedItems { get; }
        public int FinalFrame { get; }

        public UnlockRuntimeSummary(
            List<string> unlockedItems,
            int finalFrame)
        {
            UnlockedItems = unlockedItems.AsReadOnly();
            FinalFrame = finalFrame;
        }
    }
}
