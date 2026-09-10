// ====================================================================================================
//  FILE: UIStyleSheet.cs
//  PATH: ./Engine/UI/Styles/
//  MODULE: UI.Styles
//
//  ROLE:
//      Centralized deterministic style registry for all UI subsystems. Provides static, immutable
//      color palettes and visual constants for MainMenu, MapMenu, ModsMenu, HUD, and future UI
//      modules. Ensures consistent visual identity across the deterministic Option‑B UI pipeline.
//
// ====================================================================================================

namespace SASZombieAssaultTD.Engine.UI.Styles
{
    internal static class UIStyleSheet
    {
        // ====================================================================================================
        // MAIN MENU STYLE DEFINITIONS — FIXED & FULLY INITIALIZED
        // ====================================================================================================
        internal static class MainMenu
        {
            // Border + shadow colors
            public static readonly Color BorderColor = Color.Black;
            public static readonly Color ShadowColor = Color.FromArgb(20, 20, 20);

            // Button colors
            public static readonly Color ButtonBackground = Color.FromArgb(40, 40, 40);
            public static readonly Color ButtonHover = Color.FromArgb(60, 60, 60);
            public static readonly Color ButtonPressed = Color.FromArgb(80, 80, 80);
            public static readonly Color ButtonActive = Color.FromArgb(100, 100, 100);

            // Shadow offset
            public static readonly float ShadowOffset = 2f;

            // ------------------------------------------------------------------------------------------------
            // FIX: BackgroundStyle MUST be initialized or the UI layer draws a full‑screen black panel.
            // ------------------------------------------------------------------------------------------------
            public static readonly UIStyle BackgroundStyle = new UIStyle
            {
                BackgroundColor = Color.Transparent,   // Do NOT cover the scene
                BorderColor = Color.Transparent,
                ShadowColor = Color.Transparent,
                ShadowOffset = 0f
            };

            // Title style (safe defaults)
            public static readonly UIStyle TitleStyle = new UIStyle
            {
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent,
                ShadowColor = Color.Black,
                ShadowOffset = 2f
            };
        }

        // ====================================================================================================
        // MAP MENU STYLE DEFINITIONS (unchanged)
        // ====================================================================================================
        internal static class MapMenu
        {
            public static readonly Color MapEntryBackground = Color.FromArgb(40, 40, 40);
            public static readonly Color MapEntryBackgroundHover = Color.FromArgb(60, 60, 60);
            public static readonly Color MapEntryBackgroundPressed = Color.FromArgb(80, 80, 80);

            public static readonly Color BackgroundColor = Color.FromArgb(20, 20, 20);
            public static readonly Color Background = Color.FromArgb(20, 20, 20);
            public static readonly Color BackgroundHover = Color.FromArgb(40, 40, 40);
            public static readonly Color BackgroundPressed = Color.FromArgb(60, 60, 60);

            public static readonly Color Border = Color.FromArgb(80, 80, 80);
            public static readonly Color BorderHover = Color.FromArgb(100, 100, 100);
            public static readonly Color BorderPressed = Color.FromArgb(120, 120, 120);

            public static readonly Color Text = Color.FromArgb(160, 160, 160);
            public static readonly Color TextHover = Color.FromArgb(180, 180, 180);
            public static readonly Color TextPressed = Color.FromArgb(200, 200, 200);

            // Extended style fields (left uninitialized intentionally)
            public static Color BorderColor { get; internal set; }
            public static Color ButtonBackground { get; internal set; }
            public static Color ShadowColor { get; internal set; }
            public static Color ButtonHover { get; internal set; }
            public static Color ButtonPressed { get; internal set; }
            public static Color ButtonActive { get; internal set; }
            public static UIStyle BackgroundStyle { get; internal set; }
            public static UIStyle TitleStyle { get; internal set; }
            public static UIStyle MapEntryStyle { get; internal set; }
            public static object MapEntryLabelStyle { get; internal set; }
            public static Color MapEntryHover { get; internal set; }
        }
    }
}
