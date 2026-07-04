//============================================================================
// File: UIState.cs
// Purpose: Holds resolved UI state values produced by HUDPanel_Finalizer.
// Role: Runtime UI state container consumed by UIFinalizer and UIRenderAdapter.
// Notes: Contains only resolved values. No logic. No rendering.
//============================================================================

using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI
{
    /// <summary>
    /// Runtime UI state container. Values originate from HUDPanel_Finalizer.
    /// Pure data model consumed by UIFinalizer and UIRenderAdapter.
    /// </summary>
    public class UIState
    {
        //------------------------------------------------------------------------
        // PANEL GEOMETRY (resolved by HUDPanel_Finalizer)
        //------------------------------------------------------------------------
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        //------------------------------------------------------------------------
        // CROSSHAIR PARAMETERS (resolved by HUDPanel_Finalizer)
        //------------------------------------------------------------------------
        public int CrosshairCenterX { get; set; }
        public int CrosshairCenterY { get; set; }
        public int CrosshairArmLength { get; set; }
        public int CrosshairThickness { get; set; }
        public Color CrosshairColor { get; set; } = Color.LightBlue;

        //------------------------------------------------------------------------
        // FINAL RESOLVED COLORS (resolved by HUDColorInputProcessor)
        //------------------------------------------------------------------------
        public Color FinalFillColor { get; set; } = Color.White;
        public Color FinalTextColor { get; set; } = Color.White;

        //------------------------------------------------------------------------
        // FLASH BEHAVIOR (resolved by HUDPanel_Finalizer)
        //------------------------------------------------------------------------
        public bool UseFlashColor { get; set; }
        public Color FlashColor { get; set; } = Color.Red;

        //------------------------------------------------------------------------
        // ELEMENT COLLECTION (FINALIZER → UISTATE BRIDGE)
        //------------------------------------------------------------------------
        /// <summary>
        /// Finalized UI elements ready for UIFinalizer → UIRenderAdapter.
        /// </summary>
        public List<UIRenderable> UIStateElements { get; set; } = new List<UIRenderable>();
    }
}
