// =====================================================================================================
//  FILE: AchievementRuntimeState.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/AchievementRuntimeState.cs
//  SUBSYSTEM: GameRoot GameSave Subsystems
//
//  ROLE:
//      Defines the minimal deterministic lifecycle contract for any engine-hosted program.
//      This interface is implemented by engine hosts (e.g., GameRootMain) to provide a clean,
//      engine-facing API for startup, execution entry, and deterministic shutdown operations.
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface for program orchestration.
//      - Enforce the structural sequencing contract: Initialize → Run Loop Execution → Shutdown.
//      - Serve as the base contract for any future top-level engine-hosted program modules.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deep frame-level update calculation rules or rendering commands directly.
//      - Managing active systems registration pools, engine assets, or game states.
//      - Handling discrete hardware device allocation boundaries.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces the legacy GameRoot partial lifecycle methods.
//      - GameRootMain implements this interface and delegates to its subsystems:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================
namespace SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems
{
    public sealed class AchievementRuntimeState
    {
        // Internal runtime-only achievement model.

        public string Id { get; }
        public int Progress { get; set; }
        public int Required { get; set; } = 100;
        public bool IsUnlocked { get; set; }
        public int UnlockFrame { get; set; }
        public int CurrentFrame { get; set; }

        public AchievementRuntimeState(string id) => Id = id;



    }
}
