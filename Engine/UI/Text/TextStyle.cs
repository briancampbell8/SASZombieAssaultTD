// =====================================================================================================
//  FILE: TextStyle.cs
//  PATH: Engine/UI/Text/TextStyle.cs
//  SUBSYSTEM: UI Text
//
//  ROLE:
//      Deterministic text styling descriptor used by TypedTextFinalizer to produce
//      FinalizerTextInstruction objects. Encapsulates all render‑agnostic styling
//      metadata for UI text elements.
//
//  RESPONSIBILITIES:
//      - Hold deterministic color, weight, outline, glow, and shadow metadata.
//      - Provide typed‑text styling for HUD panels, UI labels, buttons, and menus.
//      - Serve as the authoritative Option‑B compliant style object for all UI text.
//
//  NON-RESPONSIBILITIES:
//      - Rendering glyphs or issuing GPU commands.
//      - Measuring text or shaping glyphs (TypedTextFinalizer handles that).
//      - Managing UI states, HUD panels, or layout containers.
//
//  ARCHITECTURAL NOTES:
//      - Consumed exclusively by TypedTextFinalizer.Build().
//      - Replaces all legacy text styling paths (FontManager, DrawString parameters).
//      - Fully deterministic: no async, no global state, no runtime mutation of metrics.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.UI.Text
{
    internal sealed class TextStyle
    {
        // -------------------------------------------------------------------------------------------------
        // Primary text color
        // -------------------------------------------------------------------------------------------------
        internal Color Color { get; set; } = Color.White;

        // -------------------------------------------------------------------------------------------------
        // Optional outline
        // -------------------------------------------------------------------------------------------------
        internal bool HasOutline { get; set; } = false;
        internal Color OutlineColor { get; set; } = Color.Black;
        internal float OutlineThickness { get; set; } = 1f;

        // -------------------------------------------------------------------------------------------------
        // Optional glow
        // -------------------------------------------------------------------------------------------------
        internal bool HasGlow { get; set; } = false;
        internal Color GlowColor { get; set; } = Color.White;
        internal float GlowRadius { get; set; } = 0f;

        // -------------------------------------------------------------------------------------------------
        // Optional shadow
        // -------------------------------------------------------------------------------------------------
        internal bool HasShadow { get; set; } = false;
        internal Color ShadowColor { get; set; } = Color.Black;
        internal float ShadowOffsetX { get; set; } = 0f;
        internal float ShadowOffsetY { get; set; } = 0f;

        // -------------------------------------------------------------------------------------------------
        // Weight / emphasis
        // -------------------------------------------------------------------------------------------------
        internal float Weight { get; set; } = 1f; // 1 = normal, >1 = bold

        // -------------------------------------------------------------------------------------------------
        // Opacity override
        // -------------------------------------------------------------------------------------------------
        internal float Opacity { get; set; } = 1f;

        // -------------------------------------------------------------------------------------------------
        // Letter spacing
        // -------------------------------------------------------------------------------------------------
        internal float LetterSpacing { get; set; } = 0f;

        // -------------------------------------------------------------------------------------------------
        // Diagnostics
        // -------------------------------------------------------------------------------------------------
        public override string ToString()
        {
            return $"TextStyle: Color={Color}, Weight={Weight}, Opacity={Opacity}";
        }
    }
}
