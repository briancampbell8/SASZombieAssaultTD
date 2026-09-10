// =====================================================================================================
//  FILE: StringExtensions.cs
//  PATH: Engine/UI/Text/StringExtensions.cs
//  MODULE: UI / Text
//
//  ROLE:
//      Provide deterministic, type‑safe helper methods for string measurement and text sizing used by
//      the engine’s UI, HUD, and debug rendering subsystems.
//
//  RESPONSIBILITIES:
//      - Provide MeasureString() behavior using a temporary GDI+ context.
//      - Provide Width() and Height() helpers using default font fallbacks.
//      - Provide object‑based MeasureString() overloads for UI components.
//      - Remain pure, deterministic, and side‑effect free.
//
//  NON‑RESPONSIBILITIES:
//      - Performing GPU text rendering (handled by TextRenderer subsystem).
//      - Managing font resources or caching strategies.
//      - Providing grid‑debug helpers (relocated to GridDebugExtensions.cs).
//
//  ARCHITECTURAL NOTES:
//      - Relocated from Engine/Extensions during subsystem cleanup.
//      - All grid‑debug helpers have been removed.
//      - This module must not introduce cross‑subsystem coupling.
// =====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Drawing;

namespace SASZombieAssaultTD.Engine.UI.Text
{
    /// <summary>
    /// Deterministic helper extensions for string measurement and text sizing.
    /// </summary>
    public static class StringExtensions
    {
        public static object graphics { get; private set; }

        public static void FromImage(Bitmap bitmap) { }
        /// <summary>
        /// Measures a string for rendering purposes using a temporary GDI+ graphics context.
        /// </summary>
        public static SizeF MeasureString(this string text, Font font)
        {
            if (string.IsNullOrEmpty(text) || font == null)
                return SizeF.Empty;

            using (var bitmap = new Bitmap(1, 1))

            {
                return graphics.MeasureString(text, font);
            }
        }


        /// <summary>
        /// Measures an object representation of a string for rendering purposes.
        /// </summary>
        public static SizeF MeasureString(this object target, string text, Font font)
        {
            ArgumentNullException.ThrowIfNull(target);
            ArgumentNullException.ThrowIfNull(font);
            ArgumentException.ThrowIfNullOrEmpty(text);

            return MeasureString(text, font);
        }

        /// <summary>
        /// Gets the width of a string using a standard default font fallback.
        /// </summary>
        public static float Width(this string text)
        {
            using (var defaultFont = new Font("Arial", 12))
            {
                return MeasureString(text, defaultFont).Width;
            }
        }

        /// <summary>
        /// Gets the height of a string using a standard default font fallback.
        /// </summary>
        public static float Height(this string text)
        {
            using (var defaultFont = new Font("Arial", 12))
            {
                return MeasureString(text, defaultFont).Height;
            }
        }
    }
}
