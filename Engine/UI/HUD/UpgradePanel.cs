// ====================================================================================================
//  FILE: UpgradePanel.cs
//  PATH: ./Engine/UI/HUD/
//  MODULE: UI
//
//  ROLE:
//      Driver shell for UpgradePanel partial subsystem.
//      Coordinates HUD upgrade panel state while delegating all logic,
//      rendering, layout, transitions, colors, and fonts to PanelsLegacy partials.
// ====================================================================================================

using System;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Drawing;
using System.Linq;
using SASZombieAssaultTD.Engine.Towers;

namespace SASZombieAssaultTD.Engine.UI.HUD
{
    public partial class UpgradePanel : ModernHUDComponent
    {
        // ---------------------------------------------------------------------------------------------
        // Core Fields
        // ---------------------------------------------------------------------------------------------

        private Tower _currentTower;
        private List<TowerUpgrade> _availableUpgrades = new();
        private TowerUpgrade _selectedUpgrade;
        private int _selectedUpgradeIndex = -1;

        private bool _canAffordUpgrade = false;

        private bool _isTransitioning = false;
        private float _transitionTimer = 0f;
        private float _transitionDuration = 0.3f;

        // Visual colors
        private Color _backgroundColor = Color.FromArgb(180, 0, 0, 0);
        private Color _borderColor = Color.FromArgb(255, 200, 200, 200);
        private Color _normalColor = Color.White;
        private Color _warningColor = Color.Orange;
        private Color _dangerColor = Color.Red;
        private Color _successColor = Color.Green;

        // Fonts
        private System.Drawing.Font _titleFont;
        private System.Drawing.Font _textFont;
        private System.Drawing.Font _smallFont;

        // Purchase button
        private RectangleF _purchaseButtonBounds;

        // Events
        public event Action<TowerUpgrade> OnUpgradePurchased;
        public event Action<TowerUpgrade> OnUpgradeSelected;

        // ---------------------------------------------------------------------------------------------
        // Constructors
        // ---------------------------------------------------------------------------------------------

        public UpgradePanel() =>
            // Calls implementation inside UpgradePanel-Fonts.cs
            LoadFonts();

        // Constructor for ENGINE FONT types
        public UpgradePanel(
            Tower currentTower,
            List<TowerUpgrade> availableUpgrades,
            TowerUpgrade selectedUpgrade,
            int selectedUpgradeIndex,
            bool canAffordUpgrade,
            bool isTransitioning,
            float transitionTimer,
            float transitionDuration,
            Color backgroundColor,
            Color borderColor,
            Color normalColor,
            Color warningColor,
            Color dangerColor,
            Color successColor,
            TextRendering.Font titleFont,
            TextRendering.Font textFont,
            TextRendering.Font smallFont,
            RectangleF purchaseButtonBounds)
        {
            _currentTower = currentTower;
            _availableUpgrades = availableUpgrades;
            _selectedUpgrade = selectedUpgrade;
            _selectedUpgradeIndex = selectedUpgradeIndex;
            _canAffordUpgrade = canAffordUpgrade;
            _isTransitioning = isTransitioning;
            _transitionTimer = transitionTimer;
            _transitionDuration = transitionDuration;
            _backgroundColor = backgroundColor;
            _borderColor = borderColor;
            _normalColor = normalColor;
            _warningColor = warningColor;
            _dangerColor = dangerColor;
            _successColor = successColor;

            // Calls implementation inside UpgradePanel-Fonts.cs
            AssignEngineFonts(titleFont, textFont, smallFont);

            _purchaseButtonBounds = purchaseButtonBounds;
        }

        // Constructor for SYSTEM.DRAWING.FONT types
        public UpgradePanel(
            Tower currentTower,
            List<TowerUpgrade> availableUpgrades,
            TowerUpgrade selectedUpgrade,
            int selectedUpgradeIndex,
            bool canAffordUpgrade,
            bool isTransitioning,
            float transitionTimer,
            float transitionDuration,
            Color backgroundColor,
            Color borderColor,
            Color normalColor,
            Color warningColor,
            Color dangerColor,
            Color successColor,
            System.Drawing.Font titleFont,
            System.Drawing.Font textFont,
            System.Drawing.Font smallFont,
            RectangleF purchaseButtonBounds)
        {
            _currentTower = currentTower;
            _availableUpgrades = availableUpgrades;
            _selectedUpgrade = selectedUpgrade;
            _selectedUpgradeIndex = selectedUpgradeIndex;
            _canAffordUpgrade = canAffordUpgrade;
            _isTransitioning = isTransitioning;
            _transitionTimer = transitionTimer;
            _transitionDuration = transitionDuration;
            _backgroundColor = backgroundColor;
            _borderColor = borderColor;
            _normalColor = normalColor;
            _warningColor = warningColor;
            _dangerColor = dangerColor;
            _successColor = successColor;

            // Calls implementation inside UpgradePanel-Fonts.cs
            AssignDrawingFonts(titleFont, textFont, smallFont);

            _purchaseButtonBounds = purchaseButtonBounds;
        }

        // ---------------------------------------------------------------------------------------------
        // Public API
        // ---------------------------------------------------------------------------------------------

        public void SetTower(Tower tower)
        {
            if (tower == null)
            {
                HidePanel();
                return;
            }

            _currentTower = tower;
            _availableUpgrades = tower.AvailableUpgrades.ToList();

            _selectedUpgrade = null;
            _selectedUpgradeIndex = -1;

            _isVisible = true;
            _isTransitioning = true;
            _transitionTimer = 0f;

            UpdateCanAffordStatus();
            StartTransition();
        }

        public void HidePanel()
        {
            _isVisible = false;
            _isTransitioning = false;

            _currentTower = null;
            _availableUpgrades.Clear();
            _selectedUpgrade = null;
            _selectedUpgradeIndex = -1;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (_isTransitioning)
                UpdateTransition(deltaTime);

            if (_currentTower != null)
                UpdateCanAffordStatus();
        }

        protected override void RenderLegacy()
        {
            // Implementation handled via separate subsystem partial code path
        }
    }
}
