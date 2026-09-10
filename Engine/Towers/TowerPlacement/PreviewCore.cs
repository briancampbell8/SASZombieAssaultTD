// =====================================================================================================
//  FILE: PreviewCore.cs
//  PATH: Engine/Towers/TowerPlacement/PreviewCore.cs
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

using SASZombieAssaultTD.Engine.Towers.Placement;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Towers.TowerEnums;

namespace SASZombieAssaultTD.Engine.Towers.TowerPlacement
{
    public sealed class PreviewCore
    {
        private readonly State _state;
        private readonly InputControl _input;
        private readonly RendererControl _renderer;
        private readonly ValidatorControl _validator;
        private readonly UIControl _ui;
        private readonly Orchestrator _orchestrator;

        public PreviewCore()
        {
            _state = new State();
            _renderer = new RendererControl(_state);
            _validator = new ValidatorControl(_state);
            _ui = new UIControl(_state);
            _orchestrator = new Orchestrator(this);
            _input = new InputControl(this);
        }

        public bool IsActive
        {
            get => _state.IsActive;
            set => _state.IsActive = value;
        }

        public bool CanPlace
        {
            get => _state.CanPlace;
            set => _state.CanPlace = value;
        }

        public TowerType SelectedTowerType
        {
            get => _state.SelectedTowerType;
            set => _state.SelectedTowerType = value;
        }

        public Vector3Int CurrentGridPosition
        {
            get => _state.CurrentGridPosition;
            set => _state.CurrentGridPosition = value;
        }

        public Vector3 CurrentWorldPosition
        {
            get => _state.CurrentWorldPosition;
            set => _state.CurrentWorldPosition = value;
        }

        public TowerData TowerData
        {
            get => _state.TowerData;
            set => _state.TowerData = value;
        }

        public RendererControl TPRenderer => _renderer;
        public ValidatorControl Validator => _validator;

        public event System.Action<Vector3Int> OnPlacementAttempt;
        public event System.Action<TowerType> OnTowerSelected;
        public event System.Action OnPlacementConfirmed;
        public event System.Action OnPlacementCancelled;
        internal void RaisePlacementAttempt(Vector3Int pos) => OnPlacementAttempt?.Invoke(pos);
        internal void RaisePlacementConfirmed() => OnPlacementConfirmed?.Invoke();
        internal void RaisePlacementCancelled() => OnPlacementCancelled?.Invoke();
        internal void RaiseTowerSelected(TowerType type) => OnTowerSelected?.Invoke(type);

        public void StartPlacement(TowerType towerType) =>
            _orchestrator.StartPlacement(towerType);

        public void StopPlacement() =>
            _orchestrator.StopPlacement();

        public void UpdatePosition(Vector3 worldPosition) =>
            _orchestrator.UpdatePosition(worldPosition);

        public bool AttemptPlacement(Vector3Int gridPosition) =>
            _orchestrator.AttemptPlacement(gridPosition);

        public void HandleInput(InputData input) =>
            _input.Handle(input);

        public void Render() =>
            _renderer.Render();

        public void Update(float deltaTime) =>
            _renderer.Update(deltaTime);

        public void SelectTowerType(TowerType towerType) =>
            _orchestrator.SelectTowerType(towerType);

        public PlacementInfo GetPlacementInfo() =>
            _orchestrator.GetPlacementInfo();

        public void ShowPlacementUI() =>
            _ui.ShowPlacementUI();

        public void HidePlacementUI() =>
            _ui.HidePlacementUI();

        public void UpdateUIFeedback() =>
            _ui.UpdateUIFeedback();

        public void Cleanup() =>
            _orchestrator.Cleanup();
    }
}
