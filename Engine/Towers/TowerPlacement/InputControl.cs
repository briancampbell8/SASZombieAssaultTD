// =====================================================================================================
//  FILE: InputControl.cs
//  PATH: Engine/Towers/TowerPlacement/InputControl.cs
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
using SASZombieAssaultTD.Engine.Camera;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Towers.TowerPlacement
{
    internal sealed class InputControl
    {
        private readonly PreviewCore _core;

        public InputControl(PreviewCore core)
        {
            _core = core;
        }

        public void Handle(InputData input)
        {
            if (!_core.IsActive) return;

            try
            {
                Vector3 pos = input.MousePosition;

                if (pos.X >= 0 && pos.Y >= 0)
                {
                    Vector3 worldPos = CameraSystem.Instance.ScreenToWorld((int)pos.X, (int)pos.Y);
                    _core.UpdatePosition(worldPos);
                }

                if (input.LeftMousePressed && _core.CanPlace)
                {
                    _core.AttemptPlacement(_core.CurrentGridPosition);
                }

                if (input.IsEscapePressed)
                {
                    _core.StopPlacement();
                }
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, $"Error handling placement input: {ex.Message}");
            }
        }
    }
}
