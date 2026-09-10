// =====================================================================================================
//  FILE: UpgradePanel-Layout.cs
//  PATH: Engine/UI/HUD/PanelsLegacy/UpgradePanel-Layout.cs
//  SUBSYSTEM: UI / HUD (Legacy Panel Partials)
//
//  ROLE:
//      Provides deterministic layout helpers for UpgradePanel.
//      This partial isolates all layout‑related behavior from rendering,
//      transitions, upgrade logic, and font/color management.
// =====================================================================================================

using System.Drawing;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.UI.HUD
{
    public partial class UpgradePanel
    {
        // ---------------------------------------------------------------------------------------------
        // Layout Constants
        // ---------------------------------------------------------------------------------------------

        private const float TitleOffsetX = 10f;
        private const float TitleOffsetY = 10f;

        private const float ListOffsetX = 10f;
        private const float ListOffsetY = 40f;
        private const float ListItemSpacing = 22f;

        private const float DetailsOffsetX = 10f;
        private const float DetailsOffsetYFromBottom = 120f;

        private const float StatusOffsetYFromBottom = 30f;

        private const float PurchaseButtonWidth = 100f;
        private const float PurchaseButtonHeight = 30f;
        private const float PurchaseButtonOffsetXFromRight = 110f;
        private const float PurchaseButtonOffsetYFromBottom = 40f;

        // ---------------------------------------------------------------------------------------------
        // Layout Helpers
        // ---------------------------------------------------------------------------------------------

        private Vector3 GetTitlePosition()
        {
            return new Vector3(
                _position.X + TitleOffsetX,
                _position.Y + TitleOffsetY,
                0f);
        }

        private Vector3 GetUpgradeListItemPosition(int index)
        {
            return new Vector3(
                _position.X + ListOffsetX,
                _position.Y + ListOffsetY + (index * ListItemSpacing),
                0f);
        }

        private Vector3 GetDetailsStartPosition()
        {
            return new Vector3(
                _position.X + DetailsOffsetX,
                _position.Y + _size.Y - DetailsOffsetYFromBottom,
                0f);
        }

        private Vector3 GetStatusPosition()
        {
            return new Vector3(
                _position.X + 10f,
                _position.Y + _size.Y - StatusOffsetYFromBottom,
                0f);
        }

        private RectangleF GetPurchaseButtonBounds()
        {
            float btnX = _position.X + _size.X - PurchaseButtonOffsetXFromRight;
            float btnY = _position.Y + _size.Y - PurchaseButtonOffsetYFromBottom;

            return new RectangleF(
                btnX,
                btnY,
                PurchaseButtonWidth,
                PurchaseButtonHeight);
        }

        // ---------------------------------------------------------------------------------------------
        // Hit Testing
        // ---------------------------------------------------------------------------------------------

        internal bool HitTestPurchaseButton(Vector3 mousePos)
        {
            return GetPurchaseButtonBounds().Contains(mousePos.X, mousePos.Y);
        }

        internal int HitTestUpgradeList(Vector3 mousePos)
        {
            float x = _position.X + ListOffsetX;
            float y = _position.Y + ListOffsetY;

            for (int i = 0; i < _availableUpgrades.Count; i++)
            {
                float itemY = y + (i * ListItemSpacing);

                RectangleF itemBounds = new RectangleF(
                    x,
                    itemY,
                    _size.X - 20f,
                    20f);

                if (itemBounds.Contains(mousePos.X, mousePos.Y))
                    return i;
            }

            return -1;
        }
    }
}
