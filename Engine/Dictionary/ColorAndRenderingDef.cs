// ====================================================================================================
//  FILE: ColorAndRenderingDef.cs
//  PATH: Engine/Dictionary/
//  MODULE: Core Rendering Definitions
//
//  ROLE:
//      Provides centralized definitions for colors, materials, blend modes, and rendering flags used
//      throughout the engine. This module replaces legacy placeholder content and removes duplicate
//      UIElement declarations.
//
//  RESPONSIBILITIES:
//      - Define deterministic color presets for UI and HUD systems.
//      - Provide rendering constants shared by ModernUIRenderer and HUD pipelines.
//      - Offer lightweight helpers for color manipulation.
//      - Maintain a stable dictionary-style module with no UI tree participation.
//
//  NON-RESPONSIBILITIES:
//      - Does NOT define UIElement or participate in UI hierarchy.
//      - Does NOT perform GPU rendering directly.
//      - Does NOT contain HUD logic or Finalizer pipeline code.
//
//  NOTES:
//      This file was repurposed from a legacy Gemini stub. All duplicate UIElement definitions have
//      been removed to unify the UI framework under Engine/UI/Components/UIElement.cs.
// ====================================================================================================

namespace SASZombieAssaultTD.Engine.Dictionary
{
    /// <summary>
    /// Common engine-wide color presets used by HUD, UI, and rendering systems.
    /// </summary>
    public static class EngineColors
    {
        public static readonly Color Transparent = Color.FromArgb(0, 0, 0, 0);
        public static readonly Color White = Color.FromArgb(255, 255, 255, 255);
        public static readonly Color Black = Color.FromArgb(255, 0, 0, 0);

        // HUD / UI Presets
        public static readonly Color HUDTitleBarBlue = Color.FromArgb(255, 33, 150, 243);
        public static readonly Color HUDTitleBarActive = Color.FromArgb(255, 48, 63, 159);
        public static readonly Color HUDWindowBG = Color.FromArgb(220, 16, 16, 16);
        public static readonly Color HUDText = Color.FromArgb(255, 255, 255, 255);
        public static readonly Color HUDHighlightCyan = Color.FromArgb(255, 0, 224, 255);

        // Rendering Presets
        public static readonly Color DebugRed = Color.FromArgb(255, 255, 64, 64);
        public static readonly Color DebugGreen = Color.FromArgb(255, 64, 255, 64);
        public static readonly Color DebugBlue = Color.FromArgb(255, 64, 64, 255);
    }

    /// <summary>
    /// Rendering flags and blend modes used by the GPU pipeline.
    /// </summary>
    public static class RenderFlags
    {
        public const int None = 0;
        public const int AlphaBlend = 1 << 0;
        public const int Additive = 1 << 1;
        public const int Multiply = 1 << 2;
        public const int DebugWire = 1 << 3;
    }

    /// <summary>
    /// Lightweight color manipulation helpers.
    /// </summary>
    public static class ColorUtils
    {
        public static Color WithAlpha(Color c, int alpha)
        {
            return Color.FromArgb(alpha, c.R, c.G, c.B);
        }

        public static Color Tint(Color c, float factor)
        {
            int r = (int)(c.R * factor);
            int g = (int)(c.G * factor);
            int b = (int)(c.B * factor);
            return Color.FromArgb(c.A, r, g, b);
        }
    }
}
