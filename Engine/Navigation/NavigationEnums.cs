// =====================================================================================================
//  FILE: NavigationEnums.cs
//  PATH: Engine/Navigation/NavigationEnums.cs
//  SUBSYSTEM: Navigation
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

namespace SASZombieAssaultTD.Engine.Navigation
{
    public class NavigationEnums
    {
        public enum NavigationFlags
        {
            None = 0,
            Walkable = 1 << 0,
            Blocked = 1 << 1,
            Water = 1 << 2,
            Lava = 1 << 3,
            Spikes = 1 << 4,
            Slow = 1 << 5,
            Fast = 1 << 6,
            Teleport = 1 << 7,
            SpawnPoint = 1 << 8,
            Objective = 1 << 9,
            Hazard = 1 << 10,
            Cover = 1 << 11,
            Elevated = 1 << 12,
            Underground = 1 << 13,
            Indoor = 1 << 14,
            Outdoor = 1 << 15
        }
    }
}
