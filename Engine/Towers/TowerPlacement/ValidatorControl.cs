// =====================================================================================================
//  FILE: ValidatorControl.cs
//  PATH: Engine/Towers/TowerPlacement/ValidatorControl.cs
//  SUBSYSTEM: Towers TowerPlacement
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

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Towers.TowerPlacement
{
    public sealed class ValidatorControl
    {
        private readonly State _state;
        public readonly PlacementValidator _validator;

        public ValidatorControl(State state)
        {
            _state = state;
            _validator = new PlacementValidator();
        }

        public bool CanPlaceTower(Vector3Int gridPos, TowerData data)
        {
            try
            {
                return _validator.CanPlaceTower(gridPos, data);
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, $"Error validating placement: {ex.Message}");
                return false;
            }
        }

        public void Cleanup()
        {
            try
            {
                _validator.Cleanup();
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, $"Error cleaning validator: {ex.Message}");
            }
        }
    }
}
