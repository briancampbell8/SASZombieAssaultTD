using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Camera;
using SASZombieAssaultTD.Engine.Audio;
using SASZombieAssaultTD.Engine.Towers;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Navigation;
using SASZombieAssaultTD.Engine.Economy;
using SASZombieAssaultTD.Engine.Dictionary;
using SASZombieAssaultTD.Engine.Extensions;
using TowerUpgrade = SASZombieAssaultTD.Engine.Towers.TowerUpgrade;
using AudioSystem = SASZombieAssaultTD.Engine.Audio.AudioSystem;
using EconomyManager = SASZombieAssaultTD.Engine.Economy.EconomyManager;

namespace SASZombieAssaultTD.Engine.Towers
{
    /// <summary>
    /// Input data for handling user input.
    /// </summary>
    public class InputData
    {
        public int MouseX { get; set; }
        public int MouseY { get; set; }
        public bool MouseClicked { get; set; }
        public bool EscapePressed { get; set; }
        public int NumberKey { get; set; }

        public InputData()
        {
            MouseX = 0;
            MouseY = 0;
            MouseClicked = false;
            EscapePressed = false;
            NumberKey = 0;
        }
    }

    /// <summary>
    /// HUD controller for tower placement preview.
    /// </summary>
    public class HUDController
    {
        public bool IsActive { get; set; }
        public TowerPlacementPreview PlacementPreview { get; set; }

        public HUDController()
        {
            IsActive = false;
        }
    }

    /// <summary>
    /// Tower placement preview system for SAS Zombie Assault TD.
    /// Provides visual feedback for tower placement with validation.
    /// </summary>
    public class TowerPlacementPreview
    {
        private bool _isActive = false;
        private TowerType _selectedTowerType = TowerType.Basic;
        private Vector3Int _currentGridPosition;
        private Vector3 _currentWorldPosition;
        private bool _canPlace = false;
        private TowerData _towerData;
        private PlacementRenderer _renderer;
        private PlacementValidator _validator;
        private HUDController _hudController;

        // Preview properties
        private float _previewAlpha = 0.7f;
        private Color _validColor = new Color(0, 255, 0, 180); // Green with transparency
        private Color _invalidColor = new Color(255, 0, 0, 180); // Red with transparency
        private Color _rangeColor = new Color(255, 255, 0, 100); // Yellow with transparency

        // Events
        public event Action<Vector3Int> OnPlacementAttempt;
        public event Action<TowerType> OnTowerSelected;
        public event Action OnPlacementConfirmed;
        public event Action OnPlacementCancelled;

        public bool IsActive => _isActive;
        public TowerType SelectedTowerType => _selectedTowerType;
        public Vector3Int CurrentGridPosition => _currentGridPosition;
        public bool CanPlace => _canPlace;

        public TowerPlacementPreview()
        {
            Initialize();
        }

        /// <summary>
        /// Initialize the placement preview system.
        /// </summary>
        private void Initialize()
        {
            _renderer = new PlacementRenderer();
            _validator = new PlacementValidator();

            Console.WriteLine("Tower Placement Preview initialized");
        }

        /// <summary>
        /// Start placement preview for a specific tower type.
        /// </summary>
        /// <param name="towerType">Type of tower to place.</param>
        public void StartPlacement(TowerType towerType)
        {
            if (_isActive && _selectedTowerType == towerType)
                return;

            try
            {
                _selectedTowerType = towerType;
                _towerData = TowerDatabase.GetTowerData(towerType.ToString());

                if (_towerData == null)
                {
                    Console.WriteLine($"No tower data found for type: {towerType}");
                    return;
                }

                _isActive = true;

                // Initialize renderer with tower data
                _renderer.Initialize(_towerData);

                // Play selection sound
                AudioSystem.PlaySound("tower_select");

                // Show placement UI
                ShowPlacementUI();

                // Notify of tower selection
                OnTowerSelected?.Invoke(towerType);

                Console.WriteLine($"Started placement preview for {towerType}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting placement preview: {ex.Message}");
                _isActive = false;
            }
        }

        /// <summary>
        /// Stop placement preview.
        /// </summary>
        public void StopPlacement()
        {
            if (!_isActive) return;

            _isActive = false;
            _canPlace = false;

            // Hide placement UI
            HidePlacementUI();

            // Play cancel sound
            AudioSystem.PlaySound("tower_cancel");

            // Notify of cancellation
            OnPlacementCancelled?.Invoke();

            Console.WriteLine("Stopped placement preview");
        }

        /// <summary>
        /// Update placement preview based on current mouse position.
        /// </summary>
        /// <param name="worldPosition">Current mouse world position.</param>
        public void UpdatePosition(Vector3 worldPosition)
        {
            if (!_isActive) return;

            try
            {
                _currentWorldPosition = worldPosition;
                _currentGridPosition = WorldToGrid(worldPosition);

                // Validate placement
                _canPlace = _validator.CanPlaceTower(_currentGridPosition, _towerData);

                // Update renderer
                _renderer.UpdatePosition(_currentWorldPosition, _currentGridPosition, _canPlace);

                // Update UI feedback
                UpdateUIFeedback();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating placement position: {ex.Message}");
            }
        }

        /// <summary>
        /// Attempt to place the tower at current position.
        /// </summary>
        /// <returns>True if placement was successful.</returns>
        public bool AttemptPlacement(Vector3Int gridPosition)
        {
            if (!_isActive) return false;

            try
            {
                // Validate placement
                if (!_validator.CanPlaceTower(gridPosition, _towerData))
                {
                    PlayInvalidPlacementSound();
                    return false;
                }

                // Check if player can afford
                if (!EconomyManager.CanAfford(_towerData.Cost))
                {
                    PlayInsufficientFundsSound();
                    return false;
                }

                // Notify of placement attempt
                OnPlacementAttempt?.Invoke(gridPosition);

                // Actually place the tower
                var success = PlaceTower(gridPosition);

                if (success)
                {
                    // Play placement sound
                    AudioSystem.PlaySound("tower_place");

                    // Notify of successful placement
                    OnPlacementConfirmed?.Invoke();

                    // Stop placement preview
                    StopPlacement();
                }

                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error attempting tower placement: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Handle input for placement preview.
        /// </summary>
        /// <param name="input">Input data.</param>
        public void HandleInput(InputData input)
        {
            if (!_isActive) return;

            try
            {
                // Update position based on mouse
                if (input.MouseX >= 0 && input.MouseY >= 0)
                {
                    var worldPos = CameraSystem.Instance.ScreenToWorld(input.MouseX, input.MouseY);
                    UpdatePosition(worldPos);
                }

                // Handle placement
                if (input.MouseClicked && _canPlace)
                {
                    AttemptPlacement(_currentGridPosition);
                }

                // Handle cancellation
                if (input.EscapePressed)
                {
                    StopPlacement();
                }

                // Handle tower type switching
                HandleTowerTypeSwitching(input);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling placement input: {ex.Message}");
            }
        }

        /// <summary>
        /// Render the placement preview.
        /// </summary>
        public void Render()
        {
            if (!_isActive) return;

            try
            {
                _renderer.Render();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rendering placement preview: {ex.Message}");
            }
        }

        /// <summary>
        /// Update placement preview.
        /// </summary>
        /// <param name="deltaTime">Time since last frame.</param>
        public void Update(float deltaTime)
        {
            if (!_isActive) return;

            try
            {
                _renderer.Update(deltaTime);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating placement preview: {ex.Message}");
            }
        }

        /// <summary>
        /// Select a different tower type.
        /// </summary>
        /// <param name="towerType">New tower type to select.</param>
        public void SelectTowerType(TowerType towerType)
        {
            if (_selectedTowerType == towerType && _isActive)
                return;

            if (_isActive)
            {
                StopPlacement();
            }

            StartPlacement(towerType);
        }

        /// <summary>
        /// Get placement information for UI display.
        /// </summary>
        /// <returns>Placement information.</returns>
        public PlacementInfo GetPlacementInfo()
        {
            if (!_isActive || _towerData == null)
                return null;

            return new PlacementInfo(_currentGridPosition, _towerData)
            {
                TowerType = _selectedTowerType,
                TowerName = _towerData.Name,
                Cost = _towerData.Cost,
                CanAfford = EconomyManager.CanAfford(_towerData.Cost),
                CanPlace = _canPlace,
                CurrentPosition = new Vector3(_currentGridPosition.X, _currentGridPosition.Y, _currentGridPosition.Z),
                Range = _towerData.Range,
                Damage = _towerData.Damage,
                FireRate = _towerData.FireRate
            };
        }

        /// <summary>
        /// Place the actual tower.
        /// </summary>
        private bool PlaceTower(Vector3Int gridPosition)
        {
            try
            {
                // Deduct cost
                EconomyManager.Spend(_towerData.Cost);

                // Create tower
                var worldPosition = GridToWorld(gridPosition);
                var tower = TowerFactory.CreateTower(_selectedTowerType.ToString(), worldPosition);

                if (tower == null)
                {
                    // Refund cost on failure
                    EconomyManager.Earn(_towerData.Cost);
                    return false;
                }

                // Mark grid as occupied
                NavigationGrid.Instance.SetOccupied(gridPosition.X, gridPosition.Y, _towerData.GridSize, true);

                // Add tower to game world
                GameWorld.Instance.AddEntity(tower);

                // Add to tower registry
                TowerRegistry.AddTower((Tower)tower);

                Console.WriteLine($"Successfully placed {_selectedTowerType} at {gridPosition}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error placing tower: {ex.Message}");
                // Refund cost on failure
                EconomyManager.Earn(_towerData.Cost);
                return false;
            }
        }

        /// <summary>
        /// Handle tower type switching with number keys.
        /// </summary>
        private void HandleTowerTypeSwitching(InputData input)
        {
            // Map number keys to tower types
            var towerType = input.NumberKey switch
            {
                1 => TowerType.Basic,
                4 => TowerType.Tesla,
                3 => TowerType.Splash,
                5 => TowerType.Rapid,
                6 => TowerType.Sniper,
                _ => _selectedTowerType
            };

            if (towerType != _selectedTowerType)
            {
                SelectTowerType(towerType);
            }
        }

        /// <summary>
        /// Show placement UI elements.
        /// </summary>
        private void ShowPlacementUI()
        {
            _hudController?.ShowPlacementInfo(GetPlacementInfo().ToString());
        }

        /// <summary>
        /// Hide placement UI elements.
        /// </summary>
        private void HidePlacementUI()
        {
            _hudController?.HidePlacementInfo();
        }

        /// <summary>
        /// Update UI feedback based on placement validity.
        /// </summary>
        private void UpdateUIFeedback()
        {
            var info = GetPlacementInfo();
            if (info != null)
            {
                _hudController?.UpdatePlacementInfo(info.ToString());
            }
        }

        /// <summary>
        /// Play sound for invalid placement.
        /// </summary>
        private void PlayInvalidPlacementSound()
        {
            AudioSystem.PlaySound("invalid_placement");
        }

        /// <summary>
        /// Play sound for insufficient funds.
        /// </summary>
        private void PlayInsufficientFundsSound()
        {
            AudioSystem.PlaySound("insufficient_funds");
        }

        /// <summary>
        /// Convert world coordinates to grid coordinates.
        /// </summary>
        private Vector3Int WorldToGrid(Vector3 worldPosition)
        {
            return NavigationGrid.Instance.WorldToGrid(worldPosition);
        }

        /// <summary>
        /// Convert grid coordinates to world coordinates.
        /// </summary>
        private Vector3 GridToWorld(Vector3Int gridPosition)
        {
            return NavigationGrid.Instance.GridToWorld(gridPosition);
        }

        /// <summary>
        /// Cleanup resources.
        /// </summary>
        public void Cleanup()
        {
            StopPlacement();
            _renderer?.Cleanup();
            _validator?.Cleanup();
        }
    }
}
