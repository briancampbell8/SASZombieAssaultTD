// ====================================================================================================
//  FILE: UIMaterial.cs
//  PATH: ./Engine/UI/
//  MODULE: UI
//
//  ROLE:
//      Provide UI layout, interaction logic, or HUD rendering.
//
//  RESPONSIBILITIES:
//      - Provide LogState() behavior for the UI subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*==============================================================================
    File: UIMaterial.cs
    Project: SASZombieAssaultTD Engine
    Module: UI
    Author: Brian D. Campbell (BDC)
    Created: 2026-07-04
    Updated: 2026-07-04
    Purpose:
        Defines the modern UI material data container used by the HUDPanel and
        Finalizer rendering pipelines. This class provides deterministic color,
        shader, and flag metadata consumed by HUDManager, UIStateBuilder,
        HUDPanelFinalizer, and D3D11Adapter_Core.

    Notes:
        - Replaces legacy UI/Rendering version.
        - Must remain a pure data container (no rendering logic).
        - Fully synchronized with the HUDPanelFinalizer pipeline.
        - Uses Engine.Core.Color (NOT System.Drawing.Color).
        - Supports diagnostics via DLogger.Log().
==============================================================================*/

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.UI.Rendering.Modern;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI
{
    /// <summary>
    /// Represents a fully modernized UI material used by the HUDPanel + Finalizer pipeline.
    /// This class is a pure data container consumed by HUDManager, UIStateBuilder,
    /// HUDPanelFinalizer, and D3D11Adapter_Core.
    /// </summary>
    public class UIMaterial
    {
        internal object Texture;

        /// <summary>
        /// Primary fill color for UI elements (panels, bars, backgrounds).
        /// </summary>
        public Color FillColor { get; set; }

        /// <summary>
        /// Primary text color for labels, numbers, and HUD text.
        /// </summary>
        public Color TextColor { get; set; }

        /// <summary>
        /// Optional highlight or flash color (used by HUDPanel_CashUpdate and similar).
        /// </summary>
        public Color FlashColor { get; set; }

        /// <summary>
        /// Optional material flags used by the Finalizer pipeline (blend modes, flash modes, etc).
        /// </summary>
        public UIMaterialFlags Flags { get; set; }

        /// <summary>
        /// Optional shader/material ID for advanced rendering paths.
        /// </summary>
        public int ShaderId { get; set; }

        /// <summary>
        /// Optional name for diagnostics and DebugOverlay.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Optional numeric value (HUDPanel_CashUpdate uses this for cash delta).
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Creates a new UIMaterial with safe defaults.
        /// </summary>
        public UIMaterial()
        {
            FillColor = Color.White;
            TextColor = Color.White;
            FlashColor = Color.Transparent;

            Flags = UIMaterialFlags.None;
            ShaderId = 0;
            Name = string.Empty;
            Value = 0;
        }

        /// <summary>
        /// Logs material state for debugging.
        /// </summary>
        public void LogState(string tag = "UIMaterial")
        {
            DLogger.Log(tag, LogSubsystems.UI,
                $"Material '{Name}' | Fill={FillColor} Text={TextColor} Flash={FlashColor} Flags={Flags} Shader={ShaderId} Value={Value}");
        }

        public static explicit operator UIMaterial(UIAtlasMaterial v)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Flags used by UIMaterial for Finalizer rendering modes.
    /// </summary>
    public enum UIMaterialFlags
    {
        None = 0,
        Flash = 1 << 0,
        Highlight = 1 << 1,
        Pulsate = 1 << 2,
        Disabled = 1 << 3
    }
}

