// =====================================================================================================
//  FILE: EconomySubsystem.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/EconomySubsystem.cs
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
    public partial class EconomySubsystem
    {
        // Runtime-only shadow state of the economy.
        private int _currentCash;
        private int _totalEarned;
        private int _totalSpent;

        private float _cashRewardMultiplier = 1.0f;
        private float _towerCostMultiplier = 1.0f;
        private float _upgradeCostMultiplier = 1.0f;

        // Called during GameRootMain.Initialize()
        public void Initialize(int startingCash,
                               float cashRewardMultiplier,
                               float towerCostMultiplier,
                               float upgradeCostMultiplier)
        {
            _currentCash = startingCash;
            _totalEarned = 0;
            _totalSpent = 0;

            _cashRewardMultiplier = cashRewardMultiplier;
            _towerCostMultiplier = towerCostMultiplier;
            _upgradeCostMultiplier = upgradeCostMultiplier;
        }

        // Called during GameRootMain.Update()
        public void Tick()
        {
            // EconomySubsystem has no per-frame behavior.
            // Deterministic placeholder for future expansion.
        }

        // Runtime economy operations
        public void AddCash(int amount)
        {
            _currentCash += amount;
            _totalEarned += amount;
        }

        public bool TrySpendCash(int amount)
        {
            if (_currentCash < amount)
                return false;

            _currentCash -= amount;
            _totalSpent += amount;
            return true;
        }

        public int GetFinalReward(int baseReward)
        {
            return (int)(baseReward * _cashRewardMultiplier);
        }

        public int GetFinalTowerCost(int baseCost)
        {
            return (int)(baseCost * _towerCostMultiplier);
        }

        public int GetFinalUpgradeCost(int baseCost)
        {
            return (int)(baseCost * _upgradeCostMultiplier);
        }

        // Called during GameRootMain.Shutdown()
        public EconomyRuntimeState ExportRuntimeState()
        {
            return new EconomyRuntimeState(
                _currentCash,
                _totalEarned,
                _totalSpent,
                _cashRewardMultiplier,
                _towerCostMultiplier,
                _upgradeCostMultiplier
            );
        }
    }
}
