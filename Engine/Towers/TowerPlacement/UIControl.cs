// =====================================================================================================
//  FILE: UIControl.cs
//  PATH: Engine/Towers/TowerPlacement/UIControl.cs
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

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Towers.Placement;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Towers.TowerPlacement
{
    internal sealed class UIControl
    {
        private readonly State _state;
        private readonly HUDController _hud;

        public Vector3Int gridPos { get; private set; }

        public UIControl(State state)
        {
            _state = state;
            _hud = new HUDController();
        }

        public void ShowPlacementUI()
        {
            try
            {
                if (_state.TowerData != null)
                    _hud.ShowPlacementInfo(_state.TowerData);
            }
            catch (System.Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, $"Error showing placement UI: {ex.Message}");
            }
        }

        public void HidePlacementUI()
        {
            try
            {
                _hud.HidePlacementInfo();
            }
            catch (System.Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, $"Error hiding placement UI: {ex.Message}");
            }
        }

        public void UpdateUIFeedback()
        {
            try
            {
                var info = new PlacementInfo(_state.CurrentGridPosition, _state.TowerData)
                {
                    CanPlace = _state.CanPlace
                };

                _hud.UpdatePlacementInfo(gridPos, _state.CanPlace);

            }
            catch (System.Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, $"Error updating placement UI feedback: {ex.Message}");
            }
        }
    }
}
