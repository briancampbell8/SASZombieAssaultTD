// =====================================================================================================
//  FILE: Orchestrator.cs
//  PATH: Engine/Towers/TowerPlacement/Orchestrator.cs
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
using SASZombieAssaultTD.Engine.Navigation;
using SASZombieAssaultTD.Engine.Towers.Placement;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.Towers.TowerEnums;
using AudioSystem = SASZombieAssaultTD.Engine.ECS.AudioSystem;
using EconomyManager = SASZombieAssaultTD.Engine.Economy.EconomyManager;

namespace SASZombieAssaultTD.Engine.Towers.TowerPlacement
{
    internal sealed class Orchestrator
    {
        private readonly PreviewCore _core;

        public Orchestrator(PreviewCore core)
        {
            _core = core;
        }

        public void StartPlacement(TowerType towerType)
        {
            if (_core.IsActive && _core.SelectedTowerType == towerType)
                return;

            try
            {
                _core.SelectedTowerType = towerType;

                var data = TowerDatabase.GetTowerData(towerType.ToString());
                if (data == null)
                {
                    DLogger.Log($"No tower data found for type: {towerType}");
                    return;
                }

                _core.TowerData = (TowerData)data;
                _core.IsActive = true;

                _core.TPRenderer.Initialize(_core.TowerData);
                AudioSystem.PlaySound("tower_select");
                _core.ShowPlacementUI();
                _core.RaiseTowerSelected(towerType);

                DLogger.Log($"Started placement preview for {towerType}");
            }
            catch (Exception ex)
            {
                DLogger.Log($"Error starting placement preview: {ex.Message}");
                _core.IsActive = false;
            }
        }

        public void StopPlacement()
        {
            if (!_core.IsActive) return;

            _core.IsActive = false;
            _core.CanPlace = false;

            _core.HidePlacementUI();
            AudioSystem.PlaySound("tower_cancel");
            _core.RaisePlacementCancelled();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Stopped placement preview");
        }

        public void UpdatePosition(Vector3 worldPosition)
        {
            if (!_core.IsActive) return;

            try
            {
                _core.CurrentWorldPosition = worldPosition;
                _core.CurrentGridPosition = NavigationGrid.Instance.WorldToGrid(worldPosition);

                _core.CanPlace = _core.Validator.CanPlaceTower(_core.CurrentGridPosition, _core.TowerData);

                _core.TPRenderer.UpdatePosition(_core.CurrentWorldPosition, _core.CurrentGridPosition, _core.CanPlace);

                _core.UpdateUIFeedback();
            }
            catch (Exception ex)
            {
                DLogger.Log($"Error updating placement position: {ex.Message}");
            }
        }

        public bool AttemptPlacement(Vector3Int gridPosition)
        {
            if (!_core.IsActive) return false;

            try
            {
                if (!_core.Validator.CanPlaceTower(gridPosition, _core.TowerData))
                {
                    AudioSystem.PlaySound("invalid_placement");
                    return false;
                }

                if (!EconomyManager.CanAfford(_core.TowerData.Cost))
                {
                    AudioSystem.PlaySound("insufficient_funds");
                    return false;
                }

                _core.RaisePlacementAttempt(gridPosition);

                bool success = PlaceTower(gridPosition);
                if (success)
                {
                    AudioSystem.PlaySound("tower_place");
                    _core.RaisePlacementConfirmed();
                    StopPlacement();
                }

                return success;
            }
            catch (Exception ex)
            {
                DLogger.Log($"Error attempting tower placement: {ex.Message}");
                return false;
            }
        }

        private bool PlaceTower(Vector3Int gridPosition)
        {
            EconomyManager.Spend(_core.TowerData.Cost);

            Vector3 worldPos = NavigationGrid.Instance.GridToWorld(gridPosition);
            var tower = TowerFactory.CreateTower(_core.SelectedTowerType.ToString(), worldPos);

            if (tower == null)
            {
                EconomyManager.Earn(_core.TowerData.Cost);
                return false;
            }

            NavigationGrid.Instance.SetOccupied(gridPosition.X, gridPosition.Y, _core.TowerData.GridSize, true);
            TowerRegistry.Instance.AddTower((Tower)tower);

            DLogger.Log($"Successfully placed {_core.SelectedTowerType} at {gridPosition}");
            return true;
        }

        public void SelectTowerType(TowerType towerType)
        {
            if (_core.SelectedTowerType == towerType && _core.IsActive)
                return;

            if (_core.IsActive)
                StopPlacement();

            StartPlacement(towerType);
        }

        public PlacementInfo GetPlacementInfo()
        {
            if (!_core.IsActive || _core.TowerData == null)
                return null;

            return new PlacementInfo(_core.CurrentGridPosition, _core.TowerData)
            {
                TowerType = _core.SelectedTowerType,
                TowerName = _core.TowerData.Name,
                Cost = _core.TowerData.Cost,
                CanAfford = EconomyManager.CanAfford(_core.TowerData.Cost),
                CanPlace = _core.CanPlace,
                CurrentPosition = new Vector3(_core.CurrentGridPosition.X, _core.CurrentGridPosition.Y, _core.CurrentGridPosition.Z),
                FireRate = _core.TowerData.FireRate
            };
        }

        public void Cleanup()
        {
            StopPlacement();
            _core.TPRenderer.Cleanup();
            _core.Validator.Cleanup();
        }
    }
}
