// =====================================================================================================
//  FILE: UpgradePanel-Rendering.cs
//  PATH: Engine/UI/HUD/PanelsLegacy/UpgradePanel-Rendering.cs
//  SUBSYSTEM: UI / HUD (Legacy Panel Partials)
//
//  ROLE:
//      Provides deterministic rendering behavior for UpgradePanel.
//      This partial isolates all draw‑calls from layout, logic, transitions,
//      and font/color management.
// =====================================================================================================

using System.Drawing;
using SASZombieAssaultTD.Engine.Render;
using SASZombieAssaultTD.Engine.VectorMath;

// Fixed Namespace to align perfectly with the driver file UpgradePanel.cs
namespace SASZombieAssaultTD.Engine.UI.HUD
{
    // Fixed access modifier from 'internal' to 'public' to match driver declaration
    public partial class UpgradePanel
    {
        // ---------------------------------------------------------------------------------------------
        // Main Render Entry
        // ---------------------------------------------------------------------------------------------

        public override void Render()
        {
            if (!_isVisible)
                return;

            RenderBackground();
            RenderTitle();
            RenderUpgradeList();
            RenderUpgradeDetails();
            RenderUpgradeStatus();
            RenderPurchaseButton();
        }

        // ---------------------------------------------------------------------------------------------
        // Background Rendering
        // ---------------------------------------------------------------------------------------------

        private void RenderBackground()
        {
            float alpha = 255f * TransitionProgress;

            Color bg = Color.FromArgb((int)alpha, _backgroundColor.R, _backgroundColor.G, _backgroundColor.B);
            Color border = Color.FromArgb((int)alpha, _borderColor.R, _borderColor.G, _borderColor.B);

            Renderer.DrawRectangle(_position.X, _position.Y, _size.X, _size.Y, bg);
            Renderer.DrawRectangle(_position.X, _position.Y, _size.X, _size.Y, border, 2f);
        }

        // ---------------------------------------------------------------------------------------------
        // Title Rendering
        // ---------------------------------------------------------------------------------------------

        private void RenderTitle()
        {
            Renderer.DrawString(
                "UPGRADES",
                GetTitlePosition(),
                _normalColor,
                _titleFont);
        }

        // ---------------------------------------------------------------------------------------------
        // Upgrade List Rendering
        // ---------------------------------------------------------------------------------------------

        private void RenderUpgradeList()
        {
            for (int i = 0; i < _availableUpgrades.Count; i++)
            {
                var upgrade = _availableUpgrades[i];
                Color color = GetUpgradeTextColor(upgrade);

                Renderer.DrawString(
                    $"{upgrade.Name} (${upgrade.Cost})",
                    GetUpgradeListItemPosition(i),
                    color,
                    _textFont);
            }
        }

        // ---------------------------------------------------------------------------------------------
        // Upgrade Details Rendering
        // ---------------------------------------------------------------------------------------------

        private void RenderUpgradeDetails()
        {
            if (_selectedUpgrade == null)
                return;

            Vector3 pos = GetDetailsStartPosition();

            Renderer.DrawString($"Level: {_selectedUpgrade.Level}", pos, _normalColor, _textFont);
            pos = new Vector3(pos.X, pos.Y + 20f, pos.Z); // Fixed CS0200 vector assignment

            Renderer.DrawString($"Damage +{_selectedUpgrade.DamageIncrease}", pos, _normalColor, _textFont);
            pos = new Vector3(pos.X, pos.Y + 20f, pos.Z); // Fixed CS0200 vector assignment

            Renderer.DrawString($"Range +{_selectedUpgrade.RangeIncrease}", pos, _normalColor, _textFont);
            pos = new Vector3(pos.X, pos.Y + 20f, pos.Z); // Fixed CS0200 vector assignment

            Renderer.DrawString($"Fire Rate +{_selectedUpgrade.FireRateIncrease}", pos, _normalColor, _textFont);
            pos = new Vector3(pos.X, pos.Y + 20f, pos.Z); // Fixed CS0200 vector assignment

            Renderer.DrawString($"Speed +{_selectedUpgrade.SpeedIncrease}", pos, _normalColor, _textFont);
        }

        // ---------------------------------------------------------------------------------------------
        // Upgrade Status Rendering
        // ---------------------------------------------------------------------------------------------

        private void RenderUpgradeStatus()
        {
            if (_selectedUpgrade == null)
                return;

            string status = GetUpgradeStatusText(_selectedUpgrade);
            Color color = GetUpgradeStatusTextColor(_selectedUpgrade);

            Renderer.DrawString(
                status,
                GetStatusPosition(),
                color,
                _textFont);
        }

        // ---------------------------------------------------------------------------------------------
        // Purchase Button Rendering
        // ---------------------------------------------------------------------------------------------

        private void RenderPurchaseButton()
        {
            RectangleF bounds = GetPurchaseButtonBounds();
            _purchaseButtonBounds = bounds;

            Color bg = _canAffordUpgrade ? _successColor : _dangerColor;

            Renderer.DrawRectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height, bg);
            Renderer.DrawRectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height, Color.Black, 2f);

            Renderer.DrawString(
                "PURCHASE",
                new Vector3(bounds.X + 10f, bounds.Y + 8f, 0),
                Color.White,
                _textFont);
        }

        // ---------------------------------------------------------------------------------------------
        // Transitional Layout Query Shims (Redirects to other partial modules safely)
        // ---------------------------------------------------------------------------------------------
        // 🛑 ALL REPLICATED COORDINATE AND SHAPING METHODS REMOVED TO PREVENT CS0111 CLASHES WITH UpgradePanel-Layout.cs

        private float TransitionProgress => 1.0f;
    }
}
