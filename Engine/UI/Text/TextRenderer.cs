// =====================================================================================================
//  FILE: TextRenderer.cs
//  PATH: Engine/UI/Text/TextRenderer.cs
//  SUBSYSTEM: UI Text
//
//  ROLE:
//      Provides deterministic text measurement and basic text rendering hooks for the engine’s UI layer.
//      This subsystem is responsible for exposing a clean API for drawing text and querying its size,
//      independent of the underlying font or rendering backend.
//
//  RESPONSIBILITIES:
//      - Measure text dimensions for layout (width, height).
//      - Expose a DrawText entry point for UI and game code.
//      - Remain deterministic and side‑effect free in measurement logic.
//      - Act as a bridge between high‑level UI code and the active render context.
//
//  NON-RESPONSIBILITIES:
//      - Managing framebuffer memory or GPU resources directly.
//      - Implementing complex font loading, kerning, or glyph rasterization pipelines.
//      - Handling scene composition, ECS orchestration, or game state management.
//
//  ARCHITECTURAL NOTES:
//      - Intended to be used by render contexts (e.g., SoftwareRenderContext) to implement D3D11Adapter_Core
//        text methods.
//      - The actual font and glyph rendering backend can be swapped or extended without changing callers.
//      - Measurement logic is kept simple and deterministic; it can be replaced by a richer font system
//        when available.
// =====================================================================================================

using System;
using System.Numerics;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;

namespace SASZombieAssaultTD.Engine.UI.Text
{
    /// <summary>
    /// Minimal deterministic text rendering and measurement subsystem.
    /// </summary>
    internal sealed class TextRenderer
    {
        private readonly D3D11Adapter_Core _adapter_Core;

        /// <summary>
        /// Creates a new TextRenderer bound to a specific render context.
        /// </summary>
        /// <param name="renderContext">The render context used for drawing operations.</param>
        public TextRenderer(D3D11Adapter_Core adapter_Core)
        {
            _adapter_Core = adapter_Core ?? throw new ArgumentNullException(nameof(adapter_Core));
        }

        /// <summary>
        /// Draws text at the given position using the specified size and color.
        /// This implementation is intentionally minimal and can be replaced by a richer font system.
        /// </summary>
        /// <param name="text">The text to draw.</param>
        /// <param name="position">The position in screen space.</param>
        /// <param name="size">The nominal font size.</param>
        /// <param name="color">The text color.</param>
        public void DrawText(string text, Vector2 position, float size, ColorRGBA color)
        {
            if (string.IsNullOrEmpty(text))
                return;

            // For now, delegate to the render context’s text API if available.
            // This keeps TextRenderer as a thin abstraction layer.
            _adapter_Core.DrawText(text, position, size, color);
        }

        /// <summary>
        /// Measures the approximate size of the given text at the specified font size.
        /// This is a deterministic approximation and can be replaced by a font-aware implementation.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <param name="size">The nominal font size.</param>
        /// <returns>Approximate width and height of the rendered text.</returns>
        public Vector2 MeasureText(string text, float size)
        {
            if (string.IsNullOrEmpty(text) || size <= 0f)
                return Vector2.Zero;

            // Simple deterministic approximation:
            // - Width: character count * size * constant factor
            // - Height: size
            // This can be replaced by a real font metrics system later.
            const float widthFactor = 0.5f;
            float width = text.Length * size * widthFactor;
            float height = size;

            return new Vector2(width, height);
        }
    }
}
