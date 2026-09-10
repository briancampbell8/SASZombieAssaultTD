// =====================================================================================================
//  FILE: UpgradePanel-Colors.cs
//  PATH: Engine/UI/HUD/PanelsLegacy/UpgradePanel-Colors.cs
//  SUBSYSTEM: UI / HUD (Legacy Panel Partials)
//
//  ROLE:
//      Provides deterministic color configuration helpers for UpgradePanel.
//      This partial isolates all color‑related logic from rendering, layout,
//      transitions, and upgrade selection/purchase behavior.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Economy;
using SASZombieAssaultTD.Engine.Towers;

// Fixed Namespace to align perfectly with the driver file UpgradePanel.cs
namespace SASZombieAssaultTD.Engine.UI.HUD
{
    // Fixed access modifier from 'internal' to 'public' to match driver declaration
    public partial class UpgradePanel
    {
        // ---------------------------------------------------------------------------------------------
        // Color Setters
        // ---------------------------------------------------------------------------------------------

        public void SetBackgroundColor(Color color) => _backgroundColor = color;
        public void SetBorderColor(Color color) => _borderColor = color;
        public void SetNormalColor(Color color) => _normalColor = color;
        public void SetWarningColor(Color color) => _warningColor = color;
        public void SetDangerColor(Color color) => _dangerColor = color;
        public void SetSuccessColor(Color color) => _successColor = color;

        // ---------------------------------------------------------------------------------------------
        // Color Helpers
        // ---------------------------------------------------------------------------------------------

        private Color GetUpgradeTextColor(TowerUpgrade upgrade)
        {
            if (_selectedUpgrade == upgrade)
                return _successColor;

            if (!EconomyManager.CanAfford(upgrade.Cost))
                return _dangerColor;

            return _warningColor;
        }

        private string GetUpgradeStatusText(TowerUpgrade upgrade)
        {
            if (!EconomyManager.CanAfford(upgrade.Cost))
                return "INSUFFICIENT FUNDS";

            if (_selectedUpgrade == upgrade)
                return "SELECTED";

            return $"AVAILABLE (${upgrade.Cost})";
        }

        private Color GetUpgradeStatusTextColor(TowerUpgrade upgrade)
        {
            if (!EconomyManager.CanAfford(upgrade.Cost))
                return _dangerColor;

            if (_selectedUpgrade == upgrade)
                return _successColor;

            return _warningColor;
        }
    }
}
