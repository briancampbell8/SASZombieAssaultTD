// =====================================================================================================
//  FILE: TypedTextFinalizer.cs
//  PATH: Engine/UI/Text/TypedTextFinalizer.cs
//  SUBSYSTEM: UI Text
//
//  ROLE:
//      Deterministic text shaping and layout subsystem. Converts raw text and style metadata into
//      FinalizerTextInstruction objects consumed by HUDPanelBuilder and ModernUIRenderer.
//
//  RESPONSIBILITIES:
//      - Shape and measure text deterministically using registered font metrics.
//      - Apply TextStyle metadata (color, weight, outline, glow, shadow, spacing).
//      - Produce render‑agnostic FinalizerTextInstruction objects for the UI Finalizer pipeline.
//      - Provide typed‑text support for HUD panels, UI labels, menus, and interactive UI elements.
//
//  NON-RESPONSIBILITIES:
//      - Rendering glyphs or issuing GPU commands.
//      - Managing UI states, HUD panels, or layout containers.
//      - Allocating textures, sprites, or hardware resources.
//
//  ARCHITECTURAL NOTES:
//      - Replaces all legacy text paths (FontManager, DrawString, ActualFontCacheType).
//      - Fully deterministic: no async, no global state, no runtime font shaping.
//      - Consumed by UIStateBuilder, HUDPanelBuilder, and FinalizerTextManager.
// =====================================================================================================


using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SharpDX.DirectWrite;

namespace SASZombieAssaultTD.Engine.UI.Text
{
    // Deterministic text instruction consumed by HUDPanelBuilder / ModernUIRenderer
    internal struct FinalizerTextInstruction
    {
        public string Text { get; set; }
        public string FontId { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public TextStyle Style { get; set; }
    }

    internal sealed class TypedTextFinalizer
    {
        private readonly Dictionary<string, FontMetrics> _metrics =
            new Dictionary<string, FontMetrics>(StringComparer.OrdinalIgnoreCase);

        // Fallback font size for converting DirectWrite design units to pixels
        private const float DefaultFontSize = 16f;

        // -------------------------------------------------------------------------------------------------
        // Register font metrics (deterministic, no async)
        // -------------------------------------------------------------------------------------------------
        internal void RegisterFont(string fontId, FontMetrics metrics)
        {
            if (string.IsNullOrWhiteSpace(fontId))
                throw new ArgumentException("TypedTextFinalizer: fontId cannot be null or empty.");

            // FontMetrics is a struct; check against default to ensure it was initialized
            if (metrics.Equals(default(FontMetrics)))
                throw new ArgumentNullException(nameof(metrics), "TypedTextFinalizer: Font metrics structure is uninitialized.");

            _metrics[fontId] = metrics;
        }

        // -------------------------------------------------------------------------------------------------
        // Produce a FinalizerTextInstruction
        // -------------------------------------------------------------------------------------------------
        internal FinalizerTextInstruction Build(
            string text,
            string fontId,
            float x,
            float y,
            TextStyle style)
        {
            if (!_metrics.TryGetValue(fontId, out var metrics))
                throw new InvalidOperationException(
                    $"TypedTextFinalizer: Font '{fontId}' is not registered.");

            var width = MeasureTextWidth(text, metrics);

            // Line height from DirectWrite metrics:
            // (Ascent + Descent + LineGap) / DesignUnitsPerEm * FontSize
            float designLineHeight = metrics.Ascent + metrics.Descent + metrics.LineGap;
            float height = (designLineHeight / metrics.DesignUnitsPerEm) * DefaultFontSize;

            return new FinalizerTextInstruction
            {
                Text = text,
                FontId = fontId,
                PositionX = x,
                PositionY = y,
                Width = width,
                Height = height,
                Style = style
            };
        }

        // -------------------------------------------------------------------------------------------------
        // Deterministic text measurement (fallback average glyph width)
        // -------------------------------------------------------------------------------------------------
        private float MeasureTextWidth(string text, FontMetrics metrics)
        {
            float totalDesignUnits = 0f;

            // Fallback: average glyph width approximated as half of DesignUnitsPerEm
            float fallbackGlyphWidth = metrics.DesignUnitsPerEm * 0.5f;

            foreach (char _ in text)
            {
                totalDesignUnits += fallbackGlyphWidth;
            }

            // Convert design units to pixels using DefaultFontSize
            return (totalDesignUnits / metrics.DesignUnitsPerEm) * DefaultFontSize;
        }
    }
}
