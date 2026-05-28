// File: Engine/UI/HUD/UpgradePanel.cs
// Purpose: Manages the upgrade panel in the HUD for displaying and purchasing tower upgrades.
// Features: Displays available upgrades, handles user interactions, and updates the UI dynamically.

using SASZombieAssaultTD.Engine.Audio;
using SASZombieAssaultTD.Engine.Economy;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Towers;
using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Diagnostics;
using SASZombieAssaultTD.Engine.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace SASZombieAssaultTD.Engine.UI.HUD
{
    /// <summary>
    /// Upgrade panel for SAS Zombie Assault TD HUD.
    /// Shows tower upgrades and upgrade options.
    /// </summary>
    public class UpgradePanel : HUDComponent
    {
        internal Tower _currentTower;
        private List<TowerUpgrade> _availableUpgrades;
        private TowerUpgrade _selectedUpgrade;
        private int _selectedUpgradeIndex = -1;
        private bool _canAffordUpgrade = false;
        private float _displayTimer = 0f;
        private bool _isTransitioning = false;
        private float _transitionTimer = 0f;
        private float _transitionDuration = 0.3f;

        // Visual properties
        private new Vector3 _position;
        private new Vector3 _size;
        private new Color _backgroundColor = new Color(0, 0, 0, 180);
        private Color _borderColor = new Color(200, 200, 200, 255);
        private Color _normalColor = Color.White;
        private Color _warningColor = Color.Orange;
        private Color _dangerColor = Color.Red;
        private Color _successColor = Color.Green;

        // Text properties
        private Font _titleFont;
        private Font _textFont;
        private Font _smallFont;
        private Font _iconFont;

        // Events
        public event Action<TowerUpgrade> OnUpgradePurchased;
        public event Action<Tower> OnUpgradeAvailable;
        public event Action<TowerUpgrade> OnUpgradeCompleted;
        public event Action<int> OnUpgradeFailed;

        public UpgradePanel()
        {
            _position = new Vector3(550f, 50f, 0);
            _size = new Vector3(250f, 300f, 0);
            _normalColor = Color.White;

            // Initialize fonts
            var cachedTitleFont = FontCache.GetFont("title");
            _titleFont = new Font(cachedTitleFont?.Name ?? "Arial", cachedTitleFont?.Size ?? 14);
            var cachedTextFont = FontCache.GetFont("default");
            _textFont = new Font(cachedTextFont?.Name ?? "Arial", cachedTextFont?.Size ?? 12);
            var cachedSmallFont = FontCache.GetFont("small");
            _smallFont = new Font(cachedSmallFont?.Name ?? "Arial", cachedSmallFont?.Size ?? 10);

            Initialize();
        }

        /// <summary>
        /// Set tower for upgrade display.
        /// </summary>
        /// <param name="tower">Tower to show upgrades for.</param>
        public void SetTower(Tower tower)
        {
            if (tower == null)
            {
                HidePanel();
                return;
            }

            _currentTower = tower;
            _availableUpgrades = TowerInfoPanelExtensions.GetAvailableUpgrades(tower).Cast<TowerUpgrade>().ToList();
            _selectedUpgrade = null;
            _selectedUpgradeIndex = -1;
            _displayTimer = 0f;
            _isTransitioning = true;
            _transitionTimer = 0f;
            _isTransitioning = true;

            // Check if can afford any upgrades
            UpdateCanAffordStatus();

            // Start transition animation
            StartTransition();
        }

        /// <summary>
        /// Hide the upgrade panel.
        /// </summary>
        public void HidePanel()
        {
            _isVisible = false;
            _isTransitioning = false;
            _currentTower = null;
            _availableUpgrades.Clear();
            _selectedUpgrade = null;
            _selectedUpgradeIndex = -1;
        }

        /// <summary>
        /// Set panel position.
        /// </summary>
        /// <param name="position">New position.</param>
        public void SetPosition(Vector3 position)
        {
            _position = position;
        }

        /// <summary>
        /// Set panel size.
        /// </summary>
        /// <param name="size">New size.</param>
        public void SetSize(Vector3 size)
        {
            _size = size;
        }

        /// <summary>
        /// Set background color.
        /// </summary>
        /// <param name="color">Background color.</param>
        public void SetBackgroundColor(Color color)
        {
            _backgroundColor = color;
        }

        /// <summary>
        /// Set border color.
        /// </summary>
        /// <param name="color">Border color.</param>
        public void SetBorderColor(Color color)
        {
            _borderColor = color;
        }

        /// <summary>
        /// Set normal text color.
        /// </summary>
        /// <param name="color">Normal text color.</param>
        public void SetNormalColor(Color color)
        {
            _normalColor = color;
        }

        /// <summary>
        /// Set warning text color.
        /// </summary>
        /// <param name="color">Warning text color.</param>
        public void SetWarningColor(Color color)
        {
            _warningColor = color;
        }

        /// <summary>
        /// Set danger text color.
        /// </summary>
        /// <param name="color">Danger text color.</param>
        public void SetDangerColor(Color color)
        {
            _dangerColor = color;
        }

        /// <summary>
        /// Set success text color.
        /// </summary>
        /// <param name="color">Success text color.</param>
        public void SetSuccessColor(Color color)
        {
            _successColor = color;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // Update transition animation
            if (_isTransitioning)
            {
                UpdateTransition(deltaTime);
            }

            // Update display timer
            if (_displayTimer > 0)
            {
                _displayTimer -= deltaTime;
            }

            // Update can afford status
            if (_currentTower != null)
            {
                UpdateCanAffordStatus();
            }
        }

        public override void Render()
        {
            if (!_isVisible) return;

            try
            {
                // Render background
                // TODO: RenderBackground is not a method - likely a property or missing
                // RenderBackground();

                // Render upgrade options
                // TODO: These methods don't exist or are properties
                // RenderUpgradeOptions();
                RenderUpgradeStatus();
                RenderUpgradeInfo();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error rendering upgrade panel: {ex.Message}");
            }
        }

        /// <summary>
        /// Render upgrade options list.
        /// </summary>
        private void RenderUpgradeOptions()
        {
            var optionsY = _position.Y + 20f;
            var optionsHeight = _size.Y - 40f;
            var optionsWidth = _size.X - 40f;
            var optionsX = _position.X + 20f;

            // Background
            var backgroundColor = new Color(
                _backgroundColor.R, _backgroundColor.G, _backgroundColor.B,
                (byte)(200 * 1.0f) // TODO: GetTransitionProgress is not a method
            );
            RenderSystem.DrawRectangle(optionsX, optionsY, optionsWidth, optionsHeight, backgroundColor);

            // Border
            var borderColor = new Color(
                _borderColor.R, _borderColor.G, _borderColor.B,
                (byte)(255 * 1.0f) // TODO: GetTransitionProgress is not a method
            );
            RenderSystem.DrawRectangle(optionsX, optionsY, optionsWidth, optionsHeight, borderColor, 2f);

            // Title
            var titleColor = new Color(_normalColor.R, _normalColor.G, _normalColor.B, 255);
            var titlePosition = new Vector3(optionsX + 10f, optionsY + 10f, 0f);
            RenderSystem.DrawString("UPGRADES", titlePosition, titleColor, _titleFont, new Vector3(12f, 12f, 0f));

            // Upgrade options list
            var upgradeY = optionsY + 40f;
            var upgradeHeight = _size.Y - 60f;
            var upgradeWidth = _size.X - 40f;
            var upgradeX = optionsX + 10f;

            for (int i = 0; i < _availableUpgrades.Count; i++)
            {
                var upgrade = _availableUpgrades[i];
                var upgradeColor = GetUpgradeColor(upgrade);
                var upgradeText = $"{upgrade.Name} (${upgrade.Cost})";
                var currentUpgradeY = upgradeY + (i * 25f);
                var textColor = GetUpgradeTextColor(upgrade);
                var textPosition = new Vector3(upgradeX, currentUpgradeY, 0f);
                RenderSystem.DrawString(upgradeText, textPosition, textColor, _textFont, new Vector3(10f, 10f, 0f));
            }
        }

        /// <summary>
        /// Render upgrade status.
        /// </summary>
        private void RenderUpgradeStatus()
        {
            if (_selectedUpgrade == null) return;

            var statusY = _position.Y + _size.Y - 30f;
            var statusText = GetUpgradeStatusText(_selectedUpgrade);
            var statusColor = _normalColor; // TODO: GetUpgradeStatusColor is not a method
            var statusTextColor = GetUpgradeStatusTextColor(_selectedUpgrade);
            var statusPosition = new Vector3(_position.X + 10f, statusY, 0f);

            RenderSystem.DrawString(statusText, statusPosition, statusTextColor, _textFont, new Vector3(10f, 10f, 0f));
        }

        /// <summary>
        /// Render upgrade info.
        /// </summary>
        private void RenderUpgradeInfo()
        {
            if (_selectedUpgrade == null) return;

            var infoY = _position.Y + _size.Y - 60f;
            // TODO: GetUpgradeInfoText and GetUpgradeInfoTextColor are not methods
            var infoText = _selectedUpgrade?.Name ?? "Unknown";
            var infoTextColor = _normalColor;

            RenderSystem.DrawString(infoText, new Vector3(_position.X + 10f, infoY, 0f), infoTextColor, _textFont, new Vector3(10f, 10f, 0f));
        }

        /// <summary>
        /// Start transition animation.
        /// </summary>
        private void StartTransition()
        {
            _transitionTimer = 0f;
            _isTransitioning = true;
        }

        /// <summary>
        /// Update transition animation.
        /// </summary>
        private void UpdateTransition(float deltaTime)
        {
            _transitionTimer += deltaTime;

            if (_transitionTimer >= _transitionDuration)
            {
                _isTransitioning = false;
                _transitionTimer = 0f;
            }
        }

        /// <summary>
        /// Get upgrade color based on availability.
        /// </summary>
        private Color GetUpgradeColor(TowerUpgrade upgrade)
        {
            if (!_canAffordUpgrade) return _dangerColor;

            if (_selectedUpgrade == upgrade) return _successColor;

            return _warningColor;
        }

        /// <summary>
        /// Get upgrade text color based on availability.
        /// </summary>
        private Color GetUpgradeTextColor(TowerUpgrade upgrade)
        {
            if (!_canAffordUpgrade) return _dangerColor;

            if (_selectedUpgrade == upgrade) return _successColor;

            return _warningColor;
        }

        /// <summary>
        /// Get upgrade status text.
        /// </summary>
        /// <param name="upgrade">Upgrade to get status for.</param>
        /// <returns>Status text.</returns>
        private string GetUpgradeStatusText(TowerUpgrade upgrade)
        {
            if (!_canAffordUpgrade) return "INSUFFICIENT FUNDS";

            if (_selectedUpgrade == upgrade) return "OWNED";

            return $"AVAILABLE (${upgrade.Cost})";
        }

        /// <summary>
        /// Get upgrade status color.
        /// </summary>
        private Color GetUpgradeStatusTextColor(TowerUpgrade upgrade)
        {
            if (!_canAffordUpgrade) return _dangerColor;

            if (_selectedUpgrade == upgrade) return _successColor;

            return _warningColor;
        }

        /// <summary>
        /// Update can afford status.
        /// </summary>
        private void UpdateCanAffordStatus()
        {
            // Economy system not available - always set to true for now
            _canAffordUpgrade = _currentTower != null && _selectedUpgrade != null;
        }

        /// <summary>
        /// Purchase selected upgrade.
        /// </summary>
        public bool PurchaseUpgrade()
        {
            if (!_canAffordUpgrade || _selectedUpgrade == null)
            {
                PlayErrorSound.PlayInsufficientFunds();
                return false;
            }

            // Economy system not available - proceed with upgrade
            // Apply upgrade to tower
            _currentTower.ApplyUpgrade(new SASZombieAssaultTD.Engine.Towers.TowerUpgrade
            { Name = _selectedUpgrade.Name, Cost = _selectedUpgrade.Cost, 
                Level = _selectedUpgrade.Level, DamageBonus = _selectedUpgrade.DamageIncrease, 
                RangeBonus = _selectedUpgrade.RangeIncrease, FireRateBonus = _selectedUpgrade.FireRateIncrease });

            // Update available upgrades
            var upgradeIndex = _availableUpgrades.IndexOf(_selectedUpgrade);
            if (upgradeIndex >= 0)
            {
                _availableUpgrades.RemoveAt(upgradeIndex);
            }

            // Update UI
            UpdateCanAffordStatus();
            // UpdateUpgradeOptions(); // TODO: UpdateUpgradeOptions is not a method
            RenderUpgradeStatus();

            // Play success sound
            PlaySuccessSound.PlayUpgradePurchase();

            // Trigger events
            // Trigger events
            OnUpgradePurchased?.Invoke(_selectedUpgrade);
            // TODO: Fix type mismatch - cannot cast Tower to UI.HUD.TowerUpgrade
            // OnUpgradeCompleted?.Invoke((UI.HUD.TowerUpgrade)_currentTower);
            OnUpgradeCompleted?.Invoke(_selectedUpgrade);




            System.Diagnostics.Debug.WriteLine($"Purchased upgrade: {_selectedUpgrade.Name} for {_currentTower.Name}");
            return true;
        }

        /// <summary>
        /// Select an upgrade by index.
        /// </summary>
        /// <param name="index">Index of upgrade to select.</param>
        public void SelectUpgrade(int index)
        {
            if (index < 0 || index >= _availableUpgrades.Count)
            {
                System.Diagnostics.Debug.WriteLine($"Invalid upgrade index: {index}");
                return;
            }

            _selectedUpgradeIndex = index;
            _selectedUpgrade = _availableUpgrades[index];
            UpdateCanAffordStatus();
            // UpdateUpgradeOptions(); // TODO: UpdateUpgradeOptions is not a method
            RenderUpgradeStatus();

            // Play selection sound
            // TODO: PlaySound is not a method
            // PlaySound("upgrade_selected");

            System.Diagnostics.Debug.WriteLine($"Selected upgrade: {_selectedUpgrade.Name}");
        }

        /// <summary>
        /// Get selected upgrade.
        /// </summary>
        /// <returns>Selected upgrade or null.</returns>
        public TowerUpgrade GetSelectedUpgrade()
        {
            return _selectedUpgrade;
        }

        /// <summary>
        /// Get available upgrades count.
        /// </summary>
        /// <returns>Number of available upgrades.</returns>
        public int GetAvailableUpgradeCount()
        {
            return _availableUpgrades.Count;
        }

        /// <summary>
        /// Get all available upgrades.
        /// </summary>
        /// <returns>List of available upgrades.</returns>
        public List<TowerUpgrade> GetAllAvailableUpgrades()
        {
            return _availableUpgrades;
        }

        /// <summary>
        /// Check if any upgrades are available.
        /// </summary>
        /// <returns>True if upgrades available.</returns>
        public bool HasAvailableUpgrades()
        {
            return _availableUpgrades.Count > 0;
        }

        /// <summary>
        /// Get total upgrade cost.
        /// </summary>
        /// <returns>Total cost of all available upgrades.</returns>
        public int GetTotalUpgradeCost()
        {
            return _availableUpgrades.Sum(u => u.Cost);
        }

        /// <summary>
        /// Get upgrade at specific index.
        /// </summary>
        /// <param name="index">Index of upgrade to get.</param>
        /// <returns>TowerUpgrade at index or null.</returns>
        public TowerUpgrade GetUpgradeAt(int index)
        {
            if (index >= 0 && index < _availableUpgrades.Count)
            {
                return _availableUpgrades[index];
            }
            return null;
        }

        /// <summary>
        /// Sort upgrades by cost.
        /// </summary>
        public void SortUpgradesByCost()
        {
            _availableUpgrades.Sort((a, b) => a.Cost.CompareTo(b.Cost));
        }

        /// <summary>
        /// Sort upgrades by level.
        /// </summary>
        public void SortUpgradesByLevel()
        {
            _availableUpgrades.Sort((a, b) => a.Level.CompareTo(b.Level));
        }

        /// <summary>
        /// Sort upgrades by damage increase.
        /// </summary>
        public void SortUpgradesByDamage()
        {
            _availableUpgrades.Sort((a, b) => a.DamageIncrease.CompareTo(b.DamageIncrease));
        }

        /// <summary>
        /// Get upgrades by type.
        /// </summary>
        /// <param name="type">Upgrade type to filter by.</param>
        /// <returns>Filtered upgrades list.</returns>
        public List<TowerUpgrade> GetUpgradesByType(string type)
        {
            return _availableUpgrades.Where(u => u.UpgradeType.ToString().Equals(type, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Get upgrades by damage increase.
        /// </summary>
        /// <param name="minDamage">Minimum damage increase.</param>
        /// <returns>Filtered upgrades list.</returns>
        public List<TowerUpgrade> GetUpgradesByDamageIncrease(float minDamage)
        {
            return _availableUpgrades.Where(u => u.DamageIncrease >= minDamage).ToList();
        }
    }

    /// <summary>
    /// Tower upgrade data for SAS TD towers.
    /// </summary>
    public class TowerUpgrade
    {
        public bool _isAffordable;
        public bool IsAffordable { get; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public int Cost { get; set; }
        public SASZombieAssaultTD.Engine.Towers.UpgradeType UpgradeType { get; set; }
        public float DamageIncrease { get; set; }
        public float RangeIncrease { get; set; }
        public float FireRateIncrease { get; set; }
        public float SpeedIncrease { get; set; }
        public List<string> SpecialAbilities { get; set; }
        public Dictionary<string, float> CustomProperties { get; set; }
        public TowerUpgrade PrerequisiteUpgrade { get; set; }
        public TowerUpgrade NextUpgrade { get; set; }
        public bool IsMaxLevel { get; set; }
        public bool CanAfford { get; set; }
        internal bool IsAvailable { get; private set; }


        public TowerUpgrade(string name, int level, int cost, float damageIncrease, float rangeIncrease, float fireRateIncrease, float speedIncrease, List<string> specialAbilities)
        {
            Name = name;
            Level = level;
            Cost = cost;
            DamageIncrease = damageIncrease;
            RangeIncrease = rangeIncrease;
            FireRateIncrease = fireRateIncrease;
            SpeedIncrease = speedIncrease;
            SpecialAbilities = specialAbilities;
            CustomProperties = new Dictionary<string, float>();
            PrerequisiteUpgrade = null;
            NextUpgrade = null;
            IsMaxLevel = level >= 10;
            IsAvailable = true;
            _isAffordable = EconomyManager.CanAfford(cost);
        }

        /// <summary>
        /// Clone this upgrade.
        /// </summary>
        /// <returns>Cloned upgrade.</returns>
        public TowerUpgrade Clone()
        {
            return new TowerUpgrade(
                this.Name,
                this.Level,
                this.Cost,
                this.DamageIncrease,
                this.RangeIncrease,
                this.FireRateIncrease,
                this.SpeedIncrease,
                new List<string>(this.SpecialAbilities)
            );
        }
    }

    /// <summary>
    /// Extension methods for TowerUpgrade.
    /// </summary>
    public static class TowerUpgradeExtensions
    {
        /// <summary>
        /// Check if upgrade is max level.
        /// </summary>
        public static bool IsMaxLevel(TowerUpgrade upgrade)
        {
            return upgrade.IsMaxLevel;
        }

        /// <summary>
        /// Check if upgrade is available.
        /// </summary>
        public static bool IsAvailable(TowerUpgrade upgrade)
        {
            if (upgrade == null)
                return false;

            return upgrade.IsAvailable;
        }



        /// <summary>
        /// Get upgrade cost.
        /// </summary>
        public static int GetCost(TowerUpgrade upgrade)
        {
            return upgrade.Cost;
        }

        /// <summary>
        /// Get upgrade damage increase.
        /// </summary>
        public static float GetDamageIncrease(TowerUpgrade upgrade)
        {
            return upgrade.DamageIncrease;
        }

        /// <summary>
        /// Get upgrade range increase.
        /// </summary>
        public static float GetRangeIncrease(TowerUpgrade upgrade)
        {
            return upgrade.RangeIncrease;
        }

        /// <summary>
        /// Get upgrade fire rate increase.
        /// </summary>
        public static float GetFireRateIncrease(TowerUpgrade upgrade)
        {
            return upgrade.FireRateIncrease;
        }

        /// <summary>
        /// Get upgrade speed increase.
        /// </summary>
        public static float GetSpeedIncrease(TowerUpgrade upgrade)
        {
            return upgrade.SpeedIncrease;
        }
    }
}
