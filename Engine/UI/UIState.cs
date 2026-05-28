/*
// File: UIState.cs
// Purpose: Holds resolved UI state values produced by HUDPanel_Finalizer.
// Role: Runtime UI state container consumed by UIFinalizer and UIRenderAdapter.
// Notes: Contains only resolved values. No logic. No rendering.
using SASZombieAssaultTD.Engine.Diagnostics;

*/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace SASZombieAssaultTD.Engine.UI
{
    /// <summary>
    /// Runtime UI state container. Values originate from HUDPanel_Finalizer.
    /// </summary>
    public class UIState
    {
        internal IEnumerable<object> Elements;

        // Panel geometry resolved by Finalizer
        public int X { get; set; }

        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        // Crosshair parameters resolved by Finalizer
        public int CrosshairCenterX { get; set; }

        public int CrosshairCenterY { get; set; }
        public int CrosshairArmLength { get; set; }
        public int CrosshairThickness { get; set; }
        public Color CrosshairColor { get; set; } = Color.LightBlue;

        // Final resolved colors for UI elements
        public Color FinalFillColor { get; set; } = Color.White;

        public Color FinalTextColor { get; set; } = Color.White;

        // Flash behavior resolved by Finalizer
        public bool UseFlashColor { get; set; }

        public Color FlashColor { get; set; } = Color.Red;

        // Quota exceeded. Please try again later. //
        // ---------------------------------------------------------
        // ELEMENT COLLECTION (FINALIZER → UISTATE BRIDGE)
        // ---------------------------------------------------------
        public List<UIRenderable> UIStateElements { get; set; } = new List<UIRenderable>();
    }
}
