// =====================================================================================================
//  FILE: EconomyRuntimeState.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/RuntimeStates/EconomyRuntimeState.cs
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
    public sealed class EconomyRuntimeState
    {
        public int CurrentCash { get; }
        public int TotalEarned { get; }
        public int TotalSpent { get; }

        public float CashRewardMultiplier { get; }
        public float TowerCostMultiplier { get; }
        public float UpgradeCostMultiplier { get; }

        public EconomyRuntimeState(int currentCash,
                                   int totalEarned,
                                   int totalSpent,
                                   float cashRewardMultiplier,
                                   float towerCostMultiplier,
                                   float upgradeCostMultiplier)
        {
            CurrentCash = currentCash;
            TotalEarned = totalEarned;
            TotalSpent = totalSpent;

            CashRewardMultiplier = cashRewardMultiplier;
            TowerCostMultiplier = towerCostMultiplier;
            UpgradeCostMultiplier = upgradeCostMultiplier;
        }
    }
}
